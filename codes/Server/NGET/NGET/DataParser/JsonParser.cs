// --------------------------------------------------------------------------------------------
// <copyright file="JsonParser.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2015 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20150329
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET.DataParser
{
    using System;
    using System.Linq;

    using FS.SMIS_Cloud.NGET.Model;

    using Newtonsoft.Json;

    public class JsonParser
    {
        public SensorAcqResult Parse(string data)
        {
            try
            {
                var json = JsonConvert.DeserializeObject<JsonData>(data);
                uint sensorId = json.S;
                Sensor sensor = new Sensor { SensorID = sensorId };
                double[] rv = json.RV;
                double[] pv = json.PV;
                double[] lv = json.LV;
                double?[] tv = json.TV;
                if (pv != null)
                {
                    tv = pv.Select(p => (double?)p).ToArray();
                }
                SensorAcqResult rslt = new SensorAcqResult
                                           {
                                               ErrorCode = json.R,
                                               AcqNum = json.N,
                                               AcqTime = new DateTime(1970, 1, 1).AddMilliseconds(json.T),
                                               Request = json.Q,
                                               Response = json.A,
                                               Sensor = sensor,
                                               Data = new SensorData(rv, pv, lv, tv)
                                           };
                return rslt;
            }
            catch (Exception e)
            {
                throw new Exception("sensor acq result format error!\njson:" + data, e);
            }
        }
    }

    public class JsonData
    {
        public uint S { get; set; }

        public int R { get; set; }

        public int N { get; set; }

        public double T { get; set; }

        public string[] Q { get; set; }

        public string[] A { get; set; }

        public double[] RV { get; set; }

        public double[] LV { get; set; }

        public double[] PV { get; set; }

        public double?[] TV { get; set; }
    }
}