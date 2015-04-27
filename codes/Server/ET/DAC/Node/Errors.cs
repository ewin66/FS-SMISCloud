using System;
using System.ComponentModel;


namespace FS.SMIS_Cloud.DAC.Node
{
    [Serializable]
    public enum Errors : int
    {

        #region ERRORCODES

        [Description("成功")]
        SUCCESS = 0,

        [Description("默认值")]
        ERR_DEFAULT = 1000,

        [Description("发送数据为空")]
        ERR_NULL_SEND_DATA = 1001,

        [Description("写串口异常")]
        ERR_WRITE_COM = 1002,

        [Description("DTU未连接")]
        ERR_NOT_CONNECTED = 1003,

        [Description("DTU忙碌")]
        ERR_DTU_BUSY = 1004,

        [Description("采集超时")]
        ERR_DTU_TIMEOUT = 1005,

        [Description("协议错误")]
        ERR_PROTOCOL_ERROR = 1006,

        [Description("解析数据失败")]
        ERR_DATA_PARSEFAILED = 1007,

        [Description("接收数据为空")]
        ERR_NULL_RECEIVED_DATA = 1008,

        [Description("不受支持的协议")]
        ERR_UNKNOW_PROTOCOL = 1009,

        [Description("无效DTU")]
        ERR_INVALID_DTU = 1010,

        [Description("无效数据")]
        ERR_INVALID_DATA = 1011,

        [Description("沉降分组计算没有基点数据")]
        ERR_NOBASE_SETTLEMENT = 1012,

        [Description("模块号错误")]
        ERR_INVALID_MODULE = 1014, 

        [Description("编译错误")]
        ERR_COMPILED = 1016,

        [Description("采集命令构造错误")]
        ERR_CREATE_CMD = 1017,

        [Description("该传感器不参与采集")]
        ERR_NOT_COLLECTED = 1018,

        [Description("找不到该传感器信息")]
        ERR_NOT_CONFIG=1019,

        [Description("未知异常")]
        ERR_UNKNOW = 1099,
        

        /// <summary>
        /// Modbus错误
        /// </summary>

        [Description("单总线忙")]
        ERR_ONE_WIRE_BUSY = 100101, // 设备返回 1

        [Description("单总线短路检测")]
        ERR_ONE_WIRE_SHORT_CIRCUIT = 100102, // 设备返回 2 

        [Description("校验码错误")]
        ERR_CRC = 100103, // 设备返回 3

        [Description("功能码错误")]
        ERR_CMD = 100104, // 设备返回 4

        [Description("项目号错误")]
        ERR_PROJECT_NO = 100105, // 设备返回 5

        [Description("广播号错误")]
        ERR_BROADCAST_NO = 100106, // 设备返回 6

        [Description("读取传感器信息错误")]
        ERR_READ_SENSOR_INFO = 100107, // 设备返回 7

        [Description("读取设备信息错误")]
        ERR_READ_RTU_INFO = 100108, // 设备返回 8

        [Description("采集时间错误")]
        ERR_COLL_TIME = 100109, // 设备返回 9

        [Description("通道号错误")]
        ERR_RECEIVED_CH = 100110, // 设备返回 10

        [Description("采集的频率异常")]
        ERR_COLL_FREQ = 100111, //设备返回 11

        [Description("采集的温度异常")]
        ERR_COLL_TEMP = 100112, // 设备返回 12

        [Description("采集的湿度异常")]
        ERR_COLL_HUMI = 100113, // 设备返回 13

        [Description("采集的电压异常")]
        ERR_COLL_VOL = 100114, // 设备返回 14

        [Description("采集的角度异常")]
        ERR_COLL_ANGLE = 100115, // 设备返回 15

        [Description("设备自动采集")]
        ERR_RTU_AUTO = 100116, // 设备返回 16

        [Description("系统忙碌")]
        ERR_SYSTEM_BUSY = 100117, // 设备返回 17

        [Description("数据存储错误")]
        ERR_DATA_STORAGE = 100118, // 设备返回 18

        [Description("数据读取错误")]
        ERR_DATA_READ = 100119, // 设备返回 19

        [Description("时间设置错误")]
        ERR_SET_TIMR = 100120, // 设备返回 20

        [Description("数据存储错误")]
        ERR_SET_AUTO_TIME = 100121, // 设备返回 21

        [Description("振弦传感器断路")]
        ERR_CIRCUIT_BREAKE = 100122, // 设备返回 22

        [Description("振弦传感器短路")]
        ERR_SHORT_CIRCUIT = 100123, // 设备返回 23 

        [Description("温度传感器断路")]
        ERR_TEMP_CIRCUIT_BREAKE = 100124, // 设备返回 24

        [Description("温度传感器短路")]
        ERR_TEMP_SHORT_CIRCUIT = 100125, // 设备返回 25

        [Description("测斜仪类传感器初始角度设置错误")]
        ERR_SET_INCLINATION_INITIAL_ANGLE = 100126, // 设备返回 26

        [Description("测斜仪类传感器温补系数设置错误")]
        ERR_SET_INCLINATION_TEMPPARAM_COMP = 100127, // 设备返回 27

        [Description("磁通量传感器采集积分电压异常")]
        ERR_MAGNETICFLUX_COLL_VOLtTAGE = 100128, // 设备返回 28

        [Description("器件初始化/复位失败")]
        ERR_INI_RESET = 100129, // 设备返回 29

        [Description("I2C总线忙")]
        ERR_I2C_WIRE_BUSY = 100130, // 设备返回 30

        [Description("设备电量异常")]
        ERR_POWER = 100131, // 设备返回 31


        [Description("频率线异常")]
        ERR_VIBRW_FREWIRE = 140101, 

        [Description("温度线异常")]
        ERR_VIBRW_TEMPWIRE = 140102,

        [Description("频率线温度线都异常")]
        ERR_VIBRW_FREWIRE_TEMPWIRE = 140103,

        /// <summary>
        /// 外购传感器返回的错误
        /// </summary>
        
        [Description("华云雨量计同步时间")]
        ERR_RAINFALL_CLOCKSYNC = 160001,

        [Description("功能号错误")]
        ERR_LVDTXW_CMD = 910201,

        [Description("寄存器错误")]
        ERR_LVDTXW_EAX = 910202,

        [Description("正常操作发生异常")]
        ERR_LVDTXW_ACTION = 910203,

        [Description("激光返回错误")]
        ERR_GLS_RECEIVE = 950201,

        /// <summary>
        /// 告警
        /// </summary>
        
        [Description("传感器测量值超过达到1级告警阈值")]
        ERR_THRESHOIL_LEVEL_ONE = 10001001,

        [Description("传感器测量值超过达到2级告警阈值")]
        ERR_THRESHOIL_LEVEL_TWO = 10001002,

        [Description("传感器测量值超过达到3级告警阈值")]
        ERR_THRESHOIL_LEVEL_THREE = 10001003,

        [Description("传感器测量值超过达到4级告警阈值")]
        ERR_THRESHOIL_LEVEL_FOUR = 10001004,

        [Description("传感器数据中断")]
        ERR_DATA_BREACH = 10001005,

        [Description("采集数据超出传感器量程")]
        ERR_OUT_RANGE = 10001006,

        #endregion
    }
}
