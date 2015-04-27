using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    class RainfallSensor:Sensor
    {
        public RainfallSensor(System.Data.DataRow dr)
            : base(dr)
        {
            System.Diagnostics.Debug.Assert(SafetyFactor == SAFE_FACT.rainfall);
        }

        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            float rain = Convert.ToSingle(dr["Value1"]);
            return new DataRainfall
            {
                Safetyfactor = SafetyFactor,
                SensorId = Convert.ToInt32(Id),
                CollectTime = dr.Field<DateTime>("CollectTime"),
                rainfall = rain
            };
        }
    }
}
