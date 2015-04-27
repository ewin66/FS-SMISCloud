// // --------------------------------------------------------------------------------------------
// // <copyright file="MonthDataDBDataPool.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150312
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
    using System.Collections.Generic;
    using System.Text;

    using Agg.Comm.DataModle;
    using Agg.Comm.Util;

    internal class MonthDataDBDataPool : DBDataPoolBase
    {
        public MonthDataDBDataPool()
        {
           
        }

        public override AggRawData GetAggRawData(DateTime nowTime)
        {
            if (base.DbHelper == null) return null;

            ///获取配置信息副本
            //if (!rwLocker.TryEnterReadLock(TimeOut))
            //    return null;
            //BaseAggConfig configTmp = ObjectHelper.DeepCopy(this.Config);     
            //rwLocker.ExitReadLock();

            DateTime lastMonth = TimeStrategy.GetAggDate(config.Type,config.TimeRange, nowTime);
            int year = lastMonth.Year;
            int month = lastMonth.Month;
            int beginDay;
            int endDay;

            if (config.TimeRange.DateBegin == -1 || config.TimeRange.DateBegin > DateTime.DaysInMonth(year, month))
            {
                beginDay = DateTimeHelper.GetLastDayOfMonth(lastMonth);
            }
            else
            {
                beginDay = config.TimeRange.DateBegin;
            }


            if (config.TimeRange.DateEnd == -1 || config.TimeRange.DateEnd > DateTime.DaysInMonth(year,month))
            {
                endDay = DateTimeHelper.GetLastDayOfMonth(lastMonth);
            }
            else
            {
                endDay = config.TimeRange.DateEnd;
            }


            DateTime beginTime = new DateTime(year,month,beginDay,0,0,0);
            DateTime endTime = new DateTime(year, month, endDay, 23, 59, 59);

            AggRawData data = new AggRawData(config.Key, config.ConfigId, GetTimeFlg(lastMonth));
            List<int> SensorIds = config.GetSensorIds();
            foreach (int sensorId in SensorIds)
            {
                RawData tempRawData = DbHelper.Accessor.GetMonthAggRawData(
                    sensorId,
                    config.FactorId,
                    config.TimeRange,
                    beginTime,
                    endTime);
                if (tempRawData != null && tempRawData.Values.Count > 0)
                {
                    data.Datas.Add(tempRawData);
                }
                
            }
            data.LastAggDatas = base.GetLastAggData();
            return data;

        }

        //public virtual List<AggData> GetLastAggData()
        //{
        //    return base.GetLastAggData();
        //}

        protected override string GetTimeFlg(DateTime time)
        {
            return time.ToString("yyyyMM");
        }

       

    }
}