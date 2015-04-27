using System.Data.SqlClient;
// --------------------------------------------------------------------------------------------
// <copyright file="DataAccessHelper.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：数据访问类
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

namespace FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess
{
    using System;
    using System.Data;
    using System.Configuration;
    using FakeData;
    using SMIS.Utils.DB;

    public static class DataAccessHelper
    {
        public static string loadDBConnName;

        static DataAccessHelper()
        {
            //             var file = new ExeConfigurationFileMap();
            //             file.ExeConfigFilename = "DataCalc.exe.config";
            //             Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            //             loadDBConnName = config.AppSettings.Settings["ConnectionString"].Value;
            loadDBConnName = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }

        #region 数据操作方法
        /// <summary>
        /// 查询ET清洗后的数据
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <param name="roundNum">采集轮数</param>
        /// <returns>数据</returns>
        public static DataTable GetETData(int structId, DateTime time)
        {
            return FakeDataSource.GetETData(structId, time);
        }

        /// <summary>
        /// 查询算法配置
        /// </summary>
        /// <param name="sensors">传感器数组</param>
        /// <returns>算法配置</returns>
        public static DataTable GetArithmeticConfig(int[] sensors)
        {
            return FakeDataSource.GetArithmeticConfig(sensors);
        }

        /// <summary>
        /// 查询分组配置
        /// </summary>
        /// <param name="sensors"></param>
        /// <returns></returns>
        public static DataTable GetGroupConfig(int[] sensors)
        {
            return FakeDataSource.GetGroupConfig(sensors);
        }

        /// <summary>
        /// 查询某传感器一段时间的ET清洗后数据
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <param name="from">开始时间</param>
        /// <param name="to">结束时间</param>
        /// <returns>数据</returns>
        public static DataTable GetETData(int sensorId, DateTime from, DateTime to)
        {
            return FakeDataSource.GetETData(sensorId, from, to);
        }

        /// <summary>
        /// 查询因异常遗漏未计算的数据
        /// </summary>
        /// <returns>遗漏的数据</returns>
        public static DataTable GetOmitToCalcData()
        {
            return FakeDataSource.GetOmitToCalcData();
        }

        /// <summary>
        /// 保存二次计算后的数据
        /// </summary>
        /// <param name="calcedData">计算后的数据</param>
        public static void SaveCalcedData(DataTable calcedData)
        {
            FakeDataSource.SaveCalcedData(calcedData);
        }

