namespace NGDAC.Test.DAC
{
    using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class MagneticFluxTester
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
        public void TestRequestMagneticFlux()
        {
            string str = "fe cd 21 78 00 04 03 00 14 00 00 00 79 ef";
            ISensorAdapter sa = new MagneticFluxSensorAdapter();
            var s = new Sensor
            {
                ModuleNo = 8568, //?
                ChannelNo = 3
            };
            int err;
           var r = this.GetSensorAcqResult();
            r.Sensor = s;
            sa.Request(ref r);
            byte[] buff = r.Request;
            byte[] expected = ValueHelper.StrToToHexByte(str);
            if (buff.Length == expected.Length)
            {
                bool result = false;
                for (int i = 0; i < expected.Length; i++)
                {
                    result = buff[i] == expected[i];
                    if (!result)
                        break;
                }
                Assert.IsTrue(result);
            }
            else
            {
                Assert.Fail();
            }
        }



        [Test]
        public void TestParseMagneticFluxResponse()
        {
            string str = "FE CD 00 0A 01 04 00 0B 9B 89 22 41 7B 14 A0 41 00 00 00 00 00 00 C8 EF";
            ISensorAdapter sa = new MagneticFluxSensorAdapter();
            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 10,
                    ChannelNo = 1,
                    TableColums = "Voltage,Temperature"
                }
            };

            sa.ParseResult(ref r);
            var data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("10.1586", data.RawValues[0].ToString("f4"));
            Assert.AreEqual("20.01", data.RawValues[1].ToString("f2"));
        }
    }
}
