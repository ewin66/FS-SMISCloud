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

namespace Storage.iSecureCloud.Test
{
    using System;
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGET.Model;
    using FS.SMIS_Cloud.NGET.Storage.iSecureCloud;

    using NUnit.Framework;

    [TestFixture]
    public class MsDbAccessorTester
    {
        private const string Connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.1;uid=sa;pwd=861004;pooling=false";
        private MsDbAccessor msDbAccessor;
        private const string Xmlpath = ".\\ThemeTables_iSecureCloud.xml";
        [TestFixtureSetUp]
        public void SetUp()
        {
            this.msDbAccessor = new MsDbAccessor(Connstr);
            var loadxml = new LoadDbConfigXml(Xmlpath);
            this.msDbAccessor.UpdateTables(loadxml.GeTableInfos());
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

        private static Sensor NewSensor(int type)
        {
            return new Sensor
            {
                SensorID = 17,
                ProtocolType = 6,
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

        [Test]
        public void TestLogErrorData()
        {
            var r = new List<SensorAcqResult>();
            var s1 = NewSensor(17);
            s1.FactorTypeTable = "T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE";
            var d = new SensorData(new double[] { 23 }, new double[] { 23 }, new double[] { 23 });
            r.Add(new SensorAcqResult
            {
                AcqTime = DateTime.Now,
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s1
            });
            Assert.IsTrue(this.msDbAccessor.LogErrorData(r[0]));
        }


        [Test]
        public void TestSeclureCloudStorge()
        {
            var st = new SeclureCloudStorge();
            var r = new List<SensorAcqResult>();
            var s = NewSensor(28);
            s.FactorTypeTable = "T_THEMES_DEFORMATION_CRACK";
            SensorData d = new SensorData(new double[] { 0.2 }, new double[] { 2.5 }, new double[] { 2.5 });
            r.Add(new SensorAcqResult { AcqTime = DateTime.Now, ErrorCode = (int)Errors.SUCCESS, Data = d, Sensor = s });

            s = NewSensor(36);
            s.FactorTypeTable = "T_THEMES_ENVI_SEEPAGE";
            d = new SensorData(new double[] { 23 }, new double[] { 23 }, new double[] { 23 });
            r.Add(new SensorAcqResult { AcqTime = DateTime.Now, ErrorCode = (int)Errors.SUCCESS, Data = d, Sensor = s });

            s = NewSensor(12);
            s.FactorTypeTable = "T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE";
            d = new SensorData(new double[] { 23 }, new double[] { 23 }, new double[] { 23 });
            r.Add(new SensorAcqResult
            {
                AcqTime = DateTime.Now,
                ErrorCode = (int)Errors.SUCCESS,
                Data = d,
                Sensor = s
            });

            st.ProcessResult(r);
        }

    }
}