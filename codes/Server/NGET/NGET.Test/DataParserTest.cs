using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGET.Test
{
    using FS.SMIS_Cloud.NGET;
    using FS.SMIS_Cloud.NGET.DataParser;

    using NUnit.Framework;

    [TestFixture]
    class DataParserTest
    {
        [Test]
        public void TestJosnParser()
        {
            GlobalConfig.ConnectionString = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";

            string json = "{" + "S: 1265," + "R: 0," + "N: 0," + "T: 60000," + "A: \"ssssssss\","
                          + "RV: [ 0.1, 0.1, 0.1]," + "PV: [ 0.2, 0.2, 0.2]" + "}";

            var parser = new JsonParser();
            var rslt =parser.Parse(json);

            Assert.AreEqual(1265, rslt.Sensor.SensorID);
            Assert.AreEqual(0, rslt.Sensor.Parameters[0].Value);
            Assert.AreEqual(true, rslt.IsOK);
            Assert.AreEqual(0, rslt.AcqNum);
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 1, 0), rslt.AcqTime);
            Assert.AreEqual("ssssssss", rslt.Response);
            Assert.AreEqual(new double[] { 0.1, 0.1, 0.1 }, rslt.Data.RawValues);
            Assert.AreEqual(new double[] { 0.2, 0.2, 0.2 }, rslt.Data.PhyValues);
            //Assert.AreEqual(new double[] { 0.1, 0.1, 0.1 }, rslt.Data.CollectPhyValues);
            //Assert.AreEqual(new double?[] { 0.2, 0.2, 0.2 }, rslt.Data.ThemeValues);
            Assert.AreEqual(true, rslt.Data.IsSaveDataOriginal);
        }
    }
}
