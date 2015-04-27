/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：SettlementWeeklyReport.cs
// 功能描述： 拱顶沉降报表处理类
// 
// 创建标识： 2015/1/22 9:21:05
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
using System.Net.NetworkInformation;
using System.Reflection;
using Aspose.Cells;
using log4net;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class PhCjWeeklyReport : TemplateHandleBase
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool ExistOrNot { get; set; }
        private int structId { get; set; }
        private int factorId { get; set; }
        private string factorName { get; set; }
        private DateTime dtTime { get; set; }
        private string template { get; set; }
        private string targetFile { get; set; }

        public PhCjWeeklyReport(TemplateHandlerPrams para)
            : base(para)
        {
            init();
           PingHanReport.CheckParamValid(structId, factorId, template, logger);

        }

        private void init()
        {
            structId = TemplateHandlerPrams.Structure.Id;
            factorId = TemplateHandlerPrams.Factor.Id;
            factorName = TemplateHandlerPrams.Factor.NameCN;
            dtTime = TemplateHandlerPrams.Date;
            ExistOrNot = false;
            template = TemplateHandlerPrams.TemplateFileName;
            targetFile = TemplateHandlerPrams.FileFullName;
        }      
        public override void WriteFile()
        {
            //读取目标
            Workbook md;// 目标Workbook
            FileStream ms;// 目标文件流
            ExistOrNot =  PingHanReport.GetTargetWorkbook(targetFile,out ms, out md);
            // 读取模版
            Workbook ww;
           PingHanReport.GetTemplate(template, out ww);
            // 获取测量时段
            List<DateTime> datePeriod = PingHanReport.GetDatePeriod(dtTime);

            // 获取隧道截面
            List<SectionSensors> sectionSensors = new List<SectionSensors>();
            try
            {
                sectionSensors = DataAccess.GetSectionSensors(structId, factorId);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                ms.Close();
                throw ex;
            }
            // 复制sheet
            if (sectionSensors.Count > 0)
            {
                try
                {
                    ww = PingHanReport.SetSheetName(sectionSensors, ww,factorName);

                    for (int i = 0; i < sectionSensors.Count; i++)
                    {
                        var sheet = ww.Worksheets[i];
                        var chart = sheet.Charts[0];
                        chart.Title.Text = ww.Worksheets[i].Name + "累计变化趋势"; 
                        sheet.Cells[1, 1].PutValue(sectionSensors[i].SectionName);
                        var sectionName = sheet.Cells[4, 0].StringValue;
                        sheet.Cells[4, 0].PutValue(sectionName.Replace("[sectionName]", sectionSensors[i].SectionName));

                        var date = sheet.Cells[5, 8].StringValue;
                        string temp = date.Replace("[dateStart]", datePeriod[0].ToShortDateString())
                            .Replace("[dateEnd]", datePeriod[1].ToShortDateString());
                        sheet.Cells[5, 8].PutValue(temp);
                       

                        int sensorId = sectionSensors[i].Sensors[0].SensorId;
                        var sensorLocation = sectionSensors[i].Sensors[0].Location;
                        sheet.Cells[7, 0].PutValue(sensorLocation);
                        InitValues initValue = DataAccess.GetSensorInitValue(sensorId);
                        decimal? initHeight = initValue.h0;
                        sheet.Cells[7, 2].PutValue(initHeight);

                        int startDataRow = 7;
                        for (int j = 0; j < 7; j++)
                        {
                            MonitorData lastData = new MonitorData();
                            MonitorData currentData = new MonitorData();                          
                            var getDate = PingHanReport.GetLastWeekEachDay((j + 1).ToString(), dtTime);
                            var getData = DataAccess.GetUnifyData(new[] { sensorId }, getDate, getDate.AddDays(+1).AddMilliseconds(-1), true);

                            var getLastDate = getDate.AddDays(-1).Date;
                            var getLastData = DataAccess.GetUnifyData(new[] { sensorId }, getLastDate, getLastDate.AddDays(+1).AddMilliseconds(-1), true);
                            // 上次累计变量
                            if (getLastData.Count != 0)
                            {
                                lastData = getLastData.ContainsKey(sensorLocation) ? getLastData[sensorLocation] : new MonitorData();                                                      
                            }
                            // 本次累计变量
                            if (getData.Count != 0)
                            {
                                currentData = getData.ContainsKey(sensorLocation) ? getData[sensorLocation] : new MonitorData();
                                if (currentData.Data != null && currentData.Data[0].Values != null)
                                {
                                    sheet.Cells[startDataRow, 6].PutValue(currentData.Data[0].Values[0]);
                                }
                                else
                                {
                                     sheet.Cells[startDataRow, 9].PutValue("/");
                                }
                            }
                            else
                            {
                                sheet.Cells[startDataRow, 9].PutValue("/");
                            }
                            if (currentData.Data != null && lastData.Data != null)
                            {
                                // 本次变量
                                if (currentData.Data[0].Values != null && lastData.Data[0].Values != null)
                                {
                                    decimal bcbl = currentData.Data[0].Values[0] - lastData.Data[0].Values[0];
                                    sheet.Cells[startDataRow, 5].PutValue(bcbl);
                                }
                                else
                                {
                                    sheet.Cells[startDataRow, 5].PutValue("/");
                                }

                            }
                            else
                            {
                                sheet.Cells[startDataRow, 5].PutValue("/");
                            }


                            if (lastData.Data != null)
                            {
                                if (lastData.Data[0].Values != null)
                                {
                                    // 上次高程
                                    decimal? scgc = initHeight - lastData.Data[0].Values[0] / 1000;
                                    sheet.Cells[startDataRow, 3].PutValue(scgc);
                                }
                                else
                                {
                                    sheet.Cells[startDataRow, 3].PutValue("/");
                                }

                            }
                            else
                            {
                                sheet.Cells[startDataRow, 3].PutValue("/");
                            }


                            if (currentData.Data != null)
                            {
                                if (currentData.Data[0].Values != null)
                                {
                                    // 本次高程
                                    decimal? bcgc = initHeight - currentData.Data[0].Values[0] / 1000;
                                    sheet.Cells[startDataRow, 4].PutValue(bcgc);
                                }
                                else
                                {
                                    sheet.Cells[startDataRow, 4].PutValue("/");
                                }

                            }
                            else
                            {
                                sheet.Cells[startDataRow, 4].PutValue("/");
                            }


                            //测量日期
                            sheet.Cells[startDataRow, 7].PutValue(getDate.ToShortDateString());
                            startDataRow++;
                        }
                    }
                    if (ExistOrNot)
                    {
                        for (int i = 0; i < ww.Worksheets.Count; i++)
                        {
                            string tmpname = sectionSensors[i].SectionName ?? "断面" + (0 + 1);
                            tmpname += TemplateHandlerPrams.Factor.NameCN;
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
                    md.Save(ms, SaveFormat.Excel97To2003);
                    ms.Close();
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message);
                    ms.Close();
                    throw ex;
                }
            }
            else
            {
                logger.WarnFormat("结构物ID：{0}，监测因素ID：{1} 施工状态下的截面没检测到可用传感器", Convert.ToString(structId), Convert.ToString(factorId));
                ms.Close();
                throw new ArgumentException();
            }
        }
    }
}
