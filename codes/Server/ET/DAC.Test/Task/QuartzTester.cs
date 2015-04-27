using System;
using System.Collections.Specialized;
using System.Threading;
using log4net;
using NUnit.Core;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;

namespace DAC.Test.Task
{
   
    [TestFixture]
    public class QuartzTester
    {
        private StdSchedulerFactory _factory;
        private static IScheduler _schedule;
        internal static ILog Log = LogManager.GetLogger("TaskManager");

        [Test]
        public void TestExecute()
        {
            CreateSchedule();
            ArrangeJob("Job1", 1);
           // ArrangeJob("Job2", 5);
            _schedule.Start();
            Thread.Sleep(20000);
            
            _schedule.Shutdown();

        }

        internal class DacJob : IJob
        {
            private static int _sequence=0;
            private int id = _sequence++;
            private static ILog log = QuartzTester.Log;
            public void Execute(IJobExecutionContext context)
            {
                string name = GetJobInfo(context.JobDetail);
                //log.DebugFormat("Schedule: {0}", _schedule.Context.Count);
                log.DebugFormat("{0}-{1} - Executing {2}", name, id, context.JobDetail.Key);
                Thread.Sleep(10000);
                log.DebugFormat("{0}-{1} - Done", name, id);
            }
        }

        public void ArrangeJob(string jobName, int seconds)
        {
            // 安排2个任务.
            string jobGroup = "Node";
            Log.DebugFormat("Arrange Job: {0}, {1} ", jobName, seconds);
            // 任务
            IJobDetail job = JobBuilder.Create<DacJob>()
                .WithIdentity(jobName, jobGroup)
                .Build();

            AssignJobInfo(job, jobName);

            // 调度策略
            DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 2);
            ISimpleTrigger trigger = (ISimpleTrigger) TriggerBuilder.Create()
                .WithIdentity(jobName, jobGroup)
                .WithSimpleSchedule(x => x.WithInterval(new TimeSpan(0, 0, seconds)).RepeatForever())
                .StartAt(startTime)
                .Build();
            _schedule.ScheduleJob(job, trigger);
        }

        private static void AssignJobInfo(IJobDetail job, string val)
        {
            job.JobDataMap.Put("INFO", val);
        }

        private static string GetJobInfo(IJobDetail job)
        {
            return (string)job.JobDataMap.Get("INFO");
        }

        private void CreateSchedule()
        {
            if (_schedule == null || _schedule.IsShutdown)
            {
                NameValueCollection props = new NameValueCollection();
                //简单线程池管理.
                props["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool,Quartz";
                //内存存储
                props["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore,Quartz";
                props["quartz.threadPool.threadCount"] = "1000";// 线程池数量.
                _factory = new StdSchedulerFactory(props);
                _schedule = _factory.GetScheduler();
            }
        }
    }
}
