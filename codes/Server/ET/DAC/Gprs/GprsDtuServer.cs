using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using log4net;
using ZYB;

namespace FS.SMIS_Cloud.DAC.Gprs
{
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
            if (OnConnectStatusChanged != null)
            {
                _log.InfoFormat("DtuConnection {0} going {1}", c, newStatus);
                OnConnectStatusChanged(c, oldStatus, newStatus);
            }
        }

        public IDtuConnection GetConnection(DtuNode  dtu)
        {
            return _connectPool.ContainsKey(dtu.DtuCode)? _connectPool[dtu.DtuCode]:null;
        }

        public GprsDtuServer(int port, int keepLiveInterval  = 200, ServerMode mode = ServerMode.SMALL)
        {
            _port = port;
            _mode = mode;
            _keepLiveInterval = keepLiveInterval;
        }

        /// <summary>
        /// 启动服务.
        /// </summary>
        public void Start()
        {
            if (_server!=null )
                return;
            // 1. 启动 Service
            _server = new Server(_port, _keepLiveInterval, ToMode(_mode));
            _server.ClientConnect += server_ClientConnect;
            _server.ClientClose += server_ClientClose;
            _server.ReceiveData += server_ReceiveData;
            _log.InfoFormat("GrpsServer started on :{0}, {1}", _port, _mode);
            try
            {
                _server.Start();
            }
            catch (Exception ex)
            {
                _log.FatalFormat(ex.Message);
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
            _server.Stop();
        }

        /// <summary>
        /// 更新传感器信息时，添加表中不存在的DTU
        /// </summary>
        /// <param name="dtuCode"></param>
        public void AddDtu(string dtuCode)
        {
            if (!_connectPool.ContainsKey(dtuCode))
            {
                var c = new GprsDtuConnection(this) { DtuID = dtuCode };
                _connectPool[dtuCode] = c;
            }
            _server.Disconnect(dtuCode);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void server_ReceiveData(object sender, ZYBEventArgs e)
        {
            string dtuid = e.DTU.ID;
            var c = _connectPool[dtuid];
            c.RefreshTime = e.DTU.RefreshTime;
            DtuMsg m = EventToMsg(e);
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
                if (!_connectPool.ContainsKey(dtuId))
                {
                    return;
                }
                var c = _connectPool[dtuId];
                c.UpdateDtuConnection(e);
                FireConnectionStatusChanged(c, WorkingStatus.IDLE, WorkingStatus.NA);
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
            if (!_connectPool.ContainsKey(dtuid))
            {
                c = new GprsDtuConnection(this) {DtuID = dtuid};
                _connectPool.TryAdd(c.DtuID, c);
               // _connectPool[c.DtuID] = c;
                isnewstate = true;
            }
            else
            {
                c =  _connectPool[dtuid];
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
                GprsDtuConnection c = RegisterConnection(e.DTU.ID, out isnewstate);
                c.UpdateDtuConnection(e);
                if (!isnewstate)
                {
                    _log.ErrorFormat("DTU {0} is online twice !!!", e.DTU.ID);
                    return;
                }
                FireConnectionStatusChanged(c, WorkingStatus.NA, WorkingStatus.IDLE);
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

            return _server.Send(dtuid, buffer);
        }

        public bool Disconnect(object obj)
        {
            var dtucode = obj as string;
            if (dtucode != null && _connectPool.ContainsKey(dtucode))
            {
               return _server.Disconnect(dtucode);
            }
            return true;
        }

        public List<string> GetDtuOnLineList()
        {
            return (from d in _connectPool.Values.ToList()
                where d.IsOnline
                select d.DtuID).ToList();
        }
    }
}
