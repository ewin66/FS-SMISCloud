//  --------------------------------------------------------------------------------------------
//  <copyright file="GpsProcess.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2013 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：lwl  20131224
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
using System.Data;
using System.Text;
using DataCenter.Accessor.ViewBLL;
using log4net;

namespace DataCenter.Task.Process
{
    /// <summary>
    /// The gps process.
    /// </summary>
    public class GpsProcess
    {
        private static Dictionary<int, object[]> gpsSensorIDinfos = new Dictionary<int, object[]>();

        private static readonly Dictionary<int, List<int>> ProjectsSensors = new Dictionary<int, List<int>>();

        private static readonly object SyncRoot = new object();

        private static GpsProcess gpsProcess;

        private static readonly ILog Log = LogManager.GetLogger(typeof(GpsProcess));

        /// <summary>
        /// Prevents a default instance of the <see cref="GpsProcess"/> class from being created.
        /// </summary>
        private GpsProcess()
        {
            this.InclinGpsProcess();
        }

        /// <summary>
        /// The get gps process instance.
        /// </summary>
        /// <returns>
        /// The <see cref="GpsProcess"/>.
        /// </returns>
        public static GpsProcess GetGpsProcessInstance()
        {
            if (gpsProcess == null)
            {
                lock (SyncRoot)
                {
                    if (gpsProcess == null)
                    {
                        return gpsProcess = new GpsProcess();
                    }
                }
            }

            return gpsProcess;
        }

        /// <summary>
        /// The inclin gps process.
        /// </summary>
        private void InclinGpsProcess()
        {
            var bll = new DeviceInfoBll();
            using (DataTable deviceInfodt = bll.GetGpsStructureSensorDeviceInfo())
            {
                foreach (DataRow dr in deviceInfodt.Rows)
                {
                    try
                    {
                        float iniXvalue;
                        float iniYvalue;
                        float iniZvalue;
                        int structureID = Convert.ToInt32(dr["STRUCT_ID"]);
                        int sensorID = Convert.ToInt32(dr["SENSOR_ID"]);
                        int safetyFactorTypeID = Convert.ToInt32(dr["SAFETY_FACTOR_TYPE_ID"]);
                        string moduleNo = dr["MODULE_NO"].ToString().Trim();
                        int channelNo = Convert.ToInt32(dr["DAI_CHANNEL_NUMBER"]);
                        float.TryParse(dr["Parameter1"].ToString(), out iniXvalue);
                        float.TryParse(dr["Parameter2"].ToString(), out iniYvalue);
                        float.TryParse(dr["Parameter3"].ToString(), out iniZvalue);
                        if (!ProjectsSensors.ContainsKey(structureID))
                        {
                            var listSensorId = new List<int>();
                            ProjectsSensors.Add(structureID, listSensorId);
                        }

                        if (!gpsSensorIDinfos.ContainsKey(sensorID))
                        {
                            var pressinfo = new object[]
                                                {
                                                    structureID, safetyFactorTypeID, moduleNo, channelNo, iniXvalue,
                                                    iniYvalue, iniZvalue
                                                };
                            gpsSensorIDinfos.Add(sensorID, pressinfo);
                        }

                        ProjectsSensors[structureID].Add(sensorID);
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
        /// The calulate gps data value.
        /// </summary>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="collDate">
        /// The coll date.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// smg；
        /// </exception>
        public float[] CalulateGpsDataValue(int sensorId, float[] collDate)
        {
            try
            {
                float iniXvalue = 0;
                float iniYvalue = 0;
                float iniZvalue = 0;
                if (gpsSensorIDinfos.ContainsKey(sensorId))
                {
                    iniXvalue = (float)gpsSensorIDinfos[sensorId][4];
                    iniYvalue = (float)gpsSensorIDinfos[sensorId][5];
                    iniZvalue = (float)gpsSensorIDinfos[sensorId][6];
                }
                else
                {
                    if (collDate.Length == 3)
                    {
                        iniXvalue = collDate[0];
                        iniYvalue = collDate[1];
                        iniZvalue = collDate[2];
                        gpsSensorIDinfos.Add(
                            sensorId, new object[] { null, null, null, null, iniXvalue, iniYvalue, iniZvalue });
                    }
                }

                if (collDate.Length == 3)
                {
                    var calu = new float[3];
                    calu[0] = collDate[0] - iniXvalue;
                    calu[1] = collDate[1] - iniYvalue;
                    calu[2] = collDate[2] - iniZvalue;
                    return calu;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }

            var smg = new StringBuilder("此数据不是GPS数据;");
            smg.Append("传感器ID:").Append(sensorId);
            throw new Exception(smg.ToString());
        }
    }
}