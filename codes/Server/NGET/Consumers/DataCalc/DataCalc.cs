#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataCache.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141111 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGET.DataCalc
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    using FS.SMIS_Cloud.NGET.Consumer;
    using FS.SMIS_Cloud.NGET.DataCalc.Accessor;
    using FS.SMIS_Cloud.NGET.DataCalc.Model;
    using FS.SMIS_Cloud.NGET.DataCalc.Plan;
    using FS.SMIS_Cloud.NGET.Model;

    using log4net;

    public class DataCalc : IDacTaskResultConsumer
    {
        private readonly MsDbAccessorAdv _dbAccessor = new MsDbAccessorAdv(ConfigurationManager.AppSettings["SecureCloud"]);

        private static readonly ILog Log = LogManager.GetLogger("DataCalc");

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            this.StartCalc(source);
        }

        private void StartCalc(List<SensorAcqResult> source)
        {
            IList<SensorGroup> groups = this._dbAccessor.QueryGroups();
            if (groups == null || groups.Count == 0)
                return;
            Calc(source, groups);
        }

        private static void Calc(List<SensorAcqResult> source, IList<SensorGroup> groups)
        {
            try
            {
                source = CalcPlanSet.Update(groups, source);
            }
            catch (Exception ex)
            {
                Log.FatalFormat("二次计算异常:{0}", ex.Message);
            }
        }
    }
}