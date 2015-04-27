#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="MsDbAccessorTester.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141118 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Storage.iSecureCloud;
using FS.SMIS_Cloud.DAC.Task;
using NUnit.Framework;

namespace DAC.Storage.iSecureCloud.Test
{
    [TestFixture]
    public class MsDbAccessorTester
    {
        private const string Connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";
        private MsDbAccessor msDbAccessor;
        private const string Xmlpath = ".\\ThemeTables_iSecureCloud.xml";
        [TestFixtureSetUp]
        public void SetUp()
        {
            msDbAccessor = new MsDbAccessor(Connstr);
            var loadxml = new LoadDbConfigXml(Xmlpath);
            msDbAccessor.UpdateTables(loadxml.GeTableInfos());
        }

        [Test]
        public void msDbAccessorTest()
        {
            try
            {
                MsDbAccessor msDbAccessor = new MsDbAccessor(Connstr);
                var loadxml = new LoadDbConfigXml(string.Empty);
                msDbAccessor.UpdateTables(loadxml.GeTableInfos());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.True(true);
                
            }

        }



        private static Sensor NewSensor(uint protocolType, SafetyFactor type)
        {
            return new Sensor
            {
                SensorID = 17,
                ProtocolType = (uint)protocolType,
                ModuleNo = 99,
                ChannelNo = 1,
                DtuID = 123,
                Name = "TEST",
                FactorType = (uint)type
            };
        }

        [Test]
        public void Doublecount()
        {
            double x = 3.000400;
            string x1 = string.Format("{0:0.000000}", x);
            string x2 = string.Format("{0:0.######}", x);
            Console.WriteLine(x1);
            Console.WriteLine(x2);
            double? y = null;
            string y1 = string.Format("{0:0.######}", y);
            Console.WriteLine(y1);          
        }


        /// <summary>
        /// 温湿度
        /// </summary>
        [Test]
        public void TestTempHumiditySerializer()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.TempHumidity, SafetyFactor.TempHumidity);
            s1.FactorTypeTable = "T_THEMES_ENVI_TEMP_HUMI";
            var d = new TempHumidityData(25.5f, 0.88f)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1,
                ResponseTime = DateTime.Now
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_ENVI_TEMP_HUMI";
            var d = new TempData(25.5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1,
                ResponseTime = DateTime.Now
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_ENVI_RAINFALL";
            var d = new RainFallData(1.5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_DEEP_DISPLACEMENT";
            var d = new InclinationData(0.023, 0.232, 0.01, 0.02)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_DEEP_DISPLACEMENT";
            var d = new InclinationData(0.023, 0.232, 0.01, 0.02)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_SETTLEMENT";
            var d = new PressureData(23, 23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestDeformationSettlementGroupDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.SettlementGroup);
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_SETTLEMENT";
            var d = new PressureData(23, 23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_CRACK";
            var d = new LVDTData(10, 5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestDeformationCrackJointDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.LVDT_XW, SafetyFactor.DeformationCrackJoint);
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_CRACK";
            var d = new LVDTData(10, 5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_ENVI_WATER_LEVEL";
            var d = new VibratingWireData(1869, 25, 18, 100)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_ENVI_WIND";
            var d = new Wind2dData(5, 256)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestWind3DDataSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Wind_OSL, SafetyFactor.Wind3D);
            s1.FactorTypeTable = "T_THEMES_ENVI_WIND";
            var d = new Wind2dData(5, 256)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_FORCE_STEELBAR";
            var d = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_CABLE_FORCE";
            var d = new MagneticFluxData(18.69, 25, 500)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
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
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_SETTLEMENT";
            var d = new PressureData(23, 23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestDeformationBridgeDeflectionDataSerializerFromGpsData()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.GPS_HC, SafetyFactor.DeformationBridgeDeflection);
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_BRIDGE_DEFLECTION";
            var d = new GpsHeightData(11, 0.01) 
            { 
                //AcqTime = DateTime.Now, 
                //ResultCode = 0, 
                //Sensor = s1 
            };
            r.AddSensorResult(new SensorAcqResult { Data = d, ErrorCode = (int)Errors.SUCCESS, Sensor = s1 });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////<summary>
        ////表面位移
        ////</summary>
        [Test]
        public void TestSurfaceDisplacementSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.GPS_HC, SafetyFactor.GpsSurfaceDisplacement);
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT";
            var d = new Gps3dData(11, 12, 13, 0.01, 0.02, 0.03) 
            {
                //AcqTime = DateTime.Now, 
                //ResultCode = 0, 
                //Sensor = s1 
            };
            r.AddSensorResult(new SensorAcqResult { Data = d, ErrorCode = (int)Errors.SUCCESS, Sensor = s1 });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        ////塔顶偏位
        [Test]
        public void TestTopOfTowerDeviationSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.GPS_HC, SafetyFactor.TopOfTowerDeviation);
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT";
            var d = new Gps3dData(11, 12, 13, 0.01, 0.02, 0.03) 
            { 
                //AcqTime = DateTime.Now, 
                //ResultCode = 0, 
                //Sensor = s1 
            };
            r.AddSensorResult(new SensorAcqResult { Data = d, ErrorCode = (int)Errors.SUCCESS, Sensor = s1 });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestVoltageDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Voltage, SafetyFactor.DeformationCrack);
            s1.FactorTypeTable = "T_THEMES_DEFORMATION_CRACK";
            var d = new VoltageData(0.2, 2.5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestForceEarthPressureDataSerializer()
        {
            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.VibratingWire, SafetyFactor.ForceEarthPressure);
            s1.FactorTypeTable = "T_THEMES_FORCE_EARTH_PRESSURE";
            var d = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult()
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestSeepageDataSerializer()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.Seepage);
            s1.FactorTypeTable = "T_THEMES_ENVI_SEEPAGE";
            var d = new SeepageData(23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(0, savedCnt); // 插入一条记录
        }

