using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using NUnit.Framework;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace DAC.DataCalc.Test
{
    [TestFixture]
    class DeepDisplacementAlgorithmTester
    {
        private SensorGroup _sensorGroup = null;
        private IList<SensorAcqResult> _acqResults = null;

        [SetUp]
        public void SetUp()
        {
            _acqResults = InitialDataAcqResult();
            _sensorGroup = InitialSensorGroup();
        }

        [Test]
        public void TestCalcData()
        {
            var t = Assembly.Load("DAC.DataCalc").GetType("FS.SMIS_Cloud.DAC.DataCalc.Algorithm.DeepDisplacementAlgorithm");
            var constructors = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNotEmpty(constructors);
            var objAlgorithm=constructors[0].Invoke(new object[] { _sensorGroup });
            t.GetMethod("CalcData", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Invoke(objAlgorithm, new object[] { _acqResults });

            Assert.AreEqual(1.0, _acqResults[0].Data.ThemeValues[2].Value, 0.000000001);
            Assert.AreEqual(1.1, _acqResults[0].Data.ThemeValues[3].Value, 0.000000001);
            Assert.AreEqual(3.0, _acqResults[1].Data.ThemeValues[2].Value, 0.000000001);
            Assert.AreEqual(3.2, _acqResults[1].Data.ThemeValues[3].Value, 0.000000001);
            Assert.AreEqual(6.0, _acqResults[2].Data.ThemeValues[2].Value, 0.000000001);
            Assert.AreEqual(6.3, _acqResults[2].Data.ThemeValues[3].Value, 0.000000001);
        }

        // for SETUP
        private IList<SensorAcqResult> InitialDataAcqResult()
        {
            var result = new List<SensorAcqResult>();
            var acq1 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 11
                },
                Data = new InclinationData(0, 0, 1.0, 1.1)
            };

            var acq2 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 12
                },
                Data = new InclinationData(0, 0, 2.0, 2.1)
            };

            var acq3 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 13
                },
                Data = new InclinationData(0, 0, 3.0, 3.1)
            };

            result.Add(acq1);
            result.Add(acq2);
            result.Add(acq3);
            return result;
        }
        
        // for SETUP
        private SensorGroup InitialSensorGroup()
        {
            // using Reflection to get internal constructor method;
            var constructor = typeof(SensorGroup).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotEmpty(constructor);
            var sensorgroup = (SensorGroup)constructor[0].Invoke(new object[] { 2, GroupType.Inclination });

            var gp1 = new GroupItem
            {
                SensorId = 11
            };
            gp1.Paramters.Add("DEPTH", 10);
            gp1.Value = _acqResults[0];

            var gp2 = new GroupItem
            {
                SensorId = 12
            };
            gp2.Paramters.Add("DEPTH", 20);
            gp2.Value = _acqResults[1];

            var gp3 = new GroupItem
            {
                SensorId = 13
            };
            gp3.Paramters.Add("DEPTH", 30);
            gp3.Value = _acqResults[2];
            sensorgroup.Items.Add(gp1);
            sensorgroup.Items.Add(gp2);
            sensorgroup.Items.Add(gp3);

            return sensorgroup;
        }

    }
}
