#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="MsgDealHelper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150306 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGET
{
    using System;
    using System.Reflection;

    using FS.Service;

    using log4net;

    public class MsgDealHelper
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void DealMsg(FsMessage msg,NGEtService service)
        {
            try
            {
                if (msg == null)
                {
                    this._log.Error("pull.OnMessageReceived, msg is NULL!");
                    return;
                }
                //if (msg.Header.R == @"/et/dtu/instant/dac")
                //{
                //    InstantCol(msg, service);
                //}
                //if (msg.Header.R.StartsWith(@"/et/dtu/instant/at"))
                //{
                //    InstantAtCommand(msg, service);
                //}
                //if (msg.Header.R.StartsWith(@"/et/config"))
                //{
                //    ColInfoConfigChanged(msg, service);
                //}
            }
            catch (Exception ce)
            {
                this._log.ErrorFormat("err {0}!", ce.Message);
            }
        }
        
        public string DealMsg(string buff, NGEtService service)
        {
            FsMessage msg = FsMessage.FromJson(buff);
            //if (String.Compare(msg.Header.R, "/et/status/structs/abnormalsensorCount", StringComparison.OrdinalIgnoreCase) == 0)
            //{
            //    return GetAbnormalSensorCount(msg, service);
            //}
            //if (String.Compare(msg.Header.R, "/et/status/sensors",
            //        StringComparison.OrdinalIgnoreCase) == 0)
            //{
            //    return GetSensorStatus(msg, service);
            //}
            //if (String.Compare(msg.Header.R, "/et/status/struct/allSensors",
            //    StringComparison.OrdinalIgnoreCase) == 0)
            //{
            //    return GetAllSensors(msg, service);
            //}
            msg.Body = string.Empty;
            return msg.ToJson();
        }
    }
}