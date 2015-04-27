using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using System.Data;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    /// <summary>
    /// GPS传感器 
    /// </summary>
    class GPSSensor : Sensor
    {
        public GPSSensor(System.Data.DataRow dr)
            : base(dr)
        {
            System.Diagnostics.Debug.Assert(SafetyFactor == SAFE_FACT.surf_displace);
            if (dr["Parameter1"] != DBNull.Value)
            {
                this._XInit = float.Parse(dr["Parameter1"].ToString());
            }
            if (dr["Parameter2"] != DBNull.Value)
            {
                this._YInit = float.Parse(dr["Parameter2"].ToString());
            }
            if (dr["Parameter3"] != DBNull.Value)
            {
                this._ZInit = float.Parse(dr["Parameter3"].ToString());
            }
        }

        private float _XInit = 0.0f;
        private float _YInit = 0.0f;
        private float _ZInit = 0.0f;

        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            float x = Convert.ToSingle(dr["Value1"]);
            float y = Convert.ToSingle(dr["Value2"]);
            float z = Convert.ToSingle(dr["Value3"]);
            return new DataSurfaceDisplacement
            {
                Safetyfactor = SafetyFactor,
                SensorId = Convert.ToInt32(Id),
                CollectTime = dr.Field<DateTime>("CollectTime"),
                XDisplacement=(x-_XInit)*1000,
                YDisplacement=(y-_YInit)*1000,
                ZDisplacement=(z-_ZInit)*1000
            };
        }
    }
}
