#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SaturationLineAlgorithm.cs" company="江苏飞尚安全监测咨询有限公司">
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

namespace FS.SMIS_Cloud.NGET.DataCalc.Algorithm
{
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGET.DataCalc.Model;
    using FS.SMIS_Cloud.NGET.Model;

    internal class SaturationLineAlgorithm : BaseAlgorithm
    {
        private SensorGroup _sensorGroup;

        public SaturationLineAlgorithm(SensorGroup group)
        {
            this._sensorGroup = group;
            this.AlgorithmName = AlgorithmNames.SaturationLineAlgo;
        }

        public override void CalcData(IList<SensorAcqResult> sensordatas)
        {
        }
    }
}