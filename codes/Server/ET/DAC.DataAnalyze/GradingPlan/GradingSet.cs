// --------------------------------------------------------------------------------------------
// <copyright file="GradingSet.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：整体评分集合,用于整个结构物的评分
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
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Model;
    using FS.SMIS_Cloud.DAC.Model;

    public class GradingSet
    {
        public static void Add(Sensor sensor, SensorAnalyzeResult sensorAnalyzeResult)
        {
            var item = new GradingItem
                           {
                               SensorId = (int)sensor.SensorID,
                               StructId = (int)sensor.StructId,
                               FactorId = (int)sensor.FactorType,
                               Score = sensorAnalyzeResult.Score
                           };
            StructGradingPlanSet.UpdatePlan(item);
        }
    }
}