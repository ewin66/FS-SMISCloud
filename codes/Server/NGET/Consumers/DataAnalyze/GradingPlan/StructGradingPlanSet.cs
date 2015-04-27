// --------------------------------------------------------------------------------------------
// <copyright file="GradingPlanDetails.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：结构物评分计划集合
// 
// 创建标识：20141110
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET.DataAnalyzer.GradingPlan
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;

    using FS.SMIS_Cloud.NGET.DataAnalyzer.Accessor;

    using log4net;

    public class StructGradingPlanSet
    {
        private static ILog Log = LogManager.GetLogger("GradingPlanSet");

        private static Timer Timer = new Timer(o => CleanUpTimeOutPlan(), null, 0, -1);

        private readonly static object lockObj = new object();

        public static List<StructGradingPlan> Plans = new List<StructGradingPlan>();

        /// <summary>
        /// 更新评分计划
        /// </summary>
        /// <param name="item">评分项</param>
        public static void UpdatePlan(GradingItem item)
        {
            DataTable orgStc;
            try
            {
                orgStc = DbAccessor.GetOrgStcByStruct(item.StructId);
            }
            catch (Exception e)
            {
                Log.Error("query org-struct by grading item failed", e);
                return;
            }

            foreach (var dataRow in orgStc.AsEnumerable())
            {
                var orgStcId = Convert.ToInt32(dataRow["OrgStcId"]);
                lock (lockObj)
                {
                    if (Plans.Any(p => p.OrgStcId == orgStcId))
                    {
                        try
                        {
                            Plans.First(p => p.OrgStcId == orgStcId).AddGradingItem(item);
                            Log.DebugFormat(
                                "org-struct:{0} update score success, item:sen-{1},stc:{2},fac={3},score={4}",
                                orgStcId,
                                item.SensorId,
                                item.StructId,
                                item.FactorId,
                                item.Score);
                        }
                        catch (Exception e)
                        {
                            Log.ErrorFormat(
                                "org-struct:{0} update score error, item:sen-{1},stc:{2},fac={3},score={4}",
                                orgStcId,
                                item.SensorId,
                                item.StructId,
                                item.FactorId,
                                item.Score);
                        }
                    }
                    else
                    {
                        try
                        {
                            var plan = new StructGradingPlan(orgStcId);
                            plan.AddGradingItem(item);
                            Plans.Add(plan);
                            Log.DebugFormat(
                                "org-struct:{0} build plan success, create time:{1}",
                                plan.OrgStcId,
                                plan.CreateTime);
                        }
                        catch (Exception e)
                        {
                            Log.ErrorFormat("org-struct:{0} build plan error", e, orgStcId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除评分计划
        /// </summary>
        /// <param name="plan"></param>
        public static void CompletePlan(StructGradingPlan plan)
        {
            lock (lockObj)
            {
                Plans.Remove(plan);
            }
            Log.DebugFormat("org-struct:{0} grade completed!", plan.OrgStcId);
        }

        /// <summary>
        /// 清理超时计划
        /// </summary>
        private static void CleanUpTimeOutPlan()
        {
            var outTime = DateTime.Now.AddHours(-1);
            //var outTime = DateTime.Now.AddMinutes(-1);
            lock (lockObj)
            {
                for (int i = 0; i < Plans.Count; i++)
                {
                    var plan = Plans[i];
                    if (plan.CreateTime <= outTime)
                    {
                        plan.EnforceGrading();
                        Log.DebugFormat("org-struct:{0} plan timeout, has been cleaned up", plan.OrgStcId);
                    }
                }
            }
            Timer.Change(10000, -1);
        }
    }
}