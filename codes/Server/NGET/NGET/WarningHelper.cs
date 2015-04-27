namespace FS.SMIS_Cloud.NGET
{
    using System;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using FS.Service;
    using FS.SMIS_Cloud.NGET.Model;
    using FS.SMIS_Cloud.NGET.Util;
    using FS.SMIS_Cloud.Services.Messages;

    using log4net;

    using Newtonsoft.Json;

    public class WarningHelper
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int SENSORDEVICETYPEID = 2;
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

        public FsMessage GetSensorMsg(SensorAcqResult result)
        {
            try
            {
                result.Sensor = DbAccessor.DbConfigAccessor.GetSensorInfo(result.Sensor.SensorID);
            }
            catch (Exception e)
            {

            }
            if (result.ErrorCode == (int)Errors.SUCCESS || result.ErrorCode == (int)Errors.ERR_DEFAULT ||
                result.ErrorCode == (int)Errors.ERR_COMPILED || result.ErrorCode == (int)Errors.ERR_CREATE_CMD)
            {
                this._log.WarnFormat("[ StructId:{2},SID:{3} -Location:{4}] error code :{0}, msg :{1}",
                    (Errors)result.ErrorCode, EnumHelper.GetDescription((Errors)result.ErrorCode),
                    result.Sensor.StructId, result.Sensor.SensorID, result.Sensor.Name);
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
            try
            {
                result.Sensor = DbAccessor.DbConfigAccessor.GetSensorInfo(result.Sensor.SensorID);
            }
            catch (Exception e)
            {

            }
            if (!this._sensorDataStatuses.ContainsKey((int) result.Sensor.SensorID))
            {
                this._sensorDataStatuses.TryAdd((int) result.Sensor.SensorID, new SensorDataStatus
                {
                    SensorId = (int) result.Sensor.SensorID,
                    StructId = (int) result.Sensor.StructId,
                    Time = result.AcqTime
                });
            }
            this._sensorDataStatuses[(int)result.Sensor.SensorID].GetSensorColResult(result);
            if (!this._sensorDataStatuses[(int)result.Sensor.SensorID].IsContinuum || this._sensorDataStatuses[(int)result.Sensor.SensorID].IsRequireWarning)
            {
                SensorDataStatus dataStatus = this._sensorDataStatuses[(int) result.Sensor.SensorID];
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
    }
}