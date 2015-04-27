#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SerialPortWrapper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20140303 created by Win
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
    using System.IO.Ports;
    using System.Threading;

    /// <summary>
    /// The serial comPort wrapper.
    /// </summary>
    public class SerialPortWrapper : ICommunicationBase
    {
        /// <summary>
        /// The received data event handler.
        /// </summary>
        public event EventHandler ReceivedDataEventHandler;

        /// <summary>
        /// 心跳事件处理
        /// </summary>
        public event EventHandler HeartbeatEventHandler;

        /// <summary>
        /// 异常
        /// </summary>
        public event EventHandler ErrorEventHandler;

        /// <summary>
        /// 请求响应
        /// </summary>
        public event EventHandler RequestOrResponseEventHandler;

        private readonly SerialPort comPort;

        private int readTimeOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortWrapper"/> class.
        /// </summary>
        /// <param name="port">
        /// The comPort.
        /// </param>
        /// <param name="config">
        /// the config
        /// </param>
        internal SerialPortWrapper(string port, int[] config)
        {
            if (config.Length == 5)
            {
                this.comPort = new SerialPort(port)
                                   {
                                       BaudRate = config[0],
                                       Parity = (Parity)config[1],
                                       DataBits = config[2],
                                       StopBits = (StopBits)config[3],
                                       ReadTimeout = config[4]
                                   };
                this.readTimeOut = config[4];
            }
            else
            {
                this.comPort = new SerialPort(port) { BaudRate = 9600, ReadTimeout = 100 };
                this.readTimeOut = 100;
            }
        }

        public void UpDateConfig(string destination, int[] config)
        {
            if (this.comPort.PortName == destination)
            {
                this.comPort.BaudRate = config[0];
                this.comPort.Parity = (Parity)config[1];
                this.comPort.DataBits = config[2];
                this.comPort.StopBits = (StopBits)config[3];
                this.comPort.ReadTimeout = config[4];
                this.readTimeOut = config[4];
            }
        }

        /// <summary>
        /// The start service.
        /// </summary>
        /// <exception cref="Exception">
        /// The ex.串口不存在，串口被占用
        /// </exception>
        public void StartService()
        {
            try
            {
                if (this.comPort != null)
                {
                    if (this.comPort.IsOpen)
                    {
                        this.comPort.Close();
                    }
                    this.comPort.DataReceived += this.DataReceived;
                    this.comPort.Open();
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The stop service.
        /// </summary>
        /// <exception cref="Exception">
        /// The ex.串口不存在
        /// </exception>
        public void StopService()
        {
            try
            {
                if (this.comPort != null)
                {
                    if (this.comPort.IsOpen)
                    {
                        this.comPort.DataReceived -= this.DataReceived;
                        this.comPort.DiscardInBuffer();
                        this.comPort.DiscardOutBuffer();
                        this.ReceivedDataEventHandler = null;
                        this.comPort.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// The ex.
        /// </exception>
        public bool Send(object obj)
        {
            lock (obj)
            {
                try
                {
                    if (!this.comPort.IsOpen)
                    {
                        this.comPort.Open();
                    }

                    this.comPort.DiscardOutBuffer();
                    byte[] bytes = (byte[])obj;
                    if (bytes != null && bytes.Length > 0)
                    {
                        this.comPort.Write(bytes, 0, bytes.Length);
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// The received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// The ex.
        /// </exception>
        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int i = this.ReceiveFrame();
                if (i > 0)
                {
                    var recByteBuffer = new byte[i];
                    this.comPort.Read(recByteBuffer, 0, recByteBuffer.Length);
                    var name = ((SerialPort)sender).PortName;
                    if (recByteBuffer.Length >= 7 && recByteBuffer[0] == 0xfe && recByteBuffer[1] == 0xef)
                    {
                        switch (recByteBuffer[4])
                        {
                            case 0x00:
                                if (this.HeartbeatEventHandler != null)
                                {
                                    this.HeartbeatEventHandler(
                                        sender,
                                        new CompackageEventArgs(ReceiveType.Heartbeat, name, recByteBuffer));
                                    
                                    this.comPort.DiscardInBuffer();
                                }
                                return;
                            case 0x01:
                            case 0x02:
                                if (this.RequestOrResponseEventHandler != null)
                                {
                                    this.RequestOrResponseEventHandler(
                                        sender,
                                        new CompackageEventArgs(ReceiveType.RequestOrResponse, name, recByteBuffer));
                                    
                                    this.comPort.DiscardInBuffer();
                                }
                                return;
                            case 0x03:
                                if (this.ErrorEventHandler != null)
                                {
                                    this.ErrorEventHandler(sender,new CompackageEventArgs(ReceiveType.Error,name,recByteBuffer));
                              
                                    this.comPort.DiscardInBuffer();
                                }
                                return;
                            default:
                                if (this.ReceivedDataEventHandler != null)
                                {
                                    this.ReceivedDataEventHandler(sender, new CompackageEventArgs(ReceiveType.Working, name, recByteBuffer));
                                    
                                    this.comPort.DiscardInBuffer();
                                }
                                return;
                        }
                    }

                    if (this.ReceivedDataEventHandler != null)
                    {
                        this.ReceivedDataEventHandler(sender, new CompackageEventArgs(ReceiveType.Working, name, recByteBuffer));
                       
                        this.comPort.DiscardInBuffer();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The receive frame.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReceiveFrame()
        {
            if (this.comPort.IsOpen)
            {
                int byteCount = 0;
                try
                {
                    while (true)
                    {
                        if (this.comPort.BytesToRead > byteCount || byteCount == 0)
                        {
                            Thread.Sleep(this.readTimeOut);
                            byteCount = this.comPort.BytesToRead;
                        }
                        else
                        {
                            break;
                        }
                    }

                    return byteCount;
                }
                catch
                {
                    return byteCount;
                }
            }

            return -1;
        }
    }
}