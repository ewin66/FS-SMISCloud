using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FS.SMIS_Cloud.Services.ConsoleCtrlManager;
using log4net;

namespace ConsoleCtrlManager.Test
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("Program");

        public static void Main(string[] args)
        {
            var instance = FS.SMIS_Cloud.Services.ConsoleCtrlManager.ConsoleCtrlManager.Instance;
            instance.RegCmd("load", LoadHandler);
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);

                    Log.Info("INFO");
                }
            });
            thread.Start();
        }

        private static string LoadHandler(string[] args)
        {
            return "执行load-shell";
        }
    }
}
