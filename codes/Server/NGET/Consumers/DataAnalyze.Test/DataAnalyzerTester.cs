namespace DataAnalyze.Test
{
    using System;
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGET.DataAnalyzer;
    using FS.SMIS_Cloud.NGET.DataAnalyzer.Model;

    using NUnit.Framework;

    [TestFixture]
    class DataAnalyzerTester
    {
        private const string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";

        [TestFixtureSetUp]
        public void SetUp()
        {
            //DbAccessorHelper.Init(new MsDbAccessor(cs));
        }

        [Test]
        public void TestAnalyzeSensorData()
        {
            DataAnalyzer da = new DataAnalyzer();
            var analyzingData = new AnalyzingData { SensorId = 17, Data = new double?[]{ 1, 2, 3 } };
            var thresholds = new List<SensorThreshold>();

            var act0 = da.AnalyzeSensorData(analyzingData, thresholds);
            Assert.AreEqual(100, act0.Score);
            Assert.IsNull(act0.ThresholdAlarm);

            thresholds.Add(
                new SensorThreshold
                    {
                        SensorId = 17,
                        ItemId = 1,
                        LevelNumber = 3,
                        Thresholds =
                            new List<Threshold>
                                {
                                    new Threshold { Level = 1, Down = 1.5, Up = double.MaxValue },
                                    new Threshold { Level = 2, Down = 0.8, Up = 1.5 },
                                    new Threshold { Level = 3, Down = 0.5, Up = 0.8 }
                                }
                    });

            var act1 = da.AnalyzeSensorData(analyzingData, thresholds);
            Assert.AreEqual(100 - (int)((1 - (double)2 / 3) / 3 * 100), act1.Score);
            Assert.AreEqual(1, act1.ThresholdAlarm.AlarmDetails.Count);
            Console.WriteLine(act1.ThresholdAlarm);

            thresholds.Add(
                new SensorThreshold
                    {
                        SensorId = 17,
                        ItemId = 2,
                        LevelNumber = 4,
                        Thresholds =
                            new List<Threshold>
                                {
                                    new Threshold { Level = 1, Down = 1.5, Up = double.MaxValue },
                                    new Threshold { Level = 2, Down = 0.8, Up = 1 },
                                    new Threshold { Level = 3, Down = 0.5, Up = 0.8 },
                                    new Threshold { Level = 4, Down = 0.3, Up = 0.5 }
                                }
                    });

            var act2 = da.AnalyzeSensorData(analyzingData, thresholds);
            Assert.AreEqual(
                100 - (int)((1 - (double)2 / 3) / 3 * 100) - (int)((1 - (double)1 / 4) / 3 * 100),
                act2.Score);
            Assert.AreEqual(2, act2.ThresholdAlarm.AlarmDetails.Count);
            Console.WriteLine(act2.ThresholdAlarm);

        }

        [Test]        
        public void TestGetSensorThreshold()
        {
            DataAnalyzer da = new DataAnalyzer();
            var act1 = da.GetSensorThreshold(new uint[] { 17, 18 });
            Assert.AreEqual(2, act1.Count);            
            Assert.AreEqual(2, act1[0].LevelNumber);
            Assert.AreEqual(1, act1[1].LevelNumber);

            var act2 = da.GetSensorThreshold(null);
            Assert.AreEqual(0, act2.Count);

            var act3 = da.GetSensorThreshold(new uint[] {});
            Assert.AreEqual(0, act3.Count);
        }
    }
}
