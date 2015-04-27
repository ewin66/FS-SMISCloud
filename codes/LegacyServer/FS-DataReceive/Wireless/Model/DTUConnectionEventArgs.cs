#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DTUConnectionEventArgs.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150325 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using DataCenter.Util;

namespace DataCenter.Model
{
    public class DTUConnectionEventArgs
    {
        public string DtuId { get; set; }

        public string Ip { get; set; }

        public string PhoneNumber { get; set; }

        public ReceiveType Status { get; set; }

        public DateTime RefreshTime { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime Time { get; set; }
    }
}