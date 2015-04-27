#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="Pressure_HS_SensorAdaper.cs" company="江苏飞尚安全监测咨询有限公司">
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
using System.Text;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    // Documents/PMO/02 产品协议/外购产品协议/2011版PTHXXX-RS485数字压力-液位变送器说....pdf
    [SensorAdapter(Protocol = ProtocolType.Pressure_HS)]
    public class Pressure_HS_SensorAdaper : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #region Implementation of ISensorAdapter

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                if ((sensorAcq.Sensor.ModuleNo >= 10 && sensorAcq.Sensor.ModuleNo < 65) || (sensorAcq.Sensor.ModuleNo > 90 && sensorAcq.Sensor.ModuleNo < 97) || sensorAcq.Sensor.ModuleNo > 122)
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                    return;
                }
                const string str = "#?OC;";
                string moduleId = sensorAcq.Sensor.ModuleNo.ToString();
                if (sensorAcq.Sensor.ModuleNo > 9)
                {
                    moduleId = ((char)sensorAcq.Sensor.ModuleNo).ToString();
                }
                var sendPressure = str.Replace("?", moduleId);
                var packet = Encoding.ASCII.GetBytes(sendPressure);
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
            try
            {
                Encoding enc = new ASCIIEncoding();
                var recbytes = enc.GetString(data, 1, 8);
                var pressure = double.Parse(recbytes);

                string jsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"压强:{2} kPa\"{3}", '{',
                    rawData.Sensor.SensorID, pressure, '}');
                double physicalQuantity = double.Epsilon;
                switch (rawData.Sensor.FormulaID)
                {
                    case 8:
                        var pressCul = pressure - rawData.Sensor.Parameters[1].Value;
                        if (ValueHelper.IsEqualZero(rawData.Sensor.Parameters[0].Value))
                            rawData.Sensor.Parameters[0].Value = 1;
                        var settlement = 1000*pressCul/(rawData.Sensor.Parameters[0].Value*9.8f);
                        physicalQuantity = settlement;
                        break;
                    case 10:
                        jsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"压强:{2} mH2O\"{3}", '{',
                            rawData.Sensor.SensorID, pressure, '}');
                        int type = (int) rawData.Sensor.Parameters[0].Value;
                        double waterLevel = pressure*100;
                        double len = rawData.Sensor.Parameters[1].Value; // 底长
                        switch (type)
                        {
                            case 0: //三角堰
                                physicalQuantity = (Math.Pow(waterLevel, 2.5)*
                                                    (0.0142 - ((int) (waterLevel/5))*0.0001));
                                break;
                            case 1: //矩形堰
                                physicalQuantity = (Math.Pow(waterLevel, 1.5)*len*0.0186);
                                break;
                            case 2: // 梯形堰
                                physicalQuantity =
                                    (Math.Pow(waterLevel, 1.5)*(len - 0.2*waterLevel)*0.01838);
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
                _logger.ErrorFormat("hs pressure sensor [id:{0},m{1}] paresfailed ,received bytes{3},ERROR :{2}",
                        rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message,
                        ValueHelper.BytesToHexStr(rawData.Response));
            }
        }

        #endregion

        public int IsValid(byte[] data)
        {
            if (data == null || data.Length != 10)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            if (data[0] != 0x2a && data[9] != 0x0D)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            return (int)Errors.SUCCESS;
        }
    }
}