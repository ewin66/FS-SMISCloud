namespace NGDAC.Test.DAC
{
    using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.Accessor;
    using FS.SMIS_Cloud.NGDAC.Accessor.MSSQL;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class TempHumidityTester
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

        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
        [Test]
        public void TestParseTempHumidityResponse()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));// 配置
            TempHumiditySensorAdapter ci = new TempHumiditySensorAdapter();
            string bs = "FD0100230000"; //0-5
            bs += "01";         // 命令号 6=01
            bs += "000055";     // Hum: 7-9: 01 H L  = 85
            bs += "41CC0000";   // Temp 10-13,float, = 25.5
            bs += "000000000000000000000000000000005EDF";
            byte[] buff = ValueHelper.ToBytes(bs);
            byte crc = ValueHelper.CheckCRC8(buff, 1, 29);
            buff[30] = crc;
            SensorAcqResult r = new SensorAcqResult
            {
                Response = buff,
                Sensor = new Sensor()
                {
                    ModuleNo = 35,
                    ChannelNo = 1,
                    TableColums = "Humidity,Temperature"
                }
            };
            ci.ParseResult(ref r);
            var data =  r.Data;
            Assert.IsNotNull(data);
            
            Assert.AreEqual(25.50, data.RawValues[0]);
            Assert.AreEqual(0.85, data.RawValues[1]);
            bs = "Fc0100230000"; //0-5
            bs += "01";         // 命令号 6=01
            bs += "000055";     // Hum: 7-9: 01 H L  = 85
            bs += "41CC0000";   // Temp 10-13,float, = 25.5
            bs += "000000000000000000000000000000005EDF";
            buff = ValueHelper.ToBytes(bs);
            crc = ValueHelper.CheckCRC8(buff, 1, 29);
            buff[30] = crc;
            r.Response = buff;
            ci.ParseResult(ref r);
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA,r.ErrorCode);
        }

        [Test]
        public void TestRequestTempHumidity()
        {
            TempHumiditySensorAdapter ci = new TempHumiditySensorAdapter();
            Sensor s = new Sensor();
            s.ModuleNo = 0x23;
            int err;
            // fc 01 00 23 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 a4 cf 
            var r = this.GetSensorAcqResult();
            r.Sensor = s;
            ci.Request(ref r);
            byte[] buff = r.Request;
            Assert.AreEqual(32, buff.Length);
            Assert.AreEqual(0xFC, buff[0]); //模块号=设备号
            Assert.AreEqual(0x23, buff[3]);
            Assert.AreEqual(0xCF, buff[31]);
        }
    }
}
