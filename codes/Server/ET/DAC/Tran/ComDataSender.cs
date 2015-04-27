using System;
using System.Collections.Generic;
using System.IO.Ports;
using FS.SMIS_Cloud.DAC.Com;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;

namespace FS.SMIS_Cloud.DAC.Tran
{
    public class ComDataSender: ITranDataSendDelegator
    {
        private ComDtuConnection _dtuConnection = null;
        public int DtuCode { get; set; }

        public void Init(Dictionary<string, string> pms)
        {
            DtuCode = 20120049;
            string com = pms["PortName"];
            DtuNode node = new DtuNode();
            node.DtuCode = com;
            node.AddProperty("serial", new SerialPort
            {
                PortName = com,
                BaudRate = Convert.ToInt32(pms["BaudRate"]),
                Parity = (Parity) Convert.ToInt32(pms["Parity"]),
                DataBits = Convert.ToInt32(pms["DataBits"]),
                StopBits = (StopBits) Convert.ToInt32(pms["StopBits"]),
                ReadTimeout = Convert.ToInt32(pms["ReadTimeOut"])
            });
            this._dtuConnection = new ComDtuConnection(node);
        }

        public bool SSend(TranMsg req, int timeout, out TranAckMsg resp)
        {
            DtuMsg msg = _dtuConnection.Ssend(req.Marshall(), timeout);
            resp = new TranAckMsg(); 
            if (msg.IsOK())
            {
                resp.Unmarshall(msg.Databuffer);
                return true;
            }
            else
            {
                resp.ErrorMsg = msg.ErrorMsg;
                resp.ErrorCode = msg.ErrorCode;
                return false;
            }
        }

        public bool Connect()
        {
            return _dtuConnection.Connect();
        }

        public void Disconnect()
        {
            _dtuConnection.Disconnect();
        }

        public bool HeartBeat()
        {
            if (_dtuConnection.IsAvaliable())
            {
                HeartBeatTranMsg req = new HeartBeatTranMsg(DtuCode, 0);
                TranAckMsg resp;
                if (SSend(req, 5, out resp))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsConnected()
        {
            return _dtuConnection!=null && _dtuConnection.IsOnline;
        }
    }
}
