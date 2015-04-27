

namespace ET.Common
{
    public enum PackageCode
    {
        /// <summary>
        /// 广播
        /// </summary>
        BroadCast = 0,

        /// <summary>
        /// 采集
        /// </summary>
        Collect,

        /// <summary>
        /// 读取传感器产品信息
        /// </summary>
        ReadSensorProduct,

        /// <summary>
        /// 读取传感器参数信息
        /// </summary>
        ReadSensorParam,

        /// <summary>
        /// 设置传感器产品信息
        /// </summary>
        SetSensorProduct,

        /// <summary>
        /// 设置传感器参数
        /// </summary>
        SetSensorParam,

        /// <summary>
        /// 设置传感器位置信息
        /// </summary>
        SetSensorAddr,

        /// <summary>
        /// 读取设备信息
        /// </summary>
        ReadLogger,

        /// <summary>
        /// 设置设备自动采集
        /// </summary>
        SetLoggerAuto,

        /// <summary>
        /// 设备复位
        /// </summary>
        SetLoggerReSet,

        /// <summary>
        /// 时间同步
        /// </summary>
        SetLoggerSTime,

        /// <summary>
        /// 格式化
        /// </summary>
        SetLoggerFormat,

        /// <summary>
        /// 读取数据报告
        /// </summary>
        ReadData,

        /// <summary>
        /// 其他
        /// </summary>
        Other
    }
}
