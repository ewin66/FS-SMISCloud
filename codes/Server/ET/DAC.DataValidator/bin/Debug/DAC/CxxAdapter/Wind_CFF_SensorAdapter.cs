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
    [SensorAdapter(Protocol = ProtocolType.Wind_CCF)]
    public class Wind_CFF_SensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                if (sensorAcq.Sensor.ModuleNo > 99 || sensorAcq.Sensor.ModuleNo <= 0)
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                    return;
                }
                StringBuilder str = new StringBuilder("#");
                str.Append(sensorAcq.Sensor.ModuleNo.ToString("00")).Append("r").Append("\r");
                var by = Encoding.UTF8.GetBytes(str.ToString());
                sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                sensorAcq.Request = by;
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
                var str = Bytes2String(data);
                var arr = str.Split(',');
                var windSpeed = float.Parse(arr[0]);
                var windDirect = float.Parse(arr[1]);
                var raws = new double[] { windSpeed, windDirect };

                rawData.Data = new SensorData(raws, raws, raws)
                {
                    JsonResultData =
                        string.Format(
                            "{0}\"sensorId\":{1},\"data\":\"风速:{2} m/s,风向:{3} °\"{4}",
                            '{',
                            rawData.Sensor.SensorID,
                            windSpeed,
                            windDirect,
                            '}')
                };
                //rawData.Data = new Wind2dData(windSpeed, windDirect)
                //{
                //    JsonResultData =
                //        string.Format(
                //            "{0}\"sensorId\":{1},\"data\":\"风速:{2} m/s,风向:{3} °\"{4}",
                //            '{',
                //            rawData.Sensor.SensorID,
                //            windSpeed,
                //            windDirect,
                //            '}')
                //};
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("wind cff sensor [id:{0},m:{1}] parsedfailed ,received bytes{3},ERROR : {2}", rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ex.Message, ValueHelper.BytesToHexStr(rawData.Response));
            }
        }

        private int IsValid(byte[] data)
        {
            if (data == null || data[0] != 0x3E)
                return (int)Errors.ERR_INVALID_DATA;
            return (int)Errors.SUCCESS;
        }

        private string Bytes2String(byte[] data)
        {
            var by = new byte[data.Length - 3];
            for (int i = 0; i < data.Length - 3; i++)
            {
                by[i] = data[i + 1];
            }

            return Encoding.Default.GetString(by);
        }
    }
}
