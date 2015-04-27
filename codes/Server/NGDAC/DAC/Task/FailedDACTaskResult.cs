using System;

namespace FS.SMIS_Cloud.NGDAC.Task
{
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model.Sensors;
    [Serializable]
    public class FailedDACTaskResult : DACTaskResult
    {
        public FailedDACTaskResult(int errCode,DACTask t)
            : base()
        {
            base.ErrorCode = errCode;
            base.ErrorMsg = "FAILED";
            this.Task = t;
            this.GetJsonresult(t, errCode);
        }

        private void GetJsonresult(DACTask t, int errcode)
        {
            if (t.Sensors != null)
                foreach (var s in t.Sensors)
                {
                    this.AddSensorResult(new SensorAcqResult
                    {
                        Data = new SensorErrorData(s, errcode)
                    });
                }
        }

    }
}
