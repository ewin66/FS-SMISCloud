// // --------------------------------------------------------------------------------------------
// // <copyright file="JobInfo.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150306
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace Aggregation
{
    using System.Collections.Generic;

    using Agg.DataPool;
    using Agg.Process;
    public class JobInfo
    {
        public IAggProcess Process { get; set; }

        public IDataPool DataPool { get; set; }

        public AggResultConsumerService ConsumerService { get; set; }
    }
}