// --------------------------------------------------------------------------------------------
// <copyright file="GradingPlan.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：结构物评分计划
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

    using FS.SMIS_Cloud.NGET.DataAnalyzer.Accessor;

    public class StructGradingPlan
    {
        public int OrgStcId { get; private set; }

        public List<ThemeScore> ThemeScores;

        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 构建结构物评分计划
        /// </summary>
        /// <param name="orgStcId">结构物编号</param>
        public StructGradingPlan(int orgStcId)
        {
            this.CreateTime = DateTime.Now;
            this.OrgStcId = orgStcId;
            this.ThemeScores = new List<ThemeScore>();
            var themes = DbAccessor.GetThemeWeightByOrgStc(orgStcId);
            foreach (var dataRow in themes.AsEnumerable())
            {
                this.ThemeScores.Add(new ThemeScore(orgStcId, Convert.ToInt32(dataRow["ThemeId"]), Convert.ToInt32(dataRow["Weight"])));
            }
        }

        /// <summary>
        /// 添加评分项
        /// </summary>
        /// <param name="item">评分项</param>
        public void AddGradingItem(GradingItem item)
        {
            var factorId = Convert.ToInt32(DbAccessor.GetSensorInfo(item.SensorId).Rows[0]["FactorId"]);
            var themeId = Convert.ToInt32(DbAccessor.GetSafetyFactorInfo(factorId).Rows[0]["ParentId"]);
            var themeScore = this.ThemeScores.FirstOrDefault(f => f.ThemeId == themeId);
            if (themeScore != null)
            {
                themeScore.UpdateSensorScores(item);
            }

            if (this.ThemeScores.All(f => f.Completed))
            {
                var stcScore = 100;
                foreach (var score in this.ThemeScores)
                {
                    stcScore -= (int)((100 - score.Score) * (score.Weight / 100.0));
                }

                DbAccessor.SaveStructureScore(this.OrgStcId, stcScore, DateTime.Now);

                StructGradingPlanSet.CompletePlan(this);
            }
        }

        /// <summary>
        /// 强制计算结构物评分
        /// </summary>
        public void EnforceGrading()
        {
            foreach (var unGraded in this.ThemeScores.Where(f=>!f.Completed))
            {
                unGraded.EnforceGrading();
            }

            var stcScore = 100;
            foreach (var themeScore in this.ThemeScores)
            {
                if (themeScore.Completed)
                {
                    stcScore -= (int)((100 - themeScore.Score) * (themeScore.Weight / 100.0));
                }
            }

            DbAccessor.SaveStructureScore(this.OrgStcId, stcScore, DateTime.Now);

            StructGradingPlanSet.CompletePlan(this);
        }
    }
}