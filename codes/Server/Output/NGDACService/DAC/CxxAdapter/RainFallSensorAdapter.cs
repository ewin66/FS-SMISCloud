#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="RainFallSensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
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

    // Documents/PMO/02 产品协议/外购产品协议/安徽雨量计通信协议及范例（整理）.doc
    [SensorAdapter(Protocol = ProtocolType.RainFall)]
    public class RainFallSensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_FLAG = 0; //发送 为55H,接收为AAH
        private const int IDX_FLAMELEN = 1; // 包长
        private const int IDX_CMD = 2;
        private const int IDX_TIME = 3;
        private const int IDX_HOURSCOUNT = 7;
        private const int IDX_CHECK = 8;
        private const int IDX_RAINDATA = 7;

        #region Implementation of ISensorAdapter

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                DateTime time = DateTime.Now;
                byte[] package = new byte[10];
                package[IDX_FLAG] = 0x55;
                package[IDX_FLAMELEN] = 0x0A;
                package[IDX_CMD] = 0x00;
                package[IDX_TIME] = Convert.ToByte(time.Year.ToString().Substring(2, 2));
                package[IDX_TIME + 1] = (byte)time.Month;
                package[IDX_TIME + 2] = (byte)time.Day;
                if (time.Minute < 30)
                {
                    package[IDX_TIME + 3] = (byte)(time.Hour - 1);
                }
                else
                {
                    package[IDX_TIME + 3] = (byte)(time.Hour);
                }
                package[IDX_HOURSCOUNT] = 1;
                package[IDX_CHECK] = 0;
                // 每周进行一次时间同步 
                if (time.DayOfWeek == DayOfWeek.Sunday && (time.Hour == 0 && time.Minute < 30))
                {
                    package[IDX_CMD] = 0x10;
                    package[IDX_TIME + 3] = (byte)time.Minute;
                }
                package[IDX_CHECK + 1] = ValueHelper.CheckXor(package, 0, IDX_CHECK);
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
            var data = rawData.Response;
            rawData.ErrorCode = IsValid(data);
            if ((int)Errors.SUCCESS != rawData.ErrorCode)
                return;
            try
            {
                switch (data[IDX_CMD])
                {
                    case 0x00:
                        {
                            var temByte = new byte[2];
                            temByte[0] = data[IDX_RAINDATA];
                            temByte[1] = data[IDX_RAINDATA + 1];
                            var rain = BitConverter.ToInt16(temByte, 0) * 0.01;

                            var raws = new double[] { rain };
                            rawData.Data = new SensorData(raws, raws, raws)
                            {
                                JsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"小时雨量:{2} mm\"{3}", '{',
                                    rawData.Sensor.SensorID, rain, '}')
                            };

                            //rawData.Data = new RainFallData(rain)
                            //{
                            //    JsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"小时雨量:{2} mm\"{3}", '{',
                            //        rawData.Sensor.SensorID, rain, '}')
                            //};
                        }
                        break;
                    case 0x10:
                        rawData.ErrorCode = (int)Errors.ERR_RAINFALL_CLOCKSYNC;
                        break;
                }
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("rainfall sensor [id:{0},m:{1}] parsedfailed ,received bytes{3},ERROR : {2}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message, ValueHelper.BytesToHexStr(rawData.Response));
            }
        }

        public int IsValid(byte[] data)
        {
            if (data == null || data.Length != data[IDX_FLAMELEN])
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            if (data[IDX_FLAG] != 0xAA)
                return (int)Errors.ERR_INVALID_DATA;
            if (data[IDX_CMD] != 0 && data[IDX_CMD] != 0x10)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            var xor = data[data.Length - 1];
            if (xor == ValueHelper.CheckXor(data, 0, data.Length - 2))
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_CRC;
        }

        #endregion
    }
}