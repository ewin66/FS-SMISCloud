using System.Collections.Generic;
using System.Linq;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config
{
    // 节点, 汇聚点, 数据集中点, Node, COM总线, 等等;
    // 泛指为一种数据的汇聚点.
    public class DtuNode
    {
        public const uint DefaultDacInterval = 30*60; //seconds
        public const uint DefaultDacTimeout = 20; //seconds

        ///  Node ID
        public uint DtuId { get; set; }

        public DtuType Type { get; set; }

        public string DtuCode { get; set; }

        public string Name { get; set; }
 
        // 采集超时时间, 默认:20 Seconds
        public uint DacTimeout { get; set; }

        ///  采集间隔, 默认 30*60 Seconds
        public uint DacInterval { get; set; }

        private readonly IList<Sensor> _sensors;

        private Dictionary<string, object> _properties;

        // gprs, cdma ... might be null.
        public NetworkType? NetworkType { get; set; }

        ///  传感器清单
        public IList<Sensor> Sensors
        {
            get { return _sensors; }
        }

        public Dictionary<string, object> Properties
        {
            get { return _properties; }
        }

        public DtuNode()
        {
            this.DacTimeout = DefaultDacTimeout;
            this.DacInterval = DefaultDacInterval;
            _sensors = new List<Sensor>();
            _properties = new Dictionary<string, object>();
            Type = DtuType.Gprs; // default.
        }

        public void AddProperty(string key, object value)
        {
            _properties[key] = value;
        }

        public object GetProperty(string key)
        {
            return _properties.ContainsKey(key) ? _properties[key] : null;
        }

        public void AddSensor(Sensor sensor)
        {
            this._sensors.Add(sensor);
        }

        public Sensor FindSensor(uint sid)
        {
            if (this._sensors == null) return null;
            return _sensors.FirstOrDefault(s => s.SensorID == sid);
        }
    }

}
