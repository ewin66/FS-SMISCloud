/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：MonitorData.cs
// 功能描述：
// 
// 创建标识： 2014/10/21 17:24:53
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
// </summary>

//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace ReportGeneratorService.DataModule
{
    public class MonitorData : MarshalByRefObject
    {
        /// <summary>
        /// 传感器Id
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// 传感器位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 数据列数组
        /// </summary>
        public List<string> Columns { get; set; }

        /// <summary>
        /// 单位 
        /// </summary>
        public List<string> Unit { get; set; }

        public List<Data> Data { get; set; }
    }
}
