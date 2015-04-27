namespace DataValidator.Test
{
    using System;
    using System.Collections.Generic;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET.DataValidator;
    using FS.SMIS_Cloud.NGET.DataValidator.Window;
    using FS.SMIS_Cloud.NGET.Model;

    using NUnit.Framework;

    internal class MockSensor : SensorData
    {
        public MockSensor(double[] rawvalues, double[] phyvalues, double[] collPhyValues)
            : base(rawvalues, phyvalues, collPhyValues)
        {
        }
    }

    [TestFixture]
    public class ProcessResultTest
    {
        [TestFixtureSetUp]
        public void TestSetUp()
        {
            this.DeleteData();
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
            this.DeleteData();
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
            var validator = new DataValidator();
            validator.SensorTypeFilter = new SensorType[] {SensorType.Data, SensorType.Entity, SensorType.Virtual};
            for (int i = 0; i < 50; i++)
            {
                var source = new List<SensorAcqResult>();
                source.Add(
                    new SensorAcqResult
                    {
                        AcqTime = DateTime.Now,
                        ErrorCode = 0,
                        Sensor = new Sensor {SensorID = 990},
                        Data =
                            new SensorData(new double[] { 1, 2, 3 }, new double[] { 0.1, 0.2, 0.3 }, new double[] { 0.1, 0.2, 0.3 })
                    });

                validator.ProcessResult(source);
            }

            var newData = new List<SensorAcqResult>();
            newData.Add(
                new SensorAcqResult
                {
                    AcqTime = DateTime.Now,
                    ErrorCode = 0,
                    Sensor = new Sensor {SensorID = 990},
                    Data =
                        new SensorData(new double[] { 1, 2, 3 }, new double[] { 1000, 100, 800 }, new double[] { 1000, 100, 800 })
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(0.1, newData[0].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, newData[0].Data.ThemeValues[1]);
            Assert.AreEqual(800, newData[0].Data.ThemeValues[2]); //没有过滤
        }

        [Test]
        public void TestProcessResultNull()
        {
            var validator = new DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            validator.ProcessResult(null);
        }
       
        [Test]
        public void TestProcessSensorNull()
        {
            var validator = new DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            var source = new List<SensorAcqResult>();
            validator.ProcessResult(source);
        }

        [Test]
        public void TestProcessDataNull()
        {
            var validator = new DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            var source = new List<SensorAcqResult>();
            source.Add(
                new SensorAcqResult
                {
                    AcqTime = DateTime.Now,
                    ErrorCode = 0,
                    Sensor = new Sensor {SensorID = 990}
                });
            validator.ProcessResult(source);
        }

        [Test]
        public void TestProcessThemeNull()
        {
            var validator = new DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            var source = new List<SensorAcqResult>();
            source.Add(
                new SensorAcqResult
                    {
                        AcqTime = DateTime.Now,
                        ErrorCode = 0,
                        Sensor = new Sensor { SensorID = 990 },
                        Data = new MockSensor(null, null, null),
                    });
            validator.ProcessResult(source);
        }

        [Test]
        public void TestConfigInfoChange()
        {
            var validator = new DataValidator();
            validator.SensorTypeFilter = new SensorType[] { SensorType.Data, SensorType.Entity, SensorType.Virtual };
            for (int i = 0; i < 50; i++)
            {
                var source = new List<SensorAcqResult>();
                source.Add(
                    new SensorAcqResult
                    {
                        AcqTime = DateTime.Now,
                        ErrorCode = 0,
                        Sensor = new Sensor {SensorID = 990},
                        Data =
                            new SensorData(new double[] { 1, 2, 3 }, new double[] { 0.1, 0.2, 0.3 }, new double[] { 0.1, 0.2, 0.3 })
                    });
                validator.ProcessResult(source);
            }

            //变更数据库里面的信息
            this.ChangeData();

            var newData = new List<SensorAcqResult>();
            newData.Add(
                new SensorAcqResult
                {
                    AcqTime = DateTime.Now,
                    ErrorCode = 0,
                    Sensor = new Sensor {SensorID = 990},
                    Data =
                        new SensorData(new double[] { 1, 2, 3 }, new double[] { 1000, 100, 800 }, new double[] { 1000, 100, 800 })
                });
            validator.ProcessResult(newData);
            Assert.AreEqual(0.1, newData[0].Data.ThemeValues[0]);
            Assert.AreEqual(0.2, newData[0].Data.ThemeValues[1]);

            Assert.AreEqual(800, newData[0].Data.ThemeValues[2]); //没有过滤

            //恢复数据
            this.TestTearDown();
            this.TestSetUp();
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
