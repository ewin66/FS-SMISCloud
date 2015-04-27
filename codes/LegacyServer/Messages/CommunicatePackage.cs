// // --------------------------------------------------------------------------------------------
// // <copyright file="CommunicatePackage.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// // 定义通信层、协议解析层、采集控制间用到的消息定义
// // 创建标识：xusuwei 20140228
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.Common.Messages.CommunicatePackage
{
    using System;

    /// <summary>
    /// 定义消息包类型
    /// </summary>
    [Serializable]
    public enum PackageType : byte
    {
        /// <summary>
        /// 端口控制包
        /// </summary>
        PortControl = 0x01,

        /// <summary>
        /// 客户端连接状态指示包
        /// </summary>
        ClientState = 0x02,

        /// <summary>
        /// 数据传输包
        /// </summary>
        DataTransfer = 0x11
    }

    /// <summary>
    /// 端口控制命令类型
    /// </summary>
    [Serializable]
    public enum PortControl : byte
    {
        /// <summary>
        /// 打开端口
        /// </summary>
        Open = 1,

        /// <summary>
        /// 关闭端口
        /// </summary>
        Close = 2
    }

    /// <summary>
    /// 客户端连接状态类型
    /// </summary>
    [Serializable]
    public enum ClientState : byte
    {
        /// <summary>
        /// 上线
        /// </summary>
        OnLine = 1,

        /// <summary>
        /// 下线
        /// </summary>
        OffLine = 2
    }

    /// <summary>
    /// 端口通信方式
    /// </summary>
    [Serializable]
    public enum PortType : byte
    {
        /// <summary>
        /// Tcp通信方式
        /// </summary>
        Tcp = 1,

        /// <summary>
        /// Udp通信方式
        /// </summary>
        Udp = 2,

        /// <summary>
        /// Dtu通信方式
        /// </summary>
        Dtu = 3
    }

    /// <summary>
    /// 端口控制命令包
    /// </summary>
    [Serializable]
    public class PortControlPackage
    {
        /// <summary>
        /// 端口控制类型
        /// </summary>
        public PortControl OperateType { get; set; }

        /// <summary>
        /// 端口号.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 端口类型
        /// </summary>
        public PortType ProtType { get; set; }
    }

    /// <summary>
    /// 端口控制命令包
    /// </summary>
    [Serializable]
    public class ClientStatePackage
    {
        /// <summary>
        /// 客户端状态类型
        /// </summary>
        public ClientState State { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="state">客户端状态</param>
        /// <param name="clientId">客户端标识</param>
        public ClientStatePackage(ClientState state, string clientId)
        {
            this.ClientId = clientId;
            this.State = state;
        }
    }

    /// <summary>
    /// 定义采集控制和通信层之间的数据包
    /// </summary>
    [Serializable]
    public class DataPackage
    {
        /// <summary>
        /// 通信端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="clientId">客户端编号</param>
        /// <param name="data">数据包</param>
        public DataPackage(int port, string clientId, byte[] data)
        {
            this.ClientId = clientId;
            this.Port = port;
            this.Data = data;
        }
    }

    /// <summary>
    /// 通信用消息包
    /// </summary>
    [Serializable]
    public class CommunicatePackage
    {
        /// <summary>
        /// 通信包类型
        /// </summary>
        public PackageType Type { get; set; }

        /// <summary>
        /// 消息包实体，具体内容会根据PackageType不同而不同
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">数据包类型</param>
        /// <param name="content">数据包内容</param>
        public CommunicatePackage(PackageType type, object content)
        {
            this.Type = type;
            this.Content = content;
        }

        /// <summary>
        /// 静态数据包构造方法
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="clientId">客户端标识</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static CommunicatePackage CreateDataPackage(int port, string clientId, byte[] data)
        {
            return new CommunicatePackage(PackageType.DataTransfer, new DataPackage(port, clientId, data));
        }

        /// <summary>
        /// 静态客户端状态包构造方法
        /// </summary>
        /// <param name="state">客户端状态</param>
        /// <param name="clientId">客户端标识</param>
        /// <returns></returns>
        public static CommunicatePackage CreateClientStatePackage(ClientState state, string clientId)
        {
            return new CommunicatePackage(PackageType.ClientState, new ClientStatePackage(state, clientId));
        }

    }
}