using System;
using DataCenter.Model;

namespace DataCenter.Communication.Communication
{
    public enum Protocol
    {
        TCPIP,
        SerialPort
    }
    public delegate void OnConnectionChangedListener(DTUConnectionEventArgs args);

    public delegate void OnDataReceivedHandler(ReceiveDataInfo receivedbuff);

    public abstract class ModBusWrapper : IDisposable
    {
        private static ModBusWrapper _Instance = null;
        
        public static ModBusWrapper CreateInstance(Protocol protocol)
        {
            if (_Instance == null)
            {
                switch (protocol)
                {
                    case Protocol.TCPIP:
                        _Instance = new ModBusTCPIPWrapper();
                        break;
                    default:
                        break;
                }
            }
            return _Instance;
        }

        public abstract bool StartService();

        public abstract bool StopService();

        public abstract bool Send(object obj);

        public OnConnectionChangedListener OnConnectionChangedHandler;

        public OnDataReceivedHandler OnDataReceived;

        #region IDisposable 成员

        public virtual void Dispose()
        {
            _Instance.Dispose();
        }

        #endregion IDisposable 成员
    }
}