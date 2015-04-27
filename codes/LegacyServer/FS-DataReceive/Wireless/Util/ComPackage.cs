using System;

namespace DataCenter.Util
{
    public enum SendType : byte
    {
        CollectingPackage = 0,
        DtuRestart = 1
    }
    public enum ReceiveType : byte
    {
        Online = 0,
        Offline = 1,
        Data = 2,
        RequestOrResponse=3,
        Error = 4
    }
    public class ComPackage : EventArgs
    {
        public int SensorSetId { get; set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; private set; }

        public ReceiveType Type { get; set; }

        public object Item { get; set; }

        public string Id { get; set; }

        public string Sender { get; set; }

        /// <summary>
        /// Gets the ip address.
        /// </summary>
        public string IPAddress { get; private set; }

        /// <summary>
        /// Gets the phone number.
        /// </summary>
        public string PhoneNumber { get; private set; }

        /// <summary>
        /// Gets the data received.
        /// </summary>
        public byte[] DataReceived { get; private set; }

        public int SafeTypeId { get; set; }

        public ComPackage() { }


        public ComPackage(ReceiveType type, string sender, byte[] dataBytes)
        {
            this.Type = type;
            this.Sender = sender;
            this.DataReceived = dataBytes;
        }

        public ComPackage(ReceiveType type, object obj)
            : this()
        {
            
                Type = type;
            Item = obj;
        }

        public ComPackage(ReceiveType type, string id, object obj)
            : this(type, obj)
        {
            Id = id;
        }
    }
}