// --------------------------------------------------------------------------------------------
// <copyright file="Alarm.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：阈值告警信息
// 
// 创建标识：20141030
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.DAC.DataAnalyzer.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class ThresholdAlarm
    {
        public int SensorId { get; private set; }

        public IList<ThresholdAlarmDetail> AlarmDetails { get; set; }

        public ThresholdAlarm(int sensorId)
        {
            this.SensorId = sensorId;
            this.AlarmDetails = new List<ThresholdAlarmDetail>();
        }

        public override string ToString()
        {
            return this.AlarmDetails != null && this.AlarmDetails.Count > 0
                       ? string.Join(";", this.AlarmDetails.Select(detail => detail.ToString()))
                       : string.Empty;
        }
    }

    public class ThresholdAlarmDetail
    {
        public string ItemName { get; private set; }

        public int ThresholdLevel { get; private set; }

        /// <summary>
        /// 告警消息格式:默认为 {0}超过{1}级阈值 {0}:监测项名称,{1}:阈值等级
        /// </summary>
        public string AlarmContentFormatter { get; set; }

        public ThresholdAlarmDetail(string itemName, int thresholdLevel)
        {
            this.ItemName = itemName;
            this.ThresholdLevel = thresholdLevel;
            this.AlarmContentFormatter = "{0}超过{1}级阈值";
        }

        public override string ToString()
        {
            return string.Format(this.AlarmContentFormatter, this.ItemName, this.ThresholdLevel);
        }
    }
}