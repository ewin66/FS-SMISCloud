namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using System;

    using FS.SMIS_Cloud.NGDAC.Util;

    public class TranAckMsg:HeartBeatTranMsg
    {
        public TranAckMsg():base(){}
        public int DtuCode { get; private set; }
        public int Received { get; private set; }

        public override void Unmarshall(byte[] buff)
        {
            base.Unmarshall(buff);
            this.DtuCode = Convert.ToInt32(ValueHelper.GetString(this.Data, 0, 8));
            this.Received = ValueHelper.GetShort(this.Data, 8);
        }
    }
}
