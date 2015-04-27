using Newtonsoft.Json;

namespace FS.SMIS_Cloud.DAC.Gprs.Cmd
{
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
            if (Count <= 0)
            {
                Count = 1;
            }
            if (Count > 3)
            {
                Count = 3;
            }
        }

        public string ToATString()
        {
            return string.Format("AT+SVRCNT={0}\r", Count);
        }
    }
}
