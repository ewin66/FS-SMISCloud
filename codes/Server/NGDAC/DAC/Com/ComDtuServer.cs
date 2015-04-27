namespace FS.SMIS_Cloud.NGDAC.Com
{
    using System.Collections.Concurrent;

    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;

    public class ComDtuServer:IDtuServer
    {
        private  ConcurrentDictionary<string, ComDtuConnection>  _connectPool = new ConcurrentDictionary<string, ComDtuConnection>();
        public ConnectStatusEventHandler OnConnectStatusChanged { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public bool Send(string dtuid, byte[] buffer)
        {
            return true;
        }

        public bool Disconnect(object dtucode)
        {
            return false;
        }

        // COM 建立连接.
        public IDtuConnection GetConnection(DtuNode dtuInfo)
        {
            ComDtuConnection cc = null;
            string dtuId = dtuInfo.DtuCode;
            if (!this._connectPool.ContainsKey(dtuId))
            {
                cc = new ComDtuConnection(dtuInfo);
                this._connectPool[dtuId] = cc;
            }
            else
            {
                cc = this._connectPool[dtuId];
            }
            return cc;
        }

    }
}
