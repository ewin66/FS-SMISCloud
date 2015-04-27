/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：SectionSensors.cs
// 功能描述：
// 
// 创建标识： 2015/1/16 15:38:29
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
   public class SectionSensors : MarshalByRefObject
   {
       public int SectionId { get; set; }
       public string SectionName { get; set; }
       public List<Sensor> Sensors { get; set; }
   }
}
