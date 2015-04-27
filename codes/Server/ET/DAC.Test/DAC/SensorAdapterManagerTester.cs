using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;

namespace DAC.Test.DAC
{
    [TestFixture]
    class SensorAdapterManagerTester
    {

        [Test]
        public void TestGetAdapter()
        {
            IAdapterManager adapterManager = SensorAdapterManager.InitializeManager();

            Assert.IsNotNull(adapterManager);

            Assert.IsNull(SensorAdapterManager.InitializeManager("abcdedfg"));

            Assert.IsNotNull(adapterManager.GetSensorAdapter(ProtocolType.Voltage));

            Assert.IsNull(adapterManager.GetSensorAdapter(ProtocolType.TempHumidity_OLD));

            Assert.IsNull(adapterManager.GetSensorAdapter((uint)1234567));

            string str = "24 32 33 52 50 31 33 32 0d";
            Sensor s = new Sensor()
            {
                ModuleNo = 23,
                ChannelNo = 1,
                ProtocolType = (uint) ProtocolType.Pressure_MPM
            };
            int err;
            var r = GetSensorAcqResult();
            r.Sensor = s;
             adapterManager.GetSensorAdapter(s.ProtocolType).Request(ref r);
            byte[] req = r.Request;
            Assert.AreEqual(str, ValueHelper.BytesToHexStr(req));
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
