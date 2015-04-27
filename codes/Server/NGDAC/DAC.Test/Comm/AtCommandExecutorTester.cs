namespace NGDAC.Test.Comm
{
    using System;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Gprs;
    using FS.SMIS_Cloud.NGDAC.Gprs.Cmd;
    using FS.SMIS_Cloud.NGDAC.Model;

    using NUnit.Framework;

    [TestFixture]
    public class AtCommandExecutorTester
    {
        DtuClient client = new DtuClient("127.0.0.1",5055);

        [Test]
        [Category("MANUAL")]
        public void TestAtCommandExecutor()
        {
            GprsDtuServer _server = new GprsDtuServer(5055);
            _server.Start();
            string dtu = "12345678";
            Thread.Sleep(500);
            if (this.client.Connect(12345678, "22222222", "127.0.0.2"))
            {
                Console.WriteLine("Connected");
            }
            else {
                Console.WriteLine("ERROR");
            }
            this.client.OnReceived = (msg, len) => this.client.Send("OK");
            Thread.Sleep(500);
            GprsDtuConnection conn = (GprsDtuConnection) _server.GetConnection(new DtuNode{DtuCode=dtu});
            ATCommand cmd = new EnterConfig();
            CommandExecutor ce = new CommandExecutor();
            Assert.IsNotNull(conn);

            if (conn != null && conn.IsAvaliable())
            {
                ATCommandResult r = ce.Execute(conn, cmd);
                Assert.IsTrue(r.IsOK);
            }

            if (conn != null && conn.IsAvaliable())
            {
                cmd = new SetPort("{'port':5008}");
                ATCommandResult r = ce.Execute(conn, cmd);
                Assert.IsTrue(r.IsOK);
            }

            if (conn != null && conn.IsAvaliable())
            {
                cmd = new SetPort("{'port':5009");
                ATCommandResult r = ce.Execute(conn, cmd);
                Assert.IsTrue(r.IsOK);
            }

        }

    }
}
