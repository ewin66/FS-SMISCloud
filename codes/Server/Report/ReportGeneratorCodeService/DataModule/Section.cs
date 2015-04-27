/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：Section.cs
// 功能描述：
// 
// 创建标识： 2015/1/20 9:40:58
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

namespace ReportGeneratorService.DataModule
{
    public class Section : MarshalByRefObject
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int? SectionStatus { get; set; }
        public string HeapMapName { get; set; }
    }
}
