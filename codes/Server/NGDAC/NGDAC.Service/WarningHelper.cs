namespace FS.SMIS_Cloud.NGDAC
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using FS.DbHelper;
    using FS.Service;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.Services.Messages;

    using log4net;

    using Newtonsoft.Json;

    using DbType = FS.DbHelper.DbType;

    public class WarningHelper
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int DTUDEVICETYPEID = 1;
        static string _warningAppName = "WarningManagementProcess"; 

        public WarningHelper()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("MQWarningAppName"))
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MQWarningAppName"]))
                {
                    _warningAppName = ConfigurationManager.AppSettings["MQWarningAppName"];
                }
            }
        }

        public FsMessage GetDtuStatusMsg(DTUConnectionStatusChangedMsg msg)
        {
            try
            {
                string cs = ConfigurationManager.AppSettings["SecureCloud"];
                ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
                DataSet ds =
                    sqlHelper.Query(string.Format("(select ID from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER ='{0}')",
                        msg.DTUID));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    this._log.InfoFormat("DTU {0} 不是一个有效的DTU", msg.DTUID);
                else
                {
                    string sqlstr =
                        string.Format(
                            "insert into T_DIM_DTU_STATUS (DtuId,Status,[Time]) values ({0},{1},'{2:yyyy-MM-dd HH:mm:ss.fff}')",
                            (string.Format("(select ID from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER ='{0}')", msg.DTUID)),
                            msg.IsOnline ? 1 : 0, DateTime.Now);
                    sqlHelper.ExecuteSql(sqlstr);
                }
            }
            catch (Exception ex)
            {
                this._log.ErrorFormat("Up dtu status error : {0}", ex.Message);
            }
            if (msg.IsOnline) return null;
            var dtustateChangedMsg = new DTUStatusChangedMsg
            {
                Id = Guid.NewGuid(),
                WarningTypeId = ((int)Errors.ERR_NOT_CONNECTED).ToString(),
                DeviceTypeId = DTUDEVICETYPEID,
                DTUID = msg.DTUID,
                IsOnline = msg.IsOnline,
                TimeStatusChanged = msg.TimeStatusChanged,
                WarningContent=msg.IsOnline?"DTU上线":"DTU下线",
                DateTime = DateTime.Now
            };
            var warningmsg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/warning/dtu",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D=_warningAppName,
                    M = "Warning"
                },
                Body = JsonConvert.SerializeObject(dtustateChangedMsg)
            };
            return warningmsg;
        }

        public void UpdateDtuStatus(List<string> dtus)
        {
            try
            {
                List<string> sqlstrs = new List<string>();
                string cs = ConfigurationManager.AppSettings["SecureCloud"];
                ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
                foreach (string dtu in dtus)
                {
                    DataSet ds =
                       sqlHelper.Query(string.Format("(select ID from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER ='{0}')",
                           dtu));
                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                        this._log.InfoFormat("DTU {0} 不是一个有效的DTU", dtu);
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
                    sqlHelper.ExecuteSqlTran(sqlstrs);
            }
            catch (Exception ex)
            {
                this._log.ErrorFormat("closed UpdateAllDtuStatus error: {0}", ex.Message);
            }
        }

        public void UpdateAllDtuStatus()
        {
            try
            {
                string cs = ConfigurationManager.AppSettings["SecureCloud"];
                ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
                DataSet ds = sqlHelper.Query(@"
select DISTINCT 
         ds.[DtuId] 
        ,ds.[Status]
        ,ds.[Time]
        ,rd.REMOTE_DTU_NUMBER
        ,rd.ProductDtuId
from [T_DIM_DTU_STATUS] ds,[T_DIM_REMOTE_DTU] rd where ds.DtuId=rd.ID and rd.ProductDtuId =1 order by ds.[Time] desc ");
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
                    sqlHelper.ExecuteSqlTran(sqls);
            }
            catch (Exception ex)
            {
                this._log.ErrorFormat("started UpdateAllDtuStatus error: {0}", ex.Message);
            }
        }
    }
}