// // --------------------------------------------------------------------------------------------
// // <copyright file="TimeStrategy.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150317
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace Agg.DataPool
{
    using System;

    using Agg.Comm.DataModle;
    using Agg.Comm.Util;

    internal class TimeStrategy
    {
        public static DateTime GetAggDate(AggType type, AggTimeRange timeRange,  DateTime nowTime)
        {
            DateTime date;
            int day;
            switch (type)
            {
                case AggType.Day:
                    date = timeRange.DataEndHour < nowTime.Hour ? nowTime : nowTime.AddDays(-1);
                    break;
                case AggType.Week:
                    day = DateTimeHelper.GetDayOfWeekByWeekFirstDayMon(nowTime);
                    if((day > timeRange.DateEnd) || (day == timeRange.DateEnd && timeRange.DataEndHour < nowTime.Hour))
                    {
                        date = nowTime;
                    }
                    else
                    {
                        date = nowTime.AddDays(-7);
                    }
                    break;
                case AggType.Month:
                    day = nowTime.Day;
                    if((day > timeRange.DateEnd) || (day == timeRange.DateEnd && timeRange.DataEndHour < nowTime.Hour))
                    {
                        date = nowTime;
                    }
                    else
                    {
                        date = nowTime.AddMonths(-1);
                    }
                    break;
                default:
                    date = nowTime;
                    break;
            }
           
            return date;
        }
    }
}