        /// <summary>
        /// 获取所有传感器的配置信息
        /// </summary>
        public static DataTable GetAllSensorConfig()
        {
            var sqlstr = @"
SELECT     dbo.T_DIM_SENSOR.STRUCT_ID, dbo.T_DIM_REMOTE_DTU.REMOTE_DTU_NUMBER, dbo.T_DIM_REMOTE_DTU.REMOTE_DTU_GRANULARITY,dbo.T_DIM_SENSOR.MODULE_NO, dbo.T_DIM_SENSOR.SENSOR_ID,dbo.T_DIM_SENSOR.DAI_CHANNEL_NUMBER, 
            Para1.ParaName AS ParaName1, dbo.T_DIM_FORMULAID_SET.Parameter1, Para2.ParaName AS ParaName2, dbo.T_DIM_FORMULAID_SET.Parameter2, Para3.ParaName AS ParaName3,
            dbo.T_DIM_FORMULAID_SET.Parameter3, Para4.ParaName AS ParaName4, dbo.T_DIM_FORMULAID_SET.Parameter4, Para5.ParaName AS ParaName5, dbo.T_DIM_FORMULAID_SET.Parameter5,
            Para6.ParaName AS ParaName6, dbo.T_DIM_FORMULAID_SET.Parameter6, Para7.ParaName AS ParaName7, dbo.T_DIM_FORMULAID_SET.Parameter7, Para8.ParaName AS ParaName8,
            dbo.T_DIM_FORMULAID_SET.Parameter8, dbo.T_DIM_SENSOR_PRODUCT.FORMAULAID, dbo.T_DIM_SENSOR.SAFETY_FACTOR_TYPE_ID, dbo.T_DIM_PROTOCOL_TYPE.PROTOCOL_CODE,
            dbo.T_DIM_REMOTE_DTU.REMOTE_DTU_SUBSCRIBER,dbo.T_DIM_SENSOR_PRODUCT.PRODUCT_TYPE_KEY
FROM         dbo.T_DIM_SENSOR INNER JOIN
                      dbo.T_DIM_REMOTE_DTU ON dbo.T_DIM_SENSOR.DTU_ID = dbo.T_DIM_REMOTE_DTU.ID INNER JOIN
                      dbo.T_DIM_SENSOR_PRODUCT ON dbo.T_DIM_SENSOR.PRODUCT_SENSOR_ID = dbo.T_DIM_SENSOR_PRODUCT.PRODUCT_ID INNER JOIN
                      dbo.T_DIM_PROTOCOL_TYPE ON dbo.T_DIM_SENSOR_PRODUCT.PROTOCOL_ID = dbo.T_DIM_PROTOCOL_TYPE.PROTOCOL_ID LEFT OUTER JOIN
                      dbo.T_DIM_FORMULAID_SET ON dbo.T_DIM_SENSOR.SENSOR_ID = dbo.T_DIM_FORMULAID_SET.SENSOR_ID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para1 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID1 = Para1.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para2 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID2 = Para2.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para3 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID3 = Para3.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para4 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID4 = Para4.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para5 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID5 = Para5.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para6 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID6 = Para6.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para7 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID7 = Para7.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para8 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID8 = Para8.FormulaParaID
WHERE     (dbo.T_DIM_SENSOR.IsDeleted = 0)";
            return SQLHelper.ExecuteDataset(loadDBConnName, CommandType.Text, sqlstr, null).Tables[0];

        }
        /// <summary>
        /// 获取传感器配置
        /// </summary>
        /// <param name="filter">过滤内容</param>
        public static DataTable GetSensorConfig(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return GetAllSensorConfig();
            var sqlstr =
                "SELECT * FROM [V_DEVICE_INFO] where " + filter;
            return SQLHelper.ExecuteDataset(loadDBConnName, CommandType.Text, sqlstr, null).Tables[0];
        }
        /// <summary>
        /// 获取分组信息表
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectSurveyGroup()
        {
            var sqlstr =
                "SELECT * FROM [T_DIM_SENSOR_GROUP_CEXIE]";
            return SQLHelper.ExecuteDataset(loadDBConnName, CommandType.Text, sqlstr).Tables[0];
        }
        /// <summary>
        /// 获取原始数据
        /// </summary>
        /// <returns></returns>
        public static DataTable GetOriginalData(DateTime time)
        {
            var sqlstr =
                "SELECT * FROM [T_DATA_ORIGINAL] where [CollectTime]=@collectTime";
            SqlParameter[] para = new[]
                {
                    new SqlParameter("@collectTime", time),
                };
            return SQLHelper.ExecuteDataset(loadDBConnName, CommandType.Text, sqlstr, para).Tables[0];
        }

        /// <summary>
        /// 获取原始数据表中某一时刻后的所有时间点
        /// </summary>
        public static DataTable SelectAllTime(DateTime after)
        {
            var sqlstr =
                "SELECT DISTINCT CollectTime FROM [T_DATA_ORIGINAL] where [CollectTime]>@begintime order by [CollectTime]";
            SqlParameter[] para = new[]
            {
                new SqlParameter("@begintime",after)
            };
            return SQLHelper.ExecuteDataset(loadDBConnName, CommandType.Text, sqlstr, para).Tables[0];
        }

