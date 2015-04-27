// // --------------------------------------------------------------------------------------------
// // <copyright file="ReportTaskManage.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141022
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace ReportGeneratorService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;

    using Quartz;
    using Quartz.Impl;

    using ReportGeneratorService.ReportModule;

    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;

    using log4net;

    public class ReportTaskManage
    {
        private Dictionary<string, List<ReportGroup>> reportTasks = new Dictionary<string, List<ReportGroup>>();
        private List<ReportGroup> allReports = new List<ReportGroup>();

        private volatile static ReportTaskManage instance = null;
        private static readonly object lockHelper = new object();
        private IScheduler schedule;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string ParaName = "para";

        private ReportTaskManage()
        {
            Initialization();
        }

        private void Initialization()
        {
            CreateReportTask();
            CreateScheduler();
            this.CreateTaskJob();
        }

        public static ReportTaskManage CreateInstance()
        {
            if(instance == null)
            {
                lock(lockHelper)
                {
                    if (instance == null)
                    {
                        instance = new ReportTaskManage();
                    } 
                }
            }
            return instance;
        }


        private void CreateReportTask()
        {
            allReports = ReportConfigDal.GetReportConfig();
            foreach (var report in this.allReports)
            {
                if (reportTasks.ContainsKey(report.Config.CreateInterval))
                {
                    reportTasks[report.Config.CreateInterval].Add(report);
                }
                else
                {
                    reportTasks.Add(report.Config.CreateInterval, new List<ReportGroup>(){report});
                }

            }
        }

        private void CreateScheduler()
        {
            if (schedule != null) return;

            NameValueCollection props = new NameValueCollection();
            props["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            props["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";
            ISchedulerFactory sf = new StdSchedulerFactory(props);
            schedule = sf.GetScheduler();
        }

        private void CreateTaskJob()
        {
            Log.InfoFormat("共需创建{0}个定时任务:", reportTasks.Count);
            foreach (var reportTask in reportTasks)
            {
                if (!CronExpression.IsValidExpression(reportTask.Key))
                {
                    Log.InfoFormat("创建【{0}】定时任务：失败，时间表达式错误", reportTask.Key);
                    continue;
                }

                try
                {
                    IJobDetail job =
                    JobBuilder.Create<ReportTask>().WithIdentity(reportTask.Key, "Report").Build();
                    AssignJobInfo(job, reportTask.Value);
                    ITrigger trigger =
                        TriggerBuilder.Create()
                                  .WithIdentity(reportTask.Key, "Report")
                                  .WithCronSchedule(reportTask.Key)
                                  .Build();

                    schedule.ScheduleJob(job, trigger);
                    Log.InfoFormat("创建【{0}】定时任务:成功", reportTask.Key);
                }
                catch (Exception e)
                {
                    
                    //Log.InfoFormat("创建【{0}】定时任务：失败", reportTask.Key);
                    Log.Error(string.Format("创建【{0}】定时任务", reportTask.Key), e);
                    continue;
                }
                
            }
        }

        public static void AssignJobInfo(IJobDetail job, List<ReportGroup> reportHandles)
        {
            job.JobDataMap.Put(ParaName, new TaskContext { ReportHandles = reportHandles });
        }

        public static TaskContext GetJobInfo(IJobDetail job)
        {
            return (TaskContext)job.JobDataMap.Get(ParaName);
        }

        public void Start()
        {
            if (schedule.IsStarted)
                return;

            schedule.Start();
        }

        public void ReStart()
        {
            if (schedule.IsStarted)
            {
                Stop();
            }

            this.Initialization();
            this.Start();
        }

        public void Stop()
        {
            if (schedule.IsStarted)
            {
                schedule.Shutdown(true);
            }
        }
    }
}