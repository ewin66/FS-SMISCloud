#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="VibratingWireSensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140912 by WIN .
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
using System.Reflection;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    //协议参考文档： Documents/PMO/02 产品协议/我司产品协议/通信协议/ModBus统一通信协议 .xls

    [SensorAdapter(Protocol = ProtocolType.VibratingWire)]
    public class VibratingWireSensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_DevType = 0; // 设备类型 振弦 10 
        private const int IDX_DevId = 2; // 模块号 
        private const int IDX_CMD = 4;   // 采集命令=0x01
        private const int IDX_DataField = 5; // 
        private const int SensorType = 10; //振弦 

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                if (sensorAcq.Sensor.ModuleNo > ushort.MaxValue)
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                    return;
                }
                var packet = new byte[9];
                ValueHelper.WriteShort_BE(packet, IDX_DevType, (short)SensorType);
                ValueHelper.WriteUShort_BE(packet, IDX_DevId, (ushort)sensorAcq.Sensor.ModuleNo);
                packet[IDX_CMD] = 0x01; //采集命令
                packet[IDX_DataField] = (byte)sensorAcq.Sensor.ChannelNo;
                packet[IDX_DataField + 1] = 0x67; // 默认采集 频率、温度、幅值 、发送激励信号、检测线状态 返回包长度为22
                // 如果采集温度编号 0x6f 返回包长度为26
                ValueHelper.CheckCRC16(packet, 0, IDX_DataField + 2, out packet[IDX_DataField + 2], out packet[IDX_DataField + 3]);
                sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                sensorAcq.Request = packet;
            }
            catch
            {
                sensorAcq.Request = null;
                sensorAcq.ErrorCode = (int)Errors.ERR_UNKNOW;
            }
        }

        public void ParseResult(ref SensorAcqResult rawData)
        {
            byte[] data = rawData.Response;
            rawData.ErrorCode = IsValid(data);
            if ((int)Errors.SUCCESS != rawData.ErrorCode)
                return;
            var module = ValueHelper.GetShort_BE(data, IDX_DevId);
            if (module != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }

            if (data[IDX_CMD] == 0xC0)
            {
                rawData.ErrorCode = ModbusErrorCode.GetErrorCode(data[IDX_CMD + 1]);
                return;
            }

            var channel = data[IDX_CMD + 1];
            if (channel == 0) channel = 1;
            if (channel != rawData.Sensor.ChannelNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_RECEIVED_CH;
                return;
            }
            var freState = data[IDX_DataField + 2] & 0x000f;
            var tempState = data[IDX_DataField + 2] & 0xfff0;

            if (freState != 0 || tempState != 0)
            {
                if (freState != 0 && tempState != 0)
                {
                    rawData.ErrorCode = (int)Errors.ERR_VIBRW_FREWIRE_TEMPWIRE;
                    return;
                }
                if (freState != 0)
                {
                    rawData.ErrorCode = (int)Errors.ERR_VIBRW_FREWIRE;
                    return;
                }
                rawData.ErrorCode = (int)Errors.ERR_VIBRW_TEMPWIRE;
                return;
            }
            try
            {
                // TODO 大字序 FLOAT
                var frequency = ValueHelper.GetFloat_BE(data, IDX_DataField + 3);
                var am = ValueHelper.GetFloat_BE(data, IDX_DataField + 7);
                var temperature = ValueHelper.GetFloat_BE(data, IDX_DataField + 11);
                double physicalQuantity = 0;
                double colphy = 0;
                double k = 0;
                double f0 = 0;
                double kt = 0;
                double t0 = 0;
                if (rawData.Sensor.Parameters.Count >= 4)
                {
                    k = rawData.Sensor.Parameters[0].Value;
                    f0 = rawData.Sensor.Parameters[1].Value;
                    kt = rawData.Sensor.Parameters[2].Value;
                    t0 = rawData.Sensor.Parameters[3].Value;
                }
                switch (rawData.Sensor.FormulaID)
                {
                    case 1:
                        colphy = physicalQuantity = (k * (Math.Pow(frequency, 2) - Math.Pow(f0, 2)) + kt * (temperature - t0));
                        break;
                    case 7:
                        double H = rawData.Sensor.Parameters[4].Value;
                        colphy = (k * (Math.Pow(f0, 2) - Math.Pow(frequency, 2)) + kt * (temperature - t0));
                        var h = colphy * 1000 / 9.8f;
                        physicalQuantity = (H + h);
                        break;
                    case 17:
                        physicalQuantity = (k * (Math.Pow(f0, 2) - Math.Pow(frequency, 2)) + kt * (temperature - t0));
                        colphy = physicalQuantity;
                        break;
                    default:
                        physicalQuantity = (k * (Math.Pow(frequency, 2) - Math.Pow(f0, 2)) + kt * (temperature - t0));
                        colphy = physicalQuantity;
                        break;
                }

                var raws = new double[] {frequency, temperature, am};
                var phys = new double[] { physicalQuantity };
                var colphys = new double[] { colphy };
                rawData.Data = new SensorData(raws, phys, colphys)
                {
                    JsonResultData =
                        string.Format("{0}\"sensorId\":{1},\"data\":\"频率:{2} Hz,温度:{3} ℃\"{4}", '{', rawData.Sensor.SensorID,
                            frequency, temperature, '}')
                };
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("ModBus VibratingWire sensor [id:{0},m:{1},ch:{4}] parsedfailed ,received bytes{3},ERROR : {2}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message, ValueHelper.BytesToHexStr(rawData.Response), rawData.Sensor.ChannelNo);
            }
        }

        private int IsValid(byte[] data)
        {
            if (data == null || (data.Length != 22 && data.Length != 8))
            {
                return (int) Errors.ERR_INVALID_DATA;
            }

            if ((data[IDX_DevType] << 8 | data[IDX_DevType + 1]) != SensorType)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            byte[] crc16 = { data[data.Length - 2], data[data.Length - 1] };
            byte crcHi;
            byte crcLo;
            ValueHelper.CheckCRC16(data, 0, data.Length - 2, out crcHi, out crcLo);
            if ((crc16[0] == crcHi) && (crc16[1] == crcLo))
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_CRC;
        }
    }
}