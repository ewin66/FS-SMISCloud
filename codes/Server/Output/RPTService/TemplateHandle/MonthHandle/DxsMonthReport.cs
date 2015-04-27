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
    public class DxsMonthReport:MonthReport
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;
        public DxsMonthReport(TemplateHandlerPrams param)
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
                    FillBasicInformation(ww, 0, 0, "projectName", TemplateHandlerPrams.Organization.SystemName);
                    FillBasicInformation(ww, 2, 0, "contractorUnit", TemplateHandlerPrams.Structure.ConstructionCompany);
                    FillBasicInformation(ww, 2, 12, "contractNo", "数据库暂无");
                    FillBasicInformation(ww, 3, 0, "supervisingUnit", "数据库暂无");
                    FillBasicInformation(ww, 3, 12, "factorNo", TemplateHandlerPrams.Factor.Id.ToString());
                    FillBasicInformation(ww, 4, 0, "position", TemplateHandlerPrams.Structure.Name);
                    FillBasicInformation(ww, 5, 3, "instrumentName", SensorProduct.ProductName);
                    //根据上月天数添加列数
                    ExtensionColumn(ww, dtTime, 9);
                    //监测日期范围
                    FillDateTimeRange(ww, 5, 0, dtTime);
                    //获取上月每天数据
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

                    for (int i = 1; i <= k; i++)
                    {
                        FillEveryDayDate(ww, 7, i + 2, GetLastMonthEachDay(i.ToString(), dtTime));
                    }
                    var sheet = ww.Worksheets[0];
                    int startRowIndex = 8;
                    for (int i = 0; i < Cdbh.Length; i++)
                    {
                        sheet.Cells[startRowIndex, 0].PutValue(Cdbh[i]);
                        for (int j = 1; j <= k; j++)
                        {
                            var date = GetLastMonthEachDay(j.ToString(), dtTime);
                            var Data = DataAccess.GetUnifyData(SensorsId, date, date.AddDays(+1).AddMilliseconds(-1), true);
                            FillEveryDayData(ww, startRowIndex, j + 2, 0, 0, startRowIndex, 1, Cdbh[i], Data);
                        }
                        var dyData = DataAccess.GetUnifyData(SensorsId, dd, dd.AddDays(+1).AddMilliseconds(-1), false);
                        var zhData = DataAccess.GetUnifyData(SensorsId, zz, zz.AddDays(+1).AddMilliseconds(-1), true);
                        FillVariation(ww, startRowIndex, 0, 0, Cdbh[i], k, dyData, zhData);
                        FillLJVariation(ww, startRowIndex, k);
                        FillLastMonthLJVariation(ww, startRowIndex, 2, k);
                        FillVaryRate(ww, startRowIndex, k);
                        var BcData = DataAccess.GetUnifyData(SensorsId, zz.Date, zz.Date.AddDays(+1).AddMilliseconds(-1), true);
                        FillBCData(ww, startRowIndex, 0, 0, k, 6, Cdbh[i], BcData);
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
