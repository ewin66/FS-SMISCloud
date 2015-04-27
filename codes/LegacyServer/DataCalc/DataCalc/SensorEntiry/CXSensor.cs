using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    class CXSensor:Sensor
    {
        public CXSensor(System.Data.DataRow dr)
            : base(dr)
        {
            int.TryParse(dr["MODULE_NO"].ToString(), out _moudleNo);
            float.TryParse(dr["Parameter1"].ToString(), out _xInit);
            float.TryParse(dr["Parameter2"].ToString(), out _yInit);
            if (!float.TryParse(dr["Parameter3"].ToString(), out _spacingLength))
                _spacingLength = 250;
        }

        #region Variants
        private int _moudleNo;
        private float _xInit;
        private float _yInit;
        private float _spacingLength;
        #endregion

        #region Methods
        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            float xangle = Convert.ToSingle(dr["Value1"]);
            float yangle = Convert.ToSingle(dr["Value2"]);
            if (SafetyFactor == SAFE_FACT.deep_displace)    // 内部位移
            {
                var xValue = (_spacingLength * Math.Sin(xangle * Math.PI / 180))
                             - (_spacingLength * Math.Sin(_xInit * Math.PI / 180));
                var yValue = (_spacingLength * Math.Sin(yangle * Math.PI / 180))
                             - (_spacingLength * Math.Sin(_yInit * Math.PI / 180));
                return new DataDeepDisplacement
                {
                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    XDisplacement = xValue,
                    YDisplacement = yValue
                };
            }
            else if (SafetyFactor == SAFE_FACT.bridge_incline)
            {
                return new DataBridgeIncline
                {
                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    xangle = xangle - _xInit,
                    yangle = yangle - _yInit
                };
            }
            else
            {
                throw new Exception("当前传感器指定检测因素下的计算未实现");
            }
        }
        #endregion
    }
}
