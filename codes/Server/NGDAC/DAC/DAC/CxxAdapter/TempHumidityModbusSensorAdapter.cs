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

    [SensorAdapter(Protocol = ProtocolType.ModbusTempHumi)]
    public class TempHumidityModbusSensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_DevType = 0; // 设备类型 振弦 10 
        private const int IDX_DevId = 2; // 模块号 
        private const int IDX_CMD = 4;   // 采集命令=0x01
        private const int IDX_DataField = 5; // 
        private const int SensorType = 1; //温度 

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
                var packet = new byte[8];
                ValueHelper.WriteShort_BE(packet, IDX_DevType, (short)SensorType);
                ValueHelper.WriteUShort_BE(packet, IDX_DevId, (ushort)sensorAcq.Sensor.ModuleNo);
                packet[IDX_CMD] = 0x01; // 采集命令
                ValueHelper.CheckCRC16(packet, 0, IDX_DataField + 1, out packet[IDX_DataField + 1],
                    out packet[IDX_DataField + 2]);
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
            var module = ValueHelper.GetShort_BE(data, IDX_DevId);
            if (module != rawData.Sensor.ModuleNo)
            {
                rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                return;
            }
            try
            {

                switch (data[IDX_CMD])
                {
                    case 0x81:
                        var temp = ValueHelper.GetFloat_BE(data, IDX_DataField);
                        var humidity = ValueHelper.GetFloat_BE(data, IDX_DataField + 4);

                        var raws = new double[] {temp, humidity};
                        rawData.Data = new SensorData(raws, raws, raws)
                        {
                            JsonResultData =
                                string.Format(
                                    "{0}\"sensorId\":{1},\"data\":\"温度:{2} ℃,湿度:{3} %\"{4}",
                                    '{',
                                    rawData.Sensor.SensorID,
                                    temp,
                                    humidity,
                                    '}')
                        };
                        return;
                    case 0xc0:
                        rawData.ErrorCode = ModbusErrorCode.GetErrorCode(rawData.Response[IDX_CMD + 1]);
                        return;
                }
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int) Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat(
                    "ModBus temphumi sensor [id:{0},m:{1}] parsedfailed ,received bytes{3},ERROR : {2}",
                    rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message,
                    ValueHelper.BytesToHexStr(rawData.Response));
            }
        }

        private int IsValid(byte[] data)
        {
            if ((data[IDX_DevType] << 8 | data[IDX_DevType + 1]) != SensorType)
            {
                return (int)Errors.ERR_INVALID_DATA;
            }
            byte[] crc16 = { data[data.Length - 2], data[data.Length - 1] };
            byte crcHi;
            byte crcLo;
            ValueHelper.CheckCRC16(data, 0, data.Length - 2, out crcHi, out crcLo);
            if ((crc16[0] == crcHi) && (crc16[1] == crcLo))
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_CRC;
        }
    }
}
