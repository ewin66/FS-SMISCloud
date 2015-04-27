using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Task;
using NUnit.Framework;

namespace PinghanDataUpload.Test
{
    [TestFixture]
    internal class UploadTester
    {
        [Test]
        public void TestUpolad()
        {
            var uploader = new DataUploader();
            uploader.ProcessResult(CreateDACResult());
        }

        private DACTaskResult CreateDACResult()
        {
            var nullData = new SensorAcqResult
            {
                ErrorCode = 103,
                Sensor = new Sensor {SensorID = 2115, ProductId = 145, Name = "test1"},
                Data = new LaserData(0.1, 0.1) 
            };
            nullData.Data.ThemeValues[0] = null;
            var rslt = new DACTaskResult();
            rslt.AddSensorResult(nullData);
            rslt.AddSensorResult(
                new SensorAcqResult
                {
                    ErrorCode = 103,
                    Sensor = new Sensor {SensorID = 2115, ProductId = 145, Name = "test1"},
                    Data = new LaserData(0.1, 0.1) 
                });
            rslt.AddSensorResult(
                new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = new Sensor {SensorID = 2115, ProductId = 145, Name = "test2"},
                    Data = new LaserData(0.2, 0.2) 
                });
            rslt.AddSensorResult(
                new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = new Sensor {SensorID = 2116, ProductId = 145, Name = "test4"},
                    Data = null
                });
            return rslt;
        }

        [Test]
        public void TestBasicConfig()
        {
            var config = new DataUploader().Config;
            Assert.AreEqual(2500, config.TimeOut);
            StringAssert.AreEqualIgnoringCase("http://117.34.104.24:10003/phxt/UploadActionWYSL", config.Url);
            Assert.AreEqual(1, config.EnableException);
            Assert.AreEqual(1, config.EnableDebug);
            StringAssert.AreEqualIgnoringCase(
                "http://218.3.150.106/ExternalSensorData.ashx?userName=pinghan&passWord=111111&themeId={0}&factorId={1}&sensorId={2}",
                config.ExceptionUrl);
        }

        [Test]
        public void TestItemsConfig()
        {
            var items = new DataUploader().ReloadSensorConfig();
            Assert.AreEqual(3, items.Count);
            Assert.IsTrue(items.ContainsKey(2115));
            Assert.IsTrue(items.ContainsKey(2116));
            Assert.IsTrue(items.ContainsKey(2109));
            //2115
            /**
             *   
              <sensor id="2115" enable="0"
              BDBH="PH-10"
              SDMC="鲜家坝隧道"
              SDZH="YK170＋766"
              ZYF="右"
              SBWZ="右洞第七截面_YK170＋766"
              SBSM=""
              SJLX="CJ" />
             */
            var item = items[2115];
            Assert.AreEqual(1, item.Enable);
            Assert.AreEqual(2115, item.Id);
            Assert.AreEqual("PH-10", item.BDBH);
            Assert.AreEqual(null, item.CJSJ);
            Assert.AreEqual(null, item.FHDZ);
            Assert.AreEqual("CJ", item.SJLX);
            Assert.AreEqual("右洞第七截面_YK170＋766", item.SBWZ);
            Assert.AreEqual("", item.SBSM);
            Assert.AreEqual("鲜家坝隧道", item.SDMC);
            Assert.AreEqual("右", item.ZYF);
            Assert.AreEqual("YK170＋766", item.SDZH);
        }

        [Test]
        [Category("MANUAL")]
        public void TestCmdA()
        {
            var args = new[] {"2115", "2015-02-01 00:00:00", "2015-02-28 23:59:59"};
            var result = new DataUploader().UploadHandler(args);
            StringAssert.AreEqualIgnoringCase(result, "Finished.");
        }

        [Test]
        [Category("MANUAL")]
        public void TestCmdB()
        {
            var args = new[] {"2109", "2015-02-01 00:00:00", "2015-02-28 23:59:59"};
            var result = new DataUploader().UploadHandler(args);
            StringAssert.AreEqualIgnoringCase(result, "Finished.");
        }

        [Test]
        [Category("MANUAL")]
        public void TestCmdC()
        {
            var args = new[] {"2115", "2015-02-01 00:00:00", "2015-02-28 23:59:59"};
            var result = new DataUploader().UploadHandler(args);
            StringAssert.AreEqualIgnoringCase(result, "Finished.");
        }

        [Test]
        public void TestArgsInvalid()
        {
            var du = new DataUploader();
            var result = du.UploadHandler(null);
            StringAssert.AreEqualIgnoringCase(result, du.CmdHelp());
            result = du.UploadHandler(new string[] {});
            StringAssert.AreEqualIgnoringCase(result, du.CmdHelp());
            result = du.UploadHandler(new[] {"2111"});
            StringAssert.AreEqualIgnoringCase(result, du.CmdHelp());
            result = du.UploadHandler(new[] {"2111", "12132", ""});
            StringAssert.AreEqualIgnoringCase(result, du.CmdHelp());
            result = du.UploadHandler(new[] {"2111", "2015-02-01 00:00:00", ""});
            StringAssert.AreEqualIgnoringCase(result, du.CmdHelp());
        }

        [Test]
        public void TestSpit()
        {
            var options = RegexOptions.None;
            var regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)", options);
            var input = @"pwd dd dd ""2015-02-01 00:00:00"" ""2015-02-28 23:59:59""";
            var result = (from Match m in regex.Matches(input)
                where m.Groups["token"].Success
                select m.Groups["token"].Value).ToArray();

            for (var i = 0; i < result.Count(); i++)
            {
                Debug.WriteLine("Token[{0}]: '{1}'", i, result[i]);
            }
        }

        [Test]
        public void TestExceptionUrl()
        {
            var loader = new DataUploader();
            var items = loader.ReloadSensorConfig();
            Assert.AreEqual(3, items.Count);
            //2115
            /**
             *   
              <sensor id="2115" enable="0"
              BDBH="PH-10"
              SDMC="鲜家坝隧道"
              SDZH="YK170＋766"
              ZYF="右"
              SBWZ="右洞第七截面_YK170＋766"
              SBSM=""
              SJLX="CJ" />
             */
            var item = items[2115];
            StringAssert.AreEqualIgnoringCase(
                "http://218.3.150.106/ExternalSensorData.ashx?userName=pinghan&passWord=111111&themeId=2&factorId=40&sensorId=2115",
                loader.CreateExceptionUrl(item));
            item = items[2116];
            StringAssert.AreEqualIgnoringCase(
                "http://218.3.150.106/ExternalSensorData.ashx?userName=pinghan&passWord=111111&themeId=2&factorId=40&sensorId=2116",
                loader.CreateExceptionUrl(item));
            item = items[2109];
            StringAssert.AreEqualIgnoringCase(
                "http://218.3.150.106/ExternalSensorData.ashx?userName=pinghan&passWord=111111&themeId=2&factorId=41&sensorId=2109",
                loader.CreateExceptionUrl(item));
        }
    }
}
