namespace FS.SMIS_Cloud.NGDAC.Com
{
    using System;
    using System.IO.Ports;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    public class ComDtuConnection : IDtuConnection
    {
        private static ILog Log = LogManager.GetLogger("ComDtuConnection");
        public bool IsOnline { get; set; } //always

        public string DtuID { get; private set; }
        public WorkingStatus Status { get; private set; }

        private readonly SerialPort _port;
        private uint _timeout;
        private IDtuDataHandler _handler;

        public bool IsAvaliable()
        {
            return WorkingStatus.IDLE == this.Status && this.IsOnline;
        }

        public void registerDataHandler(IDtuDataHandler handler)
        {
            this._handler = handler;
        }

        public ComDtuConnection(DtuNode dtu)
        {
            this.DtuID = dtu.DtuCode;
            this.Status = WorkingStatus.IDLE;
            this._port = (SerialPort)dtu.GetProperty("serial");
            this._port.ReadBufferSize = 1048;
            //    this._timeout = dtu..DacTimeout;
            this._port.DataReceived += this.OnDataReceived;
            this._port.ErrorReceived += this.OnErrorReceived;
        }

        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("ERROR:{0}:{1}", e.EventType, e.ToString());
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DtuMsg msg = null;
            int byteToRead = this.ReceiveFrame(this._port, 3); //3秒超时.
            byte[] recByteBuffer = null;
            if (byteToRead > 0)
            {
                recByteBuffer = new byte[byteToRead];
                this._port.Read(recByteBuffer, 0, recByteBuffer.Length);
                // _port.DiscardOutBuffer();
                msg = new DtuMsg
                {
                    Databuffer = recByteBuffer,
                    Refreshtime = System.DateTime.Now,
                    DtuId = this.DtuID,
                    ErrorCode = 0
                };
            }
            if (this._handler != null)
            {
                this._handler.OnDataReceived(this, msg);
            }
        }

        internal DtuMsg NewErrorMsg(int errCode, string errMsg = null)
        {
            return new DtuMsg { ErrorCode = errCode, ErrorMsg = errMsg };
        }

