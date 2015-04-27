using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers;
using NUnit.Framework;
using Newtonsoft.Json;

namespace UnitTestProject
{
    [TestFixture]
    public class UnitTest
    {
        [Test]
        public void TestMethod1()
        {
            WebClient request = new WebClient();
            string result;
            using (var response = request.OpenRead("http://192.168.1.110:8021/struct/4/factor/10/data-validate"))
            {
                using (var sr = new StreamReader(response))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                }
            }
            List<FilterConfig> json = JsonConvert.DeserializeObject<List<FilterConfig>>(result);
            Assert.IsNotNull(json);
            Assert.AreEqual(48, json.Count);
        }
        [Test]
        public void TestMethod2()
        {
            ConfigInfo configInfo = new ConfigInfo()
            {
                SensorId = 29,
                ItemId = 1,
                RvEnabled = true,
                RvLower = 10,
                RvUpper = 20,
                SvEnabled = true,
                SvWindowSize = 120,
                SvKt=12,
                SvDt = 13,
                SvRt=14
            };
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://192.168.1.110:8021/sensor/data-validate/config");
            request.Method = "POST";
            string json = JsonConvert.SerializeObject(configInfo);
            byte[] data = Encoding.UTF8.GetBytes(json);
            request.ContentLength = data.Length;
            request.ContentType = "application/json";
            using (var req = request.GetRequestStream())
            {
                req.Write(data, 0, data.Length);
                req.Close();
            }
            var response = request.GetResponse() as HttpWebResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(202, (int)response.StatusCode);
        }
    }
}
