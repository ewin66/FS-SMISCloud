using System;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Storage.SQLite;
using FS.SMIS_Cloud.DAC.Task;
using NUnit.Framework;

namespace DAC.Storage.SQLite.Test
{
    [TestFixture]
    public class SQLiteDataSerializerTester
    {
        private const string Connstr = "FSUSDB\\FSUSDataValueDB.db3";
        private const string Xmlpath = ".\\ThemeTables_SQLite.xml";
        SQLiteDbAccessor sqLiteDbAccessor;


        [TestFixtureSetUp]
        public void SetUp()
        {
            sqLiteDbAccessor = new SQLiteDbAccessor(Connstr);
            var loadxml = new LoadDbConfigXml(Xmlpath);
            sqLiteDbAccessor.UpdateTables(loadxml.GetTableMaps());
        }


        private static Sensor NewSensor(uint protocolType, SafetyFactor type)
        {
            return new Sensor
            {
                SensorID = 17,
                ProtocolType = protocolType,
                ModuleNo = 99,
                ChannelNo = 1,
                DtuID = 123,
                Name = "TEST",
                FactorType = (uint)type
            };
        }


        /// <summary>
        /// 温湿度
        /// </summary>
        [Test]
        public void TestTempHumiditySerializer()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.TempHumidity, SafetyFactor.TempHumidity);
            var d = new TempHumidityData(25.5f, 0.88f)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1,
                ResponseTime = DateTime.Now
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        /// <summary>
        /// 温度
        /// </summary>
        [Test]
        public void TestTempSerializer()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.TempHumidity, SafetyFactor.Temp);
            var d = new TempData(25.5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1,
                ResponseTime = DateTime.Now
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////<summary>
        ////雨量
        ////</summary>
        [Test]
        public void TestRainfallDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.RainFall, SafetyFactor.Rainfall);
            var d = new RainFallData(1.5)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }


        ////<summary>
        ////深部位移
        ////</summary>
        [Test]
        public void TestDeepDisplacementDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Inclinometer_BOX, SafetyFactor.DeepDisplacement);
            var d = new InclinationData(0.023, 0.232, 0.01, 0.02)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }


        ////<summary>
        ////桥墩倾斜
        ////</summary>
        [Test]
        public void TestDeepDisplacement2DataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Inclinometer_OLD, SafetyFactor.DeformationDeepDisplacement);
            var d = new InclinationData(0.023, 0.232, 0.01, 0.02)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }


        ////<summary>
        ////沉降
        ////</summary>
        [Test]
        public void TestDeformationSettlementDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.Settlement);
            var d = new PressureData(23, 23)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestDeformationSettlementGroupDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.SettlementGroup);
            var d = new PressureData(23, 23)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }



        ////<summary>
        ////裂缝
        ////</summary>
        [Test]
        public void TestDeformationCrackDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.LVDT_XW, SafetyFactor.DeformationCrack);
            var d = new LVDTData(10, 5)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestDeformationCrackJointDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.LVDT_XW, SafetyFactor.DeformationCrackJoint);
            var d = new LVDTData(10, 5)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////<summary>
        ////水位
        ////</summary>
        [Test]
        public void TestWaterLevelDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.VibratingWire, SafetyFactor.WaterLevel);
            var d = new VibratingWireData(1869, 25, 18, 100)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////<summary>
        ////风速风向
        ////</summary>
        [Test]
        public void TestWind2DDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Wind_OSL, SafetyFactor.Wind2D);
            var d = new Wind2dData(5, 256)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestWind3DDataSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Wind_OSL, SafetyFactor.Wind3D);
            var d = new Wind2dData(5, 256)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////<summary>
        ////应力应变
        ////</summary>
        [Test]
        public void TestForcesteelbarDataSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Voltage, SafetyFactor.Forcesteelbar);
            var d = new VibratingWireData(1869, 25, 18, 18)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////<summary>
        ////索力
        ////</summary>
        [Test]
        public void TestCableForceDataSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.MagneticFlux, SafetyFactor.CableForce);
            var d = new MagneticFluxData(18.69, 25, 500)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////<summary>
        ////挠度
        ////</summary>
        [Test]
        public void TestDeformationBridgeDeflectionDataSerializer()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.DeformationBridgeDeflection);
            var d = new PressureData(23, 23)
            {
                ////AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestVoltageDataSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Voltage, SafetyFactor.DeformationCrack);
            var d = new VoltageData(0.2, 2.5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });
            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestForceEarthPressureDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.VibratingWire, SafetyFactor.ForceEarthPressure);
            var d = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult()
            {
                Data = d,
                Sensor = s1
            });
            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestSeepageDataSerializer()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.Seepage);
            var d = new SeepageData(23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt); // 插入一条记录
        }

        [Test]
        public void TestStressStrainPoreWaterPressureDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.StressStrainPoreWaterPressure);
            var d = new PressureData(23, 23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });

            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);

            DACTaskResult r2 = new DACTaskResult();
            var s2 = NewSensor(ProtocolType.VibratingWire, SafetyFactor.StressStrainPoreWaterPressure);
            var d2 = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s2
            };
            r2.AddSensorResult(new SensorAcqResult
            {
                Data = d2,
                Sensor = s2
            });
            savedCnt = sqLiteDbAccessor.SaveDacResult(r2);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestLogErrorData()
        {
            var r = new DACTaskResult();
            var s2 = NewSensor(ProtocolType.GPS_HC, SafetyFactor.StressStrainPoreWaterPressure);
            var d2 = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s2
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d2,
                Sensor = s2
            });
            int savedCnt = sqLiteDbAccessor.SaveDacResult(r);
            Assert.AreEqual(0, savedCnt);
        }


        [Test]
        public void TestSQLiteStorge()
        {
            var st =new SQLiteStorge();

            var r = new DACTaskResult();
            var s2 = NewSensor(ProtocolType.GPS_HC, SafetyFactor.StressStrainPoreWaterPressure);
            ISensorData d = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s2
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s2
            });
            var s1 = NewSensor(ProtocolType.Voltage, SafetyFactor.DeformationCrack);
             d = new VoltageData(0.2, 2.5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });
             s1 = NewSensor(ProtocolType.Wind_OSL, SafetyFactor.Wind2D);
             d = new Wind2dData(5, 256)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s1
            });
             s2 = NewSensor(ProtocolType.GPS_HC, SafetyFactor.StressStrainPoreWaterPressure);
             d = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s2
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                Sensor = s2
            });
            st.ProcessResult(r);
        }
    }
}
