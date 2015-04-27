namespace FS.SMIS_Cloud.NGDAC
{
    using System;
    using System.Configuration;
    using System.Threading;

    using FS.Service;
    using FS.SMIS_Cloud.Services.ConsoleCtrlManager;

    using log4net;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("ET.MAIN");

        private static int Main()
        {
            var instance = ConsoleCtrlManager.Instance;
            var cs = ConfigurationManager.AppSettings["SecureCloud"];
            if (cs == null)
            {
                Log.Error("ConnectionString null, terminated.");
                return -1;
            }
            try
            {
                var service = new DacService("DacService.xml", AppDomain.CurrentDomain.BaseDirectory);
                new ServiceRunner(service).Run("DacService");
            }
            catch (Exception ex)
            {
                Log.FatalFormat(ex.Message);
                Log.FatalFormat(ex.StackTrace);
            }
            while (true)
            {
                Thread.Sleep(100);
            }
            // TODO 当ET关闭时所有dtu被迫下线
            return 0;
        }
    }
}