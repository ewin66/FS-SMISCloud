/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：PutChartToWordByFactor.cs
// 功能描述：
// 
// 创建标识： 2014/9/4 17:28:17
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

namespace ReportGeneratorService.DataModule
{
   public  class ChartByFactor : MarshalByRefObject
    {
       public  int factorId { get; set; }
       public string factorName { get; set; }
       public List<StreamChart> ChartStreams { get; set; }
    }
}
