// --------------------------------------------------------------------------------------------
// <copyright file="RequestDataCalcMessage.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：请求二次计算的消息
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
    /// 用于请求二次计算的消息
    /// </summary>
    [Serializable]
    public class RequestDataCalcMessage : MessageBase
    {
        /// <summary>
        /// 结构物编号
        /// </summary>
        public int StructId { get; set; }

        /// <summary>
        /// 采集轮数流水号
        /// </summary>
        public string RoundNum { get; set; }

        /// <summary>
        /// 原始数据文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 振动传感器ID
        /// </summary>
        public int SensorID { get; set; }
    }
}