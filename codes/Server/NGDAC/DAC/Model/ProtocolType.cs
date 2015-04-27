
namespace FS.SMIS_Cloud.NGDAC.Model
{
    public struct ProtocolType
    {
        /// <summary>
        /// 1100	温湿度采集协议	老版
        /// </summary>
        public const uint TempHumidity_OLD = 1100;

        /// <summary>
        /// 1101	温湿度采集协议	统一协议
        /// </summary>
        public const uint TempHumidity = 1101;

        /// <summary>
        /// 1102    温湿度采集协议	Modbus
        /// </summary>
        public const uint ModbusTempHumi = 1102;

        /// <summary>
        /// 1200	电压采集协议	NULL
        /// </summary>
        public const uint Voltage = 1200;

        //1300	LVDT采集协议	NULL

        /// <summary>
        /// 1400	振弦采集协议	老版
        /// </summary>
        public const uint VibratingWire_OLD = 1400;

        /// <summary>
        /// 1500	测斜采集协议	老版
        /// </summary>
        public const uint Inclinometer_OLD = 1500;

        /// <summary>
        /// 1600	雨量采集协议	华云
        /// </summary>
        public const uint RainFall = 1600;

        /// <summary>
        /// 1503	ModBus固定杆式测斜仪采集协议	NULL
        /// </summary>
        public const uint Inclinometer_ROD = 1503;

        /// <summary>
        /// 1502	ModBus固定盒式测斜仪采集协议	NULL
        /// </summary>
        public const uint Inclinometer_BOX = 1502;

        /// <summary>
        /// 1501	ModBus移动式测斜仪采集协议	NULL
        /// </summary>
        public const uint Inclinometer_MOBIL = 1501;

        /// <summary>
        /// 1700	磁通量采集仪	老版
        /// </summary>
        public const uint MagneticFlux = 1700;

        /// <summary>
        /// 9001	压力变送器协议	昊胜
        /// </summary>
        public const uint Pressure_HS = 9001;

        /// <summary>
        /// 9002	压力变送器协议	麦克
        /// </summary>
        public const uint Pressure_MPM = 9002;

        /// <summary>
        /// 9101	裂缝计协议	济南博林
        /// </summary>
        public const uint LVDT_BL = 9101;

        /// <summary>
        /// 9102	裂缝计协议	深圳信为
        /// </summary>
        public const uint LVDT_XW = 9102;

        /// <summary>
        /// 9201	风速风向采集仪协议	欧赛龍
        /// </summary>
        public const uint Wind_OSL = 9201;

        /// <summary>
        /// 9301	振动协议	NULL
        /// </summary>
        public const uint Vibration = 9301;

        /// <summary>
        /// 9401	GPS	中海达
        /// </summary>
        public const uint GPS_ZHD = 9401;

        /// <summary>
        /// 9402    GPS 华测
        /// </summary>
        public const uint GPS_HC = 9402;

        /// <summary>
        /// 9403    GPS 司南
        /// </summary>
        public const uint GPS_SN = 9403;

        /// <summary>
        /// 1401	Modbus振弦协议	NULL
        /// </summary>
        public const uint VibratingWire = 1401;

        /// <summary>
        /// 9501    FTM50S激光测距仪 NULL
        /// </summary>
        public const uint FTM50SSLaser = 9501;

        /// <summary>
        /// 上海激光测距传感器
        /// </summary>
        public const uint GLSB40I70Laser = 9502;

        /// <summary>
        /// 9202  风速风向  深圳智翔宇
        /// </summary>
        public const uint Wind_CCF = 9202;

    }
}
