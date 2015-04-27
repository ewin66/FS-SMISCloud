namespace NGET.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using FS.SMIS_Cloud.NGET.Model;
    using FS.SMIS_Cloud.NGET.Util;

    using NUnit.Framework;

    [TestFixture]
    class ObjectHelperTester
    {
        [Test]
        public void TestDeepCopy()
        {
            List<SensorAcqResult> source = new List<SensorAcqResult>();
            source.Add(
                new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 990 },
                    Data =
                        new SensorData(new double[]{1, 2, 3} ,new double[]{0.1, 0.2, 0.3}, new double[]{1,2,3}),
                    Response = new []{ Encoding.Default.GetString(new byte[] { 1, 2, 3, 4, 5 }) },
                    AcqNum = 2,
                    AcqTime = new DateTime(2015, 3, 30)
                });

            var copy = ObjectHelper.DeepCopy(source);

            Assert.AreNotEqual(copy, source);

            Assert.AreEqual(copy[0].Sensor.SensorID, source[0].Sensor.SensorID);
            Assert.AreEqual(copy[0].Data.RawValues, source[0].Data.RawValues);
            Assert.AreEqual(copy[0].Data.ThemeValues[0], source[0].Data.ThemeValues[0]);

            copy[0].Data.ThemeValues[0] = 1000;
            Assert.AreNotEqual(copy[0].Data.ThemeValues[0], source[0].Data.ThemeValues[0]);

            copy[0].Sensor.SensorID = 1000;
            Assert.AreNotEqual(copy[0].Sensor.SensorID, source[0].Sensor.SensorID);

            copy[0].Data.ThemeValues[0] = 1000;
            Assert.AreNotEqual(copy[0].Data.ThemeValues, source[0].Data.ThemeValues);
        }
    }
}
