using System;
using System.Reflection;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    // 协议参考文档： “统一协议-V2012-04-28.xls”

    [SensorAdapter(Protocol = ProtocolType.TempHumidity)]
    public class TempHumiditySensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_FrameH = 0;   //B:帧头 去：FC，回 FD
        private const int IDX_DevType = 1;  //B:设备类型，温湿度=1
        private const int IDX_DevId = 2;    //S:设备ID (模块号）
        private const int IDX_PN = 4;       //B:项目编号
        private const int IDX_BC = 5;       //B:广播号
        private const int IDX_ORDER = 6;    //命令号
        private const int IDX_Humidity = 8; //short，Hum, 需除以100
        private const int IDX_Temp = 10;    //float，温度
        private const int IDX_EquipInfo = 26; //4字节，仪器位置信息描述
        private const int IDX_CRC8 = 30;      //加总异或，1-29
        private const int IDX_FRAMEL = 31;    // 去：0xCF,回：0xDF

        /// <summary>
        /// 编码： PC -> Sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
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
                byte[] buff = new byte[32];
                buff[IDX_FrameH] = 0xFC;
                buff[IDX_DevType] = 0x01;
                ValueHelper.WriteUShort_BE(buff, IDX_DevId, (ushort)sensorAcq.Sensor.ModuleNo);
                buff[IDX_ORDER] = 0x01;
                buff[IDX_CRC8] = ValueHelper.CheckCRC8(buff, 1, 29);
                buff[IDX_FRAMEL] = 0xCF;
                sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                sensorAcq.Request = buff;
            }
            catch
            {
                sensorAcq.Request = null;
                sensorAcq.ErrorCode = (int)Errors.ERR_UNKNOW;
            }
        }

        /// <summary>
        /// 解码： Sensor -> PC
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public void ParseResult(ref SensorAcqResult rawData)
        {
            byte[] data = rawData.Response;
            rawData.ErrorCode = IsValid(data);
            if ((int)Errors.SUCCESS != rawData.ErrorCode)
                return;
            var module = ValueHelper.GetShort_BE(data, IDX_DevId);
            if (module != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            try
            {
                var hum = (data[IDX_Humidity] << 8 | data[IDX_Humidity + 1]) / 100.0;
                var temp = ValueHelper.GetFloat_BE(data, IDX_Temp);
                var raws = new double[] { temp, hum };
                rawData.Data = new SensorData(raws, raws, raws)
                {
                    JsonResultData =
                        string.Format(
                            "{0}\"sensorId\":{1},\"data\":\"温度:{2} ℃,湿度:{3} %\"{4}",
                            '{',
                            rawData.Sensor.SensorID,
                            temp,
                            hum,
                            '}')
                };

                //rawData.Data = new TempHumidityData(temp, hum)
                //{
                //    JsonResultData =
                //        string.Format(
                //            "{0}\"sensorId\":{1},\"data\":\"温度:{2} ℃,湿度:{3} %\"{4}",
                //            '{',
                //            rawData.Sensor.SensorID,
                //            temp,
                //            hum,
                //            '}')
                //};
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat(
                    "temphumi sensor [id:{0},m:{1}] parsedfailed ,received bytes{3},ERROR : {2}",
                        rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message,
                        ValueHelper.BytesToHexStr(rawData.Response));
            }
        }


        public int IsValid(byte[] data)
        {
            if (data == null || data.Length != 32)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            if (@data[IDX_FrameH] != 0xFD || @data[IDX_DevType] != 0x01 || @data[IDX_FRAMEL] != 0xDF)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            if (data[IDX_CRC8] == ValueHelper.CheckCRC8(data, 1, 29))
            {
                return (int)Errors.SUCCESS;
            }
            return (int)Errors.ERR_CRC;
        }
    }
}
