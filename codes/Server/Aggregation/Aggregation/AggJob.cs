// // --------------------------------------------------------------------------------------------
// // <copyright file="AggJob.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150306
// //
// // 修改标识：
// // 修改描述：    
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using Quartz;

namespace Aggregation
{
    using System;
    using System.Collections.Generic;

    using Agg.Comm.DataModle;
    using log4net;
    public class AggregationJob:IJob
    {
        private static ILog log = LogManager.GetLogger("AggregationJob");
        public void Execute(IJobExecutionContext context)
        {
            IJobDetail jobDetail = context.JobDetail;
            JobInfo jobInfo = AggJobManage.GetJobInfo(jobDetail);
            if (jobInfo == null || jobInfo.DataPool == null || jobInfo.Process == null || jobInfo.ConsumerService== null)
            {
                log.InfoFormat("aggJob id:{0},start working failed, para is null.", jobDetail.Key);
                return;
            }
            log.InfoFormat("aggJob id:{0},start working sucessful!", jobDetail.Key);
            AggRawData newDatas;
            try
            {
                newDatas = jobInfo.DataPool.GetAggRawData(DateTime.Now);
            }
            catch (Exception e)
            {
                log.InfoFormat("aggJob id:{0},get data error! error:{1}, trace{2}", jobDetail.Key,e.Message,e.StackTrace);
                log.InfoFormat("aggJob id:{0},working finished!", jobDetail.Key);
                return;
            }
            log.InfoFormat("aggJob id:{0},get data finished!", jobDetail.Key);
            AggResult aggResults;
            try
            {
                aggResults = jobInfo.Process.AggProcess(newDatas);
            }
            catch (Exception e)
            {
                log.InfoFormat("aggJob id:{0},agg process error! error:{1}, trace{2}", jobDetail.Key, e.Message, e.StackTrace);
                log.InfoFormat("aggJob id:{0},working finished!", jobDetail.Key);
                return;
            }
            
            log.InfoFormat("aggJob id:{0},agg process finished!", jobDetail.Key);
            try
            {
                jobInfo.ConsumerService.OnAggResultProduced(aggResults);
            }
            catch (Exception e)
            {
                log.InfoFormat("aggJob id:{0},result consumers process error! error:{1}, trace{2}", jobDetail.Key, e.Message, e.StackTrace);
            }
            
            log.InfoFormat("aggJob id:{0},working finished!", jobDetail.Key);
        }
    }
}