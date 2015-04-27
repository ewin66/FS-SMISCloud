using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FS.SMIS_Cloud.Services.Messages;
using SMIS.Utils.DB;
using WarningManagementProcess.Communication;

namespace FS.SMIS_Cloud.WarningManagementProcess.DataAccess
{
    public class DbAccessor
    {
        // id
        private static ConcurrentDictionary<string, DtuInfos> dtus = new ConcurrentDictionary<string, DtuInfos>();

        #region 处理订阅的告警

        public static bool Exists(int devicetype,int deviceId)
        {
            bool ishas = false;
            if (devicetype == 1)
            {
                string sqlstr = string.Format("select REMOTE_DTU_NUMBER from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER={0}",
                deviceId);
                ishas = DbHelperSQL.Exists(sqlstr);
            }
            else
            {
                string sqlstr = string.Format("select SENSOR_ID from T_DIM_SENSOR where SENSOR_ID={0}",
                deviceId);
                ishas = DbHelperSQL.Exists(sqlstr);
            }
            return ishas;
        }

        public static int SaveWarningMsg(RequestWarningReceivedMessage msg)
        {
            string sqlstr =
                string.Format(
                    "insert into T_WARNING_SENSOR (WarningTypeId, StructId, DeviceTypeId, DeviceId, Content, Time) values ('{0}',{1},{2},{3},'{4}','{5}')",
                    msg.WarningTypeId, msg.StructId, msg.DeviceTypeId, msg.DeviceId, msg.WarningContent,
                    msg.WarningTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return DbHelperSQL.ExecuteSql(sqlstr);
        }
        public static int SaveWarningMsg(RequestWarningReceivedMessage msg,int warningstatus,int dealflag)
        {
            string sqlstr =
                string.Format(
                    "insert into T_WARNING_SENSOR (WarningTypeId, StructId, DeviceTypeId, DeviceId, Content, Time,WarningStatus,DealFlag) values ('{0}',{1},{2},{3},'{4}','{5}',{6},{7})",
                    msg.WarningTypeId, msg.StructId, msg.DeviceTypeId, msg.DeviceId, msg.WarningContent,
                    msg.WarningTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),warningstatus,dealflag);
            return DbHelperSQL.ExecuteSql(sqlstr);
        }
        /// <summary>
        /// dtu状态
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int UpdateDtuState(DTUStatusChangedMsg msg)
        {
            if (!dtus.ContainsKey(msg.DTUID))
            {
                string sql = string.Format(@"SELECT [T_DIM_REMOTE_DTU].[ID],[T_DIM_REMOTE_DTU].[REMOTE_DTU_NUMBER],[T_DIM_STRUCT_DTU].StructureId FROM [dbo].[T_DIM_REMOTE_DTU] inner join [T_DIM_STRUCT_DTU] on [T_DIM_STRUCT_DTU].DtuId=[T_DIM_REMOTE_DTU].[ID] and [T_DIM_REMOTE_DTU].[REMOTE_DTU_NUMBER]={0}", msg.DTUID);
                DataSet ds = DbHelperSQL.Query(sql);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                    return 0;
                var structIds = new StringBuilder();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    structIds.Append(row["StructureId"]).Append(",");
                }
                int dtuid = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]);
                var dtu =new DtuInfos
                {
                    DtuNum = msg.DTUID,
                    Dtuid = dtuid,
                    StructIds = structIds.ToString()
                };
                dtus.TryAdd(msg.DTUID, dtu);
            }

            if (!msg.IsOnline)
            {

                string[] structids = dtus[msg.DTUID].StructIds.Split(new[] {' ', ',', '，'},
                    StringSplitOptions.RemoveEmptyEntries);
                var sqlstrs = new List<string>();
                foreach (string structid in structids)
                {
                    sqlstrs.Add(
                        string.Format(
                            "insert into T_WARNING_SENSOR (WarningTypeId, StructId, DeviceTypeId, DeviceId, Content, Time) values ('{0}',{1},{2},{3},'{4}','{5}')",
                            msg.WarningTypeId, Convert.ToInt32(structid), msg.DeviceTypeId, msg.DTUID,
                            msg.WarningContent,
                            msg.TimeStatusChanged.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                }

                return DbHelperSQL.ExecuteSqlTran(sqlstrs);
            }
            return 0;
        }

        public static int UpdateDtuState(DTUStatusChangedMsg msg,int warningstatus,int dealflag)
        {
            if (!dtus.ContainsKey(msg.DTUID))
            {
                string sql = string.Format(@"SELECT [T_DIM_REMOTE_DTU].[ID],[T_DIM_REMOTE_DTU].[REMOTE_DTU_NUMBER],[T_DIM_STRUCT_DTU].StructureId FROM [dbo].[T_DIM_REMOTE_DTU] inner join [T_DIM_STRUCT_DTU] on [T_DIM_STRUCT_DTU].DtuId=[T_DIM_REMOTE_DTU].[ID] and [T_DIM_REMOTE_DTU].[REMOTE_DTU_NUMBER]={0}", msg.DTUID);
                DataSet ds = DbHelperSQL.Query(sql);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                    return 0;
                var structIds = new StringBuilder();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    structIds.Append(row["StructureId"]).Append(",");
                }
                int dtuid = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]);
                var dtu = new DtuInfos
                {
                    DtuNum = msg.DTUID,
                    Dtuid = dtuid,
                    StructIds = structIds.ToString()
                };
                dtus.TryAdd(msg.DTUID, dtu);
            }

            if (!msg.IsOnline)
            {
                string[] structids = dtus[msg.DTUID].StructIds.Split(new[] { ' ', ',', '，' },
     StringSplitOptions.RemoveEmptyEntries);
                var sqlstrs = new List<string>();
                foreach (string structid in structids)
                {
                    sqlstrs.Add(
                        string.Format(
                            "insert into T_WARNING_SENSOR (WarningTypeId, StructId, DeviceTypeId, DeviceId, Content, Time,WarningStatus,DealFlag) values ('{0}',{1},{2},{3},'{4}','{5}',{6},{7})",
                            msg.WarningTypeId, Convert.ToInt32(structid), msg.DeviceTypeId, msg.DTUID,
                            msg.WarningContent,
                            msg.TimeStatusChanged.ToString("yyyy-MM-dd HH:mm:ss.fff"), warningstatus, dealflag));
                }

                return DbHelperSQL.ExecuteSqlTran(sqlstrs); 
            }
            return 0;
        }

        /// <summary>
        /// 数据连续性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int UpdateDataState(DataContinuWarningMsg request)
        {
            if (!request.DataStatus)
            {
                string sqlstr =
                    string.Format(
                        "insert into T_DIM_SENSOR_DATASTATUS (SensorId,DataStatus,LastAcquisitionTime) values ({0},{1},'{2}')",
                        request.DeviceId, 1, request.DateTime);
                string sqlstr2 =
                       string.Format(
                           "insert into T_WARNING_SENSOR (WarningTypeId, StructId, DeviceTypeId, DeviceId, Content, Time, WarningStatus, DealFlag) values ('{0}',{1},{2},{3},'{4}','{5}','0','False')",
                           request.WarningTypeId, request.StructId, request.DeviceTypeId, request.DeviceId, request.WarningContent,
                           request.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                var sqlstrs = new List<string> { sqlstr, sqlstr2 };
                return DbHelperSQL.ExecuteSqlTran(sqlstrs);
            }
            else
            {
                string sql =
                    string.Format(
                        "select top {0} Id from T_DIM_SENSOR_DATASTATUS where SensorId={1} and DataStatus=1 order by LastAcquisitionTime desc", 1,
                        request.DeviceId);
                object obj = DbHelperSQL.GetSingle(sql);
                int index = Convert.ToInt32(obj);
                if (index > 0)
                {
                    string sqlstr =
                    string.Format(
                        "Update T_DIM_SENSOR_DATASTATUS set CurrentAcquisitionTime='{0}',DataStatus={1} where Id={2} ",
                        request.DateTime, 0, index);
                    return DbHelperSQL.ExecuteSql(sqlstr);
                }
            }
            return 0;
        }

        public static int UpdateDataState(DataContinuWarningMsg request,int warningstatus,int dealflag)
        {
            if (!request.DataStatus)
            {
                string sqlstr =
                    string.Format(
                        "insert into T_DIM_SENSOR_DATASTATUS (SensorId,DataStatus,LastAcquisitionTime) values ({0},{1},'{2}')",
                        request.DeviceId, 1, request.DateTime);
                string sqlstr2 =
                       string.Format(
                           "insert into T_WARNING_SENSOR (WarningTypeId, StructId, DeviceTypeId, DeviceId, Content, Time, WarningStatus, DealFlag) values ('{0}',{1},{2},{3},'{4}','{5}',{6},{7})",
                           request.WarningTypeId, request.StructId, request.DeviceTypeId, request.DeviceId, request.WarningContent,
                           request.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),warningstatus,dealflag);
                var sqlstrs = new List<string> { sqlstr, sqlstr2 };
                return DbHelperSQL.ExecuteSqlTran(sqlstrs);
            }
            else
            {
                string sql =
                    string.Format(
                        "select top {0} Id from T_DIM_SENSOR_DATASTATUS where SensorId={1} and DataStatus=1 order by LastAcquisitionTime desc", 1,
                        request.DeviceId);
                object obj = DbHelperSQL.GetSingle(sql);
                int index = Convert.ToInt32(obj);
                if (index > 0)
                {
                    string sqlstr =
                    string.Format(
                        "Update T_DIM_SENSOR_DATASTATUS set CurrentAcquisitionTime='{0}',DataStatus={1} where Id={2} ",
                        request.DateTime, 0, index);
                    return DbHelperSQL.ExecuteSql(sqlstr);
                }
            }
            return 0;
        }

        #endregion 处理订阅的告警

    }
}