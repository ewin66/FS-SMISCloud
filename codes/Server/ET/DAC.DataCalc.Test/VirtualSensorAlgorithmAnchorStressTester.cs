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
    internal class VirtualSensorAlgorithmAnchorStressTester
    {
        private IList<SensorGroup> _sensorGroups = null;
        private IList<SensorAcqResult> _acqResults = null;
        private IList<SensorGroup> _sensorGroupsMax = null;

        [SetUp]
        public void SetUp()
        {
            _acqResults = InitialDataAcqResult();
            _sensorGroups = InitialSensorGroup();
            _sensorGroupsMax = InitialSensorGroup_Max();
        }

        [Test]
        public void TestCalcData()
        {
            var t = Assembly.Load("DAC.DataCalc").GetType("FS.SMIS_Cloud.DAC.DataCalc.Algorithm.VirtualSensorAlgorithm");
            var constructors = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNotEmpty(constructors);
            var objAlgorithm = constructors[0].Invoke(new object[] { _sensorGroups[0] });
            t.GetMethod("CalcData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(objAlgorithm, new object[] { _acqResults });

            Assert.AreEqual(4, _acqResults.Count);
            var res = _acqResults[3];
            Assert.IsTrue(res.Data is VirtualSensor);
            Assert.AreEqual(41.733333333333, res.Data.ThemeValues[0].Value, 0.00000001);
            //Assert.AreEqual(DateTime.Parse("2014-12-02 17:13:00"), res.Data.AcqTime);
            Assert.AreEqual(200, res.Sensor.SensorID);
            Assert.AreEqual(15, res.Sensor.FormulaID);
            Assert.AreEqual(1, res.Sensor.ProtocolType);
            Assert.AreEqual(16, res.Sensor.FactorType);
        }

        [Test]
        public void TestCalcData_max()
        {
            var t = Assembly.Load("DAC.DataCalc").GetType("FS.SMIS_Cloud.DAC.DataCalc.Algorithm.VirtualSensorAlgorithm");
            var constructors = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNotEmpty(constructors);
            var objAlgorithm = constructors[0].Invoke(new object[] { _sensorGroupsMax[0] });
            t.GetMethod("CalcData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(objAlgorithm, new object[] { _acqResults });

            Assert.AreEqual(4, _acqResults.Count);
            var res = _acqResults[3];
            Assert.IsTrue(res.Data is VirtualSensor);
            Assert.AreEqual(41.733333333333327, res.Data.ThemeValues[0].Value, 0.00000001);
            //Assert.AreEqual(DateTime.Parse("2014-12-02 17:13:00"), res.Data.AcqTime);
            Assert.AreEqual(200, res.Sensor.SensorID);
            Assert.AreEqual(31, res.Sensor.FormulaID);
            Assert.AreEqual(1, res.Sensor.ProtocolType);
            Assert.AreEqual(16, res.Sensor.FactorType);
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
                Data = new VibratingWireData(0, 0, 42.1, 42.1)
                {
                    //   AcqTime = DateTime.Parse("2014-12-02 17:13:00")
                }
            };

            var acq2 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 22
                },
                Data = new VibratingWireData(0, 0, 38.5, 38.5)
                {
                    //  AcqTime = DateTime.Parse("2014-12-02 17:13:00")
                }
            };

            var acq3 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 23
                },
                Data = new VibratingWireData(0, 0, 44.6, 44.6)
                {
                    // AcqTime = DateTime.Parse("2014-12-02 17:13:00")
                }
            };
            result.Add(acq1);
            result.Add(acq2);
            result.Add(acq3);
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
            sensorgroup1.FormulaId = 15;
            sensorgroup1.FactorTypeId = 16; // assume Anchor Force
            sensorgroup1.VirtualSensor = new Sensor()
            {
                SensorID = 200,
                DtuID = 147,
                FormulaID = 15,
                ProtocolType = 1,
                FactorType = 16
            };
            var gp1 = new GroupItem
            {
                SensorId = 21
            };
            gp1.Value = _acqResults[0];
            sensorgroup1.Items.Add(gp1);

            var gp2 = new GroupItem
            {
                SensorId = 22
            };
            gp2.Value = _acqResults[1];
            sensorgroup1.Items.Add(gp2);

            var gp3 = new GroupItem
            {
                SensorId = 23
            };
            gp3.Value = _acqResults[2];
            sensorgroup1.Items.Add(gp3);

            result.Add(sensorgroup1);
            return result;
        }

        // for SETUP
        private IList<SensorGroup> InitialSensorGroup_Max()
        {
            var result = new List<SensorGroup>();
            // using Reflection to get internal constructor method;
            var constructor = typeof(SensorGroup).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotEmpty(constructor);
            var sensorgroup1 = (SensorGroup)constructor[0].Invoke(new object[] { 200, GroupType.VirtualSensor });
            sensorgroup1.FormulaId = 15;
            sensorgroup1.FactorTypeId = 16; // assume Anchor Force
            sensorgroup1.VirtualSensor = new Sensor()
            {
                SensorID = 200,
                DtuID = 147,
                FormulaID = 31,
                ProtocolType = 1,
                FactorType = 16
            };
            var gp1 = new GroupItem
            {
                SensorId = 21
            };
            gp1.Value = _acqResults[0];
            sensorgroup1.Items.Add(gp1);

            var gp2 = new GroupItem
            {
                SensorId = 22
            };
            gp2.Value = _acqResults[1];
            sensorgroup1.Items.Add(gp2);

            var gp3 = new GroupItem
            {
                SensorId = 23
            };
            gp3.Value = _acqResults[2];
            sensorgroup1.Items.Add(gp3);

            result.Add(sensorgroup1);
            return result;
        }
    }
}
