// --------------------------------------------------------------------------------------------
// <copyright file="Serie.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：序列
// 
// 创建标识：liuxinyi20140529
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FreeSunCharts.NET
{
    using Newtonsoft.Json;

    public class Serie
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public double[][] Data { get; set; }
    }
    public class SerieDoubleAxis
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("yAxis")]
        public int YAxis { get; set; }

        [JsonProperty("data")]
        public double[][] Data { get; set; }
    }
}