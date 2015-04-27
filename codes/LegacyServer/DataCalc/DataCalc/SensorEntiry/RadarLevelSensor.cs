using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    /// <summary>
    /// 雷达物位计
    /// </summary>
    class RadarLevelSensor : Sensor
    {
        public RadarLevelSensor(System.Data.DataRow dr)
            : base(dr)
        {
            System.Diagnostics.Debug.Assert(SafetyFactor == SAFE_FACT.beach);
        }

        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            float emptyHeight = Convert.ToSingle(dr["Value1"]); // 空高值
            var data = new Data
            {
                Safetyfactor = SafetyFactor,
                SensorId = Convert.ToInt32(Id),
                CollectTime = dr.Field<DateTime>("CollectTime"),
                DataSet = new List<double>()
            };
            data.DataSet.Add(emptyHeight);
            return data;
        }
    }
}
