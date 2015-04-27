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

namespace FS.SMIS_Cloud.NGET.DataCalc.Algorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FS.SMIS_Cloud.NGET.DataCalc.Model;
    using FS.SMIS_Cloud.NGET.Model;

    using log4net;

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

        public override void CalcData(IList<SensorAcqResult> sensordatas)
        {
            var basicItem = (from s in this._sensorGroup.Items
                             where Convert.ToByte(s.Paramters["IsBase"]) == 1
                             select s).FirstOrDefault();
            if (basicItem == null)
                throw new Exception("Incorrect settlement group config, no base point found."); // 配置中无基点信息
            if (!basicItem.Paramters.ContainsKey("value") || ((SensorAcqResult)basicItem.Paramters["value"]).Data == null)
            {
                foreach (var groupItem in this._sensorGroup.Items)   // 该组内所有测点数据无效
                {
                    if (groupItem.Paramters.ContainsKey("value") &&
                        ((SensorAcqResult) groupItem.Paramters["value"]).Data != null)
                    {
                        ((SensorAcqResult) groupItem.Paramters["value"]).ErrorCode = (int)Errors.ERR_NOBASE_SETTLEMENT;
                    }
                }
                this._log.Warn("沉降基点无数据");
                return;
            }
            var basicValue = ((SensorAcqResult)basicItem.Paramters["value"]).Data.ThemeValues[0];
            foreach (var groupItem in this._sensorGroup.Items)
            {
                if (groupItem.Paramters.ContainsKey("value"))
                {
                    var sensorAcqResult = (SensorAcqResult)groupItem.Paramters["value"];
                    if (sensorAcqResult.Data != null)
                    {
                        sensorAcqResult.Data.ThemeValues[0] = basicValue - sensorAcqResult.Data.ThemeValues[0];
                    }
                }
            }
        }
    }
}