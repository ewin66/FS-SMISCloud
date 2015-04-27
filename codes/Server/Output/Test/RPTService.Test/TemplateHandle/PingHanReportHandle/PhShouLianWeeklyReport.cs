using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspose.Cells;
using log4net;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class PhShouLianWeeklyReport : TemplateHandleBase
    {
        private int flag = 0;// 传感器标识： 0 实体
        private int status = 1;// 截面施工进度: 1 施工中
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool ExistOrNot { get; set; }
        private int structId { get; set; }
        private int factorId { get; set; }
        private string factorName { get; set; }
        private DateTime dtTime { get; set; }
        private string template { get; set; }
        private string targetFile { get; set; }

        public PhShouLianWeeklyReport(TemplateHandlerPrams para)
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
            // 获取测量日期列表
            List<DateTime> dateList = PingHanReport.GetLastWeekDateList(dtTime);

            // 获取施工中的隧道截面及其下的传感器
            List<SectionSensors> sectionSensors = new List<SectionSensors>();
            try
            {
                sectionSensors = DataAccess.GetProcessingSensor(structId, factorId, flag, status);
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
                        sheet.Cells[3, 15].PutValue(sectionSensors[i].SectionName);//  截面名称
                        List<Sensor> sensors = sectionSensors[i].Sensors;
                        if (sensors.Count > 0)
                        {
                            int rowIndex = 7;
                            foreach (var sensor in sensors)
                            {
                                // 插入7行
                                sheet.Cells.InsertRows(rowIndex, 7);
                                //为每个测点填写7行对应一周数据
                                int sensorId = sensor.SensorId;
                                var sensorLocation = sensor.Location;

                                InitValues initValue = DataAccess.GetSensorInitValue(sensorId);
                                decimal? len0 = initValue.len0;
                                int startDataRow = rowIndex;
                                bool lastFlag = false;
                                decimal? lastAvgData = 0;// 上次测量的平均值
                                // 获得上一次的测量值
                                var getLastFirstStartDate = dateList[0].AddDays(-1);
                                var getLastFirstData = new Dictionary<string, MonitorData>();
                                var getLastSecondData = new Dictionary<string, MonitorData>();
                                var lastFirstData = new MonitorData();
                                var lastSecondData = new MonitorData();
                                PingHanReport.GetFirstAndSecondMinitorData(getLastFirstStartDate, new[] { sensorId }, out getLastFirstData,
                                    out getLastSecondData);

                                if (lastFirstData.Data != null && lastSecondData.Data != null)
                                {
                                    if (lastFirstData.Data[0].Values.Any() && lastSecondData.Data[0].Values.Any())
                                    {
                                        decimal? temp = (lastFirstData.Data[0].Values[0] + lastSecondData.Data[0].Values[0] + 2 * 1000 * len0) / 2;
                                        decimal z = Math.Floor(Convert.ToDecimal(temp / 1000));
                                        decimal x = Convert.ToDecimal(Convert.ToDecimal(temp % 1000).ToString("#0.0"));
                                        lastAvgData = z * 1000 + x;// 上次测量的平均值
                                    }
                                    else
                                    {
                                        lastFlag = true;
                                    }
                                }
                                else
                                {
                                    lastFlag = true;
                                }
                                for (int j = 0; j < 7; j++)
                                {
                                    // 测点
                                    sheet.Cells[startDataRow, 0].PutValue(sensorLocation);
                                    //测量日期
                                    sheet.Cells[startDataRow, 1].PutValue(dateList[j].Year);
                                    sheet.Cells[startDataRow, 2].PutValue(dateList[j].Month);
                                    sheet.Cells[startDataRow, 3].PutValue(dateList[j].Day);
                                   
                                    decimal? currentAvgData = 0;// 本次测量的平均值
                                    bool curFlag = false;
                                    var firstData = new MonitorData();
                                    var secondData = new MonitorData();
                                    var getFirstStartDate = PingHanReport.GetLastWeekEachDay((j + 1).ToString(), dtTime);
                                    var getFirstData = new Dictionary<string, MonitorData>();
                                    var getSecondData = new Dictionary<string, MonitorData>();
                                   PingHanReport.GetFirstAndSecondMinitorData(getFirstStartDate, new[] { sensorId }, out getFirstData,
                                        out getSecondData);
                                    // 测量值1
                                    PingHanReport.GetMonitorData(getFirstData, sensorLocation, ref firstData);
                                    PingHanReport.GetMonitorData(getLastFirstData, sensorLocation, ref lastFirstData);                             
                                    // 测量值2
                                    PingHanReport.GetMonitorData(getSecondData, sensorLocation, ref secondData);
                                    PingHanReport.GetMonitorData(getLastSecondData, sensorLocation, ref lastSecondData);
                                    //观测值1
                                    PingHanReport.FillDataCrackReport(sheet, firstData, len0, startDataRow, 6);
                                    //观测值2
                                    PingHanReport.FillDataCrackReport(sheet, secondData, len0, startDataRow, 8);
                                    //平均值
                                    if (firstData.Data != null && secondData.Data != null)
                                    {
                                        if (firstData.Data[0].Values.Any() && secondData.Data[0].Values.Any())
                                        {
                                            decimal z1 = Convert.ToDecimal(sheet.Cells[startDataRow,6].Value);
                                            decimal x1 = Convert.ToDecimal(sheet.Cells[startDataRow, 7].Value);
                                            decimal z2 = Convert.ToDecimal(sheet.Cells[startDataRow, 8].Value);
                                            decimal x2 = Convert.ToDecimal(sheet.Cells[startDataRow, 9].Value);
                                            decimal? temp = (z1 * 1000 + z2 * 1000 + x1 + x2) / 2;
                                            decimal z = Math.Floor(Convert.ToDecimal(temp / 1000));
                                            decimal x = Convert.ToDecimal(Convert.ToDecimal(temp % 1000).ToString("#0.0"));
                                            currentAvgData = z * 1000 + x;
                                            //平均值
                                            sheet.Cells[startDataRow, 10].PutValue(z);
                                            sheet.Cells[startDataRow, 11].PutValue(x);
                                            // 修正后观测值
                                            sheet.Cells[startDataRow, 13].PutValue(z);
                                            sheet.Cells[startDataRow, 14].PutValue(x);
                                            //相对第一次收敛值                                          
                                            sheet.Cells[startDataRow, 15].PutValue(Convert.ToDecimal(Convert.ToDecimal(len0 * 1000 - currentAvgData).ToString("#0.0")));
                                        }
                                        else
                                        {
                                            curFlag = true;                                           
                                        }
                                    }
                                    else
                                    {
                                        curFlag = true;                                       
                                    }
                                    if (curFlag)
                                    {
                                        //平均值
                                        sheet.Cells[startDataRow, 10].PutValue("/");
                                        sheet.Cells[startDataRow, 11].PutValue("/");
                                        // 修正后观测值
                                        sheet.Cells[startDataRow, 13].PutValue("/");
                                        sheet.Cells[startDataRow, 14].PutValue("/");
                                        //相对第一次收敛值
                                        sheet.Cells[startDataRow, 15].PutValue("/");
                                    }
                                   
                                   if(!curFlag && !lastFlag)
                                    {
                                        var varValue = currentAvgData - lastAvgData;
                                        sheet.Cells[startDataRow, 16].PutValue(Convert.ToDecimal(Convert.ToDecimal(varValue).ToString("#0.0"))); // 相对上一次收敛值
                                        sheet.Cells[startDataRow, 18].PutValue(Convert.ToDecimal(Convert.ToDecimal(varValue).ToString("#0.0"))); // 收敛速率
                                    }
                                   else
                                   {
                                       sheet.Cells[startDataRow, 16].PutValue("/"); // 相对上一次收敛值
                                       sheet.Cells[startDataRow, 18].PutValue("/"); // 收敛速率
                                   }                               
                                      lastAvgData = currentAvgData;
                                      lastFlag = curFlag;
                                   
                                    sheet.Cells[startDataRow, 17].PutValue(1);//时间间隔
                                    startDataRow++;
                                }
                                rowIndex += 7;
                            }
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
