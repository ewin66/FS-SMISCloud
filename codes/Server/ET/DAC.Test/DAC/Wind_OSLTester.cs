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
    public class Wind_OSLTester
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
            string str = "02 04 10 04 00 08 b4 fe";

            ISensorAdapter sa = new Wind_OSL_SensorAdapter();
            Sensor s = new Sensor()
                         {
                             ModuleNo = 2,
                             TableColums="AirSpeed,WindDirection,ElevationAngle"
                         };
            byte[] expected = ValueHelper.StrToToHexByte(str);
            int err;
            var r = GetSensorAcqResult();
            r.Sensor = s;
            sa.Request(ref r);
            byte[] buff = r.Request;
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
        public void TestParseResponse()
        {
            string str = "02 04 10 a5 25 3d b9 09 43 43 86 00 00 41 90 c7 b0 c0 b7 cd 90";
            ISensorAdapter sa = new Wind_OSL_SensorAdapter();

            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 2,
                    ChannelNo = 1,
                    TableColums="AirSpeed,WindDirection"
                }
            };

            sa.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("0.09", data.RawValues[0].ToString("f2"));
            Assert.AreEqual("268.07",data.RawValues[1].ToString("f2"));

        }
    }
}
