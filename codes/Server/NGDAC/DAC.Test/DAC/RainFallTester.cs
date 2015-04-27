namespace NGDAC.Test.DAC
{
    using System;

    using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class RainFallTester
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
            string str = "55 0A 00 0E 06 17 0F 01 00 4E";
            ISensorAdapter sa = new RainFallSensorAdapter();
            Sensor s = new Sensor
            {
                ModuleNo = 4,
                ChannelNo = 1
            };
            byte[] expected = ValueHelper.StrToToHexByte(str);
            int err;
            var r = this.GetSensorAcqResult();
            r.Sensor = s;
            sa.Request(ref r);
            byte[] buff = r.Request;
            if (buff.Length == expected.Length)
            {
                Assert.AreEqual(buff[0] , 0x55);
                Assert.AreEqual(buff[1],0x0a);
                Assert.AreEqual(buff[2],0x00);
                Assert.AreEqual(buff[3],DateTime.Now.Year-2000);
                Assert.AreEqual(buff[4],DateTime.Now.Month);
                Assert.AreEqual(buff[5],DateTime.Now.Day);
               // Assert.AreEqual(buff[6],DateTime.Now.Hour);
                Assert.AreEqual(buff[7],1);
                Assert.AreEqual(buff[8],0);
            }
            else
            {
                Assert.Fail();
            }
        }



        [Test]
        public void TestParseResponse()
        {
            string str = "AA 0D 00 0E 06 0C 0F 00 00 FF FF 00 AC";
            ISensorAdapter sa = new RainFallSensorAdapter();

            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 1,
                    ChannelNo = 1,
                    TableColums = "Rainfall"
                }
            };

            sa.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("0.0", data.RawValues[0].ToString("f1"));
        }
    }
}
