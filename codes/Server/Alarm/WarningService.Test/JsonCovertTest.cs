using System;
using FreeSun.FS_SMISCloud.Server.Common.Messages;
using FS.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace WarningService.Test
{
    [TestFixture]
    public class JsonCovertTest
    {
        [Test]
        public void TestJsonTORequestWarningReceivedMessage()
        {
            var time = DateTime.Now;
            var msg = new RequestWarningReceivedMessage
            {
                Id = Guid.NewGuid(),
                WarningTypeId = "105",
                StructId = 20,
                DeviceTypeId = 2,
                DeviceId = 150,
                WarningContent = "传感器设备产生告警",
                WarningTime = time,
                DateTime = time
            };
            var fsmsg =new FsMessage {Body = JObject.FromObject(msg)};
            var msg2 = JsonConvert.DeserializeObject<RequestWarningReceivedMessage>(fsmsg.Body.ToString());

            Assert.AreEqual(msg.Id,msg2.Id);
            Assert.AreEqual(msg.WarningTypeId, msg2.WarningTypeId);
            Assert.AreEqual(msg.StructId, msg2.StructId);
            Assert.AreEqual(msg.DeviceTypeId, msg2.DeviceTypeId);
            Assert.AreEqual(msg.DeviceId, msg2.DeviceId);
            Assert.AreEqual(msg.WarningContent, msg2.WarningContent);
            Assert.AreEqual(msg.WarningTime, msg2.WarningTime);
            Assert.AreEqual(msg.DateTime, msg2.DateTime);
        }
    }
}
