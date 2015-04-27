using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Gprs.Cmd;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DAC.Test.Comm
{
    using FS.SMIS_Cloud.DAC.Accessor;
    using FS.SMIS_Cloud.DAC.Task;

    [TestFixture]
    public class GetAtCommadTester
    {
        JObject[] cmds = new[]
            {
                JObject.Parse("{\"cmd\": \"setCount\",\"param\":{\"count\":2}}"),
                JObject.Parse("{\"cmd\": \"setIP1\",\"param\": {\"ip\":\"223.4.212.14\"}}"),
                JObject.Parse("{\"cmd\": \"setPort1\",\"param\":{\"port\":4050}}"),
                JObject.Parse("{\"cmd\": \"setIP2\",\"param\": {\"ip\":\"218.3.150.107\"}}"),
                JObject.Parse("{\"cmd\": \"setPort2\",\"param\":{\"port\":5001}}"),
                JObject.Parse("{\"cmd\": \"setMode\",\"param\": {\"mode\":\"TRNS\"}}"),
                JObject.Parse("{\"cmd\": \"setByteInterval\",\"param\":{\"byteInterval\":100}}"),
                JObject.Parse("{\"cmd\": \"setRetry\",\"param\":{\"retry\":3}}")
            };
 
        [Test]
        public void GetAtCommandTest()
        {
            //string tid, uint dtuId, JObject[] cmdobjs, TaskType type, ATTaskResultConsumer tc
            ATTask task = new ATTask(string.Empty, 0, null, TaskType.INSTANT, null);
            ConfigCommand commadns = task.GetAtCommands(cmds);
            List<ATCommand> lst =new List<ATCommand>();
            lst = commadns.AtCommands;
            Assert.AreEqual("***COMMIT CONFIG***",lst[0].ToATString());
            Assert.AreEqual("AT+SVRCNT=2\r",lst[1].ToATString());
            Assert.AreEqual("AT+IPAD=223.4.212.14\r", lst[2].ToATString());
            Assert.AreEqual("AT+PORT=4050\r", lst[3].ToATString());
            Assert.AreEqual("AT+IPAD2=218.3.150.107\r", lst[4].ToATString());
            Assert.AreEqual("AT+PORT2=5001\r", lst[5].ToATString());
            Assert.AreEqual("AT+MODE=TRNS\r", lst[6].ToATString());
            Assert.AreEqual("AT+BYTEINT=100\r", lst[7].ToATString());
            Assert.AreEqual("AT+RETRY=3\r",lst[8].ToATString());
        }
        
       
        public void AtCommandTester()
        {
            int dacServerPort = 5055;
            var _DacServer = new GprsDtuServer(dacServerPort);
            var dtm = new DACTaskManager(_DacServer, DbAccessorHelper.DbAccessor.QueryDtuNodes(), DbAccessorHelper.DbAccessor.GetUnfinishedTasks());
            string tid = "";
            uint dtu = 2;
            int taskId = dtm.ArrangeInstantTask(tid, dtu, cmds, null);
        }

        [Test]
        public void GetJsonstr()
        {
            RemoteConfigResult result1 =new RemoteConfigResult()
            {
                cmd = "setCount",
                result = "OK"
            };
            RemoteConfigResult result2 = new RemoteConfigResult()
            {
                cmd = "setIP1",
                result = "OK"
            }; RemoteConfigResult result3 = new RemoteConfigResult()
            {
                cmd = "setIP2",
                result = "OK"
            };

            RemoteConfigResults result=new RemoteConfigResults()
            {
                dtuId = 90,
                cmds = new List<RemoteConfigResult>()
            };
            result.cmds.Add(result1);
            result.cmds.Add(result2);
            result.cmds.Add(result3);

            string jsonstr = JsonConvert.SerializeObject(result);
            Assert.AreEqual("{\"dtuId\":90,\"cmds\":[{\"cmd\":\"setCount\",\"result\":\"OK\"},{\"cmd\":\"setIP1\",\"result\":\"OK\"},{\"cmd\":\"setIP2\",\"result\":\"OK\"}]}", jsonstr);
        }
    }
    public class RemoteConfigResult
    {
        public string cmd { get; set; }

        public string result { get; set; }
    }

    public class RemoteConfigResults
    {
        public uint dtuId { get; set; }

        public List<RemoteConfigResult> cmds { get; set; }
    }


    
}