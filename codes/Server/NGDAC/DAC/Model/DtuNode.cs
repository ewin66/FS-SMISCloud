namespace FS.SMIS_Cloud.NGDAC.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using log4net;

    // 节点, 汇聚点, 数据集中点, Node, COM总线, 等等;
    // 泛指为一种数据的汇聚点.
    public class DtuNode
    {
        public const uint DefaultDacInterval = 30*60; //seconds
        public const uint DefaultDacTimeout = 20; //seconds
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ///  Node ID
        public uint DtuId { get; set; }

      //  public uint StructId { get; set; }

        public DtuType Type { get; set; }

        public string DtuCode { get; set; }

        public string Name { get; set; }
 
        // 采集超时时间, 默认:20 Seconds
        public uint DacTimeout { get; set; }

        ///  采集间隔, 默认 30*60 Seconds
        public uint DacInterval { get; set; }

        private readonly List<Sensor> _sensors;

        private Dictionary<string, object> _properties;

        // gprs, cdma ... might be null.
        public NetworkType? NetworkType { get; set; }

        ///  传感器清单
        public IList<Sensor> Sensors
        {
            get { return this._sensors; }
        }

        public Dictionary<string, object> Properties
        {
            get { return this._properties; }
        }

        private ConcurrentQueue<SensorOperation> sencache;

        public DtuNode()
        {
            this.DacTimeout = DefaultDacTimeout;
            this.DacInterval = DefaultDacInterval;
            this._sensors = new List<Sensor>();
            this._properties = new Dictionary<string, object>();
            this.Type = DtuType.Gprs; // default.
        }

        public void AddProperty(string key, object value)
        {
            this._properties[key] = value;
        }

        public object GetProperty(string key)
        {
            return this._properties.ContainsKey(key) ? this._properties[key] : null;
        }

        public void AddSensor(Sensor sensor)
        {
            if (sensor == null) return;
            var sen = this._sensors.FirstOrDefault(s => s.SensorID == sensor.SensorID);
            if (sen == null)
            {
                this._sensors.Add(sensor);
            }
            else
            {
                this.ModifySensor(sensor);
            }
        }

        public Sensor FindSensor(uint sid)
        {
            if (this._sensors == null) return null;
            return this._sensors.FirstOrDefault(s => s.SensorID == sid);
        }

        public bool RemoveSensor(Sensor sensor)
        {
            if (this._sensors == null) return false;
           return this._sensors.Remove(this._sensors.FirstOrDefault(sen => sensor.SensorID == sen.SensorID));
        }

        public void ModifySensor(Sensor sensor)
        {
            if (sensor == null) return;
            var sen = this._sensors.FirstOrDefault(s => s.SensorID == sensor.SensorID);
            if (sen != null)
            {
                int index = this._sensors.IndexOf(sen);
                if (index > -1)
                {
                    this._sensors[index] = sensor;
                    this._log.InfoFormat("Dtu {0} Update Sensor config {1}", this.DtuCode,sensor.SensorID);
                }
            }
            else
            {
                this.AddSensor(sensor);
            }
        }

        public void AddSensorOperation(SensorOperation sensor)
        {
            if (this.sencache == null)
            {
                this.sencache=new ConcurrentQueue<SensorOperation>();
            }
            this.sencache.Enqueue(sensor);
        }

        private void RemoveSensor(uint senId)
        {
            Sensor sen = this.FindSensor(senId);
            if (sen == null) return;
            this._sensors.Remove(sen);
            this._log.InfoFormat("Dtu {0} delete a Sensor -{1}",this.DtuCode ,senId);
        }

        public void UpDateSensor()
        {
            if (this.sencache == null || this.sencache.IsEmpty)
            {
                this._log.InfoFormat("Dtu {0}  has no sensor need to update .", this.DtuCode);
                return;
            }
            while (!this.sencache.IsEmpty)
            {
                SensorOperation senopera;
                if (this.sencache.TryDequeue(out senopera))
                {
                    if (senopera != null)
                    {
                        this.DoIt(senopera);
                    }
                }

                Thread.Sleep(50);
            }
        }

        private void DoIt(SensorOperation senopera)
        {
            try
            {
                switch (senopera.Action)
                {
                    case Operations.Add:
                        this.AddSensor(senopera.Sensor);
                        this._log.InfoFormat("Dtu {0} add a new sensor {1}", this.DtuCode, senopera.Sensor.SensorID);
                        break;
                    case Operations.Delete:
                        this.RemoveSensor(senopera.OldSensorId);
                        break;
                    case Operations.Update:
                        this.ModifySensor(senopera.Sensor);
                        break;
                }
            }
            catch (Exception ex)
            {
                this._log.ErrorFormat("Dtu {0} upDateSensors error {1}", this.DtuCode, ex.Message);
            }
        }

    }
}
