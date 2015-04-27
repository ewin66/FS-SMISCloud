using System;
using FS.Service;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Node;
using Newtonsoft.Json;

namespace ET.Test
{
    using FS.SMIS_Cloud.DAC.DAC;
    using FS.SMIS_Cloud.DAC.Model;
    using FS.SMIS_Cloud.DAC.Task;

    using NUnit.Framework;

    using FS.SMIS_Cloud.ET;
    using FS.SMIS_Cloud.DAC.Accessor.MSSQL;

    [TestFixture]
    class EtDataStatusConsumerTester
    {
        [Test]
        public void TestGetRangeByProductId()
        {
            var consumer = new EtDataStatusConsumer(null);
            var ranges = consumer.GetRangeByProductId(145);

            Assert.AreEqual(4, ranges.Length);
            Assert.AreEqual("标距", ranges[0].Name);
            Assert.AreEqual(500, ranges[0].Upper);
            Assert.AreEqual(0, ranges[0].Lower);
            Assert.IsEmpty(ranges[1].Name);
            Assert.IsNull(ranges[1].Upper);
            Assert.IsNull(ranges[1].Lower);
        }

        [Test]
        public void TestJudgeOverRange_WithNullRange()
        {
            var consumer = new EtDataStatusConsumer(null);
            Range[] ranges = null;

            var sb = consumer.JudgeDataOverRange(new SensorAcqResult { Data = new LVDTData(400, 20) }, ranges);

            Assert.IsTrue(sb.Length == 0);
        }

        [Test]
        public void TestJudgeOverRange_False()
        {
            var consumer = new EtDataStatusConsumer(null);
            var ranges = consumer.GetRangeByProductId(145);
            var sensorResult = new SensorAcqResult { Data = new LVDTData(400, 20) };

            var sb = consumer.JudgeDataOverRange(sensorResult, ranges);

            Assert.IsTrue(sb.Length == 0);
            Assert.AreEqual(20, sensorResult.Data.ThemeValues[0]);
        }

        [Test]
        public void TestJudgeOverRange_True()
        {
            var consumer = new EtDataStatusConsumer(null);
            var ranges = consumer.GetRangeByProductId(145);
            var sensorResult = new SensorAcqResult { Data = new LVDTData(560, 20) };

            var sb = consumer.JudgeDataOverRange(sensorResult, ranges);

            Assert.IsFalse(sb.Length == 0);
            Assert.AreEqual("标距采集值:[560]超出量程[0~500]", sb);
            Assert.IsNull(sensorResult.Data.ThemeValues[0]);
        }

        [Test]
        public void TestJudgeOverRange_True_TwoRanges()
        {
            var consumer = new EtDataStatusConsumer(null);
            var ranges = consumer.GetRangeByProductId(70);
            var sensorResult = new SensorAcqResult { Data = new TempHumidityData(180, -2) };

            var sb = consumer.JudgeDataOverRange(sensorResult, ranges);

            Assert.IsFalse(sb.Length == 0);
            Assert.AreEqual("温度采集值:[180]超出量程[-40~125],湿度采集值:[-2]超出量程[0~100]", sb);
            Assert.IsNull(sensorResult.Data.ThemeValues[0]);
            Assert.IsNull(sensorResult.Data.ThemeValues[1]);
        }

        [Test]
        public void TestProcessResult()
        {
            DbAccessorHelper.Init(new MsDbAccessor("server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false"));
            IDtuServer _dacServer;
            _dacServer = new GprsDtuServer(5005);
            DACTaskManager Dtm = new DACTaskManager(_dacServer, DbAccessorHelper.DbAccessor.QueryDtuNodes(), null); 
            var rslt = new DACTaskResult();
            rslt.AddSensorResult(
                new SensorAcqResult { ErrorCode = 103, Sensor = new Sensor { SensorID = 1, ProductId = 145 }, Data = new LVDTData(560, 20) });
            rslt.AddSensorResult(
                new SensorAcqResult { Sensor = new Sensor { SensorID = 2, ProductId = 145 }, Data = new LVDTData(560, 20) });
            rslt.AddSensorResult(
                new SensorAcqResult { Sensor = new Sensor { SensorID = 3, ProductId = 70 }, Data = new TempHumidityData(180, -2) });

            new EtDataStatusConsumer(null)
                .ProcessResult(rslt);

            Assert.Pass();
        }

        [Test]
        public void GetAbnormalSensorCountTest()
        {
            var consumer =new EtDataStatusConsumer(null);

            consumer.AddOrUpdateSensorStatus(new SensorAcqResult
            {
                DtuCode = "12345678",
                ErrorCode = (int)Errors.ERR_DTU_TIMEOUT,
                ErrorMsg = "Timeout",
                ResponseTime = DateTime.Now,
                Sensor = new Sensor()
                {
                    StructId = 1,//88836080
                    SensorID = 1,
                    DtuCode = "12345678",
                    Name = "Test1"
                }
            });

            Assert.AreEqual(1,consumer.GetAbnormalSensorCount(1));
        }

        [Test]
        public void GetJsonstructs()
        {
            string jsonstr = "{\"structIds\": [2,3,4]}";
            var msg = newMessage(jsonstr);
            MyClass obj = JsonConvert.DeserializeObject<MyClass>(jsonstr);
            Assert.AreEqual(3,obj.structIds.Length);
            Assert.AreEqual(2, obj.structIds[0]);
            Assert.AreEqual(3, obj.structIds[1]);
            Assert.AreEqual(4, obj.structIds[2]);
            obj = JsonConvert.DeserializeObject<MyClass>(msg.Body.ToString());
            Assert.AreEqual(3,obj.structIds.Length);
            Assert.AreEqual(2, obj.structIds[0]);
            Assert.AreEqual(3, obj.structIds[1]);
            Assert.AreEqual(4, obj.structIds[2]);

            AbnormalCount[] abs = new AbnormalCount[2];
            abs[1]=new AbnormalCount
            {
                abnormalSensorCount = 1,
                structId = 1
            };
            abs[0] = new AbnormalCount {structId = 2, abnormalSensorCount = 2};
            abs = null;
            var josj=JsonConvert.SerializeObject(abs);
            Console.WriteLine(josj);

        }

        static FsMessage newMessage(string body)
        {
            Guid token = Guid.NewGuid();
            Guid msgId = Guid.NewGuid();
            FsMessage msg = new FsMessage();
            msg.Header = new FsMessageHeader();
            msg.Header.A = "GET";
            msg.Header.D = "LOG";
            msg.Header.L = 20;
            msg.Header.R = "log";
            msg.Header.S = "中文";
            msg.Header.T = token;
            msg.Header.U = msgId;
            msg.Body = body;
            return msg;
        }

        public class MyClass
        {
            public int[] structIds { get; set; }
        }


    }
}
