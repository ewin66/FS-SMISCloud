namespace NGET.Test
{
    using System;
    using System.Collections.Generic;

    using FS.Service;
    using FS.SMIS_Cloud.NGET;
    using FS.SMIS_Cloud.NGET.Model;

    using NUnit.Framework;

    [TestFixture]
    class EtDataStatusConsumerTester
    {
        [Test]
        public void TestGetRangeByProductId()
        {
            var consumer = new DataRangeJudge(null);
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
            var consumer = new DataRangeJudge(null);
            Range[] ranges = null;

            var sb =
                consumer.JudgeDataOverRange(
                    new SensorAcqResult
                        {
                            Data =
                                new SensorData(
                                new double[] { 400 },
                                new double[] { 20 },
                                new double[] { 20 })
                        },
                    ranges);

            Assert.IsTrue(sb.Length == 0);
        }

        [Test]
        public void TestJudgeOverRange_False()
        {
            var consumer = new DataRangeJudge(null);
            var ranges = consumer.GetRangeByProductId(145);
            var sensorResult = new SensorAcqResult
                                   {
                                       Data =
                                           new SensorData(
                                           new double[] { 400 },
                                           new double[] { 20 },
                                           new double[] { 20 })
                                   };

            var sb = consumer.JudgeDataOverRange(sensorResult, ranges);

            Assert.IsTrue(sb.Length == 0);
            Assert.AreEqual(20, sensorResult.Data.ThemeValues[0]);
        }

        [Test]
        public void TestJudgeOverRange_True()
        {
            var consumer = new DataRangeJudge(null);
            var ranges = consumer.GetRangeByProductId(145);
            var sensorResult = new SensorAcqResult
                                   {
                                       Data =
                                           new SensorData(
                                           new double[] { 560, 20 },
                                           new double[] { 560, 20 },
                                           new double[] { 560, 20 })
                                   };

            var sb = consumer.JudgeDataOverRange(sensorResult, ranges);

            Assert.IsFalse(sb.Length == 0);
            Assert.AreEqual("标距采集值:[560]超出量程[0~500]", sb);
            Assert.IsNull(sensorResult.Data.ThemeValues[0]);
        }

        [Test]
        public void TestJudgeOverRange_True_TwoRanges()
        {
            var consumer = new DataRangeJudge(null);
            var ranges = consumer.GetRangeByProductId(70);
            var sensorResult = new SensorAcqResult
                                   {
                                       Data =
                                           new SensorData(
                                           new double[] { 180, -2 },
                                           new double[] { 180, -2 },
                                           new double[] { 180, -2 })
                                   };

            var sb = consumer.JudgeDataOverRange(sensorResult, ranges);

            Assert.IsFalse(sb.Length == 0);
            Assert.AreEqual("温度采集值:[180]超出量程[-40~125],湿度采集值:[-2]超出量程[0~100]", sb);
            Assert.IsNull(sensorResult.Data.ThemeValues[0]);
            Assert.IsNull(sensorResult.Data.ThemeValues[1]);
        }

        [Test]
        public void TestProcessResult()
        {
            var rslt = new List<SensorAcqResult>();
            rslt.Add(
                new SensorAcqResult
                {
                    ErrorCode = 103,
                    Sensor = new Sensor { SensorID = 1, ProductId = 145 },
                    Data = new SensorData(
                        new double[] { 560, 20 },
                        new double[] { 560, 20 },
                        new double[] { 560, 20 })
                });
            rslt.Add(
                new SensorAcqResult
                {
                    Sensor = new Sensor { SensorID = 2, ProductId = 145 },
                    Data = new SensorData(
                        new double[] { 180, -2 },
                        new double[] { 180, -2 },
                        new double[] { 180, -2 })
                });
            rslt.Add(
                new SensorAcqResult
                {
                    Sensor = new Sensor { SensorID = 3, ProductId = 70 },
                    Data = new SensorData(
                        new double[] { 180, -2 },
                        new double[] { 180, -2 },
                        new double[] { 180, -2 })
                });

            new DataRangeJudge(null)
                .ProcessResult(rslt);

            Assert.Pass();
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
