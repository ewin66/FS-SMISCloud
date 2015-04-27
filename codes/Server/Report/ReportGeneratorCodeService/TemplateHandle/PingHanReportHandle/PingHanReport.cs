/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：PingHanReport.cs
// 功能描述：
// 
// 创建标识： 2015/1/22 11:14:50
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
// </summary>

//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Cells;
using log4net;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class PingHanReport
    {
        /// <summary>
        /// 验证报表参数的合法性
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="factorId"></param>
        /// <param name="template"></param>
        /// <param name="logger"></param>
        public static void CheckParamValid(int structId, int factorId, string template, ILog logger)
        {
            if (0 == structId)
            {
                throw new ArgumentNullException(structId.ToString(), "结构物id异常");
            }
            if (0 == factorId)
            {
                throw new ArgumentNullException(factorId.ToString(), "监测因素id异常");
            }
            if (!File.Exists(template))
            {
                logger.Warn("模版文件 : " + template + "未找到");
                throw new FileNotFoundException("模版文件 : " + template);
            }
        }

        /// <summary>
        /// 获取目标报表文件工作簿及文件流
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="ms"></param>
        /// <param name="md"></param>
        /// <returns>目标文件存在返回true 否则返回false</returns>
        public static bool GetTargetWorkbook(string targetFile, out FileStream ms, out Workbook md)
        {
            bool ExistOrNot;
            //读取目标（如果不存在则创建，否则直接使用）                      
            if (!File.Exists(targetFile))
            {
                ExistOrNot = false;
                ms = new FileStream(targetFile, FileMode.Create, FileAccess.Write);
                md = new Workbook(ms);
            }
            else
            {
                ExistOrNot = true;
                ms = new FileStream(targetFile, FileMode.Open, FileAccess.ReadWrite);
                md = new Workbook(ms);
            }
            return ExistOrNot;
        }
        /// <summary>
        /// 获取报表模板
        /// </summary>
        /// <param name="template"></param>
        /// <param name="ww">根据报表模板创建的工作簿</param>
        public static void GetTemplate(string template, out Workbook ww)
        {
            using (var fs = new FileStream(template, FileMode.Open, FileAccess.Read))
            {
                ww = new Workbook(fs);
                fs.Close();
            }
        }
        /// <summary>
        /// 设置sheet页名称
        /// </summary>
        /// <param name="sectionSensors"></param>
        /// <param name="workbook"></param>
        /// <param name="factorName"></param>
        /// <returns>工作簿</returns>
        public static Workbook SetSheetName(List<SectionSensors> sectionSensors, Workbook workbook, string factorName)
        {
            Workbook ww = workbook;
            ww.Worksheets[0].Name = (sectionSensors[0].SectionName ?? "断面" + (0 + 1)) + factorName;
            for (int i = 1; i < sectionSensors.Count; i++)
            {
                var index = ww.Worksheets.AddCopy(0);
                ww.Worksheets[index].Name = (sectionSensors[i].SectionName ?? "断面" + (i + 1)) + factorName;
            }
            return ww;
        }
        /// <summary>
        /// 拷贝sheet页
        /// </summary>
        /// <param name="ExistOrNot"></param>
        /// <param name="sectionSensors"></param>
        /// <param name="factorName"></param>
        /// <param name="ww"></param>
        /// <param name="md"></param>
        public static void CopyExcelSheet(bool ExistOrNot, List<SectionSensors> sectionSensors, string factorName, ref Workbook ww, ref  Workbook md)
        {

            if (ExistOrNot)
            {
                for (int i = 0; i < ww.Worksheets.Count; i++)
                {
                    string tmpname = sectionSensors[i].SectionName ?? "断面" + (0 + 1);
                    tmpname += factorName;
                    md.Worksheets.RemoveAt(tmpname);
                    Worksheet tmp = md.Worksheets.Add(ww.Worksheets[i].Name);
                    tmp.Copy(ww.Worksheets[i]);
                }
            }
            else
            {
                for (int i = 0; i < ww.Worksheets.Count; i++)
                {
                    if (i == 0)
                    {
                        md.Worksheets[i].Copy(ww.Worksheets[i]);
                        md.Worksheets[i].Name = ww.Worksheets[i].Name;
                    }
                    else
                    {
                        Worksheet tmp = md.Worksheets.Add(ww.Worksheets[i].Name);
                        tmp.Copy(ww.Worksheets[i]);
                    }
                }
            }

        }
        /// <summary>
        /// 获取给定日期的上一周每天日期
        /// </summary>
        /// <param name="str">代表星期几的字符串</param>
        /// <param name="dt">给定日期</param>
        /// <returns>待查询的上星期几的日期</returns>
        public static DateTime GetLastWeekEachDay(string str, DateTime dt)
        {
            DateTime date = DateTime.Now;
            int xq = Convert.ToInt32(dt.DayOfWeek);

            switch (str)
            {
                case "1":
                    date = dt.Date.AddDays(-xq).AddDays(-6).Date.ToLocalTime();

                    break;
                case "2":
                    date = dt.Date.AddDays(-xq).AddDays(-5).Date.ToLocalTime();
                    break;
                case "3":
                    date = dt.Date.AddDays(-xq).AddDays(-4).Date.ToLocalTime();
                    break;
                case "4":
                    date = dt.Date.AddDays(-xq).AddDays(-3).Date.ToLocalTime();
                    break;
                case "5":
                    date = dt.Date.AddDays(-xq).AddDays(-2).Date.ToLocalTime();
                    break;
                case "6":
                    date = dt.Date.AddDays(-xq).AddDays(-1).Date.ToLocalTime();
                    break;
                case "7":
                    date = dt.Date.AddDays(-xq).Date.ToLocalTime();
                    break;
            }
            return date;
        }
        /// <summary>
        /// 获取上周起止日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<DateTime> GetDatePeriod(DateTime dt)
        {
            List<DateTime> DatePeriod = new List<DateTime>();
            DateTime monday = GetLastWeekEachDay("1", dt);
            DateTime sunday = GetLastWeekEachDay("7", dt);
            DatePeriod.Add(monday);
            DatePeriod.Add(sunday);
            return DatePeriod;
        }


        /// <summary>
        /// 获取上周每天的日期列表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<DateTime> GetLastWeekDateList(DateTime dt)
        {
            List<DateTime> DateList = new List<DateTime>();
            for (int i = 1; i < 8; i++)
            {
                DateTime date = GetLastWeekEachDay(i.ToString(), dt);
                DateList.Add(date);
            }
            return DateList;
        }
        /// <summary>
        /// 获取一天的前12小时及后12小时监测数据的平均值
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="sensors"></param>
        /// <param name="getFirstData">输出参数: 前12小时监测数据的平均值</param>
        /// <param name="getSencondData">输出参数: 后12小时监测数据的平均值</param>
        public static void GetFirstAndSecondMinitorData(DateTime startTime, int[] sensors, out Dictionary<string, MonitorData> getFirstData, out Dictionary<string, MonitorData> getSencondData)
        {
            var getFirstStartDate = startTime;
            var getFirstEndDate = getFirstStartDate.AddHours(12);
            getFirstData = DataAccess.GetAvgData(sensors, getFirstStartDate, getFirstEndDate);
            var getSecondStartDate = getFirstEndDate;
            var getSecondEndDate = getFirstStartDate.AddDays(1).AddMilliseconds(-1);
            getSencondData = DataAccess.GetAvgData(sensors, getSecondStartDate, getSecondEndDate);
        }
        /// <summary>
        /// 填充测量值的整数部分和小数部分
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        public static void FillDataCrackReport(Worksheet sheet, MonitorData data, decimal? len0, int rowIndex, int columnIndex)
        {
            bool flag = false;
            if (data.Data != null)
            {
                if (data.Data[0].Values.Any())
                {
                    var data1 = ( len0 * 1000 - data.Data[0].Values[0]) / 1000;
                    var data2 = ( len0 * 1000 - data.Data[0].Values[0]  ) % 1000;

                    sheet.Cells[rowIndex, columnIndex].PutValue(Math.Floor(Convert.ToDecimal(data1)));
                    sheet.Cells[rowIndex, columnIndex + 1].PutValue(Convert.ToDecimal(Convert.ToDecimal(data2).ToString("#0")));
                }
                else
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                sheet.Cells[rowIndex, columnIndex].PutValue("/");
                sheet.Cells[rowIndex, columnIndex + 1].PutValue("/");
            }
        }
        /// <summary>
        /// 获取测量值（累计量）
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="key"></param>
        /// <param name="desData"></param>
        public static void GetMonitorData(Dictionary<string, MonitorData> sourceData, string key, ref MonitorData desData)
        {
            if (sourceData.Any())
            {
                desData = sourceData.ContainsKey(key) ? sourceData[key] : new MonitorData();
            }
        }
    }
}
