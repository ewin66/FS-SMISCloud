// // --------------------------------------------------------------------------------------------
// // <copyright file="DayDataDBDataPool.cs" company="江苏飞尚安全监测咨询有限公司">
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

    using Agg.Comm.DataModle;
    using Agg.Comm.Util;

    internal class DayDataDBDataPool : DBDataPoolBase
    {
        public DayDataDBDataPool()
        {

        }

        public override AggRawData GetAggRawData(DateTime nowTime)
        {
            if (base.DbHelper == null) return null;

            //if(!rwLocker.TryEnterReadLock(TimeOut))
            //    return null;
            //BaseAggConfig configTmp = ObjectHelper.DeepCopy(this.Config);
            //rwLocker.ExitReadLock();

            DateTime lastDay = TimeStrategy.GetAggDate(config.Type, config.TimeRange, nowTime);

            DateTime beginTime = lastDay.Date;
            DateTime endTime = lastDay.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            AggRawData data = new AggRawData(config.Key,config.ConfigId, GetTimeFlg(lastDay));
            List<int> SensorIds = config.GetSensorIds();
            foreach (int sensorId in SensorIds)
            {
                RawData tempRawData = DbHelper.Accessor.GetDayAggRawData(
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
            string timeFlg = time.ToString("yyyyMMdd");
            return timeFlg;
        }
    }
}