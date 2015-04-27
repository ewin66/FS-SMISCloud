/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：FactorMapping.cs
// 功能描述：
// 
// 创建标识： 2014/9/3 13:30:44
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
   public  class FactorMapping : MarshalByRefObject
    {
        public int FactorId { get; set; }

        public IEnumerable<int> Sensors { get; set; }
    }
}
