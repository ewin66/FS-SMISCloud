//using System;
//using FS.SMIS_Cloud.DAC.Accessor;
//using FS.SMIS_Cloud.DAC.DAC;
//using FS.SMIS_Cloud.DAC.Model;
//using FS.SMIS_Cloud.DAC.Task;
//using log4net;
//using NUnit.Framework;

//namespace DAC.Test.Model
//{
//    using FS.DbHelper;
//    using FS.SMIS_Cloud.DAC.Accessor.MSSQL;
//    using FS.SMIS_Cloud.DAC.Accessor.SQLite;

//    [TestFixture]
//    public class SQLiteDataSerializerTester
//    {
//        private const string Connstr = "FSUSDB\\fsuscfg.db3,FSUSDB\\FSUSDataValueDB.db3";

//        [SetUp]
//        public void SetUp()
//        {
//            ILog log = LogManager.GetLogger("Hello");
//            log.Info("Start");
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//        }

//        [Test]
//        public void TestInclinometerSerializer()
//        {
//            DACTaskResult r = new DACTaskResult();
//            var s1 = NewSensor(ProtocolType.Inclinometer_BOX);
//            var s2 = NewSensor(ProtocolType.Inclinometer_MOBIL);
//            var s3 = NewSensor(ProtocolType.Inclinometer_OLD);
//            var s4 = NewSensor(ProtocolType.Inclinometer_ROD);
//            var d = new InclinationData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(1);
//            //d.AddRawValue(2);
//            //d.AddRawValue(3);
//            //d.AddRawValue(4);
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s1
//            });
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s2
//            });
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s3
//            });
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s4
//            });
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(4, savedCnt);
//        }

//        [Test]
//        public void TestSerialInvalidData()
//        {
//            DACTaskResult r = new DACTaskResult();
//            var s1 = NewSensor(ProtocolType.Inclinometer_BOX);
//            var d = new WindData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(1);
//            //d.AddRawValue(2);
//            //d.AddRawValue(3);
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s1
//            });
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(0, savedCnt);
//        }

//        [Test]
//        public void TestLVDTSerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s1 = NewSensor(ProtocolType.LVDT_BL);
//            var s2 = NewSensor(ProtocolType.LVDT_XW);
//            var d = new LVDTData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(25.4);
//            //d.AddRawValue(0.65);
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s1
//            });
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s2
//            });
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(2, savedCnt);
//        }

//        [Test]
//        public void TestMageneticFluxSerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s1 = NewSensor(ProtocolType.MagneticFlux);
//            var d = new MagneticFluxData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(25.4);
//            //d.AddRawValue(0.65);
//            //d.AddRawValue(0.33);
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s1
//            });
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(1, savedCnt);
//        }

//        [Test]
//        public void TestPressureSerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s1 = NewSensor(ProtocolType.Pressure_HS);
//            var s2 = NewSensor(ProtocolType.Pressure_MPM);
//            var d = new PressureData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(25.4);
//            //d.AddRawValue(0.65);
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s1
//            });
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s2
//            });
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(2, savedCnt);
//        }


//        [Test]
//        public void TestRainFallSerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s = NewSensor(ProtocolType.RainFall);
//            var d = new RainFallData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(1);
//            SensorAcqResult r1 = new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s
//            };
//            r.AddSensorResult(r1);
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(1, savedCnt);
//        }


//        [Test]
//        public void TestTempHumiditySerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s1 = NewSensor(ProtocolType.TempHumidity);
//            var s2 = NewSensor(ProtocolType.TempHumidity_OLD);
//            var d = new TempHumidityData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(25.4);
//            //d.AddRawValue(0.65);
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s1
//            });
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s2
//            });
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(2, savedCnt);
//        }

//        private static Sensor NewSensor(ProtocolType type)
//        {
//            return new Sensor
//            {
//                SensorID = 999,
//                ProtocolType = (uint)type,
//                ModuleNo = 99,
//                ChannelNo = 12345,
//                DtuID = 123,
//                Name = "TEST",
//            };
//        }

//        [Test]
//        public void TestVibratingWireSerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s1 = NewSensor(ProtocolType.VibratingWire);
//            var s2 = NewSensor(ProtocolType.VibratingWire_OLD);
//            var d = new VibratingWireData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//            };
//            //d.AddRawValue(1);
//            //d.AddRawValue(2);
//            //d.AddRawValue(3);
//            //d.AddRawValue(4);
//            r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s1
//            });
//                 r.AddSensorResult(new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s2
//            });
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(2, savedCnt);
//        }


//        [Test]
//        public void TestVoltageSerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s = NewSensor(ProtocolType.Voltage);

//            var d = new VoltageData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0
//            };
//            //d.AddRawValue(12.23);
//            //d.AddRawValue(2.22);

//            SensorAcqResult r1 = new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s
//            };
//            r.AddSensorResult(r1);
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(1, savedCnt);

//            Assert.AreEqual(0, DbAccessorHelper.DbAccessor.SaveDacResult(null));
//            // null sensor
//            r.SensorResults[0].Sensor = null;
//            Assert.AreEqual(0, DbAccessorHelper.DbAccessor.SaveDacResult(r));
//            // null data/sensor
//            r.SensorResults[0].Data = null;
//            Assert.AreEqual(0, DbAccessorHelper.DbAccessor.SaveDacResult(r));
//            // null data
//            r.SensorResults[0].Sensor = s;
//            Assert.AreEqual(0, DbAccessorHelper.DbAccessor.SaveDacResult(r));
//            // protocol error.
//            s.ProtocolType = 9999;
//            r.SensorResults[0].Data = d;
//            Assert.AreEqual(0, DbAccessorHelper.DbAccessor.SaveDacResult(r));

//            // datetime is null, but value is OK
//            s.ProtocolType = 1200;
//            d.AcqTime = System.DateTime.MinValue;
//            Assert.AreEqual(1, DbAccessorHelper.DbAccessor.SaveDacResult(r));

//            SqlHelperFactory.Create(DbType.SQLite, cs[1]).ExecuteSql("delete from D_OriginalVoltageData where SENSOR_Set_ID=999");
//        }


//        [Test]
//        public void TestWindSerializer()
//        {
//            var cs = Connstr.Split(',');
//            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0], null, cs[1], null));
//            DACTaskResult r = new DACTaskResult();
//            var s = NewSensor(ProtocolType.Wind_OSL);
//            var d = new WindData
//            {
//                AcqTime = System.DateTime.Now,
//                ResultCode = 0,
//                AirSpeed = 123,
//            };
//            //d.AddRawValue(1);
//            //d.AddRawValue(2);
//            //d.AddRawValue(3);
//            SensorAcqResult r1 = new SensorAcqResult
//            {
//                Data = d,
//                Sensor = s
//            };
//            r.AddSensorResult(r1);
//            int savedCnt = DbAccessorHelper.DbAccessor.SaveDacResult(r);
//            Assert.AreEqual(1, savedCnt);
//        }

//    }
//}
