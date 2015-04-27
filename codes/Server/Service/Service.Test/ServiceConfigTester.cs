using System;

namespace FS.Service.Test
{
    using NUnit.Framework;

    [TestFixture]
    public class ServiceConfigTester
    {
        [Test]
        public void TestServiceConfig()
        {
            string alarmServiceFile = "./AlarmService.xml";
            ServiceConfig sc = ServiceConfig.FromXml(alarmServiceFile);
            Assert.AreEqual("value1", sc.GetProperty("key1"));
            Assert.AreEqual("tcp://*:9001", sc.PubSub.ListenAddress);
            Assert.AreEqual("tcp://*:9002", sc.PushPull.ListenAddress);
            Assert.AreEqual("tcp://*:9003", sc.ReqRep.ListenAddress);
        }

        [Test]
        public void TestServiceConfig2()
        {
            string alarmServiceFile = "./ETService.xml";
            ServiceConfig sc = ServiceConfig.FromXml(alarmServiceFile);
            Assert.AreEqual(null, sc.GetProperty("key1"));
        }
    }
}
