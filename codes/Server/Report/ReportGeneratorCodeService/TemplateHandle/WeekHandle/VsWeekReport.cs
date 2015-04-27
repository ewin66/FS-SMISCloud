using Aspose.Cells;
using ReportGeneratorService.Dal;
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
    public class VsWeekReport:WeekReport
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;
        public VsWeekReport(TemplateHandlerPrams param)
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
                ms = new FileStream(TemplateHandlerPrams.FileFullName, FileMode.CreateNew, FileAccess.ReadWrite);
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
                    FillBasicInformation(ww, 0, 0, "projectName", TemplateHandlerPrams.Organization.SystemName);
                    FillBasicInformation(ww, 2, 0, "contractorUnit", TemplateHandlerPrams.Structure.ConstructionCompany);
                    FillBasicInformation(ww, 2, 12, "contractNo", "数据库暂无");
                    FillBasicInformation(ww, 3, 0, "supervisingUnit", "数据库暂无");
                    FillBasicInformation(ww, 3, 12, "factorNo", TemplateHandlerPrams.Factor.Id.ToString());
                    FillBasicInformation(ww, 4, 0, "position", TemplateHandlerPrams.Structure.Name);
                    FillBasicInformation(ww, 5, 3, "instrumentName", SensorProduct.ProductName);
                    FillBasicInformation(ww, 5, 10, "instrumentNo", SensorProduct.ProductCode);

                    //获取采集时间上周一数据
                    var mondayDate = GetLastWeekEachDay("Monday", dtTime);
                    var mondayData = DataAccess.GetUnifyData(SensorsId, mondayDate, mondayDate.AddDays(+1).AddMilliseconds(-1), true);
                    FillDateTime(ww, 7, 3, mondayDate);
                    FillDateTime(ww, 5, 0, "dateFrom", mondayDate);
                    //获取采集时间上周二数据
                    var tuesdayDate = GetLastWeekEachDay("Tuesday", dtTime);
                    var tuesdayData = DataAccess.GetUnifyData(SensorsId, tuesdayDate, tuesdayDate.AddDays(+1).AddMilliseconds(-1), true);
                    FillDateTime(ww, 7, 4, tuesdayDate);
                    //获取采集时间上周三数据
                    var wednesdayDate = GetLastWeekEachDay("Wednesday", dtTime);
                    var wednesdayData = DataAccess.GetUnifyData(SensorsId, wednesdayDate, wednesdayDate.AddDays(+1).AddMilliseconds(-1),
                                                                true);
                    FillDateTime(ww, 7, 5, wednesdayDate);
                    //获取采集时间上周四数据
                    var thursdayDate = GetLastWeekEachDay("Thursday", dtTime);
                    var thursdayData = DataAccess.GetUnifyData(SensorsId, tuesdayDate, thursdayDate.AddDays(+1).AddMilliseconds(-1), true);
                    FillDateTime(ww, 7, 6, thursdayDate);
                    //获取采集时间上周五数据
                    var fridayDate = GetLastWeekEachDay("Friday", dtTime);
                    var fridayData = DataAccess.GetUnifyData(SensorsId, fridayDate, fridayDate.AddDays(+1).AddMilliseconds(-1), true);
                    FillDateTime(ww, 7, 7, fridayDate);
                    //获取采集时间上周六数据
                    var saturdayDate = GetLastWeekEachDay("Saturday", dtTime);
                    var saturdayData = DataAccess.GetUnifyData(SensorsId, saturdayDate, saturdayDate.AddDays(+1).AddMilliseconds(-1), true);
                    FillDateTime(ww, 7, 8, saturdayDate);
                    //获取采集时间上周日数据
                    var sundayDate = GetLastWeekEachDay("Sunday", dtTime);
                    var sundayData = DataAccess.GetUnifyData(SensorsId, sundayDate, sundayDate.AddDays(+1).AddMilliseconds(-1), true);
                    FillDateTime(ww, 5, 0, "dateTo", sundayDate);
                    FillDateTime(ww, 7, 9, sundayDate);
                    //本周沉降变化量
                    var mondayFirstData = DataAccess.GetUnifyData(SensorsId, mondayDate, mondayDate.AddDays(+1).AddMilliseconds(-1), false);

                    var sheet = ww.Worksheets[0];
                    int startRowIndex = 8;
                    for (int i = 0; i < Cdbh.Length; i++)
                    {
                        sheet.Cells[startRowIndex, 0].PutValue(Cdbh[i]);
                        FillEveryDayData(ww, startRowIndex, 3, 0, 2, startRowIndex, 1, Cdbh[i], mondayData);
                        FillEveryDayData(ww, startRowIndex, 4, 0, 2, startRowIndex, 1, Cdbh[i], tuesdayData);
                        FillEveryDayData(ww, startRowIndex, 5, 0, 2, startRowIndex, 1, Cdbh[i], wednesdayData);
                        FillEveryDayData(ww, startRowIndex, 6, 0, 2, startRowIndex, 1, Cdbh[i], tuesdayData);
                        FillEveryDayData(ww, startRowIndex, 7, 0, 2, startRowIndex, 1, Cdbh[i], fridayData);
                        FillEveryDayData(ww, startRowIndex, 8, 0, 2, startRowIndex, 1, Cdbh[i], saturdayData);
                        FillEveryDayData(ww, startRowIndex, 9, 0, 2, startRowIndex, 1, Cdbh[i], saturdayData);
                        FillBCData(ww, startRowIndex, 13, 0, 2, Cdbh[i], sundayData);
                        FillVariation(ww, startRowIndex, 10, 0, 2, Cdbh[i], mondayFirstData, sundayData);
                        FillLJVariation(ww, startRowIndex, 11);
                        FillLastWeekLJVariation(ww, startRowIndex, 2, 11, 10);
                        FillVaryRate(ww, startRowIndex, 12);
                        startRowIndex++;
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
                logger.WarnFormat("{0}没有找到对应的信息，结构物ID：{1}，监测因素ID：{2}", TemplateHandlerPrams.TemplateFileName, Convert.ToString(TemplateHandlerPrams.Structure.Id), Convert.ToString(TemplateHandlerPrams.Factor.Id));
                ms.Close();
                throw new ArgumentException();
            }
        }
    }
}
