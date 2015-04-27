namespace FS.SMIS_Cloud.NGET
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;

    using FS.Service;

    using log4net;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("NGET.MAIN");

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static int RemoveMenu(IntPtr hMenu, int nPos, int flags);

        private static int Main()
        {
            IntPtr WINDOW_HANDLER = Process.GetCurrentProcess().MainWindowHandle;
            IntPtr CLOSE_MENU = GetSystemMenu(WINDOW_HANDLER, IntPtr.Zero);
            int SC_CLOSE = 0xF060;
            RemoveMenu(CLOSE_MENU, SC_CLOSE, 0x0);

            var cs = ConfigurationManager.AppSettings["SecureCloud"];
            if (cs == null)
            {
                Log.Error("ConnectionString null, terminated.");
                return -1;
            }
            try
            {
                var service = new NGEtService("EtService.xml", AppDomain.CurrentDomain.BaseDirectory);
                new ServiceRunner(service).Run("NGEtService");
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
        }
    }
}