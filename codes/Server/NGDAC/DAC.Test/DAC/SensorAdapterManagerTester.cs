﻿namespace NGDAC.Test.DAC
{
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

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
            var r = this.GetSensorAcqResult();
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
