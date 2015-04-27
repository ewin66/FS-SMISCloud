using System;
using System.Collections.Generic;
using System.Text;
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Gprs.Cmd;
using Newtonsoft.Json.Linq;

namespace FS.SMIS_Cloud.DAC.Task
{
    public delegate void ATTaskResultConsumer(ExecuteResult result);
    public class ATTask
    {
        public DACTaskStatus Status { get; set; }

        // 要采集的DTU ID
        public uint DtuID { get; set; }
        
        public ConfigCommand AtCommands { get; private set; }

        public bool Saved { get; set; }
        public int ID { get; set; }
        public string TID { get; set; }
        public string Requester { get; set; }
        public DateTime Requested { get; set; }
        public DateTime Finished { get; set; }
        public TaskType Type { get; set; }

        public ATTaskResultConsumer Consumer;

        public ATTask(string tid, uint dtuId, JObject[] cmdobjs, TaskType type, ATTaskResultConsumer tc)
        {
            this.TID = tid;
            this.DtuID = dtuId;
            this.Type = type;
            this.AtCommands = this.GetAtCommands(cmdobjs);
            this.Requested = DateTime.Now;
            this.Requester = "Admin";
            this.Saved = false;
            this.Status = DACTaskStatus.RUNNING;
            Consumer = tc;
        }

        public ConfigCommand GetAtCommands(JObject[] cmdobjs)
        {
            var lst = new List<ATCommand>();
            var atCommands = new ConfigCommand();
            if (cmdobjs != null && cmdobjs.Length > 0)
            {
                try
                {
                    if (cmdobjs.Length == 8)
                    {
                        lst.Add(new SetMainIpCount(cmdobjs[0].Value<JObject>("param").ToString()));
                        lst.Add(new SetIP(new StringBuilder(cmdobjs[1].Value<JObject>("param").ToString()).Insert(1, "'index':1,").ToString()));
                        lst.Add(new SetPort(new StringBuilder(cmdobjs[2].Value<JObject>("param").ToString()).Insert(1, "'index':1,").ToString()));
                        lst.Add(new SetIP(new StringBuilder(cmdobjs[3].Value<JObject>("param").ToString()).Insert(1, "'index':2,").ToString()));
                        lst.Add(new SetPort(new StringBuilder(cmdobjs[4].Value<JObject>("param").ToString()).Insert(1, "'index':2,").ToString()));
                        lst.Add(new SetWorkMode(cmdobjs[5].Value<JObject>("param").ToString()));
                        lst.Add(new SetByteInterval(cmdobjs[6].Value<JObject>("param").ToString()));
                        lst.Add(new SetRetryTimes(cmdobjs[7].Value<JObject>("param").ToString()));
                        //lst.Add(new QuitConfig());
                    }
                    else
                    {
                        lst.Add(new SetReset());
                    }
                    
                    atCommands.AddRange(lst);
                    return atCommands;
                }
                catch (Exception ex)
                {
                    return atCommands;
                }
            }
            lst.Add(new SetReset());
            atCommands.AddRange(lst);
            return atCommands;
        }
    }
}