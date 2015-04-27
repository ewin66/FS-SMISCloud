using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using System.Data;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    class ZXSensor:Sensor
    {
        public ZXSensor(System.Data.DataRow dr)
            : base(dr)
        {
            int.TryParse(dr["MODULE_NO"].ToString(), out _moudleNo);
            int.TryParse(dr["DAI_CHANNEL_NUMBER"].ToString(), out _channelNo);

            if (Formaulaid == 11) // 干滩计算公式
            {

             
                float.TryParse(dr["Parameter1"].ToString(), out _H);
                float.TryParse(dr["Parameter2"].ToString(), out _h);
                float.TryParse(dr["Parameter3"].ToString(), out _kParam);
                float.TryParse(dr["Parameter4"].ToString(), out _iniFrequency);
                float.TryParse(dr["Parameter5"].ToString(), out _sloperatio);
              
            }
            else
            {
                float.TryParse(dr["Parameter1"].ToString(), out _kParam);
                float.TryParse(dr["Parameter2"].ToString(), out _iniFrequency);
                float.TryParse(dr["Parameter3"].ToString(), out _cParam);
                float.TryParse(dr["Parameter4"].ToString(), out _iniTemperature);
                if (dr["Parameter5"] != DBNull.Value)
                {
                    this._waterDeep = float.Parse(dr["Parameter5"].ToString());
                }
            }
        }

        #region Properties
        private int _moudleNo;
        public int MoudleNo
        {
            get { return _moudleNo; }
            set { _moudleNo = value; }
        }
        private int _channelNo;
        public int ChannelNo
        {
            get { return _channelNo; }
            set { _channelNo = value; }
        }
        private float _iniFrequency;
        public float IniFrequency
        {
            get { return _iniFrequency; }
            set { _iniFrequency = value; }
        }
        private float _iniTemperature;
        public float IniTemperature
        {
            get { return _iniTemperature; }
            set { _iniTemperature = value; }
        }
        private float _kParam;
        public float KParam
        {
            get { return _kParam; }
            set { _kParam = value; }
        }
        private float _cParam;
        public float CParam
        {
            get { return _cParam; }
            set { _cParam = value; }
        }
        private float _waterDeep;
        public float WaterDeep
        {
            get{return _waterDeep;}
            set{_waterDeep = value;}
        }
        /// <summary>
        /// 滩顶高程H
        /// </summary>
        private float _H;
        public float HBeachTop { get; set; }
        /// <summary>
        /// 安装高程h
        /// </summary>
        private float _h;
        public float HInstall { get; set; }
        /// <summary>
        /// 坡比
        /// </summary>
        private float _sloperatio;
        public float SlopeRatio
        {
            get { return _sloperatio; }
            set { _sloperatio = value; }
        }
        #endregion

        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            double phy, waterlevel;
            if (Formaulaid == 1)    // 振弦公式：含温度修正
            {
                var fre = Convert.ToSingle(dr["Value1"]);
                var tmp = Convert.ToSingle(dr["Value2"]);
                phy = _kParam * (Math.Pow(fre, 2) - Math.Pow(_iniFrequency, 2)) + _cParam * (tmp - _iniTemperature);    // MPa
                waterlevel = phy * 1000 / 9.8;  // m
            }
            else    // 7 水位公式   11 干滩长度计算
            {
                var fre = Convert.ToSingle(dr["Value1"]);
                phy = _kParam * (Math.Pow(_iniFrequency, 2) - Math.Pow(fre, 2));    // MPa
                waterlevel = phy * 1000 / 9.8;  // m
            }
            if (SafetyFactor == SAFE_FACT.saturation_line) // 浸润线
            {
                // 水深+安装高程
                waterlevel += _waterDeep;
                return new DataSaturationLine
                {
                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    height=waterlevel
                };
            }
            else if (SafetyFactor == SAFE_FACT.beach)   // 干滩
            {
                var beachlen = (_H - (waterlevel + _h)) / _sloperatio;
                return new DataBeachLen
                {
                    Safetyfactor = SafetyFactor,
                    SensorId = Convert.ToInt32(Id),
                    CollectTime = dr.Field<DateTime>("CollectTime"),
                    waterlevel = waterlevel,
                    beachlen = beachlen
                };
            }
            else
            {
                throw new Exception("当前传感器指定检测因素下的计算未实现");
            }
        }
    }
}
