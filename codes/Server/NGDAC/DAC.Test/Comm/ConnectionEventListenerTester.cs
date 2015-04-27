namespace NGDAC.Test.Comm
{
    using System;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Gprs;
    using FS.SMIS_Cloud.NGDAC.Node;

    using NUnit.Framework;

    [TestFixture]
    public class ConnectionEventListenerTester
    {
        private bool connected = false;

        [Test]
        [Category("MANUAL")]
        public void TestOnDtuConnected()
        {
            GprsDtuServer _server = new GprsDtuServer(5055);
            _server.Start();
            _server.OnConnectStatusChanged += this.OnConnectionStatusChanged;
            Thread.Sleep(6000);

            Thread t = new Thread(this.DoWork);
            t.Start();
            t.Join();
            Assert.IsTrue(this.connected);
        }

        private void DoWork(object ob)
        {
            while (!this.connected)
            {
                Thread.Sleep(100);                
            }
        }

        public void OnConnectionStatusChanged(IDtuConnection c, WorkingStatus oldStatus, WorkingStatus newStatus)
        {
            Console.Write("DtuConnection status changed.");
            this.connected = c.IsOnline;
        }
    }
}
