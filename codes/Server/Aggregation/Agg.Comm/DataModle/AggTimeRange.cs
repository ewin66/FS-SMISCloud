// // --------------------------------------------------------------------------------------------
// // <copyright file="AggTimeRange.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace Agg.Comm.DataModle
{
    using System;

    [Serializable]
    public class AggTimeRange
    {
        //public string RangeType { get; set; }

        public int DataBeginHour { get; set; }

        public int DataEndHour { get; set; }

        /// <summary>
        /// 天开始时间:周聚集时表示周中的天，月聚集时表示月中的天
        /// </summary>

        public int DateBegin { get; set; }

        /// <summary>
        /// 天开始时间:周聚集时表示周中的天，月聚集时表示月中的天
        /// </summary>
        public int DateEnd { get; set; }
    }
}