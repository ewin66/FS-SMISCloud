// --------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：告警进程通信服务
// 
// 创建标识：lingwenlong 20141028
// 
// 修改标识： netmq 服务
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
using log4net;
using Newtonsoft.Json;

namespace FreeSun.FS_SMISCloud.Server.WarningManagementProcess.Communication
{
    class WarningService : Service
    {
        private static readonly string MyName = ConfigurationManager.AppSettings["myName"];
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        private WarningManagementSever warningmm;
       

        public WarningService(string svrConfig, string busPath)
            : base(MyName, busPath+svrConfig, busPath)
        {
            warningmm = new WarningManagementSever();
            Pull(null, OnMessageReceived);
        }

        public override void PowerOn()
        {
            warningmm.Start();
        }

        private void OnMessageReceived(string buff)
        {
            try
            {
                FsMessage msg = FsMessage.FromJson(buff);
                if (msg == null)
                {
                    log.Error("pull.OnMessageReceived, msg is NULL!");
                    return; 
                }
                string sender = msg.Header.S;
                var warningmsg = msg.Body.ToString();
                warningmm.AddWarningMsg(sender,msg.Header.R, warningmsg);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error {0}:", ex.Message);
            }
        }

        
    }
}