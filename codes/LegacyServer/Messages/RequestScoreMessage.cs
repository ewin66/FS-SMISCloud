// --------------------------------------------------------------------------------------------
// <copyright file="RequestScoreMessage.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：请求评分的消息
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
    /// 请求评分的消息
    /// </summary>
    [Serializable]
    public class RequestScoreMessage : MessageBase
    {
        /// <summary>
        /// 数据流水号
        /// </summary>
        public string DataNum { get; set; }
    }
}