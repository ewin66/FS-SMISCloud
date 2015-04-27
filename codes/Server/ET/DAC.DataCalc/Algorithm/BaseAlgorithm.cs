#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="BaseAlgorithm.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.DataCalc.Algorithm
{
    internal abstract class BaseAlgorithm : IAlgorithm
    {
        public string AlgorithmName { get; set; }

        public abstract bool CalcData(IList<SensorAcqResult> sensordatas);
    }
}