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
    internal class VirtualSensorAlgorithmSettlementVirtualBase
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
        public void TestCalcVirtualBaseSettlement()
        {
            var t = Assembly.Load("DAC.DataCalc").GetType("FS.SMIS_Cloud.DAC.DataCalc.Algorithm.VirtualSensorAlgorithm");
            var constructors = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNotEmpty(constructors);
            var objAlgorithm = constructors[0].Invoke(new object[] { _sensorGroups[0] });
            t.GetMethod("CalcData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(objAlgorithm, new object[] { _acqResults });
            Assert.AreEqual(5, _acqResults.Count);
            var res = _acqResults[4];
            Assert.IsTrue(res.Data is VirtualSensor);
            Assert.AreEqual(200,res.Sensor.SensorID);
            Assert.AreEqual(32, res.Sensor.FormulaID);
            Assert.AreEqual(12.7, res.Data.ThemeValues[0].Value, 0.00000001);
        }

        // for SETUP
        private IList<SensorAcqResult> InitialDataAcqResult()
        {
            var result = new List<SensorAcqResult>();
            // Reference Base Site
            var acq1 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 1
                },
                Data = new SensorData(null, new double[] { 12.3 }, null)
            };
            // Reference Measure Site
            var acq2 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 2
                },
                Data = new SensorData(null, new double[] { 25.1 }, null)
            };
            // Base Site
            var acq3 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 3
                },
                Data = new SensorData(null, new double[] { 25.5 }, null)
            };
            // Measure Site
            var acq4 = new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 4
                },
                Data = new SensorData(null, new double[] { 33.4 }, null)
            };
            result.Add(acq1);
            result.Add(acq2);
            result.Add(acq3);
            result.Add(acq4);
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
            sensorgroup1.FormulaId = 32;
            sensorgroup1.VirtualSensor = new Sensor()
            {
                SensorID = 200,
                DtuID = 147,
                FormulaID = 32,
                FactorType = 42
            };
            var gp1 = new GroupItem
            {
                SensorId = 3
            };
            gp1.Value = _acqResults[2];
            sensorgroup1.Items.Add(gp1);

            var gp2 = new GroupItem
            {
                SensorId = 2
            };
            gp2.Value = _acqResults[1];
            sensorgroup1.Items.Add(gp2);

            var gp3 = new GroupItem
            {
                SensorId = 1
            };
            gp3.Value = _acqResults[0];
            sensorgroup1.Items.Add(gp3);

            result.Add(sensorgroup1);
            return result;
        }

    }
}
