using System;
using System.Threading;
using FreeSun.FS_SMISCloud.Server.Common.Messages;
using FreeSun.FS_SMISCloud.Server.WarningManagementProcess.Communication;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SMIS.Utils.DB;

namespace WarningService.Test
{
    [TestFixture]
    public class WarningManagementSeverTest
    {
        private string connectionString =
            @"server=192.168.1.30;database=DW_iSecureCloud_Empty;User ID=sa;Password=Windows2008";

       RequestWarningReceivedMessage request=new RequestWarningReceivedMessage
       {
           Id = Guid.NewGuid(),
           WarningTypeId = "105",
           StructId = 20,
           DeviceTypeId = 2,
           DeviceId = 150,
           WarningContent = "传感器设备产生告警",
           WarningTime = DateTime.Now,
           DateTime = DateTime.Now
       };

       DTUStatusChangedMsg dtustateChangedMsg = new DTUStatusChangedMsg
       {
           Id = Guid.NewGuid(),
           WarningTypeId = "001",
           DeviceTypeId = 1,
           DTUID = "20120049",
           IsOnline = false,
           TimeStatusChanged = DateTime.Now,
           WarningContent = false ? "DTU上线" : "DTU下线",
           DateTime = DateTime.Now
       };


        [Test]
        public void TestSensorWarnDowork()
        {
            DbHelperSQL.connectionString = connectionString;
            WarningManagementSever server =new WarningManagementSever();
            int rowcount = server.MDSClient_MessageReceived(new WarnningMsg()
            {
                Sender = "test",
                R = "/warning/sensor",
                Msg = JObject.FromObject(request).ToString()
            });
           Assert.AreEqual(1,rowcount);
        }

        [Test]
        public void TestDtuWarnDowork()
        {
            DbHelperSQL.connectionString = connectionString;
            WarningManagementSever server = new WarningManagementSever();

            int rowcount = server.MDSClient_MessageReceived(new WarnningMsg()
            {
                Sender = "test",
                R = "/warning/dtu",
                Msg = JObject.FromObject(dtustateChangedMsg).ToString()
            });
            Assert.AreEqual(2, rowcount);
        }


    }
}