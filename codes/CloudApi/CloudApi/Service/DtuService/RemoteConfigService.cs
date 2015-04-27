using System;
using System.Linq;
using System.Web;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Service.DtuService
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RemoteConfigService
    {
        static public List<CommandConfig> GetCommand(RemoteConfig rc)
        {
            var list = new List<CommandConfig>();

            var ccCount = new CommandConfig 
            {
                Cmd = "setCount",
                Param = new { count = rc.Count } 
            };
            list.Add(ccCount);

            var ccIp1 = new CommandConfig
            {
                Cmd = "setIP1",
                Param = new { ip = rc.Ip1 }
            };
            list.Add(ccIp1);

            var ccPort1 = new CommandConfig
            {
                Cmd = "setPort1",
                Param = new { port = rc.Port1 }
            };
            list.Add(ccPort1);

            var ccIp2 = new CommandConfig
            {
                Cmd = "setIP2",
                Param = new { ip = rc.Ip2 }
            };
            list.Add(ccIp2);

            var ccPort2 = new CommandConfig
            {
                Cmd = "setPort2",
                Param = new { port = rc.Port2 }
            };
            list.Add(ccPort2);

            var ccMode = new CommandConfig
            {
                Cmd = "setMode",
                Param = new { mode = rc.Mode }
            };
            list.Add(ccMode);

            var ccByteInterval = new CommandConfig
            {
                Cmd = "setByteInterval",
                Param = new { byteInterval = rc.ByteInterval }
            };
            list.Add(ccByteInterval);

            var ccRetry = new CommandConfig
            {
                Cmd = "setRetry",
                Param = new { retry = rc.Retry }
            };
            list.Add(ccRetry);

            return list;
        }
    }

    public class CommandConfig
    {
        [JsonProperty("cmd")]
        public string Cmd { get; set; }

        [JsonProperty("param")]
        public object Param { get; set; }
    }
}