using System.Collections.Generic;
using System.Data;
using System.Reflection;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using log4net;
using System.Linq;
using System;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Arithmetic
{
    /// <summary>
    ///  干滩长度算法
    /// </summary>
    public class DryBeachArithmetic : IArithmetic
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());

        private static DryBeachArithmetic drybeachArithmetic;
        private static readonly object SyncObject = new object();
        public static DryBeachArithmetic GetDryBeachArithmetic()
        {
            if (drybeachArithmetic == null)
            {
                lock (SyncObject)
                {
                    if (drybeachArithmetic == null)
                    {
                        return drybeachArithmetic = new DryBeachArithmetic();
                    }
                }
            }
            return drybeachArithmetic;
        }

        private DryBeachArithmetic()
        {
            Initial();
        }

        /// <summary>
        /// 初始化配置参数
        /// </summary>
        /// <returns></returns>
        public bool Initial()
        {
            DryBeachConfigs.Clear();
            var table = DataAccessHelper.GetSensorConfig("[SAFETY_FACTOR_TYPE_ID] = 35");   // 获取所有干滩监测传感器配置信息
            var query = table.AsEnumerable().GroupBy(t => t.Field<int>("STRUCT_ID"));
            foreach (IGrouping<int,DataRow> ig in query)
            {
                var cfg = new DryBeachConfig();
                int structId = ig.Key;
                try
                {
                    foreach (DataRow dr in ig.AsEnumerable())  // 每个结构物的干滩配置
                    {
                        bool isleida = false;
#if ANXINYUN2
                        isleida=Convert.ToInt32(dr["PRODUCT_TYPE_ID"])==20311;
#else
                        isleida=Convert.ToInt32(dr["PRODUCT_TYPE_KEY"])==16;
#endif
                        if (isleida) // 雷达物位计
                        {
                            cfg.CalcMethod = 2;
                            if (cfg.GTGId1 == -1)
                            {
                                cfg.GTGId1 = Convert.ToInt32(dr["SENSOR_ID"]);
                                cfg.GTGHeight1 = Convert.ToDouble(dr["Parameter1"]);
                                cfg.GTGHorizon1 = Convert.ToDouble(dr["Parameter2"]);
                            }
                            else
                            {
                                cfg.GTGId2 = Convert.ToInt32(dr["SENSOR_ID"]);
                                cfg.GTGHeight2 = Convert.ToDouble(dr["Parameter1"]);
                                cfg.GTGHorizon2 = Convert.ToDouble(dr["Parameter2"]);
                            }
                        }
                        else    // 水位计
                        {
                            cfg.WaterSensorID = Convert.ToInt32(dr["SENSOR_ID"]);
                            cfg.BeachTopHeight = Convert.ToDouble(dr["Parameter1"]);        // 滩顶高程
                            cfg.WaterInstallHeight = Convert.ToDouble(dr["Parameter2"]);    // 水位计安装高程
                            cfg.SlopeRatio = Convert.ToDouble(dr["Parameter5"]);            // 坡比
                        }
                    }
                    cfg.EnsureCheck();
                    DryBeachConfigs.Add(structId, cfg);
                }
                catch (System.Exception ex)
                {
                    logger.Error(string.Format("干滩计算，结构物ID:{0},{1}", structId, ex.Message));
                }
            }
            return true;
        }

        /// <summary>
        /// 干滩长度计算(计算结果放在水位计数据中)
        /// </summary>
        public void Calculate(Dictionary<int, Data> rawData)
        {
            foreach (var beach in DryBeachConfigs.Values)
            {
                if (!rawData.ContainsKey(beach.WaterSensorID)) continue;
                double waterlevel = ((DataBeachLen)rawData[beach.WaterSensorID]).waterlevel + beach.WaterInstallHeight;
                double slope_ratio = beach.SlopeRatio;
                if (beach.CalcMethod == 2)  // 雷达物位计算坡比
                {
                    if (!rawData.ContainsKey(beach.GTGId1) || !rawData.ContainsKey(beach.GTGId2)) continue;
                    double p1 = beach.GTGHeight1 - rawData[beach.GTGId1].DataSet[0];
                    double p2 = beach.GTGHeight2 - rawData[beach.GTGId2].DataSet[0];
                    slope_ratio = Math.Abs((p1 - p2) / (beach.GTGHorizon1 - beach.GTGHorizon2));
                }
                double beachlen = (beach.BeachTopHeight - waterlevel) / slope_ratio;
                ((DataBeachLen)rawData[beach.WaterSensorID]).beachlen = beachlen;
            }
        }

        private Dictionary<int, DryBeachConfig> DryBeachConfigs = new Dictionary<int, DryBeachConfig>();
    }

    /// <summary>
    /// 干滩监测配置参数
    /// </summary>
    public class DryBeachConfig
    {
        public short CalcMethod;                    // 计算方法(0-视频标杆 1-水位计固定坡比 2-水位计雷达物位计)

        public int WaterSensorID;                   // 水位计ID
        public double WaterInstallHeight;           // 水位计安装高程
        public double BeachTopHeight;               // 滩顶高程
        public double SlopeRatio;                   // 干滩坡比

        public int GTGId1;                          // 雷达物位计1
        public double GTGHeight1;                   // 雷达物位计1高程
        public double GTGHorizon1;                  // 雷达物位计1水平距离
        public int GTGId2;                          // 雷达物位计2
        public double GTGHeight2;                   // 雷达物位计2高程
        public double GTGHorizon2;                  // 雷达物位计2水平距离

        public DryBeachConfig()
        {
            CalcMethod = 1;
            WaterSensorID = -1;
            GTGId1 = -1;
            GTGId2 = -1;
        }

        public void EnsureCheck()
        {
            if (WaterSensorID == -1)
                throw new Exception("配置项中缺少水位计传感器参数");
            if (CalcMethod == 2 && (GTGId1 == -1 || GTGId2 == -1))
                throw new Exception("雷达物位计参数不全");
            if (CalcMethod == 2 && Math.Abs(GTGHorizon1 - GTGHorizon2) < 0.0001)
                throw new Exception("雷达物位计水平距离不能设置成相同值");
            if (CalcMethod == 1 && Math.Abs(SlopeRatio) < 0.0001)
                throw new Exception("干滩水位计的坡比参数不能为0");
        }
    }
}
