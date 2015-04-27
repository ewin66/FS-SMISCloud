using System;
using System.Collections;
using System.Collections.Generic;
using ZYB;

namespace DataCenter.Communication.comm.Comm
{
    public class ServerManager
    {
      //  private static log4net.ILog log = LogManager.GetLogger("SM");
        
        /// <summary>
        /// 
        /// </summary>
        private static Hashtable _connectPool = new Hashtable();

        private static IList<IConnectEventListener> _connectEventListeners = new List<IConnectEventListener>();
        private static Server _server;

        public static void RegisterConnectionEventListener(IConnectEventListener listener)
        {
            _connectEventListeners.Add(listener);
        }

        private static void fireConnectionStatusChanged(Connection c, WorkingStatus oldStatus, WorkingStatus newStatus)
        {
            foreach (IConnectEventListener listener in _connectEventListeners)
            {
                listener.OnConnectionStatusChanged(c, oldStatus, newStatus);
            }
        }

        public static Connection GetConnection(string dtuId)
        {
            return (Connection) _connectPool[dtuId];
        }

        /// <summary>
        /// 启动服务.
        /// </summary>
        public static void Start()
        {
            // 0. 从数据库初始化 未连接的 Connection;

            // 1. 启动 Service
            _server = new Server(5055, 200, Mode.小数据包模式);
            _server.Start();
            _server.ClientConnect += server_ClientConnect;
            _server.ClientClose += server_ClientClose;
            _server.ReceiveData += server_ReceiveData;
        }

        public static void Stop()
        {
            _server.Stop();
        }

        /// <summary>
        /// 更新传感器信息时，添加表中不存在的DTU
        /// </summary>
        /// <param name="dtu"></param>
        public static void AddDtu(string dtu)
        {
            if (!_connectPool.Contains(dtu))
            {
                var c = new Connection { DtuID = dtu };
                _connectPool[dtu]=c;
            }
            _server.Disconnect(dtu);
        }
        
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void server_ReceiveData(object sender, ZYBEventArgs e)
        {
            string dtuid = e.DTU.ID;
            Connection c = (Connection)_connectPool[dtuid];
            c.RefreshTime = e.DTU.RefreshTime;
            DtuMsg m = EventToMsg(e);
            m.IsWorking = true;
            c.onDataReceived(m); // passthrough
        }

        private static DtuMsg EventToMsg(ZYBEventArgs e)
        {
            DtuMsg m = new DtuMsg
                           {
                               DtuId = e.DTU.ID,
                               IsOnline = e.DTU.IsOnline,
                               Logintime = e.DTU.LoginTime,
                               Refreshtime = e.DTU.RefreshTime,
                               Databuffer = e.DTU.DataByte
                           };
            return m;
        }

        static void server_ClientClose(object sender, ZYBEventArgs e)
        {
            string dtuId = e.DTU.ID;
            if (!_connectPool.Contains(dtuId))
            {
                // DTU Not registered? system error.
                //log.Fatal(string.Format("System error, DTU '{0}' Not registered!!!", dtuId));
                return;
            }
            var c = (Connection)_connectPool[dtuId];
            DtuMsg m = EventToMsg(e);
            m.IsWorking = false;
            c.onDataReceived(m); 
            c.SetReady(false);
            fireConnectionStatusChanged(c, WorkingStatus.IDLE, WorkingStatus.NA);
            Console.WriteLine("Client closed.");
        }
        
        /// <summary>
        /// 注册DTU（上线DTU注册）
        /// </summary>
        /// <param name="dtuid"></param>
        /// <param name="ip"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private static Connection registerConnection(string dtuid, string ip, string phoneNumber)
        {
            Connection c;
            if (!_connectPool.Contains(dtuid))
            {
                c = new Connection();
                c.DtuID = dtuid;
                c.PhoneNumber = phoneNumber;
                _connectPool[c.DtuID] = c;
            }else {
                c = (Connection) _connectPool[dtuid];
            }
            c.DtuID = dtuid;
            c.IP = ip;
            return c;
        }
        
        /// <summary>
        /// Connect, 注册服务.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void server_ClientConnect(object sender, ZYBEventArgs e)
        {
            Connection c = registerConnection(e.DTU.ID, e.DTU.IP, e.DTU.PhoneNumber);
            c.IsOnline = e.DTU.IsOnline;
            c.PhoneNumber = e.DTU.PhoneNumber;
            c.LoginTime = e.DTU.LoginTime;
            c.RefreshTime = e.DTU.RefreshTime;
            c.SetReady(true);
            DtuMsg m = EventToMsg(e);
            m.IsWorking = false;
            c.onDataReceived(m);
            fireConnectionStatusChanged(c, WorkingStatus.NA, WorkingStatus.IDLE);
            Console.WriteLine(string.Format("Connection online: {0},{1}({2}) on {3}", c.DtuID, c.IP, c.PhoneNumber, c.LoginTime));
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dtuid"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool Send(string dtuid, byte[] buffer)
        {
           Console.WriteLine("Sending: size={0}", buffer.Length);
            return _server.Send(dtuid, buffer);
        }
 
    }
}
