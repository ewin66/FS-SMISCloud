namespace FS.SMIS_Cloud.NGET
{
    using System;

    using FS.SMIS_Cloud.NGET.Model;

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
                this.IsRequireWarning = this._count >= 3;
                this._count = 0;
            }
            else
            {
                this._count++;
            }
        }
        
    }
}