using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using System.Data;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    /// <summary>
    /// 压力变送器
    /// </summary>
    class PressureTransSensor : Sensor
    {
        public PressureTransSensor(System.Data.DataRow dr)
            : base(dr)
        {
            if (SafetyFactor == SAFE_FACT.seepage)  // 监测因素为渗流量
            {
                if (dr["Parameter1"] != DBNull.Value)
                    wiretype = (WeirType)(int)double.Parse(dr["Parameter1"].ToString());
                if (dr["Parameter2"] != DBNull.Value)
                    wirewidth = double.Parse(dr["Parameter2"].ToString());
            }
            else if (SafetyFactor == SAFE_FACT.water_level || SafetyFactor==SAFE_FACT.saturation_line) // 水位监测
            {
                if (dr["Parameter1"] != DBNull.Value)
                    initheight = double.Parse(dr["Parameter1"].ToString());
            }
        }

        // 量水堰类型
        private WeirType wiretype;
        // 量水堰底宽
        private double wirewidth;
        // 渗流量单位
        private string wireflowuint;

        // 水位初值(安装高程)
        private double initheight;

        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            float pre = Convert.ToSingle(dr["Value1"]);
            if(SafetyFactor == SAFE_FACT.seepage)
            {
                // MPa->mH2O
                float h = pre * 1000f / 9.8f;
                float seepage = 0.0f;
                switch (wiretype)
                {
                    case WeirType.Triangle:
                        seepage = CalcFlowTriangle(h);
                        break;
                    case WeirType.Rectangle:
                        seepage = CalcFlowRectangle(h);
                        break;
                    case WeirType.Trapezoid:
                        seepage = CalcFlowKeystone(h);
                        break;
                    default:
                        break;
                }
                // L/s->m3/s
                seepage *= 0.001f;
                return new DataSeepage
                {
                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    pressure = pre,
                    seepage = seepage
                };
            }
            else if (SafetyFactor == SAFE_FACT.water_level)
            {
                // MPa->mH2O
                float h = pre * 1000f / 9.8f;
                return new Data
                {

                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    DataSet=new List<double>()
                };
                //data.DataSet.Add(h - initheight);
            }
            else if (SafetyFactor == SAFE_FACT.saturation_line)
            {
                // MPa->mH2O
                double h = pre * 1000f / 9.8f + initheight;
                return new DataSaturationLine
                {
                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    height = h
                };
            }
            else
            {
                throw new Exception("当前传感器指定检测因素下的计算未实现");
            }
        }

        //计算渗流量-三角堰
        private float CalcFlowTriangle(float WaterLevel/*m*/)
        {
            double level = WaterLevel * 100;//cm
            return (float)(Math.Pow(level, 2.5) * (0.0142 - ((int)(level / 5)) * 0.0001));
        }
        //计算渗流量-矩形堰
        private float CalcFlowRectangle(float WaterLevel/*m*/)
        {
            double level = WaterLevel * 100;//cm
            return (float)(Math.Pow(level, 1.5) * wirewidth * 0.0186);
        }
        //计算渗流量-梯形堰
        private float CalcFlowKeystone(float WaterLevel/*m*/)
        {
            double level = WaterLevel * 100;//cm
            return (float)(Math.Pow(level, 1.5) * (wirewidth - 0.2 * level) * 0.01838);
        }
        //计算渗流量-未知
        private float CalcFlowUnkown(float WaterLevel/*m*/)
        {
            return 0f;
        }
        //单位换算(以上公式默认单位L/s)
        private float FlowUnitChange(float flow)
        {
            float ret = flow;
            switch (wireflowuint.ToLower())
            {
                case "m3/s":
                    ret *= 0.001f;
                    break;
                case "m3/h":
                    ret *= 3.6f;
                    break;
                case "m3/d":
                    ret *= 86.4f;
                    break;
                default:
                    break;
            }
            return ret;
        }
        public enum WeirType
        {
            /// <summary>
            /// 三角堰
            /// </summary>
            Triangle = 0,
            /// <summary>
            /// 矩形堰
            /// </summary>
            Rectangle = 1,
            /// <summary>
            /// 梯形堰
            /// </summary>
            Trapezoid = 2
        }
    }
}
