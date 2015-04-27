using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using System.Data;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    class WSDSensor:Sensor
    {
        public WSDSensor(System.Data.DataRow dr)
            : base(dr)
        {
            int.TryParse(dr["MODULE_NO"].ToString(), out _moudleNo);
        }

        private int _moudleNo;
        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            return new DataTempAndHumi
            {
                Safetyfactor = SafetyFactor,
                SensorId = Convert.ToInt32(Id),
                CollectTime = dr.Field<DateTime>("CollectTime"),
                tmperature = dr["Value2"] != DBNull.Value ? Convert.ToSingle(dr["Value2"]) : 0,
                huminity = dr["Value1"] != DBNull.Value ? Convert.ToSingle(dr["Value1"]) : 0
            };
        }
    }
}
