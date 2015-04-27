using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataCenter.Model;
using DataCenter.Task;
using log4net;
using DataCenter.Util;
using Newtonsoft.Json;
using System.IO;

namespace DataCenter.Accessor
{
    public class SqlDal
    {
        /// <summary>
        /// 保存插入数据表和对应的sql语句.
        /// </summary>
        private static Dictionary<string, string> InsertSqlStrs = new Dictionary<string, string>();

        private  static Dictionary<string, List<ColumnAttribute>> SqlTableColcums=new Dictionary<string, List<ColumnAttribute>>();

        private static Dictionary<int, string> safetyFactorTypeIDTableNames;

        private static Dictionary<int, object[]> moduleIDSensors;

        private static readonly ILog Log = LogManager.GetLogger(typeof(SqlDal));

        private const string sqlInsertstr = "Insert {0} ({1}) values ({2})";

        /// <summary>
        /// The get sqlstr.
        /// </summary>
        /// <param name="tableName">
        /// 表名.
        /// </param>
        /// <returns>
        ///  对应表的SQL插入语句.
        /// </returns>
        private static string GetSqlstr(string tableName)
        {
            if (InsertSqlStrs.ContainsKey(tableName))
            {
                return InsertSqlStrs[tableName];
            }

            List<ColumnAttribute> insertColums = ThemesDataUtility.GetThemesDataTableColumnAttribute(tableName);

            if (insertColums != null && !SqlTableColcums.ContainsKey(tableName))
            {
                SqlTableColcums.Add(tableName, insertColums);
            }

            if (insertColums == null)
            {
                var smg = new StringBuilder("未找到该表实体对象：");
                smg.Append(tableName);
                Log.Error(smg.ToString());
                throw new Exception(smg.ToString());
            }

            var sbInsertSql = new StringBuilder();
            foreach (ColumnAttribute colAttr in insertColums)
            {
                sbInsertSql.Append("@" + colAttr.Name + ",");
            }

            var sqlparams = sbInsertSql.ToString();
            sqlparams = sqlparams.Substring(0, sqlparams.Length - 1);

            var sqlvalues = sqlparams.Replace("@", string.Empty);
            var insertSql = string.Format(sqlInsertstr, tableName, sqlvalues, sqlparams);
            InsertSqlStrs.Add(tableName, insertSql);
            return insertSql;
        }

        /// <summary>
        /// The get sql parameters.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="datavalues">
        /// The datavalues.
        /// </param>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="safetyFactorTypeID">
        /// The safety factor type id.
        /// </param>
        /// <param name="acqTime">
        /// The acq time.
        /// </param>
        /// <returns>
        /// The <see cref="SqlParameter[]"/>.
        /// </returns>
        private static SqlParameter[] GetSqlParameters(string tableName, float[] datavalues, int sensorId, int safetyFactorTypeID,DateTime acqTime)
        {
            List<ColumnAttribute> insertColums;
            if (!SqlTableColcums.ContainsKey(tableName))
            {
                insertColums = ThemesDataUtility.GetThemesDataTableColumnAttribute(tableName);

                if (insertColums != null && !SqlTableColcums.ContainsKey(tableName))
                {
                    SqlTableColcums.Add(tableName, insertColums);
                }
            }
            else
            {
                insertColums = SqlTableColcums[tableName]; 
            }

            if (insertColums != null)
            {
                var parms = new SqlParameter[insertColums.Count];
                if (parms.Length >= datavalues.Length + 3)
                {
                    var datalen = datavalues.Length;
                    var len = parms.Length;
                    var colName = new StringBuilder();

                    for (int i = 0; i < datalen; i++)
                    {
                        colName.Append("@" + insertColums[i].Name);
                        parms[i] = new SqlParameter(colName.ToString(), datavalues[i]);
                        colName.Clear();
                    }

                    for (int i = datalen; i < len - 3; i++)
                    {
                        colName.Append("@" + insertColums[i].Name);
                        parms[i] = new SqlParameter(colName.ToString(), null);
                        colName.Clear();
                    }

                    colName.Append("@" + insertColums[len - 3].Name);
                    parms[len - 3] = new SqlParameter(colName.ToString(), sensorId);
                    colName.Clear();
                    colName.Append("@" + insertColums[len - 2].Name);
                    parms[len - 2] = new SqlParameter(colName.ToString(), safetyFactorTypeID);
                    colName.Clear();
                    colName.Append("@" + insertColums[len - 1].Name);
                    parms[len - 1] = new SqlParameter(colName.ToString(), acqTime);
                    colName.Clear();

                    // endif
                }
                else
                {
                    var msg = new StringBuilder(tableName);
                    msg.Append("参数个数不匹配");
                    throw new Exception(msg.ToString());
                }

                return parms;
            } // endif

            return null;
        }

