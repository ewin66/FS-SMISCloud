// --------------------------------------------------------------------------------------------
// <copyright file="CxGroup.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20140522
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;

namespace ReportGeneratorService.DataModule
{
    public class CxGroup : MarshalByRefObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal[] Depth { get; set; }
    }
}