using System;
using System.Linq;
using NUnit.Framework;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.nunit
{
    [TestFixture]
    class TimeRecorderTester
    {
        [Test]
        public void TestGetRecordTime()
        {
            var act = ConfigHelper.GetRecordTime(42);
            var exp = new DateTime(2015, 12, 12, 12, 12, 12);
            Assert.IsTrue(exp == act);

            var time1 = new DateTime(2013, 1, 1, 0, 0, 0);
            ConfigHelper.SetRecordTime(1, time1);
            var act1 = ConfigHelper.GetRecordTime(1);
            Assert.IsTrue(time1 == act1);
        }

        [Test]
        public void TestSetRecordTime()
        {
            var t1 = DateTime.Now;
            var exp = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
            ConfigHelper.SetRecordTime(100, exp);
            var act = ConfigHelper.GetRecordTime(100);
            Assert.IsTrue(exp == act);
        }

        [Test]
        public void TestGetAllStructs()
        {
            var act = ConfigHelper.GetStructs();
            //Assert.AreEqual(1, act.Length);
            Assert.IsTrue(act.Contains(87));
        }

        [Test]
        public void TestGetStructWorkPaths()
        {
            var act = ConfigHelper.GetStructWorkPaths();
            Assert.IsNotEmpty(act);
            var exp = @"D:\yjp\acc";
            Assert.AreEqual(exp, act[87]);
        }
    }
}
