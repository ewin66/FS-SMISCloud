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
    public class LVDT_BL_Tester
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
        public void TestRequestLVDT_BL_()
        {
            LVDT_BL_SensorAdapter ci = new LVDT_BL_SensorAdapter();
            Sensor s = new Sensor();
            var r = GetSensorAcqResult();
            r.Sensor = s;
            //17 03 00 00 00 01 86 fc
            s.ModuleNo = 0x17;
            r.Sensor = s;
            ci.Request(ref r);
            var buff = r.Request;
            Assert.AreEqual(8, buff.Length);
            Assert.AreEqual(0x17, buff[0]); //模块号=设备号
            Assert.AreEqual(0x03, buff[1]);
            Assert.AreEqual(0x01,buff[5]);
            Assert.AreEqual(0x86,buff[6]);
            Assert.AreEqual(0xfc,buff[7]);
        }


        [Test]
        public void TestParseLVDT_BL_Response()
        {
            LVDT_BL_SensorAdapter ci = new LVDT_BL_SensorAdapter();
            string bs = "170302fffd71f6"; 
            byte[] buff = ValueHelper.ToBytes(bs);

            SensorAcqResult r = new SensorAcqResult { Response = buff ,Sensor = (new Sensor()
                              {
                                  ModuleNo = 0x17,
                                  TableColums = "ElongationIndicator"
                              })};
           
            ci.ParseResult(ref r);
            var data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(-0.3, data.RawValues[0]);
            //Assert.AreEqual(25.50f, data.Temperature);
        }

    }
}
