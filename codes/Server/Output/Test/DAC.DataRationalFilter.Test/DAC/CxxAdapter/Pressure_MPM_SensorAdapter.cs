#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="Pressure_MPM_SensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
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
using System.Linq;
using System.Reflection;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    // Documents/PMO/02 产品协议/外购产品协议/麦克公司数字化变送器通信指令集.pdf
    [SensorAdapter(Protocol = ProtocolType.Pressure_MPM)]
    public class Pressure_MPM_SensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #region Implementation of ISensorAdapter

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                if (sensorAcq.Sensor.ModuleNo > 99)
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                    return;
                }
                var sendstr = new StringBuilder();
                sendstr.Append(sensorAcq.Sensor.ModuleNo.ToString("00").Trim());
                sendstr.Append("RP");
                byte by = 0;
                var pp = new char[2];
                sendstr.Append(sensorAcq.Sensor.ChannelNo.ToString().Trim());
                foreach (var c in sendstr.ToString())
                {
                    by = (byte)((@by ^ c) & 0x000000ff);
                }
                pp[0] = ((@by & 0xf0) >> 4).ToString("X").ElementAt(0);
                pp[1] = (@by & 0x0f).ToString("X").ElementAt(0);
                sendstr.Insert(0, "$");
                sendstr.Append(pp);
                sendstr.Append((char)0x0D);
                sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                sensorAcq.Request = Encoding.ASCII.GetBytes(sendstr.ToString());
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
            Encoding enc = new ASCIIEncoding();
            var recbytes = enc.GetString(data);
            var module = recbytes.Substring(1, 2);
            if (int.Parse(module) != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            try
            {
                var pdata = recbytes.Substring(3, recbytes.Length - 6);
                var pressure = Convert.ToSingle(pdata);
                string jsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"压强:{2} kPa\"{3}", '{',
                  rawData.Sensor.SensorID, pressure, '}');
                double physicalQuantity = 0;
                switch (rawData.Sensor.FormulaID)
                {
                    case 8:
                        var pressCul = pressure - rawData.Sensor.Parameters[1].Value;
                        if (ValueHelper.IsEqualZero(rawData.Sensor.Parameters[0].Value))
                            rawData.Sensor.Parameters[0].Value = 1;
                        var settlement = 1000 * pressCul / (rawData.Sensor.Parameters[0].Value * 9.8f);
                        physicalQuantity = settlement;
                        break;
                    case 10:
                        jsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"米水柱:{2} mH2O\"{3}", '{',
                            rawData.Sensor.SensorID, pressure, '}');
                        int type = (int)rawData.Sensor.Parameters[0].Value;
                        double waterLevel = pressure * 100;
                        double len = rawData.Sensor.Parameters[1].Value; // 底长
                        switch (type)
                        {
                            case 0: //三角堰
                                physicalQuantity =
                                    (float)(Math.Pow(waterLevel, 2.5) * (0.0142 - ((int)(waterLevel / 5)) * 0.0001));
                                break;
                            case 1: //矩形堰
                                physicalQuantity = (float)(Math.Pow(waterLevel, 1.5) * len * 0.0186);
                                break;
                            case 2: // 梯形堰
                                physicalQuantity =
                                    (float)(Math.Pow(waterLevel, 1.5) * (len - 0.2 * waterLevel) * 0.01838);
                                break;
                        }
                        break;
                }

                var raws = new double[] { pressure };
                var phys = new double[] { physicalQuantity };
                rawData.Data = new SensorData(raws, phys, raws)
                {
                    JsonResultData = jsonResultData
                };

                //rawData.Data = new PressureData(pressure, physicalQuantity)
                //{
                //    JsonResultData = jsonResultData
                //};
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("mpm pressure [id:{0},m:{1}] parsedfailed ,received bytes{3},ERROR : {2}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message, ValueHelper.BytesToHexStr(rawData.Response));
            }
        }

        #endregion

        public int IsValid(byte[] data)
        {
            if (data == null || data.Length != 13)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            if (data[0] != 0x24 && data[12] != 0x0D)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            byte[] check = { data[data.Length - 3], data[data.Length - 2] };

            byte result = ValueHelper.CheckXor(data, 1, data.Length - 3);
            byte high = (byte)char.Parse((((byte)(result & 0xf0) >> 4).ToString("X")));
            byte low = (byte)char.Parse((((byte)(result & 0x0f)).ToString("X")));
            if ((check[0] == high) && (check[1] == low))
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_CRC;
        }
    }
}