// --------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：传感器阈值配置
// 
// 创建标识：liuxinyi20140425
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET.DataAnalyzer.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// 传感器阈值
    /// </summary>
    public class SensorThreshold
    {
        /// <summary>
        /// 传感器编号
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// 监测项编号
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 等级数量
        /// </summary>
        public int LevelNumber { get; set; }

        /// <summary>
        /// 阈值列表
        /// </summary>
        public IList<Threshold> Thresholds { get; set; }
    }

    public class Threshold
    {
        /// <summary>
        /// 阈值等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 下限
        /// </summary>
        public double? Down { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        public double? Up { get; set; }
    }
}