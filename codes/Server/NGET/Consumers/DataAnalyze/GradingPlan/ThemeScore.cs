// --------------------------------------------------------------------------------------------
// <copyright file="SafetyFactorScore.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：监测因素评分项
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

    public class ThemeScore
    {
        private List<SensorScore> sensorScores = new List<SensorScore>();

        public int OrgStcId { get; private set; }

        public int ThemeId { get; private set; }

        public int Weight { get; private set; }

        public int Score { get; private set; }

        public bool Completed { get; private set; }

        public ThemeScore(int orgStcId, int themeId, int weight)
        {
            this.OrgStcId = orgStcId;
            this.ThemeId = themeId;
            this.Weight = weight;
            var sensors = DbAccessor.GetSensorWeightByOrgStc(orgStcId, themeId);
            foreach (var dataRow in sensors.AsEnumerable())
            {
                this.sensorScores.Add(
                    new SensorScore(Convert.ToInt32(dataRow["SensorId"]), Convert.ToInt32(dataRow["Weight"])));
            }
        }

        public void UpdateSensorScores(GradingItem item)
        {
            var sensorScore = this.sensorScores.FirstOrDefault(s => s.SensorId == item.SensorId);
            if (sensorScore != null)
            {
                sensorScore.SetScore(item.Score);
            }

            if (this.sensorScores.All(s => s.Completed))
            {
                this.Score = 100;
                foreach (var score in this.sensorScores)
                {
                    this.Score -= (int)((100 - score.Score) * (score.Weight / 100.0));
                }

                DbAccessor.SaveThemeScore(this.OrgStcId, this.ThemeId, this.Score, DateTime.Now);

                this.Completed = true;
            }
        }

        public void EnforceGrading()
        {
            this.Score = 100;
            foreach (var score in this.sensorScores)
            {
                this.Score -= (int)((100 - score.Score) * (score.Weight / 100.0));
            }

            DbAccessor.SaveThemeScore(this.OrgStcId, this.ThemeId, this.Score, DateTime.Now);

            this.Completed = true;
        }
    }
}