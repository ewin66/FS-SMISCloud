using Aspose.Cells;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.ReportModule;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReportGeneratorService.TemplateHandle
{
    public class CxWeekReport:WeekReport
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;
        public CxWeekReport(TemplateHandlerPrams param)
            : base(param)
        { 
        }
        public override void WriteFile()
        {
            if (TemplateHandlerPrams.Structure.Id == 0)
            {
                throw new ArgumentNullException(TemplateHandlerPrams.Structure.Id.ToString(), "结构物id异常");
            }
            if (TemplateHandlerPrams.Factor.Id == 0)
            {
                throw new ArgumentNullException(TemplateHandlerPrams.Factor.Id.ToString(), "监测因素id异常");
            }
            if (!File.Exists(TemplateHandlerPrams.TemplateFileName))
            {
                logger.Warn("模版文件 : " + TemplateHandlerPrams.TemplateFileName + "未找到");
                throw new FileNotFoundException("模版文件 : " + TemplateHandlerPrams.TemplateFileName);
            }
            //读取目标（如果不存在则创建，否则直接使用）
            Workbook md;
            FileStream ms;
            if (!File.Exists(TemplateHandlerPrams.FileFullName))
            {
                ExistOrNot = false;
                ms = new FileStream(TemplateHandlerPrams.FileFullName, FileMode.CreateNew, FileAccess.Write);
                md = new Workbook(ms);
            }
            else
            {
                ExistOrNot = true;
                ms = new FileStream(TemplateHandlerPrams.FileFullName, FileMode.Open, FileAccess.ReadWrite);
                md = new Workbook(ms);
            }
            // 读取模版
            Workbook ww;
            using (var fs = new FileStream(TemplateHandlerPrams.TemplateFileName, FileMode.Open, FileAccess.Read))
            {
                ww = new Workbook(fs);
                fs.Close();
            }

            DateTime dtTime = TemplateHandlerPrams.Date;
            List<CxGroup> groups=new List<CxGroup>();
            try
            {
               groups = DataAccess.GetGroupByStuAndFac(TemplateHandlerPrams.Structure.Id, TemplateHandlerPrams.Factor.Id);
            }
            catch (Exception ex)
            {
                ms.Close();
                logger.Warn(ex.Message);
                throw ex;
            }
            if (groups.Count > 0)
            {
                try
                {
                    // 复制sheet
                    ww.Worksheets[0].Name = TemplateHandlerPrams.Factor.NameCN + groups[0].Name ?? "测斜管" + (0 + 1);
                    for (int i = 1; i < groups.Count; i++)
                    {
                        var index = ww.Worksheets.AddCopy(0);
                        ww.Worksheets[index].Name = TemplateHandlerPrams.Factor.NameCN + groups[i].Name ??
                                                    "测斜管" + (i + 1);
                    }

                    for (int i = 0; i < groups.Count; i++)
                    {
                        var sheet = ww.Worksheets[0 + i];
                        var projectName = sheet.Cells[0, 0].StringValue;
                        sheet.Cells[0, 0].PutValue(projectName.Replace("projectName",
                                                                       TemplateHandlerPrams.Organization.SystemName));
                        var contractorUnit = sheet.Cells[2, 0].StringValue;
                        sheet.Cells[2, 0].PutValue(contractorUnit.Replace("contractorUnit",
                                                                          TemplateHandlerPrams.Structure
                                                                                              .ConstructionCompany));
                        var contractNo = sheet.Cells[2, 10].StringValue;
                        sheet.Cells[2, 10].PutValue(contractNo.Replace("contractNo", "数据库暂无"));
                        var supervisingUnit = sheet.Cells[3, 0].StringValue;
                        sheet.Cells[3, 0].PutValue(supervisingUnit.Replace("supervisingUnit", "数据库暂无"));
                        var factorNo = sheet.Cells[3, 10].StringValue;
                        sheet.Cells[3, 10].PutValue(factorNo.Replace("factorNo",
                                                                     TemplateHandlerPrams.Factor.Id.ToString()));
                        var position = sheet.Cells[4, 0].StringValue;
                        sheet.Cells[4, 0].PutValue(position.Replace("position", TemplateHandlerPrams.Structure.Name));
                        var groupNo = sheet.Cells[5, 0].StringValue;
                        sheet.Cells[5, 0].PutValue(groupNo.Replace("groupNo", groups[i].Name ?? "测斜管" + (i + 1)));
                        var instrumentName = sheet.Cells[8, 0].StringValue;
                        sheet.Cells[8, 0].PutValue(instrumentName.Replace("instrumentName", SensorProduct.ProductName));
                        //获取采集时间上周一数据
                        var mondayDate = GetLastWeekEachDay("Monday", dtTime);
                        var mondayData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, mondayDate,
                                                                                 mondayDate.AddDays(+1).AddMilliseconds(-1), true);
                        var mondayMinData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, mondayDate,
                                                                                    mondayDate.AddDays(+1).AddMilliseconds(-1), false);
                        var date = sheet.Cells[6, 0].StringValue;
                        sheet.Cells[6, 0].PutValue(date.Replace("dateFrom", mondayDate.ToShortDateString()));
                        date = sheet.Cells[7, 0].StringValue;
                        sheet.Cells[7, 0].PutValue(date.Replace("timeFrom", mondayDate.ToShortTimeString()));
                        var Unit = sheet.Cells[10, 2].StringValue;
                        sheet.Cells[10, 2].PutValue(Unit.Replace("月  日", mondayDate.ToShortDateString()));
                        //获取采集时间上周二数据
                        var tuesdayDate = GetLastWeekEachDay("Tuesday", dtTime);
                        var tuesdayData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, tuesdayDate,
                                                                                  tuesdayDate.AddDays(+1).AddMilliseconds(-1), true);
                        Unit = sheet.Cells[10, 3].StringValue;
                        sheet.Cells[10, 3].PutValue(Unit.Replace("月  日", tuesdayDate.ToShortDateString()));
                        //获取采集时间上周三数据
                        var wednesdayDate = GetLastWeekEachDay("Wednesday", dtTime);
                        var wednesdayData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, wednesdayDate,
                                                                                    wednesdayDate.AddDays(+1).AddMilliseconds(-1), true);
                        Unit = sheet.Cells[10, 4].StringValue;
                        sheet.Cells[10, 4].PutValue(Unit.Replace("月  日", wednesdayDate.ToShortDateString()));
                        //获取采集时间上周四数据
                        var thursdayDate = GetLastWeekEachDay("Thursday", dtTime);
                        var thursdayData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, tuesdayDate,
                                                                                   thursdayDate.AddDays(+1).AddMilliseconds(-1), true);
                        Unit = sheet.Cells[10, 5].StringValue;
                        sheet.Cells[10, 5].PutValue(Unit.Replace("月  日", thursdayDate.ToShortDateString()));
                        //获取采集时间上周五数据
                        var fridayDate = GetLastWeekEachDay("Friday", dtTime);
                        var fridayData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, fridayDate,
                                                                                 fridayDate.AddDays(+1).AddMilliseconds(-1), true);
                        Unit = sheet.Cells[10, 6].StringValue;
                        sheet.Cells[10, 6].PutValue(Unit.Replace("月  日", fridayDate.ToShortDateString()));
                        //获取采集时间上周六数据
                        var saturdayDate = GetLastWeekEachDay("Saturday", dtTime);
                        var saturdayData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, saturdayDate,
                                                                                   saturdayDate.AddDays(+1).AddMilliseconds(-1), true);
                        Unit = sheet.Cells[10, 7].StringValue;
                        sheet.Cells[10, 7].PutValue(Unit.Replace("月  日", saturdayDate.ToShortDateString()));
                        //获取采集时间上周日数据
                        var sundayDate = GetLastWeekEachDay("Sunday", dtTime);
                        var sundayData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, sundayDate,
                                                                                 sundayDate.AddDays(+1).AddMilliseconds(-1), true);
                        date = sheet.Cells[6, 0].StringValue;
                        sheet.Cells[6, 0].PutValue(date.Replace("dateTo", sundayDate.ToShortDateString()));
                        date = sheet.Cells[7, 0].StringValue;
                        sheet.Cells[7, 0].PutValue(date.Replace("timeTo", mondayDate.ToShortTimeString()));
                        Unit = sheet.Cells[10, 8].StringValue;
                        sheet.Cells[10, 8].PutValue(Unit.Replace("月  日", sundayDate.ToShortDateString()));
                        int startRowIndex = 11;
                        foreach (var depth in groups[i].Depth)
                        {
                            sheet.Cells[startRowIndex, 0].PutValue(depth);
                            decimal? mondayDataValue = 0;
                            if (mondayData.Count != 0)
                            {
                                var depthValue = mondayData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    mondayDataValue = depthValue.YValue;
                                }
                            }
                            decimal? mondayMinDataValue = 0;
                            if (mondayMinData.Count != 0)
                            {
                                var depthValue = mondayMinData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    mondayMinDataValue = depthValue.YValue;
                                }
                            }
                            decimal? tuesdayDataValue = 0;
                            if (tuesdayData.Count != 0)
                            {
                                var depthValue = tuesdayData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    tuesdayDataValue = depthValue.YValue;
                                }
                            }
                            decimal? wednesdayDataValue = 0;
                            if (wednesdayData.Count != 0)
                            {
                                var depthValue = wednesdayData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    wednesdayDataValue = depthValue.YValue;
                                }
                            }
                            decimal? fridayDataValue = 0;
                            if (fridayData.Count != 0)
                            {
                                var depthValue = fridayData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    fridayDataValue = depthValue.YValue;
                                }
                            }
                            decimal? thursdayDataValue = 0;
                            if (thursdayData.Count != 0)
                            {
                                var depthValue = thursdayData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    thursdayDataValue = depthValue.YValue;
                                }
                            }
                            decimal? saturdayDataValue = 0;
                            if (saturdayData.Count != 0)
                            {
                                var depthValue = saturdayData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    saturdayDataValue = depthValue.YValue;
                                }
                            }
                            decimal? sundayDataValue = 0;
                            if (sundayData.Count != 0)
                            {
                                var depthValue = sundayData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    sundayDataValue = depthValue.YValue;
                                }
                            }
                            sheet.Cells[startRowIndex, 2].PutValue(mondayDataValue);
                            sheet.Cells[startRowIndex, 3].PutValue(tuesdayDataValue);
                            sheet.Cells[startRowIndex, 4].PutValue(wednesdayDataValue);
                            sheet.Cells[startRowIndex, 5].PutValue(thursdayDataValue);
                            sheet.Cells[startRowIndex, 6].PutValue(fridayDataValue);
                            sheet.Cells[startRowIndex, 7].PutValue(saturdayDataValue);
                            sheet.Cells[startRowIndex, 8].PutValue(sundayDataValue);
                            //本周变量
                            sheet.Cells[startRowIndex, 9].PutValue(sundayDataValue - mondayMinDataValue);
                            //累计变量
                            decimal? variation = 0;
                            for (int k = 0; k < 7; k++)
                            {
                                variation += Convert.ToDecimal(sheet.Cells[startRowIndex, k + 2].StringValue);
                            }
                            sheet.Cells[startRowIndex, 10].PutValue(variation);
                            //上周累计位移量
                            sheet.Cells[startRowIndex, 1].PutValue(
                                Convert.ToDecimal(sheet.Cells[startRowIndex, 10].StringValue) -
                                Convert.ToDecimal(sheet.Cells[startRowIndex, 9].StringValue));
                            //本周变化速率
                            sheet.Cells[startRowIndex, 11].PutValue(
                                (Convert.ToDecimal(sheet.Cells[startRowIndex, 9].StringValue)/7).ToString("0.000"));
                            startRowIndex++;
                        }
                    }
                    if (ExistOrNot == true)
                    {
                        for (int i = 0; i < ww.Worksheets.Count; i++)
                        {
                            string tmpname = TemplateHandlerPrams.Factor.NameCN + groups[i].Name ?? "测斜管" + (0 + 1);
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
                logger.WarnFormat("{0}没有找到对应的信息，结构物ID：{1}，监测因素ID：{2}", TemplateHandlerPrams.TemplateFileName, Convert.ToString(TemplateHandlerPrams.Structure.Id), Convert.ToString(TemplateHandlerPrams.Factor.Id));
                ms.Close();
                throw new ArgumentException();
            }
        }
    }
}
