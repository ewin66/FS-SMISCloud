using System;

namespace DataCenter.Model
{
    public class ReceiveDataInfo : EventArgs
    {
        public ReceiveDataInfo(string sender, byte[] packet)
        {
            this.Sender = sender;
            this.PackagesBytes = packet;
        }

        public string Sender { get;private set; }

        public byte[] PackagesBytes { get; private set; }
    }
}
