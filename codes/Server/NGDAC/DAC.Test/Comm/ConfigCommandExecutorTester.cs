namespace NGDAC.Test.Comm
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Gprs;
    using FS.SMIS_Cloud.NGDAC.Gprs.Cmd;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Task;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class ConfigCommandExecutorTester
    {
        private GprsDtuConnection conn;

  
        [Test]
        [Category("MANUAL")]
        public void TestConfigCommandExecutor()
        {
            ConfigCommand cfgCmd = new ConfigCommand();
            cfgCmd.AddRange(new List<ATCommand>{ new SetIP("{'ip':'192.168.1.145'}"), new SetPort("{'port':5050}"), new SetReset() });
           string dtu = "12345678";
           GprsDtuServer _server = new GprsDtuServer(5055);
           _server.Start();
            DacTaskContext context=new DacTaskContext()
            {
                DtuConnection = new GprsDtuConnection(_server),
                Node = new DtuNode(){DtuCode = dtu}
            };
          
            Thread.Sleep(8000);
            
            //conn = (GprsDtuConnection) _server.GetConnection(new DtuNode{DtuCode = dtu});

            if (context.DtuConnection != null && context.DtuConnection.IsAvaliable())
            {
                var cce = new ConfigCommandExecutor();
                //cce.Execute(conn, cfgCmd, 12000);
                ExecuteResult result = cce.Execute(null, context, 12000);
                // Assert.IsTrue(result.IsOK);
            }
            else
            {
                 //Assert.
            }

        }

        [Test]
        public void SensorOperationSerializeTest()
        {
            var senact = new SensorOperation
            {
                Sensor = new Sensor
                {
                    AcqInterval = 10,
                    ChannelNo = 1,
                    DtuCode = "1234432",
                    ModuleNo = 2345,
                    Name = "tear",
                    ProtocolType = 12,
                    FactorType = 23,
                    StructId = 1
                },
                Action = Operations.Add
            };

            string str = JsonConvert.SerializeObject(senact);
            Console.WriteLine(str);
            
            var senopera = JsonConvert.DeserializeObject<SensorOperation>(str);
            Console.WriteLine(senopera.OldDtuId);
            JObject jsobj = JObject.Parse(str);
            var sen = JsonConvert.DeserializeObject<Sensor>(jsobj["Sensor"].ToString());
            uint dtuid = Convert.ToUInt32(jsobj["DtuId"]);
            Console.WriteLine(sen.SensorID);
            Console.WriteLine(dtuid);
            // OldSensorId
            // OldDtuId 
            uint oldSenId = Convert.ToUInt32(jsobj["OldSensorId"]); 
            Console.WriteLine("OldSensorId :{0}", oldSenId);
            uint oldDtuId = Convert.ToUInt32(jsobj["OldDtuId"]); 
            Console.WriteLine("OldDtuId: {0}", oldDtuId);

        }

    }
}
