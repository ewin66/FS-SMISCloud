// --------------------------------------------------------------------------------------------
// <copyright file="Data.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：数据模型
// 
// 创建标识：刘歆毅20140217
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FSDE.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 数据
    /// </summary>
    public class Data
    {
        /// <summary>
        /// 传感器ID
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// 安全监测因素
        /// </summary>
        public int SafeTypeId { get; set; }

        /// <summary>
        /// 模块号
        /// </summary>
        public string MoudleNo { get; set; }

        /// <summary>
        /// 通道号
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// 预留标记
        /// </summary>
        public object OFlag { get; set; }
        
        /// <summary>
        /// 该条数据集合
        /// </summary>
        public IList<double> DataSet { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 数据库ID
        /// </summary>
        public int DataBaseId { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public short ProjectCode { get; set; }

        /// <summary>
        /// 预留
        /// </summary>
        public object Reserve { get; set; }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>展示字符串</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(20*this.DataSet.Count);

            for (int i = 0; i < this.DataSet.Count; i++)
            {
                sb.Append("数据").Append(i + 1).Append(":");
                sb.Append(this.DataSet[i]);
                if (i != this.DataSet.Count - 1)
                {
                    sb.Append(";");
                }
            }

            return string.Format("传感器编号：{0},{1},{2}", this.SensorId, sb, this.CollectTime);
        }
    }
}