// // --------------------------------------------------------------------------------------------
// // <copyright file="AggDataStorage.cs" company="江苏飞尚安全监测咨询有限公司">
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
    using Agg.Comm.DataModle;
    using Agg.DataPool;
    using Agg.Storage;

    using log4net;

    public class AggDataStorage:IAggResultConsumer
    {
        private static ILog Log = LogManager.GetLogger("AggDataStorage");

        public string GetConsumerName()
        {
            return this.GetType().Name;
        }

        public bool ProcessAggResult(ref AggResult result)
        {
            if (result == null || result.AggDatas.Count == 0)
            {
                Log.Info("agg data storage failed, para is null!");
                return false;
            }

            SeclureCloudDbHelper dbHelper = SeclureCloudDbHelper.Instance();

            if (dbHelper == null)
            {
                Log.Info("agg data storage failed, dbhelper is null!");
                return false;
            }

            Log.InfoFormat("struct:{0},factorId:{1},type:{2}, statrt data storage...", result.StructId, result.SafeFactorId, result.AggType);
            if (dbHelper.Accessor.SaveAggResult(result) > 0)
            {
                Log.InfoFormat("struct:{0},factorId:{1},type:{2}, data storage sucessful!", result.StructId, result.SafeFactorId, result.AggType);
                return true;
            }
            else
            {
                Log.InfoFormat("struct:{0},factorId:{1},type:{2}, data storage failed!", result.StructId, result.SafeFactorId, result.AggType);
                return false;
            }
        }
    }
}