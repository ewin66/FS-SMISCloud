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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FS.SMIS_Cloud.DAC.Consumer;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Accessor;
using FS.SMIS_Cloud.DAC.DataCalc.Algorithm;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.DataCalc.Plan;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Task;
using log4net;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace FS.SMIS_Cloud.DAC.DataCalc
{
    public class DataCalc : IDACTaskResultConsumer
    {
        private readonly MsDbAccessorAdv _dbAccessor = new MsDbAccessorAdv(ConfigurationManager.AppSettings["SecureCloud"]);

        private static readonly ILog _logger = LogManager.GetLogger("DataCalc");

        public SensorType[] SensorTypeFilter { get; set; }

        private readonly static object LockObj = new object();

        public void ProcessResult(DACTaskResult source)
        {
            _logger.Info("DataCalc has recieved DACTaskResult, starts to calculate..");
            StartCalc(source);
        }

        private void StartCalc(DACTaskResult source)
        {
            if (source.IsOK)
            {
                uint dtuid = source.Task.DtuID;
                IList<SensorGroup> groups = _dbAccessor.QuerySensorGroupsByDtuid(dtuid);
                if (groups == null || groups.Count == 0)
                    return;

                _logger.InfoFormat("DataCalc find DTU_{0}'s groups: {1}", source.DtuCode, string.Join(",", groups));
                var calcedcnt = Calc(source, groups);
                _logger.InfoFormat("DataCalc finished, calculate successed: {0}", calcedcnt);
            }
        }

        private static int Calc(DACTaskResult source, IEnumerable<SensorGroup> groups)
        {
            lock (LockObj)
            {
                int calccnt = 0;
                var sensordatum = source.SensorResults;
                foreach (SensorGroup gp in groups)
                {
                    try
                    {
                        if (gp.HasMultiDtuJob())
                        {
                            var plancalccnt = CalcPlanSet.Update(source.Task.DtuID, gp, sensordatum, source.Task.TID);
                            calccnt += plancalccnt;
                            continue;
                        }

                        var items = gp.GetAllItems();
                        foreach (var groupItem in items)
                        {
                            var sensordata =
                                (from s in sensordatum where s.Sensor.SensorID == groupItem.SensorId select s)
                                    .FirstOrDefault();
                            if (sensordata != null && sensordata.IsOK && sensordata.Data != null && sensordata.Data.ThemeValues != null)
                            {
                                groupItem.Value = sensordata;
                            }
                        }
                        if (Calc(gp, sensordatum))
                        {
                            calccnt++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnFormat("二次计算出现异常 GROUP_ID={0} GROUP_TYPE={1} ERR={2}", gp.GroupId, gp.GroupType, ex.Message);
                    }
                }
                foreach (var sensorAcqResult in source.SensorResults)
                {
                    if (sensorAcqResult.CalcPlanState == (int)CalcPlanState.AddToPlan)
                    {
                        sensorAcqResult.ErrorCode = (int)Errors.ERR_DEFAULT;
                    }
                }
                return calccnt;
            }
        }

        public static bool Calc(SensorGroup gp, IList<SensorAcqResult> acqs)
        {
            var alv = gp.GetVirtualItems();
            if (alv.Any())
            {
                foreach (var groupItem in alv)
                {
                    if (groupItem.VirtualGroup.GroupType == GroupType.VirtualSensor)
                    {
                        AlgorithmFactory.CreateAlgorithm(groupItem.VirtualGroup).CalcData(acqs);

                        groupItem.Value = (from acq in acqs
                                           where acq.Sensor.SensorID == groupItem.VirtualGroup.VirtualSensor.SensorID
                                           select acq).FirstOrDefault();
                    }
                }

                return AlgorithmFactory.CreateAlgorithm(gp).CalcData(acqs);
            }
            else
            {
                return AlgorithmFactory.CreateAlgorithm(gp).CalcData(acqs);
            }
        }
    }
}