using System;
using System.Data;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    class LvdtSensor:Sensor
    {
        public LvdtSensor(DataRow dr) : base(dr)
        {
            int.TryParse(dr["MODULE_NO"].ToString(), out _moudleNo);
            float.TryParse(dr["Parameter1"].ToString(), out _initval);
        }

        #region Variants
        private int _moudleNo;
        private float _initval;
        #endregion

        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            var displacement = Convert.ToSingle(dr["Value1"]);
            if (SAFE_FACT.bearingdisplace == SafetyFactor)  // 支座位移
            {
                return new DataSurfaceDisplacement
                {
                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    XDisplacement = displacement - _initval,
                    YDisplacement = 0,
                    ZDisplacement = 0
                };
            }
            return null;
        }
    }
}
