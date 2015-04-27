using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;

namespace FS.SMIS_Cloud.DAC.Task
{
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
            return DtuConnection != null && Node != null && DtuConnection.IsAvaliable();
        }
    }
}
