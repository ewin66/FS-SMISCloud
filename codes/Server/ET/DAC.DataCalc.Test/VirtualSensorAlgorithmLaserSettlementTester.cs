using System;
using System.Collections.Generic;
using System.Reflection;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using NUnit.Framework;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace DAC.DataCalc.Test
{
    [TestFixture]
    class VirtualSensorAlgorithmLaserSettlementTester
    {
        private IList<SensorGroup> _sensorGroups = null;
        private IList<SensorAcqResult> _acqResults = null;


        [SetUp]
        public void SetUp()
        {
            _acqResults = InitialDataAcqResult();
            _sensorGroups = InitialSensorGroup();
        }

        [Test]
        public void TestCalcData()
        {
            var t = Assembly.Load("DAC.DataCalc").GetType("FS.SMIS_Cloud.DAC.DataCalc.Algorithm.VirtualSensorAlgorithm");
            var constructors = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNotEmpty(constructors);
            foreach (SensorGroup sensorGroup in _sensorGroups)
            {
                var objAlgorithm = constructors[0].Invoke( new object[] { sensorGroup });
                t.GetMethod("CalcData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(objAlgorithm, new object[] { _acqResults });
            }
            Assert.AreEqual(4, _acqResults.Count);
            Assert.AreEqual(1197.36, _acqResults[2].Data.ThemeValues[0].Value, 0.000000001);
            Assert.AreEqual(1398.768, _acqResults[3].Data.ThemeValues[0].Value, 0.000000001);

            Assert.IsTrue(_acqResults[2].Data is VirtualSensor);
            var sensor = _acqResults[2].Sensor;
            Assert.AreEqual(200, sensor.SensorID);
            Assert.AreEqual(13, sensor.FormulaID);
            Assert.AreEqual(40, sensor.FactorType);
            Assert.AreEqual(147, sensor.DtuID);
            Assert.AreEqual(1, sensor.ProtocolType);

            Assert.IsTrue(_acqResults[3].Data is VirtualSensor);
            var datasensor = _acqResults[3].Sensor;
            Assert.AreEqual(201, datasensor.SensorID);
            Assert.AreEqual(13, datasensor.FormulaID);
            Assert.AreEqual(40, datasensor.FactorType);
            Assert.AreEqual(147, datasensor.DtuID);
            Assert.AreEqual(1, datasensor.ProtocolType);
        }

        // for SETUP
        private IList<SensorAcqResult> InitialDataAcqResult()
        {
            var result = new List<SensorAcqResult>();
            var acq1 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 21
                },
                Data = new LaserData(0,1.1)
                {
                   // AcqTime = DateTime.Parse("2014-12-02 16:13:00")
                }
            };

            var acq2 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 22
                },
                Data = new LaserData(0, 2.2)
                {
                   // AcqTime = DateTime.Parse("2014-12-02 17:13:00")
                }
            };

            result.Add(acq1);
            result.Add(acq2);
            return result;
        }

        // for SETUP
        private IList<SensorGroup> InitialSensorGroup()
        {
            var result = new List<SensorGroup>();
            // using Reflection to get internal constructor method;
            var constructor = typeof(SensorGroup).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotEmpty(constructor);
            var sensorgroup1 = (SensorGroup)constructor[0].Invoke(new object[] { 200, GroupType.VirtualSensor });
            sensorgroup1.FormulaId = 13;
            sensorgroup1.FormulaParams = new double[] { 0.5, 1.2 };
            sensorgroup1.FactorTypeId = 40;
            sensorgroup1.VirtualSensor = new Sensor()
            {
                SensorID = 200,
                DtuID = 147,
                FormulaID = 13,
                ProtocolType = 1,
                FactorType = 40
            };
            var gp1 = new GroupItem
            {
                SensorId = 21
            };
            gp1.Value = _acqResults[0];
            sensorgroup1.Items.Add(gp1);
            result.Add(sensorgroup1);

            var sensorgroup2 = (SensorGroup)constructor[0].Invoke(new object[] { 201, GroupType.VirtualSensor });
            sensorgroup2.FormulaId = 13;
            sensorgroup2.FormulaParams = new double[] { 2.5, 1.4 };
            sensorgroup2.FactorTypeId = 40;
            sensorgroup2.VirtualSensor = new Sensor()
            {
                SensorID = 201,
                DtuID = 147,
                FormulaID = 13,
                ProtocolType = 1,
                FactorType = 40
            };
            var gp2 = new GroupItem
            {
                SensorId = 22
            };
            gp2.Value = _acqResults[1];
            sensorgroup2.Items.Add(gp2);
            result.Add(sensorgroup2);

            return result;
        }
    }
}
