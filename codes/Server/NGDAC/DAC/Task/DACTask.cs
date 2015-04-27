namespace FS.SMIS_Cloud.NGDAC.Task
{
    using System;
    using System.Collections.Generic;

    public delegate void TaskResultConsumer(DACTaskResult result);
    // DCU DAC Task
    [Serializable]
    public class DACTask
    {
        public DACTaskStatus Status { get; set; }

        // 要采集的DTU ID
        public uint DtuID { get; set; }

        // 要采集的传感器， 当为空时表示全部。
        public IList<uint> Sensors { get; set; }

        public bool Saved { get; set; }

        // Task ID, GUID
        public string TID { get; set; }
        public int ID { get; set; }
        public string Requester { get; set; }
        public DateTime Requested { get; set; }
        public DateTime Finished { get; set; }
        public TaskType Type { get; set; }

        public TaskResultConsumer Consumer;
        public DACTask()
        {
        }
        public DACTask(string tid, uint dtuId, IList<uint> sensors, TaskType type, TaskResultConsumer tc,int taaskid=0)
        {
            this.TID = tid;
            this.DtuID = dtuId;
            this.Type = type;
            this.Sensors = sensors;
            this.Requested = System.DateTime.Now;
            this.Requester = "Admin";
            this.Saved = false;
            this.Status = DACTaskStatus.RUNNING;
            this.Consumer = tc;
            this.ID = taaskid;
        }

    }
}
