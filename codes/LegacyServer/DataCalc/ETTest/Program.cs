using System;
using System.Threading;
using System.Threading.Tasks;

using MDS;
using MDS.Client;

namespace ETTest
{
    using System.IO;
    using FreeSun.FS_SMISCloud.Server.Common.Messages;

    class Program
    {
        static MDSClient mdsClient = null;
        private static void Ts()
        {
            mdsClient = new MDSClient("ExtractionTransformationProcess");
            mdsClient.Connect();

            while (true)
            {
                try
                {
                    var tmp = Console.ReadLine().ToLower().Trim();
                    var parts = tmp.Split(null);
                    var cmd = parts[0];
                    switch (cmd)
                    {
                        case "exit":
                        case "quit":
                            break;
                        case "help":
                        case "h":
                            ShowHelp();
                            break;

                        case "q":
                            if (parts.Length < 2) Console.WriteLine("need parameter");
                            DateTime paratime;
                            if (!DateTime.TryParseExact(parts[1],"yyyyMMddHHmmss",null,System.Globalization.DateTimeStyles.None, out paratime)) Console.WriteLine("error time format");
                            Send(paratime, 58, @"C:\Users\yinweiwen\Desktop\Vibration\9999_1_20140723151516925.odb", 415);
                            break;

                        case "qz1":
                            Send(DateTime.Now, 95, parts[1], 2782);
                            break;
                        case "qzall":
                            SendAll();
                            break;
                        case "circ":// 测试消息通知
                            var interval = Convert.ToInt32(parts[1]);
                            CircTest(interval);
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void CircTest(int interval)
        {
            int index = 1;
            Task t=new Task(() =>
            {
                while (true)
                {
                    var time = DateTime.Now;
                    Console.WriteLine(string.Format("发送消息{0} 时间{1}", index, time));
                    Send(time, index++, "", 0);
                    Thread.Sleep(interval);
                }
            });
            t.Start();
        }

        static void ShowHelp()
        {
            try
            {

                StreamReader sr = File.OpenText("readme.txt");
                string s = sr.ReadLine();
                while (s != null)
                {
                    Console.WriteLine(s);
                    s = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void Send(DateTime time,int structid,string filepath,int sensorid)
        {
            var tsk = new Task(() =>
            {
                var message = mdsClient.CreateMessage();
                message.DestinationApplicationName = "DataCalc";
                message.MessageData = GeneralHelper.SerializeObject(new RequestDataCalcMessage()
                {
                    StructId = structid,
                    DateTime = time,
                    FilePath=filepath,
                    SensorID=sensorid
                });

                message.TransmitRule = MDS.Communication.Messages.MessageTransmitRules.NonPersistent;
                message.Send();
                Console.WriteLine("发送消息:结构物编号{0},采集时间：{1}", structid, DateTime.Now);
            });
            tsk.Start();
            tsk.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Console.WriteLine(t.Exception.Message);
                    }
                    else
                    {
                        Console.WriteLine("done");
                    }
                });
        }

        static void SendAll()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "-calc");
            DirectoryInfo di=new DirectoryInfo(path);
            var files=di.GetFiles("4729_1_*.odb");
            foreach (var fileInfo in files)
            {
                Send(DateTime.Now, 95, fileInfo.FullName, 2144);
                Console.WriteLine(fileInfo.Name);
                Thread.Sleep(1000);
            }
        }

        private static void Main(string[] args)
        {
            Ts(); return;
            var mdsClient = new MDSClient("ET");
            mdsClient.Connect();

            bool isAuto = false;
            int autoCount = 100;
            Console.WriteLine("输入（auto）,自动执行100次");
            isAuto = Console.ReadLine() == "auto";

            while (true)
            {
                if (!isAuto)
                {
                    string str = Console.ReadLine();
                    if (str == "end")
                    {
                        break;
                    }
                }
                else
                {
                    if (autoCount -- <= 0)
                    {
                        break;
                    }
                }

                Task[] ts = new Task[100];
                for (int i = 0; i < 100; i++)
                {
                    ts[i] = new Task(() =>
                    {
                        Random ran = new Random();
                        int number = ran.Next(0, 10);

                        var message = mdsClient.CreateMessage();
                        message.DestinationApplicationName = "DataCalc";
                        message.MessageData = GeneralHelper.SerializeObject(new RequestDataCalcMessage()
                        {
                            StructId = number,
                            DateTime = DateTime.Now
                        });

                        message.TransmitRule = MDS.Communication.Messages.MessageTransmitRules.NonPersistent;
                        message.Send();
                        Console.WriteLine("发送消息:结构物编号{0},采集时间：{1}", number, DateTime.Now);
                    });                                        
                }
                
                foreach (Task task in ts)
                {
                    task.Start();
                }

                Thread.Sleep(200);
            }

            mdsClient.Disconnect();
            Console.ReadLine();
        }
    }
}
