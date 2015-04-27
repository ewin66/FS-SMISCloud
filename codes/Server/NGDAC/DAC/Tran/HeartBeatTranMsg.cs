namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using FS.SMIS_Cloud.NGDAC.Util;

    public class HeartBeatTranMsg : TranMsg
    {
        public HeartBeatTranMsg()
        {
        }

        public HeartBeatTranMsg(int DtuCode, int receivedSize) : base()
        {
/*
data: 10 bytes
0-7:  DTUID号BCD编码, 8字节字符
8/9: 00  接收到的长度 (ACK)
*/

            this.Type = TranMsgType.HeartBeat;
            this.PackageCount = 1;
            this.PackageIndex = 0;
            this.Data = new byte[10];
            this.LoadSize = (ushort) this.Data.Length;
            ValueHelper.WriteAscii(this.Data, 0, DtuCode); // 8 bytes
            ValueHelper.WriteShort(this.Data, 8, (short) receivedSize); // 2bytes;
        }
 
    }
}
