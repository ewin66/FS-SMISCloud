#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="CompackageEventArgs.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140410 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace Communication
{
    using System;

    /// <summary>
    /// The receive type.
    /// </summary>
    public enum ReceiveType
    {
        Online,
        OffLine,
        Working,
        Heartbeat,
        RequestOrResponse,
        Error
    }

    /// <summary>
    /// The compackage event args.
    /// </summary>
    public class CompackageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompackageEventArgs"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="dataBytes">
        /// The data bytes.
        /// </param>
        public CompackageEventArgs(ReceiveType type, string sender, byte[] dataBytes)
        {
            this.DataType = type;
            this.Sender = sender;
            this.DataReceived = dataBytes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompackageEventArgs"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="dataBytes">
        /// The data bytes.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="phoneNumber">
        /// The phone number.
        /// </param>
        public CompackageEventArgs(ReceiveType type, string sender, byte[] dataBytes, int port, string ip, string phoneNumber)
            : this(type, sender, dataBytes)
        {
            this.IPAddress = ip;
            this.PhoneNumber = phoneNumber;
            this.Port = port;
        }

        /// <summary>
        /// Gets or sets the sensor id.
        /// </summary>
        public int SensorSetId { get; set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the ip address.
        /// </summary>
        public string IPAddress { get; private set; }

        /// <summary>
        /// Gets the phone number.
        /// </summary>
        public string PhoneNumber { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public ReceiveType DataType { get; private set; }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public string Sender { get; private set; }

        /// <summary>
        /// Gets the data received.
        /// </summary>
        public byte[] DataReceived { get; private set; }

        /// <summary>
        /// Gets or sets the acq time.
        /// </summary>
        public string AcqTime { get; set; }

        public int SafeTypeId { get; set; }

    }
}