namespace DAC.DataAnalyze.Test
{
    using System;
    using System.Threading;

    using FS.SMIS_Cloud.DAC.DataAnalyzer.GradingPlan;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Model;
    using FS.SMIS_Cloud.DAC.Model;

    using NUnit.Framework;

    [TestFixture]
    class GradingPlanTester
    {
        [Test]
        public void TestGradingSet()
        {
            Assert.AreEqual(0, StructGradingPlanSet.Plans.Count);

            GradingSet.Add(
                new Sensor { SensorID = 17, FactorType = 10, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 17, Score = 80, ThresholdAlarm = null });

            Assert.AreEqual(1, StructGradingPlanSet.Plans.Count);

            GradingSet.Add(
                new Sensor { SensorID = 18, FactorType = 10, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 18, Score = 80, ThresholdAlarm = null });

            GradingSet.Add(
                new Sensor { SensorID = 19, FactorType = 10, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 19, Score = 80, ThresholdAlarm = null });

            GradingSet.Add(
                new Sensor { SensorID = 20, FactorType = 10, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 20, Score = 80, ThresholdAlarm = null });

            GradingSet.Add(
                new Sensor { SensorID = 21, FactorType = 10, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 21, Score = 80, ThresholdAlarm = null });

            Assert.AreEqual(1, StructGradingPlanSet.Plans.Count);

            GradingSet.Add(
                new Sensor { SensorID = 22, FactorType = 10, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 22, Score = 80, ThresholdAlarm = null });

            GradingSet.Add(
                new Sensor { SensorID = 23, FactorType = 17, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 23, Score = 80, ThresholdAlarm = null });

            GradingSet.Add(
                new Sensor { SensorID = 24, FactorType = 17, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 24, Score = 80, ThresholdAlarm = null });

            GradingSet.Add(
                new Sensor { SensorID = 26, FactorType = 9, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 26, Score = 80, ThresholdAlarm = null });

            GradingSet.Add(
                new Sensor { SensorID = 27, FactorType = 9, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 27, Score = 80, ThresholdAlarm = null });

            Assert.AreEqual(0, StructGradingPlanSet.Plans.Count);
        }

        [Test]
        [Category("MANUAL")]
        public void TestGradingSetCleanUpTimeOutPlan()
        {
            Assert.AreEqual(0, StructGradingPlanSet.Plans.Count);

            GradingSet.Add(
                new Sensor { SensorID = 807, FactorType = 20, StructId = 71 },
                new SensorAnalyzeResult { SensorId = 807, Score = 80, ThresholdAlarm = null });

            Assert.AreEqual(1, StructGradingPlanSet.Plans.Count);

            Thread.Sleep(80000);

            Console.WriteLine(StructGradingPlanSet.Plans.Count);
            Assert.AreEqual(0, StructGradingPlanSet.Plans.Count);

            // another
            GradingSet.Add(
                new Sensor { SensorID = 807, FactorType = 20, StructId = 71 },
                new SensorAnalyzeResult { SensorId = 807, Score = 80, ThresholdAlarm = null });

            Assert.AreEqual(1, StructGradingPlanSet.Plans.Count);

            Thread.Sleep(80000);

            Console.WriteLine(StructGradingPlanSet.Plans.Count);
            Assert.AreEqual(0, StructGradingPlanSet.Plans.Count);
        }
    }
}
