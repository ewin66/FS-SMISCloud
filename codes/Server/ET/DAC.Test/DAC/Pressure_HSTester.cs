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
    public class Pressure_HSTester
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
        public void TestRequestPressure_HS()
        {
            string str = "23 35 4F 43 3B";
            ISensorAdapter sa = new Pressure_HS_SensorAdaper();
            Sensor s = new Sensor()
            {
                ModuleNo = 5,
                ChannelNo = 1
            };
            byte[] expected = ValueHelper.StrToToHexByte(str);
            int err;
            var r= GetSensorAcqResult();
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
        public void TestParsePressure_HSResponse()
        {
            string str = "2A 2B 30 30 2E 30 34 34 32 0D";
            ISensorAdapter sa = new Pressure_HS_SensorAdaper();

            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 5,
                    ChannelNo = 1,
                    TableColums = "Pressure"
                }
            };

            sa.ParseResult(ref r);
            var data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("0.0442", data.RawValues[0].ToString("f4"));
        }

    }
}
