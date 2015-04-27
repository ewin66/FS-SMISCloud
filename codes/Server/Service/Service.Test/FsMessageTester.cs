using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace FS.Service.Test
{
    using NUnit.Framework;

    [TestFixture]
    public class UnitTest_FsMessage
    {
        [Test]
        public void TestFsMessageToJson()
        {
            Guid token = Guid.NewGuid();
            Guid msgId = Guid.NewGuid();
            FsMessage msg = new FsMessage();
            msg.Header = new FsMessageHeader();
            msg.Header.A = "GET";
            msg.Header.D = "LOG";
            msg.Header.L = 20;
            msg.Header.R = "log/all";
            msg.Header.S = "中文";
            msg.Header.T = token;
            msg.Header.U = msgId;
            msg.Body  = new { name = "Hello", intv = 1, data = "2012-2-2", av = new[] { 1, 2, 3, 4.2 }, bv = true };

            string json = msg.ToJson();
            //  {"Body":{"Data":null},"Header":{"A":"GET","D":"LOG","L":20,"R":"log\/all","S":"Tester","T":"69623642-264c-4260-a7b4-bea916b8cf0c","U":"661f43ca-7d52-418a-8f46-12854a25561d"}}


            Console.WriteLine("json = {0}", json);
            Assert.IsNotNull(json);

            FsMessage msg2 = FsMessage.FromJson(json);
            Assert.AreEqual(msg.Header.T, msg2.Header.T);

            Assert.AreEqual(JsonConvert.SerializeObject(msg2.Body), JsonConvert.SerializeObject(msg.Body));

            JObject body = (JObject) msg2.Body;
            int iv1 = msg2.BodyValue<int>("intv");
            double[] sensors = msg2.BodyValues<double>("av");
            Assert.AreEqual(1, iv1);
            Assert.AreEqual(4, sensors.Length);
            Assert.AreEqual(4.2, sensors[3]);
            Assert.AreEqual(true, msg2.BodyValue<bool>("bv"));
            Assert.AreEqual("Hello", msg2.BodyValue<string>("name"));
        }

        [Test]
        public void TestFsMessageConvertTime()
        {
            Guid token = Guid.NewGuid();
            Guid msgId = Guid.NewGuid();
            FsMessage msg = new FsMessage();
            msg.Header = new FsMessageHeader();
            msg.Body = null;
            msg.Header.A = "GET";
            msg.Header.D = "LOG";
            msg.Header.L = 20;
            msg.Header.R = "log/all";
            msg.Header.S = "中文";
            msg.Header.T = token;
            msg.Header.U = msgId;

            long start = DateTime.Now.Ticks;
            int looptimes = 100000;
            string json="";
            for (int i=0;i<looptimes;i++) 
            {
                json = msg.ToJson();
            }
            double elapsed = (DateTime.Now.Ticks - start) / 10000.0; //s
            Console.WriteLine("FsMessage.Json : {0:#0.00} ms, speed = {1:#0.00} m/s", elapsed, looptimes / elapsed *1000);

            start = DateTime.Now.Ticks;
            
            for (int i = 0; i < looptimes; i++)
            {
                json = JsonConvert.SerializeObject(msg);
            }
            elapsed = (DateTime.Now.Ticks - start) / 10000.0; //s
            Console.WriteLine("JsonConvert:SerializableObject: {0:#0.00} ms , speed = {1:#0.00} m/s", elapsed, looptimes*1000 / elapsed);

            start = DateTime.Now.Ticks;
            for (int i = 0; i < looptimes; i++)
            {
                msg = FsMessage.FromJson(json);
            }
            elapsed = (DateTime.Now.Ticks - start) / 10000.0; //s
            Console.WriteLine("FsMessage.FromJson: {0:#0.00} ms , speed = {1:#0.00} m/s", elapsed, looptimes*1000 / elapsed);

            start = DateTime.Now.Ticks;

            for (int i = 0; i < looptimes; i++)
            {
                msg = JsonConvert.DeserializeObject<FsMessage>(json);
            }
            elapsed = (DateTime.Now.Ticks - start) / 10000.0; //s
            Console.WriteLine("JsonConvert.DeserializeObject: {0:#0.00} ms , speed = {1:#0.00} m/s", elapsed, looptimes * 1000 / elapsed);


        }

    }
}
