// // --------------------------------------------------------------------------------------------
// // <copyright file="AggData.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace Agg.Comm.DataModle
{
    using System;
    using System.Collections.Generic;
    [Serializable]
    public class AggData
    {
        public int SensorId { get; set; }
        public List<double> Values { get; set; }

        public AggData()
        {
            Values = new List<double>();
        }
    }
}