// // --------------------------------------------------------------------------------------------
// // <copyright file="IAggResultConsumer.cs" company="江苏飞尚安全监测咨询有限公司">
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
    using System.Security.Cryptography.X509Certificates;

    using Agg.Comm.DataModle;
    using Agg.DataPool;

    public interface IAggResultConsumer
    {
        string GetConsumerName();
        bool ProcessAggResult(ref AggResult result);
    }
}