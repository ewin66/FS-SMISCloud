namespace DAC.DataRationalFilter.Test
{
    using System.Configuration;

    using FS.DbHelper;
    using FS.SMIS_Cloud.DAC.DAC;
    using FS.SMIS_Cloud.DAC.Model;
    using FS.SMIS_Cloud.DAC.Model.Sensors;
    using FS.SMIS_Cloud.DAC.Task;
    using FS.SMIS_Cloud.DAC.DataRationalFilter;

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

        public DACTaskResult CreateDACTaskResult()
        {
            var rslt = new DACTaskResult { ErrorCode = 0 };
            rslt.AddSensorResult(
                new SensorAcqResult
                    {
                        Sensor = new Sensor { SensorID = 1, SensorType = SensorType.Entity },
                        ErrorCode = 0,
                        Data = new TempHumidityData(30, 70)
                    });
            rslt.AddSensorResult(
                new SensorAcqResult
                    {
                        Sensor = new Sensor { SensorID = 2, SensorType = SensorType.Data },
                        ErrorCode = 0,
                        Data = new VibratingWireData(1000, 0, 0.03, 1000)
                    });
            rslt.AddSensorResult(
                new SensorAcqResult
                    {
                        Sensor = new Sensor { SensorID = 3, SensorType = SensorType.Virtual },
                        ErrorCode = 0,
                        Data = new Gps3dData(1000, 2000, 3000, 0.1, 0.2, 0.3)
                    });
            return rslt;
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
            var rslt = CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.ProcessResult(rslt);

            Assert.AreEqual(30, rslt.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(0.03, rslt.SensorResults[1].Data.ThemeValues[0]);
            Assert.AreEqual(0.1, rslt.SensorResults[2].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, rslt.SensorResults[2].Data.ThemeValues[1]);
            Assert.AreEqual(0.3, rslt.SensorResults[2].Data.ThemeValues[2]);
        }

        [Test]
        public void TestProcessRequest_WithOneRangeEnabled()
        {
            var rslt = CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter=new SensorType[]{SensorType.Data, SensorType.Entity, SensorType.Virtual};
            InsertRationalRange(1, 1, -10, 10, true);

            filter.ProcessResult(rslt);

            Assert.IsNull(rslt.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(0.03, rslt.SensorResults[1].Data.ThemeValues[0]);
            Assert.AreEqual(0.1, rslt.SensorResults[2].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, rslt.SensorResults[2].Data.ThemeValues[1]);
            Assert.AreEqual(0.3, rslt.SensorResults[2].Data.ThemeValues[2]);
        }

        [Test]
        public void TestProcessRequest_WithOneRangeNotEnabled()
        {
            var rslt = CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            InsertRationalRange(1, 1, -10, 10, false);

            filter.ProcessResult(rslt);

            Assert.AreEqual(30, rslt.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(0.03, rslt.SensorResults[1].Data.ThemeValues[0]);
            Assert.AreEqual(0.1, rslt.SensorResults[2].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, rslt.SensorResults[2].Data.ThemeValues[1]);
            Assert.AreEqual(0.3, rslt.SensorResults[2].Data.ThemeValues[2]);
        }

        [Test]
        public void TestProcessRequest_WithTwoItemsRangeEnabled()
        {
            var rslt = CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            InsertRationalRange(1, 1, -10, 10, true);
            InsertRationalRange(1, 2, -10, 10, true);

            filter.ProcessResult(rslt);

            Assert.IsNull(rslt.SensorResults[0].Data.ThemeValues[0]);
            Assert.IsNull(rslt.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(0.03, rslt.SensorResults[1].Data.ThemeValues[0]);
            Assert.AreEqual(0.1, rslt.SensorResults[2].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, rslt.SensorResults[2].Data.ThemeValues[1]);
            Assert.AreEqual(0.3, rslt.SensorResults[2].Data.ThemeValues[2]);
        }

        [Test]
        public void TestProcessRequest_WithOneRangeEnabled_WithSensorTypeFilter()
        {
            var rslt = CreateDACTaskResult();
            var filter = new DataRationalFilter();
            filter.SensorTypeFilter = new[] { SensorType.Data, SensorType.Entity };

            InsertRationalRange(1, 1, -10, 10, true);
            InsertRationalRange(2, 1, -10, 0, true);
            InsertRationalRange(3, 1, -10, 0, true);

            filter.ProcessResult(rslt);

            Assert.IsNull(rslt.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(70, rslt.SensorResults[0].Data.ThemeValues[1]);
            Assert.IsNull(rslt.SensorResults[1].Data.ThemeValues[0]);
            Assert.AreEqual(0.1, rslt.SensorResults[2].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, rslt.SensorResults[2].Data.ThemeValues[1]);
            Assert.AreEqual(0.3, rslt.SensorResults[2].Data.ThemeValues[2]);
        }
    }
}
