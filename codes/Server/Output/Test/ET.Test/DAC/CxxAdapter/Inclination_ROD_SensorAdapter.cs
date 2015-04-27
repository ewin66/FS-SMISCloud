#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="InclinometrySensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
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
using FS.SMIS_Cloud.NGDAC.DAC.CxxAdapter;
using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    //协议参考文档： Documents/PMO/02 产品协议/我司产品协议/通信协议/ModBus统一通信协议 .xls
    [SensorAdapter(Protocol = ProtocolType.Inclinometer_ROD)]
    public class Inclination_ROD_SensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_DevType = 0; // 设备类型 杆式 22 
        private const int IDX_DevId = 2; // 模块号 
        private const int IDX_CMD = 4; // 采集命令=0x01
        private const int IDX_DataField = 5; // 
        private const int SensorType = 22; //杆式测斜

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
                var package = new byte[7];
                ValueHelper.WriteShort_BE(package, IDX_DevType, (short)SensorType);
                ValueHelper.WriteUShort_BE(package, IDX_DevId, (ushort)sensorAcq.Sensor.ModuleNo);
                package[IDX_CMD] = 0x01; //采集命令
                ValueHelper.CheckCRC16(package, 0, 5, out package[IDX_DataField], out package[IDX_DataField + 1]);
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
            rawData.ErrorCode = IsValid(rawData.Response);
            if (rawData.ErrorCode != (int)Errors.SUCCESS)
                return;
            short mno = ValueHelper.GetShort_BE(rawData.Response, 2);
            if (mno != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            switch (rawData.Response[IDX_CMD])
            {
                case 0x81: // 采集命令回复
                    try
                    {
                        var surveyXAng = ValueHelper.GetInt_BE(rawData.Response, 13)/1000000f;
                        var surveyYAng = ValueHelper.GetInt_BE(rawData.Response, 17)/1000000f;

                        double changeX = 0;
                        double changeY = 0;
                        if (rawData.Sensor.Parameters != null && rawData.Sensor.Parameters.Count == 3)
                        {
                            double xinit = rawData.Sensor.Parameters[0].Value;
                            double yinit = rawData.Sensor.Parameters[1].Value;
                            double len = rawData.Sensor.Parameters[2].Value;
                            var xvalue = (len*Math.Sin(surveyXAng*Math.PI/180)) - (len*Math.Sin(xinit*Math.PI/180));
                            var yvalue = (len*Math.Sin(surveyYAng*Math.PI/180)) - (len*Math.Sin(yinit*Math.PI/180));
                            changeX = xvalue;
                            changeY = yvalue;
                        }

                        var raws = new double[] { surveyXAng, surveyYAng };
                        var phys = new double[] { changeX, changeY };
                        
                        rawData.Data = new SensorData(raws, phys, raws)
                        {
                            JsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"X方向角度:{2} °,Y方向角度:{3} °\"{4}", '{', rawData.Sensor.SensorID, surveyXAng, surveyYAng, '}')
                        };
                    }
                    catch (Exception ex)
                    {
                        rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                        _logger.ErrorFormat(
                                "rod Inclination sensor [Id:{0} m: {1}] parsedfailed,received bytes{2}, ERROR: {3}",
                                rawData.Sensor.SensorID, rawData.Sensor.ModuleNo,
                                ValueHelper.BytesToHexStr(rawData.Response), ex.Message);
                    }
                    break;
                case 0xC0: // 回复错误码
                    rawData.ErrorCode = ModbusErrorCode.GetErrorCode(rawData.Response[IDX_CMD + 1]);
                    break;
            }
        }

        public int IsValid(byte[] data)
        {
            if (data == null)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }

            if ((data[IDX_DevType] << 8 | data[IDX_DevType + 1]) != SensorType)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            if (data[IDX_CMD] != 0x81 && data[IDX_CMD] != 0xC0)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            byte[] crc16 = {data[data.Length - 2], data[data.Length - 1]};
            byte crcHi;
            byte crcLo;
            ValueHelper.CheckCRC16(data, 0, data.Length - 2, out crcHi, out crcLo);
            if ((crc16[0] != crcHi) || (crc16[1] != crcLo))
                return (int)Errors.ERR_CRC;
            return (int)Errors.SUCCESS;
        }
    }
}