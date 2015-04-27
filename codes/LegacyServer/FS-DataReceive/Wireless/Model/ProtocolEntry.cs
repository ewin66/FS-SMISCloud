

namespace DataCenter.Model
{
    class ProtocolEntry
    {
        public byte[] HeaderOfFrame { get; set; }

        public byte[] EndOfFrame { get; set; }

        public byte LengthOfFrame { get; set; }

        public byte[] CheckCode { get; set; }
    }
}
