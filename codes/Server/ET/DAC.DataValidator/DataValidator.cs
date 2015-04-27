#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataValidator.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141120 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：完成算法的实现
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.Consumer;
using FS.SMIS_Cloud.DAC.DataValidator.Window;
using FS.SMIS_Cloud.DAC.Task;

using System.Collections.Generic;
using DbType = FS.DbHelper.DbType;


namespace FS.SMIS_Cloud.DAC.DataValidator
{
    using FS.SMIS_Cloud.DAC.Model;
    using System.Collections.Concurrent;

    public class DataValidator : IDACTaskResultConsumer
    {
        /// <summary>
        /// 窗口的集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, ValidateWindow> ValidateWindowses =
            new ConcurrentDictionary<string, ValidateWindow>();

        public SensorType[] SensorTypeFilter { get; set; }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void ProcessResult(DACTaskResult source)
        {
            if (source == null || source.SensorResults == null)
            {
                return;
            }
            Dictionary<string, ConfigInfo> configInfos = GetConfigInfos(source);
            foreach (var sensorResult in source.SensorResults)
            {
                //传感器的过滤类型
                if (SensorTypeFilter.Contains(sensorResult.Sensor.SensorType))
                {
                    var sensor = sensorResult.Sensor;
                    if (sensor == null || sensorResult.Data == null || sensorResult.Data.ThemeValues == null)
                    {
                        continue;
                    }
                    var sensorId = sensor.SensorID.ToString();
                    for (var i = 0; i < sensorResult.Data.ThemeValues.Count; i++)
                    {
                        var keyi = sensorId + "-" + (i + 1).ToString();
                        var themeValue = sensorResult.Data.ThemeValues[i];
                        var validateWindow = ValidateWindowses.ContainsKey(keyi) ? ValidateWindowses[keyi] : null;
                        var configInfo = configInfos.ContainsKey(keyi) ? configInfos[keyi] : null;
                        RefreshValidatorWindows(validateWindow, configInfo, keyi);
                        if (themeValue != null && ValidateWindowses.ContainsKey(keyi)) //有配置信息，同时有窗口
                        {
                            var analysisValue = new AnalysisValue(Convert.ToDecimal(themeValue.Value));
                                //计算的过程中使用Decimal类型
                            ValidateWindowses[keyi].ProcessValue(analysisValue); //进行数据的验证
                            sensorResult.Data.ThemeValues[i] = analysisValue.IsValid
                                                                   ? Convert.ToDouble(analysisValue.RawValue)
                                                                   : Convert.ToDouble(analysisValue.ValidValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validateWindowse"></param>
        /// <param name="configInfo"></param>
        private void RefreshValidatorWindows(ValidateWindow validateWindowse, ConfigInfo configInfo, string windowKey)
        {
            if (validateWindowse == null) //没有窗口，没有配置
            {
                if (configInfo == null)
                {
                    return;
                }
                else
                {
                    if (configInfo.IsOpenWindow) //需要过滤
                    {
                        ValidateWindowses.TryAdd(windowKey, new ValidateWindow(configInfo));
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (configInfo == null)
                {
                    validateWindowse.IsOpenWindow = false;
                }
                else
                {
                    validateWindowse.IsOpenWindow = configInfo.IsOpenWindow; //重新赋值
                    validateWindowse.KThreshold = configInfo.KThreshold;
                    validateWindowse.DiscreteThreshold = configInfo.DiscreteThreshold;
                    validateWindowse.ReCalcRValueThreshold = configInfo.ReCalcRValueThreshold;
                    validateWindowse.WindowSize = configInfo.WindowSize;
                    validateWindowse.NeedLog = configInfo.NeedLog;
                    validateWindowse.SensorId = configInfo.SensorId;
                    validateWindowse.ValueIndex = configInfo.ValueIndex;
                }
            }
        }

        /// <summary>
        /// 获取传感器的异常数据处理配置信息
        /// </summary>
        /// <param name="source">传感器采集信息</param>
        /// <returns>相应传感器的异常数据处理配置信息</returns>
        private Dictionary<string, ConfigInfo> GetConfigInfos(DACTaskResult source)
        {
            var configInfos = new Dictionary<string, ConfigInfo>();
            if (source != null && source.SensorResults != null)
            {
                List<uint> sensorIdList =
                    source.SensorResults.Select(sensorResult => sensorResult.Sensor.SensorID).ToList();
                if (sensorIdList.Count > 0)
                {
                    try
                    {
                        string cs = ConfigurationManager.AppSettings["SecureCloud"];
                        ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);

                        string sql = string.Format(@"
select [ItemId] as factorId,
       [Enabled] as isValitor,
       [WindowSize] as windowSize,
       [KT] as KThreshold,
       [DT] as DiscreteThreshold,
       [RT] as ReCalcRValueThreshold,
       [SensorId] as SensorId,
       [NeedLog] as NeedLog
from[T_DATA_STABLE_FILTER_CONFIG] 
where SensorId in ({0})", string.Join(",", sensorIdList.ToArray()));
                        DataTable dt = sqlHelper.Query(sql).Tables[0];
                        foreach (DataRow item in dt.Rows)
                        {
                            var sensorId = item[6];
                            var keyi = sensorId + "-" + (item[0]).ToString();

                            if (item[2] == DBNull.Value || item[3] == DBNull.Value || item[4] == DBNull.Value ||
                                item[5] == DBNull.Value)
                            {
                                continue; //有一个数据为空，整条记录无效
                            }

                            if (!configInfos.ContainsKey(keyi))
                            {
                                var values = new ConfigInfo();
                                configInfos.Add(keyi, values);
                            }

                            configInfos[keyi].IsOpenWindow = Convert.ToBoolean(item[1]);
                            configInfos[keyi].WindowSize = Convert.ToInt32(item[2]);
                            configInfos[keyi].KThreshold = Convert.ToDecimal(item[3]);
                            configInfos[keyi].DiscreteThreshold = Convert.ToInt32(item[4]);
                            configInfos[keyi].ReCalcRValueThreshold = Convert.ToInt32(item[5]);
                            configInfos[keyi].NeedLog = Convert.ToBoolean(item[7]);
                            configInfos[keyi].SensorId = Convert.ToInt32(item[6]);
                            configInfos[keyi].ValueIndex = Convert.ToInt32(item[0]);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            return configInfos;
        }
    }
}