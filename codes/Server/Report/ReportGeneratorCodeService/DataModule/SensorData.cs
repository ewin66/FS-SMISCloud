/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：SensorData.cs
// 功能描述：
// 
// 创建标识： 2014/9/2 14:55:17
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
   public  class SensorData : MarshalByRefObject
    {
        /// <summary>
        /// 传感器Id
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// 传感器位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 数据列数组
        /// </summary>
        public string[] Columns { get; set; }

        /// <summary>
        /// 单位 
        /// </summary>
        public string[] Unit { get; set; }

        /// <summary>
        /// 数值数组
        /// </summary>
        public decimal[] Values { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime AcquisitionTime { get; set; }

    }
}
