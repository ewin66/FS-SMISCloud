namespace NGDACService.Test
{
    using System;

    using FS.Service;
    using FS.SMIS_Cloud.NGDAC;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Model.Sensors;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Task;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    class DataStatusConsumerTester
    {
        [Test]
        public void TestProcessResult()
        {
            var rslt = new DACTaskResult();
            rslt.AddSensorResult(
                new SensorAcqResult { ErrorCode = 103, Sensor = new Sensor { SensorID = 1, ProductId = 145 }, Data = new LVDTData(560, 20) });
            rslt.AddSensorResult(
                new SensorAcqResult { Sensor = new Sensor { SensorID = 2, ProductId = 145 }, Data = new LVDTData(560, 20) });
            rslt.AddSensorResult(
                new SensorAcqResult { Sensor = new Sensor { SensorID = 3, ProductId = 70 }, Data = new TempHumidityData(180, -2) });

            new DataStatusConsumer(null)
                .ProcessResult(rslt);

            Assert.Pass();
        }

        [Test]
        public void GetAbnormalSensorCountTest()
        {
            var consumer =new DataStatusConsumer(null);

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
            
            msg = newMessage("{\"structId\":3}");
          var i =  msg.BodyValue<uint>("structId");
            Console.WriteLine(i);
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
