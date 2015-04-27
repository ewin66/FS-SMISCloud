using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Task;
using NUnit.Framework;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace DAC.DataCalc.Test
{
    [TestFixture]
    class SettlementAlgorithmTester
    {
        private SensorGroup _sensorGroup = null;
        private IList<SensorAcqResult> _acqResults = null;

        [SetUp]
        public void SetUp()
        {
            _sensorGroup = InitialSensorGroup();
            _acqResults = InitialDataAcqResult();
        }

        [Test]
        public void TestCalcData()
        {
            var calcMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.DataCalc).GetMethod("Calc",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            DACTaskResult dtr=new DACTaskResult();
            foreach (var sensorAcqResult in _acqResults)
            {
                dtr.AddSensorResult(sensorAcqResult);
            }
            calcMethod.Invoke(null, new object[] { dtr, new SensorGroup[] { _sensorGroup } });

            Assert.AreEqual(0.0, _acqResults[0].Data.ThemeValues[0].Value, 0.000000001);
            Assert.AreEqual(-0.1, _acqResults[1].Data.ThemeValues[0].Value, 0.000000001);
            Assert.AreEqual(-0.2, _acqResults[2].Data.ThemeValues[0].Value, 0.000000001);
        }

        [Test]
        public void TestCalcDataWhenBasicMiss()
        {
            var calcMethod = typeof (FS.SMIS_Cloud.DAC.DataCalc.DataCalc).GetMethod("Calc",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var acqres = InitialDataAcqResultMissBasic();
            DACTaskResult dtr = new DACTaskResult();
            foreach (var sensorAcqResult in acqres)
            {
                dtr.AddSensorResult(sensorAcqResult);
            }
            calcMethod.Invoke(null, new object[] { dtr, new SensorGroup[] { _sensorGroup } });

            Assert.AreEqual(2, acqres.Count);
            Assert.IsFalse(acqres[0].IsOK);
            Assert.IsFalse(acqres[1].IsOK);
        }

        // for SETUP
        private SensorGroup InitialSensorGroup()
        {
            // using Reflection to get internal constructor method;
            var constructor = typeof(SensorGroup).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotEmpty(constructor);
            var sensorgroup = (SensorGroup)constructor[0].Invoke(new object[] { 1, GroupType.Settlement });

            var gp1 = new GroupItem
            {
                SensorId = 1
            };
            gp1.Paramters.Add("IsBase", 1);

            var gp2 = new GroupItem
            {
                SensorId = 2
            };
            gp2.Paramters.Add("IsBase", 0);

            var gp3 = new GroupItem
            {
                SensorId = 3
            };
            gp3.Paramters.Add("IsBase", 0);
            sensorgroup.Items.Add(gp1);
            sensorgroup.Items.Add(gp2);
            sensorgroup.Items.Add(gp3);

            return sensorgroup;
        }

        // for SETUP
        private IList<SensorAcqResult> InitialDataAcqResult()
        {
            var result = new List<SensorAcqResult>();
            var acq1 = new SensorAcqResult()
            {
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 1
                },
                Data = new PressureData(0,1.1)
            };

            var acq2 = new SensorAcqResult()
            {
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 2
                },
                Data = new PressureData(0, 1.2)
            };

            var acq3 = new SensorAcqResult()
            {
                ErrorCode = 0,
                Sensor = new Sensor()
                {
                    SensorID = 3
                },
                Data = new PressureData(0, 1.3)
            };

            result.Add(acq1);
            result.Add(acq2);
            result.Add(acq3);
            return result;
        }
        // miss base value
        private IList<SensorAcqResult> InitialDataAcqResultMissBasic()
        {
            var result = new List<SensorAcqResult>();

            var acq2 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 2
                },
                Data = new PressureData(0, 1.2)
            };

            var acq3 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 3
                },
                Data = new PressureData(0, 1.3)
            };

            result.Add(acq2);
            result.Add(acq3);
            return result;
        }
    }
}
