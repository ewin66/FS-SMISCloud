using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Task;
using NUnit.Framework;

namespace DAC.DataCalc.Test
{
    [TestFixture]
    class SettlementDoubleSysTester
    {
        private DACTaskResult res1 = null;
        private DACTaskResult res2 = null;
        [SetUp]
        public void SetUp()
        {
            setupDACTaskResult();
        }

        [Test]
        public void Test()
        {
            FS.SMIS_Cloud.DAC.DataCalc.DataCalc dac = new FS.SMIS_Cloud.DAC.DataCalc.DataCalc();
            dac.ProcessResult(res1);
            Assert.IsFalse(res1.SensorResults[0].IsOK);
            Assert.IsFalse(res1.SensorResults[1].IsOK);
            Assert.IsFalse(res1.SensorResults[2].IsOK);
            Assert.IsFalse(res1.SensorResults[3].IsOK);

            dac = null;
            dac = new FS.SMIS_Cloud.DAC.DataCalc.DataCalc();
            dac.ProcessResult(res2);
            Assert.IsNotNull(res2);

        }

        public void setupDACTaskResult()
        {
            res1 = new DACTaskResult();
            res1.Task=new DACTask()
            {
                DtuID = 185
            };
            res1.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 173 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2920,
                    Name = "远端S1"
                }
            });
            res1.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 163 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2922,
                    Name = "#1基点"
                }
            });
            res1.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 45 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2924,
                    Name = "#1S2"
                }
            });
            res1.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 156 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2925,
                    Name = "#2基点"
                }
            });


            res2 = new DACTaskResult();
            res2.Task = new DACTask()
            {
                DtuID = 192
            };
            res2.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 85 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2919,
                    Name = "远端基点"
                }
            });
            res2.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 62 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2921,
                    Name = "远端S2"
                }
            });
            res2.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 84 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2923,
                    Name = "#1S1"
                }
            });
            res2.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 152 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2926,
                    Name = "#2S1"
                }
            });
            res2.AddSensorResult(new SensorAcqResult()
            {
                Data = new SensorData(null, new double[] { 114 }, null),
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2927,
                    Name = "#2S2"
                }
            });
        }
    }
}
