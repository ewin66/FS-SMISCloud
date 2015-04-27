/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：CrackWeeklyReport.cs
// 功能描述：
// 
// 创建标识： 2015/1/22 9:22:36
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
using System.Reflection;
using System.Text;
using Aspose.Cells;
using log4net;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class PhCrackWeeklyReport : TemplateHandleBase
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool ExistOrNot { get; set; }
        private int structId { get; set; }
        private int factorId { get; set; }
        private string factorName { get; set; }
        private DateTime dtTime { get; set; }
        private string template { get; set; }
        private string targetFile { get; set; }

        public PhCrackWeeklyReport(TemplateHandlerPrams para)
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
            ExistOrNot = PingHanReport.GetTargetWorkbook(targetFile, out ms, out md);
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
                    ww = PingHanReport.SetSheetName(sectionSensors, ww, factorName);

                    for (int i = 0; i < sectionSensors.Count; i++)
                    {
                        var sheet = ww.Worksheets[i];
                        var chart = sheet.Charts[0];
                        chart.Title.Text = ww.Worksheets[i].Name + "累计变化趋势";
                        sheet.Cells[1, 2].PutValue(sectionSensors[i].SectionName);
                        var sectionName = sheet.Cells[4, 0].StringValue;
                        sheet.Cells[4, 0].PutValue(sectionName.Replace("[sectionName]", sectionSensors[i].SectionName));

                        var date = sheet.Cells[5, 11].StringValue;
                        string temp = date.Replace("[dateStart]", datePeriod[0].ToShortDateString())
                            .Replace("[dateEnd]", datePeriod[1].ToShortDateString());
                        sheet.Cells[5, 11].PutValue(temp);


                        int sensorId = sectionSensors[i].Sensors[0].SensorId;
                        var sensorLocation = sectionSensors[i].Sensors[0].Location;
                        sheet.Cells[8, 0].PutValue(sensorLocation);
                        InitValues initValue = DataAccess.GetSensorInitValue(sensorId);
                        decimal? len0 = initValue.len0;
                        sheet.Cells[8, 3].PutValue(len0);

                        int startDataRow = 8;
                        for (int j = 0; j < 7; j++)
                        {
                            sheet.Cells[startDataRow, 8].PutValue(1); // 时间间隔

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
                                    sheet.Cells[startDataRow, 7].PutValue(currentData.Data[0].Values[0]);
                                }
                                else
                                {
                                    sheet.Cells[startDataRow, 12].PutValue("/");
                                }
                            }
                            else
                            {
                                sheet.Cells[startDataRow, 12].PutValue("/");
                            }
                            if (currentData.Data != null && lastData.Data != null)
                            {
                                // 本次变量
                                if (currentData.Data[0].Values != null && lastData.Data[0].Values != null)
                                {
                                    decimal bcbl = currentData.Data[0].Values[0] - lastData.Data[0].Values[0];
                                    sheet.Cells[startDataRow, 6].PutValue(bcbl);
                                    sheet.Cells[startDataRow, 9].PutValue(currentData.Data[0].Values[0]);// 收敛速率
                                }
                                else
                                {
                                    sheet.Cells[startDataRow, 6].PutValue("/");
                                    sheet.Cells[startDataRow, 9].PutValue("/");
                                }

                            }
                            else
                            {
                                sheet.Cells[startDataRow, 6].PutValue("/");
                                sheet.Cells[startDataRow, 9].PutValue("/");
                            }


                            if (lastData.Data != null)
                            {
                                if (lastData.Data[0].Values != null)
                                {
                                    // 上次测量值
                                    decimal? scgc = len0 + lastData.Data[0].Values[0] / 1000;
                                    sheet.Cells[startDataRow, 4].PutValue(scgc);
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


                            if (currentData.Data != null)
                            {
                                if (currentData.Data[0].Values != null)
                                {
                                    // 本次测量值
                                    decimal? bcgc = len0 + currentData.Data[0].Values[0] / 1000;
                                    sheet.Cells[startDataRow, 5].PutValue(bcgc);
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

                            //测量日期
                            sheet.Cells[startDataRow, 10].PutValue(getDate.ToShortDateString());
                            startDataRow++;
                        }
                    }

                    PingHanReport.CopyExcelSheet(ExistOrNot, sectionSensors, factorName, ref ww, ref md);

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
