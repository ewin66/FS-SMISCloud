
namespace DataAnalyze.Test
{
    using FS.SMIS_Cloud.NGET.DataAnalyzer.Model;
    using FS.SMIS_Cloud.NGET.DataAnalyzer.Warning;

    using NUnit.Framework;

    [TestFixture]
    class WarningTester
    {
        [Test]
        [System.ComponentModel.Category("MANUAL")]
        public void TestSendWarning()
        {
            WarningHelper.SendWarning(
                17,
                2,
                new ThresholdAlarm(17) { AlarmDetails = new[] { new ThresholdAlarmDetail("x方向位移", 2) } });

            WarningHelper.SendWarning(
                17,
                2,
                new ThresholdAlarm(17) { AlarmDetails = new[] { new ThresholdAlarmDetail("x方向位移", 2) } });
        }
    }
}
