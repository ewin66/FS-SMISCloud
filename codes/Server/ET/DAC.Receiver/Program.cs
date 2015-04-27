using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Tran;
using log4net;

namespace DAC.Receiver
{
    class Program
    {
        private static ILog Log = LogManager.GetLogger("DAC.Receiver");
        static void Main(string[] args)
        {
            int port = 6066;
            if (args.Length >= 1)
            {
                port = Convert.ToInt32(args[0]);
            }
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            Log.InfoFormat("DAC.Receiver({0}) started, listening on {1}", fvi.FileVersion, port);
            TranDataReceiver r = new TranDataReceiver(new GprsDtuServer(port));
            r.Start();
            r.OnClientConnected += (IDtuConnection c, WorkingStatus s) =>
            {
                GprsDtuConnection c2 = (GprsDtuConnection) c;
                Log.InfoFormat("[Server] Client connected: {0}-ip={1},phone={2}", c2.DtuID, c2.IP, c2.PhoneNumber);
            };
            r.OnTranMsgReceived += (TranMsgType type, TranMsg msg) =>
            {
                TranMsg m2 = msg;
                Log.InfoFormat("[Server] Msg Received: id={3}, type={0},len={1}, pkg={2}", type, msg.LoadSize,
                    msg.PackageCount, msg.ID);
            };
            while (true)
            {
                string read = Console.ReadLine();
                if (read == "exit")
                {
                    break;
                }
            }
        }
    }
}
