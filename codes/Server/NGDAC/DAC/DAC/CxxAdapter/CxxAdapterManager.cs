namespace FS.SMIS_Cloud.NGDAC.DAC.CxxAdapter
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using FS.DynamicScript;
    using FS.SMIS_Cloud.NGDAC.Model;

    using log4net;

    public class CxxAdapterManager : IAdapterManager
    {
        private static readonly ILog Log = LogManager.GetLogger("SensorEncoder");

        [ImportMany] 
        private Lazy<ISensorAdapter, ISensorProtocol>[] _adapters = null;

        private Dictionary<uint, ISensorAdapter> _adapterMap;

        private const string Adaptersconfig = "adaptersconfig.xml";
        private const string AdapterReferences = "AdapterReferences.json";

        private static object locker =new object();

        public void Initialize()
        {
            //var catalog = new AssemblyCatalog(typeof(CxxAdapterManager).Assembly);
            var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "*.*");
            CompositionContainer cc = new CompositionContainer(catalog);
            cc.ComposeParts(this);

            this._adapterMap = new Dictionary<uint, ISensorAdapter>();
            foreach (Lazy<ISensorAdapter, ISensorProtocol> sa in this._adapters)
            {
                this._adapterMap[sa.Metadata.Protocol] = sa.Value;
            }
            this.ReLoad();
            try
            {
                CrossDomainCompiler.LoadScript(string.Format("{0}\\{1}", this._adapterPath, AdapterReferences), this._adapterPath);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("脚本编译错误{0}",ex.Message);
            }
        }

        public ISensorAdapter GetSensorAdapter(uint protocolType)
        {
            if (this._adapterMap.ContainsKey(protocolType))
            {
                return this._adapterMap[protocolType];
            }
            Log.ErrorFormat("Adapter for {0} NOT found.", protocolType);
            return null;
        }
        
        public void EncodeRequest(ref SensorAcqResult sensorAcq)
        {
            ISensorAdapter cv = this.GetSensorAdapter(sensorAcq.Sensor.ProtocolType);
            if(cv==null)
            {
                sensorAcq.ErrorCode = (int)Node.Errors.ERR_UNKNOW_PROTOCOL;
                sensorAcq.Request = null;
                return;
            }
            cv.Request(ref sensorAcq);
        }
        
        /// <summary>
        /// 脚本列表
        /// </summary>
        private readonly ConcurrentDictionary<uint, Adapter> _scriptlst = new ConcurrentDictionary<uint, Adapter>();

        private string _adapterPath = AppDomain.CurrentDomain.BaseDirectory;

        public Adapter GetAdapter(uint protocolType)
        {
            if (this._scriptlst.Count == 0)
            {
                lock (locker)
                {
                    if (this._scriptlst.Count == 0)
                    {
                        this.ReLoad();
                    }
                }
            }
            if (this._scriptlst.ContainsKey(protocolType))
                return this._scriptlst[protocolType];
            return null;
        }

        public void ReLoad()
        {
            this._adapterPath = ConfigurationManager.AppSettings["ScriptPath"];
            if (!Directory.Exists(this._adapterPath))
            {
                Log.ErrorFormat("{0} is not exist !", this._adapterPath);
                return;
            }

            if (!System.IO.File.Exists(string.Format("{0}\\{1}", this._adapterPath, Adaptersconfig)))
            {
                Log.ErrorFormat("{0} is not exist !", this._adapterPath);
                return;
            }
            var settings = new XmlReaderSettings { IgnoreComments = true };
            XmlReader reader = XmlReader.Create(string.Format("{0}\\{1}", this._adapterPath, Adaptersconfig), settings);
            var xmldoc = new XmlDocument();
            xmldoc.Load(reader);

            var xmlNode = xmldoc.SelectSingleNode("/Adapters");
            if (xmlNode != null)
            {
                foreach (var node in xmlNode.ChildNodes.Cast<XmlElement>().Where(node => node.Name == "Adapter"))
                {
                    uint protocol;
                    if (uint.TryParse(node.Attributes["protocol"].Value, out protocol))
                    {
                        if (!string.IsNullOrEmpty(node.Attributes["filename"].Value))
                        {
                            var adapter = new Adapter(protocol, node.Attributes["filename"].Value)
                            {
                                ScriptPath = this._adapterPath
                            };
                            this._scriptlst.AddOrUpdate(protocol, adapter, (k, v) => adapter);
                        }
                    }
                }
            }
        }

    }
}
