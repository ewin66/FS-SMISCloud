#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="MagneticFluxSensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
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
    // Documents/PMO/02 产品协议/我司产品协议/通信协议/磁通量协议.xlsx
     [SensorAdapter(Protocol = ProtocolType.MagneticFlux)]
    public class MagneticFluxSensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_FrameH = 0;  // 帧头 0xfe 0xCD 
        private const int IDX_ADRESS = 2;    // 接收端地址（高地址在前）
        private const int IDX_Req = 4;             // 请求=0x00 响应=0x01
        private const int IDX_CMD = 5;            // 数据采集命令 == 0x03
        private const int IDX_CONTENT = 6;        // 请求参数 ：通道号
        private const int IDX_CHSUM_Req = 12;  // 校验码
        private const int IDX_FRAMETAIL_REQ = 13; // 帧尾=0xef

        private const int IDX_CAPVOL = 6;
        private const int IDX_VOLTAGE = 8;
        private const int IDX_TEMP = 12;
        private const int IDX_CHSUM_Resp = 22;     //校验码
        private const int IDX_FRAMETAIL_RESP = 23; // 帧尾=0xef



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
                 var packet = new byte[14];
                 packet[IDX_FrameH] = 0xFE;
                 packet[IDX_FrameH + 1] = 0xCD;
                 ValueHelper.WriteUShort_BE(packet, IDX_ADRESS, (ushort)sensorAcq.Sensor.ModuleNo);
                 packet[IDX_CMD] = 0x04;
                 packet[IDX_CONTENT] = Convert.ToByte(sensorAcq.Sensor.ChannelNo);

                 short vol;
                 try
                 {
                     if (!short.TryParse(sensorAcq.Sensor.Parameters[7].Value.ToString(), out vol))
                     {
                         vol = 20;
                     }
                     if (vol == 0)
                     {
                         vol = 20;
                     }
                 }
                 catch
                 {
                     vol = 20;
                 }

                 ValueHelper.WriteShort_BE(packet, (short)(IDX_CONTENT + 1), vol);
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
             rawData.ErrorCode = IsValid(rawData.Response);
             if ((int)Errors.SUCCESS != rawData.ErrorCode)
                 return;
             int module = (int) ((rawData.Response[IDX_ADRESS] << 8) | (rawData.Response[1 + IDX_ADRESS]));
             if (module != rawData.Sensor.ModuleNo)
             {
                 rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                 return;
             }
             int channel = Convert.ToInt16(rawData.Response[IDX_Req]);
             if (channel != rawData.Sensor.ChannelNo)
             {
                 rawData.ErrorCode = (int)Errors.ERR_RECEIVED_CH;
                 return;
             }
             try
             {
                 var capvol = (rawData.Response[IDX_CAPVOL] << 8) | rawData.Response[IDX_CAPVOL + 1];
                 var voltage = ValueHelper.GetFloat(rawData.Response, IDX_VOLTAGE);
                 var temp = ValueHelper.GetFloat(rawData.Response, IDX_TEMP);
                 double force = 0;
                 if (rawData.Sensor.FormulaID == 4 && rawData.Sensor.Parameters != null &&
                     rawData.Sensor.Parameters.Count == 7)
                 {
                     double kt = rawData.Sensor.Parameters[0].Value;
                     double t0 = rawData.Sensor.Parameters[1].Value;
                     double v0 = rawData.Sensor.Parameters[2].Value;
                     double v0B = rawData.Sensor.Parameters[3].Value;
                     double c0 = rawData.Sensor.Parameters[4].Value;
                     double c1 = rawData.Sensor.Parameters[5].Value;
                     double c2 = rawData.Sensor.Parameters[6].Value;

                     double vol = voltage - kt*(temp - t0) - (v0 - v0B);
                     double phy = c2*Math.Pow(vol, 2) + c1*vol + c0;
                     force = phy;
                 }
                 var raws = new double[] { voltage, temp };
                 var phys = new double[] { force };
                 
                 rawData.Data = new SensorData(raws, phys, phys)
                 {
                     JsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"电压:{2} V, 温度:{3} ℃\"{4}", '{', rawData.Sensor.SensorID, voltage, temp, '}')
                 }; 
                 //rawData.Data = new MagneticFluxData(voltage, temp, force)
                 //{
                 //    JsonResultData =
                 //        string.Format("{0}\"sensorId\":{1},\"data\":\"电压:{2} V, 温度:{3} ℃\"{4}", '{',
                 //            rawData.Sensor.SensorID, voltage, temp, '}')
                 //};
             }
             catch (Exception ex)
             {
                 rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                 _logger.ErrorFormat("ctl sensor [Id:{0} m: {1} ch:{2}] parsedfailed,received bytes{3}, ERROR: {4}",
                         rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, rawData.Sensor.ChannelNo,
                         ValueHelper.BytesToHexStr(rawData.Response), ex.Message);
             }
         }

         #endregion

        private int IsValid(byte[] data)
        {
            if (data.Length != 24)
                return (int)Errors.ERR_INVALID_DATA;
            if (data[IDX_FrameH] != 0xfe || data[IDX_FrameH + 1] != 0xCD || data[IDX_FRAMETAIL_RESP] != 0xef)
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