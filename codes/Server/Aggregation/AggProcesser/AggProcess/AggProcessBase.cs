// // --------------------------------------------------------------------------------------------
// // <copyright file="AggProcessBase.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150315
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

    using Agg.Comm.DataModle;

    public abstract class AggProcessBase : IAggProcess
    {
        public virtual AggResult AggProcess(AggRawData datas)
        {
            if (datas == null || datas.Datas.Count == 0 || datas.Datas[0].Values.Count == 0) return null;

            AggResult result = new AggResult(datas.StructId, datas.FactorId, datas.TimeTag, datas.Type, datas.ConfigId);
            result.AggDatas = new List<AggData>();
            int ColNum = datas.Datas[0].Values[0].Count; // 监测数据项个数
            int id = 0;
            
            List<double> temp = new List<double>();
            foreach (var aggRawData in datas.Datas)
            {
                if (aggRawData.Values == null)
                    continue;

                id = aggRawData.SensorId;
                AggData aggData = new AggData();
                aggData.SensorId = aggRawData.SensorId;
                for (int i = 0; i < ColNum; i++)
                {
                    if (temp.Count > 0)
                    {
                        temp.Clear();
                    }
                    foreach (var value in aggRawData.Values)
                    {
                        if (i < aggRawData.Values[i].Count)
                        {
                            temp.Add(value[i]);
                        }
                    }
                    if (temp.Count > 0)
                    {
                        aggData.Values.Add( this.GetAggValue(temp));
                    }
                        
                }
                if (aggData.Values.Count > 0)
                {
                    result.AggDatas.Add(aggData);
                } 
            }
           
            result.LastAggDatas = datas.LastAggDatas;
            return result;
        }

        protected abstract double GetAggValue(List<double> data);
    }
}