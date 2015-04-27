namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using System;

    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    public class TranMsg
    {
        private static readonly ILog Log = LogManager.GetLogger("TranMsg");

        private static byte _idSeq = 0;
        private const int LoadOffset = 8;
        public const byte ByteHead = 0xFA;
        public const byte ByteTail = 0xAF;
        public byte ID { get; set; }
        public TranMsgType Type { get; set; }
        public byte PackageIndex { get; set; }
        public byte PackageCount { get; set; }
        public ushort LoadSize { get; set; }


        public byte[] Data;

        //
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        public TranMsg()
        {
            this.ID = _idSeq++;
        }

        public TranMsg(byte[] buff)
        {
            if (buff != null)
            {
                this.Unmarshall(buff);
            }
        }

        private const int InfoSize = 10;

        public byte[] Marshall()
        {
            int size = this.Data.Length + InfoSize;
            byte[] buff = new byte[size];
            buff[0] = ByteHead;
            buff[1] = this.ID;
            buff[2] = (byte) this.Type;
            buff[3] = this.PackageIndex;
            buff[4] = this.PackageCount;
            ValueHelper.WriteShort(buff,5, (short) this.LoadSize); // 5,6
            buff[7] = 0xFF;
            Array.Copy(this.Data, 0, buff, LoadOffset, this.Data.Length);
            // XOR
            buff[size - 2] = ValueHelper.CheckCRC8(buff, 0, size - 3); 
            buff[size - 1] = ByteTail;
            return buff;
        }

        public virtual void Unmarshall(byte[] buff)
        {
            if (!this.IsValid(buff))
            {
                return;
            }
            this.ID = buff[1];
            this.Type = (TranMsgType) buff[2];
            this.PackageIndex = buff[3];
            this.PackageCount = buff[4];
            this.LoadSize = (ushort) ValueHelper.GetShort(buff, 5);
            this.Data = new byte[this.LoadSize];
            Array.Copy(buff, LoadOffset, this.Data, 0, this.LoadSize);
        }

        private bool IsValid(byte[] buff)
        {
            if (buff==null || buff.Length <= InfoSize)
            {
                this.ErrorCode = (int)Errors.ERR_INVALID_DATA;
                this.ErrorMsg = string.Format("Invalid msg: null or too short: {0}", buff != null ? buff.Length : 0);
                Log.Debug(this.ErrorMsg+":"+ValueHelper.BytesToHexStr(buff));
                return false; 
            }
            if (buff[0] != ByteHead || buff[buff.Length - 1] != ByteTail )
            {
                this.ErrorCode = (int)Errors.ERR_INVALID_DATA;
                this.ErrorMsg = string.Format("Invalid TranMsg format: HEAD/TAIL error: len={0}, H={1},T={2}", buff.Length,
                    buff[0], buff[buff.Length - 1]);
                Log.DebugFormat(this.ErrorMsg);
                return false;
            }
            byte crc8 = ValueHelper.CheckCRC8(buff, 0, buff.Length - 3);
            if (crc8 != buff[buff.Length - 2])
            {
                this.ErrorCode = (int)Errors.ERR_INVALID_DATA;
                this.ErrorMsg = string.Format("CRC Error: {0}!={1}", crc8, buff[buff.Length - 2]);
                Log.DebugFormat(this.ErrorMsg);
                return false;
            }
            this.ErrorCode = (int)Errors.SUCCESS;
            return true;
        }

        public static TranMsg Combine(TranMsg[] msgs)
        {
            TranMsg msg = new TranMsg();
            int len = 0;
            foreach (TranMsg tm in msgs)
            {
                len += tm.LoadSize;
            }
            msg.Data = new byte[len];
            msg.PackageIndex = 0;
            msg.PackageCount = 1;
            msg.LoadSize = (ushort) len;
            int offset = 0;
            foreach (TranMsg tm in msgs)
            {
                Array.Copy(tm.Data,0,msg.Data,offset, tm.LoadSize);
                offset += tm.LoadSize;
            }
            return msg;
        }

        public bool IsLastPackage()
        {
            return this.PackageIndex == this.PackageCount - 1;
        }
    }
}
