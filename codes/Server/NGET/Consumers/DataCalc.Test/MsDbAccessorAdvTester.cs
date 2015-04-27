using FS.SMIS_Cloud.NGET.DataCalc.Accessor;
using FS.SMIS_Cloud.NGET.DataCalc.Model;

namespace DataCalc.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using NUnit.Framework;

    [TestFixture]
    class MsDbAccessorAdvTester
    {
        private MsDbAccessorAdv _accessor = null;

        private const string ConnectString = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";

        private MethodInfo _querySensorGroupsMethod;
        private MethodInfo _queryVirtualSensorsMethod;
        [SetUp]
        public void SetUp()
        {
            this._accessor=new MsDbAccessorAdv(ConnectString);
            this._querySensorGroupsMethod = typeof(MsDbAccessorAdv).GetMethod("QuerySensorGroups",
                   BindingFlags.NonPublic | BindingFlags.Instance);
            this._queryVirtualSensorsMethod = typeof(MsDbAccessorAdv).GetMethod("QueryVirtualSensors",
                   BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [Test]
        public void TestQuerySensorGroup_CJ_NOMARL()
        {
            var groups = this._querySensorGroupsMethod.Invoke(this._accessor, new object[] { (uint)92 }) as IList<SensorGroup>;
            if (groups != null)
            {
                Assert.AreEqual(1,groups.Count);
                var group = groups.ElementAt(0);
                Assert.IsNotNull(@group);
                Assert.AreEqual(86,@group.GroupId);
                Assert.AreEqual(GroupType.Settlement,@group.GroupType);
                Assert.AreEqual(6,@group.Items.Count);
                var item1 = @group.Items[0];
                var item2 = @group.Items[1];
                Assert.AreEqual(92, item1.DTUId);
                Assert.AreEqual(743, item1.SensorId);
                Assert.IsTrue(item1.Paramters.ContainsKey("IsBase"));
                Assert.AreEqual(1,item1.Paramters["IsBase"]);
                Assert.AreEqual(92, item2.DTUId);
                Assert.AreEqual(744, item2.SensorId);
                Assert.IsTrue(item2.Paramters.ContainsKey("IsBase"));
                Assert.AreEqual(0, item2.Paramters["IsBase"]);
            }
        }

        [Test]
        public void TestQuerySensorGroup_CJ_MultiDTU()
        {
            var groups = this._querySensorGroupsMethod.Invoke(this._accessor, new object[] { (uint)136 }) as IList<SensorGroup>;
            if (groups != null)
            {
                Assert.AreEqual(2, groups.Count);
                var group1 = groups.ElementAt(0);
                var group2 = groups.ElementAt(1);
                Assert.AreEqual(84, group1.GroupId);
                Assert.AreEqual(85, group2.GroupId);
                Assert.AreEqual(GroupType.Settlement, group1.GroupType);
                Assert.AreEqual(GroupType.Settlement, group2.GroupType);
                Assert.AreEqual(18, group1.Items.Count);
                Assert.AreEqual(13, group2.Items.Count);
                var item1 = group1.Items[0];
                var item2 = group1.Items[1];
                Assert.AreEqual(136, item1.DTUId);
                Assert.AreEqual(1567, item1.SensorId);
                Assert.IsTrue(item1.Paramters.ContainsKey("IsBase"));
                Assert.AreEqual(1, item1.Paramters["IsBase"]);
                Assert.AreEqual(137, item2.DTUId);
                Assert.AreEqual(1568, item2.SensorId);
                Assert.IsTrue(item2.Paramters.ContainsKey("IsBase"));
                Assert.AreEqual(0, item2.Paramters["IsBase"]);
            }
        }

        [Test]
        public void TestQuerySensorGroup_CX()
        {
            var groups = this._querySensorGroupsMethod.Invoke(this._accessor, new object[] { (uint)1 }) as IList<SensorGroup>;
            if (groups != null)
            {
                Assert.AreEqual(2, groups.Count);
                var group1 = groups.ElementAt(0);
                var group2 = groups.ElementAt(1);
                Assert.AreEqual(1, group1.GroupId);
                Assert.AreEqual(2, group2.GroupId);
                Assert.AreEqual(GroupType.Inclination, group1.GroupType);
                Assert.AreEqual(GroupType.Inclination, group2.GroupType);
                Assert.AreEqual(3, group1.Items.Count);
                Assert.AreEqual(3, group2.Items.Count);

                Assert.AreEqual(1, group1.Items[0].DTUId);
                Assert.AreEqual(17, group1.Items[0].SensorId);
                Assert.IsTrue(group1.Items[0].Paramters.ContainsKey("DEPTH"));
                Assert.AreEqual(-14.4, group1.Items[0].Paramters["DEPTH"]);

                Assert.AreEqual(1, group1.Items[1].DTUId);
                Assert.AreEqual(18, group1.Items[1].SensorId);
                Assert.IsTrue(group1.Items[1].Paramters.ContainsKey("DEPTH"));
                Assert.AreEqual(-9.4, group1.Items[1].Paramters["DEPTH"]);

                Assert.AreEqual(1, group1.Items[2].DTUId);
                Assert.AreEqual(19, group1.Items[2].SensorId);
                Assert.IsTrue(group1.Items[2].Paramters.ContainsKey("DEPTH"));
                Assert.AreEqual(-4.4, group1.Items[2].Paramters["DEPTH"]);
            }
        }

        [Test]
        public void TestQuerySensorGroup_JRX()
        {
            var groups = this._querySensorGroupsMethod.Invoke(this._accessor, new object[] { (uint)97 }) as IList<SensorGroup>;
            Assert.AreEqual(20, groups.Count);
            var group1 = groups.ElementAt(15);
            var group2 = groups.ElementAt(16);
            Assert.AreEqual(44, group1.GroupId);
            Assert.AreEqual(45, group2.GroupId);
            Assert.AreEqual(GroupType.SaturationLine, group1.GroupType);
            Assert.AreEqual(GroupType.SaturationLine, group2.GroupType);
            Assert.AreEqual(3, group1.Items.Count);
            Assert.AreEqual(3, group2.Items.Count);


            Assert.AreEqual(97, group1.Items[0].DTUId);
            Assert.AreEqual(899, group1.Items[0].SensorId);
            Assert.IsTrue(group1.Items[0].Paramters.ContainsKey("HEIGHT"));
            Assert.AreEqual(-20, group1.Items[0].Paramters["HEIGHT"]);

            Assert.AreEqual(97, group1.Items[1].DTUId);
            Assert.AreEqual(901, group1.Items[1].SensorId);
            Assert.IsTrue(group1.Items[1].Paramters.ContainsKey("HEIGHT"));
            Assert.AreEqual(-15, group1.Items[1].Paramters["HEIGHT"]);

            Assert.AreEqual(98, group1.Items[2].DTUId);
            Assert.AreEqual(903, group1.Items[2].SensorId);
            Assert.IsTrue(group1.Items[2].Paramters.ContainsKey("HEIGHT"));
            Assert.AreEqual(-10, group1.Items[2].Paramters["HEIGHT"]);
        }

        [Test]
        public void TestQueryVirtualSensor()
        {
            var vsensors = this._queryVirtualSensorsMethod.Invoke(this._accessor, new object[] {(uint) 135}) as IList<SensorGroup>;
            Assert.AreEqual(6,vsensors.Count);
            var vsensor1 = vsensors[0];
            var vsensor2 = vsensors[1];
            Assert.AreEqual(1614,vsensor1.GroupId);
            Assert.AreEqual(GroupType.VirtualSensor,vsensor1.GroupType);
            Assert.AreEqual(15,vsensor1.FormulaId);
            Assert.AreEqual(16,vsensor1.FactorTypeId);
            Assert.IsTrue("T_THEMES_FORCE_ANCHOR" == vsensor1.FactorTypeTable);
            Assert.IsTrue("ANCHOR_FORCE_VALUE" == vsensor1.TableColums);
            Assert.AreEqual(1,vsensor1.FormulaParams.Count);
            Assert.AreEqual(200,vsensor1.FormulaParams[0]);
            Assert.AreEqual(1508, vsensor1.Items[0].SensorId);
            Assert.AreEqual(1509, vsensor1.Items[1].SensorId);
            Assert.AreEqual(1510, vsensor1.Items[2].SensorId);
            Assert.AreEqual(135, vsensor1.Items[0].DTUId);
            Assert.AreEqual(135, vsensor1.Items[1].DTUId);
            Assert.AreEqual(135, vsensor1.Items[2].DTUId);
        }
    }
}
