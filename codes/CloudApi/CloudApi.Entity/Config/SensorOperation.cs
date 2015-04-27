using System;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config
{
    [Serializable]
    public class SensorOperation
    {
        public Sensor Sensor { get; set; }

        public uint OldSensorId { get; set; }

        public uint OldDtuId { get; set; }

        public Operations Action { get; set; }
    }
}