#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ReloveGeneralHasCalcuTransData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140620 by WIN .
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
using System.Text;
using DataCenter.Accessor;
using DataCenter.Model;
using ET.Common;
using log4net;
using R.WirelessCustomTransmission;

namespace DataCenter.WirelessCustomTransmission
{
    public class ReloveGeneralHasCalcuTransData:IReloveTransData
    {
        private const int ProjectCodeOfFrame = 2;
        private const int TypeOfDataOfFrame = 4;
        private const int LengthOfFrame = 5;
        private const int ModuleNumOfFrame = 7;
        private const int ChannelIdOfFrame = 9;
        private const int Structure = 18;
        private const int AcqTimeOfFrame = 22;
        private const int DataValueOfFrame = 30;

        private static readonly ILog Log = LogManager.GetLogger(typeof(ReloveGeneralHasCalcuTransData));
        private const string OriginalDataTable = "T_COL_ORIGINAL_DATAVALUE";
        public void ReloveReceivedData(string dtuid, byte[] bytes)
        {
            string dtunum = dtuid;
            int sensortype = bytes[TypeOfDataOfFrame];
            string moduleId = BitConverter.ToInt16(bytes, ModuleNumOfFrame).ToString();
            int channelid = bytes[ChannelIdOfFrame];

           // IList<SensorInfo> sensorlist = new List<SensorInfo>();
            IList<SensorInfo> sensorlist = DeceiveInfoDic.GetDeceiveInfoDic().GeSensorInfosByChannel(dtuid, sensortype, moduleId, channelid);
            if (sensorlist.Count > 0)
            {
                DateTime time = DateTime.MinValue;
                StringBuilder str = new StringBuilder();
                for (int si = 0; si < sensorlist.Count; si++)
                {
                    int project = BitConverter.ToInt16(bytes, ProjectCodeOfFrame);
                    int sensorid = sensorlist[si].SensorId;
                    int safetype = sensorlist[si].SafetyFactorTypeId;
                    var bll = new Bll();
                    string tableName = bll.GetTableName(safetype);
                    int structureId = BitConverter.ToInt32(bytes, Structure);
                    long ticks = BitConverter.ToInt64(bytes, AcqTimeOfFrame);
                    time = new DateTime(ticks);
                    int floatcount = (bytes.Length - 31) / 4;
                    var values = new float[floatcount];
                    for (int i = 0; i < floatcount; i++)
                    {
                        float value = BitConverter.ToSingle(bytes, DataValueOfFrame + (4 * i));
                        values[i] = value;
                    }
                    str.Append(sensorid).Append(",");
                    str.Append(time.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append(",");
                    float[] original;
                    float[] calcu;

                    if (sensortype == 15)
                    {
                        original = new float[values.Length - 2];
                        calcu = new float[2];
                    }
                    else
                    {
                        original = new float[values.Length - 1];
                        calcu = new float[1];
                    }

                    int j = original.Length;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i < j)
                        {
                            original[i] = values[i];
                            str.Append(values[i]).Append(",");
                        }
                        else
                        {
                            calcu[i - j] = values[i];
                            str.Append(values[i]).Append(",");
                        }
                    }

                    bll.InsertOrigalData(sensorid, time, original);
                }

                try
                {
                    if (!DeceiveInfoDic.GetDeceiveInfoDic().LastAcqTime.ContainsKey(dtuid))
                    {
                        DeceiveInfoDic.GetDeceiveInfoDic().LastAcqTime.TryAdd(dtuid, time);
                    }
                    else
                    {
                        Log.Debug("比较");
                        if (DeceiveInfoDic.GetDeceiveInfoDic().LastAcqTime[dtuid] < time)
                        {
                            var msg = MakeMsgToDataCalc.MakeMsgToRequestDataCalc(
                                Convert.ToInt32(dtuid),
                                DeceiveInfoDic.GetDeceiveInfoDic().LastAcqTime[dtuid]);

                            Log.DebugFormat("通知计算进程计算 MSG_ID={0},DTU={1},TIME={2}", msg.Id, dtuid, DeceiveInfoDic.GetDeceiveInfoDic().LastAcqTime[dtuid]);

                            DeceiveInfoDic.GetDeceiveInfoDic().LastAcqTime[dtuid] = time;
                        }
                    }
                    Log.Debug(str.ToString());
                }
                catch (Exception ex)
                {
                    StringBuilder msg = new StringBuilder();
                    str.Append(ex.Message + "::").Append(string.Format("[{0},{1},{2}]", dtuid, moduleId, channelid)).Append(">>").Append(ValueHelper.ByteToHexStr(bytes));
                    Log.ErrorFormat(msg.ToString());
                }
            }
            else
            {
                StringBuilder str = new StringBuilder();
                str.Append("未找到该传感器:").Append(string.Format("[{0},{1},{2}]", dtuid, moduleId, channelid)).Append(">>").Append(ValueHelper.ByteToHexStr(bytes));
                Log.ErrorFormat(str.ToString());
            }
        }
    }
}