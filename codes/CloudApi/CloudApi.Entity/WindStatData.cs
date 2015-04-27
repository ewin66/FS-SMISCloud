// --------------------------------------------------------------------------------------------
// <copyright file="WindStatData.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：风玫瑰图统计数据模型
// 
// 创建标识：20140305
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity
{
    /// <summary>
    /// 风玫瑰统计
    /// </summary>
    public class WindStatData
    {
        /// <summary>
        /// 风向
        /// </summary>
        public string Direct { get; set; }

        /// <summary>
        /// 1段风速比率
        /// </summary>
        public decimal Percent1 { get; set; }

        /// <summary>
        /// 2段风速比率
        /// </summary>
        public decimal Percent2 { get; set; }

        /// <summary>
        /// 3段风速比率
        /// </summary>
        public decimal Percent3 { get; set; }

        /// <summary>
        /// 4段风速比率
        /// </summary>
        public decimal Percent4 { get; set; }

        /// <summary>
        /// 5段风速比率
        /// </summary>
        public decimal Percent5 { get; set; }

        /// <summary>
        /// 6段风速比率
        /// </summary>
        public decimal Percent6 { get; set; }

        /// <summary>
        /// 7段风速比率
        /// </summary>
        public decimal Percent7 { get; set; }

        /// <summary>
        /// 方向总比率
        /// </summary>
        public decimal TotalPercent { get; set; }
    }
}