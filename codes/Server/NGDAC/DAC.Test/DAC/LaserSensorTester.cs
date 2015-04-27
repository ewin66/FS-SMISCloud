#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="LaserSensorTester.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141111 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace NGDAC.Test.DAC
{
    using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.Accessor;
    using FS.SMIS_Cloud.NGDAC.Accessor.MSSQL;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class LaserSensorTester
    {
        private SensorAcqResult GetSensorAcqResult()
        {
            var r = new SensorAcqResult
            {
                Request = null,
                Response = null,
                Data = null,
                ErrorCode = (int)Errors.ERR_DEFAULT
            };
            return r;
        }

        [Test]
        public void TestRequestInclinometr()
        {
            ISensorAdapter ci = new GLSB40I70Laser_SensorAdapter();
            // 64 06 02 94        100
            var s = new Sensor {ModuleNo = 100};
            int err;
            var r = this.GetSensorAcqResult();
            r.Sensor = s;
            ci.Request(ref r);
            byte[] buff = r.Request;
            Assert.AreEqual(4, buff.Length);
            Assert.AreEqual(0x64,buff[0]);
            Assert.AreEqual(0x06,buff[1]);
            Assert.AreEqual(0x02,buff[2]);
            Assert.AreEqual(0x94,buff[3]);
        }
        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
        [Test]
        public void TestParseInclinometrResponse()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));// 配置
            ISensorAdapter ci = new GLSB40I70Laser_SensorAdapter();
            string bs = "6406823030302e333131c1";
            byte[] buff = ValueHelper.ToBytes(bs);
            SensorAcqResult r = new SensorAcqResult()
            {
                Response = buff,
                Sensor = new Sensor
                {
                    ModuleNo = 100,
                    TableColums = "Angle_X,Angle_Y"
                }
            };
            r.Sensor.AddParameter(new SensorParam(new FormulaParam())
            {
               Value = 0.1
            });
            ci.ParseResult(ref r);
            var data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(0.311, data.RawValues[0]);
            Assert.AreEqual(0.211, data.PhyValues[0]);

            bs = "6406833030302e333131c1";
            buff = ValueHelper.ToBytes(bs);
            r.Response = buff;
            ci.ParseResult(ref r);
            Assert.IsNotNull(data);
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA,r.ErrorCode);
            bs = "6406823030302e33";
            buff = ValueHelper.ToBytes(bs);
            r.Response = buff;
            ci.ParseResult(ref r);
            Assert.IsNotNull(data);
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA, r.ErrorCode);
        }
    }
}