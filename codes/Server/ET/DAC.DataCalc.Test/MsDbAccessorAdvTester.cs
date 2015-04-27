using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FS.SMIS_Cloud.DAC.DataCalc.Accessor;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using NUnit.Framework;

namespace DAC.DataCalc.Test
{
    [TestFixture]
    class MsDbAccessorAdvTester
    {
        private MsDbAccessorAdv _accessor = null;

        private const string ConnectString = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";

        private MethodInfo QuerySensorGroupsMethod;
        private MethodInfo QueryVirtualSensorsMethod;
        private MethodInfo QuerySettlementWithVirtualGroupMethod;
        private MethodInfo QuerySensorGroupsByDtuidMethod;
        [SetUp]
        public void SetUp()
        {
            _accessor=new MsDbAccessorAdv(ConnectString);
            QuerySensorGroupsMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.Accessor.MsDbAccessorAdv).GetMethod("QuerySensorGroups",
                   BindingFlags.NonPublic | BindingFlags.Instance);
            QueryVirtualSensorsMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.Accessor.MsDbAccessorAdv).GetMethod("QueryVirtualSensors",
                   BindingFlags.NonPublic | BindingFlags.Instance);
            QuerySettlementWithVirtualGroupMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.Accessor.MsDbAccessorAdv).GetMethod("QuerySettlementWithVirtualGroup",
                   BindingFlags.NonPublic | BindingFlags.Instance);
            QuerySensorGroupsByDtuidMethod = typeof(FS.SMIS_Cloud.DAC.DataCalc.Accessor.MsDbAccessorAdv).GetMethod("QuerySensorGroupsByDtuid",
                   BindingFlags.Public | BindingFlags.Instance);
        }

        [Test]
        public void QuerySettlementWithVirtualGroup()
        {
            var groups = QuerySettlementWithVirtualGroupMethod.Invoke(_accessor, new object[] { (uint)184 }) as IList<SensorGroup>;

            Assert.AreEqual(2, groups.Count);
            var gp1 = groups.Where(a => a.GroupId == 138).ElementAt(0);
            Assert.AreEqual(3, gp1.Items.Count);
            var gp1_item1 = (from s in gp1.Items
                             where s.SensorId == 2255
                             select s).FirstOrDefault();
            Assert.IsNotNull(gp1_item1);
            Assert.AreEqual(184, gp1_item1.DtuId);
            Assert.AreEqual(0, Convert.ToInt32(gp1_item1.Paramters["IsBase"]));
            Assert.IsNull(gp1_item1.VirtualGroup);

            var gp1_item2 = (from s in gp1.Items
                             where s.SensorId == 2256
                             select s).FirstOrDefault();
            Assert.IsNotNull(gp1_item2);
            Assert.AreEqual(184, gp1_item2.DtuId);
            Assert.AreEqual(0, Convert.ToInt32(gp1_item2.Paramters["IsBase"]));
            Assert.IsNull(gp1_item2.VirtualGroup);

            var gp1_item3 = (from s in gp1.Items
                             where s.SensorId == 2260
                             select s).FirstOrDefault();
            Assert.IsNotNull(gp1_item3);
            Assert.AreEqual(184, gp1_item3.DtuId);
            Assert.AreEqual(1, Convert.ToInt32(gp1_item3.Paramters["IsBase"]));
            Assert.IsNotNull(gp1_item3.VirtualGroup);
            Assert.AreEqual(2254, gp1_item3.VirtualGroup.Items[0].SensorId);
            Assert.AreEqual(184, gp1_item3.VirtualGroup.Items[0].DtuId);
            Assert.AreEqual(0, gp1_item3.VirtualGroup.Items[0].Paramters.Count);
            Assert.IsNull(gp1_item3.VirtualGroup.Items[0].VirtualGroup);

            Assert.AreEqual(2252, gp1_item3.VirtualGroup.Items[1].SensorId);
            Assert.AreEqual(183, gp1_item3.VirtualGroup.Items[1].DtuId);
            Assert.AreEqual(0, gp1_item3.VirtualGroup.Items[1].Paramters.Count);
            Assert.IsNull(gp1_item3.VirtualGroup.Items[1].VirtualGroup);

            Assert.AreEqual(2251, gp1_item3.VirtualGroup.Items[2].SensorId);
            Assert.AreEqual(183, gp1_item3.VirtualGroup.Items[2].DtuId);
            Assert.AreEqual(0, gp1_item3.VirtualGroup.Items[2].Paramters.Count);
            Assert.IsNull(gp1_item3.VirtualGroup.Items[2].VirtualGroup);

            var items = gp1.GetAllItems();
            var itemls = new[] { 2255, 2256, 2254, 2252, 2251 };
            var dres = (from s in items
                        where !itemls.Contains(s.SensorId)
                        select s);
            Assert.IsEmpty(dres);

            Assert.IsTrue(gp1.HasMultiDtuJob());


        }

        [Test]
        public void QueryWhole()
        {
            var groups = QuerySensorGroupsByDtuidMethod.Invoke(_accessor, new object[] { (uint)184 }) as IList<SensorGroup>;
            Assert.AreEqual(2, groups.Count);
            var gp1 = groups.Where(a => a.GroupId == 138).ElementAt(0);
            Assert.AreEqual(3, gp1.Items.Count);
            var gp1_item1 = (from s in gp1.Items
                             where s.SensorId == 2255
                             select s).FirstOrDefault();
            Assert.IsNotNull(gp1_item1);
            Assert.AreEqual(184, gp1_item1.DtuId);
            Assert.AreEqual(0, Convert.ToInt32(gp1_item1.Paramters["IsBase"]));
            Assert.IsNull(gp1_item1.VirtualGroup);

            var gp1_item2 = (from s in gp1.Items
                             where s.SensorId == 2256
                             select s).FirstOrDefault();
            Assert.IsNotNull(gp1_item2);
            Assert.AreEqual(184, gp1_item2.DtuId);
            Assert.AreEqual(0, Convert.ToInt32(gp1_item2.Paramters["IsBase"]));
            Assert.IsNull(gp1_item2.VirtualGroup);

            var gp1_item3 = (from s in gp1.Items
                             where s.SensorId == 2260
                             select s).FirstOrDefault();
            Assert.IsNotNull(gp1_item3);
            Assert.AreEqual(184, gp1_item3.DtuId);
            Assert.AreEqual(1, Convert.ToInt32(gp1_item3.Paramters["IsBase"]));
            Assert.IsNotNull(gp1_item3.VirtualGroup);
            Assert.AreEqual(2254, gp1_item3.VirtualGroup.Items[0].SensorId);
            Assert.AreEqual(184, gp1_item3.VirtualGroup.Items[0].DtuId);
            Assert.AreEqual(0, gp1_item3.VirtualGroup.Items[0].Paramters.Count);
            Assert.IsNull(gp1_item3.VirtualGroup.Items[0].VirtualGroup);

            Assert.AreEqual(2252, gp1_item3.VirtualGroup.Items[1].SensorId);
            Assert.AreEqual(183, gp1_item3.VirtualGroup.Items[1].DtuId);
            Assert.AreEqual(0, gp1_item3.VirtualGroup.Items[1].Paramters.Count);
            Assert.IsNull(gp1_item3.VirtualGroup.Items[1].VirtualGroup);

            Assert.AreEqual(2251, gp1_item3.VirtualGroup.Items[2].SensorId);
            Assert.AreEqual(183, gp1_item3.VirtualGroup.Items[2].DtuId);
            Assert.AreEqual(0, gp1_item3.VirtualGroup.Items[2].Paramters.Count);
            Assert.IsNull(gp1_item3.VirtualGroup.Items[2].VirtualGroup);

            var items = gp1.GetAllItems();
            var itemls = new[] { 2255, 2256, 2254, 2252, 2251 };
            var dres = (from s in items
                        where !itemls.Contains(s.SensorId)
                        select s);
            Assert.IsEmpty(dres);

            //-- 测试HasMultiDtuJob方法
            Assert.IsTrue(gp1.HasMultiDtuJob());
            var gps = QuerySensorGroupsByDtuidMethod.Invoke(_accessor, new object[] { (uint)135 }) as IList<SensorGroup>;
            Assert.IsFalse(gps.ElementAt(0).HasMultiDtuJob());
        }

        [Test]
        public void TestQuerySensorGroup_CJ_NOMARL()
        {
            var groups = QuerySensorGroupsMethod.Invoke(_accessor, new object[] { (uint)92 }) as IList<SensorGroup>;
            Assert.AreEqual(1,groups.Count);
            var group = groups.ElementAt(0);
            Assert.IsNotNull(group);
            Assert.AreEqual(86,group.GroupId);
            Assert.AreEqual(GroupType.Settlement,group.GroupType);
            Assert.AreEqual(6,group.Items.Count);
            var item1 = group.Items[0];
            var item2 = group.Items[1];
            Assert.AreEqual(92, item1.DtuId);
            Assert.AreEqual(743, item1.SensorId);
            Assert.IsTrue(item1.Paramters.ContainsKey("IsBase"));
            Assert.AreEqual(1,item1.Paramters["IsBase"]);
            Assert.AreEqual(92, item2.DtuId);
            Assert.AreEqual(744, item2.SensorId);
            Assert.IsTrue(item2.Paramters.ContainsKey("IsBase"));
            Assert.AreEqual(0, item2.Paramters["IsBase"]);
        }

        [Test]
        public void TestQuerySensorGroup_CJ_MultiDTU()
        {
            var groups = QuerySensorGroupsMethod.Invoke(_accessor, new object[] { (uint)136 }) as IList<SensorGroup>;
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
            Assert.AreEqual(136, item1.DtuId);
            Assert.AreEqual(1567, item1.SensorId);
            Assert.IsTrue(item1.Paramters.ContainsKey("IsBase"));
            Assert.AreEqual(1, item1.Paramters["IsBase"]);
            Assert.AreEqual(137, item2.DtuId);
            Assert.AreEqual(1568, item2.SensorId);
            Assert.IsTrue(item2.Paramters.ContainsKey("IsBase"));
            Assert.AreEqual(0, item2.Paramters["IsBase"]);
        }

        [Test]
        public void TestQuerySensorGroup_CX()
        {
            var groups = QuerySensorGroupsMethod.Invoke(_accessor, new object[] { (uint)1 }) as IList<SensorGroup>;
            Assert.AreEqual(2, groups.Count);
            var group1 = groups.ElementAt(0);
            var group2 = groups.ElementAt(1);
            Assert.AreEqual(1, group1.GroupId);
            Assert.AreEqual(2, group2.GroupId);
            Assert.AreEqual(GroupType.Inclination, group1.GroupType);
            Assert.AreEqual(GroupType.Inclination, group2.GroupType);
            Assert.AreEqual(3, group1.Items.Count);
            Assert.AreEqual(3, group2.Items.Count);

            Assert.AreEqual(1, group1.Items[0].DtuId);
            Assert.AreEqual(17, group1.Items[0].SensorId);
            Assert.IsTrue(group1.Items[0].Paramters.ContainsKey("DEPTH"));
            Assert.AreEqual(-14.4, group1.Items[0].Paramters["DEPTH"]);

            Assert.AreEqual(1, group1.Items[1].DtuId);
            Assert.AreEqual(18, group1.Items[1].SensorId);
            Assert.IsTrue(group1.Items[1].Paramters.ContainsKey("DEPTH"));
            Assert.AreEqual(-9.4, group1.Items[1].Paramters["DEPTH"]);

            Assert.AreEqual(1, group1.Items[2].DtuId);
            Assert.AreEqual(19, group1.Items[2].SensorId);
            Assert.IsTrue(group1.Items[2].Paramters.ContainsKey("DEPTH"));
            Assert.AreEqual(-4.4, group1.Items[2].Paramters["DEPTH"]);
        }

        [Test]
        public void TestQuerySensorGroup_JRX()
        {
            var groups = QuerySensorGroupsMethod.Invoke(_accessor, new object[] { (uint)97 }) as IList<SensorGroup>;
            Assert.AreEqual(20, groups.Count);
            var group1 = groups.ElementAt(15);
            var group2 = groups.ElementAt(16);
            Assert.AreEqual(44, group1.GroupId);
            Assert.AreEqual(45, group2.GroupId);
            Assert.AreEqual(GroupType.SaturationLine, group1.GroupType);
            Assert.AreEqual(GroupType.SaturationLine, group2.GroupType);
            Assert.AreEqual(3, group1.Items.Count);
            Assert.AreEqual(3, group2.Items.Count);


            Assert.AreEqual(97, group1.Items[0].DtuId);
            Assert.AreEqual(899, group1.Items[0].SensorId);
            Assert.IsTrue(group1.Items[0].Paramters.ContainsKey("HEIGHT"));
            Assert.AreEqual(-20, group1.Items[0].Paramters["HEIGHT"]);

            Assert.AreEqual(97, group1.Items[1].DtuId);
            Assert.AreEqual(901, group1.Items[1].SensorId);
            Assert.IsTrue(group1.Items[1].Paramters.ContainsKey("HEIGHT"));
            Assert.AreEqual(-15, group1.Items[1].Paramters["HEIGHT"]);

            Assert.AreEqual(97, group1.Items[2].DtuId);
            Assert.AreEqual(903, group1.Items[2].SensorId);
            Assert.IsTrue(group1.Items[2].Paramters.ContainsKey("HEIGHT"));
            Assert.AreEqual(-10, group1.Items[2].Paramters["HEIGHT"]);
        }

        [Test]
        public void TestQueryVirtualSensor()
        {
            var vsensors = QueryVirtualSensorsMethod.Invoke(_accessor, new object[] {(uint) 135}) as IList<SensorGroup>;
            Assert.AreEqual(9,vsensors.Count);
            var vsensor1 = vsensors[0];
            var vsensor2 = vsensors[1];
            Assert.AreEqual(1614,vsensor1.GroupId);
            Assert.AreEqual(GroupType.VirtualSensor,vsensor1.GroupType);
            Assert.AreEqual(15,vsensor1.FormulaId);
            Assert.AreEqual(16,vsensor1.FactorTypeId);
            Assert.IsTrue("T_THEMES_FORCE_ANCHOR" == vsensor1.FactorTypeTable);
            Assert.IsTrue("ANCHOR_FORCE_VALUE" == vsensor1.TableColums);
            //Assert.AreEqual(0,vsensor1.FormulaParams.Count);
            //Assert.AreEqual(200,vsensor1.FormulaParams[0]);
            Assert.AreEqual(1508, vsensor1.Items[0].SensorId);
            Assert.AreEqual(1509, vsensor1.Items[1].SensorId);
            Assert.AreEqual(1510, vsensor1.Items[2].SensorId);
            Assert.AreEqual(135, vsensor1.Items[0].DtuId);
            Assert.AreEqual(135, vsensor1.Items[1].DtuId);
            Assert.AreEqual(135, vsensor1.Items[2].DtuId);
        }
    }
}
