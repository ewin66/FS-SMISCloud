using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace FS.Service
{
    public class ServiceRunner
    {

        //public delegate bool ConsoleCtrlDelegate(int ctrlType);

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        //public static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool add);
        ////当用户关闭Console时，系统会发送次消息
        //private const int CTRL_CLOSE_EVENT = 2;
        ////Ctrl+C，系统会发送次消息
        //private const int CTRL_C_EVENT = 0;
        ////Ctrl+break，系统会发送次消息
        //private const int CTRL_BREAK_EVENT = 1;
        ////用户退出（注销），系统会发送次消息
        //private const int CTRL_LOGOFF_EVENT = 5;
        ////系统关闭，系统会发送次消息
        //private const int CTRL_SHUTDOWN_EVENT = 6;

 //       static bool _IsExit = false;
        private Service service = null;
        public ServiceRunner(Service service)
        {
            this.service = service;
        }

        public void Run(string title)
        {
            Console.Title = title;
            //Thread threadMonitorInput = new Thread(new ThreadStart(MonitorInput));
            //threadMonitorInput.Start();
            service.Start();
        }


        //private static bool HandlerRoutine(int ctrlType)
        //{
        //    switch (ctrlType)
        //    {
        //        case CTRL_C_EVENT:
        //            Console.WriteLine("CTRL+C");
        //            break;
        //        case CTRL_BREAK_EVENT:
        //            Console.WriteLine("CTRL+Break, ignore.");
        //            return true;
        //        case CTRL_CLOSE_EVENT:
        //            Console.WriteLine("Close");
        //            break;
        //        case CTRL_LOGOFF_EVENT:
        //            Console.WriteLine("Log Off");
        //            break;
        //        case CTRL_SHUTDOWN_EVENT:
        //            Console.WriteLine("Shutdown");
        //            break;
        //    }
        //    return false;//忽略处理，让系统进行默认操作
        //}

        //static void MonitorInput()
        //{
        //    while (true)
        //    {
        //        string input = Console.ReadLine();
        //        if (input == "exit")
        //        {
        //            Thread.CurrentThread.Abort();
        //        }
        //        else
        //        {
        //            Console.Write("CTRL+C to Exit");
        //        }
        //    }
        //}

    }
     
}
