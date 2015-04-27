#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorType.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140808 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model
{
    public enum SensorCategory
    {
        /// <summary>
        /// 1电压
        /// </summary>
        Voltage=1,

        /// <summary>
        /// 2	振弦
        /// </summary>
        VibratingWire=2,

        /// <summary>
        /// 3	雨量计
        /// </summary>
        Rainfall=3,

        /// <summary>
        /// GPS
        /// </summary>
        GPS=4,

        /// <summary>
        /// 5	风速
        /// </summary>
        Wind=5,

        /// <summary>
        /// 6	磁通量
        /// </summary>
        MagneticFlux=6,

        /// <summary>
        /// 8	振动
        /// </summary>
        Vibration=8,

        /// <summary>
        /// 9	温湿度
        /// </summary>
        TemperatureHumidity=9,

        /// <summary>
        /// 11	数字液压变送器
        /// </summary>
        Hydraulic=11,

        /// <summary>
        /// 12	LVDT
        /// </summary>
        LVDT=12,
        
        /// <summary>
        /// 光栅光纤
        /// </summary>
        GratingFiber=13,

        /// <summary>
        /// 15	测斜
        /// </summary>
        Inclinometer=14
    }
}