namespace FS.SMIS_Cloud.NGDAC.Gprs.Cmd
{
    using Newtonsoft.Json;

    public class SetMainIpCount:ATCommand
    {
        public ushort Count { get; set; }
        
        /// <summary>
        /// 设置中心IP数量（需和 SetIP，setPort匹配）
        /// </summary>
        /// <param name="jsonStr">{count:[int, [1,3] ] }</param>
        public SetMainIpCount(string jsonStr)
        {
            var anonymous = new { count = 1 };
            var jsonobj = JsonConvert.DeserializeAnonymousType(jsonStr, anonymous);
            
            this.Count = (byte)jsonobj.count;
            if (this.Count <= 0)
            {
                this.Count = 1;
            }
            if (this.Count > 3)
            {
                this.Count = 3;
            }
        }

        public string ToATString()
        {
            return string.Format("AT+SVRCNT={0}\r", this.Count);
        }
    }
}
