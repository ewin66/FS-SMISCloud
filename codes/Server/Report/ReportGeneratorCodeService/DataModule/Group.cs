/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：Group.cs
// 功能描述：
// 
// 创建标识： 2014/9/2 14:56:30
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
   public  class Group : MarshalByRefObject
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public decimal MaxDepth { get; set; }
    }
}
