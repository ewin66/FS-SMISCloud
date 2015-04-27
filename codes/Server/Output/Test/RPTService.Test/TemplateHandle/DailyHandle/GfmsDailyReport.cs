using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Aspose.Cells;
using log4net;
using ReportGeneratorService.Dal;
using ReportGeneratorService.ReportModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class GfmsDailyReport : DailyReport
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;

        public GfmsDailyReport(TemplateHandlerPrams param)
            : base(param,2)
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
            Worksheet tmp;
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
                    FillBasicInformation(ww, 1, 5, "factorName", TemplateHandlerPrams.Factor.NameCN);
                    FillBasicInformation(ww, 2, 0, "address", TemplateHandlerPrams.Organization.Address);
                    FillBasicInformation(ww, 2, 5, "instrumentNo", this.SensorProduct.ProductCode);
                    FillBasicInformation(ww, 3, 1, "monitorday", dtTime.AddDays(-1).ToString("yyyy-MM-dd"));
                    var dateTime = dtTime.Date;
                    var todayData = DataAccess.GetUnifyData(SensorsId, dateTime.AddDays(-1), dateTime, true);
                    var sortedData = todayData.OrderBy(r => r.Key);
                    int sheetidx = 0;
                    var groupid = -1;
                    int rowidx = 5;
                    foreach (var data in sortedData)
                    {
                        var strGroupid = data.Key.ToString().Substring(0, 1);
                        int tempid = 0;
                        tempid = int.TryParse(strGroupid, out tempid) ? tempid : 0;
                        if (groupid == -1)
                        {
                            groupid = tempid;
                        }
                        if (groupid != tempid)
                        {
                            if (ExistOrNot == true)
                            {
                                md.Worksheets.RemoveAt(TemplateHandlerPrams.Factor.NameCN + groupid);
                                tmp = md.Worksheets.Add(TemplateHandlerPrams.Factor.NameCN + groupid);
                                tmp.Copy(ww.Worksheets[sheetidx]);
                            }
                            else
                            {
                                md.Worksheets[0].Copy(ww.Worksheets[sheetidx]);
                                md.Worksheets[0].Name = TemplateHandlerPrams.Factor.NameCN + groupid;
                                ExistOrNot = true;
                            }
                            AddSheetCopy(ww, sheetidx);
                            sheetidx++;
                            DeleteRows(ww, 5, rowidx - 5, sheetidx);
                            rowidx = 5;
                            groupid = tempid;
                        }
                        InsertRow(ww, rowidx, sheetidx);
                        CellMerge(ww, sheetidx, rowidx, 0, 1, 2);
                        CellMerge(ww, sheetidx, rowidx, 2, 1, 2);
                        CellMerge(ww, sheetidx, rowidx, 4, 1, 2);
                        CellMerge(ww, sheetidx, rowidx, 6, 1, 2);
                        CellFormula(ww, sheetidx, rowidx, 6, string.Format("=(C{0}-E{0})/C{0}*100",rowidx + 1));
                        CellMerge(ww, sheetidx, rowidx, 8, 1, 2);
                        CellFormula(ww, sheetidx, rowidx, 8, string.Format("=C{0}*60% + C{0}", rowidx + 1));
                        FillDayDataEx(ww, sheetidx, rowidx, 0, data.Key);
                        FillDayDataEx(ww, sheetidx, rowidx, 2, DataAccess.GetSensorParm(data.Value.SensorId).ToString("0.000"));
                        FillDayDataEx(ww, sheetidx, rowidx, 4, data.Value.Data[0].Values[0].ToString());
                        //FillDayDataEx(ww, sheetidx, rowidx, 6, ((data.Value.Data[0].Values[0] - data.Value.Data[0].Values[0]) / data.Value.Data[0].Values[0]*100).ToString());
                        //FillDayDataEx(ww, sheetidx, rowidx, 8, (data.Value.Data[0].Values[0]*(decimal)1.6).ToString());
                        rowidx++;
                    }
                    if (ExistOrNot == true)
                    {
                        md.Worksheets.RemoveAt(TemplateHandlerPrams.Factor.NameCN + groupid);
                        tmp = md.Worksheets.Add(TemplateHandlerPrams.Factor.NameCN + groupid);
                        tmp.Copy(ww.Worksheets[sheetidx]);
                    }
                    else
                    {
                        md.Worksheets[0].Copy(ww.Worksheets[sheetidx]);
                        md.Worksheets[0].Name = TemplateHandlerPrams.Factor.NameCN + groupid;
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
