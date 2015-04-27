namespace FS.SMIS_Cloud.NGDAC.Node
{
    using System;

    public class DtuMsg
    {

        public string DtuId { get; set; }

        public bool IsOnline { get; set; }

        public DateTime Logintime { get; set; }

        public DateTime Refreshtime { get; set; }

        public byte[] Databuffer { get; set; }

        public bool IsWorking { get; set; }

        // 错误消息。
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        public long Elapsed { get; set; }

        public DtuMsg()
        {
            this.ErrorCode = 0;
        }
        public bool IsOK()
        {
            return this.ErrorCode == 0;
        }
    }
}
