// --------------------------------------------------------------------------------------------
// <copyright file="DataAnalyzer.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：数据分析类-实现IDACTaskResultConsumer接口,作为DAC插件,用于评分入库和发送阈值告警
// 
// 创建标识：20141029
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.DAC.DataAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using FS.SMIS_Cloud.DAC.Model;

    using log4net;

    using FS.SMIS_Cloud.DAC.Consumer;
    using FS.SMIS_Cloud.DAC.DAC;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Accessor;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.GradingPlan;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Model;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Warning;
    using FS.SMIS_Cloud.DAC.Task;

    public class DataAnalyzer : IDACTaskResultConsumer
    {
        private static readonly ILog Log = LogManager.GetLogger("DataAnalyzer");

        public DataAnalyzer()
        {
            WarningHelper.Service = new WarningService(
                "DataAnalyzer",
                    "DataAnalyzerService.xml",
                    AppDomain.CurrentDomain.BaseDirectory);
            WarningHelper.Service.Start();
        }

        ~DataAnalyzer()
        {
            WarningHelper.Service.Stop();
        }

        public SensorType[] SensorTypeFilter { get; set; }

        /// <summary>
        /// 分析采集数据
        /// </summary>
        /// <param name="rslt"></param>
        public void ProcessResult(DACTaskResult rslt)
        {
            Log.Info("DataAnalyzer has recieved DACTaskResult, starts to analyze..");
            int count = 0;
            if (rslt.IsOK)
            {
                var sens = rslt.SensorResults.Select(s => s.Sensor.SensorID);
                var thresholds = this.GetSensorThreshold(sens); // 所有传感器阈值
                foreach (var sensorResult in rslt.SensorResults)
                {
                    if (sensorResult.IsOK && sensorResult.Data != null && sensorResult.ErrorCode == 0)
                    {
                        var sensor = sensorResult.Sensor;

                        var analyzingData = this.GetAnalyzingData(sensorResult);

                        if (analyzingData.Data == null || analyzingData.Data.Any(d => d == null))
                        {
                            Log.WarnFormat(
                                "data:[{0}] not meet the Analyze's condition",
                                sensorResult.Data.ThemeValues == null
                                    ? null
                                    : string.Join(",", sensorResult.Data.ThemeValues));
                            continue;
                        }

                        var sensorThreshold = thresholds.Where(s => s.SensorId == sensor.SensorID).ToList();
                        // 当前传感器阈值

                        var sensorAnalyzeResult = this.AnalyzeSensorData(analyzingData, sensorThreshold);

                        // 添加到整体评分集合
                        GradingSet.Add(sensor, sensorAnalyzeResult);
                        // 发送阈值告警
                        if (sensorAnalyzeResult.ThresholdAlarm != null
                            && sensorAnalyzeResult.ThresholdAlarm.AlarmDetails.Any())
                        {
                            try
                            {
                                Log.InfoFormat(
                                    "Sensor:{0} generate a OVER THRESHOLD alarm, sending..",
                                    sensor.SensorID);
                                WarningHelper.SendWarning(
                                    (int)sensor.SensorID,
                                    (int)sensor.StructId,
                                    sensorAnalyzeResult.ThresholdAlarm);
                                Log.InfoFormat("Sensor:{0} alarm send success", sensor.SensorID);
                            }
                            catch (Exception e)
                            {
                                Log.ErrorFormat("Sensor:{0} alarm send error", e, sensor.SensorID);
                            }
                        }

                        count++;
                    }
                }
            }
            Log.InfoFormat("Data Analyzed, success:{0}", count);
        }

        /// <summary>
        /// 数据分析(默认满分)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="thresholds"></param>
        public SensorAnalyzeResult AnalyzeSensorData(AnalyzingData data, IList<SensorThreshold> thresholds)
        {
            var rslt = new SensorAnalyzeResult();
            rslt.SensorId = (int)data.SensorId;
            rslt.Score = 100;
            rslt.ThresholdAlarm = null;

            if (!thresholds.Any() || data.Data == null)
            {
                // 未配置阈值,默认以无告警处理
                return rslt;
            }

            var sensor = DbAccessor.GetSensorInfo((int)data.SensorId).AsEnumerable().FirstOrDefault();
            var sensorData = data.Data;

            var alarm = new ThresholdAlarm((int)data.SensorId);
            // 遍历各监测项
            for (int i = 0; i < sensorData.Count(); i++)
            {
                int itemId = i + 1;
                var threshold = thresholds.FirstOrDefault(t => t.ItemId == itemId);

                if (threshold != null)
                {
                    var itemName = this.GetItemName(sensor, itemId);
                    var totalLvl = threshold.LevelNumber;

                    // 查询落在哪个阈值区域
                    foreach (var sensorThreshold in threshold.Thresholds)
                    {
                        if (sensorData[i] >= sensorThreshold.Down && sensorData[i] <= sensorThreshold.Up)
                        {
                            rslt.Score -= (int)((1 - (double)sensorThreshold.Level / totalLvl) / sensorData.Count * 100);
                            var alarmDetail = new ThresholdAlarmDetail(itemName, sensorThreshold.Level);
                            alarm.AlarmDetails.Add(alarmDetail);
                            break;
                        }
                    }
                }
            }
            if (alarm.AlarmDetails.Count > 0)
            {
                rslt.ThresholdAlarm = alarm;
            }

            return rslt;
        }

        /// <summary>
        /// 获取传感器阈值配置
        /// </summary>        
        /// <param name="sensors">传感器编号数组</param>
        /// <returns>阈值配置列表</returns>
        public IList<SensorThreshold> GetSensorThreshold(IEnumerable<uint> sensors)
        {
            var dataFromDb = DbAccessor.GetSensorThreshold(sensors);
            var query =
                dataFromDb.AsEnumerable()
                    .GroupBy(s => new { SensorId = s.Field<int>("SensorId"), ItemId = s.Field<int>("ItemId") })
                    .Select(
                        s =>
                        new SensorThreshold
                            {
                                SensorId = s.Key.SensorId,
                                ItemId = s.Key.ItemId,
                                LevelNumber = s.Select(g => g.Field<int>("ThresholdLevel")).Max(),
                                Thresholds =
                                    s.Select(
                                        g =>
                                        new Threshold
                                            {
                                                Level = g.Field<int>("ThresholdLevel"),
                                                Up = g.Field<double>("ThresholdUpValue"),
                                                Down = g.Field<double>("ThresholdDownValue")
                                            })
                                    .ToList()
                            });

            return query.ToList();
        }

        /// <summary>
        /// 获取要分析的数据
        /// </summary>
        /// <param name="sensorResult">传感器采集结果</param>
        /// <returns>数据列表</returns>
        public AnalyzingData GetAnalyzingData(SensorAcqResult sensorResult)
        {
            var data2Analyze = new AnalyzingData { SensorId = sensorResult.Sensor.SensorID };
            var themeColums = sensorResult.Sensor.TableColums.Split(',');
            List<double?> data = new List<double?>(themeColums.Length);

            if (sensorResult.Data.ThemeValues.Count() >= themeColums.Length)
            {
                for (int i = 0; i < themeColums.Length; i++)
                {
                    data.Add(sensorResult.Data.ThemeValues[i]);
                }
            }
            else
            {
                for (int i = 0; i < themeColums.Length; i++)
                {
                    if (i > sensorResult.Data.ThemeValues.Count() - 1)
                    {
                        data.Add(0);
                    }
                    else
                    {
                        data.Add(sensorResult.Data.ThemeValues[i]);
                    }
                }
            }

            data2Analyze.Data = data;

            return data2Analyze;
        }

        private string GetItemName(DataRow sensor, int itemId)
        {
            string itemName = string.Empty;
            if (sensor != null)
            {
                var factorId = sensor.Field<int>("FactorId");
                var factor = DbAccessor.GetSafetyFactorInfo(factorId).AsEnumerable().FirstOrDefault();
                if (factor != null)
                {
                    var columns = factor.Field<string>("Columns").Split(',');
                    if (itemId - 1 < columns.Length)
                    {
                        itemName = columns[itemId - 1];
                    }
                }
            }

            return itemName;
        }
    }
}