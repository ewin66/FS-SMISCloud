#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SetWorkMode.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140909 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC.Gprs.Cmd
{
    using System;

    using Newtonsoft.Json;

    public class SetWorkMode:ATCommand
    {
        public string WorkMode { get; set; }

        /// <summary>
        /// 设置工作模式
        /// </summary>
        /// <param name="jsonStr"> {mode=TCP|TRNS|UDP} </param>
        public SetWorkMode(string jsonStr)
        {
            var anonymous = new { mode = "" };
            var jsobj = JsonConvert.DeserializeAnonymousType(jsonStr, anonymous);
            this.WorkMode = jsobj.mode;
            if (this.WorkMode != "TCP" && this.WorkMode != "TRNS" && this.WorkMode != "UDP")
            {
                throw new Exception("WorkMode is error");
            }
        }

        public string ToATString()
        {
            return string.Format("AT+MODE={0}\r", this.WorkMode);
        }
    }
}