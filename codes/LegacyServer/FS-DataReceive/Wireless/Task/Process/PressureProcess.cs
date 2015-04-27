//  --------------------------------------------------------------------------------------------
//  <copyright file="PressureProcess.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2013 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20131223
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using DataCenter.Accessor.ViewBLL;
using DataCenter.Task.DataSmoothModel;
using log4net;

namespace DataCenter.Task.Process
{
    /// <summary>
    /// The pressure process.
    /// </summary>
    public class PressureProcess
    {
        private static readonly int CountPressData = string.IsNullOrEmpty(ConfigurationManager.AppSettings["CountPress"])
                                                ? 48
                                                : Convert.ToInt32(ConfigurationManager.AppSettings["CountPress"]);

        private static readonly int StartIndexAvg = string.IsNullOrEmpty(ConfigurationManager.AppSettings["StartIndexAvg"])
                                               ? 5
                                               : Convert.ToInt32(ConfigurationManager.AppSettings["StartIndexAvg"]);

        private static readonly int EndIndexAvg = string.IsNullOrEmpty(ConfigurationManager.AppSettings["EndIndexAvg"])
                                                      ? (CountPressData - 5)
                                                      : Convert.ToInt32(ConfigurationManager.AppSettings["EndIndexAvg"]);

        // private static Dictionary<int, DateTime> lastTimes = new Dictionary<int, DateTime>();
        private static readonly Dictionary<int, object[]> SensorIDinfo = new Dictionary<int, object[]>();

        private static readonly Dictionary<int, List<int>> ProjectsSensors = new Dictionary<int, List<int>>();

        private static Dictionary<int, DataInfo> pressDataInfos = new Dictionary<int, DataInfo>();

        private static readonly object SyncRoot = new object();

        private static readonly ILog Log = LogManager.GetLogger(typeof(PressureProcess));

        /// <summary>
        /// Prevents a default instance of the <see cref="PressureProcess"/> class from being created.
        /// </summary>
        private PressureProcess()
        {
            this.InclinProcess();
            this.InclinPressDataInfos();
        }

        private static PressureProcess process;

        /// <summary>
        /// The get ptessure process instance.
        /// </summary>
        /// <returns>
        /// The <see cref="PressureProcess"/>.
        /// </returns>
        public static PressureProcess GetPtessureProcessInstance()
        {
            if (process == null)
            {
                lock (SyncRoot)
                {
                    if (process == null)
                    {
                        return process = new PressureProcess();
                    }
                }
            }

            return process;
        }

        /// <summary>
        /// The inclin process.
        /// </summary>
        private void InclinProcess()
        {
            var bll = new DeviceInfoBll();
            using (DataTable deviceInfodt = bll.GetPressStructureSensorDeviceInfo())
            {
                foreach (DataRow dr in deviceInfodt.Rows)
                {
                    try
                    {
                        float inivalue;
                        int structureID = Convert.ToInt32(dr["STRUCT_ID"]);
                        int sensorID = Convert.ToInt32(dr["SENSOR_ID"]);
                        int safetyFactorTypeID = Convert.ToInt32(dr["SAFETY_FACTOR_TYPE_ID"]);
                        string moduleNo = dr["MODULE_NO"].ToString().Trim();
                        int channelNo = Convert.ToInt32(dr["DAI_CHANNEL_NUMBER"]);
                        float.TryParse(dr["Parameter1"].ToString(), out inivalue);
                        if (!ProjectsSensors.ContainsKey(structureID))
                        {
                            var sensors = new List<int>();
                            ProjectsSensors.Add(structureID, sensors);
                        }

                        if (!pressDataInfos.ContainsKey(sensorID))
                        {
                            var pressinfo = new DataInfo();
                            pressDataInfos.Add(sensorID, pressinfo);
                        }

                        ProjectsSensors[structureID].Add(sensorID);
                        SensorIDinfo.Add(
                            sensorID, new object[] { structureID, safetyFactorTypeID, moduleNo, channelNo, inivalue });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// The inclin press data infos.
        /// </summary>
        private void InclinPressDataInfos()
        {
            var bll = new DeviceInfoBll();
            foreach (int sensorId in SensorIDinfo.Keys)
            {
                using (DataTable dt = bll.GetPressureDataBySensor(sensorId, CountPressData))
                {
                    try
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["CollectOriginalValue1"] != DBNull.Value)
                                {
                                    float colPressValue = Convert.ToSingle(dr["CollectOriginalValue1"]);
                                    pressDataInfos[sensorId].InQueue(colPressValue);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// The calculate press value.
        /// </summary>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="collectData">
        /// The collect data.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public float CalculatePressValue(int sensorId, float collectData)
        {
            try
            {
                if (pressDataInfos.ContainsKey(sensorId))
                {
                    pressDataInfos[sensorId].InQueue(collectData);
                    if (pressDataInfos[sensorId].GetCount() > CountPressData)
                    {
                        pressDataInfos[sensorId].OutQueue();
                    }
                }
                else
                {
                    pressDataInfos.Add(sensorId, new DataInfo());
                    pressDataInfos[sensorId].InQueue(collectData);
                }

                float pressAvg = pressDataInfos[sensorId].GetCount() >= CountPressData
                                     ? pressDataInfos[sensorId].GetAverage(StartIndexAvg, EndIndexAvg)
                                     : pressDataInfos[sensorId].GetAverage();
                float pressure = pressAvg - (float)SensorIDinfo[sensorId][4];
                return pressure;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}