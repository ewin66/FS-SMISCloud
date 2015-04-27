// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReloveCustomTransData.cs" company="江苏飞尚安全监测咨询有限公司">
//   Copyright (C) 2013 飞尚科技
//   //  版权所有。
// </copyright>
//  <summary>
//   文件功能描述：
//   创建标识：20131223
//   修改标识：
//   修改描述：
//   修改标识：
//   修改描述：
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using DataCenter.Accessor;
using DataCenter.Model;
using DataCenter.Task.Process;
using log4net;

namespace DataCenter.WirelessCustomTransmission
{
    /// <summary>
    ///  解析澜沧江数据.
    /// </summary>
    public class ReloveCustomTransLCJData
    {
        private const int LengthOfFrame = 5;

        private const int ProjectCodeOfFrame = 2;

        private const int ModuleNumOfFrame = 6;

        private const int TypeOfDataOfFrame = 4;

        private const int ChannelIDOfFrame = 8;

        private const int AcqTimeOfFrame = 9;

        private const int DataValueOfFrame = 17;

        private const string OriginalDataTable = "T_COL_ORIGINAL_DATAVALUE";

        private static readonly ILog Log = LogManager.GetLogger(typeof(ReloveCustomTransData));

        private static PressureProcess pressureProcess = PressureProcess.GetPtessureProcessInstance();

        private static GpsProcess gpsProcess = GpsProcess.GetGpsProcessInstance();

        /// <summary>
        /// 桥墩沉降GPS
        /// </summary>
        private const int BridgeSettle = 19;

        /// <summary>
        /// 梁段挠度
        /// </summary>
        private const int BridgeDeflection = 21;

        /// <summary>
        /// 桥面振动
        /// </summary>
        private const int BridgeVibration = 27;

        /// <summary>
        /// The protocol order.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ProtocolOrder(ReceiveDataInfo data)
        {
            var packet = data.PackagesBytes;
            if (packet != null)
            {
                var package = new ArrayList(packet);
                var bll = new Bll();
                while (package.Count > 0)
                {
                    string dtuId = data.Sender;
                    int len = (byte)package[LengthOfFrame];
                    var by = new byte[len];
                    package.CopyTo(0, @by, 0, len);
                    int project = BitConverter.ToInt16(@by, ProjectCodeOfFrame);
                    int typeOfData = @by[TypeOfDataOfFrame];
                    string moduleID = BitConverter.ToInt16(@by, ModuleNumOfFrame).ToString();
                    int channelId = @by[ChannelIDOfFrame];
                    var ticks = BitConverter.ToInt64(@by, AcqTimeOfFrame);
                    var acqTime = new DateTime(ticks);
                    var countOfData = (len - 20) / 4; // 一个float数据占4个字节
                    var value = new float[countOfData];
                    for (int i = 0; i < countOfData; i++)
                    {
                        value[i] = BitConverter.ToSingle(@by, DataValueOfFrame + (4 * i));
                    }

                    package.RemoveRange(0, len);
                    try
                    {
                        int sensorID = bll.GetSensorID(project, typeOfData, dtuId, moduleID, channelId);
                        string tableName = bll.GetTableName(typeOfData);
                        switch (typeOfData)
                        {
                            case BridgeVibration: // 27桥面振动
                                for (int i = 0; i < value.Length; i++)
                                {
                                    value[i] = value[i] * 100;
                                }

                                break;
                            case BridgeDeflection: // 21梁段挠度
                                try
                                {
                                    var oridata = new[] { project, value[0] };
                                    bll.InsertDataValues(OriginalDataTable, oridata, sensorID, typeOfData, acqTime);
                                    float calData = pressureProcess.CalculatePressValue(sensorID, value[0]);
                                    value[0] = calData;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.Message);
                                }

                                break;
                            case BridgeSettle: // 19GPS
                                 try
                                 {
                                     var oridata = new[] { project, value[0], value[1], value[2] };
                                    bll.InsertDataValues(OriginalDataTable, oridata, sensorID, typeOfData, acqTime);
                                    float[] calData = gpsProcess.CalulateGpsDataValue(sensorID, value);
                                    value = calData;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.Message);
                                }

                                break;
                        }

                        bll.InsertDataValues(tableName, value, sensorID, typeOfData, acqTime);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
            }

            return null;
        }
    }
}