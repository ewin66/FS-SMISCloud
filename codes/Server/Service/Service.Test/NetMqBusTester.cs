using System;

namespace FS.Service.Test
{
    using FS.Service;
    using System.Threading;
    using System.IO;

    using NUnit.Framework;

    [TestFixture]
    public class NetMqBusTester
    {
        private static string xmlPath = System.Environment.CurrentDirectory;
        //static void Main(string[] args)
        //{
        //    var tester = new NetMqBusTester();
        //    tester.TestReqResp();
        //    tester.TestPushPull();
        //    tester.TestPubSub();
        //    Console.ReadLine();
        //}

        [Test]
        public void TestOpenClose()
        {
            NetMQMessageBus busAlarm = new NetMQMessageBus();
            NetMQMessageBus busEt = new NetMQMessageBus();
            uint loopSize = 10;
            uint sendloop = 1;
            
            Console.WriteLine("Testing start/stop benchmark..., {0} times", loopSize);
            for (uint i = 0; i < loopSize; i++)
            {
                busAlarm.Start(Path.Combine(xmlPath, "AlarmService.xml"));
                busEt.Start(Path.Combine(xmlPath, "EtService.xml"));
                MsgHandler h2 = new MsgHandler { Received = 0, Name = "H2(ET)" };
                busEt.Pull(h2.OnMessageReceived);
                new Thread(new MsgPusher { Name = "et", bus = busAlarm, LoopSize = sendloop }.DoWork).Start();

                while (h2.Received != sendloop)
                {
                    Thread.Sleep(10);
                }
                busAlarm.Stop();
                busEt.Stop();
            }
            Console.WriteLine("Done!");
        }

        [Test]
        public void TestPushPull()
        {
            uint loopSize = 10000;

            Console.WriteLine("Testing pull/push benchmark..., {0} times", loopSize);
            NetMQMessageBus busAlarm = new NetMQMessageBus();
            NetMQMessageBus busEt = new NetMQMessageBus();
            busAlarm.Start(Path.Combine(xmlPath, "AlarmService.xml"));
            busEt.Start(Path.Combine(xmlPath, "EtService.xml"));
            MsgHandler h2 = new MsgHandler { Received = 0, Name = "H2(ET)" };

            long start = DateTime.Now.Ticks;
            busEt.Pull( h2.OnMessageReceived);
            new Thread(new MsgPusher { Name = "et", bus = busAlarm, LoopSize = loopSize }.DoWork).Start();

            while (h2.Received != loopSize)
            {
                Thread.Sleep(10);
            }
            double elapsed = (DateTime.Now.Ticks - start) / 10000000.0; //s
            double speed = loopSize / elapsed;
            // 15479
            Console.WriteLine("DONE, loops = {0},Speed = {1:#0.00} msg/s", h2.Received, speed);
            busAlarm.Stop();
            busEt.Stop();
        }

        [Test]
        public void TestPubSub()
        {
            uint loopSize = 100;

            Console.WriteLine("Testing PubSub benchmark..., {0} times", loopSize);
            NetMQMessageBus busAlarm = new NetMQMessageBus();
            NetMQMessageBus busEt = new NetMQMessageBus();
            busAlarm.Start(Path.Combine(xmlPath, "AlarmService.xml"));
            busEt.Start(Path.Combine(xmlPath, "EtService.xml"));
            MsgHandler handler = new MsgHandler { Received = 0, Name = "H2(ET)" };

            long start = DateTime.Now.Ticks;
            busEt.Subscriber("alarm", "你好", handler.OnMessageReceived);
            // alarm publish to et
            new Thread(new MsgPublisher { bus = busAlarm, Topic = "你好", LoopSize = loopSize }.DoWork).Start();
            while (handler.Received != loopSize)
            {
                Thread.Sleep(10);
            }
            double elapsed = (DateTime.Now.Ticks - start) / 10000000.0; //s
            double speed = loopSize / elapsed;
            // 15479
            Console.WriteLine("DONE, loops = {0},Speed = {1:#0.00} msg/s", handler.Received, speed);
            busAlarm.Stop();
            busEt.Stop();
        }

