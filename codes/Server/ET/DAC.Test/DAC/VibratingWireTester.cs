using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;

namespace DAC.Test.DAC
{
    [TestFixture]
    public class VibratingWireTester
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
        public void TestRequest()
        {
            ISensorAdapter sa = new VibratingWireSensorAdapter();
            Sensor s = new Sensor()
            {
                ModuleNo = 5864,
                ChannelNo = 1
            };
            int err;
            var r = GetSensorAcqResult();
            r.Sensor = s;
            sa.Request(ref r);
            byte[] buff = r.Request;
            Assert.AreEqual("00 0a 16 e8 01 01 67 73 76", ValueHelper.BytesToHexStr(buff));

            Sensor s2 = new Sensor()
            {
                ModuleNo = 5135,
                ChannelNo = 1
            };
            r.Sensor = s2;
            sa.Request(ref r);
            buff = r.Request;
            Assert.AreEqual("00 0a 14 0f 01 01 67 07 39", ValueHelper.BytesToHexStr(buff));
        }


        [Test]
        public void TestParseResponse()
        {
            // 00 0a 0f a6 81 09 6f 00 44 e5 9d 5d 43 ba 10 47 41 cf 00 00 05 00 00 ec f1 f3
            ISensorAdapter sa = new VibratingWireSensorAdapter();
            string str = "00 0A 0F A6 81 09 6F 00 44 E5 9D 5D 43 BA 10 47 41 CF 00 00  96 59";

            //string str = "00 0a 16 e8 81 01 67 00 44 86 07 ea 45 07 ff 96 41 b0 dc 5d 16 b1";
            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor
                {
                    ModuleNo = 4006,//?
                    ChannelNo = 9,
                    TableColums = "Frequency"
                }
            };

            sa.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(1836.9176f, data.RawValues[0]);
            Assert.AreEqual("25.875", data.RawValues[1].ToString());

            str = "00 0A 0F A6 81 09 6F 01 44 E5 9D 5D 43 BA 10 47 41 CF 00 00 17 5B";
            r.Response = ValueHelper.StrToToHexByte(str);
            sa.ParseResult(ref r);
            Assert.AreEqual(140101, r.ErrorCode);

            str = "00 0A 0F A6 81 09 6F 10 44 E5 9D 5D 43 BA 10 47 41 CF 00 00 06 67";
            r.Response = ValueHelper.StrToToHexByte(str);
            sa.ParseResult(ref r);
            Assert.AreEqual(140102, r.ErrorCode);

            str = "00 0A 0F A6 C0 16 E3 AA";
            r.Response = ValueHelper.StrToToHexByte(str);
            sa.ParseResult(ref r);
            Assert.AreEqual(100122, r.ErrorCode);
        }
    }
}