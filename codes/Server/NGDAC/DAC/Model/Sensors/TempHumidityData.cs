namespace FS.SMIS_Cloud.NGDAC.Model.Sensors
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TempHumidityData : BasicSensorData
    {
        public TempHumidityData(double t,double h)
        {
            this.Temperature = t;
            this.Humidity = h;
            this._themsValues =new List<double?>(new double?[] {t, h});
            this.IsSaveDataOriginal = true;
        }
        public double Temperature { get;private set; }
        public double Humidity { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { this.Temperature,this.Humidity}; }
        }

        public override double[] PhyValues
        {
            get { return new [] { this.Temperature, this.Humidity }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.Temperature, this.Humidity }; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            this._themsValues[colphyindex] = null;
        }
    }
}
