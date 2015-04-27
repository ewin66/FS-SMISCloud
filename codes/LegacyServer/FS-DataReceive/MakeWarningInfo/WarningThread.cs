using System;
using FreeSun.FS_SMISCloud.Server.Common.Messages;

namespace MakeWarningInfo
{
    using System.Configuration;
    using log4net;
    using MDS;
    using MDS.Client;

    public class WarningThread
    {
        private static MDSClient mdsClient;

        readonly ILog log = LogManager.GetLogger(typeof(WarningThread));
        WarningThread()
        {
            //mdsClient = new MDSClient("WirelessReceiver");
            //mdsClient.Connect();
            //Console.WriteLine("启动服务");
        }

        private static WarningThread warningThread;

        private static object obj = new object();

        private static readonly string WarningAppName = ConfigurationManager.AppSettings["ProcessCalcu"];

        public static WarningThread GetWarningThread()
        {
            if (warningThread == null)
            {
                lock (obj)
                {
                    if (warningThread == null)
                    {
                        warningThread =new WarningThread();;
                    }
                }
            }

            return warningThread;
        }

        private bool isConnect = false;

        public void Start()
        {
            mdsClient = new MDSClient("WirelessReceiver");
            try
            {
                mdsClient.Connect();
                Console.WriteLine("启动服务");
                isConnect = true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Console.WriteLine("启动服务失败");
            }
        }

        public void SendWarningProcessMessage(RequestDataCalcMessage request)
        {
            if (!isConnect)
            {
                this.Start();
            }
                var reqWarnMsg = mdsClient.CreateMessage();
                reqWarnMsg.MessageData = GeneralHelper.SerializeObject(request);
                reqWarnMsg.TransmitRule = MDS.Communication.Messages.MessageTransmitRules.NonPersistent;
                reqWarnMsg.DestinationApplicationName = WarningAppName;
                reqWarnMsg.Send();
                log.Info("成功发送告警");
            
        }
    }
}
