/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：DateType.cs
// 功能描述：
// 
// 创建标识： 2014/10/21 13:43:01
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGeneratorService.ReportModule
{
    public enum DateType
    {
        /// <summary>
        /// 日
        /// </summary>
        Day = 1,
        /// <summary>
        /// 周
        /// </summary>
        Week = 2,
        /// <summary>
        /// 月
        /// </summary>
        Month = 3,
        /// <summary>
        /// 年
        /// </summary>
        Year = 4
    }
}
