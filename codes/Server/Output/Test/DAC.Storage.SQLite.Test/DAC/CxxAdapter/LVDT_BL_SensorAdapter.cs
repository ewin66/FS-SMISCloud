#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="BlLVDTSensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
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
    //协议参考： Documents/PMO/02 产品协议/外购产品协议/拉线位移通讯协议2013.doc
    [SensorAdapter(Protocol = ProtocolType.LVDT_BL)]
    public class LVDT_BL_SensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_Adrress = 0; //byte 模块地址
        private const int IDX_CMD = 1; // byte 采集命令 = 0x03
        private const int IDX_Length = 2; // byte 数据长度
        private const int IDX_CRC16_Req = 6; // 2 byte 请求包校验 
        private const int IDX_CRC16_Res = 5; // 2 byte 应答包校验
        private const int IDX_FrameLength_Req = 8; // byte 请求包长度
        private const int IDX_FrameLength_Res = 7; // byte 应答包长度

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                byte m;
                if (byte.TryParse(sensorAcq.Sensor.ModuleNo.ToString().Trim(), out m))
                {
                    var packet = new byte[8];
                    packet[IDX_Adrress] = Convert.ToByte(sensorAcq.Sensor.ModuleNo);
                    packet[IDX_CMD] = 0x03; // 采集命令 = 0x03
                    packet[5] = 0x01; // 目的寄存器长度低8位
                    ValueHelper.CheckCRC16(packet, 0, 6, out packet[IDX_CRC16_Req + 1], out packet[IDX_CRC16_Req]);
                    //start, end
                    sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                    sensorAcq.Request = packet;
                }
                else
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                }
            }
            catch
            {
                sensorAcq.Request = null;
                sensorAcq.ErrorCode = (int)Errors.ERR_UNKNOW;
            }
        }

        public void ParseResult(ref SensorAcqResult rawData)
        {
            rawData.ErrorCode = IsValid(rawData.Response);
            if (rawData.ErrorCode != (int)Errors.SUCCESS)
                return;
            byte[] data = rawData.Response;
            var module = data[IDX_Adrress];
            if (module != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            try
            {
                var elongationIndicator = ValueHelper.GetShort_BE(data, IDX_Length + 1)/10.0;
                double changeElongation = 0;
                if (rawData.Sensor.Parameters != null && rawData.Sensor.Parameters.Count > 0)
                {
                    var displacement = elongationIndicator - rawData.Sensor.Parameters[0].Value;
                    changeElongation = displacement;
                }

                var raws = new double[] { elongationIndicator };
                var phys = new double[] { changeElongation };

                rawData.Data = new SensorData(raws, phys, raws)
                {
                    JsonResultData =
                        string.Format("{0}\"sensorId\":{1},\"data\":\"偏移量:{2} mm\"{3}", '{', rawData.Sensor.SensorID,
                            elongationIndicator, '}')
                };

                //rawData.Data = new LVDTData(elongationIndicator, changeElongation)
                //{
                //    JsonResultData =
                //        string.Format("{0}\"sensorId\":{1},\"data\":\"偏移量:{2} mm\"{3}", '{', rawData.Sensor.SensorID,
                //            elongationIndicator, '}')
                //};
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("bl lvdt sensor [Id:{0} m: {1}] parsedfailed,received bytes{2}, ERROR: {3}",
                        rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ValueHelper.BytesToHexStr(rawData.Response),
                        ex.Message);
            }
        }

        public int IsValid(byte[] data)
        {
            if (data == null || data.Length != 7)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            if (data[IDX_CMD] != 0x03)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            byte[] crc16 = {data[IDX_CRC16_Res], data[IDX_CRC16_Res + 1]};
            byte crcHi;
            byte crcLo;
            ValueHelper.CheckCRC16(data, 0, 5, out crcLo, out crcHi);
            if ((crc16[0] != crcHi) || (crc16[1] != crcLo))
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            return (int)Errors.SUCCESS;
        }

    }
}