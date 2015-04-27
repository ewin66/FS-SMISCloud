#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="VibratingWire_OLD_SensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
//  协议参考文档： Documents/PMO/02 产品协议/我司产品协议/通信协议/江西飞尚科技有限公司多通道振弦采集仪协议.doc
//  创建标识：lonwin lonwin ling20140913
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
    // Documents/PMO/02 产品协议/我司产品协议/通信协议/32通道振弦采集仪通讯协议.doc

    [SensorAdapter(Protocol = ProtocolType.VibratingWire_OLD)]
    public class VibratingWire_OLD_SensorAdapter : ISensorAdapter
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
                if (sensorAcq.Sensor.ModuleNo > ushort.MaxValue)
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                    return;
                }
                byte[] package = new byte[19];
                package[IDX_FrameH] = 0xfe;
                package[IDX_FrameH + 1] = 0x54;
                package[IDX_FrameH + 2] = 0x46;
                package[IDX_FrameH + 3] = 0x4c;
                package[IDX_FRAME_LEN] = 19;
                ValueHelper.WriteUShort_BE(package, IDX_DEST, (ushort)sensorAcq.Sensor.ModuleNo);
                package[IDX_CMD] = 0x03;
                package[IDX_CONTENT] = Convert.ToByte(sensorAcq.Sensor.ChannelNo);
                package[IDX_CHSUM_Req] = ValueHelper.CheckXor(package, 0, IDX_CHSUM_Req);
                package[IDX_FRAMETAIL_REQ] = 0xef;
                sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                sensorAcq.Request = package;
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
            int module = ValueHelper.GetShort_BE(data, IDX_SOURCE);
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
                float frequency = ValueHelper.GetFloat(data, IDX_FREDATA);
                float temperature = ValueHelper.GetFloat(data, IDX_TEMPDATA);
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
                        physicalQuantity = (k * (Math.Pow(frequency, 2) - Math.Pow(f0, 2)) + kt * (temperature - t0));
                        colphy = physicalQuantity;
                        break;
                    case 7:

                        double H = rawData.Sensor.Parameters[4].Value;
                        var P = (k * (Math.Pow(f0, 2) - Math.Pow(frequency, 2)) + kt * (temperature - t0));
                        var h = P * 1000 / 9.8f;
                        physicalQuantity = (H + h);
                        colphy = P;
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

                var raws = new double[] { frequency, temperature };
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
                _logger.ErrorFormat("VibratingWire sensor [id:{0},m:{1},ch:{4}] parsedfailed ,received bytes{3},ERROR : {2}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message, ValueHelper.BytesToHexStr(rawData.Response), rawData.Sensor.ChannelNo);
            }
        }

        #endregion

        private int IsValid(byte[] data)
        {
            if (data.Length != 28)
                return (int)Errors.ERR_INVALID_DATA;
            if (data[IDX_FrameH] != 0xfe || data[IDX_FrameH + 1] != 0x54 || data[IDX_FrameH + 2] != 0x46 ||
                data[IDX_FrameH + 3] != 0x4c || data[IDX_FRAMETAIL_RESP] != 0xef)
                return (int)Errors.ERR_INVALID_DATA;
            byte check = data[IDX_CHSUM_Resp];
            if (check == ValueHelper.CheckXor(data, 0, IDX_CHSUM_Resp))
            {
                return (int)Errors.SUCCESS;
            }
            return (int)Errors.ERR_CRC;
        }

    }
}