// --------------------------------------------------------------------------------------------
// <copyright file="AlarmService.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20140902
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FS.SMIS_Cloud.NGET
{
    using System;
    using System.Configuration;

    using FS.Service;
    using log4net;

    using FS.SMIS_Cloud.Services.ConsoleCtrlManager;

    public class NGEtService :   Service
    {
        private const string MyName = "nget";
        private static readonly ILog Log = LogManager.GetLogger("NGET");
        private readonly MsgDealHelper _msgHelper = new MsgDealHelper();

        private readonly EtController etController;
        public NGEtService(string svrConfig, string busPath)
            : base(MyName, svrConfig, busPath)
        {
            string cs = ConfigurationManager.AppSettings["SecureCloud"];
            Log.InfoFormat("DBConnecting: {0}", cs);
            GlobalConfig.ConnectionString = cs;

            string ds = ConfigurationManager.AppSettings["DataSourcePath"];
            Log.InfoFormat("DataSourcePath: {0}", ds);
            GlobalConfig.DataSourcePath = ds;

            string ep = ConfigurationManager.AppSettings["ErrorFilePath"];
            Log.InfoFormat("ErrorFilePath: {0}", ep);
            GlobalConfig.ErrorFilePath = ep;

            string pp = ConfigurationManager.AppSettings["ParsedFilePath"];
            Log.InfoFormat("ParsedFilePath: {0}", pp);
            GlobalConfig.ParsedFilePath = pp;

            if (ConfigurationManager.AppSettings["FileScanInterval"] != null)
            {
                GlobalConfig.FileScanInterval = int.Parse(ConfigurationManager.AppSettings["FileScanInterval"]) * 1000;
            }
            Log.InfoFormat("FileScanInterval: {0}", GlobalConfig.FileScanInterval);

            etController = new EtController(this);

            ConsoleCtrlManager.Instance.RegCmd(
                "close",
                s =>
                    {
                        this.InstanceExit();
                        return null;
                    });
            ConsoleCtrlManager.Instance.Exit += InstanceExit;
        }

        void InstanceExit()
        {
            try
            {
                etController.Exit();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Log.Error("closing throw an exception", e);
                Console.WriteLine("closing throw an exception");
                Environment.Exit(-1);
            }
        }

        public void OnSubscribed(string msg)
        {
        }

        public override void PowerOn()
        {
            Log.Info("PowerOn");
            etController.DoTimerWork();
        }

        public void OnMessageReceived(string buff)
        {
            try
            {
                FsMessage msg = FsMessage.FromJson(buff);
                Log.InfoFormat("pull({0})", buff);
                _msgHelper.DealMsg(msg, this);
            }
            catch (Exception ce)
            {
                Log.ErrorFormat("err {0}!", ce.Message);
            }
        }

        public void Push(FsMessage msg)
        {
            if (msg != null)
            {
                msg.Header.S = MyName;
                Push(msg.Header.D, msg);
            }
        }

        public string ResponseHandler(string msg)
        {
            Log.InfoFormat("Response({0})", msg);
            return _msgHelper.DealMsg(msg, this);
        }
    }
}