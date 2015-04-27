// --------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：告警发送服务
// 
// 创建标识：彭玲20140415
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System.Threading;
using FS.Service;
using log4net;

namespace FreeSun.FS_SMISCloud.Server.WarningSenderTest
{
    using System;

    /// <summary>
    /// 告警发送服务
    /// </summary>
    public class Program
    {
        static ILog log = LogManager.GetLogger("WarningSourceSimulator");
        static WarningSourceSimulator service;
        static int interval = 1;
        /// <summary>
        /// 主函数
        /// </summary>
        /// <param name="args">args</param>
        public static void Main(string[] args)
        {
            service = new WarningSourceSimulator();
            interval =  10 ;
            log.InfoFormat("Start sim, send msg per {0} sencods.", interval);
            service.Pull(OnMessageReceived);
            new Thread(DoWork).Start();
            // Start Service.
            try
            {
                new ServiceRunner(service).Run("WarningSourceSimulator");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            

            log.Debug("Done");


            #region old Test
            /**
            // Create MDSClient object to connect to DotNetMQ
            // Name of this application: WarningSenderProcess
            var mdsClient = new MDSClient("WarningSenderTest");

            // Connect to DotNetMQ server
            mdsClient.Connect();

            bool isAuto = false;
            int autoCount = 2;
            Console.WriteLine("press enter to send to DestinationApplication. Write 'exit' to stop application.");
            Console.WriteLine("输入（auto）,自动执行");
            isAuto = Console.ReadLine() == "auto";

            while (true)
            {
                if (!isAuto)
                {
                    // Get a message from user
                    string str = Console.ReadLine();
                    if (string.IsNullOrEmpty(str) || str == "exit")
                    {
                        break;
                    }
                }
                else
                {
                    if (autoCount-- <= 0)
                    {
                        break;
                    }
                }

                // Create a DotNetMQ Message to send to Application2
                IOutgoingMessage message = mdsClient.CreateMessage();

                // Set destination application name
                message.DestinationApplicationName = "WarningManagementProcess";

                // Set message data
                message.MessageData = GeneralHelper.SerializeObject(new RequestWarningReceivedMessage
                {
                    // WarningTypeId = Convert.ToString("001"), // [001~105]
                    WarningTypeId = "105",
                    StructId = 20,
                    DeviceTypeId = 2,
                    DeviceId = 150,
                    // WarningLevel = 5, // 该属性已删除
                    WarningContent = "传感器设备产生告警",
                    WarningTime = DateTime.Now

                    // WarningTime = DateTime.Parse("2014-04-18 08:01:30.123")
                    // WarningTime = Convert.ToDateTime("2014/1/1")
                    // WarningTime = DateTime.ParseExact("2014-04-17 08:00:00.001", "yyyy-MM-dd HH:mm:ss.fff", null)
                });

                // Send message
                message.Send();
            }

            // Disconnect from DotNetMQ server
            mdsClient.Disconnect();
             */

            #endregion  old Test
        }

        private static void DoWork()
        {
            int sent = 0;
            while (true)
            {
                if (service.IsWorking())
                {
                    service.Push("WarningManagementProcess", CreateAMessage());
                    sent++;
                    if (sent % 100 == 0)
                    {
                        log.InfoFormat("{0} message sent.", sent);
                    }
                }
                Thread.Sleep(interval * 1000);

            }
        }

        private static FsMessage CreateAMessage()
        {
            string text = System.IO.File.ReadAllText(@"msg.txt");
            FsMessage msg = FsMessage.FromJson(text);
            log.Debug(msg.ToJson());
            return msg;
        }

        private static void OnMessageReceived(string msg)
        {
            log.InfoFormat("[pull] OnMsgReceived: {0}", msg != null ? msg : "null");
        }
    }
}