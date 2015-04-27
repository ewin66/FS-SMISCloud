#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="VoltageTester.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：lonwin lonwin ling20140917
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

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
    public class VoltageTester   //??
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
            string str = "fe 46 41 53 13 1b 76 00 00 00 01 41 0a 00 00 00 00 9e ef";
            ISensorAdapter sa = new VoltageSensorAdapter();
            Sensor s = new Sensor()
            {
                ModuleNo = 7030,
                ChannelNo = 10,
                ProductCode = "FS-LF10"
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
        public void TestRequestLvdTnew()
        {
            byte[] expected = ValueHelper.StrToToHexByte("fe 46 41 53 13 03 e8 00 00 00 01 48 01 00 00 00 00 1a ef");

            ISensorAdapter sa=new VoltageSensorAdapter();
            var s = new Sensor()
            {
                ModuleNo = 1000,
                ChannelNo = 1,
                ProductCode = "FS-LFV-VM5P5"
            };
            int err;
            var r = GetSensorAcqResult();
            r.Sensor = s;
            sa.Request(ref r);
            var actual = r.Request;

            if (actual.Length == expected.Length)
            {
                bool result = false;
                for (int i = 0; i < expected.Length; i++)
                {
                    result = actual[i] == expected[i];
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
            // 25 长度.
            string str = "fe 46 41 53 17 00 00 1b 76 01 01 0a 07 b4 00 00 00 00 00 00 00 00 19 69 ef";
 
            ISensorAdapter sa = new VoltageSensorAdapter();

            byte[] buff = ValueHelper.StrToToHexByte(str);
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 7030,
                    ChannelNo = 10,
                    ProductCode = "FS-LF10"
                }
            };

            sa.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("2.41", data.RawValues[1].ToString("f2"));

            // 24长度
            str = "fe 46 41 53 17 00 00 1b 76 01 01 0a 07 b4 00 00 00 00 00 00 00 00 69 ef";
            r.Response = ValueHelper.StrToToHexByte(str);
            sa.ParseResult(ref r);
            data = r.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual("2.41", data.RawValues[1].ToString("f2"));
        }

        [Test]
        public void TestParseResponseLvdtnew()
        {
            ISensorAdapter sa = new VoltageSensorAdapter();
            var sensor = new Sensor()
            {
                ModuleNo = 1010,
                ChannelNo = 10,
                ProductCode = "FS-LFV-V0P10",
                FormulaID = 16
            };
            sensor.AddParameter(new SensorParam(new FormulaParam())
            {
                Value = 1.5
            });
            sensor.AddParameter(new SensorParam(new FormulaParam())
            {
                Value = 1.5
            });
            sensor.AddParameter(new SensorParam(new FormulaParam())
            {
                Value = 2.0
            });
            var res = new SensorAcqResult()
            {
                Response = ValueHelper.StrToToHexByte("fe 46 41 53 17 00 00 03 f2 01 01 0a 08 00 00 00 00 00 00 00 00 00 4e ef"),
                Sensor = sensor
            };
            sa.ParseResult(ref res);
            var data =  res.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(5.00, data.RawValues[1], 0.00000001);
            Assert.AreEqual(6.00, data.PhyValues[0], 0.00000001);
        }
    }
}