using Newtonsoft.Json;

namespace FS.SMIS_Cloud.DAC.Gprs.Cmd
{
    public class SetRetryTimes:ATCommand
    {
        public ushort ReryTimes { get; set; }

        /// <summary>
        ///  设置重连次数
        /// </summary>
        /// <param name="jsonStr"> {retry: [int] }</param>
        public SetRetryTimes(string jsonStr)
        {
            var anonymous = new { retry = 9 };
            var jsobj = JsonConvert.DeserializeAnonymousType(jsonStr, anonymous);
            this.ReryTimes = (byte)jsobj.retry;
        }

        public string ToATString()
        {
            return string.Format("AT+RETRY={0}\r", this.ReryTimes);
        }
    }
}
