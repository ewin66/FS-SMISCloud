namespace DAC.DataAnalyze.Test
{
    using System;
    using System.Data;
    using System.Linq;

    using FS.SMIS_Cloud.DAC.DataAnalyzer.Accessor;

    using NUnit.Framework;

    [TestFixture]
    class DbAccessorTester
    {
        [Test]
        public void TestGetSensorThreshold()
        {
            var act1 = DbAccessor.GetSensorThreshold(new uint[] { 17, 18 });
            Assert.AreEqual(3, act1.Rows.Count);

            var act2 = DbAccessor.GetSensorThreshold(null);
            Assert.AreEqual(0, act2.Rows.Count);

            var act3 = DbAccessor.GetSensorThreshold(new uint[]{});
            Assert.AreEqual(0, act3.Rows.Count);
        }

        [Test]
        public void TestGetSensorInfo()
        {
            var act1 = DbAccessor.GetSensorInfo(1);
            Assert.AreEqual(0, act1.Rows.Count);

            var act2 = DbAccessor.GetSensorInfo(17);
            Assert.AreEqual(1, act2.Rows.Count);
        }

        [Test]
        public void TestGetSafetyFactorInfo()
        {
            var act1 = DbAccessor.GetSafetyFactorInfo(-1);
            Assert.AreEqual(0, act1.Rows.Count);

            var act2 = DbAccessor.GetSafetyFactorInfo(17);
            Assert.AreEqual(1, act2.Rows.Count);
        }

        [Test]
        public void TestGetOrgStcByStruct()
        {
            var act = DbAccessor.GetOrgStcByStruct(71).Rows[0];

            Assert.AreEqual(65, act["OrgStcId"]);
            Assert.AreEqual(71, act["StcId"]);
        }

        [Test]
        public void TestGetThemeWeightByOrgStc()
        {
            var act = DbAccessor.GetThemeWeightByOrgStc(2).AsEnumerable();

            Assert.AreEqual(50, Convert.ToInt32(act.Where(r => r.Field<int>("ThemeId") == 1).First()["Weight"]));
            Assert.AreEqual(50, Convert.ToInt32(act.Where(r => r.Field<int>("ThemeId") == 2).First()["Weight"]));
        }

        [Test]
        public void TestGetSensorWeightByOrgStc()
        {
            var act = DbAccessor.GetSensorWeightByOrgStc(2, 2).AsEnumerable();

            Assert.AreEqual(50, Convert.ToInt32(act.Where(r => r.Field<int>("SensorId") == 17).First()["Weight"]));
        }

        [Test]
        public void TestSaveStructureScore()
        {
            DbAccessor.SaveStructureScore(2, 100, DateTime.Now);
            Assert.Pass();
        }

        [Test]
        public void TestSaveThemeScore()
        {
            DbAccessor.SaveThemeScore(2, 2, 100, DateTime.Now);
            Assert.Pass();
        }
    }
}
