// // --------------------------------------------------------------------------------------------
// // <copyright file="AggResultConsumerService.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150318
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

    using Agg.DataPool;

    using log4net;
    using Agg.Comm.DataModle;
    using Agg.Comm.Util;
    public class AggResultConsumerService
    {
        private Dictionary<string, Type> Consumers;

        public List<IAggResultConsumer> ConsumerQueues = new List<IAggResultConsumer>();
        private static string ConfigFileName = AppDomain.CurrentDomain.BaseDirectory + " \\ResultConsumers.xml";
        private static ILog Log = LogManager.GetLogger("AggResultConsumerService");

        private volatile static AggResultConsumerService _instance = null;
        private static readonly object lockHelper = new object();

        private AggResultConsumerService()
        {
            Init();
        }

        private void Init()
        {
            Consumers = ConsumerConfig.FromXML(ConfigFileName);
            foreach (var consumer in Consumers)
            {
                RegisterConsumer(consumer.Value);
            }
        }

        public static AggResultConsumerService Instance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        try
                        {
                            _instance = new AggResultConsumerService();
                        }
                        catch (Exception)
                        {
                            Log.ErrorFormat("AggResultConsumerService初始化失败");
                        }
                    }
                }
            }
            return _instance;
        }

        private bool RegisterConsumer(Type type)
        {
            IAggResultConsumer consumer;
            try
            {
                consumer = Activator.CreateInstance(type) as IAggResultConsumer;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("create consumer failed, name:{0},error:{1},stacktrace{2}", type.Name, e.Message, e.StackTrace);
                return false;
            }

            if (consumer != null)
            {
                ConsumerQueues.Add(consumer);
            }
            return true;
        }

        public void OnAggResultProduced(AggResult result)
        {
            if (result == null || result.AggDatas.Count == 0)
            {
                Log.Info("has no new agg data, agg consumers process finished!");
                return;
            }
            Log.InfoFormat("struct:{0},factorId:{1},type:{2}, agg consumers process start...", result.StructId, result.SafeFactorId, result.AggType);
            AggResult resultCopy = ObjectHelper.DeepCopy(result);
            foreach (var Consumer in ConsumerQueues)
            {
                bool ret = Consumer.ProcessAggResult(ref resultCopy);

                if (!ret)
                {
                    Log.ErrorFormat("{0},Process aggresult failed!", Consumer.GetConsumerName());
                }
            }
            Log.InfoFormat("struct:{0},factorId:{1},type:{2}, agg consumers process finished", result.StructId, result.SafeFactorId, result.AggType);
        }
    }
}