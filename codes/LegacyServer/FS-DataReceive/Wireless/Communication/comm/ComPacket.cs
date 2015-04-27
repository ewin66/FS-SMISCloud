using System;
using ZYB;

namespace DataCenter.Communication.comm
{
    public enum ReceiveType : byte
    {
        Online = 0,
        Offline = 1,
        Data = 2
    }

    public class ComPacket : EventArgs
    {
        public ComPacket()
        {
        }

        public ComPacket(byte type)
        {
            Type = type;
        }
        public ComPacket(byte type,byte[] by)
            : this()
        {

            Type = type;
            Item = by;
        }

        public ComPacket(byte type, byte[] by, string dtuId):this(type,by)
        {
            DtuId = dtuId;
        }

        public string DtuId { get; set; }

        public byte Type { get; set; }

        public byte[] Item { get; set; }
    }

    public class ComPacketEventArgs:ComPacket
    {
        public ComPacketEventArgs()
        {
        }

        public ComPacketEventArgs(byte type):base(type)
        {
        }

       public ComPacketEventArgs(byte type,ZYBEventArgs e):this(type)
        {
            Arg = e;
        }

        public ZYBEventArgs Arg { get; set; }

    }
}
