﻿// --------------------------------------------------------------------------------------------
// <copyright file="SensorData.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141030
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.DAC.DataAnalyzer.Model
{
    using System.Collections.Generic;

    public class AnalyzingData
    {
        public uint SensorId { get; set; }

        public IList<double?> Data { get; set; }
    }
}