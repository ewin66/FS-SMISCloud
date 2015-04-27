#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="GroupType.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141124 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FS.SMIS_Cloud.NGET.DataCalc.Model
{
    public enum GroupType
    {
        /// <summary>
        /// 虚拟传感器
        /// </summary>
        VirtualSensor=0,

        /// <summary>
        /// 测斜 1
        /// </summary>
        Inclination = 1,

        /// <summary>
        /// 沉降 2
        /// </summary>
        Settlement = 2,

        /// <summary>
        /// 浸润线 3
        /// </summary>
        SaturationLine = 3,
    }
}