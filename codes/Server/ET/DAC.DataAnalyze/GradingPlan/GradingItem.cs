﻿// --------------------------------------------------------------------------------------------
// <copyright file="GradingItem.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141110
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.DAC.DataAnalyzer.GradingPlan
{
    public class GradingItem
    {
        public int SensorId { get; set; }

        public int StructId { get; set; }

        public int FactorId { get; set; }

        public int Score { get; set; }
    }
}