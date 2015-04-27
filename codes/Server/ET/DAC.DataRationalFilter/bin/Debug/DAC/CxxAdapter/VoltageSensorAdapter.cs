#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="VoltageSensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：lonwin lonwin ling20140914
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
    // Documents/PMO/02 产品协议/我司产品协议/通信协议/32通道电压采集仪通讯协议.doc

    [SensorAdapter(Protocol = ProtocolType.Voltage)]
    public class VoltageSensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_FrameH = 0;          // 帧头 0xfe 0x43 0x58
        private const int IDX_FRAME_LEN = 4;       // 帧长 请求包 19，响应包 28
        private const int IDX_DEST = 5;            // 接收端地址（高地址在前）
        private const int IDX_SOURCE = 7;          // 发送端地址（高地址在前）
        // 当发送端或接收端为PC时，地址为 00 
        private const int IDX_Req = 9;             // 请求=0x00 响应=0x01
        private const int IDX_CMD = 10;            // 数据采集命令 == 0x03
        private const int IDX_CONTENT = 11;        // 请求参数 ：通道号

        private const int IDX_CHSUM_Req = 17;      // 校验码
        private const int IDX_FRAMETAIL_REQ = 18;  // 帧尾=0xef
        // 数据包
        private const int IDX_FREDATA = 12;        // 频率
        private const int IDX_TEMPDATA = 16;       // 温度
        private const int IDX_TIME = 20;           // 时间
        private const int IDX_CHSUM_Resp = 26;     //校验码
        private const int IDX_FRAMETAIL_RESP = 27; // 帧尾=0xef

        #region Implementation of ISensorAdapter

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                if (sensorAcq.Sensor.ModuleNo > ushort.MaxValue || sensorAcq.Sensor.ModuleNo < ushort.MinValue)
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                    return;
                }
                var packet = new byte[19];
                packet[IDX_FrameH] = 0xfe;
                packet[IDX_FrameH + 1] = 0x46;
                packet[IDX_FrameH + 2] = 0x41;
                packet[IDX_FrameH + 3] = 0x53;
                packet[IDX_FRAME_LEN] = 19;
                ValueHelper.WriteUShort_BE(packet, IDX_DEST, (ushort)sensorAcq.Sensor.ModuleNo);
                packet[IDX_CMD] = 0x01;
                byte control = 0;
                //int range = si.Parameters != null && si.ParamCount >= 3 ? (int) si.Parameters[3].Value : 1;
                //Id	Range
                //1	    0~5V
                //2	    -5~5V
                //3	    0~10V
                //4	    -10~10V
                var range = (VoltageSensorRange)EnumHelper.GetItemFromDesc(typeof(VoltageSensorRange), sensorAcq.Sensor.ProductCode);
                switch (range)
                {
                    case VoltageSensorRange.FSLF10:
                        control = 0x40; // 0x47
                        break;
                    case VoltageSensorRange.FSLF25:
                        control = 0x41; // 0x4f
                        break;
                    case VoltageSensorRange.FSLF50:
                        control = 0x50; // 0x57
                        break;
                    case VoltageSensorRange.FSLF100:
                        control = 0x51; // 0x5f
                        break;
                    case VoltageSensorRange.FS_LFV_V0P5:
                        control = 0x40;
                        break;
                    case VoltageSensorRange.FS_LFV_V0P10:
                        control = 0x50;
                        break;
                    case VoltageSensorRange.FS_LFV_VM5P5:
                        control = 0x48;
                        break;
                    case VoltageSensorRange.FS_LFV_VM10P10:
                        control = 0x58;
                        break;
                    default:
                        control = 0x40;
                        break;
                }
                control = (byte)(control | (byte)(0x07 & (byte)(sensorAcq.Sensor.ChannelNo - 1)));
                packet[IDX_CONTENT] = control;
                packet[IDX_CONTENT + 1] = (byte)(sensorAcq.Sensor.ChannelNo);
                packet[IDX_CHSUM_Req] = ValueHelper.CheckXor(packet, 0, IDX_CHSUM_Req);
                packet[IDX_FRAMETAIL_REQ] = 0xef;
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
            var data = rawData.Response;
            rawData.ErrorCode = IsValid(data);
            if ((int)Errors.SUCCESS != rawData.ErrorCode)
                return;
            var module = ValueHelper.GetShort_BE(data, IDX_SOURCE);
            if (module != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            int channel = Convert.ToInt16(data[IDX_CONTENT]);
            if (channel != rawData.Sensor.ChannelNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_RECEIVED_CH;
                return;
            }
            try
            {
                var temByte = new byte[4];
                for (var j = 0; j < 2; j++)
                {
                    temByte[1 - j] = data[12 + j];
                }

                uint voltageAd = this.bytesToInt(temByte); //原始电压AD值
                var range =
                    (VoltageSensorRange)
                        EnumHelper.GetItemFromDesc(typeof(VoltageSensorRange), rawData.Sensor.ProductCode);
                float voltage = Getvoltage(range, voltageAd);
                double phyValue = 0;
                if (rawData.Sensor.FormulaID == 5) // 电压公式 ε=k*vol-c
                {
                    phyValue = (rawData.Sensor.Parameters[0].Value * voltage -
                                rawData.Sensor.Parameters[1].Value);
                }
                else if (rawData.Sensor.FormulaID == 16) // LVDT电压公式 L=k(V-V0)+C
                {
                    phyValue = (rawData.Sensor.Parameters[0].Value *
                                (voltage - rawData.Sensor.Parameters[2].Value) +
                                rawData.Sensor.Parameters[1].Value);
                }

                var raws = new double[] { voltageAd, voltage };
                var phys = new double[] { phyValue };

                rawData.Data = new SensorData(raws, phys, raws)
                {
                    JsonResultData =
                        string.Format("{0}\"sensorId\":{1},\"data\":\"电压:{2} V\"{3}", '{',
                            rawData.Sensor.SensorID, voltage, '}')
                };

                //rawData.Data = new VoltageData(voltage, phyValue)
                //{
                //    JsonResultData =
                //        string.Format("{0}\"sensorId\":{1},\"data\":\"电压:{2} V\"{3}", '{',
                //            rawData.Sensor.SensorID, voltage, '}')
                //};
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("voltage sensor [id:{0},m:{1},ch:{4}] parsedfailed ,received bytes{3},ERROR : {2}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message, ValueHelper.BytesToHexStr(rawData.Response), rawData.Sensor.ChannelNo);
            }
        }

        #endregion

        private int IsValid(byte[] data)
        {
            if (data.Length < 24 || data.Length > 26)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            if (data[IDX_FrameH] != 0xfe || data[IDX_FrameH + 1] != 0x46 || data[IDX_FrameH + 2] != 0x41 || data[IDX_FrameH + 3] != 0x53 || data[data.Length - 1] != 0xef)
                return (int)Errors.ERR_INVALID_DATA;
            var xorValue = data[data.Length - 2];
            var xorCalc = 0;
            xorCalc = data.Length == 25 ? ValueHelper.CheckXor(data, 0, data.Length - 3) : ValueHelper.CheckXor(data, 0, data.Length - 2);
            if (xorCalc == xorValue)
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_CRC;
        }

        private uint bytesToInt(byte[] bytes)
        {
            uint addr = (uint)(bytes[0] & 0xFF);
            addr |= (uint)((bytes[1] << 8) & 0xFF00);
            addr |= (uint)((bytes[2] << 16) & 0xFF0000);
            addr |= (uint)((bytes[3] << 24) & 0xFF000000);
            return addr;
        }

        private float Getvoltage(VoltageSensorRange range, uint volAd)
        {
            float voltage;
            switch (range)
            {
                case VoltageSensorRange.FSLF10:
                case VoltageSensorRange.FS_LFV_V0P5:
                    voltage = volAd * 5 / 4096f;
                    break;
                case VoltageSensorRange.FSLF50:
                case VoltageSensorRange.FS_LFV_V0P10:
                    voltage = volAd * 10 / 4096f;
                    break;
                case VoltageSensorRange.FSLF25:
                case VoltageSensorRange.FS_LFV_VM5P5:
                    if (volAd < 2048)
                        voltage = volAd * 10 / 4096f;
                    else
                        voltage = (0xFFF - volAd) * 10 / 4096f * (-1);
                    break;
                case VoltageSensorRange.FSLF100:
                case VoltageSensorRange.FS_LFV_VM10P10:
                    if (volAd < 2048)
                        voltage = volAd * 20 / 4096f;
                    else
                        voltage = (0xFFF - volAd) * 20 / 4096f * (-1);
                    break;
                default:
                    voltage = volAd * 10 / 4096f;
                    break;
            }
            return voltage;
        }
    }
}