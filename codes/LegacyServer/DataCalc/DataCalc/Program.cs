namespace FreeSun.FS_SMISCloud.Server.DataCalc
{
    using System.Diagnostics;
    using System.Threading;

    using FreeSun.FS_SMISCloud.Server.DataCalc.Communication;
    using FreeSun.FS_SMISCloud.Server.HeartBeat.Client;

    class Program
    {
        private static readonly HeartBeatSender heartBeatSender = new HeartBeatSender(Process.GetCurrentProcess());
        private static Timer heartBeaTimer;

        static void Main(string[] args)
        {
            #region 每分钟发送1次心跳
            //heartBeaTimer = new Timer(o => heartBeatSender.SendHeartBeat(), null, 0, 60000); 
            #endregion

            CalcServer calcServer = new CalcServer();
            calcServer.Start();
        }
    }
}
