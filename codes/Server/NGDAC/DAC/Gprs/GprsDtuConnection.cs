namespace FS.SMIS_Cloud.NGDAC.Gprs
{
    using System;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    using ZYB;

    public class GprsDtuConnection: IDtuConnection
    {
        private readonly ILog _log = LogManager.GetLogger("DtuConnection");

        public string DtuID { get; set; }
        public bool IsOnline { get; private set; }
        public WorkingStatus Status { get; private set; }

        public string IP { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime LoginTime { get; private set; }
        public DateTime RefreshTime { get; set; }

        private IDtuDataHandler _handler;
        private readonly GprsDtuServer _server;

        public GprsDtuConnection(GprsDtuServer server)
        {
            this._server = server;
        }

        public bool Connect()
        {
            return true;
        }

        public void Disconnect()
        {
        }

        public void registerDataHandler(IDtuDataHandler handler)
        {
            this._handler = handler;
        }

        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="buffer"></param>
        public void OnDataReceived(DtuMsg buffer)
        {
            if (this._handler != null)
            {
                this._handler.OnDataReceived(this, buffer);
            }
        }

        public bool IsAvaliable()
        {
            return this.IsOnline && this.Status == WorkingStatus.IDLE;
        }

        private delegate bool SendHandler(string dtuid, byte[] buffer);

        public bool Asend(byte[] buffer)
        {
            if (!this.IsAvaliable())
            {
                return false ;
            }
            SendHandler sh = this._server.Send;
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
                this._log.Error("Null send buffer required. ");
                return NewErrorMsg((int)Errors.ERR_NULL_SEND_DATA);
            }
            if (!this.IsOnline)
            {
                this._log.Error("DtuConnection Not Ready");
                return NewErrorMsg((int)Errors.ERR_NOT_CONNECTED);
            }
            if (this.Status != WorkingStatus.IDLE)  
            {
                this._log.Error("I'm BUSY!");
                return NewErrorMsg((int)Errors.ERR_DTU_BUSY);
            }
            this.Status = WorkingStatus.WORKING_SYNC;
            this._log.DebugFormat("DTU {0} SSending message, size={1}, {2}, timeout={3} s", this.DtuID, buffer.Length, ValueHelper.BytesToHexStr(buffer), timeout);
            IDtuDataHandler oldHandler = this._handler;
            var worker = new AsyncDtuMsgHandler(timeout);
            this._handler = worker;
            this._server.Send(this.DtuID, buffer);
            var thread = new Thread(worker.DoWork);
            thread.Start();
            thread.Join(); // wait thread dead.
            this.Status = WorkingStatus.IDLE;
            this._handler = oldHandler;
            return worker.Received();
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

        internal void UpdateDtuConnection(ZYBEventArgs e)
        {
            this.IsOnline = e.DTU.IsOnline;
            this.PhoneNumber = e.DTU.PhoneNumber;
            this.LoginTime = e.DTU.LoginTime;
            this.RefreshTime = e.DTU.RefreshTime;
            this.IP = e.DTU.IP;
            this.SetReady(e.DTU.IsOnline);
        }

        /// <summary>
        /// 同步应答
        /// </summary>
        /// <returns></returns>
        internal bool IsSyncSending()
        {
            return this.Status == WorkingStatus.WORKING_SYNC;
        }

        public override string ToString()
        {
            return String.Format("({0},{1},{2}", this.DtuID, this.IP, this.PhoneNumber);
        }
    }
}
