#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="PortManager.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140606 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System.Linq;
using System.Text;
using log4net;

namespace FSDE.Core
{
    using System;
    using System.Configuration;
    using System.Threading;

    using Communication;

    using FSDE.Commn.CheckMode;
    using FSDE.Dictionaries.config;

    internal class PortManager
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ExtractionManager));
        private static PortManager portManager = null;

        private static object obj = new object();

        public static PortManager GetPortManager()
        {
            if (null == portManager)
            {
                lock (obj)
                {
                    if (null == portManager)
                    {
                        portManager = new PortManager();
                    }
                }
            }
            return portManager;
        }

        private ICommunicationBase comport;
        private PortManager()
        {
            SerialPortCreator creator =new SerialPortCreator();
            comport = creator.CreateCommunicationFactory(ProjectInfoDic.GetInstance().GetProjectInfo().TargetName, new int[] { 9600,0,8,1,100 });
        }

        private static bool isReady = false;

        public static Timer heartbeatTimer;

        private static bool isResponseheartbeat = false;

        private int heartbeatInterval;

        private int outofheartbeartCount = 0;

        public void StartService()
        {
            comport.StartService();
            comport.ReceivedDataEventHandler += comport_ReceivedDataEventHandler;
            comport.RequestOrResponseEventHandler += comport_RequestOrResponseEventHandler;
        }
        
        /// <summary>
        /// 当前状态请求响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comport_RequestOrResponseEventHandler(object sender, System.EventArgs e)
        {
            //Console.WriteLine("收到返回信息");
           
            //CompackageEventArgs cArgs = e as CompackageEventArgs;
            
            //switch (cArgs.DataReceived[4])
            //{
            //    case 0:
            //    case 1:
            //    case 2:
            //        isReady = true;
            //        isResponseheartbeat = true;
            //        log.Info("收到DTU在线回应！");
            //        break;
            //}
        }

        /// <summary>
        /// 接收数据(可以考虑将配置从云平台同步到统一采集软件)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comport_ReceivedDataEventHandler(object sender, System.EventArgs e)
        {
            // throw new System.NotImplementedException();
            Console.WriteLine("收到返回信息");
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public bool SendData(byte[] dataBytes)
        {
            if (true)
            {
                log.Info("进入发送函数SendData(byte[] dataBytes)");
                return comport.Send(dataBytes);
            }

            return false;
        }

        public void SendSatateBytes()
        {
            isReady = false;
            log.Info("进入发送心跳函数SendSatateBytes");
            this.comport.Send(this.CreateSatateBytes(0));
            log.Info("离开发送心跳函数SendSatateBytes");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte[] CreateSatateBytes(byte i)
        {
            log.Info("进入函数CreateSatateBytes");
            byte[] packBytes =new byte[7];
            packBytes[0] = 0xfe;
            packBytes[1] = 0xef;
            Array.Copy(
                BitConverter.GetBytes(Convert.ToInt16(ProjectInfoDic.GetInstance().GetProjectInfo().ProjectCode)),
                0,
                packBytes,
                2,
                2);
            packBytes[4] = i;
            byte[] crc16 = CheckModeResult.GetCheckResult(packBytes, 0, 3, CheckType.CRC16HighByteFirst);
            packBytes[5] = crc16[0];
            packBytes[6] = crc16[1];

            StringBuilder temp = new StringBuilder();
            for (int j = 0; j < 7; j++)
            {
                temp.Append(packBytes[j]);
            }
            log.Info("离开函数CreateSatateBytes：" + temp.ToString());
            return packBytes;
        }
    }
}