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
    public class LVDT_XW_Tester
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
        public void TestRequestLVDT_XW_()
        {
            string str = "01 04 00 04 00 02 30 0A";
            ISensorAdapter sa = new LVDT_XW_SensorAdapter();
            Sensor s=new Sensor()
                         {
                             ModuleNo = 1
                         };
            int err;
            var r = GetSensorAcqResult();
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
        public void TestParseLVDT_XW_Response()
        {
            string str = "01 04 04 80 01 01 48 82 22";
            ISensorAdapter sa = new LVDT_XW_SensorAdapter();

            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 1,
                    ChannelNo = 1,
                    TableColums="ElongationIndicator"
                }
            };

            sa.ParseResult(ref r);
            var data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(-1.328, data.RawValues[0]);

        }
    }
}
