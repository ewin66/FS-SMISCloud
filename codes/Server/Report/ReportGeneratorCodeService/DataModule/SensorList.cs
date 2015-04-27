/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：SensorList.cs
// 功能描述：
// 
// 创建标识： 2014/9/2 14:54:18
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
    public  class SensorList : MarshalByRefObject
    {
        public string SensorType { get; set; }

        public List<Sensor> Sensors { get; set; }
    }
}
