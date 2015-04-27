// --------------------------------------------------------------------------------------------
// <copyright file="CxGroupArithmetic.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：测斜管组累积位移算法
// 
// 创建标识：刘歆毅20140219
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Arithmetic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
    using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
    using log4net;

    public class CxGroupArithmetic : IArithmetic
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());

        private static CxGroupArithmetic groupArithmetic;
        private static readonly object SyncObject = new object();
        public static CxGroupArithmetic GetGroupArithmetic()
        {
            if (groupArithmetic == null)
            {
                lock (SyncObject)
                {
                    if (groupArithmetic == null)
                    {
                        return groupArithmetic = new CxGroupArithmetic();
                    }
                }
            }
            return groupArithmetic;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        private CxGroupArithmetic()
        {
            Initial();
        }

        /// <summary>
        /// 测斜分组计算
        /// </summary>
        public void Calculate(Dictionary<int, Data> rawData)
        {
            foreach (int gid in GroupSenId.Keys)
            {
                var senidSg = GroupSenId[gid];
                foreach (var senid in senidSg)
                {
                    double xdisplacement = 0;
                    double ydisplacement = 0;
                    if (!rawData.ContainsKey(senid)) continue;

                    if (!(rawData[senid] is DataDeepDisplacement)) continue;
                    foreach (int sid in senidSg)
                    {
                        try
                        {
                            if (SensorDeep[senid] >= SensorDeep[sid])
                            {
                                if (!rawData.ContainsKey(sid))
                                {
                                    continue;
                                }
                                xdisplacement += ((DataDeepDisplacement)rawData[sid]).XDisplacement;
                                ydisplacement += ((DataDeepDisplacement)rawData[sid]).YDisplacement;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("SurveyCalcuByGroup sensorGroupsCalculate :{0}", ex.Message);
                        }
                    }
                    ((DataDeepDisplacement)rawData[senid]).XAccumulate = xdisplacement;
                    ((DataDeepDisplacement)rawData[senid]).YAccumulate = ydisplacement;
                }
            }
        }

        public bool Initial()
        {
            logger.Info("初始化信息：组别");
            DataTable sensorgroup = DataAccessHelper.SelectSurveyGroup();
            if (sensorgroup.Rows.Count > 0)
            {
                var groups = sensorgroup.DefaultView.ToTable("groups", true, "GROUP_ID");
                foreach (DataRow g in groups.Rows)
                {
                    var gid = Convert.ToInt32(g["GROUP_ID"].ToString());
                    var expression = string.Format("GROUP_ID={0}", gid);
                    DataRow[] sensors = sensorgroup.Select(expression);
                    var gsid = new List<int>();
                    foreach (var dataRow in sensors)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(dataRow["SENSOR_ID"].ToString().Trim()) &&
                                !string.IsNullOrEmpty(dataRow["DEPTH"].ToString().Trim()))
                            {
                                var sensorid = Convert.ToInt32(dataRow["SENSOR_ID"]);
                                var deep = Convert.ToSingle(dataRow["DEPTH"]);
                                this._sensordeep.Add(sensorid, deep);
                                gsid.Add(sensorid);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.WarnFormat("读取测斜分组配置时(GROUPID{1}) {0}", ex.Message, gid);
                        }
                    }

                    this._groupSenId.Add(gid,gsid.ToArray());
                }
            }
            logger.Info("初始化完成");
            return true;
        }

        #region Properties
        private Dictionary<int, int[]> _groupSenId = new Dictionary<int, int[]>();

        /// <summary>
        /// 传感器分组字典
        /// </summary>
        public Dictionary<int, int[]> GroupSenId
        {
            get { return this._groupSenId; }
        }

        private Dictionary<int, float> _sensordeep = new Dictionary<int, float>();

        /// <summary>
        /// 传感器深度
        /// </summary>
        public Dictionary<int, float> SensorDeep
        {
            get { return this._sensordeep; }
        }
        #endregion

        /// <summary>
        /// 实现计算算法接口
        /// </summary>
        /// <param name="rawData">要计算的原始数据</param>
        /// <returns>结果数据</returns>
        /*
         * public IList<Data> Calculate(IList<Data> rawData)
        {
            IList<Data> rslt = new List<Data>();
            // 获取分组配置
            DataTable config = DataAccessHelper.GetGroupConfig(rawData.Select(d => d.SensorId).ToArray());
            // 按组将数据分开
            var joinQuery = from raw in rawData
                from cfg in config.AsEnumerable()
                where raw.SensorId == cfg.Field<int>("SensorId")
                select new
                {
                    GroupId = cfg.Field<int>("GroupId"),
                    GroupArg = cfg.Field<double>("GroupArg"),
                    SensorId = raw.SensorId,
                    DataSet = raw.DataSet,
                    CollectTime = raw.CollectTime
                };

            var groupData = (from d in joinQuery
                group d by d.GroupId
                into g
                select new
                {
                    g.Key,
                    g
                }).ToDictionary(k => k.Key, k => k.g);

            // 按组进行计算
            foreach (var gd in groupData)
            {
                var list = gd.Value.ToDictionary(k => k.GroupArg, k => k);// 按深度构成键值对
                var depth = list.Keys.ToList(); // 深度列表
                depth.Sort();// 深度从小到达排序
                // 计算累积
                foreach (var d in depth)
                {
                    Data data = new Data
                    {
                        SensorId = list[d].SensorId,
                        CollectTime = list[d].CollectTime,
                        DataSet = new List<double>()
                    };
                    var raw = list.Values.Where(v => v.GroupArg <= d).Select(v => v.DataSet);// 深度小于自身的数据
                    // 求和
                    for (int i = 0; i < rawData[0].DataSet.Count; i++)
                    {
                        data.DataSet.Add(raw.Sum(v => v[i]));
                    }
                    rslt.Add(data);
                }
            }
            return rslt;
        }
         * */
    }
}