/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：StreamChart.cs
// 功能描述：
// 
// 创建标识： 2014/11/17 11:48:36
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
using System.IO;
using System.Linq;
using System.Text;

namespace ReportGeneratorService.DataModule
{
    public class StreamChart : MarshalByRefObject
    {
        public Stream ChartStream { get; set; }

        public string ChartType { get; set; }
    }
}
