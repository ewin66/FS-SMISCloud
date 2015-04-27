// --------------------------------------------------------------------------------------------
// <copyright file="WarningHelper.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：告警处理类
// 
// 创建标识：刘歆毅20140429
// 
// 修改标识：刘歆毅20141110
// 修改描述：用于DAC.DataAnalyze告警发送
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System.Reflection;
using FS.SMIS_Cloud.Services.Messages;
using log4net;

namespace FS.SMIS_Cloud.DAC.DataAnalyzer.Warning
{
    using System;
    using System.Linq;

    using FS.Service;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Model;

    using Newtonsoft.Json;

    /// <summary>
    /// 告警处理类
    /// </summary>
    public class WarningHelper
    {
        // 设备类型
        private const int DEVICETYPEID = 2;
        // 告警服务名称
        private const string WarningAppName = "WarningManagementProcess";

        public static Service Service;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 发送传感器阈值告警
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <param name="structId"></param>
        /// <param name="thresholdAlarm"></param>
        public static void SendWarning(int sensorId, int structId, ThresholdAlarm thresholdAlarm)
        {
            var msg = new RequestWarningReceivedMessage
            {
                Id = Guid.NewGuid(),
                StructId = structId,
                DeviceTypeId = DEVICETYPEID,
                DeviceId = sensorId,
                WarningTypeId = GetWarningTypeId(thresholdAlarm.AlarmDetails.Min(d => d.ThresholdLevel)),
                WarningTime = DateTime.Now,
                WarningContent =
                    thresholdAlarm.ToString(),
                DateTime = DateTime.Now
            };

            var warningMsg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/warning/sensor",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = WarningAppName,
                    M = "Warning"
                },
                Body = JsonConvert.SerializeObject(msg)
            };
            try
            {
                Service.Push(warningMsg.Header.D, warningMsg);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("push msg failed ,msg : {0}, error : {1}", warningMsg.ToJson(), ex);
            }
            
        }

        private static string GetWarningTypeId(int level)
        {
            switch (level)
            {
                case 1:
                    return "10001001";//"102001";
                case 2:
                    return "10001002";//"102002";
                case 3:
                    return "10001003";//"102003";
                case 4:
                    return "10001004";//"102004";
                default:
                    return "10001004"; //"102004";
            }
        }
    }
}