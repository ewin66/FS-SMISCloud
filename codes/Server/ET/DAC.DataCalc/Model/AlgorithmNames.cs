#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="AlgorithmNames.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141112 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FS.SMIS_Cloud.DAC.DataCalc.Model
{
    internal class AlgorithmNames
    {
        /// <summary>
        /// 深部位移
        /// </summary>
        public const string DeepDisplaceAlgo = "DeepDisplaceAlgo";

        /// <summary>
        /// 浸润线
        /// </summary>
        public const string SaturationLineAlgo = "SaturationLineAlgo";

        /// <summary>
        /// 沉降
        /// </summary>
        public const string SettlementAlgo = "SettlementAlgo";

        /// <summary>
        /// 虚拟传感器
        /// </summary>
        public const string VirtualSensorAlgo = "VirtualSensorAlgo";
    }

    internal enum CalcPlanState
    {
        None = 0,
        AddToPlan
    }
}