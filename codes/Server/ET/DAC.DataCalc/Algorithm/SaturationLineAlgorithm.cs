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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace FS.SMIS_Cloud.DAC.DataCalc.Algorithm
{
    internal class SaturationLineAlgorithm : BaseAlgorithm
    {
        private SensorGroup _sensorGroup;

        public SaturationLineAlgorithm(SensorGroup group)
        {
            this._sensorGroup = group;
            this.AlgorithmName = AlgorithmNames.SaturationLineAlgo;
        }

        public override bool CalcData(IList<SensorAcqResult> sensordatas)
        {
            return true;
        }
    }
}