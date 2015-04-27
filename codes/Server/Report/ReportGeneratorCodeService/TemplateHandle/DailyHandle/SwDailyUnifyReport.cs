using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Aspose.Cells;
using log4net.Repository.Hierarchy;
using NPOI.SS.Formula.Functions;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.ReportModule;
using log4net;
using System.Threading;

namespace ReportGeneratorService.TemplateHandle
{
    public class SwDailyUnifyReport : DailyReport
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _existOrNot = false;
        private decimal limit = 500;// 预警值
        public SwDailyUnifyReport(TemplateHandlerPrams param)
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
            FileStream ms;
            if (!File.Exists(TemplateHandlerPrams.FileFullName))
            {
                _existOrNot = false;
                ms = new FileStream(TemplateHandlerPrams.FileFullName, FileMode.CreateNew, FileAccess.Write);

            }
            else
            {
                _existOrNot = true;
                ms = new FileStream(TemplateHandlerPrams.FileFullName, FileMode.Open, FileAccess.ReadWrite);

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
            if (Cdbh.Count() > 0)
            {
                try
                {
                    string  sb = TemplateHandlerPrams.Structure.Id.ToString() + '_' + TemplateHandlerPrams.Factor.Id.ToString();
                    Thread.Sleep(500);
                    int cnt = ReportCounter.Get(sb);
                    cnt += 1;
                    DateTime dtTime = TemplateHandlerPrams.Date;
                    if (TemplateHandlerPrams.TemplateFileName.Contains("SwDailyReportThy"))
                    {
                        FillBasicInformation(ww, 3, 0, "[projName]", TemplateHandlerPrams.Organization.SystemName);
                    }

                    FillBasicInformation(ww, 2, 0, "[rptNo]", this.rptNo);
                    FillBasicInformation(ww, 4, 0, "[date]",
                                         TemplateHandlerPrams.Date.AddDays((-1)).ToString("yyyy年MM月dd日"));

                    FillBasicInformation(ww, 4, 10, "[count]", cnt.ToString());
                    // 查询水位数据
                    var todayData = new Dictionary<string, string>();
                    var yesterData = new Dictionary<string, string>();

                    using (var db = new DW_iSecureCloud_EmptyEntities())
                    {
                        int stcid = TemplateHandlerPrams.Structure.Id;
                        DateTime dateToday = TemplateHandlerPrams.Date;
                        DateTime dateYesterday = TemplateHandlerPrams.Date.AddDays(-1);
                        var query = from sw in db.T_THEMES_ENVI_WATER_LEVEL
                                    from s in db.T_DIM_SENSOR
                                    where s.STRUCT_ID == stcid && s.SENSOR_ID == sw.SENSOR_ID
                                    select new
                                    {
                                        sw.ID,
                                        sw.SENSOR_ID,
                                        s.SENSOR_LOCATION_DESCRIPTION,
                                        sw.WATER_LEVEL_CUMULATIVEVALUE,
                                        sw.ACQUISITION_DATETIME
                                    };

                        var td = query.Where(d => d.ACQUISITION_DATETIME < dateToday)
                                      .GroupBy(d => d.SENSOR_ID).Select(d => d.Select(s => s.ID).Max());
                        var ys =
                            query.Where(d => d.ACQUISITION_DATETIME < dateYesterday)
                                 .GroupBy(d => d.SENSOR_ID)
                                 .Select(d => d.Select(s => s.ID).Max());

                        todayData =
                            query.Where(d => td.Contains(d.ID))
                                 .Select(d => new { d.SENSOR_LOCATION_DESCRIPTION, d.WATER_LEVEL_CUMULATIVEVALUE })
                                 .ToDictionary(d => d.SENSOR_LOCATION_DESCRIPTION,
                                               d => Convert.ToDecimal(d.WATER_LEVEL_CUMULATIVEVALUE).ToString("#0.000"));
                        yesterData =
                            query.Where(d => ys.Contains(d.ID))
                                 .Select(d => new { d.SENSOR_LOCATION_DESCRIPTION, d.WATER_LEVEL_CUMULATIVEVALUE })
                                 .ToDictionary(d => d.SENSOR_LOCATION_DESCRIPTION,
                                               d => Convert.ToDecimal(d.WATER_LEVEL_CUMULATIVEVALUE).ToString("#0.000"));
                    }
                    var sheet = ww.Worksheets[0];
                    int StartRowIndex = 6;
                    int InsertRowIndex = 8;
                    Array.Sort(Cdbh);
                    if (Cdbh.Count() > 3)
                    {
                        if (TemplateHandlerPrams.TemplateFileName.Contains("SwDailyReportThy"))
                        {
                            if (Cdbh.Count() > 4)
                            {
                                sheet.Cells.InsertRows(InsertRowIndex, Cdbh.Length - 4);
                            }
                        }
                        else
                        {
                            sheet.Cells.InsertRows(InsertRowIndex, Cdbh.Length - 3);
                        }                        
                    }
                    int structId = TemplateHandlerPrams.Structure.Id;
                    int factorId = TemplateHandlerPrams.Factor.Id;
                    List<SensorList> list = DataAccess.FindSensorsByStructAndFactor(structId, factorId);
                   foreach (SensorList sensorList in list)
                     {
                      foreach (Sensor sensor in sensorList.Sensors)
                      {
                          bool todayFlag = false;
                          bool yestodayFlag = false;
                            //测点位置
                            sheet.Cells[StartRowIndex, 0].PutValue(sensor.Location);
                            //初始高程
                            decimal? waterLevelInit = DataAccess.GetWaterLevelInit(sensor.SensorId);
                            if (waterLevelInit == null)
                            {
                                sheet.Cells[StartRowIndex, 1].PutValue("/");
                            }
                            else
                            {
                                sheet.Cells[StartRowIndex, 1].PutValue(Convert.ToDecimal(Convert.ToDecimal(waterLevelInit).ToString("#0.000")));
                            }
                            //本次高程
                            if (todayData.ContainsKey(sensor.Location) && todayData[sensor.Location] != null)
                            {
                                sheet.Cells[StartRowIndex, 2].PutValue(Convert.ToDecimal(todayData[sensor.Location]));
                            }
                            else
                            {
                                sheet.Cells[StartRowIndex, 2].PutValue("/");
                                todayFlag = true;
                            }
                            
                            //上次高程
                            if (yesterData.ContainsKey(sensor.Location) && yesterData[sensor.Location] != null)
                            {
                                sheet.Cells[StartRowIndex, 3].PutValue(Convert.ToDecimal(yesterData[sensor.Location]));
                            }
                            else
                            {
                                sheet.Cells[StartRowIndex, 3].PutValue("/");
                                yestodayFlag = true;
                            }
                            //本次变化量 
                            //变化速率
                            if (todayFlag || yestodayFlag )
                            {
                                sheet.Cells[StartRowIndex, 4].PutValue("/");
                                sheet.Cells[StartRowIndex, 6].PutValue("/");
                            }
                            else
                            {
                                var temp = (Convert.ToDecimal(todayData[sensor.Location]) - Convert.ToDecimal(yesterData[sensor.Location])) * 1000;
                                sheet.Cells[StartRowIndex, 4].PutValue(Convert.ToDecimal(temp.ToString("#0.000")));
                                sheet.Cells[StartRowIndex, 6].PutValue(Convert.ToDecimal(temp.ToString("#0.000"))); 
                            }
                            //累计变化量
                            if (waterLevelInit == null || todayFlag)
                            {
                                sheet.Cells[StartRowIndex, 5].PutValue("/");
                            }
                            else
                            {
                                var init = Convert.ToDecimal(Convert.ToDecimal(waterLevelInit).ToString("#0.000"));
                                var value = (Convert.ToDecimal(todayData[sensor.Location]) - init)*1000;
                                sheet.Cells[StartRowIndex, 5].PutValue(value);  
                            }
                            // 预警值
                            sheet.Cells[StartRowIndex, 7].PutValue(limit);
                            StartRowIndex++;
                        }
                    }
                    Thread.Sleep(500);
                    ReportCounter.Inc(sb);                    
                    ww.Save(ms, SaveFormat.Excel97To2003);
                    ms.Close();
                }
                catch (Exception ex)
                {
                    logger.Warn(ex);
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
