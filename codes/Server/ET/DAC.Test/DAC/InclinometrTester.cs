using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;
using FS.SMIS_Cloud.DAC.Accessor.MSSQL;

namespace DAC.Test.DAC
{
    [TestFixture]
    public class InclinometrTester
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
            ISensorAdapter ci = new Inclination_ROD_SensorAdapter();
            // 00 16 20 f0 01 42 a4
            // 8432
            Sensor s = new Sensor();
            s.ModuleNo = 8432;
            int err;
            var r = GetSensorAcqResult();
            r.Sensor = s;
            ci.Request(ref r);
            var buff = r.Request;
            Assert.AreEqual(7, buff.Length);
            Assert.AreEqual(buff[0], 0x00);
            Assert.AreEqual(buff[1], 0x16);
            Assert.AreEqual(buff[2], 0x20);
            Assert.AreEqual(buff[3], 0xf0);
            Assert.AreEqual(buff[4], 0x01);
            Assert.AreEqual(buff[5], 0x42);
            Assert.AreEqual(buff[6], 0xa4);

            ci = new Inclination_BOX_SensorAdapter();
            // 00 16 20 f0 01 42 a4
            // 8432
            s = new Sensor();
            s.ModuleNo = 8432;
            r.Sensor = s;
            ci.Request(ref r);
            buff = r.Request;
            Assert.AreEqual(7, buff.Length);
            Assert.AreEqual(buff[0], 0x00);
            Assert.AreEqual(buff[1], 0x15);
            Assert.AreEqual(buff[2], 0x20);
            Assert.AreEqual(buff[3], 0xf0);
            Assert.AreEqual(buff[4], 0x01);
            Assert.AreEqual(buff[5], 0x06);
            Assert.AreEqual(buff[6], 0xa4);
        }
        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
        [Test]
        public void TestParseInclinometrResponse()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));// 配置
            ISensorAdapter ci = new Inclination_ROD_SensorAdapter();
            string bs = "001626c68184def28aa5120d61ffdacfbeffc22929f831";
            byte[] buff = ValueHelper.ToBytes(bs);
            var r = new SensorAcqResult()
            {
                ErrorCode = (int)Errors.SUCCESS,
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 9926,
                    TableColums = "Angle_X,Angle_Y"
                }
            };
            ci.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(r.Data);
            Assert.AreEqual(-2.437186f, data.RawValues[0]);
            Assert.AreEqual(-4.052695f, data.RawValues[1]);
            
            ci = new Inclination_BOX_SensorAdapter();
            bs = "001626c68184def28aa5120d61ffdacfbeffc22929f831";
            buff = ValueHelper.ToBytes(bs);
            r.Response = buff;
            try
            {
                ci.ParseResult(ref r);
            }
            catch 
            {
                throw;
            }
           
            Assert.IsNotNull(data);
            r.Response = null;
            ci.ParseResult(ref r);
            Assert.IsNotNull(data);
            bs = "001626c68184def28aa5120d61ffdacfbeffc22929";
            buff = ValueHelper.ToBytes(bs);
            r.Response = buff;
            ci.ParseResult(ref r);
            Assert.IsNotNull(data);


            bs = "001526c68184def28aa5120d61ffdacfbeffc229290752";
            buff = ValueHelper.StrToToHexByte("00 15 22 28 81 8F EA 0D 71 0E 40 0F CB FF F5 08 77 FF F9 6B 7B 78 9E");
            r = new SensorAcqResult()
            {
                ErrorCode = (int)Errors.SUCCESS,
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 8744,
                    TableColums = "Angle_X,Angle_Y"
                }
            };
            ci.ParseResult(ref r);
            data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(-0.718729f, data.RawValues[0], 0.000001);
            Assert.AreEqual(-0.431237f, data.RawValues[1], 0.000001);

            bs = "00152228C01A6356";
            buff = ValueHelper.StrToToHexByte(bs);
            r.Response = buff;
            ci.ParseResult(ref r);
            Assert.AreEqual(100126,r.ErrorCode);

        }

        [Test]
        public void TestRequestOldInclinometr()
        {
            ISensorAdapter ci = new Inclination_OLD_SensorAdapter();
            // fe 43 58 20 cc 00 00 00 00 00 00 00 00 00 00 09 ef
            // 8432
            var s = new Sensor {ModuleNo = 8396};
            int err;
             var r = GetSensorAcqResult();
            r.Sensor = s;
             ci.Request(ref r);
            byte[] buff = r.Request;
            Assert.AreEqual(17, buff.Length);
            Assert.AreEqual(buff[0], 0xfe);
            Assert.AreEqual(buff[1], 0x43);
            Assert.AreEqual(buff[2], 0x58);
            Assert.AreEqual(buff[3], 0x20);
            Assert.AreEqual(buff[4], 0xcc);
            Assert.AreEqual(buff[5], 0x00);
            Assert.AreEqual(buff[6], 0x00);
            Assert.AreEqual(buff[7], 0x00);
            Assert.AreEqual(buff[8], 0x00);
            Assert.AreEqual(buff[9], 0x00);
            Assert.AreEqual(buff[10], 0x00);
            Assert.AreEqual(buff[11], 0x00);
            Assert.AreEqual(buff[12], 0x00);
            Assert.AreEqual(buff[13], 0x00);
            Assert.AreEqual(buff[14], 0x00);
            Assert.AreEqual(buff[15], 0x09);
            Assert.AreEqual(buff[16], 0xef);
        }

        [Test]
        public void TestParseOldInclinometrResponse()
        {
            ISensorAdapter ci = new Inclination_OLD_SensorAdapter();
            string bs = "fe 43 58 00 00 20 cc 0b 00 2f 1c 0d 00 2c 03 13 ef";
            byte[] buff = ValueHelper.StrToToHexByte(bs);
            var r = new SensorAcqResult()
            {
                Response = buff,
                Sensor = new Sensor
                {
                    ModuleNo = 8396,
                    TableColums = "Angle_X,Angle_Y"
                }
            };
            ci.ParseResult(ref r);
            var data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("0.7911", data.RawValues[0].ToString("0.####"));
            Assert.AreEqual("-0.7342", data.RawValues[1].ToString("0.####"));
        }       
    }
}
