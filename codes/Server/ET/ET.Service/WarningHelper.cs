using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using FS.Service;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.Services.Messages;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.Util;
using log4net;
using DbType = FS.DbHelper.DbType;

namespace FS.SMIS_Cloud.ET
{
    public class WarningHelper
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int SENSORDEVICETYPEID = 2;
        private const int DTUDEVICETYPEID = 1;
        private string SENSORWARNINGTYPE = "003";
        private string DTUStATUSWARNINGTYPE = "001";
        static string _warningAppName = "WarningManagementProcess";

        private ConcurrentDictionary<int, SensorDataStatus> _sensorDataStatuses =
            new ConcurrentDictionary<int, SensorDataStatus>(); 

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
                    _log.InfoFormat("DTU {0} 不是一个有效的DTU", msg.DTUID);
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
                _log.ErrorFormat("Up dtu status error : {0}", ex.Message);
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

        public FsMessage GetSensorMsg(SensorAcqResult result)
        {
            if (result.ErrorCode == (int) Errors.SUCCESS || result.ErrorCode == (int) Errors.ERR_DEFAULT ||
                result.ErrorCode == (int) Errors.ERR_COMPILED || result.ErrorCode == (int) Errors.ERR_CREATE_CMD)
            {
                this._log.WarnFormat("[ StructId:{2},SID:{3} -M:{4} -C:{5} -Location:{6}] error code :{0}, msg :{1}",
                    (Errors) result.ErrorCode, EnumHelper.GetDescription((Errors) result.ErrorCode),
                    result.Sensor.StructId, result.Sensor.SensorID, result.Sensor.ModuleNo, result.Sensor.ChannelNo,
                    result.Sensor.Name);
                return null;
            }

            var msg = new RequestWarningReceivedMessage
            {
                Id = Guid.NewGuid(),
                StructId = (int)result.Sensor.StructId,
                DeviceTypeId = SENSORDEVICETYPEID,
                DeviceId = (int)result.Sensor.SensorID,
                WarningTypeId = result.ErrorCode.ToString(),
                WarningTime = DateTime.Now,
                DateTime = DateTime.Now,
                WarningContent = EnumHelper.GetDescription((Errors)result.ErrorCode) //string.Format(result.Response == null ? "采集超时" : result.ErrorMsg)
            };
            var warningmsg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/warning/sensor",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = _warningAppName,
                    M = "Warning"
                },
                Body = JsonConvert.SerializeObject(msg)
            };
            return warningmsg;
        }

        public FsMessage DataStatusMsg(SensorAcqResult result)
        {
            if (!_sensorDataStatuses.ContainsKey((int) result.Sensor.SensorID))
            {
                this._sensorDataStatuses.TryAdd((int) result.Sensor.SensorID, new SensorDataStatus
                {
                    SensorId = (int) result.Sensor.SensorID,
                    StructId = (int) result.Sensor.StructId,
                    Time = result.ResponseTime
                });
            }
            _sensorDataStatuses[(int)result.Sensor.SensorID].GetSensorColResult(result);
            if (!_sensorDataStatuses[(int)result.Sensor.SensorID].IsContinuum || _sensorDataStatuses[(int)result.Sensor.SensorID].IsRequireWarning)
            {
                SensorDataStatus dataStatus = _sensorDataStatuses[(int) result.Sensor.SensorID];
                var msg = new DataContinuWarningMsg
                {
                    Id = Guid.NewGuid(),
                    StructId = dataStatus.StructId,
                    DeviceTypeId = SENSORDEVICETYPEID,
                    DeviceId = dataStatus.SensorId,
                    WarningTypeId = dataStatus.IsContinuum ?((int)Errors.SUCCESS).ToString():((int)Errors.ERR_DATA_BREACH).ToString(),
                    WarningTime = dataStatus.Time,
                    DateTime = DateTime.Now,
                    WarningContent = string.Format(dataStatus.IsContinuum ? "数据恢复" : EnumHelper.GetDescription(Errors.ERR_DATA_BREACH)),
                    DataStatus = dataStatus.IsContinuum
                };
                var warningmsg = new FsMessage
                {
                    Header = new FsMessageHeader
                    {
                        A = "PUT",
                        R = "/warning/datacontinu",
                        U = Guid.NewGuid(),
                        T = Guid.NewGuid(),
                        D = _warningAppName,
                        M = "Warning"
                    },
                    Body = JsonConvert.SerializeObject(msg)
                };
                return warningmsg;
            }
            return null;
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
                        _log.InfoFormat("DTU {0} 不是一个有效的DTU", dtu);
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
                _log.ErrorFormat("closed UpdateAllDtuStatus error: {0}", ex.Message);
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