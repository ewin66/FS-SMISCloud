// --------------------------------------------------------------------------------------------
// <copyright file="WarningManagementSever.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：告警进程通信服务
// 
// 创建标识：彭玲20140415
// 
// 修改标识：彭玲20140512
// 修改描述：增加邮件预警发送功能
// 
// 修改标识：lingwenlong 20141028
// 修改描述：修改消息框架
//    增加 warningmsgs队列 _DoWork(); 和 AddWarningMsg方法；
// </summary>
// ---------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Threading;
using FS.SMIS_Cloud.Services.Messages;
using FS.SMIS_Cloud.WarningManagementProcess.DataAccess;
using Newtonsoft.Json;

namespace FreeSun.FS_SMISCloud.Server.WarningManagementProcess.Communication 
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Net.Mail;
    using System.Reflection;
    using log4net;
    using SMIS.Utils.DB;

    /// <summary>
    /// 告警管理服务
    /// </summary>
    public class WarningManagementSever
    {
        private readonly string _toClient = ConfigurationManager.AppSettings["ToClient"];
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        //private MDSClient mdsClient;
        // 实例化Timer类，设置间隔时间为30分钟；
       // private System.Timers.Timer timer = new System.Timers.Timer(1800000); // 30分钟
        private DateTime latestWarningTimeOfSupport = new DateTime(2010, 1, 1, 0, 0, 0, 0); // 2010/1/1 0:0:0.0
        private DateTime latestWarningTimeOfClient = new DateTime(2010, 1, 1, 0, 0, 0, 0); // 2010/1/1 0:0:0.0
        private ConcurrentQueue<WarnningMsg> warningmsgs = new ConcurrentQueue<WarnningMsg>();
        private CancellationTokenSource _source;
        private CancellationToken _token;

        public WarningManagementSever()
        {
            this._source = new CancellationTokenSource();
            this._token = this._source.Token;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            // Create MDSClient object to connect to DotNetMQ
            // Name of this application: WarningManagementProcess
            //this.mdsClient = new MDSClient("WarningManagementProcess"); 

            // Register to MessageReceived event to get messages.
            //this.mdsClient.MessageReceived += this.MDSClient_MessageReceived;

            try
            {
                // Connect to DotNetMQ server
                //this.mdsClient.Connect();
                this.logger.Info("告警管理服务启动成功");

                // Wait user to press enter to terminate application
                Console.WriteLine("Press enter to exit...");
                Console.WriteLine();

                // Disconnect from DotNetMQ server
                //this.mdsClient.Disconnect();
            }
            catch (Exception ex)
            {
                this.logger.Error("告警管理服务启动失败", ex);
            }
            // added by lingwenlong
            System.Threading.Tasks.Task.Factory.StartNew(this._DoWork, this._token);
            //// 到达时间的时候执行事件；
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(AccessDatabase);
            //// 设置是执行一次（false）还是一直执行(true)；    
            //timer.AutoReset = true;
            //// 是否执行System.Timers.Timer.Elapsed事件；     
            //timer.Enabled = true;

            // CheckMailSendedToSupportAndClient(); // 调试用
        }
         
         /// <summary>
         /// 循环取消息队列的内容处理
         ///  added by lingwenlong
         /// </summary>
        private void _DoWork()
        {
            while (!_source.IsCancellationRequested)
            {
                WarnningMsg msg;
                if (!this.warningmsgs.IsEmpty && this.warningmsgs.TryDequeue(out msg))
                {
                    if (msg == null)
                    {
                        continue;
                    }
                    try
                    {
                        this.MDSClient_MessageReceived(msg);
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("error  : {0}",ex.Message);
                    }
                }
                else
                {
                    // 30秒
                    Thread.Sleep(30000);
                }
            }
        }

        /// <summary>
        /// 向消息队列中添加消息
        /// added by lingwenlong
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="r"></param>
        /// <param name="request"></param>
        public void AddWarningMsg(string sender, string r, string request)
        {
            this.warningmsgs.Enqueue(new WarnningMsg
            {
                Sender = sender,
                R = r,
                Msg = request
            });
        }

         #region MQ消息接收方法

        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Message parameters</param>
        public int MDSClient_MessageReceived(WarnningMsg warningmsg)
        {
            if (warningmsg == null)
            {
                return 0;
            }
            if (warningmsg.R.EndsWith("sensor", StringComparison.OrdinalIgnoreCase) || warningmsg.R.EndsWith("grade", StringComparison.OrdinalIgnoreCase))
            {
                return this.SensorOrGradeWarningHandler(warningmsg);
            }
            else if (warningmsg.R.EndsWith("dtu", StringComparison.OrdinalIgnoreCase))
            {
                return this.DtuWarningHandler(warningmsg);
            }
            else if (warningmsg.R.EndsWith("datacontinu", StringComparison.OrdinalIgnoreCase))
            {
                return this.DataContinuWarningHandler(warningmsg);
            }
            return 0;
        }

        /// <summary>
        /// 处理传感器采集超时和评分告警
        /// </summary>
        /// <param name="warningmsg"></param>
        /// <returns></returns>
        private int SensorOrGradeWarningHandler(WarnningMsg warningmsg)
        {
            var request = JsonConvert.DeserializeObject<RequestWarningReceivedMessage>(warningmsg.Msg);
            if (request == null)
                return 0;
            try
            {
                // 检测消息中“DeviceId”是否存在于数据库，并限制告警等级
                if (!DbAccessor.Exists(request.DeviceTypeId,request.DeviceId))
                {
                    // 日志记录未正确插入数据库中的数据
                    this.logger.Info("数据库insert操作失败:");
                    string errLog =
                        string.Format(
                            "接收到的告警数据： WarningTypeId={0}, StructId={1}, DeviceTypeId={2}, DeviceId={3}, WarningContent={4}, WarningTime={5}",
                            request.WarningTypeId, request.StructId, request.DeviceTypeId, request.DeviceId,
                            request.WarningContent, request.WarningTime);
                    this.logger.Error(errLog);
                    return 0;
                }
                // 将告警消息保存至数据库
                int rowcount;
                if (_toClient=="true")
                {
                    rowcount = DbAccessor.SaveWarningMsg(request, 4, 3);
                }
                else
                {
                    rowcount = DbAccessor.SaveWarningMsg(request, 1, 1);
                }
                // rowcount = DbAccessor.SaveWarningMsg(request);
                // Process message
                Console.WriteLine();
                Console.WriteLine("Text message received : WarningTypeId={0};  WarningTime={1}",
                    request.WarningTypeId, request.WarningTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.WriteLine("Source application    : {0}", warningmsg.Sender);
                return rowcount;
            }
            catch (Exception ex)
            {
                // 日志记录未正确插入数据库中的数据
                this.logger.Info("数据库insert操作失败:");
                string errLog =
                    string.Format(
                        "接收到的告警数据： WarningTypeId={0}, StructId={1}, DeviceTypeId={2}, DeviceId={3}, WarningContent={4}, WarningTime={5}",
                        request.WarningTypeId, request.StructId, request.DeviceTypeId, request.DeviceId,
                        request.WarningContent, request.WarningTime);
                this.logger.Error(errLog, ex);
                return 0;
            }
        }

        /// <summary>
        /// 处理dtu上下线告警
        /// </summary>
        /// <param name="warningmsg"></param>
        /// <returns></returns>
        private int DtuWarningHandler(WarnningMsg warningmsg)
        {
            var request = JsonConvert.DeserializeObject<DTUStatusChangedMsg>(warningmsg.Msg);
            if (request == null)
            {
                return 0;
            }
            logger.InfoFormat("DTU{0}在 {1} {2}", request.DTUID, request.TimeStatusChanged.ToString("F"),
                request.IsOnline ? "上线" : "下线");
            int rowcount;
            if (_toClient == "true")
            {
                rowcount = DbAccessor.UpdateDtuState(request, 4, 3);
            }
            else
            {
                rowcount = DbAccessor.UpdateDtuState(request, 1, 1);
            }
            //return DbAccessor.UpdateDtuState(request);
            return rowcount;
        }

        /// <summary>
        /// 数据连续性告警
        /// </summary>
        /// <param name="warningmsg"></param>
        /// <returns></returns>
        private int DataContinuWarningHandler(WarnningMsg warningmsg)
        {
            var request = JsonConvert.DeserializeObject<DataContinuWarningMsg>(warningmsg.Msg);
            if (request == null)
            {
                return 0;
            }
            logger.InfoFormat("传感器 {0} 的数据在 {1} {2}", request.DeviceId, request.DateTime.ToString("U"),
                request.DataStatus ? "恢复" : "中断");
            int rowcount;
            if (_toClient == "true")
            {
                rowcount = DbAccessor.UpdateDataState(request,4,3);
            }
            else
            {
                rowcount = DbAccessor.UpdateDataState(request,1,1);
            }
            //return DbAccessor.UpdateDataState(request);
            return rowcount;
        }

        #endregion

        /// <summary>
        /// 定时器执行方法
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void AccessDatabase(object source, System.Timers.ElapsedEventArgs e)
        {
            // Console.WriteLine("OK!");

            //// 检测是否有未发送成功的邮件，有则重新发送邮件
            //ResendFailingMail();

            try
            {
                CheckMailSendedToSupportAndClient();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }           
        }
        
       //#region 检测是否有未发送成功的邮件，有则重新发送邮件
        ///// <summary>
        ///// 检测是否有未发送成功的邮件，有则重新发送邮件
        ///// </summary>
        //private void ResendFailingMail()
        //{
        //    // 读取文本文件
        //    string path = Environment.CurrentDirectory + @"\failingmail.txt";
        //    using (StreamReader sr = new StreamReader(path))
        //    {
        //        string str;
        //        while ((str = sr.ReadLine()) != null)
        //        {
        //            Console.WriteLine(str); 
        //        }
        //    }
        //}
        //#endregion

        #region 检测是否存在待发送至“技术支持/客户”的告警
        /// <summary>
        /// 检测是否存在待发送至“技术支持/客户”的告警
        /// </summary>
        private void CheckMailSendedToSupportAndClient()
        {
            string pathSupport = Environment.CurrentDirectory + @"\latestWarningTimeOfSupport.txt";
            string pathClient = Environment.CurrentDirectory + @"\latestWarningTimeOfClient.txt";

            // 查询数据库，检测是否有新的告警产生
            string strSelectLatestWarningTime = "select MAX(Time) WarningTime from T_WARNING_SENSOR";
            DataSet dsLatestWarningTime = DbHelperSQL.Query(strSelectLatestWarningTime);
            // 告警时间为空
            if (dsLatestWarningTime.Tables["ds"].Rows[0]["WarningTime"].ToString() == "")
            {
                logger.Info("告警表中告警时间为 null，无待发送告警邮件");
                Console.WriteLine();

                // 写入文件
                string warningTime = new DateTime(2010, 1, 1, 0, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss.fff"); // 2010-01-01 00:00:00.000  
                File.WriteAllText(pathSupport, warningTime);  
                File.WriteAllText(pathClient, warningTime);

                return;
            }

            // 读取文件
            if (!File.Exists(pathSupport))
            {
                logger.Info("未找到 latestWarningTimeOfSupport.txt 文件");
                Console.WriteLine();
            }
            else
            {
                latestWarningTimeOfSupport = DateTime.Parse(File.ReadAllText(pathSupport)); // 更新“latestWarningTime”字段
            }
            string strLatestWarningTime = Convert.ToDateTime(dsLatestWarningTime.Tables["ds"].Rows[0]["WarningTime"]).ToString("yyyy-MM-dd HH:mm:ss.fff");
            // 检测是否存在待发送至“技术支持”的告警
            CheckMailSendedToSupport(strLatestWarningTime);

            // 查询数据库，检测是否有待发送至“客户”的新告警
            string strSelectLatestWarningTimeOfClient = "select MAX(Time) WarningTime from T_WARNING_SENSOR where WarningStatus=1";
            DataSet dsLatestWarningTimeOfClient = DbHelperSQL.Query(strSelectLatestWarningTimeOfClient);  
            if (dsLatestWarningTimeOfClient.Tables["ds"].Rows[0]["WarningTime"].ToString() == "")
            {
                logger.Info("告警表中待发送至【客户】的告警时间为 null");
                Console.WriteLine();
                return;
            }
            if (!File.Exists(pathClient))
            {
                logger.Info("未找到 latestWarningTimeOfClient.txt 文件");
                Console.WriteLine();
            }
            else
            {
                latestWarningTimeOfClient = DateTime.Parse(File.ReadAllText(pathClient)); // 更新“latestWarningTime”字段
            }
            string sqlLatestWarningTimeOfClient = Convert.ToDateTime(dsLatestWarningTimeOfClient.Tables["ds"].Rows[0]["WarningTime"]).ToString("yyyy-MM-dd HH:mm:ss.fff");
            // 检测是否存在待发送至“客户”的告警
            CheckMailSendedToClient(sqlLatestWarningTimeOfClient);
        }
        #endregion

        #region 检测是否存在待发送至“技术支持”的告警
        /// <summary>
        /// 检测是否存在待发送至“技术支持”的告警。若存在，则发送告警邮件，并更新“latestWarningTime”字段
        /// </summary>
        private void CheckMailSendedToSupport(string sqlLatestWarningTime)
        {
            // 没有新的告警产生
            if (latestWarningTimeOfSupport == DateTime.Parse(sqlLatestWarningTime))
            {
                logger.Info("无待发送至【技术支持】的新告警：目前没有新告警");
                Console.WriteLine();
                return;
            }

            string strSelectSupport = "select RecieverName, RecieverMail, FilterLevel, UserNo from T_WARNING_SMS_RECIEVER where UserNo in (" 
                + "select USER_NO from T_DIM_USER_STRUCTURE where STRUCTURE_ID in (" 
                + "select distinct StructId from T_WARNING_SENSOR where WarningStatus=0 and Time>'" 
                + latestWarningTimeOfSupport.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')) and RoleId=0 and ReceiveMode='false'";
            DataSet dsSupport = DbHelperSQL.Query(strSelectSupport);
            if (dsSupport.Tables["ds"].Rows.Count == 0)
            {
                logger.Info("产生的新告警无【技术支持】接收");
                Console.WriteLine();
            }
            foreach (DataRow dr in dsSupport.Tables["ds"].Rows)
            {
                string sqlWarning = "select Id, WarningTypeId, StructId, DeviceTypeId, DeviceId, WarningLevel, Description, Content, Time"
                    + " from T_WARNING_SENSOR inner join T_DIM_WARNING_TYPE on T_WARNING_SENSOR.WarningTypeId=T_DIM_WARNING_TYPE.TypeId"
                    + " where StructId in (select STRUCTURE_ID from T_DIM_USER_STRUCTURE where USER_NO=" + dr["UserNo"] + ")"
                    + " and Time>'" + latestWarningTimeOfSupport.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and WarningStatus=0 and WarningLevel<=" + dr["FilterLevel"];                
                DataSet dsWarning = DbHelperSQL.Query(sqlWarning);         
                if (dsWarning.Tables["ds"].Rows.Count == 0)
                {
                    MailAddress rec = new MailAddress(Convert.ToString(dr["RecieverMail"]), Convert.ToString(dr["RecieverName"]));
                    logger.Info("【技术支持】" + rec + "不接收低于" + dr["FilterLevel"] + "级的告警");
                    Console.WriteLine();
                    continue;
                }
                string warningInfos = string.Empty;
                string structIds = string.Empty;
                for (int j = 0; j < dsWarning.Tables["ds"].Rows.Count; j++)
                {
                    string warningInfo = PopulateWarningInfo(dsWarning.Tables["ds"].Rows[j], "技术支持");
                    if (warningInfo == "error")
                    {
                        continue;
                    }
                    warningInfos += warningInfo + "<br />";

                    if (j == dsWarning.Tables["ds"].Rows.Count - 1)
                    {
                        structIds += dsWarning.Tables["ds"].Rows[j]["StructId"].ToString();
                    }
                    else
                    {
                        structIds += dsWarning.Tables["ds"].Rows[j]["StructId"].ToString() + ",";
                    }   
                }

                // 结构物名称
                string sqlStructNames = "select distinct STRUCTURE_NAME_CN from T_DIM_STRUCTURE where ID in (" + structIds + ")";
                DataSet dsStructName = DbHelperSQL.Query(sqlStructNames);
                string structNames = string.Empty;
                for (int k = 0; k < dsStructName.Tables["ds"].Rows.Count; k++)
                {
                    if (k == dsStructName.Tables["ds"].Rows.Count - 1)
                    {
                        structNames += dsStructName.Tables["ds"].Rows[k]["STRUCTURE_NAME_CN"].ToString();
                    }
                    else
                    {
                        structNames += dsStructName.Tables["ds"].Rows[k]["STRUCTURE_NAME_CN"].ToString() + ",";
                    }
                }

                if (warningInfos == string.Empty)
                {
                    continue;
                }
                MailAddress recipient = new MailAddress(Convert.ToString(dr["RecieverMail"]), Convert.ToString(dr["RecieverName"]));
                // 发送邮件至“技术支持”
                SendMail(recipient, structNames, warningInfos, "技术支持");
            }

            // 写入文件，更新“latestWarningTimeOfSupport.txt”中保存的最新告警时间
            string warningTime = DateTime.Parse(sqlLatestWarningTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string fileDirectory = Environment.CurrentDirectory + @"\latestWarningTimeOfSupport.txt";
            File.WriteAllText(fileDirectory, warningTime);
            Console.WriteLine("更新告警时间，写入 latestWarningTimeOfSupport.txt 文件");
            Console.WriteLine();
        }
        #endregion

        #region 检测是否存在待发送至“客户”的告警
        /// <summary>
        /// 检测是否存在待发送至“客户”的告警。若存在，则根据客户接收的告警级别发送告警邮件
        /// </summary>
        private void CheckMailSendedToClient(string sqlLatestWarningTime)
        {
            // 没有待发送至客户的新告警
            if (latestWarningTimeOfClient == DateTime.Parse(sqlLatestWarningTime))
            {
                logger.Info("无待发送至【客户】的新告警");
                Console.WriteLine();
                return;
            }

            System.Text.StringBuilder warningIds = new System.Text.StringBuilder();
            string strSelectClient = "select RecieverName, RecieverMail, FilterLevel, UserNo from T_WARNING_SMS_RECIEVER where UserNo in (" 
                + "select USER_NO from T_DIM_USER_STRUCTURE where STRUCTURE_ID in (" 
                + "select distinct StructId from T_WARNING_SENSOR where WarningStatus=1 and Time>'" + latestWarningTimeOfClient.ToString("yyyy-MM-dd HH:mm:ss.fff") 
                + "')) and RoleId=1 and ReceiveMode='false'";
            DataSet dsClient = DbHelperSQL.Query(strSelectClient);
            if (dsClient.Tables["ds"].Rows.Count == 0)
            {
                logger.Info("产生的新告警无【客户】接收");
                Console.WriteLine();
            }
            for (int i = 0; i < dsClient.Tables["ds"].Rows.Count; i++)
            {
                string sqlWarning = "select Id, WarningTypeId, StructId, DeviceTypeId, DeviceId, WarningLevel, Description, Content, Time"
                    + " from T_WARNING_SENSOR inner join T_DIM_WARNING_TYPE on T_WARNING_SENSOR.WarningTypeId=T_DIM_WARNING_TYPE.TypeId"
                    + " where StructId in (select STRUCTURE_ID from T_DIM_USER_STRUCTURE where USER_NO=" + dsClient.Tables["ds"].Rows[i]["UserNo"] + ")"
                    + " and Time>'" + latestWarningTimeOfClient.ToString("yyyy-MM-dd HH:mm:ss.fff") 
                    + "' and WarningStatus=1 and WarningLevel<=" + dsClient.Tables["ds"].Rows[i]["FilterLevel"];    
                DataSet dsWarning = DbHelperSQL.Query(sqlWarning);
                if (dsWarning.Tables["ds"].Rows.Count == 0)
                {
                    MailAddress rec = new MailAddress(Convert.ToString(dsClient.Tables["ds"].Rows[i]["RecieverMail"]), Convert.ToString(dsClient.Tables["ds"].Rows[i]["RecieverName"]));
                    logger.Info("【客户】" + rec.ToString() + "不接收低于" + dsClient.Tables["ds"].Rows[i]["FilterLevel"] + "级的告警");
                    Console.WriteLine();
                    continue;
                }

                if (i > 0 && warningIds.Length > 0)
                {
                    warningIds.Append(",");
                }

                string warningInfos = string.Empty;
                string structIds = string.Empty;
                for (int j = 0; j < dsWarning.Tables["ds"].Rows.Count; j++)
                {
                    if (j == dsWarning.Tables["ds"].Rows.Count - 1)
                    {
                        warningIds.Append(dsWarning.Tables["ds"].Rows[j]["Id"]);
                    }
                    else
                    {
                        warningIds.Append(dsWarning.Tables["ds"].Rows[j]["Id"]).Append(",");
                    }
                    // 构造告警信息
                    string warningInfo = PopulateWarningInfo(dsWarning.Tables["ds"].Rows[j], "客户");
                    if (warningInfo == "error")
                    {
                        continue;
                    }
                    warningInfos += warningInfo + "<br />";

                    if (j == dsWarning.Tables["ds"].Rows.Count - 1)
                    {
                        structIds += dsWarning.Tables["ds"].Rows[j]["StructId"].ToString();
                    }
                    else
                    {
                        structIds += dsWarning.Tables["ds"].Rows[j]["StructId"].ToString() + ",";
                    }   
                }

                // 结构物名称
                string sqlStructNames = "select distinct STRUCTURE_NAME_CN from T_DIM_STRUCTURE where ID in (" + structIds + ")";
                DataSet dsStructName = DbHelperSQL.Query(sqlStructNames);
                string structNames = string.Empty;
                for (int k = 0; k < dsStructName.Tables["ds"].Rows.Count; k++)
                {
                    if (k == dsStructName.Tables["ds"].Rows.Count - 1)
                    {
                        structNames += dsStructName.Tables["ds"].Rows[k]["STRUCTURE_NAME_CN"].ToString();
                    }
                    else
                    {
                        structNames += dsStructName.Tables["ds"].Rows[k]["STRUCTURE_NAME_CN"].ToString() + ",";
                    }
                }

                if (warningInfos == string.Empty)
                {
                    continue;
                }

                MailAddress recipient = new MailAddress(Convert.ToString(dsClient.Tables["ds"].Rows[i]["RecieverMail"]), Convert.ToString(dsClient.Tables["ds"].Rows[i]["RecieverName"]));
                // 发送邮件至“客户”
                SendMail(recipient, structNames, warningInfos, "客户");
            }

            // 写入文件，更新“latestWarningTimeOfClient.txt”中保存的最新告警时间
            string warningTime = DateTime.Parse(sqlLatestWarningTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string fileDirectory = Environment.CurrentDirectory + @"\latestWarningTimeOfClient.txt";
            File.WriteAllText(fileDirectory, warningTime);
            Console.WriteLine("更新告警时间，写入 latestWarningTimeOfClient.txt 文件");
            Console.WriteLine();

            if (warningIds.Length > 0)
            {
                try
                {
                    // 更新数据库中已向客户发送预警邮件的告警状态“WarningStatus”
                    string strUpdate = "update T_WARNING_SENSOR set WarningStatus=3 where WarningStatus=1 and Id in (" + warningIds + ")";
                    DbHelperSQL.ExecuteSql(strUpdate);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    logger.Error("异常：数据库更新“WarningStatus”操作失败");
                }   
            }  
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recipient">收件人</param>
        /// <param name="mailTitle">邮件标题</param>
        /// <param name="warningInfo">待发送的一条告警内容</param>
        /// <param name="userRole">用户角色</param>
        private void SendMail(MailAddress recipient, string mailTitle, string warningInfo, string userRole)
        {
            string messageSended = recipient.DisplayName + "您好，" + "<br />" + warningInfo;
            // 发送邮件
            MailAddress sender = new MailAddress(ConfigurationManager.AppSettings["senderAddress"], ConfigurationManager.AppSettings["senderName"]);
            string password = ConfigurationManager.AppSettings["senderPassword"];
            IList<MailAddress> recipientsList = new List<MailAddress>();
            recipientsList.Add(recipient);
            mailTitle += "有新告警产生，请注意查收";
            MailSendService.MailSendService mailSended = new MailSendService.MailSendService(sender, password, recipientsList, mailTitle, messageSended);
            MailMessage mailMessage = mailSended.GetMailMessage();
            if (userRole == "技术支持")
            {
                mailSended.SendAsync(SendCompletedToSupport, mailMessage); // 异步发送
            }
            if (userRole == "客户")
            {
                mailSended.SendAsync(SendCompletedToClient, mailMessage); // 异步发送
            }
        }
        #endregion

        #region 构成一条告警信息
        /// <summary>
        /// 构成一条告警信息
        /// </summary>
        private string PopulateWarningInfo(DataRow dr, string userRole)
        {
            // 结构物名称
            string strSelectStructName = "select top 1 STRUCTURE_NAME_CN from T_DIM_STRUCTURE where ID=" + dr["StructId"];
            DataSet dsStructName = DbHelperSQL.Query(strSelectStructName);
            string structName = dsStructName.Tables["ds"].Rows[0]["STRUCTURE_NAME_CN"].ToString();

            // 告警原因
            string sqlWarningReason = "select Reason from T_DIM_WARNING_TYPE where TypeId=" + dr["WarningTypeId"];
            DataSet dsWarningReason = DbHelperSQL.Query(sqlWarningReason);
            string warningReason = string.Empty;
            // 设备信息
            string strSelect = string.Empty;
            string deviceType = string.Empty;
            string deviceLocation = string.Empty;
            string sensorInfo = string.Empty; // 传感器归属dtu、模块号、通道号
            int deviceTypeId = Convert.ToInt32(dr["DeviceTypeId"]);
            switch (deviceTypeId)
            {
                case 1: // dtu设备 
                    strSelect = "select DESCRIPTION, REMOTE_DTU_NUMBER from T_DIM_REMOTE_DTU where REMOTE_DTU_NUMBER='" + dr["DeviceId"] + "'";
                    DataSet dsDTU = DbHelperSQL.Query(strSelect);
                    if (dsDTU.Tables["ds"].Rows.Count == 0)
                    {
                        logger.Error("不存在此‘REMOTE_DTU_NUMBER’值，" + "请检查告警表中【Id=" + dr["Id"] + "】【DeviceId】为" + dr["DeviceId"] + "的值是否合理");
                        return "error";
                    }
                    deviceLocation = dsDTU.Tables["ds"].Rows[0]["DESCRIPTION"].ToString() + dsDTU.Tables["ds"].Rows[0]["REMOTE_DTU_NUMBER"].ToString();
                    deviceType = "DTU设备";
                    break;
                case 2: // 传感器设备
                    strSelect = "select SENSOR_LOCATION_DESCRIPTION, DTU_ID, MODULE_NO, DAI_CHANNEL_NUMBER from T_DIM_SENSOR where SENSOR_ID='" + dr["DeviceId"] + "'";
                    DataSet dsSensorLocation = DbHelperSQL.Query(strSelect);
                    if (dsSensorLocation.Tables["ds"].Rows.Count == 0)
                    {
                        logger.Error("不存在此‘SENSOR_ID’值，" + "请检查告警表中【Id=" + dr["Id"] + "】【DeviceId】为" + dr["DeviceId"] + "的值是否合理");
                        return "error";
                    }
                    deviceLocation = dsSensorLocation.Tables["ds"].Rows[0]["SENSOR_LOCATION_DESCRIPTION"].ToString();
  
                    if (userRole == "技术支持")
                    {
                        strSelect = "select DESCRIPTION, REMOTE_DTU_NUMBER from T_DIM_REMOTE_DTU where ID='" + dsSensorLocation.Tables["ds"].Rows[0]["DTU_ID"] + "'";
                        DataSet dsDtu = DbHelperSQL.Query(strSelect);
                        if (dsDtu.Tables["ds"].Rows.Count > 0)
                        {
                            sensorInfo = "，传感器归属DTU：" + dsDtu.Tables["ds"].Rows[0]["DESCRIPTION"].ToString() + dsDtu.Tables["ds"].Rows[0]["REMOTE_DTU_NUMBER"].ToString() + "DTU，"
                                        + "模块号：" + dsSensorLocation.Tables["ds"].Rows[0]["MODULE_NO"].ToString() + "，通道号：" + dsSensorLocation.Tables["ds"].Rows[0]["DAI_CHANNEL_NUMBER"].ToString();
                        }                            
                    }                   
                    deviceType = "传感器设备";
                    break;
                // case 3: // 采集设备
                //    break;
                default:
                    break;     
            }
            if (userRole == "技术支持")
            {
                warningReason = "，告警原因：" + dsWarningReason.Tables["ds"].Rows[0]["Reason"].ToString();
            }
            // 告警信息
            string warningInfo = string.Empty;
            if (warningReason == "，告警原因：无" && dr["Content"].ToString() == "")
            {
                warningInfo = Convert.ToDateTime(dr["Time"]).ToString("yyyy-MM-dd HH:mm") + "，" + structName + "，" + deviceLocation
                            + deviceType + "产生" + dr["WarningLevel"].ToString() + "级告警，告警描述：" + dr["Description"] + sensorInfo + "。";
            }
            else if (warningReason == "，告警原因：无")
            {
                warningInfo = Convert.ToDateTime(dr["Time"]).ToString("yyyy-MM-dd HH:mm") + "，" + structName + "，" + deviceLocation
                            + deviceType + "产生" + dr["WarningLevel"].ToString() + "级告警，告警描述：" + dr["Description"] + "，" + dr["Content"]
                            + sensorInfo + "。";
            }
            else if (dr["Content"].ToString() == "")
            {
                warningInfo = Convert.ToDateTime(dr["Time"]).ToString("yyyy-MM-dd HH:mm") + "，" + structName + "，" + deviceLocation
                            + deviceType + "产生" + dr["WarningLevel"].ToString() + "级告警，告警描述：" + dr["Description"]  
                            + warningReason + sensorInfo + "。";
            }
            else
            {
                warningInfo = Convert.ToDateTime(dr["Time"]).ToString("yyyy-MM-dd HH:mm") + "，" + structName + "，" + deviceLocation
                            + deviceType + "产生" + dr["WarningLevel"].ToString() + "级告警，告警描述：" + dr["Description"] + "，" + dr["Content"]
                            + warningReason + sensorInfo + "。";
            }
            return warningInfo;
        }
        #endregion

        #region 异步发送邮件完成时的回调
        /// <summary>
        /// 异步发送邮件(技术支持)完成时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCompletedToSupport(object sender, AsyncCompletedEventArgs e)
        {
            MailMessage mailMessage = (MailMessage)e.UserState;
            string recipients = string.Empty;
            foreach (MailAddress to in mailMessage.To)
            {
                recipients = Convert.ToString(to) + "&";
            }
            string userToken = recipients + mailMessage.Subject + "&" + mailMessage.Body + "&&";

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] 发送被取消", userToken);
                Console.WriteLine();
            }
            if (e.Error != null)
            {
                Console.WriteLine("异步发送失败");
                Console.WriteLine("[{0}] {1}", userToken, e.Error.ToString());
                Console.WriteLine();

                // 保存发送失败邮件
                string path = Environment.CurrentDirectory + @"\failingMailToSupport.txt";
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(userToken); // 直接追加文件末尾，换行   
                }
            }
            else
            {
                Console.WriteLine("异步发送成功");
                //Console.WriteLine("userToken=" + userToken);
                Console.WriteLine("邮件接收人【技术支持】：" + mailMessage.To.ToString());
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 异步发送邮件(客户)完成时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCompletedToClient(object sender, AsyncCompletedEventArgs e)
        {
            MailMessage mailMessage = (MailMessage)e.UserState;
            string recipients = string.Empty;
            foreach (MailAddress to in mailMessage.To)
            {
                recipients = Convert.ToString(to) + "&";
            }
            string userToken = recipients + mailMessage.Subject + "&" + mailMessage.Body + "&&";

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", userToken);
                Console.WriteLine();
            }
            if (e.Error != null)
            {
                Console.WriteLine("异步发送失败");
                Console.WriteLine("[{0}] {1}", userToken, e.Error.ToString());
                Console.WriteLine();

                // 保存发送失败邮件
                string path = Environment.CurrentDirectory + @"\failingMailToClient.txt";
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(userToken); // 直接追加文件末尾，换行   
                }
            }
            else
            {
                Console.WriteLine("异步发送成功");
                //Console.WriteLine("userToken=" + userToken);
                Console.WriteLine("邮件接收人【客户】：" + mailMessage.To.ToString());
                Console.WriteLine();
            }            
        }
        #endregion
    }
}