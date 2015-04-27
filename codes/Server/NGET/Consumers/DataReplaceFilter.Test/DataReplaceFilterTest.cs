namespace DataReplaceFilter.Test
{
    using System;
    using System.Collections.Generic;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET.DataReplaceFilter;
    using FS.SMIS_Cloud.NGET.Model;

    using NUnit.Framework;

    [TestFixture]
    public class DataReplaceFilterTest
    {
        [TestFixtureSetUp]
        public void TestSetUp()
        {
            this.DeleteData();
            this.InsertData(17, 171, 172, 173, 174);
            this.InsertData(18, 181, 182, 183, null);
            this.InsertData(19, 191, 192, null, null);
        }

        private void DeleteData()
        {
            string sqlString = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, sqlString);
            string sql = "delete from T_DIM_ABNORMALSENSOR_CONFIG where SensorId in (17,18,19)";
            sqlHelper.ExecuteSql(sql);
        }

        private void InsertData(int sensorId, decimal? value1, decimal? value2, decimal? value3, decimal? value4)
        {
            string sqlString = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, sqlString);
            //用代码向数据库插数据
            var sql = string.Format(@"
                        insert into T_DIM_ABNORMALSENSOR_CONFIG
                        values({0},{1},{2},{3},{4},1,{5},{6})
             ", sensorId, value1 == null ? "null" : Convert.ToString(value1), value2 == null ? "null" : Convert.ToString(value2), value3 == null ? "null" : Convert.ToString(value3), value4 == null ? "null" : Convert.ToString(value4), "null", "null");
            sqlHelper.ExecuteSql(sql);
        }
        [TestFixtureTearDown]
        public void TestTearDown()
        {
            this.DeleteData();
        }
        [Test]
        public void TestProcessResult()
        {
            this.TestSetUp();
            var validator = new DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            List<SensorAcqResult> newData = new List<SensorAcqResult>(3);
            newData.Add(
                new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 18 },
                    Data =
                        new SensorData( new double[]{1, 2, 3},
                            new double[] {1000, 100, 800},
                            new double[] {1000, 100, 800})
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(181, newData[0].Data.ThemeValues[0]);
            Assert.AreEqual(182, newData[0].Data.ThemeValues[1]);
            Assert.AreEqual(183, newData[0].Data.ThemeValues[2]);
        }

        [Test]
        public void TestSensorIdNotInConfig()
        {
            this.TestSetUp();
            var validator = new DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            List<SensorAcqResult> newData = new List<SensorAcqResult>(3);
            newData.Add(
                new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 20 },
                    Data =
                        new SensorData(new double[] { 1, 2, 3 },
                            new double[] { 1000, 100, 800 },
                            new double[] { 1000, 100, 800 })
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(1000, newData[0].Data.ThemeValues[0]);
            Assert.AreEqual(100, newData[0].Data.ThemeValues[1]);
            Assert.AreEqual(800, newData[0].Data.ThemeValues[2]); 
        }
        [Test]
        public void TestWithOneThemeValue()
        {
            this.TestSetUp();
            var validator = new DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            List<SensorAcqResult> newData = new List<SensorAcqResult>(3);
            newData.Add(
                new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 18 },
                    Data =
                        new SensorData(new double[] { 1, 2, 3 },
                            new double[] { 1000, 100, 800 },
                            new double[] { 1000, 100, 800 })
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(181, newData[0].Data.ThemeValues[0]);
            
        }
        [Test]
        public void TestConfigWithoutValue()
        {
            this.DeleteData();
            var validator = new DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            List<SensorAcqResult> newData = new List<SensorAcqResult>(3);
            newData.Add(
                new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 18 },
                    Data =
                        new SensorData(new double[] { 1, 2, 3 },
                            new double[] { 1000, 100, 800 },
                            new double[] { 1000, 100, 800 })
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(1000, newData[0].Data.ThemeValues[0]);
            Assert.AreEqual(100, newData[0].Data.ThemeValues[1]);
            Assert.AreEqual(800, newData[0].Data.ThemeValues[2]); 
        }
    }
}
