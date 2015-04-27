using System;
using System.Data;

namespace ET.Common
{
    public class LoggerInfo
    {
        public LoggerInfo(){}
        public LoggerInfo(DataRow dataInfo)
        {
            ProtocolCode = dataInfo[""].ToString();
            ModuleId = dataInfo[""].ToString();

        }
         /// <summary>
        /// 位置信息
        /// </summary>
        public string AddrInfo { get; set; }

        /// <summary>
        /// 协议编号
        /// </summary>
        public string ProtocolCode { get; set; }

        /// <summary>
        /// 模块号
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// 通道号
        /// </summary>
        public int? ChannelId { get; set; }


        public PackageCode Order { get; set; }

        /// <summary>
        /// 通道总数
        /// </summary>
        public int? ChannelCount { get; set; }

        /// <summary>
        /// 初值
        /// </summary>
        public float?[] InitialValue { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public float?[] CalcParameters { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime DateOfProduction { get; set; }

        /// <summary>
        /// 设备/传感器  编号
        /// </summary>
        public string SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 传感器类型
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 广播号
        /// </summary>
        public byte? BroadCastNo { get; set; }
        
        /// <summary>
        /// 硬件版本
        /// </summary>
        public string HardwareVersion { get; set; }

        /// <summary>
        /// 软件版本
        /// </summary>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// 预留
        /// </summary>
        public object Reserved { get; set; }
    }

    //class A
    //{
    //    LoggerInfo li = new LoggerInfo();
    //    public void B()
    //    {
    //        li.ReceiveBag = null;
    //        li.SerialNumber = null;
    //    }
    //}
}
