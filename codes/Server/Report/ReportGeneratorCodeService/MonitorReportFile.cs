// // --------------------------------------------------------------------------------------------
// // <copyright file="MonitorReportFile.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141024
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using FS.DynamicScript;

namespace ReportGeneratorService
{

    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using ReportGeneratorService.Common;
    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.Interface;
    using ReportGeneratorService.ReportModule;

    using log4net;

    public class MonitorReportFile : ReportFileBase
    {
        private static string ConfirmedReportPath = ConfigurationManager.AppSettings["ConfirmedReportPath"];
        private static string UnconfirmedReportPath = ConfigurationManager.AppSettings["UnconfirmedReportPath"];
        private static string DependPath = ConfigurationManager.AppSettings["DependPath"];
        private static string TemplateHandleRootDirPath = ConfigurationManager.AppSettings["TemplateHandleRootDirPath"];

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Organization organization;

        private Structure structure;

        // private Factor factor;

        private string fileFullName;

        private DateTime createDate;

        // 获取日数据时间
        private double getDataHour;

        private ReportInfo reportInfo;

        public MonitorReportFile(ReportGroup reportGroup)
            : base(reportGroup)
        {
            organization = DataAccess.GetOrganizationInfo(reportGroup.Config.OrgId);
            structure = DataAccess.GetStructureInfo(reportGroup.Config.StructId);
            // factor = DataAccess.GetFactorInfoById(reportGroup.Config.FactorId);
            createDate = reportGroup.CreateDate;
            fileFullName = GetReportFullName(reportGroup.Config.IsNeedConfirmed, reportGroup.Config.ReportName, reportGroup.Config.DateType);
            getDataHour = Convert.ToDouble(reportGroup.Config.GetDataTime);
            reportInfo = this.GetReportInfo();
        }
        /// <summary>
        /// 获取日期是月中第几周
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
        public static int GetWeekNumInMonth(DateTime daytime)
        {
            int dayInMonth = daytime.Day;
            //本月第一天
            DateTime firstDay = daytime.AddDays(1 - daytime.Day);
            //本月第一天是周几
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;
            //本月第一周有几天
            int firstWeekEndDay = 7 - (weekday - 1);
            //当前日期和第一周之差
            int diffday = dayInMonth - firstWeekEndDay;
            diffday = diffday > 0 ? diffday : 1;
            //当前是第几周,如果整除7就减一天
            int WeekNumInMonth = ((diffday % 7) == 0
             ? (diffday / 7 - 1)
             : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
            return WeekNumInMonth;
        }

        protected string GetReportFullName(bool isNeedConfirmed, string reportName, DateType type)
        {
            DateTime time = createDate;
            DateTime timeTmp;
            string reportPath;
            reportPath = (isNeedConfirmed ? UnconfirmedReportPath : ConfirmedReportPath);

            StringBuilder sb = new StringBuilder();
            //   sb.Append(reportPath);
            sb.Append(reportName);
            switch (type)
            {
                case DateType.Day:
                    timeTmp = time.AddDays(-1);
                    sb.Append("_日报表").AppendFormat("_{0}年{1:D2}月{2:D2}日", timeTmp.Year, timeTmp.Month, timeTmp.Day);
                    break;
                case DateType.Week:
                    int dayOfWeek = Convert.ToInt32(time.DayOfWeek);
                    timeTmp = time.Date.AddDays(-dayOfWeek).AddDays(-6); // 上周一
                    GregorianCalendar gc=new GregorianCalendar();//3-12
                    int weekNumInMonth = gc.GetWeekOfYear(timeTmp, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    //int weekNumInMonth = GetWeekNumInMonth(timeTmp);
                    //sb.Append("_周报表").AppendFormat("_{0}年{1:D2}月第{2:D2}周", timeTmp.Year, timeTmp.Month, weekNumInMonth);
                    sb.Append("_周报表").AppendFormat("_{0}年第{1:D2}周({2}~{3})", timeTmp.Year, weekNumInMonth,timeTmp.ToString("yyyyMMdd"),timeTmp.AddDays(+6).ToString("yyyyMMdd"));
                    break;
                case DateType.Month:
                    timeTmp = time.AddMonths(-1);
                    sb.Append("_月报表").AppendFormat("_{0}年{1:D2}月", timeTmp.Year, timeTmp.Month);
                    break;
                case DateType.Year:
                    sb.Append("_年报表").AppendFormat("_{0}年", time.AddYears(-1).Year);
                    break;
            }
            sb.Append(ReportGroup.ExtName);
            reportPath = Path.Combine(reportPath, time.Year.ToString(), time.Month.ToString("D2"), time.Day.ToString("D2"), sb.ToString());
            return reportPath;
        }

        /// <summary>
        /// 模板处理类所需参数转换
        /// </summary>
        /// <param name="template">配置参数</param>
        /// <returns>处理类所需参数</returns>
        protected TemplateHandlerPrams GetHandlerPara(ReportTemplate template)
        {
            TemplateHandlerPrams retValue = new TemplateHandlerPrams();
            retValue.Organization = this.organization;
            retValue.Structure = this.structure;
            retValue.Factor = DataAccess.GetFactorInfoById(template.FactorId);
            retValue.FileFullName = this.fileFullName;
            retValue.TemplateFileName = template.FullName;
            retValue.Date = this.createDate.Date.AddHours(getDataHour);
            return retValue;
        }

        /// <summary>
        /// 获取报表编号
        /// </summary>
        /// <remarks>
        /// 6位结构物Id + 6位报表配置Id + 2位报表类型 + (创建时间 - “1970-01-01”经过的毫秒数)
        /// </remarks>
        /// <returns>报表编号</returns>
        protected string GetFileId()
        {
            var sb = new StringBuilder(50);
            //   6位结构物Id + 8位报表配置Id + 创建时间 - “1970-01-01”经过的毫秒数
            //sb.AppendFormat("{0:D6}", this.ReportGroup.Config.StructId).AppendFormat("-{0:D8}", this.ReportGroup.Config.Id);
            sb.AppendFormat("{0:D6}", this.ReportGroup.Config.StructId)
                .AppendFormat("-{0:D6}", this.ReportGroup.Config.Id)
                .AppendFormat("-{0:D2}", (int)this.ReportGroup.Config.DateType);
            sb.Append("-").Append((long)(this.createDate - new DateTime(1970, 1, 1)).TotalMilliseconds);
            return sb.ToString();
        }

        /// <summary>
        /// 创建任务返回值
        /// </summary>
        /// <param name="result">任务执行状态</param>
        /// <returns>任务返回值</returns>
        protected ReportTaskResult CreateTaskResult(Result result)
        {
            ReportTaskResult ret = new ReportTaskResult();
            ret.Result = result;
            if (result == Result.Successful)
            {
                ret.ReportInfo = this.reportInfo;
            }
            return ret;
        }

        /// <summary>
        /// 获取新创建的报表信息
        /// </summary>
        /// <returns>返回报表信息</returns>
        protected ReportInfo GetReportInfo()
        {
            return new ReportInfo
            {
                CreatedDate = this.createDate,
                FullName = this.fileFullName,
                Id = this.GetFileId(),
                Name = Path.GetFileNameWithoutExtension(this.fileFullName),
                Statue = !ReportGroup.Config.IsNeedConfirmed,
                OrgId = ReportGroup.Config.OrgId,
                StructureId = ReportGroup.Config.StructId,
                DateType = ReportGroup.Config.DateType
            };
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return this.reportInfo;
            }
        }

        /// <summary>
        /// 删除报表文件
        /// </summary>
        /// <remarks>报表生成失败时，删除该报表</remarks>
        /// <param name="reportName">报表全名称</param>
        private void DeleteWrongReport(string reportName)
        {
            if (File.Exists(reportName))
            {
                try
                {
                    File.Delete(reportName);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("删除错误文件{0}失败", reportName), e);
                }
            }
        }

