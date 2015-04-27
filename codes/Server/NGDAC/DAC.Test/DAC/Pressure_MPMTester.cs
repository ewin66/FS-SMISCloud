namespace NGDAC.Test.DAC
{
    using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class Pressure_MPMTester
    {
        [Test]
        public void TestRequestPressure_MPM()
        {
            //  $  0  2 R P 0 
            //  24 30 32 52 50 30 33 30 0d
            string str = "24 32 33 52 50 31 33 32 0d";
            ISensorAdapter sa = new Pressure_MPM_SensorAdapter();
            Sensor s = new Sensor()
            {
                ModuleNo = 23,
                ChannelNo = 1
            };
            byte[] expected = ValueHelper.StrToToHexByte(str);
            int err;
            var r= this.GetSensorAcqResult();
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
        public void TestParsePressure_MPMResponse()
        {
            string str = "2a 31 37 2b 33 2e 35 31 36 39 33 42 0d";
            ISensorAdapter sa = new Pressure_MPM_SensorAdapter();

            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 17,
                    ChannelNo = 1,
                    TableColums = "Preasure"
                }
            };

            sa.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("3.5169", data.RawValues[0].ToString("f4"));
        }


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
    }
}
