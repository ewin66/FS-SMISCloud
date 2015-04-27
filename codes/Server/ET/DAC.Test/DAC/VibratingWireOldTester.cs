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
    public class VibratingWireOldTester
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
            ISensorAdapter sa = new VibratingWire_OLD_SensorAdapter();
            // fe 54 46 4c 13 15 04 00 00 00 03 01 00 00 00 00 00 a0 ef
            // fe 54 46 4c 12 13 15 00 00 00 03 00 00 00 00 00 00 b7 ef
            int err;
            Sensor s = new Sensor()
            {
                ModuleNo = 5380,
                ChannelNo = 1
            };
            var r = GetSensorAcqResult();
            r.Sensor = s;
             sa.Request(ref r);
            byte[] buff = r.Request;
            Assert.AreEqual("fe 54 46 4c 13 15 04 00 00 00 03 01 00 00 00 00 00 a0 ef", ValueHelper.BytesToHexStr(buff));

            Sensor s2 = new Sensor()
            {
                ModuleNo = 5135,
                ChannelNo = 1
            };
            r.Sensor = s2;
             sa.Request(ref r);
            byte[] buff2 = r.Request;
            Assert.AreEqual("fe 54 46 4c 13 14 0f 00 00 00 03 01 00 00 00 00 00 aa ef", ValueHelper.BytesToHexStr(buff2));
        }

        [Test]
        public void TestParseResponse()
        {
            ISensorAdapter ci = new VibratingWire_OLD_SensorAdapter();
            string bs = "fe 54 46 4c 1c 00 00 15 04 01 03 01 a8 77 49 45 00 00 00 00 00 00 00 00 00 00 7d ef";
            // fe 54 46 4c 1c 00 00 0f bb 01 03 01 a8 77 49 45 00 00 00 00 00 00 00 00 00 00 D8 ef
            byte[] buff = ValueHelper.StrToToHexByte(bs);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
               { 
                    ModuleNo = 5380,
                    ChannelNo = 1,
                    TableColums = "Frequency"
                }
            };
            ci.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(3223.47852f, data.RawValues[0]);

            bs = "FE 54 46 4C 1C 00 00 14 0F 01 03 01 4E 33 A8 44 00 00 CE 41 00 00 00 00 00 00 BA EF";
            r = new SensorAcqResult
            {
                Response = ValueHelper.StrToToHexByte(bs),
                Sensor = new Sensor()
                {
                    ModuleNo = 5135,
                    ChannelNo = 1,
                    TableColums = "Frequency"
                }
            };
            ci.ParseResult(ref r);
            data =  r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(1345.60327f , data.RawValues[0]);
        }
    }
}
