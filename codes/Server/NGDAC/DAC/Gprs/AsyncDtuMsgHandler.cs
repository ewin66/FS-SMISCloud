namespace FS.SMIS_Cloud.NGDAC.Gprs
{
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    class AsyncDtuMsgHandler : IDtuDataHandler
    {
        private readonly ILog _log = LogManager.GetLogger("AsyncDtuMsgHandler");
        readonly int _timeout;
        //string _dtuId;
        DtuMsg _receivedMsg;
        bool _dataReceived;
        long _started;
        public AsyncDtuMsgHandler(int timeout)
        {
            //_dtuId = dtuid;
            this._timeout = timeout;
        }

        public void OnDataReceived(IDtuConnection c, DtuMsg msg)
        {
            this._log.DebugFormat("Data received: from {0} buff: {1}", c.DtuID, msg != null && msg.Databuffer != null ? ValueHelper.BytesToHexStr(msg.Databuffer) : " empty.");
            this._dataReceived = true;
            this._receivedMsg = msg;
        }

        internal DtuMsg Received()
        {
            if (!this._dataReceived)
                return new DtuMsg { ErrorCode = (int)Errors.ERR_DTU_TIMEOUT, ErrorMsg = "TimeOut" };
            return this._receivedMsg;
        }

        public void DoWork()
        {
            this._started = System.DateTime.Now.Ticks;
            while (!this._dataReceived)
            {
                if (this.IsTimeOut())
                    break;
                Thread.Sleep(10);
            }
            if (!this._dataReceived)
            {
                this._log.WarnFormat("Timeout {0} seconds!", this._timeout);
            }
            else
            {
                this._receivedMsg.Elapsed = (System.DateTime.Now.Ticks - this._started) / 10000; //ms
                this._log.DebugFormat("Msg received in {0} ms", this._receivedMsg.Elapsed);
            }
        }

        bool IsTimeOut()
        {
            long elapsed = System.DateTime.Now.Ticks - this._started;
            // Log.ErrorFormat("elapsed： {0} ms!", elapsed/10000);
            return elapsed / 10000 >= this._timeout * 1000; // ms
        }
    }
}
