#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorAcqResultTimeType.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150209 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FS.SMIS_Cloud.DAC.Model
{
    public enum SensorAcqResultTimeType
    {
        /// <summary>
        /// 采集任务开始时间
        /// </summary>
        TaskStartTime,
        /// <summary>
        /// 采集任务结束时间
        /// </summary>
        TaskFinishedTime,
        /// <summary>
        /// 请求传感器数据时间
        /// </summary>
        SensorRequestTime,
        /// <summary>
        /// 传感器应答时间
        /// </summary>
        SensorResponseTime
    }
}