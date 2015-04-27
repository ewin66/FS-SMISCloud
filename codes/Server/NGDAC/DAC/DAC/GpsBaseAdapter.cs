namespace FS.SMIS_Cloud.NGDAC.DAC
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Model.Sensors;
    using FS.SMIS_Cloud.NGDAC.Node;

    using log4net;

    using Newtonsoft.Json;

    public abstract class GpsBaseAdapter : IFileSensorAdapter
    {
        protected int _idxModule;   // 模块号
        protected int _idxTime;     // 采集时间
        protected int _idxDX;       // 数据X
        protected int _idxDY;       // 数据Y
        protected int _idxDH;       // 数据H
        protected ILog _log;        // 日志

        protected DateTimeFormatInfo _dtformat;     // 时间字段格式

        public virtual IEnumerable<byte[]> ReadData(Sensor si, string filePath)
        {
            return null;
        }


        public void UpdateSensorLastDataAcqTime(uint sensorId, DateTime acqTime)
        {
            try
            {
                string dat;
                if (System.IO.File.Exists("lastAcqDate.dat"))
                {
                    using (var fs = new FileStream("lastAcqDate.dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            dat = sr.ReadToEnd();
                            sr.Close();
                        }
                        fs.Close();
                    }
                    if (!string.IsNullOrEmpty(dat))
                    {
                        var d = JsonConvert.DeserializeObject<Dictionary<uint, DateTime>>(dat);
                        if (d.ContainsKey(sensorId))
                        {
                            d[sensorId] = acqTime;
                        }
                        else
                        {
                            d.Add(sensorId, acqTime);
                        }
                        System.IO.File.WriteAllText("lastAcqDate.dat", JsonConvert.SerializeObject(d));
                    }
                    else
                    {
                        var dic = new Dictionary<uint, DateTime>();
                        dic.Add(sensorId, acqTime);
                        System.IO.File.WriteAllText("lastAcqDate.dat", JsonConvert.SerializeObject(dic));
                    }
                }
                else
                {
                    var dic = new Dictionary<uint, DateTime>();
                    dic.Add(sensorId, acqTime);
                    System.IO.File.WriteAllText("lastAcqDate.dat", JsonConvert.SerializeObject(dic));
                }
            }
            catch (Exception e)
            {
                this._log.WarnFormat("get sensor:{0} last acq time error", sensorId);
            }
        }

        public DateTime? GetSensorLastDataAcqTime(uint sensorId)
        {
            try
            {
                if (System.IO.File.Exists("lastAcqDate.dat"))
                {
                    string dat;
                    using (
                        var fs = new FileStream(
                            "lastAcqDate.dat",
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            dat = sr.ReadToEnd();
                            sr.Close();
                        }
                        fs.Close();
                    }
                    if (!string.IsNullOrEmpty(dat))
                    {
                        var d = JsonConvert.DeserializeObject<Dictionary<uint, DateTime>>(dat);
                        return d.ContainsKey(sensorId) ? (DateTime?)d[sensorId] : null;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                this._log.WarnFormat("get sensor:{0} last acq time error", sensorId);
                return null;
            }
        }

        [Obsolete("文件适配器不再使用该方法", true)]
        public void Request(ref SensorAcqResult sensorAcq)
        {
            if (sensorAcq != null)
            {
                sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                sensorAcq.Request = null;
            }
        }

        public virtual void ParseResult(ref SensorAcqResult rawData)
        {
            string raw = System.Text.Encoding.Default.GetString(rawData.Response);
            string[] d = raw.Split(',');
            try
            {
                double x = Convert.ToDouble(d[this._idxDX]) * 1000;
                double y = Convert.ToDouble(d[this._idxDY]) * 1000;
                double height = Convert.ToDouble(d[this._idxDH]) * 1000;
                DateTime acqTime = Convert.ToDateTime(d[this._idxTime], this._dtformat);

                // 减去初值
                double iniX = rawData.Sensor.Parameters[0].Value;
                double iniY = rawData.Sensor.Parameters[1].Value;
                double iniZ = rawData.Sensor.Parameters[2].Value;
                if (iniX == 0 || iniY == 0 || iniZ == 0)
                {
                    // 查询第一条数据?
                }

                double cx = x - iniX;
                double cy = y - iniY;
                double cz = height - iniZ;

                // 计算偏角
                double drift = rawData.Sensor.Parameters[3].Value;
                cx *= Math.Cos(drift);
                cy *= Math.Cos(drift);
                if (rawData.Sensor.TableColums.Split(',').Length == 3)
                {
                    rawData.ResponseTime = acqTime;
                    rawData.Data = new Gps3dData(x, y, height, cx, cy, cz)
                    {
                        //Sensor = rawData.Sensor,
                        //AcqTime = acqTime,
                        //ResultCode = Errors.SUCCESS,
                        JsonResultData =
                            string.Format(
                                "{0}\"sensorId\":{1},\"data\":\"X方向位移:{2} mm,Y方向位移:{3} mm,Z方向位移:{4} mm\"{5}",
                                '{',
                                rawData.Sensor.SensorID,
                                x,
                                y,
                                height,
                                '}')
                    };
                }
                else
                {
                    rawData.ResponseTime = acqTime;
                    rawData.Data = new GpsHeightData(height, cz)
                    {
                        JsonResultData =
                            string.Format(
                                "{0}\"sensorId\":{1},\"data\":\"沉降值:{2} mm\"{3}",
                                '{',
                                rawData.Sensor.SensorID,
                                height,
                                '}')
                    };
                }
            }
            catch (Exception e)
            {
                this._log.Warn("Data parse error", e);
                rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
            }
        }
    }
}
