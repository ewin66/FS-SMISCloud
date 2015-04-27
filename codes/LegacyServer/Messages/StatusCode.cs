// --------------------------------------------------------------------------------------------
// <copyright file="StatusCode.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：响应状态
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
    /// <summary>
    /// 响应状态
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// 已接收
        /// </summary>
        Received = 0,

        /// <summary>
        /// 已完成请求
        /// </summary>
        Completed = 1,

        /// <summary>
        /// 完成请求时失败
        /// </summary>
        Failed = -1,

        /// <summary>
        /// 请求内容有误
        /// </summary>
        Error = -2
    }
}