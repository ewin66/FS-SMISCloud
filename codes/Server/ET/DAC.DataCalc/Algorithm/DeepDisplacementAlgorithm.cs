#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DeepDisplacementAlgorithm.cs" company="江苏飞尚安全监测咨询有限公司">
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
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using log4net;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace FS.SMIS_Cloud.DAC.DataCalc.Algorithm
{
    internal class DeepDisplacementAlgorithm : BaseAlgorithm
    {
        private readonly SensorGroup _sensorGroup = null;

        private readonly ILog _log = LogManager.GetLogger("DeepDisplacementAlgorithm");
        
        public DeepDisplacementAlgorithm(SensorGroup group)
        {
            this._sensorGroup = group;
            this.AlgorithmName = AlgorithmNames.DeepDisplaceAlgo;
        }

        /// <summary>
        /// 深部位移/Deepdisplacement(倾斜/Inclination)
        /// </summary>
        public override bool CalcData(IList<SensorAcqResult> sensordatas)
        {
            var groupItems = (from s in _sensorGroup.Items
                              orderby s.Paramters["DEPTH"]
                              select s).ToList();
            double? depthInclinationX = 0;
            double? depthInclinationY = 0;
            for (int i = 0; i < groupItems.Count; i++)
            {
                if (groupItems[i].Value == null)
                {
                    _log.WarnFormat("深部累积位移-{0}-计算异常:组内数据不完整", _sensorGroup.GroupId);
                    continue;
                }
                var sensorAcqResult = groupItems[i].Value;
                depthInclinationX += sensorAcqResult.Data.ThemeValues[0];
                depthInclinationY += sensorAcqResult.Data.ThemeValues[1];
                sensorAcqResult.Data.ThemeValues.Add(depthInclinationX);
                sensorAcqResult.Data.ThemeValues.Add(depthInclinationY);
            }
            return true;
        }
    }
}