        /// <summary>
        /// The insert data values.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="datavalues">
        /// The datavalues.
        /// </param>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="safetyFactorTypeID">
        /// The safety factor type id.
        /// </param>
        /// <param name="acqTime">
        /// The acq time.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public static void InsertDataValues(string tableName, float[] datavalues, int sensorId, int safetyFactorTypeID,DateTime acqTime)
        {
            try
            {
                string sqlinsert = GetSqlstr(tableName);
                sqlinsert += " ;SELECT @@IDENTITY";

                SqlParameter[] parameters = GetSqlParameters(
                    tableName,
                    datavalues,
                    sensorId,
                    safetyFactorTypeID,
                    acqTime);
                DbHelperSQL.ExecuteSql(sqlinsert, parameters);
                // DbHelperSQL.ExecuteCommand(sqlinsert, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// The get safety factor type.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        private static DataTable GetSafetyFactorType()
        {
            var sqlstr =
                "Select [SAFETY_FACTOR_TYPE_ID],[THEMES_TABLE_NAME] from [T_DIM_SAFETY_FACTOR_TYPE] where THEMES_TABLE_NAME is not null";
           DataSet dst = DbHelperSQL.Query(sqlstr);
            return dst.Tables[0];
        }

        /// <summary>
        /// The get device info.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        private static DataTable GetDeviceInfo()
        {
            var sqlstr =
                "Select [STRUCT_ID],[SENSOR_ID],[SAFETY_FACTOR_TYPE_ID],[REMOTE_DTU_NUMBER],[MODULE_NO],[DAI_CHANNEL_NUMBER] from [V_DEVICE_INFO] where MODULE_NO is not null";
            DataSet dst = DbHelperSQL.Query(sqlstr);
            return dst.Tables[0];
        }

        //  { structureId, safeTypeId, dtuId, moduleNo, channelNo }
        public static int GetSensorID(int projectId, int safetyFactorTypeID, string dtuId,string moduleNo, int channelNo)
        {
            if (moduleIDSensors == null)
            {
                InitializeModuleIDSensors();
            }

            if (moduleIDSensors == null || (moduleIDSensors != null && moduleIDSensors.Count < 0))
            {
                throw new Exception("没有找到模块号和传感器ID的相关信息");
            }

            var obj = new object[] { projectId, safetyFactorTypeID, dtuId, moduleNo, channelNo };
            foreach (int k in moduleIDSensors.Keys)
            {
                bool isequql = false;
                for (int i = 0; i < moduleIDSensors[k].Length; i++)
                {
                    isequql = moduleIDSensors[k][i].Equals(obj[i]);
                    if (!isequql)
                    {
                        break;
                    }
                }

                if (isequql)
                {
                    return k;
                }
            }

            var emsg = new StringBuilder();
            emsg.Append("未找到该模块号和传感器ID相关的信息;");
            emsg.Append("结构物ID:")
                .Append(projectId)
                .Append("监测类型ID:")
                .Append(safetyFactorTypeID)
                .Append("DTUID:")
                .Append(dtuId)
                .Append("模块号：")
                .Append(moduleNo)
                .Append("通道号：")
                .Append(channelNo);
            throw new Exception(emsg.ToString());
        }

        /// <summary>
        /// The get table name.
        /// </summary>
        /// <param name="safetyFactorTypeID">
        /// The safety factor type id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static string GetTableName(int safetyFactorTypeID)
        {
            if (safetyFactorTypeIDTableNames == null)
            {
                InitializeSafetyFactorTypeIDTableNames();
            }

            if (safetyFactorTypeIDTableNames == null && (safetyFactorTypeIDTableNames != null || safetyFactorTypeIDTableNames.Count < 0))
            {
                throw new Exception("没有找到与safetyFactorTypeID对应表的相关信息");
            }

            if (safetyFactorTypeIDTableNames.ContainsKey(safetyFactorTypeID))
            {
                return safetyFactorTypeIDTableNames[safetyFactorTypeID];
            }
            var emsg = new StringBuilder();
            emsg.Append("没有找到与safetyFactorTypeID对应表的相关信息;");
            emsg.Append("监测类型ID:").Append(safetyFactorTypeID);
            throw new Exception(emsg.ToString());
        }
        
        /// <summary>
        /// The initialize safety factor type id table names.
        /// </summary>
        private static void InitializeSafetyFactorTypeIDTableNames()
        {
            if (safetyFactorTypeIDTableNames == null)
            {
                safetyFactorTypeIDTableNames = new Dictionary<int, string>();
            }
            else
            {
                safetyFactorTypeIDTableNames.Clear();
            }

            using (DataTable dtSafetyFactorType = GetSafetyFactorType())
            {
                if (dtSafetyFactorType != null && dtSafetyFactorType.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtSafetyFactorType.Rows)
                    {
                        if (dr["SAFETY_FACTOR_TYPE_ID"] != DBNull.Value && dr["THEMES_TABLE_NAME"] != DBNull.Value)
                        {
                            int safetyFactorTypeID = Convert.ToInt32(dr["SAFETY_FACTOR_TYPE_ID"]);
                            string tableName = dr["THEMES_TABLE_NAME"].ToString().Trim();
                            safetyFactorTypeIDTableNames.Add(safetyFactorTypeID, tableName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The initialize module id sensors.
        /// </summary>
        private static void InitializeModuleIDSensors()
        {
            if (moduleIDSensors == null)
            {
                moduleIDSensors=new Dictionary<int, object[]>();
            }
            else
            {
                moduleIDSensors.Clear();
            }

            using (DataTable dtDeviceInfo=GetDeviceInfo())
            {
                if (dtDeviceInfo != null && dtDeviceInfo.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtDeviceInfo.Rows)
                    {
                        if (dr["SENSOR_ID"] != DBNull.Value && dr["MODULE_NO"] != DBNull.Value && dr["SAFETY_FACTOR_TYPE_ID"] != DBNull.Value)
                        {
                            int structureId = Convert.ToInt32(dr["STRUCT_ID"]);
                            int sensorId = Convert.ToInt32(dr["SENSOR_ID"]);
                            int safeTypeId = Convert.ToInt32(dr["SAFETY_FACTOR_TYPE_ID"]);
                            string dtuId = dr["REMOTE_DTU_NUMBER"].ToString();
                            string moduleNo = dr["MODULE_NO"].ToString().Trim();
                            int channelNo = 1;
                            if (dr["DAI_CHANNEL_NUMBER"] != DBNull.Value)
                            {
                                channelNo = Convert.ToInt32(dr["DAI_CHANNEL_NUMBER"]);
                            }

                            moduleIDSensors.Add(
                                sensorId, new object[] { structureId, safeTypeId, dtuId, moduleNo, channelNo });
                        }
                    }
                }
            }
        }

        public static void InsertOrigalData(int sensorId,DateTime acqTime,float[] values)
        {
            string field = "Value";
            StringBuilder sqlstr = new StringBuilder("Insert into T_DATA_ORIGINAL (SensorId,CollectTime,");
            int count = values.Length;
            for (int i = 1; i <= count; i++)
            {
                sqlstr.Append(field).Append(i);
                if (i < count)
                {
                    sqlstr.Append(",");
                }
            }
            sqlstr.Append(") Values (@SensorId,@CollectTime,");
            for (int i = 1; i <= count; i++)
            {
                sqlstr.Append("@").Append(field).Append(i);
                if (i < count)
                {
                    sqlstr.Append(",");
                }
            }
            sqlstr.Append(")");

            SqlParameter[] para = new SqlParameter[count+2];
            para[0] = new SqlParameter("@SensorId",sensorId);
            para[1] = new SqlParameter("@CollectTime",acqTime);
            for (int i = 2; i < count+2; i++)
            {
               string fieldstr = string.Format("@{0}{1}", field, i - 1);
               para[i] = new SqlParameter(fieldstr, values[i - 2]);
            }
            DbHelperSQL.ExecuteCommand(sqlstr.ToString(), para);
        }

        public static void CopyDateToDataBase(DataTable table)
        {
            DbHelperSQL.ImportDataIntoDb(table);
        }

        public static void UpdateDTUStatus(DTUConnectionEventArgs args)
        {
            try
            {
                DataSet ds =
                    DbHelperSQL.Query(string.Format("(select ID from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER ='{0}')",
                        args.DtuId));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    Log.InfoFormat("DTU {0} 不是一个有效的DTU", args.DtuId);
                else
                {
                    string sqlstr =
                        string.Format(
                            "insert into T_DIM_DTU_STATUS (DtuId,Status,[Time]) values ({0},{1},'{2:yyyy-MM-dd HH:mm:ss.fff}')",
                            (string.Format("(select ID from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER ='{0}')",
                                args.DtuId)),
                            args.Status == ReceiveType.Online ? 1 : 0, args.Time);
                    DbHelperSQL.ExecuteSql(sqlstr);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Up dtu status error : {0}", ex.Message);
            }
        }
        
        internal static DataSet GetAllSensorData(List<int> senlst, DateTime dateTime, DateTime thisTime)
        {
            StringBuilder senstr=new StringBuilder();
            senstr.Append("0");
            foreach (int i in senlst)
            {
                senstr.AppendFormat(",{0}", i);
            }
            string sqlstr =
                string.Format(
                    "select * from T_DATA_ORIGINAL where CollectTime > '{0:yyyy-MM-dd HH:mm:ss.fff}' and CollectTime < '{1:yyyy-MM-dd HH:mm:ss.fff}' ",
                    dateTime, thisTime);
           return DbHelperSQL.Query(sqlstr);
        }

        private const string WarntypeStr = "10001005";
        private const int SenTypeCodeId = 2;
        private const string ContentStr = "传感器数据中断";

        internal static void InsertWarningMsg(int structId, int senId)
        {
            try
            {
                string sqlstr =
                    string.Format(
                        "insert into T_WARNING_SENSOR (WarningTypeId,DeviceTypeId,StructId,DeviceId,Content,Time) values ('{0}',{1},{2},{3},'{4}','{5:yyyy-MM-dd HH:mm:ss.fff}') ",
                        WarntypeStr, SenTypeCodeId, structId, senId, ContentStr, DateTime.Now);
                DbHelperSQL.ExecuteSql(sqlstr);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("InsertWarningMsg error : {0}", ex.Message);
            }
        }

        internal static void InsertWarningMsg(int structId, List<int> senIds)
        {
            try
            {
                List<string> sqlstrlst = senIds.Select(senId => string.Format("insert into T_WARNING_SENSOR (WarningTypeId,DeviceTypeId,StructId,DeviceId,Content,Time) values ('{0}',{1},{2},{3},'{4}','{5:yyyy-MM-dd HH:mm:ss.fff}') ", WarntypeStr, SenTypeCodeId, structId, senId, ContentStr, DateTime.Now)).ToList();
                DbHelperSQL.ExecuteSqlTran(sqlstrlst);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("InsertWarningMsgs error : {0}", ex.Message);
            }
        }
        
        internal static List<int> GetAllSensorIds(int structId)
        {
            List<int> lst =new List<int>();
            string sqlstr =
                string.Format(
                    "select SENSOR_ID from T_DIM_SENSOR where IsDeleted=0 and [Enable]=0 and Identification <> 2 and STRUCT_ID={0}",
                    structId);
            DataSet ds = DbHelperSQL.Query(sqlstr);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lst.AddRange(from DataRow row in ds.Tables[0].Rows select Convert.ToInt32(row["SENSOR_ID"]));
            }

            return lst;
        }

        public static void UpdateAllDtuStatus(List<string> dtus)
        {
            try
            {
                List<string> sqlstrs = new List<string>();
                foreach (string dtu in dtus)
                {
                    DataSet ds =
                       DbHelperSQL.Query(string.Format("(select ID from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER ='{0}')",
                           dtu));
                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                        Log.InfoFormat("DTU {0} 不是一个有效的DTU", dtu);
                    else
                    {
                        sqlstrs.Add(string.Format(
                            "insert into T_DIM_DTU_STATUS (DtuId,Status,[Time]) values ({0},{1},'{2:yyyy-MM-dd HH:mm:ss.fff}')",
                            (string.Format("(select ID from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER ='{0}')",
                                dtu)),
                            0, DateTime.Now));
                    }
                }
                if (sqlstrs.Count > 0)
                    DbHelperSQL.ExecuteSqlTran(sqlstrs);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("closed UpdateAllDtuStatus error: {0}", ex.Message);
            }
        }

        public static void UpdateAllDtuStatus()
        {
            try
            {
                DataSet ds = DbHelperSQL.Query(@"
select DISTINCT 
         ds.[DtuId] 
        ,ds.[Status]
        ,ds.[Time]
        ,rd.REMOTE_DTU_NUMBER
        ,rd.ProductDtuId
from [T_DIM_DTU_STATUS] ds,[T_DIM_REMOTE_DTU] rd where ds.DtuId=rd.ID and  rd.ProductDtuId =3 order by ds.[Time] desc ");

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return;
                List<int> onlinedtus = (from r in ds.Tables[0].AsEnumerable()
                    where r.Field<bool>("Status")
                    orderby r.Field<DateTime>("Time") descending
                    select r.Field<int>("DtuId")).ToList();
                List<string> sqls =
                    onlinedtus.Select(
                        i =>
                            string.Format(
                                "insert into T_DIM_DTU_STATUS (DtuId,Status,[Time]) values ({0},{1},'{2:yyyy-MM-dd HH:mm:ss.fff}')",
                                i, 0, DateTime.Now)).ToList();
                if (sqls.Count > 0)
                    DbHelperSQL.ExecuteSqlTran(sqls);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("started UpdateAllDtuStatus error: {0}", ex.Message);
            }
        }
    }
}