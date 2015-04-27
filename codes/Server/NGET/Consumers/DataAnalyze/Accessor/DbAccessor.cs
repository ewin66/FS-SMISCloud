// --------------------------------------------------------------------------------------------
// <copyright file="DbAccessor.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141029
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET.DataAnalyzer.Accessor
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;

    using FS.DbHelper;

    using log4net;

    public class DbAccessor
    {
        private static readonly string connStr = ConfigurationManager.AppSettings["SecureCloud"];

        private static readonly ILog Log = LogManager.GetLogger("DataRationalFilter");

        private static ISqlHelper sqlHelper = SqlHelperFactory.Create(FS.DbHelper.DbType.MSSQL, connStr);

        /// <summary>
        /// 获取传感器阈值配置
        /// </summary>        
        /// <param name="sensors">传感器编号数组</param>
        /// <returns>阈值配置列表</returns>
        public static DataTable GetSensorThreshold(IEnumerable<uint> sensors)
        {
            if (sensors == null) return new DataTable();
            var uints = sensors as uint[] ?? sensors.ToArray();
            if (!uints.Any()) return new DataTable();

            string sql = string.Format(@"
SELECT[SensorId]
      ,[ItemId]
      ,[ThresholdLevel]
      ,[ThresholdDownValue]
      ,[ThresholdUpValue]
FROM [dbo].[T_FACT_SENSOR_THRESHOLD]
WHERE [SensorId] in ({0})", string.Join(",", uints));

            try
            {
                return sqlHelper.Query(sql).Tables[0];
            }
            catch (Exception e)
            {
                Log.Error("get sensors' threshold error", e);
                return new DataTable();
            }
        }

        /// <summary>
        /// 获取传感器信息
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <returns></returns>
        public static DataTable GetSensorInfo(int sensorId)
        {
            string sql = string.Format(@"
SELECT [SENSOR_ID] AS Id
      ,[PRODUCT_SENSOR_ID] AS ProductId     
      ,[SENSOR_LOCATION_DESCRIPTION] AS Location            
      ,[STRUCT_ID] AS StructureId
      ,[SAFETY_FACTOR_TYPE_ID] AS FactorId
      ,[DTU_ID] AS DtuId
      ,[MODULE_NO] AS ModuleNo
      ,[DAI_CHANNEL_NUMBER] AS ChannelNo  
FROM [dbo].[T_DIM_SENSOR]
WHERE [SENSOR_ID]={0} AND [IsDeleted]=0
", sensorId);

            try
            {
                return sqlHelper.Query(sql).Tables[0];
            }
            catch (Exception e)
            {
                Log.Error("get sensor info error", e);
                return new DataTable();
            }
        }

        /// <summary>
        /// 查询监测因素信息
        /// </summary>
        /// <param name="factorId">监测因素编号</param>
        /// <returns></returns>
        public static DataTable GetSafetyFactorInfo(int factorId)
        {
            string sql = string.Format(@"
SELECT [SAFETY_FACTOR_TYPE_ID] AS FactorId
      ,[SAFETY_FACTOR_TYPE_PARENT_ID] AS ParentId
      ,[SAFETY_FACTOR_TYPE_NAME] AS Name      
      ,[FACTOR_VALUE_COLUMN_NUMBER] AS ColNumber
      ,[FACTOR_VALUE_COLUMNS] AS Columns
      ,[FACTOR_VALUE_DECIMAL_PLACES] AS Precisions
      ,[FACTOR_VALUE_UNIT] AS Units         
      ,[THEMES_TABLE_NAME] AS TableName
      ,[THEMES_COLUMNS] AS TableColumns
      ,[DESCRIPTION] AS Description
FROM [dbo].[T_DIM_SAFETY_FACTOR_TYPE]
WHERE [SAFETY_FACTOR_TYPE_ID]={0}", factorId);

            try
            {
                return sqlHelper.Query(sql).Tables[0];
            }
            catch (Exception e)
            {
                Log.Error("get factor info error", e);
                return new DataTable();
            }
        }

        /// <summary>
        /// 查询组织结构物
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns></returns>
        public static DataTable GetOrgStcByStruct(int structId)
        {
            string sql = string.Format(@"
SELECT [ORG_STRUC_ID] AS OrgStcId
	,[ORGANIZATION_ID] AS OrgId
	,[STRUCTURE_ID]	AS StcId
FROM [T_DIM_ORG_STUCTURE]
WHERE STRUCTURE_ID={0}
", structId);
            try
            {
                return sqlHelper.Query(sql).Tables[0];
            }
            catch (Exception e)
            {
                Log.Error("get orgstc error", e);
                return new DataTable();
            }
        }

        /// <summary>
        /// 查询主题权重
        /// </summary>
        /// <param name="orgStcId">组织结构物编号</param>
        /// <returns></returns>
        public static DataTable GetThemeWeightByOrgStc(int orgStcId)
        {
            string sql = string.Format(@"
SELECT DISTINCT t.[SAFETY_FACTOR_TYPE_ID] AS ThemeId
      ,t.[SAFETY_FACTOR_TYPE_NAME] AS ThemeName
      ,ISNULL(w.[SAFETY_FACTOR_WEIGHTS], 0) AS Weight
FROM [T_DIM_SAFETY_FACTOR_TYPE] f
JOIN [T_DIM_STRUCTURE_FACTOR] sf on f.SAFETY_FACTOR_TYPE_ID=sf.SAFETY_FACTOR_TYPE_ID
JOIN [T_DIM_ORG_STUCTURE] os on os.STRUCTURE_ID=sf.STRUCTURE_ID
JOIN [T_DIM_SAFETY_FACTOR_TYPE] t on f.SAFETY_FACTOR_TYPE_PARENT_ID=t.SAFETY_FACTOR_TYPE_ID
LEFT JOIN [T_FACT_SAFETY_FACTOR_WEIGHTS] w on os.ORG_STRUC_ID=w.ORG_STRUC_ID AND t.SAFETY_FACTOR_TYPE_ID=w.SAFETY_FACTOR_TYPE_ID
WHERE os.ORG_STRUC_ID={0}
", orgStcId);
            try
            {
                return sqlHelper.Query(sql).Tables[0];
            }
            catch (Exception e)
            {
                Log.Error("get theme weight error", e);
                return new DataTable();
            }
        }

        /// <summary>
        /// 查询传感器权重 (因素权重*传感器权重)
        /// </summary>
        /// <param name="orgStcId">组织结构物编号</param>
        /// <param name="themeId">主题编号</param>
        /// <returns></returns>
        public static DataTable GetSensorWeightByOrgStc(int orgStcId, int themeId)
        {
            string sql = string.Format(@"
SELECT s.SENSOR_ID AS SensorId
	,ISNULL(w.SENSOR_WEIGHTS, 0) / 100
	* CAST((SELECT ISNULL(MAX(fw.SUB_SAFETY_FACTOR_WEIGHTS), 0)
	FROM T_FACT_SUB_SAFETY_FACTOR_WEIGHTS fw
	WHERE os.ORG_STRUC_ID=fw.ORG_STRUC_ID AND s.SAFETY_FACTOR_TYPE_ID=fw.SAFETY_FACTOR_TYPE_ID)AS int) AS Weight
FROM [T_DIM_ORG_STUCTURE] os
JOIN [T_DIM_SENSOR] s on os.STRUCTURE_ID=s.STRUCT_ID
JOIN [T_DIM_SAFETY_FACTOR_TYPE] f on s.SAFETY_FACTOR_TYPE_ID=f.SAFETY_FACTOR_TYPE_ID
LEFT JOIN [T_FACT_SENSOR_WEIGHTS] w on s.SENSOR_ID=w.SENSOR_ID AND os.ORG_STRUC_ID=w.ORG_STRUC_ID
WHERE os.ORG_STRUC_ID={0} AND f.SAFETY_FACTOR_TYPE_PARENT_ID={1} AND s.IsDeleted=0
", orgStcId, themeId);

            try
            {
                return sqlHelper.Query(sql).Tables[0];
            }
            catch (Exception e)
            {
                Log.Error("get sensor weight error", e);
                return new DataTable();
            }
        }

        /// <summary>
        /// 结构物分数入库
        /// </summary>
        /// <param name="orgStcId"></param>
        /// <param name="stcScore"></param>
        /// <param name="time"></param>
        public static void SaveStructureScore(int orgStcId, int stcScore, DateTime time)
        {
            string sql = string.Format(@"
INSERT INTO[T_FACT_STRUCTURE_SCORE](ORG_STRUC_ID,STRUCTURE_SCORE,EVALUATION_DATETIME)
VALUES({0},{1},'{2}')
", orgStcId, stcScore, time);

            try
            {
                sqlHelper.ExecuteSql(sql);
            }
            catch (Exception e)
            {
                Log.Error("save stc score error", e);
            }
        }

        /// <summary>
        /// 主题分数入库
        /// </summary>
        /// <param name="orgStcId"></param>
        /// <param name="themeId"></param>
        /// <param name="score"></param>
        /// <param name="time"></param>
        public static void SaveThemeScore(int orgStcId, int themeId, int score, DateTime time)
        {
            string sql = string.Format(@"
INSERT INTO[T_FACT_SAFETY_FACTOR_SCORE](ORG_STRUC_ID,SAFETY_FACTOR_TYPE_ID,SAFETY_FACTOR_SCORE,EVALUATION_DATETIME)
VALUES({0},{1},{2},'{3}')
", orgStcId, themeId, score, time);

            try
            {
                sqlHelper.ExecuteSql(sql);
            }
            catch (Exception e)
            {
                Log.Error("save theme score error");
            }
        }
    }
}