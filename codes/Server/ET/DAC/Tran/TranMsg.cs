using System;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.Tran
{
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
            ID = _idSeq++;
        }

        public TranMsg(byte[] buff)
        {
            if (buff != null)
            {
                Unmarshall(buff);
            }
        }

        private const int InfoSize = 10;

        public byte[] Marshall()
        {
            int size = Data.Length + InfoSize;
            byte[] buff = new byte[size];
            buff[0] = ByteHead;
            buff[1] = ID;
            buff[2] = (byte) Type;
            buff[3] = PackageIndex;
            buff[4] = PackageCount;
            ValueHelper.WriteShort(buff,5, (short) LoadSize); // 5,6
            buff[7] = 0xFF;
            Array.Copy(Data, 0, buff, LoadOffset, Data.Length);
            // XOR
            buff[size - 2] = ValueHelper.CheckCRC8(buff, 0, size - 3); 
            buff[size - 1] = ByteTail;
            return buff;
        }

        public virtual void Unmarshall(byte[] buff)
        {
            if (!IsValid(buff))
            {
                return;
            }
            this.ID = buff[1];
            this.Type = (TranMsgType) buff[2];
            this.PackageIndex = buff[3];
            this.PackageCount = buff[4];
            this.LoadSize = (ushort) ValueHelper.GetShort(buff, 5);
            this.Data = new byte[LoadSize];
            Array.Copy(buff, LoadOffset, Data, 0, LoadSize);
        }

        private bool IsValid(byte[] buff)
        {
            if (buff==null || buff.Length <= InfoSize)
            {
                ErrorCode = (int)Errors.ERR_INVALID_DATA;
                ErrorMsg = string.Format("Invalid msg: null or too short: {0}", buff != null ? buff.Length : 0);
                Log.Debug(ErrorMsg+":"+ValueHelper.BytesToHexStr(buff));
                return false; 
            }
            if (buff[0] != ByteHead || buff[buff.Length - 1] != ByteTail )
            {
                ErrorCode = (int)Errors.ERR_INVALID_DATA;
                ErrorMsg = string.Format("Invalid TranMsg format: HEAD/TAIL error: len={0}, H={1},T={2}", buff.Length,
                    buff[0], buff[buff.Length - 1]);
                Log.DebugFormat(ErrorMsg);
                return false;
            }
            byte crc8 = ValueHelper.CheckCRC8(buff, 0, buff.Length - 3);
            if (crc8 != buff[buff.Length - 2])
            {
                ErrorCode = (int)Errors.ERR_INVALID_DATA;
                ErrorMsg = string.Format("CRC Error: {0}!={1}", crc8, buff[buff.Length - 2]);
                Log.DebugFormat(ErrorMsg);
                return false;
            }
            ErrorCode = (int)Errors.SUCCESS;
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
