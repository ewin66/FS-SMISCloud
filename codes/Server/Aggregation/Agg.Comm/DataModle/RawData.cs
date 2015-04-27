// // --------------------------------------------------------------------------------------------
// // <copyright file="RawData.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150313
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
    using System.Collections.Generic;

    public class RawData
    {
        public int SensorId { get; set; }
        public List<List<double>> Values { get; set; }

        public RawData()
        {
            this.Values = new List<List<double>>();
        }
    }
}