// --------------------------------------------------------------------------------------------
// <copyright file="AverageArithmetic.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：均值去除跳变算法
// 
// 创建标识：刘歆毅20140219
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Arithmetic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
    using FreeSun.FS_SMISCloud.Server.DataCalc.Model;

    using log4net;

    public class AverageArithmetic : IArithmetic
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        // 历史数据缓存
        private static Dictionary<int, SortedList<DateTime, Data>> cache = new Dictionary<int, SortedList<DateTime, Data>>();

        public void Calculate(Dictionary<int, Data> rawData)
        {

        }
        public bool Initial()
        {
            return true;
        }
        /// <summary>
        /// 实现计算算法接口
        /// </summary>
        /// <param name="rawData">要计算的原始数据</param>
        /// <returns>计算后的数据</returns>
        public IList<Data> Calculate(IList<Data> rawData)
        {
            IList<Data> rslt = new List<Data>();

            foreach (Data data in rawData)
            {
                Data value = this.GetAverageValue(data);
                rslt.Add(value);
            }

            return rslt;
        }

        /// <summary>
        /// 计算传感器的平均值
        /// </summary>
        /// <param name="raw">传感器的当前采集值</param>
        /// <returns>均值</returns>
        private Data GetAverageValue(Data raw)
        {
            // 提取历史数据
            IList<Data> histData = this.GetHistoricalData(raw);
            histData.Add(raw);

            Data value = new Data
            {
                SensorId = raw.SensorId,
                CollectTime = raw.CollectTime,
                DataSet = new List<double>()
            };
            // 平均
            for (int i = 0; i < histData[0].DataSet.Count; i++)
            {
                value.DataSet.Add(histData.Average(v => v.DataSet[i]));
            }

            return value;
        }

        /// <summary>
        /// 提取历史数据
        /// </summary>
        /// <param name="raw">当前数据</param>
        /// <returns>历史数据</returns>
        private IList<Data> GetHistoricalData(Data raw)
        {
            IList<Data> histData;            

            lock(cache)
            {
                if (!cache.ContainsKey(raw.SensorId))
                {
                    histData = this.GetHistoricalDataFromDB(raw);
                    // 添加到缓存
                    cache.Add(raw.SensorId, new SortedList<DateTime, Data>());
                    int i = 0;
                    foreach (Data data in histData)
                    {
                        if (++i > 50)
                        {
                            break;
                        }
                        cache[raw.SensorId].Add(data.CollectTime, data);
                    }
                }
                else // 存在缓存
                {
                    histData = cache[raw.SensorId].Select(s => s.Value).ToList(); // 从缓存中取数据
                    while (cache[raw.SensorId].Count >= 50)
                    {
                        cache[raw.SensorId].RemoveAt(0);
                    }
                    if (!cache[raw.SensorId].ContainsKey(raw.CollectTime))
                    {
                        cache[raw.SensorId].Add(raw.CollectTime, raw);
                    }
                }
            }

            return histData;
        }

        /// <summary>
        /// 从数据库提取最近24小时数据
        /// </summary>
        /// <param name="raw">当前数据</param>
        /// <returns>数据集合</returns>
        private IList<Data> GetHistoricalDataFromDB(Data raw)
        {
            IList<Data> histData = new List<Data>();
            DataTable historicalTable = DataAccessHelper.GetETData(raw.SensorId, raw.CollectTime.AddHours(-24),
                raw.CollectTime);
            if (historicalTable != null)
            {
                foreach (DataRow src in historicalTable.AsEnumerable())
                {
                    Data data = new Data()
                    {
                        SensorId = src.Field<int>("SensorId"),
                        DataSet = new List<double>(),
                        CollectTime = src.Field<DateTime>("Time")
                    };

                    for (int i = 1; i < src.ItemArray.Length - 1; i++)
                    {
                        if (src.ItemArray[i] == DBNull.Value || src.ItemArray[i] == null)
                        {
                            break;
                        }
                        data.DataSet.Add(Convert.ToDouble(src.ItemArray[i]));
                    }
                    histData.Add(data);
                }
            }

            return histData;
        }
    }
}