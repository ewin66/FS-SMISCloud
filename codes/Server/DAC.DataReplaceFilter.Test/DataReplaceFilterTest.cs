using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Task;
using NUnit;
using NUnit.Framework;


namespace DAC.DataReplaceFilter.Test
{
    [TestFixture]
    public class DataReplaceFilterTest
    {
        [TestFixtureSetUp]
        public void TestSetUp()
        {
            DeleteData();
            InsertData(17, 171, 172, 173, 174);
            InsertData(18, 181, 182, 183, null);
            InsertData(19, 191, 192, null, null);
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
            DeleteData();
        }
        [Test]
        public void TestProcessResult()
        {
            TestSetUp();
            var validator = new FS.SMIS_Cloud.DAC.DataReplaceFilter.DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            DACTaskResult newData = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            newData.SensorResults.Add(
                new SensorAcqResult
                {
                    Elapsed = 100,
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 18 },
                    Data =
                        new Gps3dData(1, 2, 3, 1000, 100, 800)
                        {
                            //Sensor = new Sensor { SensorID = 18 }
                        },
                    Request = new byte[] { 1, 2, 3, 4, 5 }
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(181, newData.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(182, newData.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(183, newData.SensorResults[0].Data.ThemeValues[2]); 
        }
        [Test]
        public void TestSensorIdNotInConfig()
        {
            TestSetUp();
            var validator = new FS.SMIS_Cloud.DAC.DataReplaceFilter.DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            DACTaskResult newData = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            newData.SensorResults.Add(
                new SensorAcqResult
                {
                    Elapsed = 100,
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 20 },
                    Data =
                        new Gps3dData(1, 2, 3, 1000, 100, 800)
                        {
                            //Sensor = new Sensor { SensorID = 20 }
                        },
                    Request = new byte[] { 1, 2, 3, 4, 5 }
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(1000, newData.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(100, newData.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(800, newData.SensorResults[0].Data.ThemeValues[2]); 
        }
        [Test]
        public void TestWithOneThemeValue()
        {
            TestSetUp();
            var validator = new FS.SMIS_Cloud.DAC.DataReplaceFilter.DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            DACTaskResult newData = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            newData.SensorResults.Add(
                new SensorAcqResult
                {
                    Elapsed = 100,
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 18 },
                    Data =
                        new MagneticFluxData( 3, 1000, 10)
                        {
                          // Sensor = new Sensor { SensorID = 18 }
                        },
                    Request = new byte[] { 1, 2, 3, 4, 5 }
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(181, newData.SensorResults[0].Data.ThemeValues[0]);
            
        }
        [Test]
        public void TestConfigWithoutValue()
        {
            DeleteData();
            var validator = new FS.SMIS_Cloud.DAC.DataReplaceFilter.DataReplaceFilter();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            DACTaskResult newData = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            newData.SensorResults.Add(
                new SensorAcqResult
                {
                    Elapsed = 100,
                    ErrorCode = 0,
                    Sensor = new Sensor { SensorID = 18 },
                    Data =
                        new Gps3dData(1, 2, 3, 1000, 100, 800)
                        {
                            //Sensor = new Sensor { SensorID = 18 }
                        },
                    Request = new byte[] { 1, 2, 3, 4, 5 }
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(1000, newData.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(100, newData.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(800, newData.SensorResults[0].Data.ThemeValues[2]); 
        }
    }
}
