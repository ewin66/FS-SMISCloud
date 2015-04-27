using System;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Node;

namespace FS.SMIS_Cloud.ET
{
    public class SensorDataStatus
    {
        public SensorDataStatus()
        {
            this._count = 0;
        }

        public int SensorId { get; set; }

        public int StructId { get; set; }

        private int _count;

        public DateTime Time { get; set; }

        public bool IsRequireWarning { get; private set; }

        public bool IsContinuum
        {
            get
            {
                if (this._count >= 3)
                {
                    return false;
                }
                return true;
            }
        }

        public void GetSensorColResult(SensorAcqResult sensorAcqResult)
        {
            if (sensorAcqResult.Data != null)
            {
                IsRequireWarning = _count >= 3;
                this._count = 0;
            }
            else
            {
                this._count++;
            }
        }
        
    }
}