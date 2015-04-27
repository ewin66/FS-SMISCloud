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
    [SensorAdapter(Protocol = ProtocolType.FTM50SSLaser)]
    public class FTM50SSmallLaserSensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Request(ref SensorAcqResult senAcq)
        {
            try
            {
                byte m;
                if (byte.TryParse(senAcq.Sensor.ModuleNo.ToString(), out m))
                {
                    byte[] package = { 0xCC, m };
                    senAcq.ErrorCode = (int)Errors.SUCCESS;
                    senAcq.Request = package;
                }
                else
                {
                    senAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    senAcq.Request = null;
                }
            }
            catch
            {
                senAcq.Request = null;
                senAcq.ErrorCode = (int)Errors.ERR_UNKNOW;
            }
        }

        public void ParseResult(ref SensorAcqResult rawData)
        {
            rawData.ErrorCode = IsValid(rawData.Response);
            if (rawData.ErrorCode != (int)Errors.SUCCESS)
                return;
            Encoding enc = new ASCIIEncoding();
            var recstr = enc.GetString(rawData.Response);
			int startIndex;
            try
            {
				if (!IsModule(rawData, out startIndex)){
					rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
					return;
				}
                var pdata = recstr.Substring(startIndex, 6);
                var len = Convert.ToInt32(pdata)/1000.0; //mm --> m
                var changelen = (len - rawData.Sensor.Parameters[0].Value)*1000; // m --> mm
                if (rawData.Sensor.FormulaID == 22)
                {
                    changelen *= -1;
                }
                double[] raws = { len };
                double[] phys = { changelen };
                rawData.Data = new SensorData(raws, phys,raws)
                {
                    JsonResultData =
                        string.Format("{0}\"sensorId\":{1},\"data\":\"测量距离:{2} m\"{3}", '{', rawData.Sensor.SensorID,
                            len, '}')
                };

                //rawData.Data = new LaserData(len, changelen)
                //{
                //    JsonResultData =
                //        string.Format("{0}\"sensorId\":{1},\"data\":\"测量距离:{2} m\"{3}", '{', rawData.Sensor.SensorID,
                //            len, '}')
                //};
            }
            catch (Exception ex)
            {
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                _logger.ErrorFormat("ftm laser sensor [Id:{0} m: {1}] pasefailed, received bytes{2}: {3}",
                    rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ValueHelper.BytesToHexStr(rawData.Response),
                    ex.Message);
            }
        }
		
		private bool IsModule(SensorAcqResult rawData,out int startIndex)
        {
            uint moduleNo;
            Encoding enc = new ASCIIEncoding();
            var recstr = enc.GetString(rawData.Response);
            if (recstr.IndexOf("U") > 1)
            {
                uint.TryParse(recstr.Substring(0, 2), out moduleNo);
                startIndex = recstr.IndexOf("U") + 1;
                return rawData.Sensor.ModuleNo == moduleNo ? true : false;
            }
            else
            {
                var d=Convert.ToUInt32(rawData.Response[0]);
                if (d >= 48 && d <= 57)
                {
                    uint.TryParse(recstr.Substring(0, 1), out moduleNo);
                    startIndex = recstr.IndexOf("U") + 1;
                    return rawData.Sensor.ModuleNo == moduleNo || (rawData.Sensor.ModuleNo==d?true:false);
                }
                else
                {
                    startIndex = recstr.IndexOf("U") + 1;
                    return rawData.Sensor.ModuleNo == d  ? true : false;
                }
            }
        }
		
        private int IsValid(byte[] data)
        {
		    Encoding enc = new ASCIIEncoding();
            var recbytes = enc.GetString(data);
            if (recbytes[1] == 'U' || recbytes[2] == 'U')
                return (int)Errors.SUCCESS;
            return (int)Errors.ERR_INVALID_DATA;
        }                
    }
}