        /// <summary>
        /// 创建新报表
        /// </summary>
        /// <returns>新报表信息</returns>
        protected override ReportTaskResult Create()
        {
            //ReportTaskResult result;
            Log.InfoFormat("开始生成【{0}】", ReportInfo.Name);
            string filePath = Path.GetDirectoryName(reportInfo.FullName);

            if (!Directory.Exists(filePath))
            {
                if (!DirectoryHelper.CreateDirectory(filePath))
                {
                    Log.Error(string.Format("创建路径{0}，失败", filePath));
                    return CreateTaskResult(Result.Failed);
                }
            }

            if (File.Exists(ReportInfo.FullName))
            {
                if (!DeleteOldFile(reportInfo))
                {
                    Log.Info(string.Format("删除已有{0}失败，本次不生成新文件", reportInfo.FullName));
                    return CreateTaskResult(Result.Failed);
                }
            }
            Log.Debug("创建目录成功");
            foreach (var template in ReportGroup.Templates)
            {
                TemplateHandlerPrams para = this.GetHandlerPara(template);
                var cp = new object[] { para };
                try
                {
                    string templateHandleRootDirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplateHandleRootDirPath));
                    string dependPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DependPath));
                    string templateHandlePath = GetTemplateHandlePath(templateHandleRootDirPath, template.HandleName);
                    Log.Debug("开始写文件");
                    CrossDomainCompiler.Call(templateHandlePath, dependPath, typeof(TemplateHandleBase), "WriteFile", ref cp);
                    Log.Debug("写文件结束");
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("{0}生成失败", ReportInfo.Name), e);
                    DeleteWrongReport(this.fileFullName);
                    return CreateTaskResult(Result.Failed);
                }
            }

            Log.InfoFormat("生成【{0}】成功", ReportInfo.Name);
            return CreateTaskResult(Result.Successful);
        }

        private bool DeleteOldFile(ReportInfo reportInfo)
        {
            try
            {
                File.Delete(reportInfo.FullName);
            }
            catch (Exception e)
            {

                Log.Error("删除旧文件失败", e);
                return false;
            }
            return ReportConfigDal.DeleteOldReportInfo(reportInfo.Name);
        }

        /// <summary>
        /// 获取模板处理类的全路径
        /// </summary>
        /// <param name="templateHandleRootPath">模板处理类的根目录</param>
        /// <param name="handleName">模板处理类名称</param>
        /// <returns></returns>
        public static string GetTemplateHandlePath(string templateHandleRootPath, string handleName)
        {
            string fileFullName = string.Empty;
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(templateHandleRootPath);
                foreach (FileInfo fileInfo in Dir.GetFiles("*.cs", SearchOption.AllDirectories))
                {
                    fileFullName = fileInfo.FullName;
                    var fileName = Path.GetFileNameWithoutExtension(fileFullName);
                    if (fileName == handleName)
                    {
                        break;
                    }
                }
                return fileFullName;
            }
            catch (Exception e)
            {
                Log.Error("获取模板处理类路径失败", e);
                throw e;
            }
        }
    }
   
}