using System;
using System.Configuration;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataValidator.Window;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Task;
using NUnit.Framework;

namespace DAC.DataValidator.Test
{
    internal class MockSensor : BasicSensorData
    {
        public override double[] RawValues
        {
            get { throw new NotImplementedException(); }
        }

        public override double[] PhyValues
        {
            get { throw new NotImplementedException(); }
        }

        public override double[] CollectPhyValues
        {
            get { throw new NotImplementedException(); }
        }

        public override void DropThemeValue(int colphyindex)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class ProcessResultTest
    {
        [TestFixtureSetUp]
        public void TestSetUp()
        {
            DeleteData();
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            //用代码向数据库插数据
            string sql = @"insert into T_DATA_STABLE_FILTER_CONFIG
Values
('990','1','true','50','0.1','10','25','true'),
('990','2','true','50','0.2','20','25','false'),
('990','3','false','25','0.3','5','25','false')";
            sqlHelper.ExecuteSql(sql);
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {
            DeleteData();
        }

        private void DeleteData()
        {
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = "delete from T_DATA_STABLE_FILTER_CONFIG where SensorId =990";
            sqlHelper.ExecuteSql(sql);
        }

        private void ChangeData()
        {
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                "update T_DATA_STABLE_FILTER_CONFIG set WindowSize='25', KT='0.05', DT='8', RT='20' where SensorId =990 and ItemId=1";
            sqlHelper.ExecuteSql(sql);
        }

        [Test]
        public void TestProcessResult()
        {
            var validator = new FS.SMIS_Cloud.DAC.DataValidator.DataValidator();
            validator.SensorTypeFilter = new SensorType[] {SensorType.Data, SensorType.Entity, SensorType.Virtual};
            for (int i = 0; i < 50; i++)
            {
                DACTaskResult source = new DACTaskResult
                {
                    Elapsed = 1000,
                    ErrorCode = 0,
                    ErrorMsg = "123",
                    Finished = new DateTime(1990, 2, 8),
                    Task = null
                };
                source.SensorResults.Add(
                    new SensorAcqResult
                    {
                        Elapsed = 100,
                        ErrorCode = 0,
                        Sensor = new Sensor {SensorID = 990},
                        Data =
                            new Gps3dData(1, 2, 3, 0.1, 0.2, 0.3)
                            {
                                //Sensor = new Sensor {SensorID = 990}
                            },
                        Request = new byte[] {1, 2, 3, 4, 5}
                    });

                validator.ProcessResult(source);
            }
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
                    Sensor = new Sensor {SensorID = 990},
                    Data =
                        new Gps3dData(1, 2, 3, 1000, 100, 800)
                        {
                            //Sensor = new Sensor {SensorID = 990}
                        },
                    Request = new byte[] {1, 2, 3, 4, 5}
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(0.1, newData.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, newData.SensorResults[0].Data.ThemeValues[1]);
            Assert.AreEqual(800, newData.SensorResults[0].Data.ThemeValues[2]); //没有过滤
        }

        [Test]
        public void TestProcessResultNull()
        {
            var validator = new FS.SMIS_Cloud.DAC.DataValidator.DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            validator.ProcessResult(null);
        }
       
        [Test]
        public void TestProcessSensorNull()
        {
            var validator = new FS.SMIS_Cloud.DAC.DataValidator.DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            DACTaskResult source = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            validator.ProcessResult(source);
        }

        [Test]
        public void TestProcessDataNull()
        {
            var validator = new FS.SMIS_Cloud.DAC.DataValidator.DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            DACTaskResult source = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            source.SensorResults.Add(
                new SensorAcqResult
                {
                    Elapsed = 100,
                    ErrorCode = 0,
                    Sensor = new Sensor {SensorID = 990}
                });
            validator.ProcessResult(source);
        }

        [Test]
        public void TestProcessThemeNull()
        {
            var validator = new FS.SMIS_Cloud.DAC.DataValidator.DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            DACTaskResult source = new DACTaskResult
            {
                Elapsed = 1000,
                ErrorCode = 0,
                ErrorMsg = "123",
                Finished = new DateTime(1990, 2, 8),
                Task = null
            };
            source.SensorResults.Add(
                new SensorAcqResult
                {
                    Elapsed = 100,
                    ErrorCode = 0,
                    Sensor = new Sensor {SensorID = 990},
                    Data =
                        new MockSensor()
                        {
                            //Sensor = new Sensor {SensorID = 990}
                        },
                });
            validator.ProcessResult(source);
        }

        [Test]
        public void TestConfigInfoChange()
        {
            var validator = new FS.SMIS_Cloud.DAC.DataValidator.DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            for (int i = 0; i < 50; i++)
            {
                DACTaskResult source = new DACTaskResult
                {
                    Elapsed = 1000,
                    ErrorCode = 0,
                    ErrorMsg = "123",
                    Finished = new DateTime(1990, 2, 8),
                    Task = null
                };
                source.SensorResults.Add(
                    new SensorAcqResult
                    {
                        Elapsed = 100,
                        ErrorCode = 0,
                        Sensor = new Sensor {SensorID = 990},
                        Data =
                            new Gps3dData(1, 2, 3, 0.1, 0.2, 0.3)
                            {
                                //Sensor = new Sensor {SensorID = 990}
                            },
                        Request = new byte[] {1, 2, 3, 4, 5}
                    });
                validator.ProcessResult(source);
            }

            //变更数据库里面的信息
            ChangeData();

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
                    Sensor = new Sensor {SensorID = 990},
                    Data =
                        new Gps3dData(1, 2, 3, 1000, 100, 800)
                        {
                            //Sensor = new Sensor {SensorID = 990}
                        },
                    Request = new byte[] {1, 2, 3, 4, 5}
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(0.1, newData.SensorResults[0].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, newData.SensorResults[0].Data.ThemeValues[1]);

            Assert.AreEqual(800, newData.SensorResults[0].Data.ThemeValues[2]); //没有过滤

            //恢复数据
            TestTearDown();
            TestSetUp();
        }

        [Test]
        public void TestAnalysisValueToString()
        {
            var value = new AnalysisValue(100.0m);
            Console.WriteLine(value.ToString());
            Assert.AreEqual("Raw value:          100.0, Valid value:              0, Is valid: True", value.ToString());
            value.IsValid = false;
            value.ValidValue = 90;
            Assert.AreEqual("Raw value:          100.0, Valid value:             90, Is valid:False", value.ToString());
        }

        [Test]
        public void TestWindowToString()
        {
            var window = new ValidateWindow();
            Console.WriteLine(window.ToString());
            for (int i = 0; i < window.WindowSize; i++)
            {
                window.ProcessValue(new AnalysisValue(100.0003m));
            }
            var value = new AnalysisValue(100);
            window.ProcessValue(value);
            Console.WriteLine(window.ToString());
        }
    }
}
