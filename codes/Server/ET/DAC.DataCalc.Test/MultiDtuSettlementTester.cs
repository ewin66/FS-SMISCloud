using System;
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
    class MultiDtuSettlementTester
    {
        private string TID1;
        private string TID2;
        private SensorGroup _sensorGroup = null;
        private DACTaskResult _dactask1 = null;
        private DACTaskResult _dactask2 = null;

        [SetUp]
        public void SetUp()
        {
            TID1 = Guid.NewGuid().ToString();
            TID2 = Guid.NewGuid().ToString();
        }

        [Test]
        public void TestSettlementCalc()
        {
            _dactask1 = Acq1(TID1);
            _dactask2 = Acq2(TID1);
            _sensorGroup = InitialSensorGroup();
            var calcMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.DataCalc).GetMethod("Calc",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            calcMethod.Invoke(null, new object[] { _dactask1, new SensorGroup[] { _sensorGroup } });

            Assert.AreEqual(2, _dactask1.SensorResults.Count);
            Assert.IsFalse(_dactask1.SensorResults[0].IsOK);
            Assert.IsFalse(_dactask1.SensorResults[1].IsOK);

            _sensorGroup = InitialSensorGroup();
            calcMethod.Invoke(null, new object[] { _dactask2, new SensorGroup[] { _sensorGroup } });
            Assert.AreEqual(4, _dactask2.SensorResults.Count);
            Assert.IsFalse(_dactask2.SensorResults[0].IsOK);
            Assert.IsTrue(_dactask2.SensorResults[1].IsOK);
            Assert.IsTrue(_dactask2.SensorResults[2].IsOK);
            Assert.IsTrue(_dactask2.SensorResults[3].IsOK);
            Assert.AreEqual(0.0, _dactask2.SensorResults[1].Data.ThemeValues[0].Value, 0.000000001);
            Assert.AreEqual(-1.5, _dactask2.SensorResults[2].Data.ThemeValues[0].Value, 0.000000001);
            Assert.AreEqual(-2.6, _dactask2.SensorResults[3].Data.ThemeValues[0].Value, 0.000000001);

        }

        [Test]
        [Description("包含基点的非完整采集")]
        public void TestSettlementCalc_IncompleteAcqWithBase()
        {
            _dactask1 = Acq1(TID1);
            _dactask2 = Acq2(TID2);
            _sensorGroup = InitialSensorGroup();
            var calcMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.DataCalc).GetMethod("Calc",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            calcMethod.Invoke(null, new object[] { _dactask1, new SensorGroup[] { _sensorGroup } });
            Assert.AreEqual(2, _dactask1.SensorResults.Count);
            Assert.IsFalse(_dactask1.SensorResults[0].IsOK);
            Assert.IsFalse(_dactask1.SensorResults[1].IsOK);

            _sensorGroup = InitialSensorGroup();
            calcMethod.Invoke(null, new object[] { _dactask2, new SensorGroup[] { _sensorGroup } });
            Assert.AreEqual(3,_dactask2.SensorResults.Count);
            Assert.IsFalse(_dactask2.SensorResults[0].IsOK);
            Assert.IsTrue(_dactask2.SensorResults[1].IsOK);
            Assert.IsTrue(_dactask2.SensorResults[2].IsOK);
            Assert.AreEqual(0.0, _dactask2.SensorResults[1].Data.ThemeValues[0].Value, 0.000000001);
            Assert.AreEqual(-1.5, _dactask2.SensorResults[2].Data.ThemeValues[0].Value, 0.000000001);
        }

        [Test]
        [Description("不包含基点的非完整采集")]
        public void TestSettlementCalc_IncompleteAcqWithoutBase()
        {
            _dactask1 = Acq2(TID1);
            _dactask2 = Acq1(TID2);
            _sensorGroup = InitialSensorGroup();
            var calcMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.DataCalc).GetMethod("Calc",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            calcMethod.Invoke(null, new object[] { _dactask1, new SensorGroup[] { _sensorGroup } });
            Assert.AreEqual(1,_dactask1.SensorResults.Count);
            Assert.IsFalse(_dactask1.SensorResults[0].IsOK);

            _sensorGroup = InitialSensorGroup();
            calcMethod.Invoke(null, new object[] { _dactask2, new SensorGroup[] { _sensorGroup } });
            Assert.AreEqual(2,_dactask2.SensorResults.Count);
            Assert.IsFalse(_dactask2.SensorResults[0].IsOK);
            Assert.IsFalse(_dactask2.SensorResults[1].IsOK);
        }

        private SensorGroup InitialSensorGroup()
        {
            var constructor = typeof(SensorGroup).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotEmpty(constructor);
            var sensorgroup = (SensorGroup)constructor[0].Invoke(new object[] { 1, GroupType.Settlement });

            var gp1 = new GroupItem
            {
                SensorId = 1,
                DtuId = 100
            };
            gp1.Paramters.Add("IsBase", 1);

            var gp2 = new GroupItem
            {
                SensorId = 2,
                DtuId = 100
            };
            gp2.Paramters.Add("IsBase", 0);

            var gp3 = new GroupItem
            {
                SensorId = 3,
                DtuId = 101
            };
            gp3.Paramters.Add("IsBase", 0);
            sensorgroup.Items.Add(gp1);
            sensorgroup.Items.Add(gp2);
            sensorgroup.Items.Add(gp3);

            return sensorgroup;
        }

        private DACTaskResult Acq1(string tid)
        {
            DACTaskResult res = new DACTaskResult()
            {
                Task = new DACTask()
                {
                    TID = tid,
                    DtuID = 100
                }
            };
            res.AddSensorResult(new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 1
                },
                Data = new PressureData(0, 1),
                ErrorCode = 0
            });
            res.AddSensorResult(new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 2
                },
                Data = new PressureData(0, 2.5),
                ErrorCode = 0
            });

            return res;
        }

        private DACTaskResult Acq2(string tid)
        {
            DACTaskResult res = new DACTaskResult()
            {
                Task = new DACTask()
                {
                    TID = tid,
                    DtuID = 101
                }
            };
            res.AddSensorResult(new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    SensorID = 3
                },
                Data = new PressureData(0, 3.6),
                ErrorCode = 0
            });

            return res;
        }
    }
}
