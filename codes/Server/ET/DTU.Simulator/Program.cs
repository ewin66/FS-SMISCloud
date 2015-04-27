using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Util;
using System;
using System.Threading;

namespace DTU.Simulator
{
    class Program
    {
        private static byte[] result = new byte[1024];
        static DtuClient client;
        static string AutoReply = "fc0100230000010000000000000000000000000000000000000000000000a4cf";
        static void Main(string[] args)
        {
            MainArgs arg = MainArgs.ValueOf(args);
            client = new DtuClient(
                arg.Get(MainArgs.KEY_SERVER_IP, "127.0.0.1"),
                arg.GetInt(MainArgs.KEY_SERVER_PORT, 5055)
            );
            client.OnReceived += OnMsg;
            client.Connect(20120049, "18651895100", "218.3.150.108");
            Thread t = new Thread(DoWork);
            t.Start();
            t.Join();
        }
        
        public static void OnMsg(byte[] buff, int len)
        {
            Console.WriteLine("< {0}", ValueHelper.BytesToHexStr(buff, 0, len));
            Console.WriteLine("> {0}", AutoReply);
        //    client.Send(AutoReply);
        }

        public static void DoWork()
        {
            while (true)
            {
                Thread.Sleep(10);
            }
        }
    }

}
