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
   public class CxDailyReport:DailyReport
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;
        public CxDailyReport(TemplateHandlerPrams param)
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
                ms = new FileStream(TemplateHandlerPrams.FileFullName, FileMode.Create, FileAccess.Write);
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
            List<CxGroup> groups = new List<CxGroup>();
            try
            {
                groups = DataAccess.GetGroupByStuAndFac(TemplateHandlerPrams.Structure.Id, TemplateHandlerPrams.Factor.Id);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                ms.Close();
                throw ex;
            }

            // 复制sheet
            if (groups.Count > 0)
            {
                try
                {
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
                        var contractNo = sheet.Cells[2, 4].StringValue;
                        sheet.Cells[2, 4].PutValue(contractNo.Replace("contractNo", "数据库暂无"));
                        var supervisingUnit = sheet.Cells[3, 0].StringValue;
                        sheet.Cells[3, 0].PutValue(supervisingUnit.Replace("supervisingUnit", "数据库暂无"));
                        var factorNo = sheet.Cells[3, 4].StringValue;
                        sheet.Cells[3, 4].PutValue(factorNo.Replace("factorNo",
                                                                    TemplateHandlerPrams.Factor.Id.ToString()));
                        var position = sheet.Cells[4, 0].StringValue;
                        sheet.Cells[4, 0].PutValue(position.Replace("position", TemplateHandlerPrams.Structure.Name));
                        var groupNo = sheet.Cells[5, 0].StringValue;
                        sheet.Cells[5, 0].PutValue(groupNo.Replace("groupNo", groups[i].Name ?? "测斜管" + (i + 1)));
                        var date = sheet.Cells[6, 0].StringValue;
                        sheet.Cells[6, 0].PutValue(date.Replace("date", dtTime.AddDays(-1).ToShortDateString()));
                        var time = sheet.Cells[7, 0].StringValue;
                        sheet.Cells[7, 0].PutValue(time.Replace("time", dtTime.AddDays(-1).ToShortTimeString()));
                        var instrumentName = sheet.Cells[8, 0].StringValue;
                        sheet.Cells[8, 0].PutValue(instrumentName.Replace("instrumentName", SensorProduct.ProductName));
                        var instrumentNo = sheet.Cells[9, 0].StringValue;
                        sheet.Cells[9, 0].PutValue(instrumentNo.Replace("instrumentNo", SensorProduct.ProductCode));
                        //获取本次累计位移
                        var todayMaxData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, dtTime.AddDays(-1).Date,
                                                                                   dtTime.Date, true);
                        var todayMinData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, dtTime.AddDays(-1).Date,
                                                                                   dtTime.Date, false);
                        //获取上次累计位移
                        var yesterdayMaxData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id,
                                                                                       dtTime.AddDays(-2).Date,
                                                                                       dtTime.AddDays(-1).Date, true);
                        var yesterdayMinData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id,
                                                                                       dtTime.AddDays(-2).Date,
                                                                                       dtTime.AddDays(-1).Date, false);
                        //获取上上次累计位移
                        var lastMaxData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, dtTime.AddDays(-3).Date,
                                                                                  dtTime.AddDays(-2).Date, true);
                        var lastMinData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, dtTime.AddDays(-3).Date,
                                                                                  dtTime.AddDays(-2).Date, false);
                        int startRowIndex = 12;
                        foreach (var depth in groups[i].Depth)
                        {
                            sheet.Cells[startRowIndex, 0].PutValue(depth);
                            decimal? yesterdayMaxValue = 0;
                            if (yesterdayMaxData.Count != 0)
                            {
                                var depthValue = yesterdayMaxData.FirstOrDefault(p => p.Depth * (-1) == depth);
                                if (depthValue != null)
                                {
                                    yesterdayMaxValue = depthValue.YValue;
                                }
                            }
                            decimal? yesterdayMinValue = 0;
                            if (yesterdayMinData.Count != 0)
                            {
                                var depthValue = yesterdayMinData.FirstOrDefault(p => p.Depth * (-1) == depth);
                                if (depthValue != null)
                                {
                                    yesterdayMinValue = depthValue.YValue;
                                }
                            }
                            decimal? todayMaxValue = 0;
                            if (todayMaxData.Count != 0)
                            {
                                var depthValue = todayMaxData.FirstOrDefault(p => p.Depth * (-1) == depth);
                                if (depthValue != null)
                                {
                                    todayMaxValue = depthValue.YValue;
                                }
                            }
                            decimal? todayMinValue = 0;
                            if (todayMinData.Count != 0)
                            {
                                var depthValue = todayMinData.FirstOrDefault(p => p.Depth * (-1) == depth);
                                if (depthValue != null)
                                {
                                    todayMinValue = depthValue.YValue;
                                }
                            }
                            decimal? lastMaxValue = 0;
                            if (lastMaxData.Count != 0)
                            {
                                var depthValue = lastMaxData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    lastMaxValue = depthValue.YValue;
                                }
                            }

                            //前次累计变量
                            decimal? qclj = yesterdayMaxValue - yesterdayMinValue;
                            //本次累计变量
                            decimal? bclj = todayMaxValue - todayMinValue;
                            //本次位移变量
                            decimal? bcbl = todayMaxValue - yesterdayMaxValue;
                            //上次位移变量
                            decimal? scbl = yesterdayMaxValue - lastMaxValue;
                            //填写
                            sheet.Cells[startRowIndex, 1].PutValue(qclj);
                            sheet.Cells[startRowIndex, 2].PutValue(scbl);
                            sheet.Cells[startRowIndex, 3].PutValue(bcbl);
                            sheet.Cells[startRowIndex, 4].PutValue(bclj);

                            startRowIndex++;
                        }
                    }
                 // ww.Worksheets.ActiveSheetIndex = 0;
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
                logger.WarnFormat("{0}没有找到对应的信息，结构物ID：{1}，监测因素ID：{2}", TemplateHandlerPrams.TemplateFileName,Convert.ToString(TemplateHandlerPrams.Structure.Id), Convert.ToString(TemplateHandlerPrams.Factor.Id));
                ms.Close();
                throw new ArgumentException();
            }           
        }
    }
}