        /// <summary>
        /// 获取传感器所属结构物ID
        /// </summary>
        public static int GetSensorStruct(int sensorId)
        {
            try
            {
#if STRUCT_IS_STRUCT
                var sql = @"select [STRUCT_ID] from [T_DIM_SENSOR] where [SENSOR_ID]=@sensorid"; 
                SqlParameter[] para = new[]
                {
                    new SqlParameter("@sensorid", sensorId)
                };
#else
                var sql = @"
select d.REMOTE_DTU_NUMBER from T_DIM_SENSOR s
join T_DIM_REMOTE_DTU d on d.ID=s.DTU_ID
where s.SENSOR_ID=@sensorid";
                SqlParameter[] para = new[]
                {
                    new SqlParameter("@sensorid", sensorId)
                };
#endif
                object res = SQLHelper.ExecuteScalar(loadDBConnName, CommandType.Text, sql, para);
                return Convert.ToInt32(res.ToString());
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 原始数据库中取出当前结构物所有历史遗漏数据
        /// </summary>
        public static DataTable SelectAllTime(DateTime after, int dtuid)
        {
            var sqlstr = @"
SELECT DISTINCT CollectTime 
FROM [T_DATA_ORIGINAL] o
LEFT JOIN [dbo].[T_DIM_SENSOR] s
on s.[SENSOR_ID]=o.SensorId 
JOIN [dbo].[T_DIM_REMOTE_DTU] d
on s.DTU_ID=d.ID
where [CollectTime]>@begintime and d.[REMOTE_DTU_NUMBER]=@dtuid
order by [CollectTime]";
            SqlParameter[] para = new[]
            {
                new SqlParameter("@begintime",after),
                new SqlParameter("@dtuid",dtuid)
            };
            return SQLHelper.ExecuteDataset(loadDBConnName, CommandType.Text, sqlstr, para).Tables[0];
        }

        /// <summary>
        /// 获取传感器参数表中传感器对应的第一个参数数据
        /// </summary>
        /// <param name="sensorId">传感器ID</param>
        /// <param name="parameter">输出结果</param>
        /// <returns>正确true 错误false</returns>
        public static bool GetSensorParameter1(int sensorId, out double parameter, out int formulaid)
        {
            parameter = 0;
            formulaid = 0;
            try
            {
                string sqlstr =
                    "select p.FORMAULAID from dbo.T_DIM_SENSOR_PRODUCT p left join dbo.T_DIM_SENSOR s on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID where s.SENSOR_ID=@sensorId";
                SqlParameter[] para = new[]
                {
                    new SqlParameter("@sensorId", sensorId)
                };
                object res = SQLHelper.ExecuteScalar(loadDBConnName, CommandType.Text, sqlstr, para);
                if (res == null || !int.TryParse(res.ToString(), out formulaid)) return false;

                switch (formulaid)
                {
                    case 12:    // 振动公式	A=V/k
                    case 26:    // 振动公式(含滤波)	A=V/k(含滤波)
                        sqlstr = "SELECT [Parameter1] FROM [T_DIM_FORMULAID_SET] where [SENSOR_ID]=@sensorId";
                        break;
                    case 28:    // 微震传感器公式	Microseismic(X,Y,Z,k)
                        sqlstr = "SELECT [Parameter4] FROM [T_DIM_FORMULAID_SET] where [SENSOR_ID]=@sensorId";
                        break;
                    default:
                        throw new Exception("不支持的振动计算公式.FormulaID = " + formulaid);
                }
                res = SQLHelper.ExecuteScalar(loadDBConnName, CommandType.Text, sqlstr, para);
                if (res == null || !double.TryParse(res.ToString(), out parameter)) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据结构物ID、模块号、通道号获取传感器ID
        /// </summary>
        public static int GetSensorId(int structid, int module, int channel)
        {
            try
            {
#if STRUCT_IS_STRUCT
                var sql = @"select SENSOR_ID from T_DIM_SENSOR where STRUCT_ID=@struct and MODULE_NO=@module and DAI_CHANNEL_NUMBER=@channel";
                SqlParameter[] para = new[]
                {
                    new SqlParameter("@struct", structid),
                    new SqlParameter("@module", module),
                    new SqlParameter("@channel", channel),
                };
#else
                var sql = @"select SENSOR_ID from T_DIM_SENSOR  s
left join T_DIM_REMOTE_DTU dtu on s.DTU_ID=dtu.ID
where dtu.REMOTE_DTU_NUMBER=@dtu and s.MODULE_NO=@module and s.DAI_CHANNEL_NUMBER=@channel";
                SqlParameter[] para = new[]
                {
                    new SqlParameter("@dtu", structid),
                    new SqlParameter("@module", module),
                    new SqlParameter("@channel", channel),
                };
#endif
                var restable = SQLHelper.ExecuteDataset(loadDBConnName, CommandType.Text, sql, para).Tables[0];
                if (restable != null && restable.Rows.Count == 1)
                {
                    return Convert.ToInt32(restable.Rows[0][0]);
                }
            }
            catch (Exception)
            {
            }
            return -1;
        }
        #endregion
    }
}