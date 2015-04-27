using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Node;
using log4net;

namespace FS.SMIS_Cloud.DAC.Tran 
{
    public delegate void TranMsgListener(TranMsgType type, TranMsg msg);

    public delegate void ClientConnectListener(IDtuConnection c,WorkingStatus status);

    public class TranDataReceiver : IDtuDataHandler, IDisposable
    {
        public const int SendAckTimeOut = 2; //seconds
        private static readonly ILog Log = LogManager.GetLogger("DacReceiver");
        public TranMsgListener OnTranMsgReceived;
        public ClientConnectListener OnClientConnected;

        private ConcurrentDictionary<TranMsgType, Queue<TranMsg>> _msgQueue;
        private IDtuServer _server;
        public TranDataReceiver(IDtuServer server)
        {
            _server = server;//new GprsDtuServer(listenOn);
            _server.OnConnectStatusChanged += DtuOnline;
            _msgQueue = new ConcurrentDictionary<TranMsgType, Queue<TranMsg>>();
        }

        public void Start()
        {
            _server.Start();
        }

        public void Dispose()
        {
            _server.OnConnectStatusChanged -= DtuOnline;
        }

        public void Stop()
        {
            _server.Stop();
        }

        private void DtuOnline(IDtuConnection dtuConnection, WorkingStatus old, WorkingStatus news)
        {
            var gprsDtuConnection = dtuConnection as GprsDtuConnection;
            if (gprsDtuConnection == null) return;
            if (news == WorkingStatus.IDLE)
                gprsDtuConnection.registerDataHandler(this);
            // DtuConnection.onDataReceived+= OnDataReceived;
            if (OnClientConnected != null)
            {
                OnClientConnected(gprsDtuConnection,news);
            }
        }

        public void OnDataReceived(IDtuConnection c, DtuMsg msg)
        {
            if (msg == null)
            {
                Log.ErrorFormat("Received a null msg from {0}.", c);
                return;
            }
 
            if (!msg.IsOK())
            {
                Log.DebugFormat("Invalid Msg, dropped.");
                return;
            }
            TranMsg tm = new TranMsg(msg.Databuffer);
            // Send ack.
            TranMsg ack = new HeartBeatTranMsg(Convert.ToInt32(msg.DtuId), tm.LoadSize); // ACK
            ack.ID = tm.ID; // SAME.

            c.Asend(ack.Marshall());
            if (TranMsgType.HeartBeat == tm.Type)
            {
                // HeartBeat. 
                Log.DebugFormat("heartBeat, send ack and continue");
                return;
            }
            Log.DebugFormat("OnDataReceived: id={4},result={0}, len={1}, pkg={2}/{3}", msg.ErrorCode,
                msg.Databuffer != null ? msg.Databuffer.Length : 0, tm.PackageIndex, tm.PackageCount,tm.ID);
            if (!tm.IsLastPackage())
            {
                // 入列.
                EnqueueMsg(tm);
                return;
            }
            // 最后一包已到达, 或仅一包.
            TranMsg outMsg = null;
            if (tm.PackageCount > 1)
            {
                Log.DebugFormat("Last package, combine them.");
                outMsg = CombineMsg(tm.Type); //组包
            }
            else
            {
                outMsg = tm; //单包.
            }
            // 委托调用.    
            if (OnTranMsgReceived != null)
            {
                OnTranMsgReceived(tm.Type, outMsg);
            }
        }

        private void EnqueueMsg(TranMsg tm)
        {
            TranMsgType t = tm.Type;
            Queue<TranMsg> queue = null;
            if (!this._msgQueue.ContainsKey(t))
                this._msgQueue[t] = queue = new Queue<TranMsg>();
            else
            {
                queue = this._msgQueue[t];
                if (tm.PackageIndex == 0 && queue.Count>0)
                {
                    Log.ErrorFormat("Invalid msg idx: {0}, purge old packages.", tm.PackageIndex);
                    queue.Clear();
                }
            }
            queue.Enqueue(tm);
        }

        private TranMsg CombineMsg(TranMsgType type)
        {
            var queue = this._msgQueue[type];
            TranMsg ms=TranMsg.Combine(queue.ToArray());
            queue.Clear();
            return ms;
        }
    }
 
}
