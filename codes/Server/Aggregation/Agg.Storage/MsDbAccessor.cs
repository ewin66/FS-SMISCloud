namespace Agg.Storage
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Text;

    using Agg.Comm.DataModle;
  
    using FS.DbHelper;

    using log4net;

    using DbType = FS.DbHelper.DbType;

    public class MsDbAccessor
    {
        private static readonly ILog Log = LogManager.GetLogger("MsDbAccessor");
        private const string AggDataTableName = "T_DATA_AGGREGATION";

        private const string AggDataColNames = "CureentData1,CureentData2,CureentData3,CureentData4";
        private const string AggDataChangeColNames = "Dlt1,Dlt2,Dlt3,Dlt4";

        private readonly Dictionary<AggType, string> DataTimeTableColNames;
        private readonly ISqlHelper helper;

        private ConcurrentDictionary<int, TableInfo> themeTableInfos;

        public MsDbAccessor(string connStr)
        {
            this.helper = SqlHelperFactory.Create(DbType.MSSQL, connStr);
            this.DataTimeTableColNames = InitDataTimeTableColName();
            this.InitThemeTableInfo();
        }

        private static Dictionary<AggType, string> InitDataTimeTableColName()
        {
            Dictionary<AggType, string> colName = new Dictionary<AggType, string>();
            colName.Add(AggType.Day, "DAY_CODE");
            colName.Add(AggType.Week, "WEEK_CODE");
            colName.Add(AggType.Month, "MONTH_CODE");
            return colName;
        }

        //internal ConcurrentDictionary<int, TableInfo> ThemeTableInfos
        //{
        //    get
        //    {
        //        return this.themeTableInfos;
        //    }
        //}

        private void InitThemeTableInfo()
        {
            this.themeTableInfos = this.GetThemeTableInfo();

        }

        /// <summary>
        /// 根据监测因素ID，获取主题表信息
        /// </summary>
        /// <param name="factorId"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        private bool GetTableInfo(int factorId, out TableInfo tableInfo)
        {
            tableInfo = null;

            if (this.themeTableInfos.ContainsKey(factorId))
            {
                tableInfo = this.themeTableInfos[factorId];
                return true;
            }
            else
            {
                TableInfo info = this.GetThemeTableInfo(factorId);
                if (info != null)
                {
                    this.themeTableInfos.TryAdd(factorId, info);
                    tableInfo = info;
                    return true;
                }
                else
                {
                    return false;
                }
            }


        }

        /// <summary>
        /// 根据监测因素Id获取主题表信息
        /// </summary>
        /// <param name="factorId">监测因素Id</param>
        /// <returns></returns>
        private TableInfo GetThemeTableInfo(int factorId)
        {
            string sql =
                String.Format(@"select [SAFETY_FACTOR_TYPE_ID], [THEMES_TABLE_NAME], [THEMES_COLUMNS] from [T_DIM_SAFETY_FACTOR_TYPE]
                where SAFETY_FACTOR_TYPE_ID = {0} and [THEMES_TABLE_NAME] is not null and [THEMES_COLUMNS] is not null",
                    factorId);
            try
            {
                DataSet ds = this.helper.Query(sql);
                if (ds == null && ds.Tables.Count != 1 && ds.Tables[0].Rows.Count != 1) return null;
                DataRow row = ds.Tables[0].Rows[0];
                return new TableInfo(Convert.ToString(row["THEMES_TABLE_NAME"]), Convert.ToString(row["THEMES_COLUMNS"]));
            }
            catch (Exception e)
            {
                Log.ErrorFormat("获取主题表信息失败,sql:{0}, error:{1},trace{2}", sql, e.Message, e.StackTrace);
                return null;
            }

        }

        private ConcurrentDictionary<int, TableInfo> GetThemeTableInfo()
        {
            ConcurrentDictionary<int, TableInfo> tables = new ConcurrentDictionary<int, TableInfo>();

            string sql =
                @"select [SAFETY_FACTOR_TYPE_ID], [THEMES_TABLE_NAME], [THEMES_COLUMNS] from [T_DIM_SAFETY_FACTOR_TYPE]
                where [THEMES_TABLE_NAME] is not null and [THEMES_COLUMNS] is not null";
            try
            {
                DataSet ds = this.helper.Query(sql);
                if (ds == null && ds.Tables.Count != 1) return tables;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    tables.TryAdd(
                        Convert.ToInt32(row["SAFETY_FACTOR_TYPE_ID"]),
                        new TableInfo(Convert.ToString(row["THEMES_TABLE_NAME"]), Convert.ToString(row["THEMES_COLUMNS"])));
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("获取主题表信息失败,sql:{0}, error:{1},trace{2}", sql, e.Message, e.StackTrace);
            }
            return tables;
        }

        public DataTable GetConfig()
        {
            //DataTable dt;
            string sql =
                @"select [Id], [StructureId],[FacotrId],[AggTypeId],[AggWayId],[DataBeginHour],[DataEndHour],[DateBegin],[DateEnd],[TimeMode] from [T_DIM_AGG_CONFIG] where [IsEnable] = 1 and [IsDelete]=0";
            DataSet ds;
            try
            {
                ds = this.helper.Query(sql);
                if (ds == null || ds.Tables.Count != 1) return null;

                return ds.Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 判断传感器ID是否有变更
        /// </summary>
        /// <param name="structId">结构物ID</param>
        /// <param name="safeFactorId">监测因素Id</param>
        /// <param name="sensorIds">传感器Id列表</param>
        /// <returns></returns>
        public bool IsSensorIdUpdated(int structId, int safeFactorId, List<int> sensorIds)
        {
            if (sensorIds == null || sensorIds.Count == 0)
                return false;

            ///传感器总数是否有变更
            string sql =
                String.Format(
                    @"select COUNT([SENSOR_ID]) num from [T_DIM_SENSOR] where STRUCT_ID = {0} and SAFETY_FACTOR_TYPE_ID = {1} and Identification in (0,2)",
                    structId,
                    safeFactorId);

            DataSet ds;
            int num;
            try
            {
                ds = this.helper.Query(sql);
                if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return false;
                num = Convert.ToInt16(ds.Tables[0].Rows[0]["num"]);
                if (num != sensorIds.Count) return true;
            }
            catch (Exception)
            {
                return false;
            }


            ///传感器具体信息是否有变更
            StringBuilder sb = new StringBuilder();
            sb.Append(" (");
            for (int i = 0; i < sensorIds.Count; i++)
            {
                sb.Append(sensorIds[i]);
                /// 最后一个
                if (i == sensorIds.Count - 1)
                {
                    sb.Append(")");
                }
                else
                {
                    sb.Append(",");
                }
            }

            sql =
                String.Format(
                    @"select COUNT([SENSOR_ID]) num from [T_DIM_SENSOR] where STRUCT_ID = {0} and SAFETY_FACTOR_TYPE_ID = {1} and Identification in (0,2) and SENSOR_ID in {2}",
                    structId,
                    safeFactorId,
                    sb.ToString());

            try
            {
                ds = this.helper.Query(sql);
                if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return false;
                num = Convert.ToInt16(ds.Tables[0].Rows[0]["num"]);
                if (num != sensorIds.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static List<List<double>> ConvertDataSetToValue(DataSet ds, string[] colName)
        {
            List<List<double>> ret = new List<List<double>>();
            if (ds != null && ds.Tables.Count == 1)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    List<double> items = new List<double>();
                    for (int i = 0; i < colName.Count(); i++)
                    {
                        if (row[colName[i]].ToString() != string.Empty)
                        {
                            items.Add(Convert.ToDouble(row[colName[i]]));
                        }
                    }
                    ret.Add(items);
                }
            }
            return ret;

        }

        /// <summary>
        /// 获取数据列非空的条件
        /// </summary>
        /// <param name="colNames"></param>
        /// <returns></returns>
        private string GetDataNotNullCondition(string[] colNames)
        {
            if (colNames.Count() == 0) return String.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (var colName in colNames)
            {
                sb.AppendFormat(" and {0} is not null ", colName);
            }
            return sb.ToString();

        }

        /// <summary>
        /// 获取周数据时间
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        private static string GetDayOfWeekString(int startDay, int endDay)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = startDay; i <= endDay; i++)
            {
                if (i != 7)
                {
                    sb.Append((i + 1) % 8);
                }
                else
                {
                    sb.Append(1);
                }
                
                if (i < endDay) sb.Append(",");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取周聚集数据
        /// </summary>
        /// <param name="sensorId">传感器ID</param>
        /// <param name="factorId"></param>
        /// <param name="timeRange">周中的天，周一：1-周日：7</param>
        /// <param name="beginTime">开始时间范围</param>
        /// <param name="endTime">结束时间范围</param>
        /// <returns></returns>
        public RawData GetWeekAggRawData(int sensorId, int factorId, AggTimeRange timeRange , DateTime beginTime, DateTime endTime)
        {
            TableInfo info;
            if (!this.GetTableInfo(factorId, out info)) return null;

            string colums = info.ColumnNames;
            string tableName = info.TableName;
            string[] colName = colums.Split(',');

            string sql = String.Format(
                @"select {0} from {1} where ACQUISITION_DATETIME between '{2}' and '{3}' and DATEPART(dw,[ACQUISITION_DATETIME]) in ({4})
                and DATEPART(hh,[ACQUISITION_DATETIME])>= {5} and DATEPART(hh,[ACQUISITION_DATETIME]) < {6}
                and SENSOR_ID = {7} {8}",
                colums,
                tableName,
                beginTime,
                endTime,
                GetDayOfWeekString(timeRange.DateBegin, timeRange.DateEnd),
                timeRange.DataBeginHour,
                timeRange.DataEndHour,
                sensorId,
                this.GetDataNotNullCondition(colName)
                );

            RawData rawData = new RawData();
            rawData.SensorId = sensorId;

            try
            {

                DataSet ds = this.helper.Query(sql);
                Log.DebugFormat("{0}传感器获取周聚集数据Sql:{1}",sensorId,sql);
                rawData.Values = ConvertDataSetToValue(ds, colName);

            }
            catch (Exception e)
            {
                Log.ErrorFormat("{0}传感器周聚集数据获取失败,sql:{1}, error:{2},trace{3}", sensorId, sql, e.Message, e.StackTrace);
            }

            return rawData;
        }

        /// <summary>
        /// 获取周聚集数据
        /// </summary>
        /// <param name="sensorId">传感器ID</param>
        /// <param name="factorId"></param>
        /// <param name="timeRange">月的天</param>
        /// <param name="beginTime">开始时间范围</param>
        /// <param name="endTime">结束时间范围</param>
        /// <returns></returns>
        public RawData GetMonthAggRawData(int sensorId, int factorId, AggTimeRange timeRange, DateTime beginTime, DateTime endTime)
        {
            TableInfo info;
            if (!this.GetTableInfo(factorId, out info)) return null;

            string colums = info.ColumnNames;
            string tableName = info.TableName;
            string[] colName = colums.Split(',');

            string sql = String.Format(
                @"select {0} from {1} where ACQUISITION_DATETIME between '{2}' and '{3}' 
                and DATEPART(hh,[ACQUISITION_DATETIME])>= {4} and DATEPART(hh,[ACQUISITION_DATETIME]) < {5}
                and SENSOR_ID = {6} {7}",
                colums,
                tableName,
                beginTime,
                endTime,
                timeRange.DataBeginHour,
                timeRange.DataEndHour,
                sensorId,
                this.GetDataNotNullCondition(colName)
                );

            RawData rawData = new RawData();
            rawData.SensorId = sensorId;

            try
            {
                DataSet ds = this.helper.Query(sql);
                Log.DebugFormat("{0}传感器获取月聚集数据Sql:{1}", sensorId, sql);
                rawData.Values = ConvertDataSetToValue(ds, colName);
            }
            catch (Exception e)
            {

                Log.ErrorFormat("{0}传感器月聚集数据获取失败,sql:{1}, error:{2},trace{3}", sensorId, sql, e.Message, e.StackTrace);
            }

            return rawData;
        }

        /// <summary>
        /// 获取周聚集数据
        /// </summary>
        /// <param name="sensorId">传感器ID</param>
        /// <param name="factorId"></param>
        /// <param name="timeRange">开始小时和结束小时</param>
        /// <param name="beginTime">开始时间范围</param>
        /// <param name="endTime">结束时间范围</param>
        /// <returns></returns>
        public RawData GetDayAggRawData(int sensorId, int factorId, AggTimeRange timeRange, DateTime beginTime, DateTime endTime)
        {
            TableInfo info;
            if (!this.GetTableInfo(factorId, out info)) return null;

            string colums = info.ColumnNames;
            string tableName = info.TableName;
            string[] colName = colums.Split(',');

            string sql = String.Format(
                @"select {0} from {1} where ACQUISITION_DATETIME between '{2}' and '{3}' and
                DATEPART(hh,[ACQUISITION_DATETIME])>= {4} and DATEPART(hh,[ACQUISITION_DATETIME]) < {5} and SENSOR_ID = {6} {7}",
                colums,
                tableName,
                beginTime,
                endTime,
                timeRange.DataBeginHour,
                timeRange.DataEndHour,
                sensorId,
                this.GetDataNotNullCondition(colName)
                );
            RawData rawData = new RawData();
            rawData.SensorId = sensorId;
            try
            {
                DataSet ds = this.helper.Query(sql);
                Log.DebugFormat("{0}传感器获取日聚集数据Sql:{1}", sensorId, sql);
                rawData.Values = ConvertDataSetToValue(ds, colName);
            }
            catch (Exception e)
            {

                Log.ErrorFormat("{0}传感器日聚集数据获取失败,sql:{1}, error:{2},trace{3}", sensorId, sql, e.Message, e.StackTrace);
            }

            return rawData;
        }

        /// <summary>
        /// 获取非数据的传感器ID
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="safeFactorId"></param>
        /// <returns></returns>
        public List<int> GetSensorIds(int structId, int safeFactorId)
        {
            string sql = String.Format(@"select SENSOR_ID from [dbo].[T_DIM_SENSOR]
                WHERE [STRUCT_ID]={0} AND SAFETY_FACTOR_TYPE_ID ={1} and [IsDeleted]=0 and [Identification] in (0,2)", structId, safeFactorId);
            List<int> sensorIds = new List<int>();

            try
            {
                DataSet ds = this.helper.Query(sql);
                if (ds != null && ds.Tables.Count == 1)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        sensorIds.Add(Convert.ToInt16(row["SENSOR_ID"]));
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("获取传感器Id失败,sql:{0}, error:{1},trace{2}", sql, e.Message, e.StackTrace);
            }
            return sensorIds;
        }

        /// <summary>
        /// 获取上次聚集的时间ID
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="safeFactorId"></param>
        /// <param name="type"></param>
        /// <returns>-1：表示失败，其他表示聚集时间Id</returns>
        public int GetLastestDateTimeId(int structId, int safeFactorId, AggType type)
        {
            string sql = String.Format(
                @"select top 1 ([DateTimeId]) from {3} where StructureId = {0}  
                and SafeFactorId = {1} and [AggDataTypeId] = {2} order by [DateTimeId] desc",
                structId,
                safeFactorId,
                Convert.ToInt16(type),
                AggDataTableName);

            try
            {
                DataSet ds = this.helper.Query(sql);
                if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return -1;
                int id = Convert.ToInt32(ds.Tables[0].Rows[0]["DateTimeId"]);
                return id;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(
                    "{0}结构物，监测因素{1}，{2}聚集数据获取失败,sql:{3}, error:{4},trace{5}",
                    structId,
                    safeFactorId,
                    type,
                    sql,
                    e.Message,
                    e.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// 获取最新的聚集数据
        /// </summary>
        /// <param name="sensorId">传感器Id</param>
        /// <param name="type">聚集类型</param>
        /// <param name="dateTimeId">日期Id</param>
        /// <returns></returns>
        public AggData GetLastestAggData(int sensorId, AggType type, int dateTimeId)
        {
            string sql =
                String.Format(
                    @"select {0} from {1} where [SensorId] = {2} and [AggDataTypeId] = {3} and [DateTimeId] = {4}",
                    AggDataColNames,
                    AggDataTableName,
                    sensorId,
                    Convert.ToInt32(type),
                    dateTimeId);

            try
            {
                DataSet ds = this.helper.Query(sql);
                if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return null;
                AggData aggData = new AggData();
                string[] colNames = AggDataColNames.Split(',');
                aggData.SensorId = sensorId;
                DataRow row = ds.Tables[0].Rows[0];
                foreach (var colName in colNames)
                {
                    if (row[colName].ToString() != string.Empty)
                    {
                        aggData.Values.Add(Convert.ToDouble(row[colName]));
                    }
                }
                return aggData;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(
                    "传感器{0}，{1}的{2}聚集数据获取失败,sql:{3}, error:{4},trace{5}",
                    sensorId,
                    dateTimeId,
                    type,
                    sql,
                    e.Message,
                    e.StackTrace);
                return null;
            }

        }


        /// <summary>
        ///保存聚集数据结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public int SaveAggResult(AggResult result)
        {
            if (result.AggDatas == null || result.AggDataChanges == null) return -1;

            int dateTimeId = this.GetDateTimeId(result.AggType, result.TimeTag);

            if (dateTimeId < 1) return 0;

            List<string> sqlCmds = new List<string>();

            for (int i = 0; i < result.AggDatas.Count; i++)
            {
                sqlCmds.Add(
                    this.CreateAddOrUpdateAggDataSql(
                        result.StructId,
                        result.SafeFactorId,
                        dateTimeId,
                        result.AggType,
                        result.AggDatas[i],
                        result.AggDataChanges[i],result.ConfigId));
            }

            return this.helper.ExecuteSqlTran(sqlCmds);
        }

        /// <summary>
        /// 根据时间编号获取Id
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="timeTag">时间Code</param>
        /// <returns>Id</returns>
        private int GetDateTimeId(AggType type, string timeTag)
        {
            string sql = String.Format(@"select top 1 DATETIME_ID from T_DIM_DATETIME where {0} = '{1}' order by DATETIME_ID", this.DataTimeTableColNames[type], timeTag);
            try
            {
                DataSet ds = this.helper.Query(sql);
                if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return -1;
               
                int id = Convert.ToInt32(ds.Tables[0].Rows[0]["DATETIME_ID"]);
               
                return id;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(
                    "获取DateTimeId失败,Type:{0},code:{1},sql:{2}, error:{3},trace{4}",
                    type.ToString(),
                    timeTag,
                    sql,
                    e.Message,
                    e.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// 判断是否存在历史聚集数据
        /// </summary>
        /// <param name="DateTimeId">时间Id</param>
        /// <param name="SensorId">传感器Id</param>
        /// <param name="type">聚集数据类型</param>
        /// <returns></returns>
        private bool IsAggDataExist(int DateTimeId, int SensorId, AggType type)
        {
            string sql =
                String.Format(
                    @"select count([Id]) count from [T_DATA_AGGREGATION] where [SensorId]={0} and [DateTimeId]={1} and [AggDataTypeId] = {2}",
                    SensorId,
                    DateTimeId,
                    Convert.ToInt32(type));
            try
            {
                DataSet ds = this.helper.Query(sql);
                if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return false;
               
                int num = Convert.ToInt32(ds.Tables[0].Rows[0]["count"]);
               
                return (num > 0);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(
                    "IsAggDataExist查询失败,sql:{0}, error:{1},trace{2}",
                    sql,
                    e.Message,
                    e.StackTrace);
                return false;
            }
        }

        private string GetUpdateAggDataString(string[] colName, List<double> data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < colName.Length; i++)
            {
                if (i < data.Count) /// 
                {
                    sb.AppendFormat("{0} = {1}", colName[i], data[i]);
                }
                else
                {
                    sb.AppendFormat("{0} = null", colName[i]);
                }
                if (i != colName.Length - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 产生聚集数据Sql
        /// </summary>
        /// <param name="structureId"></param>
        /// <param name="factorId"></param>
        /// <param name="dateTimeId"></param>
        /// <param name="type"></param>
        /// <param name="aggData"></param>
        /// <param name="aggDataChange"></param>
        /// <returns></returns>
        private string CreateAddOrUpdateAggDataSql(
            int structureId, int factorId, int dateTimeId, AggType type, AggData aggData, AggData aggDataChange, int configId)
        {
            string sql;
            string[] aggCol = AggDataColNames.Split(',');
            string[] aggChangeCol = AggDataChangeColNames.Split(',');

            if (this.IsAggDataExist(dateTimeId, aggData.SensorId, type))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.GetUpdateAggDataString(aggCol, aggData.Values));
                sb.Append(",");
                sb.Append(this.GetUpdateAggDataString(aggChangeCol, aggDataChange.Values));
                sb.Append(",");
                sb.Append(string.Format("[AggCofigId]={0}", configId));
                sql =
                    String.Format(
                        @"update [T_DATA_AGGREGATION] set {0} where [SensorId]={1} and [DateTimeId]={2} and [AggDataTypeId] = {3}",
                        sb,
                        aggData.SensorId,
                        dateTimeId,
                        Convert.ToInt32(type));
            }
            else
            {
                StringBuilder col = new StringBuilder();
               
                StringBuilder val = new StringBuilder();
                
                for (int i = 0; i < aggData.Values.Count; i++)
                {
                    col.AppendFormat("{0},{1}", aggCol[i], aggChangeCol[i]);
                    val.AppendFormat("{0},{1}", aggData.Values[i], aggDataChange.Values[i]);
                    if (i != aggData.Values.Count - 1)
                    {
                        col.Append(",");
                        val.Append(",");
                    }
                }
              
                sql =
                    String.Format(
                        @"insert into [T_DATA_AGGREGATION] ({0},[DateTimeId],[StructureId],[SafeFactorId],[SensorId],[AggDataTypeId],[AggCofigId]) values({1},{2},{3},{4},{5},{6},{7})",
                        col,
                        val,
                        dateTimeId,
                        structureId,
                        factorId,
                        aggData.SensorId,
                        Convert.ToInt32(type),
                        configId);
            }
            return sql;
        }

        //DateTime GetStorageTime(DACTaskResult tr, SensorAcqResult ar)
        //{
        //    switch (tr.StoragedTimeType)
        //    {
        //        case SensorAcqResultTimeType.TaskFinishedTime:
        //            return tr.Finished;
        //        case SensorAcqResultTimeType.TaskStartTime:
        //            return tr.Started;
        //        case SensorAcqResultTimeType.SensorRequestTime:
        //            return ar.RequestTime;
        //        case SensorAcqResultTimeType.SensorResponseTime:
        //            return ar.ResponseTime;
        //        default:
        //            return tr.Finished;
        //    }
        //}


        /// <summary>
        /// 数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        /// <param name="acqtime"></param>
        /// <returns></returns>
        //public string GenerateAddValueSql(string tableName, SensorAcqResult senacqreslt,DateTime acqtime)
        //{
        //    ISensorData data =senacqreslt.RawData;
        //    double?[] values = tableName == RawDataTable ? Double2NullableDouble(data.RawValues) :  data.ThemeValues.ToArray();
        //    if (values == null || values.Length == 0)
        //        return null;
        //    TableInfo table = themeTableInfos.ContainsKey(tableName)
        //        ? themeTableInfos[tableName] : default(TableInfo);
        //    if (table == null)
        //    {
        //        string error = string.Format(" miss table {0} ;", tableName);
        //        try
        //        {
        //            LogErrorData(senacqreslt);
        //        }
        //        catch (Exception ex)
        //        {
        //            error += ex.Message;
        //        }
        //        throw new Exception(error);
        //    }
        //    string columnStr = themeTableInfos[tableName].Colums;
        //    var valueStr = new StringBuilder();
        //    string tableCommonColumns = GetTableCommonColumns(tableName, senacqreslt.Sensor, acqtime);
        //    valueStr.Append(tableCommonColumns);
        //    int tableCommonColumnscount =
        //        tableCommonColumns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Length;
        //    int columscount = columnStr.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Length;

        //    if (values.Length + tableCommonColumnscount > columscount)
        //    {
        //        string error = string.Format("values's count is out of {0}'s columns's count ",
        //            table.ThemeTableName);
        //        try
        //        {
        //            LogErrorData(senacqreslt, tableName);
        //        }
        //        catch (Exception ex)
        //        {
        //            error += ex.Message;
        //        }
        //        throw new Exception(error);
        //    }

        //    for (int i = tableCommonColumnscount; i < columscount; i++)
        //    {
        //        int index = i - tableCommonColumnscount;
        //        if (index >= values.Length || values[index] == null)
        //            valueStr.AppendFormat(",{0}", "null");
        //        else
        //            valueStr.AppendFormat(",{0:0.######}", values[index].Values);
        //    }
        //    string sql = string.Format(@"insert into {0} ({1}) values ({2})", table.ThemeTableName, table.Colums,
        //        valueStr);
        //    return sql;
        //}

        //private string GetTableCommonColumns(string table, Sensor sensor,DateTime acqtime)
        //{
        //    // 由于网站展示数据时，需要时间统一，故入库时统一修改时间data.AcqTime 为入库时间
        //    string valuecommoncolumns = table == RawDataTable
        //        ? string.Format("{0},'{1:yyyy-MM-dd HH:mm:ss.fff}'", sensor.SensorID, acqtime)
        //        : string.Format("{0},{1},'{2:yyyy-MM-dd HH:mm:ss.fff}'", sensor.SensorID, sensor.FactorType,
        //            acqtime);
        //    return valuecommoncolumns;
        //}

        /// <summary>
        /// 把double[]转成double?[]
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        //public double?[] Double2NullableDouble(double[] values)
        //{
        //    if(values==null||values.Length==0)
        //        return null;
        //    var nulablevalues = new double?[values.Length];
        //    for (var i = 0; i < values.Length; i++)
        //    {
        //        nulablevalues[i] = values[i];
        //    }
        //    return nulablevalues;
        //}

        //public bool LogErrorData(SensorAcqResult senacqreslt,string tablename="")
        //{
        //    ISensorData data = senacqreslt.RawData;
        //    var errdata = new StringBuilder();
        //    errdata.AppendFormat("{0},{1}", senacqreslt.Sensor.SensorID, senacqreslt.ResponseTime);
        //    foreach (var d in data.RawValues)
        //    {
        //        errdata.AppendFormat(",{0:0.######}", d);
        //    }
        //    if (!string.IsNullOrEmpty(tablename))
        //    {
        //        errdata.AppendFormat(",{0}", tablename);
        //    }
        //    FileService.Write(_errorPath, errdata.ToString());
        //    errdata.Clear();
        //    errdata.AppendFormat("{0},{1},{2}", senacqreslt.Sensor.SensorID, senacqreslt.ResponseTime, senacqreslt.Sensor.FactorType);
        //    foreach (var d in data.ThemeValues)
        //    {
        //        errdata.AppendFormat(",{0:0.######}", d);
        //    }
        //    if (!string.IsNullOrEmpty(tablename))
        //    {
        //        errdata.AppendFormat(",{0}", tablename);
        //    }
        //    FileService.Write(_errorPath, errdata.ToString());
        //    return true;
        //}
    }
}
