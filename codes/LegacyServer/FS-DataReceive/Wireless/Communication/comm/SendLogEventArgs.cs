using System;

namespace DataCenter.Communication.comm
{
    internal class SendLogEventArgs : EventArgs
    {
        public string Dtuid { get; set; }

        public string MoudleId { get; set; }

        public int ChannelNum { get; set; }

        public byte[] ReceiveData { get; set; }

        public byte DataType { get; set; }
    }

    internal class ReceiveLogEventArgs : EventArgs
    {
        public string Dtuid { get; set; }

        public string MoudleId { get; set; }

        public int ChannelNum { get; set; }

        public byte[] ReceiveData { get; set; }
        
    }


}
