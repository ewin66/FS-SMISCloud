#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="LVDT_XW_SensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
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
    // Documents/PMO/02 产品协议/外购产品协议/深圳信为数字式LVDT传感器编程手册_客户版2.pdf
    [SensorAdapter(Protocol = ProtocolType.LVDT_XW)]
    public class LVDT_XW_SensorAdapter: ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 地址位 0.
        /// </summary>
        private const int IDX_ADDRESS = 0;
        /// <summary>
        /// 命令号所在位 1
        /// </summary>
        private const int IDX_CMD = 1;  // 采集命令 0x04
        private const int IDX_DATA = 3;
        private const int IDX_CRC16_REQ = 6; // 2 byte 请求包校验
        private const int IDX_CRC16_RESP = 5; // 2 byte 应答包校验

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                byte m;
                if (byte.TryParse(sensorAcq.Sensor.ModuleNo.ToString().Trim(), out m))
                {
                    var packet = new byte[8];
                    packet[IDX_ADDRESS] = Convert.ToByte(sensorAcq.Sensor.ModuleNo);
                    packet[IDX_CMD] = 0x04;
                    //输入寄存器起始地址为 0x0004,寄存器数量为 2
                    packet[3] = 0x04; // 目的寄存器长度低8位
                    packet[5] = 0x02;
                    ValueHelper.CheckCRC16(packet, 0, IDX_CRC16_REQ, out packet[IDX_CRC16_REQ + 1], out packet[IDX_CRC16_REQ]);
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
            if ((int)Errors.SUCCESS != rawData.ErrorCode)
                return;
            byte[] data = rawData.Response;
            var module = data[IDX_ADDRESS];
            if (module != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            try
            {
                if (data[IDX_CMD] == 0x04)
                {
                    var by = new byte[4];
                    Array.Copy(data, IDX_DATA, by, 0, 4);
                    var sign = 1;
                    if (by[0] >= 0x80)
                    {
                        by[0] = (byte) (by[0] & 0x7f);
                        sign = -1;
                    }

                    var integer = (((by[0] & 0xffff) << 8) | by[1])*sign;
                    var decim = (((by[2] & 0xffff) << 8) | by[3])/1000d*sign;
                    var elongationIndicator = integer + decim;
                    double changeElongation = elongationIndicator;
                    if (rawData.Sensor.Parameters.Count > 0)
                    {
                        var displacement = elongationIndicator - rawData.Sensor.Parameters[0].Value;
                        changeElongation = displacement;
                    }

                    var raws = new double[] { elongationIndicator };
                    var phys = new double[] { changeElongation };

                    rawData.Data = new SensorData(raws, phys, raws)
                    {
                        JsonResultData =
                            string.Format("{0}\"sensorId\":{1},\"data\":\"偏移量:{2} mm\"{3}", '{',
                                rawData.Sensor.SensorID, elongationIndicator, '}')
                    };

                    //rawData.Data = new LVDTData(elongationIndicator, changeElongation)
                    //{
                    //    JsonResultData =
                    //        string.Format("{0}\"sensorId\":{1},\"data\":\"偏移量:{2} mm\"{3}", '{',
                    //            rawData.Sensor.SensorID, elongationIndicator, '}')
                    //};
                }
                else
                {
                    switch (data[IDX_CMD + 1])
                    {
                        case 0x01:
                        case 0x06:
                            rawData.ErrorCode = (int)Errors.ERR_LVDTXW_CMD;
                            break;
                        case 0x02:
                        case 0x03:
                        case 0x05:
                            rawData.ErrorCode = (int)Errors.ERR_LVDTXW_EAX;
                            break;
                        case 0x04:
                            rawData.ErrorCode = (int)Errors.ERR_LVDTXW_ACTION;
                            break;
                    } 
                }
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("xw lvdt sensor [Id:{0} m: {1}] parsedfailed,received bytes{2}, ERROR: {3}",
                        rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ValueHelper.BytesToHexStr(rawData.Response),
                        ex.Message);
            }
        }

        public int IsValid(byte[] data)
        {
            if (data == null || (data.Length != 9 && data.Length != 5))
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            if (data[IDX_CMD] != 0x04 && data[IDX_CMD] != 0x84)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            byte[] crc16 = {data[data.Length - 2], data[data.Length - 1]};
            byte crcHi;
            byte crcLo;
            ValueHelper.CheckCRC16(data, 0, data.Length - 2, out crcLo, out crcHi);
            if ((crc16[0] == crcHi) && (crc16[1] == crcLo))
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_CRC;
        }
    }
}