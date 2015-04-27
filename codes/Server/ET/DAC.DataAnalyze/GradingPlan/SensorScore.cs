// --------------------------------------------------------------------------------------------
// <copyright file="SensorScore.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
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
namespace FS.SMIS_Cloud.DAC.DataAnalyzer.GradingPlan
{
    public class SensorScore
    {
        public int SensorId { get; private set; }

        public int Weight { get; private set; }

        public int Score { get; private set; }

        public bool Completed { get; private set; }

        public SensorScore(int sensorId, int weight)
        {
            this.SensorId = sensorId;
            this.Weight = weight;
            this.Score = 100;
            this.Completed = false;
        }

        public void SetScore(int score)
        {
            this.Score = score;
            this.Completed = true;
        }
    }
}