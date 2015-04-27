namespace FS.SMIS_Cloud.NGET
{
    using System;

    using FS.SMIS_Cloud.NGET.Model;

    using log4net;

    public class DataStatusJudge
    {
        private Service.Service service;

        private static readonly ILog Log = LogManager.GetLogger("EtDataStatusConsumer");

        private WarningHelper warningHelper = new WarningHelper();

        private static string _warningAppName = "WarningManagementProcess";

        public DataStatusJudge(Service.Service service)
        {
            this.service = service;
        }

        public bool JudgeDataStatusIsOk(SensorAcqResult sensorAcqResult)
        {
            if (!(sensorAcqResult.IsOK && sensorAcqResult.Data != null && sensorAcqResult.ErrorCode == 0))
            {
                try
                {
                    var msgStatus = this.warningHelper.GetSensorMsg(sensorAcqResult);
                    if (msgStatus != null) this.service.Push(msgStatus.Header.D, msgStatus);
                    var msgData = this.warningHelper.DataStatusMsg(sensorAcqResult);
                    if (msgData != null) this.service.Push(msgData.Header.D, msgData);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("传感器采集超时告警推送失败 ： {0}", ex.Message);
                }
                return false;
            }

            return true;
        }
    }
}


