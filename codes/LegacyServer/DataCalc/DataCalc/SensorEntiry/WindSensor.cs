using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;


namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    class WindSensor:Sensor
    {
        public WindSensor(DataRow dr) : base(dr)
        {
        }


        public override Data CalcValue(DataRow dr)
        {
            return new DataWind
            {
                Safetyfactor = SafetyFactor,
                SensorId = Convert.ToInt32(Id),
                CollectTime = dr.Field<DateTime>("CollectTime"),
                azimuth = Convert.ToDouble(dr["Value1"]),
                speed = Convert.ToDouble(dr["Value2"]),
                elevation = dr["Value3"] != DBNull.Value ? Convert.ToDouble(dr["Value3"]) : 0
            };
        }
    }
}
