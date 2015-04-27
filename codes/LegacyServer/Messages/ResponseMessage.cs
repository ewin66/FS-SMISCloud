// --------------------------------------------------------------------------------------------
// <copyright file="ResponseMessage.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：响应请求的消息
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

namespace FreeSun.FS_SMISCloud.Server.Common.Messages
{
    using System;

    /// <summary>
    /// 响应二次计算请求
    /// </summary>
    [Serializable]
    public class ResponseMessage : MessageBase
    {
        /// <summary>
        /// 响应状态
        /// </summary>
        public StatusCode StatusCode { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 相应的请求编号
        /// </summary>
        public Guid RequestMessageId { get; set; }
    }
}