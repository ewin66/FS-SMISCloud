using System;
using FS.SMIS_Cloud.DAC.Model.Sensors;

namespace DAC.Test
{
    using FS.SMIS_Cloud.DAC.DAC;
    using FS.SMIS_Cloud.DAC.Model;
    using FS.SMIS_Cloud.DAC.Task;
    using FS.SMIS_Cloud.DAC.Util;

    using NUnit.Framework;

    [TestFixture]
    class ObjectHelperTester
    {
        [Test]
        public void TestDeepCopy()
        {
            DACTaskResult source = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            source.SensorResults.Add(
                new SensorAcqResult
                {
                    Elapsed = 100,
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 990 },
                    Data =
                        new Gps3dData(1, 2, 3, 0.1, 0.2, 0.3)
                        {
                            //Sensor = new Sensor{ SensorID = 990 }
                        },
                    Request = new byte[] { 1, 2, 3, 4, 5 }
                });

            DACTaskResult copy = ObjectHelper.DeepCopy(source);

            Assert.AreNotEqual(copy, source);

            Assert.AreEqual(copy.SensorResults[0].Sensor.SensorID, source.SensorResults[0].Sensor.SensorID);
            Assert.AreEqual(copy.SensorResults[0].Data.RawValues, source.SensorResults[0].Data.RawValues);
            Assert.AreEqual(copy.SensorResults[0].Data.ThemeValues[0], source.SensorResults[0].Data.ThemeValues[0]);

            copy.SensorResults[0].Data.ThemeValues[0] = 1000;
            Assert.AreNotEqual(copy.SensorResults[0].Data.ThemeValues[0], source.SensorResults[0].Data.ThemeValues[0]);

            copy.SensorResults[0].Sensor.SensorID = 1000;
            Assert.AreNotEqual(copy.SensorResults[0].Sensor.SensorID, source.SensorResults[0].Sensor.SensorID);

            (copy.SensorResults[0].Data as Gps3dData).ThemeValues[0] = 1000;
            Assert.AreNotEqual(copy.SensorResults[0].Data.ThemeValues, source.SensorResults[0].Data.ThemeValues);
        }
    }
}
