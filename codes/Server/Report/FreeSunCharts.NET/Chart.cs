// --------------------------------------------------------------------------------------------
// <copyright file="Chart.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：图表基类
// 
// 创建标识：liuxinyi20140606
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
    using System.IO;

    using HightChartsGenerator.NET;

    public abstract class Chart
    {
        public abstract string Option { get; set; }

        public Stream Draw(out string[] output)
        {
            return HighchartsConvert.GenerateViaOption(this.Option, out output);//将Option里面的JSON格式的图片配置文件传参进去，返回输出内存流格式的图片
        }
    }
}