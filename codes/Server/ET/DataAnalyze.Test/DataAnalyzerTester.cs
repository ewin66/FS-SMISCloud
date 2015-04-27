namespace DAC.DataAnalyze.Test
{
    using System;
    using System.Collections.Generic;

    using FS.SMIS_Cloud.DAC.Accessor;
    using FS.SMIS_Cloud.DAC.Accessor.MSSQL;
    using FS.SMIS_Cloud.DAC.DAC;
    using FS.SMIS_Cloud.DAC.DataAnalyzer;
    using FS.SMIS_Cloud.DAC.DataAnalyzer.Model;
    using FS.SMIS_Cloud.DAC.Model;
    using FS.SMIS_Cloud.DAC.Model.Sensors;

    using NUnit.Framework;

    [TestFixture]
    class DataAnalyzerTester
    {
        private const string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";

        [TestFixtureSetUp]
        public void SetUp()
        {
            DbAccessorHelper.Init(new MsDbAccessor(cs));
        }

        [Test]
        public void TestAnalyzeSensorData()
        {
            DataAnalyzer da = new DataAnalyzer();
            var analyzingData = new AnalyzingData { SensorId = 17, Data = new double?[]{ 1, 2, 3 } };
            var thresholds = new List<SensorThreshold>();

            var act0 = da.AnalyzeSensorData(analyzingData, thresholds);
            Assert.AreEqual(100, act0.Score);
            Assert.IsNull(act0.ThresholdAlarm);

            thresholds.Add(
                new SensorThreshold
                    {
                        SensorId = 17,
                        ItemId = 1,
                        LevelNumber = 3,
                        Thresholds =
                            new List<Threshold>
                                {
                                    new Threshold { Level = 1, Down = 1.5, Up = double.MaxValue },
                                    new Threshold { Level = 2, Down = 0.8, Up = 1.5 },
                                    new Threshold { Level = 3, Down = 0.5, Up = 0.8 }
                                }
                    });

            var act1 = da.AnalyzeSensorData(analyzingData, thresholds);
            Assert.AreEqual(100 - (int)((1 - (double)2 / 3) / 3 * 100), act1.Score);
            Assert.AreEqual(1, act1.ThresholdAlarm.AlarmDetails.Count);
            Console.WriteLine(act1.ThresholdAlarm);

            thresholds.Add(
                new SensorThreshold
                    {
                        SensorId = 17,
                        ItemId = 2,
                        LevelNumber = 4,
                        Thresholds =
                            new List<Threshold>
                                {
                                    new Threshold { Level = 1, Down = 1.5, Up = double.MaxValue },
                                    new Threshold { Level = 2, Down = 0.8, Up = 1 },
                                    new Threshold { Level = 3, Down = 0.5, Up = 0.8 },
                                    new Threshold { Level = 4, Down = 0.3, Up = 0.5 }
                                }
                    });

            var act2 = da.AnalyzeSensorData(analyzingData, thresholds);
            Assert.AreEqual(
                100 - (int)((1 - (double)2 / 3) / 3 * 100) - (int)((1 - (double)1 / 4) / 3 * 100),
                act2.Score);
            Assert.AreEqual(2, act2.ThresholdAlarm.AlarmDetails.Count);
            Console.WriteLine(act2.ThresholdAlarm);

        }

        [Test]        
        public void TestGetSensorThreshold()
        {
            DataAnalyzer da = new DataAnalyzer();
            var act1 = da.GetSensorThreshold(new uint[] { 17, 18 });
            Assert.AreEqual(2, act1.Count);            
            Assert.AreEqual(2, act1[0].LevelNumber);
            Assert.AreEqual(1, act1[1].LevelNumber);

            var act2 = da.GetSensorThreshold(null);
            Assert.AreEqual(0, act2.Count);

            var act3 = da.GetSensorThreshold(new uint[] {});
            Assert.AreEqual(0, act3.Count);
        }

        // TempHumidityData-温湿度
        [Test]
        public void TestGetAnalyzingData_Th_Th()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.TempHumidity,
                        TableColums = "温度,湿度"
                    },
                Data =
                    new TempHumidityData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 1, 2 }, act1.Data);
        }

        // RainFallData-雨量
        [Test]
        public void TestGetAnalyzingData_Rain_Rain()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Rainfall,
                        TableColums = "雨量"
                    },
                Data =
                    new RainFallData(1)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 1 }, act1.Data);
        }

        // GPSData-表面
        [Test]
        public void TestGetAnalyzingData_GPS_Surf()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
                            {
                                Sensor =
                                    new Sensor
                                        {
                                            SensorID = 17,
                                            FactorType = (int)SafetyFactor.GpsSurfaceDisplacement,
                                            TableColums = "x,y,z"
                                        },
                                Data =
                                    new Gps3dData(100,200,300,1,2,3)
                            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 1, 2, 3 }, act1.Data);
        }

        // InclinationData-内部
        [Test]
        public void TestGetAnalyzingData_Inc_Deep()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.DeepDisplacement,
                        TableColums = "x,y"
                    },
                Data =
                    new InclinationData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3, 4 }, act1.Data);
        }

        // PressureData-沉降
        [Test]
        public void TestGetAnalyzingData_Pre_Set()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Settlement,
                        TableColums = "沉降"
                    },
                Data =
                    new PressureData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // GpsData-沉降
        [Test]
        public void TestGetAnalyzingData_Gps_Set()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Settlement,
                        TableColums = "沉降"
                    },
                Data =
                    new GpsHeightData(3,6)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 6 }, act1.Data);
        }

        // PressureData-孔隙水压力
        [Test]
        public void TestGetAnalyzingData_Pre_Wp()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.StressStrainPoreWaterPressure,
                        TableColums = "压力"
                    },
                Data =
                    new PressureData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // VibratingWireData-孔隙水压力
        [Test]
        public void TestGetAnalyzingData_Vib_Wp()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.StressStrainPoreWaterPressure,
                        TableColums = "压力"
                    },
                Data =
                    new VibratingWireData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // VibratingWireData-土体压力
        [Test]
        public void TestGetAnalyzingData_Vib_Ep()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.ForceEarthPressure,
                        TableColums = "压力"
                    },
                Data =
                    new VibratingWireData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // VibratingWireData-水位
        [Test]
        public void TestGetAnalyzingData_Vib_Wl()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.WaterLevel,
                        TableColums = "水位"
                    },
                Data =
                    new VibratingWireData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // WindData-风速风向
        [Test]
        public void TestGetAnalyzingData_Wind_W2()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Wind2D,
                        TableColums = "风速,风向"
                    },
                Data =
                    new Wind2dData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 1,2 }, act1.Data);
        }

        // GpsData-塔顶偏位
        [Test]
        public void TestGetAnalyzingData_Gps_Td()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
                           {
                               Sensor =
                                   new Sensor
                                       {
                                           SensorID = 17,
                                           FactorType = (int)SafetyFactor.TopOfTowerDeviation,
                                           TableColums = "x,y,z"
                                       },
                               Data =
                                   new Gps3dData(100, 200, 300, 1, 2, 3)
                           };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 1, 2, 3 }, act1.Data);
        }

        // InclinationData-桥墩倾斜
        [Test]
        public void TestGetAnalyzingData_Inc_Bd()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
                           {
                               Sensor =
                                   new Sensor
                                       {
                                           SensorID = 17,
                                           FactorType =
                                               (int)SafetyFactor.DeformationDeepDisplacement,
                                            TableColums = "x,y"
                                       },
                               Data =
                                   new InclinationData(1,2,3,4)
                           };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3, 4 }, act1.Data);
        }

        // PressureData-梁段挠度
        [Test]
        public void TestGetAnalyzingData_Pre_Bs()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.DeformationBridgeDeflection,
                        TableColums = "挠度"
                    },
                Data =
                    new PressureData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // GpsData-梁段挠度
        [Test]
        public void TestGetAnalyzingData_Gps_Bs()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.DeformationBridgeDeflection,
                        TableColums = "挠度"
                    },
                Data =
                    new GpsHeightData(3,6)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 6 }, act1.Data);
        }

        // LVDTData-桥梁伸缩缝
        [Test]
        public void TestGetAnalyzingData_Lvdt_Cj()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.DeformationCrackJoint,
                        TableColums = "裂缝"
                    },
                Data =
                    new LVDTData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // VibratingWireData-应力应变
        [Test]
        public void TestGetAnalyzingData_Vib_Fb()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Forcesteelbar,
                        TableColums = "应变"
                    },
                Data =
                    new VibratingWireData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // LVDTData-Lvdt表面
        [Test]
        public void TestGetAnalyzingData_Lvdt_Surf()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.LvdtSurfaceDisplacement,
                        TableColums = "x"
                    },
                Data =
                    new LVDTData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // TempHumidityData-温度
        [Test]
        public void TestGetAnalyzingData_Th_T()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Temp,
                        TableColums = "温度"
                    },
                Data =
                    new TempData(1)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 1 }, act1.Data);
        }

        // LVDTData-裂缝
        [Test]
        public void TestGetAnalyzingData_Lvdt_Cr()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.DeformationCrack,
                        TableColums = "裂缝"
                    },
                Data =
                    new LVDTData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // VoltageData-裂缝
        [Test]
        public void TestGetAnalyzingData_Vol_Cr()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.DeformationCrack,
                        TableColums = "裂缝"
                    },
                Data =
                    new VoltageData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // MagneticFluxData-索力
        [Test]
        public void TestGetAnalyzingData_Mag_Cf()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.CableForce,
                        TableColums = "索力"
                    },
                Data =
                    new MagneticFluxData(1,2,3)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // WindData-风速风向风仰角
        [Test]
        public void TestGetAnalyzingData_Wind_W3()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Wind3D,
                        TableColums = "风速,风向,风仰角"
                    },
                Data =
                    new Wind3dData(1,2,3)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 1, 2, 3 }, act1.Data);
        }

        // GPSData-挠度分组
        [Test]
        public void TestGetAnalyzingData_GPS_Set_Group()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.SettlementGroup,
                        TableColums = "挠度"
                    },
                Data =
                    new GpsHeightData(2,3)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // PressureData-渗流
        [Test]
        public void TestGetAnalyzingData_Pre_Spg()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Seepage,
                        TableColums = "渗流"
                    },
                Data =
                    new PressureData(1,2)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 2 }, act1.Data);
        }

        // VibratingWireData-渗流
        [Test]
        public void TestGetAnalyzingData_Vib_Spg()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.Seepage,
                        TableColums = "渗流"
                    },
                Data =
                    new VibratingWireData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // VibratingWireData-衬砌受压力
        [Test]
        public void TestGetAnalyzingData_Vib_Ip()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.IningPressure,
                        TableColums = "压力"
                    },
                Data =
                    new VibratingWireData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }

        // VibratingWireData-衬砌应变监测
        [Test]
        public void TestGetAnalyzingData_Vib_Ls()
        {
            DataAnalyzer da = new DataAnalyzer();

            var rslt = new SensorAcqResult()
            {
                Sensor =
                    new Sensor
                    {
                        SensorID = 17,
                        FactorType = (int)SafetyFactor.LiningStress,
                        TableColums = "应变"
                    },
                Data =
                    new VibratingWireData(1,2,3,4)
            };
            var act1 = da.GetAnalyzingData(rslt);
            Assert.AreEqual(new double[] { 3 }, act1.Data);
        }
    }
}
