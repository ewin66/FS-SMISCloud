using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using NUnit.Framework;
using FS.SMIS_Cloud.DAC.DataCalc.Accessor;

namespace DAC.DataCalc.Test
{
    [TestFixture]
    class MsDbAccessorTester
    {
        private MsDbAccessor _accessor = null;

        private const string ConnectString = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";

        [SetUp]
        public void SetUp()
        {
            _accessor=new MsDbAccessor(ConnectString);
        }

        [Test(Description = "Test the field DBHelper is created rightly")]
        public void TestConstructor()
        {
            var filed = _accessor.GetType().GetField("_dbHelper", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
            if (filed != null)
            {
                var sqlhptmp = filed.GetValue(_accessor);
                Assert.AreEqual(sqlhptmp.GetType().Name, "MsSqlHelper");
                var fieldInfo = sqlhptmp.GetType().GetField("_connectionString", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
                if (fieldInfo != null)
                    Assert.AreEqual(ConnectString, fieldInfo
                        .GetValue(sqlhptmp).ToString());
            }
        }

        [Test]
        public void TestQuerySensorGroupsByDtuid()
        {
            var ac = _accessor.QuerySensorGroupsByDtuid(127);
            Assert.AreEqual(ac.Count,3);
            var gp1 = ac[0];
            Assert.AreEqual(2114, gp1.GroupId);
            Assert.AreEqual(GroupType.VirtualSensor,gp1.GroupType);
            Assert.AreEqual(1, gp1.Items.Count);
            Assert.AreEqual(2111, gp1.Items[0].SensorId);
            Assert.AreEqual(40, gp1.FactorTypeId);
            Assert.AreEqual("T_THEMES_DEFORMATION_SETTLEMENT", gp1.FactorTypeTable);
            Assert.AreEqual("SETTLEMENT_VALUE", gp1.TableColums);
            Assert.AreEqual(13,gp1.FormulaId);
            Assert.AreEqual(10.08, gp1.FormulaParams[0]);
            Assert.AreEqual(6.22, gp1.FormulaParams[1]);
            AssertEqualVirtualSensor(new Sensor()
            {
                SensorID = 1265,
                DtuID = 127,
                FactorType = 40,
                FactorTypeTable = "T_THEMES_DEFORMATION_SETTLEMENT",
                FormulaID = 13,
                ProtocolType = 0,
                TableColums = "SETTLEMENT_VALUE"
            }, gp1.VirtualSensor);


            var gp2 = ac[1];
            Assert.AreEqual(1323, gp2.GroupId);
            Assert.AreEqual(GroupType.VirtualSensor, gp2.GroupType);
            Assert.AreEqual(1, gp2.Items.Count);
            Assert.AreEqual(1319, gp2.Items[0].SensorId);
            Assert.AreEqual(40, gp2.FactorTypeId);
            Assert.AreEqual("T_THEMES_DEFORMATION_SETTLEMENT", gp2.FactorTypeTable);
            Assert.AreEqual("SETTLEMENT_VALUE", gp2.TableColums);
            Assert.AreEqual(13, gp2.FormulaId);
            Assert.AreEqual(10.04, gp2.FormulaParams[0]);
            Assert.AreEqual(6.243, gp2.FormulaParams[1]);
            AssertEqualVirtualSensor(new Sensor()
            {
                SensorID = 1323,
                DtuID = 127,
                FactorType = 40,
                FactorTypeTable = "T_THEMES_DEFORMATION_SETTLEMENT",
                FormulaID = 13,
                ProtocolType = 0,
                TableColums = "SETTLEMENT_VALUE"
            }, gp2.VirtualSensor);
        }

        private void AssertEqualVirtualSensor(Sensor expectedSensor, Sensor actualSensor)
        {
            Assert.AreEqual(expectedSensor.SensorID, actualSensor.SensorID);
            Assert.AreEqual(expectedSensor.ChannelNo, actualSensor.ChannelNo);
            Assert.AreEqual(expectedSensor.DtuID, actualSensor.DtuID);
            Assert.AreEqual(expectedSensor.FactorType, actualSensor.FactorType);
            Assert.AreEqual(expectedSensor.FactorTypeTable, actualSensor.FactorTypeTable);
            Assert.AreEqual(expectedSensor.FormulaID, actualSensor.FormulaID);
            Assert.AreEqual(expectedSensor.ModuleNo, actualSensor.ModuleNo);
            Assert.AreEqual(expectedSensor.ProtocolType, actualSensor.ProtocolType);
            Assert.AreEqual(expectedSensor.TableColums, actualSensor.TableColums);
        }

        [Test]
        [Ignore("this method is deprecated")]
        public void TestGetEntrySensors()
        {
            var ac = _accessor.GetEntrySensors();
            var exp = new int[] { 1266, 1319 };
            Assert.AreEqual(exp.Length,ac.Count);
            for (int i = 0; i < exp.Length; i++)
            {
                Assert.AreEqual(exp[i],ac[i]);
            }
        }
    }
}
