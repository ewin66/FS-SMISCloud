// --------------------------------------------------------------------------------------------
// <copyright file="RequestWarningReceivedMessage.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：请求告警接收的消息
//
// 创建标识：彭玲20140415
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
    /// 用于请求告警接收的消息
    /// </summary>
    [Serializable]
    public class RequestWarningReceivedMessage : MessageBase
    {
        /// <summary>
        /// Gets or sets 告警类型ID
        /// </summary>
        public string WarningTypeId { get; set; }

        /// <summary>
        /// Gets or sets 结构物ID
        /// </summary>
        public int StructId { get; set; }

        /// <summary>
        /// Gets or sets 设备类型ID
        /// </summary>
        public int DeviceTypeId { get; set; }

        /// <summary>
        /// Gets or sets 设备ID（传感器设备“DeviceId”对应传感器表中“SENSOR_ID”；DTU设备“DeviceId”对应DTU表中“REMOTE_DTU_NUMBER”）
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets 告警级别
        /// </summary>
       // public int WarningLevel { get; set; }

        /// <summary>
        /// Gets or sets 告警内容
        /// </summary>
        public string WarningContent { get; set; }

        /// <summary>
        /// Gets or sets 告警产生时间
        /// </summary>
        public DateTime WarningTime { get; set; } 
    }
}