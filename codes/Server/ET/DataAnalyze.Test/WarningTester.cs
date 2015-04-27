using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAC.DataAnalyze.Test
{
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Model;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Warning;

    using NUnit.Framework;

    [TestFixture]
    class WarningTester
    {
        [Test]
        [Category("MANUAL")]
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
