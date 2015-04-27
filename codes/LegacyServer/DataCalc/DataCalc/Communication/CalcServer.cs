// --------------------------------------------------------------------------------------------
// <copyright file="CalcServer.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：计算进程通信服务
// 
// 创建标识：刘歆毅20140217
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Threading.Tasks;

    using FreeSun.FS_SMISCloud.Server.Common.Messages;
    using FreeSun.FS_SMISCloud.Server.DataCalc.Calculation;
    using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;

    using log4net;

    using MDS;
    using MDS.Client;

    /// <summary>
    /// 二次计算通信服务
    /// </summary>
    public class CalcServer
    {
        private MDSClient mdsClient;
        private MessageProcessor msgProcessor;
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        public static object timeRecorderLock = new object();
        private FileProcessor fileProcessor;

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            new DataProcessor().ReadConfig();

            this.msgProcessor = new MessageProcessor();

            this.mdsClient = new MDSClient("DataCalc");
            // 注册消息接受事件
            this.mdsClient.MessageReceived += this.MDSClient_MessageReceived;
            // 连接到MQ
            try
            {
                this.mdsClient.Connect();
                this.logger.Info("二次计算服务启动成功");
                fileProcessor = new FileProcessor();
                fileProcessor.Start();
                this.logger.Info("文件处理线程启动");
                // 启动时进行历史数据处理
                ProcessOmittedData();
            }
            catch (Exception ex)
            {
                this.logger.Error("二次计算服务启动失败", ex);
            }
        }

        /// <summary>
        /// 处理因异常遗漏计算的数据
        /// </summary>
        private void ProcessOmittedData()
        {
            int[] structs = GetStructsList();
            if (structs == null || structs.Length == 0)
            {
                this.logger.Error("未能找到任何结构物信息");
                return;
            }
            this.logger.Debug("开始处理遗漏的计算请求");

            DateTime coltime = DateTime.Now;

            foreach (var stc in structs)
            {
                try
                {
                    ConfigHelper.GetAccFilter(stc);
                }
                catch (Exception e)
                {
                    logger.WarnFormat("结构物{0}配置存在错误:" + e.Message);
                }
            }
            // 计算任务
            Task task = new Task(() =>
            {
                foreach (int structID in structs)
                {
                    if (structID == -1 || structID == 0) continue;
                    // 查询原始数据
                    DataTable alltime = DataAccessHelper.SelectAllTime(ReadLastCalcTime(structID), structID);
                    if (alltime != null && alltime.Rows.Count > 0)
                    {
                        DataProcessor dataProcessor = new DataProcessor();

                        foreach (DataRow dr in alltime.Rows)
                        {
                            coltime = Convert.ToDateTime(dr["CollectTime"]);
                            this.logger.Debug(string.Format("处理计算遗漏数据,DTU:{0}  采集时间：{1}", structID, coltime));
                            // 查询原始数据
                            DataTable source = DataAccessHelper.GetOriginalData(coltime);
                            if (dataProcessor.ProcessRequest(structID, source, null) > 0)
                            {
                                RecordCalcTime(structID, coltime);
                            }
                        }
                    }
                }
            });
            task.Start();

            // 异步处理完成后判定异常
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    this.logger.Error("处理遗漏数据失败", t.Exception);
                }
                else
                {
                    this.logger.Info("处理遗漏数据成功");
                }
            });
        }

        /// <summary>
        /// 接收消息处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MDSClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // 获取消息内容
            var request = GeneralHelper.DeserializeObject(e.Message.MessageData) as RequestDataCalcMessage;
            if (request == null)
            {
                this.logger.Warn("消息格式错误!");
                e.Message.Acknowledge();
                return;
            }
            if (request.DateTime < DateTime.Parse("2010-1-1 0:0:0"))
            {
                this.logger.Warn("消息日期错误!");
                e.Message.Acknowledge();
                return;
            }

            this.logger.InfoFormat("接收到二次计算请求 MSGID={0},DTU={1},TIME={2}", request.Id, request.StructId, request.DateTime);

            // 调用处理消息方法
            Task task = this.msgProcessor.ProcessRequestAsyn(request, this.SendRequestScoreMessage);
            task.Start();

            // 异步处理完成后判定异常
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    this.logger.Error("二次计算失败", t.Exception);
                }
                else
                {
                    RecordCalcTime(request.StructId, request.DateTime);
                }
            });
            e.Message.Acknowledge();
        }

        /// <summary>
        /// 发送请求评分消息
        /// </summary>
        /// <param name="request">请求列表</param>
        private void SendRequestScoreMessage(IEnumerable<RequestScoreMessage> request)
        {
            foreach (RequestScoreMessage requestScoreMessage in request)
            {
                var reqScoreMsg = this.mdsClient.CreateMessage();
                reqScoreMsg.MessageData = GeneralHelper.SerializeObject(requestScoreMessage);
                reqScoreMsg.TransmitRule = MDS.Communication.Messages.MessageTransmitRules.NonPersistent;
                reqScoreMsg.DestinationApplicationName = "Score";

                reqScoreMsg.Send();
            }
        }

        private void RecordCalcTime(int structId, DateTime time)
        {
            if (structId < 1) return;
            try
            {
                lock (timeRecorderLock)
                {
                    ConfigHelper.SetRecordTime(structId, time);
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(string.Format("写入时间记录时发生错误，当前结构物ID={0}, {1}", structId, ex.Message));
            }
        }

        private DateTime ReadLastCalcTime(int structId)
        {
            try
            {
                return ConfigHelper.GetRecordTime(structId);
            }
            catch (System.Exception ex)
            {
                logger.Error(string.Format("读取时间记录时发生错误，当前结构物ID={0}, {1}", structId, ex.Message));
                return DateTime.Now;
            }
        }

        private int[] GetStructsList()
        {
            return ConfigHelper.GetStructs();
        }
    }
}