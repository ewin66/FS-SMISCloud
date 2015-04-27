namespace DataAnalyze.Test
{
    using FS.SMIS_Cloud.NGET.DataAnalyzer.GradingPlan;
    using FS.SMIS_Cloud.NGET.DataAnalyzer.Model;
    using FS.SMIS_Cloud.NGET.Model;

    using NUnit.Framework;

    [TestFixture]
    class StructGradingTester
    {
        [Test]
        public void TestThemeEnforceGrading()
        {
            Assert.AreEqual(0, StructGradingPlanSet.Plans.Count);

            GradingSet.Add(
                new Sensor { SensorID = 17, FactorType = 10, StructId = 2 },
                new SensorAnalyzeResult { SensorId = 17, Score = 80, ThresholdAlarm = null });

            Assert.AreEqual(1, StructGradingPlanSet.Plans.Count);

            StructGradingPlanSet.Plans[0].ThemeScores[0].EnforceGrading();
            Assert.IsTrue(StructGradingPlanSet.Plans[0].ThemeScores[0].Completed);
            Assert.AreEqual(100, StructGradingPlanSet.Plans[0].ThemeScores[0].Score);
        }
    }
}
