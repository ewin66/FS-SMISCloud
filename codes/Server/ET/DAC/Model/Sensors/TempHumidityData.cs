using System;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model.Sensors
{
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
            get { return new [] { Temperature,Humidity}; }
        }

        public override double[] PhyValues
        {
            get { return new [] { Temperature, Humidity }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { Temperature, Humidity }; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            this._themsValues[colphyindex] = null;
        }
    }
}
