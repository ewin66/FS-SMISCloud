using System;
using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.Task;
using Newtonsoft.Json;

namespace FS.SMIS_Cloud.DAC.Gprs.Cmd
{
    [Serializable]
    public class ExecuteResult
    {
        public bool IsOK { get; set; }
        public byte[] ResultBuffer { get; set; }
        public DateTime Finished { get; set; }

        public long Elapsed { get; set; }

        public IList<ATCommandResult> AtResults { get; private set; }

        public ATTask Task { get; set; }
        public string ErrorMsg { get; set; }
        public int ErrorCode { get; set; }

        public void AddAtResult(ATCommandResult r)
        {
            if (AtResults == null)
            {
                AtResults=new List<ATCommandResult>();
            }
            AtResults.Add(r);
        }

        public string ToJsonString()
        {
            var results = new RemoteConfigResults
            {
                dtuId = this.Task.DtuID,
                cmds = new List<RemoteConfigResult>()
            };
            if (AtResults != null)
            {
                foreach (ATCommandResult result in AtResults)
                {
                    if (result != null && result.JsonResult != null)
                    {
                        results.cmds.Add(result.JsonResult);
                    }
                }
                return JsonConvert.SerializeObject(results);
            }
            return string.Empty;
        }
    }

    [Serializable]
    public class ATCommandResult
    {
        public ATCommandResult()
        {
            IsOK = false;
        }

        public byte[] ResultBuffer { get; set; }

        public long Elapsed { get; set; }

        public bool IsOK { get; set; }

        public void GetJsonResult(string atstring)
        {
            string[] atcmdstrs = atstring.Split(new[] {'+', '=', '\r', ' '}, StringSplitOptions.RemoveEmptyEntries);
            switch (atcmdstrs[1].Trim())
            {
                case "SVRCNT":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setCount",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "IPAD":
                case "IPAD1":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setIP1",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "PORT":
                case "PORT1":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setPort1",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "IPAD2":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setIP2",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "PORT2":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setPort2",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "MODE":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setMode",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "BYTEINT":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setByteInterval",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "RETRY":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "setRetry",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                case "RESET":
                    this.JsonResult = new RemoteConfigResult
                    {
                        cmd = "restart",
                        result = this.IsOK ? "OK" : "ERROR"
                    };
                    break;
                default:
                    this.JsonResult = null;
                    break;
            }
        }

        internal RemoteConfigResult JsonResult { get; set; }
    }

    public class RemoteConfig
    {
        /// <summary>
        /// 中心IP数量 (目前数量为2个)
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip1 { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int? Port1 { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip2 { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int? Port2 { get; set; }

        /// <summary>
        /// DTU工作模式
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 封包间隔时间
        /// </summary>
        public int? ByteInterval { get; set; }

        /// <summary>
        /// 重连次数
        /// </summary>
        public int? Retry { get; set; }
    }

    internal class RemoteConfigResult
    {
       public string cmd { get; set; }

       public string result { get; set; }
    }

    internal class RemoteConfigResults
    {
        public uint dtuId { get; set; }

        public List<RemoteConfigResult> cmds { get; set; }
    }
}