        public bool Asend(byte[] buffer)
        {
            if (!this.Connect())
            {
                return false;
            }
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }
            this._port.DiscardInBuffer();
            this._port.Write(buffer, 0, buffer.Length);
            return true;
        }

        public DtuMsg Ssend(byte[] buffer, int timeout)
        {

            try
            {
                if (!this.Connect())
                {
                    return this.NewErrorMsg((int)Errors.ERR_NOT_CONNECTED);
                }
                if (this.Status != WorkingStatus.IDLE)
                {
                    return this.NewErrorMsg((int)Errors.ERR_DTU_BUSY);
                }
                if (buffer == null || buffer.Length == 0)
                {
                    return this.NewErrorMsg((int)Errors.ERR_NULL_SEND_DATA);
                }
                this.Status = WorkingStatus.WORKING_SYNC;
                IDtuDataHandler oldHandler = this._handler;

                this._port.DiscardInBuffer();
                this._port.DataReceived -= this.OnDataReceived;
                SyncComReceiver worker = new SyncComReceiver(this, this.DtuID, timeout);
                Thread t = new Thread(worker.DoWork);
                long sentStart = DateTime.Now.Ticks;
                t.Start();
                Log.DebugFormat("Sending message: {0} , timeout = {1}", buffer.Length, timeout);
                this._port.Write(buffer, 0, buffer.Length);
                Log.DebugFormat("Sent in {0} ms.", (System.DateTime.Now.Ticks - sentStart) / 10000);
                t.Join();
                this._port.DataReceived += this.OnDataReceived;

                return worker.Received();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("SSend error: {0}", ex.Message);
                return this.NewErrorMsg((int)Errors.ERR_WRITE_COM, ex.Message);
            }
            finally
            {
                this.Status = WorkingStatus.IDLE;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", this._port.PortName, this._port.BaudRate);
        }

        /// <summary>
        /// Open the connection
        /// </summary>
        public bool Connect()
        {
            if (this._port.IsOpen)
            {
                return true;
            }
            try
            {
                this._port.Open();
               // _port.WriteBufferSize = 1024;
                this.IsOnline = true;
                return true;
            }
            catch (Exception e)
            {
                this.IsOnline = false;
                Log.ErrorFormat("Connect error: {0}", e.Message);
                return false;
            }
        }

        /// <summary>
        ///  Close the connection
        /// </summary>
        public void Disconnect()
        {
            this._port.DiscardInBuffer();
            this._port.DiscardOutBuffer();
            this._port.Close();
            this.IsOnline = false;
        }

        private int ReceiveFrame(SerialPort port, float timoutInSecond)
        {
            if (!port.IsOpen) return -1;
            long started = System.DateTime.Now.Ticks;
            int validBytes = 0;
            try
            {
                while (true)
                {
                    if (this.IsTimeOut(started, timoutInSecond)) // 200ms
                        return 0;
                    if (port.BytesToRead > validBytes || validBytes == 0)
                    {
                        Thread.Sleep(50); //200ms 内无数据, 认为结束.
                        validBytes = port.BytesToRead;
                    }
                    else
                    {
                        break;
                    }
                }
                return validBytes;
            }
            catch
            {
                return validBytes;
            }
        }

        private bool IsTimeOut(long started, float timeout)
        {
            long elapsed = System.DateTime.Now.Ticks - started;
            // Log.ErrorFormat("elapsed： {0} ms!", elapsed/10000);
            return elapsed / 10000 >= timeout * 1000; // ms
        }

        private class SyncComReceiver
        {
            private ILog Log = LogManager.GetLogger("SyncComReceiver");
            private float timeout = 3; //in second.
            private string dtuId;
            private DtuMsg receivedMsg = null;
            private bool dataReceived = false;
            private long started = 0;
            private SerialPort port;
            private ComDtuConnection _comconn;
            public SyncComReceiver(ComDtuConnection comconn, string dtuid, float timeout)
            {
                this.port = comconn._port;
                this._comconn = comconn;
                this.dtuId = dtuid;
                this.timeout = timeout;
                this.port.DataReceived += this.OnDataReceived;
            }

            public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                int byteToRead = this._comconn.ReceiveFrame(this.port, this.timeout);
                byte[] recByteBuffer = null;
                if (byteToRead > 0)
                {
                    recByteBuffer = new byte[byteToRead];
                    this.port.Read(recByteBuffer, 0, recByteBuffer.Length);
                    this.port.DiscardOutBuffer();
                    var name = ((SerialPort)sender).PortName;
                    this.Log.DebugFormat("[{0}] received: {1}", this.port.PortName, ValueHelper.BytesToHexStr(recByteBuffer));
                    this.receivedMsg = new DtuMsg
                    {
                        Databuffer = recByteBuffer,
                        Refreshtime = System.DateTime.Now,
                        DtuId = this.dtuId,
                        ErrorCode = 0
                    };
                    this.port.DataReceived -= this.OnDataReceived;
                    this.dataReceived = true;
                }
            }



            internal DtuMsg Received()
            {
                if (!this.dataReceived)
                    return new DtuMsg { ErrorCode = (int)Errors.ERR_DTU_TIMEOUT, ErrorMsg = "Timeout" };
                else
                    return this.receivedMsg;
            }

            public void DoWork()
            {
                this.started = System.DateTime.Now.Ticks;
                while (!this.dataReceived)
                {
                    if (this._comconn.IsTimeOut(this.started, this.timeout))
                    {
                        this.Log.ErrorFormat("Timeout!");
                        break;
                    }
                    Thread.Sleep(10);
                }
                if (!this.dataReceived)
                {
                    this.Log.ErrorFormat("[{0}] Timeout: ({1} seconds)", this.port.PortName, this.timeout);
                }
                else
                {
                    this.receivedMsg.Elapsed = (System.DateTime.Now.Ticks - this.started) / 10000; //ms
                    this.Log.InfoFormat("[{0}] Msg received in {1} ms, {2}:{3}",
                        this.port.PortName, this.receivedMsg.Elapsed, this.receivedMsg.Databuffer.Length,
                        ValueHelper.BytesToHexStr(this.receivedMsg.Databuffer, 0, ""));
                }
                this.Log.Info("Send task finished");
            }

        }
    }
}
