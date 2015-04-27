using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FreeSun.FS_SMISCloud.Server.DataCalc.Communication;
using log4net;
using System.Reflection;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry;
using FreeSun.FS_SMISCloud.Server.DataCalc.Arithmetic;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Calculation
{
    /// <summary>
    /// 数据处理类
    /// </summary>
    class DataProcessor
    {
        private ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        /// <summary>
        /// 结构物编号
        /// </summary>
        public int StructId { get; set; }
        /// <summary>
        /// 数据采集时间
        /// </summary>
        public DateTime CollectTime { get; set; }
        /// <summary>
        /// 传感器列表
        /// </summary>
        public static Dictionary<int, Sensor> SensorList = new Dictionary<int, Sensor>();
        /// <summary>
        /// 算法列表
        /// </summary>
        public static List<IArithmetic> Arithmetics = new List<IArithmetic>();

        /// <summary>
        /// 获取所有传感器的配置信息
        /// </summary>
        public void ReadConfig()
        {
            // 初始化传感器配置信息
            SensorList.Clear();
            DataTable table = DataAccessHelper.GetAllSensorConfig();
            foreach (DataRow dr in table.Rows)
            {
                int sensorId = Convert.ToInt32(dr["SENSOR_ID"]);
                Sensor s = CreatSensor(dr);
                SensorList.Add(sensorId, s);
            }
            _logger.Info("获取传感器配置" + SensorList.Count + "条");
            // 获取传感器分组信息
            Arithmetics.Clear();
            Arithmetics.Add(CxGroupArithmetic.GetGroupArithmetic());
            //Arithmetics.Add(DryBeachArithmetic.GetDryBeachArithmetic());
        }

        /// <summary>
        /// 根据传感器协议码号生产传感器实例
        /// </summary>
        /// <param name="dr">配置表项</param>
        /// <returns></returns>
        private Sensor CreatSensor(DataRow dr)
        {
            int protocolType = Convert.ToInt32(dr["PROTOCOL_CODE"]);
            string obj = ProtocolFactory.getInstance().GetSensorType(protocolType);
            try
            {
                object[] para = new object[1];
                para[0] = dr;
                object r = Assembly.GetExecutingAssembly().CreateInstance(obj, true, BindingFlags.Default, null, para/*args*/, null, null);
                return (Sensor)r;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("传感器配置信息读取有误! SensorId={0} ", dr["SENSOR_ID"].ToString()) + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 处理计算请求
        /// </summary>
        /// <param name="source">原始数据表</param>
        /// <param name="calcCompletion">计算完成通知方法</param>
        public int ProcessRequest(int dtuid, DataTable source, Action<IEnumerable<string>> calcCompletion)
        {
            _logger.Info(string.Format("获取原始数据{0}条.", source.Rows.Count));
            var calcDatum = new Dictionary<int, Data>();
            int sensorId = 0;
            Data data = null;
            var allsens = new List<int>();
            int redundancySensorCnt = 0;
            var redsens = new List<int>();
            // 物理量计算
            foreach (DataRow dr in source.Rows)
            {
                try
                {
                    sensorId = dr.Field<int>("SensorId");
                    if (allsens.Contains(sensorId))
                    {
                        redundancySensorCnt++;//存在冗余数据
                        if (!redsens.Contains(sensorId)) redsens.Add(sensorId);
                        continue;
                    }
                    allsens.Add(sensorId);
                    if (!SensorList.ContainsKey(sensorId) || SensorList[sensorId] == null)
                    {
                        throw new Exception(string.Format("未找到传感器对应配置信息"));
                    }
                    
                    if (SensorList[sensorId].DTUId != dtuid)
                    {
                        _logger.Info(string.Format("传感器{0}不归属在当前计算DTU{1}下", sensorId, dtuid));
                        continue;
                    }

                    data = SensorList[sensorId].CalcValue(dr);

                    if (data != null)
                        calcDatum.Add(sensorId, data);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("计算物理量 传感器ID: {0}, Time:{1} , {2}", sensorId, data != null ? data.CollectTime.ToString() : "", ex.Message));
                }
            }
            if (redundancySensorCnt != 0)
            {
                _logger.WarnFormat("存在冗余数据{0}项  包含传感器ID{1}", redundancySensorCnt, string.Join(",", redsens));
            }

            _logger.Info(string.Format("计算传感器数据{0}条.", calcDatum.Count));
            // 组合计算 (分组/干滩)
            foreach (var art in Arithmetics)
            {
                art.Calculate(calcDatum);
            }
            // 插入数据库
            InsertDataBase(calcDatum.Values.ToList());

            return calcDatum.Count;// 返回计算的条数
        }

        /// <summary>
        /// 数据插入
        /// </summary>
        public void InsertDataBase(IList<Data> datum)
        {
            var groups = datum.AsEnumerable().GroupBy(d => d.Safetyfactor);
            foreach (IGrouping<SAFE_FACT, Data> ig in groups)
            {
                try
                {
                    Data.InsertData(ig.Key, ig.AsEnumerable());
                    _logger.Info(string.Format("插入数据 监测因素{0},数据个数{1}", ig.Key, ig.Count()));
                }
                catch (System.Exception ex)
                {
                    _logger.Error(string.Format("插入数据 监测因素{0},{1}", ig.Key, ex.Message));
                }
            }
        }

        #region 振动数据
        /// <summary>
        /// 处理振动数据计算请求
        /// </summary>
        /// <param name="filename">原始数据文件路径</param>
        /// <param name="sensorId">传感器ID</param>
        /// <param name="acqTime">数据采集时间</param>
        /// <param name="calcCompletion">计算完成通知方法</param>
        public void ProcessRequest(string filename, int sensorId, DateTime acqTime, Action<IEnumerable<string>> calcCompletion)
        {
            if (string.IsNullOrEmpty(filename)) return;
            try
            {
                ACCSensor.CalcAndSave(sensorId, acqTime, filename);
                _logger.Info(string.Format("处理数据 监测因素:振动 传感器ID:{0}, 采集时间:{1}, 数据文件:{2}", sensorId, acqTime, filename));
            }
            catch (System.Exception ex)
            {
                _logger.Error(string.Format("处理振动数据计算请求时发生异常 SensorID:{0}, AcqTime:{1}, File:{2}, {3}", sensorId, acqTime, filename, ex.Message));
            }
        }
        #endregion
    }
}