        [Test]
        public void TestReqResp()
        {
            uint loopSize = 100;

            Console.WriteLine("Testing Req/Resp benchmark..., {0} times", loopSize);
            NetMQMessageBus busAlarm = new NetMQMessageBus();
            NetMQMessageBus busEt = new NetMQMessageBus();
            busAlarm.Start(Path.Combine(xmlPath, "AlarmService.xml"));
            busEt.Start(Path.Combine(xmlPath, "EtService.xml"));
            MsgResponser h2 = new MsgResponser { Received = 0, Name = "H2(ET)" };

            long start = DateTime.Now.Ticks;
            busEt.Response(h2.OnMessageReceived);
            string resp = null;
            for (int i = 0; i < loopSize; i++)
            {
                resp = busAlarm.Request("et", newMessage(string.Format("ABC {0}", i)).ToJson(), 2000);
            }
            double elapsed = (DateTime.Now.Ticks - start) / 10000000.0; //s
            double speed = loopSize / elapsed;
            // 15479
            Console.WriteLine("DONE, loops = {0},Speed = {1:#0.00} msg/s", h2.Received, speed);
            busAlarm.Stop();
            busEt.Stop();
        }

        class MsgPusher
        {
            public NetMQMessageBus bus;
            public String Name;

            public uint LoopSize;

            public void DoWork()
            {
                int index = 0;
                //while (true)
                while (index < LoopSize)
                {
                //FsMessage msg = newMessage(string.Format("T1 msg {0},To Somone, from alarm.", index));

                    FsMessage msg = newMessage(string.Format("你好 {0},To Somone, from alarm.", index));
                    bus.Push(Name, msg.ToJson(),100);
                    index++;
                }
            }
        }

        class MsgPublisher
        {
            public NetMQMessageBus bus;
            public string Topic;
            public uint LoopSize;
            public void DoWork()
            {
                int index = 0;
                //while (true)
                while (index < LoopSize)
                {
                    //FsMessage msg = newMessage(string.Format("T1 msg {0},To Somone, from alarm.", index));
                    FsMessage msg = newMessage(string.Format("你好 {0},To Somone, from alarm.", index));
                    bus.Publish(Topic, msg.ToJson());
                    index++;
                }
            }
        }

        class MsgResponser
        {
            public uint Received { get; set; }
            public string Name { get; set; }
            public string OnMessageReceived(string msg)
            {
                Received++;
                // System.Console.WriteLine("[{0}]: Msg received: {1}", Name, msg.Body);
                // msg.Body  = msg.Body  + "(RESP)";
                // FsMessage resp = msg!=null? FsMessage.FromJson(msg):null;
                return msg;
            }
        }

        class MsgHandler
        {
            public uint Received  { get; set; }
            public string Name{get;set;}
            public void OnMessageReceived(string msg)
            {
                 Received++;
                System.Console.WriteLine("[{0}]: Msg received: {1}", Name, msg );
            }
        }

        class SubMsgHandler
        {
            public uint Received { get; set; }
            public string Name { get; set; }
            public void OnMessageReceived(string topic, string msg)
            {
                Received++;
                System.Console.WriteLine("[{0}]: Msg received: {1} - {2}", topic, Name, msg );
            }
        }

        static FsMessage newMessage(string body)
        {
            Guid token = Guid.NewGuid();
            Guid msgId = Guid.NewGuid();
            FsMessage msg = new FsMessage();
            msg.Header = new FsMessageHeader();
            msg.Header.A = "GET";
            msg.Header.D = "LOG";
            msg.Header.L = 20;
            msg.Header.R = "log";
            msg.Header.S = "中文";
            msg.Header.T = token;
            msg.Header.U = msgId;
            msg.Body = body;
            return msg;
        }
    }
}
