using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    class TempHumidityModbusTester
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
            ISensorAdapter sa = new TempHumidityModbusSensorAdapter();

            var s = new Sensor()
            {
                ModuleNo=2014
            };

            byte[] expected = ValueHelper.StrToToHexByte("00 01 07 de 01 00 05 5d");
            int err;
            var r=GetSensorAcqResult();
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
            ISensorAdapter sa = new TempHumidityModbusSensorAdapter();

            byte[] data = ValueHelper.StrToToHexByte("00 01 07 DE 81 12 12 12 13 13 13 13 E1 F9");
            var res=new SensorAcqResult()
            {
                Sensor = new Sensor()
                {
                    ModuleNo = 2014
                },
                Response = data
            };
            sa.ParseResult(ref res);
            var d = res.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(4.6091755059538789E-28, d.RawValues[0], 0.00000001);
            Assert.AreEqual(1.8563668955399244E-27, d.RawValues[1], 0.00000001);
        }
    }
}
