using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Accessor.MSSQL;
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
    class FTM50SSmallLaserTester
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
            var sa = new FTM50SSmallLaserSensorAdapter();
            var r = GetSensorAcqResult();

            var s = new Sensor
            {
                ModuleNo = 2
            };
            r.Sensor = s;
            sa.Request(ref r);
            var bts = r.Request;
            Assert.AreEqual(0xcc, bts[0]);
            Assert.AreEqual(0x02, bts[1]);
        }
        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
        [Test]
        public void TestParseResult()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));// 配置
            // X U 0 0 3 0 0 2
            var bts = ValueHelper.StrToToHexByte("32 55 30 30 33 30 30 32");
            var s = new Sensor()
            {
                ModuleNo = 2
            };
            s.AddParameter(new SensorParam(new FormulaParam())
            {
                Value = 0.002
            });
            var res = new SensorAcqResult
            {
                Response = bts,
                Sensor = s
            };

            var sa = new FTM50SSmallLaserSensorAdapter();
            sa.ParseResult(ref res);
            var data = res.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(3.002, data.RawValues[0], 0.0000001);
            Assert.AreEqual(3000.000, data.PhyValues[0], 0.0000001);
            bts = ValueHelper.StrToToHexByte("32 55 30 46 33 59 30 32");
            Trace.WriteLine(res.Data.JsonResultData);
            res.Response = bts;
            try
            {
                sa.ParseResult(ref res);
            }
            catch
            {

            }

            Assert.AreEqual((int)Errors.ERR_DATA_PARSEFAILED, res.ErrorCode);
            Trace.WriteLine(res.Data.JsonResultData);
            bts = ValueHelper.StrToToHexByte("32 56 30 46 33 59 30 32");
            res.Response = bts;
            sa.ParseResult(ref res);
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA, res.ErrorCode);
            Trace.WriteLine(res.Data.JsonResultData);
        }
    }
}
