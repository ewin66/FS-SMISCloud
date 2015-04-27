// // --------------------------------------------------------------------------------------------
// // <copyright file="DataPoolFactory.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150309
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
    using System.Collections.Concurrent;  
    using System.Collections.Generic;

    using Agg.Comm.DataModle;

    using log4net;

    public abstract class DataPoolFactory
    {
        private static string ConfigFileName = AppDomain.CurrentDomain.BaseDirectory + " \\DataPool.xml";

        private static Dictionary<string, Type> dataPoolTypes = new Dictionary<string, Type>(); 
       
        private static ILog Log = LogManager.GetLogger("DataPoolFactory");
        public static void Init()
        {
            dataPoolTypes = DataPoolConfig.FromXML(ConfigFileName);
        }

        public static IDataPool GetDataPool(BaseAggConfig config)
        {
            IDataPool iDataPool = null;
            if (!dataPoolTypes.ContainsKey(config.Type.ToString()))
            {
                return null;
            }

            Type type = dataPoolTypes[config.Type.ToString()];

            try
            {

                iDataPool = Activator.CreateInstance(type) as IDataPool;
                if (iDataPool != null) iDataPool.Config = config;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("create aggprocess failed, type:{0},error:{1},stacktrace{2}", config.Type.ToString(), e.Message, e.StackTrace);
            }
            return iDataPool;
        }
    }
}