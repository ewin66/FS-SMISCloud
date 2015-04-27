#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorTypes.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140620 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System.Configuration;

namespace DataCenter.WirelessCustomTransmission
{
    public class SensorTypes
    {
        private SensorTypes()
        {
            var file = new ExeConfigurationFileMap { ExeConfigFilename = "WirelessReceiver.exe.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            this.Voltage = int.Parse(config.AppSettings.Settings["Voltage"].Value);
            this.Vibrating = int.Parse(config.AppSettings.Settings["Vibrating"].Value);
            this.Rainfall = int.Parse(config.AppSettings.Settings["Rainfall"].Value);
            this.GPS = int.Parse(config.AppSettings.Settings["GPS"].Value);
            this.Wind = int.Parse(config.AppSettings.Settings["Wind"].Value);
            this.MagneticFlux = int.Parse(config.AppSettings.Settings["MagneticFlux"].Value);
            this.Vibration = int.Parse(config.AppSettings.Settings["Vibration"].Value);
            this.TempHumi = int.Parse(config.AppSettings.Settings["TempHumi"].Value);
            this.HydraulicTra = int.Parse(config.AppSettings.Settings["HydraulicTransmitter"].Value);
            this.LVDT = int.Parse(config.AppSettings.Settings["LVDT"].Value);
            this.FiberGrating = int.Parse(config.AppSettings.Settings["FiberGrating"].Value);
            this.TempSpecial = int.Parse(config.AppSettings.Settings["TempSpecial"].Value);
            this.Inclinometer = int.Parse(config.AppSettings.Settings["Inclinometer"].Value);
        }

        private static readonly SensorTypes SensorType = new SensorTypes();

        public static SensorTypes GetSensorTypes()
        {
            return SensorType;
        }

        /// <summary>
        /// 电压
        /// </summary>
        public int Voltage { get; private set; }

        /// <summary>
        /// 振弦
        /// </summary>
        public int Vibrating { get; private set; }

        /// <summary>
        /// 雨量
        /// </summary>
        public int Rainfall { get; private set; }

        /// <summary>
        /// GPS
        /// </summary>
        public int GPS { get; private set; }

        /// <summary>
        /// 风速风向
        /// </summary>
        public int Wind { get; private set; }

        /// <summary>
        /// 磁通量
        /// </summary>
        public int MagneticFlux { get; private set; }

        /// <summary>
        /// 振动
        /// </summary>
        public int Vibration { get; private set; }

        /// <summary>
        /// 温湿度
        /// </summary>
        public int TempHumi { get; private set; }

        /// <summary>
        /// 液压变送器
        /// </summary>
        public int HydraulicTra { get; private set; }

        /// <summary>
        /// LVDT裂缝计
        /// </summary>
        public int LVDT { get; private set; }

        /// <summary>
        /// 光栅光纤
        /// </summary>
        public int FiberGrating { get; private set; }

        /// <summary>
        /// 温度（于家堡）
        /// </summary>
        public int TempSpecial { get; private set; }

        /// <summary>
        /// 测斜
        /// </summary>
        public int Inclinometer { get; private set; }
    }
}