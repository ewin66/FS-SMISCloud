using System;
using System.Reflection;
using System.Text;
using FS.SMIS_Cloud.DAC.Node;
using log4net;

namespace FS.SMIS_Cloud.DAC.Gprs.Cmd
{

    public class CommandExecutor
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ATCommandResult Execute(GprsDtuConnection conn, ATCommand cmd, ushort timeout = 2)
        {
            var r = new ATCommandResult();
            if (!conn.IsOnline)
            {
                r.IsOK = false;
                r.GetJsonResult(cmd.ToATString());
                return r;
            }
            try
            {
                logger.InfoFormat("======> AtCommand:{0}-{1} ,timeout={2}", conn.DtuID, cmd.ToATString(), timeout);
                DtuMsg result = conn.Ssend(Encoding.ASCII.GetBytes(cmd.ToATString()), timeout);

                r.ResultBuffer = result.Databuffer;
                string bufstr = string.Empty;
                if (r.ResultBuffer != null)
                    bufstr = Encoding.ASCII.GetString(result.Databuffer);
                r.IsOK = ((bufstr.Contains("OK")) || ("Remote Config Ready" == bufstr)) ||
                         cmd.ToATString().Contains("RESET");
                // r.GetJsonResult(cmd.ToATString());
                r.Elapsed = result.Elapsed;
                // System.Console.WriteLine("> {0}\r\n< {1} : {2}", cmd.ToATString(), bufstr, r.IsOK? "successed":"failed");
                logger.InfoFormat("======> AtCommand:{0}-{1} ,result: {2}, {3} ", conn.DtuID, cmd.ToATString(), bufstr,
                    r.IsOK ? "successed" : "failed");

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("CommandExecutor Error: {0}", ex);
            }
            r.GetJsonResult(cmd.ToATString());
            return r;
        }
    }
}
