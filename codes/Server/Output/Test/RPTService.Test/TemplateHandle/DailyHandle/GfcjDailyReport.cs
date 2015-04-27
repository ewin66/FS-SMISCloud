using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspose.Cells;
using log4net;
using ReportGeneratorService.Dal;
using ReportGeneratorService.ReportModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class GfcjDailyReport : DailyReport
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;
        public GfcjDailyReport(TemplateHandlerPrams param)
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
            if (Cdbh == null)
            {
                ms.Close();
                throw new ArgumentException();
            }
            if (Cdbh.Any())
            {
                try
                {
                    DateTime dtTime = TemplateHandlerPrams.Date;
                    FillBasicInformation(ww, 1, 0, "projectName", TemplateHandlerPrams.Organization.SystemName);
                    FillBasicInformation(ww, 2, 5, "monitorday", dtTime.AddDays(-1).ToString("yyyy-MM-dd"));
                    
                    var dateTime = dtTime.Date;
                    var todayData = DataAccess.GetUnifyData(SensorsId, dateTime.AddDays(-1), dateTime, true);
                    var yesterData = DataAccess.GetUnifyData(SensorsId, dateTime.AddDays(-2), dateTime.AddDays(-1),
                                                             true);
                    var sheet = ww.Worksheets[0];
                    int rowidx = 4;
                    for (int i = 0; i < Cdbh.Length; i++)
                    {
                        var todayMaxValue = todayData.ContainsKey(Cdbh[i])
                            ? todayData[Cdbh[i]].Data[0].Values[0]
                            : 0;
                        var yesterdayMaxValue = yesterData.ContainsKey(Cdbh[i])
                            ? yesterData[Cdbh[i]].Data[0].Values[0] 
                            : 0;
                        InsertRow(ww, rowidx, 0);
                        CellMerge(ww, 0, rowidx, 1, 1, 2);
                        CellMerge(ww, 0, rowidx, 3, 1, 2);
                        CellMerge(ww, 0, rowidx, 5, 1, 2);
                        FillDayDataEx(ww, 0, rowidx, 0, Cdbh[i]);
                        FillDayDataEx(ww, 0, rowidx, 1, (todayMaxValue - yesterdayMaxValue).ToString("0.000"));
                        FillDayDataEx(ww, 0, rowidx, 3, (todayMaxValue - yesterdayMaxValue).ToString("0.000"));
                        FillDayDataEx(ww, 0, rowidx, 5, (todayMaxValue).ToString("0.000"));
                        rowidx++;
                    }
                    if (ExistOrNot == true)
                    {
                        md.Worksheets.RemoveAt(TemplateHandlerPrams.Factor.NameCN);
                        Worksheet tmp = md.Worksheets.Add(TemplateHandlerPrams.Factor.NameCN);
                        tmp.Copy(ww.Worksheets[0]);
                    }
                    else
                    {
                        md.Worksheets[0].Copy(ww.Worksheets[0]);
                        md.Worksheets[0].Name = TemplateHandlerPrams.Factor.NameCN;
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
                logger.WarnFormat("{0}没有找到对应的信息，结构物ID：{1}，监测因素ID：{2}", TemplateHandlerPrams.TemplateFileName,
                              Convert.ToString(TemplateHandlerPrams.Structure.Id),
                              Convert.ToString(TemplateHandlerPrams.Factor.Id));
                ms.Close();
                throw new ArgumentException();
            }
        }
    }
}