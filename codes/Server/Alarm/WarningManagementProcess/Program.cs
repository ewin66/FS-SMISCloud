// --------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：告警进程通信服务
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
using System;
using System.Configuration;
using System.Reflection;
using FS.Service;
using FS.SMIS_Cloud.Alarm.Forwarder.Config;
using FS.SMIS_Cloud.Alarm.Forwarder.Impl;
using FreeSun.FS_SMISCloud.Server.WarningManagementProcess.Communication;
using log4net;

namespace FreeSun.FS_SMISCloud.Server.WarningManagementProcess
{

    //using Communication;

    /// <summary>
    /// 告警管理主线程
    /// </summary>
    internal class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());

        /// <summary>
        /// 主函数
        /// </summary>
        /// <param name="args">args</param>
        public static void Main(string[] args)
        {
            ////ForwardingService 
            try
            {
                object config = ConfigurationManager.GetSection("forwarderService");
                var serviceConfig = config as ForwarderServiceConfig;
                if (serviceConfig == null)
                {
                    logger.Info("Invaild configuration, service start abnormal.");
                    return;
                }
                var service = new ForwarderService();
                service.Init(serviceConfig);
                service.Start();
            }
            catch (Exception e)
            {
                logger.Info("Service start abnormal.", e);
            }
            try
            {
                WarningService service = new WarningService("WarningService.xml", AppDomain.CurrentDomain.BaseDirectory);
                new ServiceRunner(service).Run("WarningService");
            }
            catch (Exception ex)
            {
                logger.FatalFormat("ERROR DESCRIBE: {0} \n\r", ex.Message);

            }
            Console.ReadLine();
        }
    }
}