        [Test]
        public void TestStressStrainPoreWaterPressureDataSerializer()
        {

            DACTaskResult r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.StressStrainPoreWaterPressure);
            s1.FactorTypeTable = "T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE";
            var d = new PressureData(23, 23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                Data = d,
                ErrorCode = (int)Errors.SUCCESS,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            int savedCnt = msDbAccessor.SaveDacResult(r);
            Assert.AreEqual(1, savedCnt);

            DACTaskResult r2 = new DACTaskResult();
            var s2 = NewSensor(ProtocolType.VibratingWire, SafetyFactor.StressStrainPoreWaterPressure);
            s2.FactorTypeTable = "T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE";
            var d2 = new VibratingWireData(1869, 25, 18, 18)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s2
            };
            r2.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d2,
                Sensor = s2
            });
            r2.Finished = DateTime.Now;
            savedCnt = msDbAccessor.SaveDacResult(r2);
            Assert.AreEqual(1, savedCnt);
        }

        [Test]
        public void TestLogErrorData()
        {
            var r = new DACTaskResult();
            var s1 = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.StressStrainPoreWaterPressure);
            s1.FactorTypeTable = "T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE";
            var d = new PressureData(23, 23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s1
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            r.Finished = DateTime.Now;
            Assert.IsTrue(msDbAccessor.LogErrorData(r.SensorResults[0]));
        }


        [Test]
        public void TestSeclureCloudStorge()
        {
            var st = new SeclureCloudStorge();
            var r = new DACTaskResult();
            var s = NewSensor(ProtocolType.Voltage, SafetyFactor.DeformationCrack);
            s.FactorTypeTable = "T_THEMES_DEFORMATION_CRACK";
            ISensorData d = new VoltageData(0.2, 2.5)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s
            });

            s = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.Seepage);
            s.FactorTypeTable = "T_THEMES_ENVI_SEEPAGE";
             d = new SeepageData(23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s
            });

            s = NewSensor(ProtocolType.Pressure_MPM, SafetyFactor.StressStrainPoreWaterPressure);
            s.FactorTypeTable = "T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE";
             d = new PressureData(23, 23)
            {
                //AcqTime = DateTime.Now,
                //ResultCode = 0,
                //Sensor = s
            };
            r.AddSensorResult(new SensorAcqResult
            {
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s
            });
            r.Finished = DateTime.Now;
            r.Task=new DACTask()
            {
                DtuID = 2,
            };
            r.DtuCode = "20150202";
            st.ProcessResult(r);
        }

    }
}