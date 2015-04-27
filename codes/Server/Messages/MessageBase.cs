// --------------------------------------------------------------------------------------------
// <copyright file="MessageBase.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：消息基类
// 
// 创建标识：刘歆毅20140217
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;

namespace FS.SMIS_Cloud.Services.Messages
{
    /// <summary>
    /// 消息基类
    /// </summary>
    [Serializable]    
    public class MessageBase
    {
        /// <summary>
        /// 消息编号
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}