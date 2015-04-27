using System.Threading;
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
    public class DxsDailyReport:DailyReport
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool ExistOrNot = false;
        public DxsDailyReport(TemplateHandlerPrams param)
            : base(param)
        { 
        }
       
        public override void WriteFile( )
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
            if (Cdbh.Any()){
                try
                {
                    DateTime dtTime = TemplateHandlerPrams.Date;
                    FillBasicInformation(ww, 0, 0, "projectName", TemplateHandlerPrams.Organization.SystemName);
                    FillBasicInformation(ww, 2, 0, "contractorUnit", TemplateHandlerPrams.Structure.ConstructionCompany);
                    FillBasicInformation(ww, 2, 11, "contractNo", "数据库暂无");
                    FillBasicInformation(ww, 3, 0, "supervisingUnit", "数据库暂无");
                    FillBasicInformation(ww, 3, 11, "factorNo", Convert.ToString(TemplateHandlerPrams.Factor.Id));
                    FillBasicInformation(ww, 4, 0, "position", TemplateHandlerPrams.Structure.Name);
                    FillBasicInformation(ww, 5, 0, "date", dtTime.AddDays(-1).ToShortDateString());
                    FillBasicInformation(ww, 5, 4, "time", dtTime.ToShortTimeString());
                    FillBasicInformation(ww, 6, 0, "instrumentName", SensorProduct.ProductName);
                    FillBasicInformation(ww, 6, 4, "instrumentNo", SensorProduct.ProductCode);
                    var dateTime = dtTime.Date;
                    var todayData = DataAccess.GetUnifyData(SensorsId, dateTime.AddDays(-1), dateTime.AddMilliseconds(-1), true);
                    var yesterData = DataAccess.GetUnifyData(SensorsId, dateTime.AddDays(-2), dateTime.AddDays(-1).AddMilliseconds(-1), true);
                    var sheet = ww.Worksheets[0];
                    int StartRowIndex = 9;
                    int AnotherRowIndex = 9;
                    for (int i = 0; i < Cdbh.Length; i++)
                    {
                        if (i >= 8)
                        {

                            sheet.Cells[AnotherRowIndex, 7].PutValue(Cdbh[i]);
                            FillDayData(ww, AnotherRowIndex, 11, 0, 0, Cdbh[i], yesterData);
                            FillDayData(ww, AnotherRowIndex, 12, 0, 0, Cdbh[i], todayData);
                            FillVariation(ww, AnotherRowIndex, 8, 12, 1);
                            FillLJVariation(ww, AnotherRowIndex, 9, 12, 2);
                            AnotherRowIndex++;
                        }
                        else
                        {
                            sheet.Cells[StartRowIndex, 0].PutValue(Cdbh[i]);
                            FillDayData(ww, StartRowIndex, 4, 0, 0, Cdbh[i], yesterData);
                            FillDayData(ww, StartRowIndex, 5, 0, 0, Cdbh[i], todayData);
                            FillVariation(ww, StartRowIndex, 1, 5, 1);
                            FillLJVariation(ww, StartRowIndex, 2, 5, 2);
                            StartRowIndex++;
                        }
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
            else{
                logger.WarnFormat("{0}没有找到对应的信息，结构物ID：{1}，监测因素ID：{2}",TemplateHandlerPrams.TemplateFileName,Convert.ToString(TemplateHandlerPrams.Structure.Id),Convert.ToString(TemplateHandlerPrams.Factor.Id));
                ms.Close();
                throw new ArgumentException();
            }
        }
       
    }
}
