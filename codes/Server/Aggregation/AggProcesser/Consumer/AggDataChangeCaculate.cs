// // --------------------------------------------------------------------------------------------
// // <copyright file="AggDataChangeCaculate.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace Agg.Process
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    using Agg.Comm.DataModle;
    using Agg.DataPool;
    using log4net;
    using log4net.Appender;

    public class AggDataChangeCaculate:IAggResultConsumer
    {
        private const double defalultValue = 0.0d;
        private static ILog Log = LogManager.GetLogger("AggDataChangeCaculate");

        public string GetConsumerName()
        {
            return this.GetType().Name;
        }

        public bool ProcessAggResult(ref AggResult result)
        {
            if (result == null)
            {
                Log.Info("agg data change caculate failed, para is null!");
                return false;
            }

            Log.InfoFormat("struct:{0},factorId:{1},type:{2}, statrt AggDataChangeCaculate...", result.StructId, result.SafeFactorId,result.AggType);

            List<AggData> lastAggDatas = result.LastAggDatas;
            result.AggDataChanges = new List<AggData>();
            foreach (AggData aggData in result.AggDatas)
            {
                AggData lasAggRawData;
                AggData dataChange = new AggData();
                dataChange.SensorId = aggData.SensorId;
                ///存在上次聚集数据
                if (GetLastAggData(aggData.SensorId, lastAggDatas, out lasAggRawData))
                {
                    
                    for (int i = 0; i < aggData.Values.Count; i++)
                    {
                        if (i < lasAggRawData.Values.Count)
                        {
                            dataChange.Values.Add(aggData.Values[i] - lasAggRawData.Values[i]);
                        }
                        else
                        {
                            dataChange.Values.Add(defalultValue);
                        }
                        
                    }
                }
                else /// 无上次聚集数据
                {

                    for (int i = 0; i < aggData.Values.Count; i++)
                    {
                        dataChange.Values.Add(defalultValue);
                    }
                    Log.InfoFormat("sensorid:{0},timetag:{1},type:{2}, has no last agg data",aggData.SensorId,result.TimeTag,result.AggType);
                }
                result.AggDataChanges.Add(dataChange);
            }
            Log.InfoFormat("struct:{0},factorId:{1},type:{2}, end AggDataChangeCaculate...", result.StructId, result.SafeFactorId, result.AggType);
            return true;
        }


        /// <summary>
        /// 获取最近一次聚集数据
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="lastDatas"></param>
        /// <param name="lastAggData"></param>
        /// <returns></returns>
        private bool GetLastAggData(int sensorId, List<AggData> lastDatas, out AggData lastAggData)
        {
            lastAggData = null;
            if (lastDatas == null) return false;

            try
            {
                lastAggData = (from data in lastDatas where data.SensorId == sensorId select data).ToList().First();
                return true;
            }
            catch (Exception)
            {
                return false;
                
            }

        }
    }
}