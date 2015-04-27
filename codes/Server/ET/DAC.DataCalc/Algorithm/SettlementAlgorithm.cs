#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SettlementAlgorithm.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Collections.Generic;
using System.Linq;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using log4net;
using log4net.Core;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace FS.SMIS_Cloud.DAC.DataCalc.Algorithm
{
    /// <summary>
    /// 沉降分组计算(减基点)
    /// </summary>
    internal class SettlementAlgorithm : BaseAlgorithm
    {
        private readonly SensorGroup _sensorGroup;

        private readonly ILog _log = LogManager.GetLogger("SettlementAlgorithm");

        public SettlementAlgorithm(SensorGroup group)
        {
            this._sensorGroup = group;
            this.AlgorithmName = AlgorithmNames.SettlementAlgo;
        }

        public override bool CalcData(IList<SensorAcqResult> sensordatas)
        {
            var basicItem = (from s in _sensorGroup.Items
                             where Convert.ToByte(s.Paramters["IsBase"]) == 1
                             select s).FirstOrDefault();
            if (basicItem == null)
            {
                _log.WarnFormat("沉降分组配置异常，没有找到基点.GROUPID={0}", _sensorGroup.GroupId);
                return false;
            }
            if (basicItem.Value == null)
            {
                foreach (var groupItem in _sensorGroup.Items)   // 该组内所有测点数据无效
                {
                    if (groupItem.Value != null)
                    {
                        groupItem.Value.ErrorCode = (int)Errors.ERR_NOBASE_SETTLEMENT;
                    }
                }
                _log.WarnFormat("沉降基点无数据.GROUPID={0}", _sensorGroup.GroupId);
                return false;
            }
            var basicValue = basicItem.Value.Data.ThemeValues[0];
            foreach (var groupItem in this._sensorGroup.Items)
            {
                if (groupItem.Value != null)
                {
                    groupItem.Value.Data.ThemeValues[0] = basicValue - groupItem.Value.Data.ThemeValues[0];
                }
            }
            return true;
        }
    }
}