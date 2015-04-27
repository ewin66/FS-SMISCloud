#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="Wind_OSL_SensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
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

using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    // Documents/PMO/02 产品协议/外购产品协议/协议解析V2.0.pdf

    [SensorAdapter(Protocol = ProtocolType.Wind_OSL)]
    public class Wind_OSL_SensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_ADRESS = 0;
        private const int IDX_CMD = 1;
        private const int IDX_CRC16_REQ = 6;  // 校验码
        private const int IDX_CRC16_RESP = 19;
        /// <summary>
        /// The speed of wind.
        /// </summary>
        private const int IDX_SPEED = 3;
        /// <summary>
        /// The azimuth.
        /// </summary>
        private const int IDX_AZIMUTH = 7;
        /// <summary>
        /// The speed of sound.
        /// </summary>
        private const int IDX_SpeedOfSound = 11;
        /// <summary>
        /// The probes temperature.
        /// </summary>
        private const int IDX_ProbesTemperature = 15;

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                byte m;
                if (byte.TryParse(sensorAcq.Sensor.ModuleNo.ToString().Trim(), out m))
                {
                    var package = new byte[8];
                    package[IDX_ADRESS] = Convert.ToByte(sensorAcq.Sensor.ModuleNo);
                    package[IDX_CMD] = 0x04;
                    package[2] = 0x10;
                    package[3] = 0x04;
                    package[5] = 0x08;
                    ValueHelper.CheckCRC16(package, 0, IDX_CRC16_REQ, out package[IDX_CRC16_REQ + 1], out package[IDX_CRC16_REQ]);
                    sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                    sensorAcq.Request = package;
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
            var data = rawData.Response;
            var module = data[0];
            if (module != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_DATA;
                return;
            }
            try
            {
                var speedofWind = this.Bytes2Float(data, IDX_SPEED);
                var azimuth = this.Bytes2Float(data, IDX_AZIMUTH);
                var speedOfSound = this.Bytes2Float(data, IDX_SpeedOfSound);
                var probesTemperature = this.Bytes2Float(data, IDX_ProbesTemperature);
                var raws = new double[] { speedofWind, azimuth };
                rawData.Data = new SensorData(raws, raws, raws)
                {
                    JsonResultData =
                        string.Format(
                            "{0}\"sensorId\":{1},\"data\":\"风速:{2} m/s,风向:{3} °\"{4}",
                            '{',
                            rawData.Sensor.SensorID,
                            speedofWind,
                            azimuth,
                            '}')
                };
                //rawData.Data = new Wind2dData(speedofWind, azimuth)
                //{
                //    JsonResultData =
                //        string.Format(
                //            "{0}\"sensorId\":{1},\"data\":\"风速:{2} m/s,风向:{3} °\"{4}",
                //            '{',
                //            rawData.Sensor.SensorID,
                //            speedofWind,
                //            azimuth,
                //            '}')
                //};
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("wind osl sensor [id:{0},m:{1}] parsedfailed ,received bytes{3},ERROR : {2}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message, ValueHelper.BytesToHexStr(rawData.Response));
            }
        }

        int IsValid(byte[] data)
        {
            if (data == null || data.Length != IDX_CRC16_RESP + 2)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            if (data[IDX_CMD] != 0x04)
                return (int)Errors.ERR_INVALID_DATA;
            byte[] crc16 = { data[IDX_CRC16_RESP], data[IDX_CRC16_RESP + 1] };
            byte crcHi;
            byte crcLo;
            ValueHelper.CheckCRC16(data, 0, IDX_CRC16_RESP, out crcHi, out crcLo);
            if ((crc16[0] == crcLo) && (crc16[1] == crcHi))
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_CRC;
        }

        private float Bytes2Float(byte[] data, int start)
        {
            var by = new byte[4];
            by[0] = data[start + 1];
            by[1] = data[start];
            by[2] = data[start + 3];
            by[3] = data[start + 2];
            return BitConverter.ToSingle(by, 0);
        }
    }
}