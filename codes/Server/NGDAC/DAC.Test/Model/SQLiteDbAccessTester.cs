namespace NGDAC.Test.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using FS.SMIS_Cloud.NGDAC.Accessor;
    using FS.SMIS_Cloud.NGDAC.Accessor.SQLite;
    using FS.SMIS_Cloud.NGDAC.Model;

    using NUnit.Framework;

    [TestFixture]
    public class SQLiteDbAccessTester
    {
        private static string connstr = ".\\FSUSDB\\fsuscfg.db3,.\\FSUSDB\\FSUSDataValueDB.db3";

        [Test]
        public void TestGetDtus()
        {
            var cs = connstr.Split(',');
            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0]));

            IList<DtuNode> dtus = DbAccessorHelper.DbAccessor.QueryDtuNodes();
            Assert.IsTrue(dtus != null);

            Assert.IsTrue(dtus.Count > 0);

            DtuNode d0 = dtus[0];
            Assert.AreEqual((ushort) 2, d0.DtuId);
            Assert.AreEqual("COM6", d0.DtuCode);
            Assert.AreEqual((uint) 300, d0.DacInterval); // 5m
            // dtuId	sid	mno	cno	factor	PRODUCT_SENSOR_ID	PROTOCOL_CODE	name
            //  1	    17	9596	1	10	82	1503	K765左侧一阶平台1号测斜孔-下

            IList<Sensor> sensors = d0.Sensors;
            Assert.IsTrue(sensors.Count > 0);
            Sensor s1 = sensors[0];
            Assert.AreEqual((uint) 2, s1.DtuID);
            Assert.AreEqual((uint) 1, s1.SensorID);
            Assert.AreEqual((uint) 9877, s1.ModuleNo);

            DtuNode d91 = dtus.Last();
            Sensor s89 = d91.FindSensor(89); //89号传感器. 参数, 2, 归属公式: 9, 类型 28,27
            // param.
            SensorParam sp0 = s89.Parameters[0];
            Assert.AreEqual(9, sp0.FormulaParam.FID);
            Assert.AreEqual(27, sp0.FormulaParam.PID);

        }

        [Test]
        public void TestGetDtuByCode()
        {
            var cs = connstr.Split(',');
            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0]));
            DtuNode d0 = DbAccessorHelper.DbAccessor.QueryDtuNode("COM14");
            Assert.AreEqual((ushort) 3, d0.DtuId);
            Assert.AreEqual("COM14", d0.DtuCode);
            Assert.AreEqual("", d0.Name);
        }

    }
}
