// // --------------------------------------------------------------------------------------------
// // <copyright file="ReportTask.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141021
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System.Reflection;

namespace ReportGeneratorService
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    using Quartz;
    using log4net;

    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.Interface;

    

    public class ReportTask : IJob
    {
        private CancellationTokenSource source = new CancellationTokenSource();

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationToken token; 

        private TaskFactory factory;

        public ReportTask()
        {
            this.token = this.source.Token;
            factory = new TaskFactory(source.Token);
        }

        public void Execute(IJobExecutionContext context)
        {
            IJobDetail jobDetail = context.JobDetail;
            TaskContext taskContext = ReportTaskManage.GetJobInfo(jobDetail);
            List<Task<ReportTaskResult>> tasks = new List<Task<ReportTaskResult>>();
            DateTime nowTime = DateTime.Now;
            foreach (var reportTask in taskContext.ReportHandles)
            {
                reportTask.CreateDate = nowTime;
                ReportFileBase file = new MonitorReportFile(reportTask);

                if (file.CreateNewFile != null)
                {
                    tasks.Add(factory.StartNew(file.CreateNewFile, token));
                }
                
            }
          

            factory.ContinueWhenAll(tasks.ToArray(), (results) =>
                {
                    foreach (var t in tasks)
                    {
                        if (t.Result.Result == Result.Successful)
                        {
                            logger.Debug(string.Format("{0}..{1}..start save", t.Result.ReportInfo.Id, t.Result.ReportInfo.Name));
                            UpdateReportCollect(t.Result.ReportInfo);
                            logger.Debug("end save");
                        }
                    }
                });
        }

        private static void UpdateReportCollect(ReportInfo reportInfo)
        {
            ReportConfigDal.SaveReportInfo(reportInfo);
        }
    }
}