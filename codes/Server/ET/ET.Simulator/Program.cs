using FS.Service;
using log4net;
using System;
using System.Diagnostics;
using System.Threading;

namespace ET.Simulator
{
    class Program
    {
        static ILog log = LogManager.GetLogger("ET.Sim");
        static ETSimulator service;
        static int interval = 1;
        static void Main(string[] args)
        {
            try
            {
                service = new ETSimulator();
                interval = args.Length >= 1 ? Convert.ToInt16(args[0]) : 1;
                log.InfoFormat("Start sim, send msg per {0} sencods.", interval);
                service.Pull(OnMessageReceived);
            }
            catch (Exception ex)
            {
                log.ErrorFormat(ex.Message);
            }
            

            new Thread(DoWork).Start();
            // Start Service.
            new ServiceRunner(service).Run("ET.Simulator");

            log.Debug("Done");
        }
        static bool msgReceived = true;
        private static void OnMessageReceived(string msg)
        {
            msgReceived = true;
            log.InfoFormat("[pull] OnMsgReceived: {0}", msg != null ? msg : "null");
        }

        public static void DoWork()
        {
            int sent = 0;
            while (true)
            {
                if (msgReceived && service.IsWorking())
                {
                    msgReceived = false;
                    service.Push("et", CreateAMessage());
                    sent++;
                    if (sent % 100 == 0)
                    {
                        log.InfoFormat("{0} message sent.",sent);
                    }
                }
                Thread.Sleep(interval*1000); 
            
            }
        }

        static FsMessage CreateAMessage(){
            string text = System.IO.File.ReadAllText(@"msg.txt");
            FsMessage msg =  FsMessage.FromJson(text);
            log.Debug(msg.ToJson());
            return msg;
        }



        // 获得为该进程(程序)分配的内存. 做一个计时器,就可以时时查看程序占用系统资源   

        public double GetProcessUsedMemory()
        {

            double usedMemory = 0;

            usedMemory = Process.GetCurrentProcess().WorkingSet64 / 1024.0 / 1024.0;

            return usedMemory;

        }  

    }
}
