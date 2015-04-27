using System.Threading;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.Gprs
{
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
            _timeout = timeout;
        }

        public void OnDataReceived(IDtuConnection c, DtuMsg msg)
        {
            _log.DebugFormat("Data received: from {0} buff: {1}", c.DtuID, msg != null && msg.Databuffer != null ? ValueHelper.BytesToHexStr(msg.Databuffer) : " empty.");
            _dataReceived = true;
            _receivedMsg = msg;
        }

        internal DtuMsg Received()
        {
            if (!_dataReceived)
                return new DtuMsg { ErrorCode = (int)Errors.ERR_DTU_TIMEOUT, ErrorMsg = "TimeOut" };
            return _receivedMsg;
        }

        public void DoWork()
        {
            _started = System.DateTime.Now.Ticks;
            while (!_dataReceived)
            {
                if (IsTimeOut())
                    break;
                Thread.Sleep(10);
            }
            if (!_dataReceived)
            {
                _log.WarnFormat("Timeout {0} seconds!", _timeout);
            }
            else
            {
                _receivedMsg.Elapsed = (System.DateTime.Now.Ticks - _started) / 10000; //ms
                _log.DebugFormat("Msg received in {0} ms", _receivedMsg.Elapsed);
            }
        }

        bool IsTimeOut()
        {
            long elapsed = System.DateTime.Now.Ticks - _started;
            // Log.ErrorFormat("elapsed： {0} ms!", elapsed/10000);
            return elapsed / 10000 >= _timeout * 1000; // ms
        }
    }
}
