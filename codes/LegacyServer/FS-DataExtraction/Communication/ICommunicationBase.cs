#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="ICommunicationBase.cs" company="江苏飞尚安全监测咨询有限公司">
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

    /// <summary>
    /// The communication base.
    /// </summary>
    public interface ICommunicationBase
    {
        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Send(object obj);

        /// <summary>
        /// The start service.
        /// </summary>
        void StartService();

        /// <summary>
        /// The stop serice.
        /// </summary>
        void StopService();

        void UpDateConfig(string destination, int[] config);

        /// <summary>
        /// The received data event handler.
        /// </summary>
        event EventHandler ReceivedDataEventHandler;

        /// <summary>
        /// 心跳
        /// </summary>
        event EventHandler HeartbeatEventHandler;

        /// <summary>
        /// 异常
        /// </summary>
        event EventHandler ErrorEventHandler;

        /// <summary>
        /// 请求响应
        /// </summary>
        event EventHandler RequestOrResponseEventHandler;
    }
}