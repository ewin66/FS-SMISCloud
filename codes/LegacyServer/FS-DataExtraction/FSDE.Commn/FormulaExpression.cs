#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="FormulaExpression.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140521 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Commn
{
    public enum FormulaExpression
    {
        /// <summary>
        /// 振弦数据
        /// </summary>
        VibratingWire=1,
        
        /// <summary>
        /// 杆式测斜
        /// </summary>
        RodInclination,

        /// <summary>
        /// 磁通量
        /// </summary>
        MagneticFlux,

        /// <summary>
        /// 电压
        /// </summary>
        Voltage,

        /// <summary>
        /// 盒式测斜
        /// </summary>
        CassetteInclination,

        /// <summary>
        /// 水位
        /// </summary>
        WaterLevel,

        /// <summary>
        /// 压力
        /// </summary>
        Pressure,

        /// <summary>
        /// 通用
        /// </summary>
        CommnFormula
    }
}