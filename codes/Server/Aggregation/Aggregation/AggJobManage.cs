// // --------------------------------------------------------------------------------------------
// // <copyright file="AggJobManage.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150309
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace Aggregation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Agg.Comm.DataModle;
    using Agg.DataPool;
    using Agg.Process;

    using Quartz;
    using Quartz.Impl;

    using log4net;

    public class AggJobManage
    {
        Dictionary<AggTaskKey, JobInfo> TaskInfo = new Dictionary<AggTaskKey, JobInfo>();
        Dictionary<AggTaskKey, IJobDetail> AllJobs = new Dictionary<AggTaskKey, IJobDetail>(); 
        private IScheduler schedule;
        private const string ParaName = "para";

        private static ILog log = LogManager.GetLogger("AggJobManage");
        public AggJobManage(List<BaseAggConfig> aggConfigs)
        {
            ProcessFactory.Init();
            DataPoolFactory.Init();
            schedule = CreateScheduler();
            CreateJobs(aggConfigs);

        }

        private IScheduler CreateScheduler()
        {
            IScheduler schedule;
            NameValueCollection props = new NameValueCollection();
            props["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            props["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";
            ISchedulerFactory sf = new StdSchedulerFactory(props);
            schedule = sf.GetScheduler();
            return schedule;
        }

        private IJobDetail CreateNewJob(string key, JobInfo info, string cronTime)
        {
            IJobDetail job = JobBuilder.Create<AggregationJob>().WithIdentity(key, "Agg").Build();
            AssignJobInfo(job, info);
            ITrigger trigger =
                TriggerBuilder.Create()
                            .WithIdentity(key, "Agg")
                            .WithCronSchedule(cronTime)
                            .Build();

            schedule.ScheduleJob(job, trigger);

            return job;
        }

        private void CreateJobs(List<BaseAggConfig> aggConfigs)
        {
            int failed = 0;
            foreach (var config in aggConfigs)
            {
                try
                {
                    JobInfo info = new JobInfo
                    {
                        ConsumerService = AggResultConsumerService.Instance(),
                        DataPool = DataPoolFactory.GetDataPool(config),
                        Process = ProcessFactory.GetAggProcess(config.Way)
                    };
                    IJobDetail job = this.CreateNewJob(config.Key.ToString(), info, config.TimingMode);
                    TaskInfo.Add(config.Key, info);
                    AllJobs.Add(config.Key, job);
                }
                catch (Exception e)
                {
                    log.WarnFormat("job create failed,key:{0},error:{1}",config.Key.ToString(),e.Message);
                    failed++;
                    continue;
                }
            }
            log.InfoFormat("create {0} jobs finished,successful:{1},failed:{2}", aggConfigs.Count, aggConfigs.Count-failed,failed);
        }

        

        public static void AssignJobInfo(IJobDetail job, JobInfo info)
        {
            job.JobDataMap.Put(ParaName, info);
        }

        public static JobInfo GetJobInfo(IJobDetail job)
        {
            return (JobInfo)job.JobDataMap.Get(ParaName);
        }

        public void StartWork()
        {
            if (schedule.IsStarted)
                return;

            schedule.Start();
            log.InfoFormat("start working...");
        }
        public void StopWork()
        {
            if (!schedule.IsStarted && !schedule.IsShutdown) return;
 
            schedule.Shutdown(true);
            log.InfoFormat("stop working...");
            //schedule.Clear();
        }

        public void ReStart(List<BaseAggConfig> aggConfigs)
        {
            schedule = CreateScheduler();
            TaskInfo.Clear();
            AllJobs.Clear();
            CreateJobs(aggConfigs);
            StartWork();
        }
    }
}