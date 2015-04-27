#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="FTM50sLaser_SensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141111 by LINGWENLONG .
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

using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    [SensorAdapter(Protocol = ProtocolType.GLSB40I70Laser)]
    public class GLSB40I70Laser_SensorAdapter : ISensorAdapter 
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly char[] DataChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                byte module;
                if (byte.TryParse(sensorAcq.Sensor.ModuleNo.ToString(), out module))
                {
                    byte[] packBytes = { (byte)sensorAcq.Sensor.ModuleNo, 0x06, 0x02, 0 };
                    ValueHelper.ICMPCheckSum(packBytes, 0, 3, out packBytes[3]);
                    sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                    sensorAcq.Request = packBytes;
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
            rawData.ErrorCode = IsValid(rawData);
            if (rawData.ErrorCode != (int)Errors.SUCCESS)
                return;
            uint moudle = rawData.Response[0];
            if (moudle != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            Encoding enc = new ASCIIEncoding();
            string datastr = enc.GetString(rawData.Response);
            try
            {
                datastr = datastr.Substring(3, datastr.Length - 4);
                if (datastr.ToCharArray().All(c => DataChars.Contains(c)))
                {
                    double len = Convert.ToDouble(datastr);
                    double changelen = len - rawData.Sensor.Parameters[0].Value;
                    if (rawData.Sensor.FormulaID == 22)
                    {
                        changelen *= -1;
                    }
                    double[] raws = { len };
                    double[] phys = { changelen };

                    rawData.Data = new SensorData(raws, phys, raws)
                    {
                        JsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"测量距离:{2} m\"{3}", '{', rawData.Sensor.SensorID, len, '}')
                    };
                    //rawData.Data = new LaserData(len, changelen)
                    //{
                    //    JsonResultData =
                    //        string.Format("{0}\"sensorId\":{1},\"data\":\"测量距离:{2} m\"{3}", '{',
                    //            rawData.Sensor.SensorID, len, '}')
                    //};
                }
                else
                {
                    rawData.ErrorCode = (int)Errors.ERR_GLS_RECEIVE;
                }
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("GLSB Laser sensor [Id:{0} m:{1}]  parsedfailed,received bytes{2} ERROR: {3}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo,
                                ValueHelper.BytesToHexStr(rawData.Response), ex.Message);
            }
        }

        public int IsValid(SensorAcqResult rawData)
        {
            if (rawData.Response.Length == 11 && rawData.Response[1] == 0x06 &&
                   rawData.Response[2] == 0x82)
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_INVALID_DATA;
        }
    }
}