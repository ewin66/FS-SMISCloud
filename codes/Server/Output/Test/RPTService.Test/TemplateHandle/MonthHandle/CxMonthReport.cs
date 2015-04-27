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
    public class CxMonthReport:MonthReport
    {

        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;
        public CxMonthReport(TemplateHandlerPrams param)
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
            int k;
            DateTime dd;
            DateTime zz;
            if (dtTime.Month == 1)
            {
                k = DateTime.DaysInMonth(dtTime.Year - 1, 12);
                dd = new DateTime(dtTime.Year - 1, 12, 1).Date;
                zz = new DateTime(dtTime.Year - 1, 12, k).Date;
            }
            else
            {
                k = DateTime.DaysInMonth(dtTime.Year, dtTime.Month - 1);
                dd = new DateTime(dtTime.Year, dtTime.Month - 1, 1).Date;
                zz = new DateTime(dtTime.Year, dtTime.Month - 1, k).Date;
            }
            List<CxGroup> groups=new List<CxGroup>();
            try
            {
                groups = DataAccess.GetGroupByStuAndFac(TemplateHandlerPrams.Structure.Id,
                                                            TemplateHandlerPrams.Factor.Id);
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
                        var timeFrom = sheet.Cells[7, 0].StringValue;
                        sheet.Cells[7, 0].PutValue(timeFrom.Replace("timeFrom", dd.ToShortTimeString()));
                        var timeTo = sheet.Cells[7, 0].StringValue;
                        sheet.Cells[7, 0].PutValue(timeTo.Replace("timeTo", zz.ToShortTimeString()));
                        //根据上月天数添加列数
                        ExtensionColumn(ww, dtTime, i,8);
                        //监测日期范围
                        FillDateTimeRange(ww, 6, 0,i, dtTime);
                        for (int j = 1; j <= k; j++)
                        {
                            FillEveryDayDate(ww, 10, j + 1, i,GetLastMonthEachDay(j.ToString(), dtTime));
                        }
                        int startRowIndex = 11;
                        foreach (var depth in groups[i].Depth)
                        {
                            sheet.Cells[startRowIndex, 0].PutValue(depth);


                            for (int m = 1; m <= k; m++)
                            {
                                var date = GetLastMonthEachDay(m.ToString(), dtTime);
                                var Data = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, date, date.AddDays(+1).AddMilliseconds(-1),
                                                                                   true);
                                decimal? DataValue = 0;
                                if (Data.Count != 0)
                                {
                                    var depthValue = Data.FirstOrDefault(p => p.Depth*(-1) == depth);
                                    if (depthValue != null)
                                    {
                                        DataValue = depthValue.YValue;
                                    }
                                }
                                sheet.Cells[startRowIndex, m + 1].PutValue(DataValue);
                            }
                            //本月变量
                            var yihaoMinData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, dd,
                                                                                       dd.AddDays(+1).AddMilliseconds(-1),
                                                                                       false);
                            var zuihouData = DataAccess.GetByGroupAndDateGroupByTime(groups[i].Id, zz,
                                                                                     zz.AddDays(+1).AddMilliseconds(-1),
                                                                                     true);
                            decimal? yihaoMinDataValue = 0;
                            if (yihaoMinData.Count != 0)
                            {
                                var depthValue = yihaoMinData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    yihaoMinDataValue = depthValue.YValue;
                                }
                            }
                            decimal? zuihouDataValue = 0;
                            if (zuihouData.Count != 0)
                            {
                                var depthValue = zuihouData.FirstOrDefault(p => p.Depth*(-1) == depth);
                                if (depthValue != null)
                                {
                                    zuihouDataValue = depthValue.YValue;
                                }
                            }
                            sheet.Cells[startRowIndex, k + 2].PutValue(zuihouDataValue - yihaoMinDataValue);
                            //累计变量
                            decimal? variation = 0;
                            for (int n = 0; n < k; n++)
                            {
                                variation += Convert.ToDecimal(sheet.Cells[startRowIndex, n + 2].StringValue);
                            }
                            sheet.Cells[startRowIndex, k + 3].PutValue(variation);
                            //上月累计位移量
                            sheet.Cells[startRowIndex, 1].PutValue(
                                Convert.ToDecimal(sheet.Cells[startRowIndex, k + 3].StringValue) -
                                Convert.ToDecimal(sheet.Cells[startRowIndex, k + 2].StringValue));
                            //本月变化速率
                            sheet.Cells[startRowIndex, k + 4].PutValue(
                                (Convert.ToDecimal(sheet.Cells[startRowIndex, k + 2].StringValue)/k).ToString("0.000"));
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
                ms.Close();
                logger.WarnFormat("{0}没有找到对应的信息，结构物ID：{1}，监测因素ID：{2}", TemplateHandlerPrams.TemplateFileName, Convert.ToString(TemplateHandlerPrams.Structure.Id), Convert.ToString(TemplateHandlerPrams.Factor.Id));
                throw new ArgumentException();
            }
        }
    }
}
