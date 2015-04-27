namespace FS.SMIS_Cloud.NGDAC.Gprs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;

    using log4net;

    using ZYB;

    public class GprsDtuServer : IDtuServer
    {

        private readonly ILog _log = LogManager.GetLogger("GPRS");

        /// <summary>
        /// 
        /// </summary>
        private ConcurrentDictionary<string,GprsDtuConnection> _connectPool = new ConcurrentDictionary<string,GprsDtuConnection>();

        public ConnectStatusEventHandler OnConnectStatusChanged { get; set; }
        private Server _server;
        private int _port;
        private ServerMode _mode;
        private int _keepLiveInterval;

        private void FireConnectionStatusChanged(GprsDtuConnection c, WorkingStatus oldStatus, WorkingStatus newStatus)
        {
            if (this.OnConnectStatusChanged != null)
            {
                this._log.InfoFormat("DtuConnection {0} going {1}", c, newStatus);
                this.OnConnectStatusChanged(c, oldStatus, newStatus);
            }
        }

        public IDtuConnection GetConnection(DtuNode  dtu)
        {
            return this._connectPool.ContainsKey(dtu.DtuCode)? this._connectPool[dtu.DtuCode]:null;
        }

        public GprsDtuServer(int port, int keepLiveInterval  = 200, ServerMode mode = ServerMode.SMALL)
        {
            this._port = port;
            this._mode = mode;
            this._keepLiveInterval = keepLiveInterval;
        }

        /// <summary>
        /// 启动服务.
        /// </summary>
        public void Start()
        {
            if (this._server!=null )
                return;
            // 1. 启动 Service
            this._server = new Server(this._port, this._keepLiveInterval, this.ToMode(this._mode));
            this._server.ClientConnect += this.server_ClientConnect;
            this._server.ClientClose += this.server_ClientClose;
            this._server.ReceiveData += this.server_ReceiveData;
            this._log.InfoFormat("GrpsServer started on :{0}, {1}", this._port, this._mode);
            try
            {
                this._server.Start();
            }
            catch (Exception ex)
            {
                this._log.FatalFormat(ex.Message);
            }
            
        }

        private Mode ToMode(ServerMode mode)
        {
            if (ServerMode.BIG == mode)
                return Mode.大数据包模式;
            if (ServerMode.SMALL == mode)
                return Mode.小数据包模式;
            return Mode.全透明模式;
        }

        public void Stop()
        {
            this._server.Stop();
        }

        /// <summary>
        /// 更新传感器信息时，添加表中不存在的DTU
        /// </summary>
        /// <param name="dtuCode"></param>
        public void AddDtu(string dtuCode)
        {
            if (!this._connectPool.ContainsKey(dtuCode))
            {
                var c = new GprsDtuConnection(this) { DtuID = dtuCode };
                this._connectPool[dtuCode] = c;
            }
            this._server.Disconnect(dtuCode);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void server_ReceiveData(object sender, ZYBEventArgs e)
        {
            string dtuid = e.DTU.ID;
            var c = this._connectPool[dtuid];
            c.RefreshTime = e.DTU.RefreshTime;
            DtuMsg m = this.EventToMsg(e);
            m.IsWorking = true;
            c.OnDataReceived(m); // passthrough
        }

        private DtuMsg EventToMsg(ZYBEventArgs e)
        {
            return new DtuMsg
            {
                DtuId = e.DTU.ID,
                IsOnline = e.DTU.IsOnline,
                Logintime = e.DTU.LoginTime,
                Refreshtime = e.DTU.RefreshTime,
                Databuffer = e.DTU.DataByte
            };
        }

        private void server_ClientClose(object sender, ZYBEventArgs e)
        {
            lock (sender)
            {
                string dtuId = e.DTU.ID;
                if (!this._connectPool.ContainsKey(dtuId))
                {
                    return;
                }
                var c = this._connectPool[dtuId];
                c.UpdateDtuConnection(e);
                this.FireConnectionStatusChanged(c, WorkingStatus.IDLE, WorkingStatus.NA);
            }
        }

        /// <summary>
        /// 注册DTU（上线DTU注册）
        /// </summary>
        /// <param name="dtuid"></param>
        /// <param name="isnewstate"></param>
        /// <returns></returns>
        private GprsDtuConnection RegisterConnection(string dtuid, out bool isnewstate)
        {
            isnewstate = false;
            GprsDtuConnection c;
            if (!this._connectPool.ContainsKey(dtuid))
            {
                c = new GprsDtuConnection(this) {DtuID = dtuid};
                this._connectPool.TryAdd(c.DtuID, c);
               // _connectPool[c.DtuID] = c;
                isnewstate = true;
            }
            else
            {
                c =  this._connectPool[dtuid];
                if (!c.IsOnline)
                    isnewstate = true;
            }
            c.DtuID = dtuid;
            return c;
        }

        /// <summary>
        /// Connect, 注册服务.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void server_ClientConnect(object sender, ZYBEventArgs e)
        {
            lock (sender)
            {
                bool isnewstate;
                GprsDtuConnection c = this.RegisterConnection(e.DTU.ID, out isnewstate);
                c.UpdateDtuConnection(e);
                if (!isnewstate)
                {
                    this._log.ErrorFormat("DTU {0} is online twice !!!", e.DTU.ID);
                    return;
                }
                this.FireConnectionStatusChanged(c, WorkingStatus.NA, WorkingStatus.IDLE);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dtuid"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool Send(string dtuid, byte[] buffer)
        {
           // Console.WriteLine("Sending: size={0}", buffer.Length);

            return this._server.Send(dtuid, buffer);
        }

        public bool Disconnect(object obj)
        {
            var dtucode = obj as string;
            if (dtucode != null && this._connectPool.ContainsKey(dtucode))
            {
               return this._server.Disconnect(dtucode);
            }
            return true;
        }

        public List<string> GetDtuOnLineList()
        {
            return (from d in this._connectPool.Values.ToList()
                where d.IsOnline
                select d.DtuID).ToList();
        }
    }
}
