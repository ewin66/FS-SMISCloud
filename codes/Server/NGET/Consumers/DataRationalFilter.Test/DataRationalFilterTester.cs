namespace DataRationalFilter.Test
{
    using System.Collections.Generic;
    using System.Configuration;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET.DataRationalFilter;
    using FS.SMIS_Cloud.NGET.Model;

    using NUnit.Framework;

    using DbType = FS.DbHelper.DbType;

    [TestFixture]
    class DataRationalFilterTester
    {
        private static string cs = ConfigurationManager.AppSettings["SecureCloud"];

        private static ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);

        [SetUp]
        public void ClearAllRationalRange()
        {
            string sql = "truncate table T_DATA_RATIONAL_FILTER_CONFIG";

            sqlHelper.ExecuteSql(sql);
        }

        public List<SensorAcqResult> CreateDACTaskResult()
        {
            return new List<SensorAcqResult>()
                       {
                           new SensorAcqResult
                               {
                                   Sensor =
                                       new Sensor
                                           {
                                               SensorID = 1,
                                               SensorType =
                                                   SensorType.Entity
                                           },
                                   ErrorCode = 0,
                                   Data =
                                       new SensorData(
                                       new double[] { 30, 70 },
                                       new double[] { 30, 70 },
                                       new double[] { 30, 70 })
                               }
                       };
        }

        public void InsertRationalRange(int sensorId, int itemId, double lower, double upper, bool enabled)
        {
            string sql = string.Format(@"
INSERT INTO T_DATA_RATIONAL_FILTER_CONFIG
VALUES({0},{1},{2},{3},{4})", sensorId, itemId, enabled ? 1 : 0, lower, upper);

            sqlHelper.ExecuteSql(sql);
        }

        [Test]
        public void TestProcessRequest_WithoutAnyRange()
        {
            var rslt = this.CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.ProcessResult(rslt);

            Assert.AreEqual(30, rslt[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt[0].Data.ThemeValues[1]);
        }

        [Test]
        public void TestProcessRequest_WithOneRangeEnabled()
        {
            var rslt = this.CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter=new SensorType[]{SensorType.Data, SensorType.Entity, SensorType.Virtual};
            this.InsertRationalRange(1, 1, -10, 10, true);

            filter.ProcessResult(rslt);

            Assert.IsNull(rslt[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt[0].Data.ThemeValues[1]);
        }

        [Test]
        public void TestProcessRequest_WithOneRangeNotEnabled()
        {
            var rslt = this.CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            this.InsertRationalRange(1, 1, -10, 10, false);

            filter.ProcessResult(rslt);

            Assert.AreEqual(30, rslt[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt[0].Data.ThemeValues[1]);
        }

        [Test]
        public void TestProcessRequest_WithTwoItemsRangeEnabled()
        {
            var rslt = this.CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            this.InsertRationalRange(1, 1, -10, 10, true);
            this.InsertRationalRange(1, 2, -10, 10, true);

            filter.ProcessResult(rslt);

            Assert.IsNull(rslt[0].Data.ThemeValues[0]);
            Assert.IsNull(rslt[0].Data.ThemeValues[1]);
        }

        [Test]
        public void TestProcessRequest_WithOneRangeEnabled_WithSensorTypeFilter()
        {
            var rslt = this.CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter = new[] { SensorType.Data, SensorType.Entity };

            this.InsertRationalRange(1, 1, -10, 10, true);
            this.InsertRationalRange(2, 1, -10, 0, true);
            this.InsertRationalRange(3, 1, -10, 0, true);

            filter.ProcessResult(rslt);

            Assert.IsNull(rslt[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt[0].Data.ThemeValues[1]);
        }
    }
}
