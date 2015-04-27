using System;
using System.Threading;
using log4net;

namespace DataCenter.Communication.comm.Comm
{
    public class Connection
    {
        private static ILog log = LogManager.GetLogger("Connection");
        public string DtuID { get; set; }
        public string IP { get; set; }
        public bool IsOnline { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime RefreshTime { get; set; }
        private WorkingStatus Status { get; set; }

        private IDtuDataHandler handler;



        public void registerDataHandler(IDtuDataHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="handler"></param>
        internal void onDataReceived(DtuMsg buffer)
        {
            if (this.handler != null)
            {
                this.handler.OnDataReceived(buffer);
            }
        }

        public bool IsAvaliable()
        {
            return this.IsOnline && this.Status == WorkingStatus.IDLE;
        }

        private delegate bool SendHandler(string dtuid, byte[] buffer);

        public bool Send(byte[] buffer)
        {
            if (!this.IsAvaliable())
            {
                return false ;
            }
            SendHandler sh = new SendHandler(ServerManager.Send);
            return sh.Invoke(this.DtuID, buffer); // synchronized.
        }

        internal static DtuMsg NewErrorMsg(int errCode, string errMsg = null)
        {
            return  new DtuMsg { ErrorCode = errCode, ErrorMsg = errMsg };
        }

        /// <summary>
        /// 同步发送接收
        /// </summary>
        /// <param name="buffer">Can't be null! </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public DtuMsg Ssend(byte[] buffer, int timeout)
        {
            if (buffer == null || buffer.Length == 0)
            {
                log.Error("Null send buffer required. ");
                return NewErrorMsg(Errors.ERR_NULL_SEND_DATA);
            }
            if (!this.IsOnline)
            {
                log.Error("Connection Not Ready");
                return NewErrorMsg(Errors.ERR_NOT_CONNECTED);
            }
            if (this.Status != WorkingStatus.IDLE)  
            {
                log.Error("I'm BUSY!");
                return NewErrorMsg(Errors.ERR_DTU_BUSY);
            }
            this.Status = WorkingStatus.WORKING_SYNC;
            log.DebugFormat("SSending message, size={0}, timeout={1} s", buffer.Length, timeout);
            IDtuDataHandler oldHandler = this.handler;
            AsyncMsgReceiver worker = new AsyncMsgReceiver(this.DtuID, timeout);
            this.handler = worker;
            ServerManager.Send(this.DtuID, buffer);
            Thread thread = new Thread(worker.DoWork);
            thread.Start();
            thread.Join(); // wait thread dead.
            this.Status = WorkingStatus.IDLE;
            this.handler = oldHandler;
            return worker.Received();
        }


        internal class AsyncMsgReceiver: IDtuDataHandler
        {
            int timeout;
            string dtuId;
            DtuMsg receivedMsg = null;
            bool dataReceived = false;
            long started =0;
            public AsyncMsgReceiver(string dtuid, int timeout)
            {
                this.dtuId = dtuid;
                this.timeout = timeout;
            }

            public void OnDataReceived(DtuMsg msg)
            {
                log.Debug("Data received in MessageWorker");
                this.dataReceived = true;
                this.receivedMsg = msg;
            }

            internal DtuMsg Received()
            {
                if (!this.dataReceived)
                    return NewErrorMsg(Errors.ERR_DTU_TIMEOUT);
                else
                    return this.receivedMsg;
            }

            public void DoWork()
            {
                this.started = System.DateTime.Now.Ticks;
                while (!this.dataReceived)
                {
                    if (this.IsTimeOut())
                        break;
                    Thread.Sleep(10);
                }
                if (!this.dataReceived )
                {
                    log.ErrorFormat("Timeout {0} seconds!", this.timeout);
                }
            }
            bool IsTimeOut()
            {
                long elapsed = System.DateTime.Now.Ticks - this.started;
                // log.ErrorFormat("elapsed： {0} ms!", elapsed/10000);
                return elapsed / 10000 >= this.timeout * 1000; // ms
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ready"></param>
        internal void SetReady(bool ready)
        {
            this.Status = ready? WorkingStatus.IDLE : WorkingStatus.NA;
            this.IsOnline = ready;
        }

        /// <summary>
        /// 同步应答
        /// </summary>
        /// <returns></returns>
        internal bool IsSyncSending()
        {
            return this.Status == WorkingStatus.WORKING_SYNC;
        }
    }
}
