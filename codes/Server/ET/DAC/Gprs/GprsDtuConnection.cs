using System;
using System.Threading;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;
using ZYB;

namespace FS.SMIS_Cloud.DAC.Gprs
{
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
            _server = server;
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
            _handler = handler;
        }

        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="buffer"></param>
        public void OnDataReceived(DtuMsg buffer)
        {
            if (_handler != null)
            {
                _handler.OnDataReceived(this, buffer);
            }
        }

        public bool IsAvaliable()
        {
            return IsOnline && Status == WorkingStatus.IDLE;
        }

        private delegate bool SendHandler(string dtuid, byte[] buffer);

        public bool Asend(byte[] buffer)
        {
            if (!IsAvaliable())
            {
                return false ;
            }
            SendHandler sh = _server.Send;
            return sh.Invoke(DtuID, buffer); // synchronized.
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
                _log.Error("Null send buffer required. ");
                return NewErrorMsg((int)Errors.ERR_NULL_SEND_DATA);
            }
            if (!IsOnline)
            {
                _log.Error("DtuConnection Not Ready");
                return NewErrorMsg((int)Errors.ERR_NOT_CONNECTED);
            }
            if (Status != WorkingStatus.IDLE)  
            {
                _log.Error("I'm BUSY!");
                return NewErrorMsg((int)Errors.ERR_DTU_BUSY);
            }
            Status = WorkingStatus.WORKING_SYNC;
            _log.DebugFormat("DTU {0} SSending message, size={1}, {2}, timeout={3} s", DtuID, buffer.Length, ValueHelper.BytesToHexStr(buffer), timeout);
            IDtuDataHandler oldHandler = _handler;
            var worker = new AsyncDtuMsgHandler(timeout);
            _handler = worker;
            _server.Send(DtuID, buffer);
            var thread = new Thread(worker.DoWork);
            thread.Start();
            thread.Join(); // wait thread dead.
            Status = WorkingStatus.IDLE;
            _handler = oldHandler;
            return worker.Received();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ready"></param>
        internal void SetReady(bool ready)
        {
            Status = ready? WorkingStatus.IDLE : WorkingStatus.NA;
            IsOnline = ready;
        }

        internal void UpdateDtuConnection(ZYBEventArgs e)
        {
            IsOnline = e.DTU.IsOnline;
            PhoneNumber = e.DTU.PhoneNumber;
            LoginTime = e.DTU.LoginTime;
            RefreshTime = e.DTU.RefreshTime;
            IP = e.DTU.IP;
            SetReady(e.DTU.IsOnline);
        }

        /// <summary>
        /// 同步应答
        /// </summary>
        /// <returns></returns>
        internal bool IsSyncSending()
        {
            return Status == WorkingStatus.WORKING_SYNC;
        }

        public override string ToString()
        {
            return String.Format("({0},{1},{2}", DtuID, IP, PhoneNumber);
        }
    }
}
