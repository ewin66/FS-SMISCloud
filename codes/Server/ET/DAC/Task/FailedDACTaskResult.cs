

using System;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model.Sensors;

namespace FS.SMIS_Cloud.DAC.Task
{
    [Serializable]
    public class FailedDACTaskResult : DACTaskResult
    {
        public FailedDACTaskResult(int errCode,DACTask t)
            : base()
        {
            base.ErrorCode = errCode;
            base.ErrorMsg = "FAILED";
            Task = t;
            GetJsonresult(t, errCode);
        }

        private void GetJsonresult(DACTask t, int errcode)
        {
            if (t.Sensors != null)
                foreach (var s in t.Sensors)
                {
                    AddSensorResult(new SensorAcqResult
                    {
                        Data = new SensorErrorData(s, errcode)
                    });
                }
        }

    }
}
