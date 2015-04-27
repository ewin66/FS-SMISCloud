namespace NGDAC.Test.Receiver
{
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Gprs;
    using FS.SMIS_Cloud.NGDAC.Tran;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    using NUnit.Framework;

    [TestFixture]
    public class DacReceiverTester
    {
        private static ILog log = LogManager.GetLogger("TestReceiver");

        [Test]
        [Category("MANUAL")]
        [Timeout(10000)] // 10s
        public void TestReceiver()
        {
            DtuClient c = new DtuClient("127.0.0.1", 6066);

            TranDataReceiver r = new TranDataReceiver(new GprsDtuServer(6066));
            r.Start();

            c.Connect(20120049, "18900000000", "192.168.1.222");
            c.OnReceived = (buff, len) =>
            {
                Thread.Sleep(500);
                var msg =
                    @"FA 2A 0C 00 01 74 01 FF 00 00 02 63 00 39 30 38 56 7A 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 38 56 7A 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 B8 70 80 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 B8 70 80 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 60 BA 01 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 60 BA 01 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 00 28 1C 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 00 28 1C 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 30 13 B0 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 30 13 B0 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 C0 E3 30 2A 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 C0 E3 30 2A 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 16 AF";
                log.DebugFormat("[Client] Sending data...");
                c.Send(ValueHelper.StrToToHexByte(msg));
            };

            // heartbeat, 激励
            c.Send(new HeartBeatTranMsg(20120049, 0).Marshall());

            int receivedCnt = 0;
            r.OnTranMsgReceived += (TranMsgType type, TranMsg msg) =>
            {
                TranMsg m2 = msg;
                log.DebugFormat("[Server] Msg Received: id={3}, type={0},len={1}, pkg={2}", type, msg.LoadSize,
                    msg.PackageCount, msg.ID);
                if (msg.ID == 0)
                {
                    Assert.AreEqual(TranMsgType.HeartBeat, type);
                }
                else
                {
                    Assert.AreEqual(TranMsgType.Dac, type);
                }
                receivedCnt++;
            };
            while (receivedCnt <= 5)
            {
                Thread.Sleep(10);
            }
            c.Close();
            log.DebugFormat("Test Done.");
            Assert.IsTrue(true);
        }
    }
}
