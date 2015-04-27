/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：PhSettlementWeeklyReport.cs
// 功能描述：
// 
// 创建标识： 2015/1/30 16:57:21
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
// </summary>

//----------------------------------------------------------------*/
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
    public class PhSettlementWeeklyReport : TemplateHandleBase
    {
        private int flag = 2;// 传感器标识： 2组合
        private int status = 1;// 截面施工进度: 1 施工中
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool ExistOrNot { get; set; }
        private int structId { get; set; }
        private int factorId { get; set; }
        private string factorName { get; set; }
        private DateTime dtTime { get; set; }
        private string template { get; set; }
        private string targetFile { get; set; }

        public PhSettlementWeeklyReport(TemplateHandlerPrams para)
            : base(para)
        {
            Init();
            PingHanReport.CheckParamValid(structId, factorId, template, logger);

        }

        private void Init()
        {
            structId = TemplateHandlerPrams.Structure.Id;
            factorId = TemplateHandlerPrams.Factor.Id;
            factorName = TemplateHandlerPrams.Factor.NameCN;
            dtTime = TemplateHandlerPrams.Date;
            ExistOrNot = false;
            template = TemplateHandlerPrams.TemplateFileName;
            targetFile = TemplateHandlerPrams.FileFullName;
        }

        private int GetSectionSensorCount(List<SectionSensors> sectionSensors)
        {
            int sensorCount = 0;
            if (sectionSensors.Count > 0)
            {
                for (int i = 0; i < sectionSensors.Count; i++)
                {
                    if (sectionSensors[i].Sensors.Any())
                    {
                        sensorCount += sectionSensors[i].Sensors.Count;
                    }
                }
            }
            return sensorCount;
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
            string tmpname = "PH-10" + factorName;
            int  sensorCount = 0;
            var flagSensorCount = false;
            if (sectionSensors.Count > 0)
            {
                try
                {
                    ww.Worksheets[0].Name = tmpname;
                    var sheet = ww.Worksheets[0];
                    sheet.Cells.InsertRows(8, 6);
                    int count = 1;
                    for (int i = 0; i < sectionSensors.Count; i++)
                    {
                        List<Sensor> sensors = sectionSensors[i].Sensors;
                        if (sensors.Any())
                        {
                            for (int k = 0; k < sensors.Count; k++)
                            {
                                sensorCount++;
                                if (sensorCount > 5)
                                {
                                    flagSensorCount = true;
                                    break;
                                }
                                int sensorId = sectionSensors[i].Sensors[k].SensorId;
                                var sensorLocation = sectionSensors[i].Sensors[k].Location;
                                InitValues initValue = DataAccess.GetSensorInitValue(sensorId);
                                decimal? initHeight = initValue.h0;

                                sheet.Cells[4, count].PutValue(sensorLocation);
                                sheet.Cells[5, count].PutValue(initHeight);

                                int startDataRow = 7;
                                for (int j = 0; j < 7; j++)
                                {
                                    MonitorData currentData = new MonitorData();
                                    var getDate = PingHanReport.GetLastWeekEachDay((j + 1).ToString(), dtTime);
                                    var getData = DataAccess.GetUnifyData(new[] { sensorId }, getDate,
                                        getDate.AddDays(1).AddMilliseconds(-1), true);

                                    // 本次累计变量
                                    if (getData.Count != 0)
                                    {
                                        currentData = getData.ContainsKey(sensorLocation)
                                            ? getData[sensorLocation]
                                            : new MonitorData();
                                        if (currentData.Data != null && currentData.Data[0].Values != null)
                                        {
                                            sheet.Cells[startDataRow, count + 1].PutValue(currentData.Data[0].Values[0]);
                                        }
                                        else
                                        {
                                            sheet.Cells[startDataRow, count + 1].PutValue("/");
                                        }
                                    }
                                    else
                                    {
                                        sheet.Cells[startDataRow, count + 1].PutValue("/");
                                    }

                                    if (currentData.Data != null && currentData.Data[0].Values != null)
                                    {
                                        // 本次高程
                                        decimal? bcgc = initHeight - currentData.Data[0].Values[0] / 1000;
                                        sheet.Cells[startDataRow, count].PutValue(bcgc);
                                    }
                                    else
                                    {
                                        sheet.Cells[startDataRow, count].PutValue("/");
                                    }
                                    //测量日期
                                    sheet.Cells[startDataRow, 0].PutValue(getDate.ToShortDateString());
                                    startDataRow++;
                                }
                                count += 2;
                            }
                        }
                        if (flagSensorCount)
                        {
                            break;
                        }
                    }
                    if (flagSensorCount)
                    {
                        logger.InfoFormat("施工中截面下的测点超过报表模板..{0}..设定的数目,暂只取模板设定的测点的数目,其余测点数据报表不填充", template);
                    }
                    if (ExistOrNot)
                    {
                        md.Worksheets.RemoveAt(tmpname);
                        Worksheet tmp = md.Worksheets.Add(tmpname);
                        tmp.Copy(ww.Worksheets[0]);
                    }
                    else
                    {
                        md.Worksheets[0].Copy(ww.Worksheets[0]);
                        md.Worksheets[0].Name = tmpname;
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
                logger.WarnFormat("结构物ID：{0}，监测因素ID：{1} 施工状态下的截面没检测到可用传感器", Convert.ToString(structId), Convert.ToString(factorId));
                ms.Close();
                throw new ArgumentException();
            }
        }
    }
}
