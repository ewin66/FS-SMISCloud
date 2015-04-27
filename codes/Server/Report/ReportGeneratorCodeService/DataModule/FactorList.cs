/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：FactorList.cs
// 功能描述：
// 
// 创建标识： 2014/9/3 14:22:04
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

namespace ReportGeneratorService.DataModule
{
    public class FactorList : MarshalByRefObject
    {
        public int StructId { get; set; }

        public int? OrgStc { get; set; }

        public int? FactorId { get; set; }

        public string FactorName { get; set; }
    }
}
