namespace FS.SMIS_Cloud.NGDAC.Model.Sensors
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TempData : BasicSensorData
    {
        public TempData(double t)
        {
            this.Temperature = t;
            this._themsValues =new List<double?>(new double?[] {t});
            this.IsSaveDataOriginal = true;
        }

        public double Temperature { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { this.Temperature }; }
        }

        public override double[] PhyValues
        {
            get { return new [] { this.Temperature }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.Temperature }; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            if (colphyindex == 0)
            {
                this._themsValues[colphyindex] = null;
            }
        }

       
    }
}
