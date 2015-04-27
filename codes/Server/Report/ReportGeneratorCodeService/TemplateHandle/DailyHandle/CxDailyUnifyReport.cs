using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Aspose.Cells;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.ReportModule;
using log4net;
using ReportGeneratorService.Dal;

namespace ReportGeneratorService.TemplateHandle
{
    public class CxDailyUnifyReport : DailyReport
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _existOrNot = false;
        private decimal limitValue = 30;// 控制值
        private decimal warnValue = 20;// 预警值
        public CxDailyUnifyReport(TemplateHandlerPrams param)
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
            DateTime dtTime = TemplateHandlerPrams.Date;
            var groups = new List<CxGroup>();
            // 根据传感器列表创建sheet页
            try
            {
                using (var db = new DW_iSecureCloud_EmptyEntities())
                {
                    var stcId = TemplateHandlerPrams.Structure.Id;
                    var fctId = TemplateHandlerPrams.Factor.Id;
                    groups = (from s in db.T_DIM_SENSOR
                              from sg in db.T_DIM_SENSOR_GROUP_CEXIE
                              from g in db.T_DIM_GROUP
                              where
                                  s.SENSOR_ID == sg.SENSOR_ID && sg.GROUP_ID == g.GROUP_ID && s.STRUCT_ID == stcId
                                  && s.SAFETY_FACTOR_TYPE_ID == fctId
                              select new { g.GROUP_ID, g.GROUP_NAME, sg.DEPTH }).ToList()
                        .GroupBy(g => new { g.GROUP_ID, g.GROUP_NAME })
                        .Select(
                            s =>
                            new CxGroup
                            {
                                Id = s.Key.GROUP_ID,
                                Name = s.Key.GROUP_NAME,
                                Depth = s.Select(d => d.DEPTH * -1).OrderBy(d => d).ToArray()
                            })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                ms.Close();
                throw ex;
            }
            if (groups.Count > 0)
            {
                try
                {
                    string sb = TemplateHandlerPrams.Structure.Id.ToString() + '_' + TemplateHandlerPrams.Factor.Id.ToString();
                    Thread.Sleep(300);
                    int cs = ReportCounter.Get(sb);
                    cs += 1;
                    // 复制sheet
                    ww.Worksheets[0].Name = groups[0].Name ?? "测斜管" + (0 + 1);
                    for (int i = 1; i < groups.Count; i++)
                    {
                        var index = ww.Worksheets.AddCopy(0);
                        ww.Worksheets[index].Name = groups[i].Name ?? "测斜管" + (i + 1);
                    }

                    // 填写数据
                    for (int i = 0; i < ww.Worksheets.Count; i++)
                    {
                        var sheet = ww.Worksheets[i];
                        // 设置项目编号
                        var rptNo = sheet.Cells[2, 0].StringValue;
                        sheet.Cells[2, 0].PutValue(rptNo.Replace("[rptNo]", this.rptNo));
                        // 设置项目名称
                        if (!TemplateHandlerPrams.TemplateFileName.Contains("CxDailyReportNcdwy"))
                        {
                            var projName = sheet.Cells[3, 0].StringValue;
                            sheet.Cells[3, 0].PutValue(projName.Replace("[projName]",
                                                                        TemplateHandlerPrams.Organization.SystemName));
                        }
                        // 设置监测点信息
                        var info = sheet.Cells[4, 0].StringValue;
                        sheet.Cells[4, 0].PutValue(
                            info.Replace("[date]", TemplateHandlerPrams.Date.AddDays(-1).ToString("yyyy年MM月dd日"))
                                .Replace("[sensor]", sheet.Name));
                        // 设置生成次数
                        var cout = sheet.Cells[4, 8].StringValue;
                        sheet.Cells[4, 8].PutValue(cout.Replace("[count]", cs.ToString()));
                        // 获取本次累积位移                
                        var dataToday = DataAccess.GetByGroupDirectAndDateGroupByTime(
                            groups[i].Id,
                            TemplateHandlerPrams.Date.AddDays(-1).Date, /*当前时间0时0分0秒*/
                            TemplateHandlerPrams.Date.Date.AddSeconds(-1) /*当前时间23:59:59*/);
                        // 获取上次累积位移
                        var dataYesterday = DataAccess.GetByGroupDirectAndDateGroupByTime(
                            groups[i].Id,
                            TemplateHandlerPrams.Date.AddDays(-2).Date, /*前一天0时0分0秒*/
                            TemplateHandlerPrams.Date.AddDays((-1)).Date.AddSeconds(-1) /*前一天23:59:59*/);
                        int startRowIndex = 6;
                        #region 测斜管组下的深度个数如果超出模板设定范围,动态增加行
                        int InsertRowIndex = 8;
                        if (groups[i].Depth.Length > 3)
                        {
                            sheet.Cells.InsertRows(InsertRowIndex, groups[i].Depth.Length - 3);
                        }
                        #endregion
                        // 设置数据
                        foreach (var depth in groups[i].Depth)
                        {
                            // 设置深度
                            sheet.Cells[startRowIndex, 0].PutValue(depth);

                            // 上次累积位移
                            decimal? yesterdayValue = 0;
                            if (dataYesterday.Count != 0)
                            {
                                var depthValue = dataYesterday[0].Data.FirstOrDefault(d => d.Depth * -1 == depth);
                                if (depthValue != null)
                                {
                                    yesterdayValue = depthValue.YValue;
                                }
                            }
                            // 本次累积位移
                            decimal? todayValue = 0;
                            if (dataToday.Count != 0)
                            {
                                var depthValue = dataToday[0].Data.FirstOrDefault(d => d.Depth * -1 == depth);
                                if (depthValue != null)
                                {
                                    todayValue = depthValue.YValue;
                                }
                            }
                            // 变化量
                            decimal? dlta = todayValue - yesterdayValue;

                            // 设置数据
                            sheet.Cells[startRowIndex, 1].PutValue(dlta);
                            sheet.Cells[startRowIndex, 2].PutValue(yesterdayValue);
                            sheet.Cells[startRowIndex, 3].PutValue(todayValue);

                            sheet.Cells[startRowIndex, 5].PutValue(warnValue);
                            sheet.Cells[startRowIndex, 6].PutValue(limitValue);

                            startRowIndex++;
                        }
                    }
                    Thread.Sleep(300);
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
