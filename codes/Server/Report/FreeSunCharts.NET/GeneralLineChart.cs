// --------------------------------------------------------------------------------------------
// <copyright file="GeneralLineChart.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：通用折线图
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

using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FreeSunCharts.NET
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    public sealed class GeneralLineChart : Chart//当对一个类应用 sealed 修饰符时，此修饰符会阻止其他类从该类继承。
    {

        public override string Option { get; set; }//重写方法（图片JSON格式的配置）
        //const string Template = "template/line.json";

        
        public GeneralLineChart(string template, IEnumerable<Serie> series, string title, string yAxis)
        {
            if (!File.Exists(template))
            {
                throw new FileNotFoundException("模板文件:" + template + "不存在");
            }

            string seriesJson = JsonConvert.SerializeObject(series);

            string option = string.Empty;
            using (var fs = new FileStream(template, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    option = sr.ReadToEnd();
                    sr.Close();
                }
                fs.Close();
            }
            if (template == "template/deep_displacement.json")
            {
                //替换JSON格式的图片配置的数据列、图片名称、Y轴名称
                var xAxis = "深度(m)";
                this.Option = option
                    .Replace("$title", title ?? string.Empty)
                    .Replace("$yAxis", yAxis)
                    .Replace("$xAxis", xAxis)
                    .Replace("$series", seriesJson ?? string.Empty)
                    .Replace("\"data\":null", "\"data\":[]");
                    
            }
            else
            {
                this.Option = option
                    .Replace("$title", title ?? string.Empty)
                    .Replace("$yAxis", yAxis)
                    .Replace("$series", seriesJson ?? string.Empty)
                    .Replace("\"data\":null", "\"data\":[]");
            }

        }

        public GeneralLineChart(string template, IEnumerable<Serie> series, string title, string yAxis,string max,string min)
        {
            if (!File.Exists(template))
            {
                throw new FileNotFoundException("模板文件:" + template + "不存在");
            }

            string seriesJson = JsonConvert.SerializeObject(series);

            string option = string.Empty;
            using (var fs = new FileStream(template, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    option = sr.ReadToEnd();
                    sr.Close();
                }
                fs.Close();
            }
            if (template == "template/deep_displacement.json")
            {
                //替换JSON格式的图片配置的数据列、图片名称、Y轴名称
                var xAxis = "深度(m)";
                this.Option = option
                    .Replace("$title", title ?? string.Empty)
                    .Replace("$yAxis", yAxis)
                    .Replace("$xAxis", xAxis)
                    .Replace("$series", seriesJson ?? string.Empty)
                    .Replace("\"data\":null", "\"data\":[]");

            }
                else if (template == "template/border.json")
                {
                    this.Option = option
                   .Replace("$title", title ?? string.Empty)
                   .Replace("$yAxis", yAxis)
                   .Replace("$max", max)
                   .Replace("$min", min)
                   .Replace("$series", seriesJson ?? string.Empty)
                   .Replace("\"data\":null", "\"data\":[]");
                }
                else
                {
                    this.Option = option
                        .Replace("$title", title ?? string.Empty)
                        .Replace("$yAxis", yAxis)
                        .Replace("$series", seriesJson ?? string.Empty)
                        .Replace("\"data\":null", "\"data\":[]");
                }

        }

        public GeneralLineChart(string template, List<SerieDoubleAxis> series, string title, JArray yAxis)
        {
            if (!File.Exists(template))
            {
                throw new FileNotFoundException("模板文件:" + template + "不存在");
            }
           string[] seriesArray = new string[series.Count];
            for (int i = 0; i < series.Count; i++)
            {
                seriesArray[i] = JsonConvert.SerializeObject(series[i]);
            }
            string seriesJson = "[";

            for (int i = 0; i < seriesArray.Length; i++)
            {
                if (i > 0)
                {
                    seriesJson += ",";
                    seriesJson += seriesArray[i];
                }
                else
                {
                    seriesJson += seriesArray[i];
                }

            }
            seriesJson += "]";
            //string seriesJson = JsonConvert.SerializeObject(series);
           
            string yAxisJson = JsonConvert.SerializeObject(yAxis);
            string option;
            using (var fs = new FileStream(template, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    option = sr.ReadToEnd();
                    sr.Close();
                }
                fs.Close();
            }
            
                this.Option = option
                    .Replace("$title", title ?? string.Empty)
                    .Replace("$yAxis", yAxisJson ?? string.Empty)
                    .Replace("$series", seriesJson ?? string.Empty)
                    .Replace("\"data\":null", "\"data\":[]");
        }
    }
}