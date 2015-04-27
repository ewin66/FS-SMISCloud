namespace FS.SMIS_Cloud.NGDAC.Task
{
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;

    public  delegate bool SensorFilter(Sensor sensor);

    // 任务资源
    public class DacTaskContext
    {
        public IDtuConnection DtuConnection { get; set; }
        public DtuNode Node { get; set; }
        public SensorFilter SensorMatcher { get; set; }
        internal void UpdateConnection(IDtuConnection c)
        {
            this.DtuConnection = c;
        }

        internal bool IsAvaliable()
        {
            return this.DtuConnection != null && this.Node != null && this.DtuConnection.IsAvaliable();
        }
    }
}
