
using System;
using System.Diagnostics;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.TemplateHandle;

namespace ReportGeneratorService
{
    using System.Reflection;
    using System.Threading;

    using log4net;

    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static AutoResetEvent autoReset = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            Log.Debug("Start....");
            SetTimedTask();
            ReportTaskManage.CreateInstance().Start();
            //  Console.ReadKey();//出现线程死锁
            autoReset.WaitOne();
            Log.Debug("Exit....");
        }

        private static void SetTimedTask()
        {
            System.Timers.Timer t = new System.Timers.Timer(1000 * 1000);//实例化Timer类，设置间隔 1分钟;
            t.Elapsed += new System.Timers.ElapsedEventHandler(GetMemory);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        private static void GetMemory(object source, System.Timers.ElapsedEventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            Log.Debug(string.Format("进程标识： {0} , 进程名称： {1}, 占用内存： {2}", currentProcess.Id.ToString(), currentProcess.ProcessName, (currentProcess.WorkingSet64 / 1024).ToString() + "  KB"));
        }
    }
}
