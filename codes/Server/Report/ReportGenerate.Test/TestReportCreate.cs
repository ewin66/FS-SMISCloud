using System;
using System.IO;
using System.Text;
using System.Configuration;
using NUnit.Framework;
using ReportGeneratorService;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;
using ReportGeneratorService.TemplateHandle;

namespace ReportGenerate.Test
{
    [TestFixture]
    public class TestReportCreate
    {
        private static string TemplatePath = ConfigurationManager.AppSettings["TemplatePath"];

        [Test]
        public void MonthReportFileByWordTest()
        {
            ReportGroup reportGroup = GetMonthReportGroup();
            ReportFileBase file = new MonitorReportFile(reportGroup);
            ReportTaskResult result = file.CreateNewFile();
            Assert.IsTrue(CheckResult(reportGroup, result));
        }

        [Test]
        public void DailyReportFileByExcelTest()
        {
            ReportGroup reportGroup = GetDailyReportGroup();
            ReportFileBase file = new MonitorReportFile(reportGroup);
            ReportTaskResult result = file.CreateNewFile();
            Assert.IsTrue(CheckResult(reportGroup, result));
        }

        [Test]
        public void ReportInfoTest()
        {
            ReportGroup reportGroup = GetMonthReportGroup();
            ReportFileBase file = new MonitorReportFile(reportGroup);
            Assert.IsTrue(CheckReportInfo(reportGroup, file.ReportInfo));
        }

        private bool CheckReportInfo(ReportGroup reportGroup, ReportInfo reportInfo)
        {
            Console.WriteLine("ID：{0}，名称：{1}，全名称:{2}", reportInfo.Id, reportInfo.Name, reportInfo.FullName);
            return (reportInfo.DateType == reportGroup.Config.DateType)
                   && (reportInfo.OrgId == reportGroup.Config.OrgId)
                   && (reportInfo.StructureId == reportGroup.Config.StructId)
                   && (reportInfo.Statue == !reportGroup.Config.IsNeedConfirmed);
        }

        private bool CheckResult(ReportGroup reportGroup, ReportTaskResult result)
        {
            if (result.Result != Result.Successful) 
                return false;
            return CheckReportInfo(reportGroup, result.ReportInfo);
        }

        public static ReportGroup GetDailyReportGroup()
        {
            ReportGroup reportGroup = new ReportGroup();
            //reportGroup.Config = new ReportConfig();
            //reportGroup.Templates = new List<ReportTemplate>();
            reportGroup.Config.DateType = DateType.Day;
            reportGroup.Config.ReportName = "myTestExcelReport";
            reportGroup.Config.IsNeedConfirmed = false;
            reportGroup.Config.OrgId = 15;
            reportGroup.Config.StructId = 3;
            reportGroup.Config.Id = 7;
            reportGroup.CreateDate = new DateTime(2014, 8, 1);
            ReportTemplate template = new ReportTemplate();
            template.Name = "测斜监测日模板.xls";
            template.HandleName = "CxDailyReport";
            template.FactorId = 10;
            reportGroup.Templates.Add(template);
            return reportGroup;
        }

        public static ReportGroup GetMonthReportGroup()
        {
            ReportGroup reportGroup = new ReportGroup();
            //reportGroup.Config = new ReportConfig();
            //reportGroup.Templates = new List<ReportTemplate>();
            reportGroup.Config.DateType = DateType.Month;
            reportGroup.Config.ReportName = "myTestReport";
            reportGroup.Config.IsNeedConfirmed = true;
            reportGroup.Config.OrgId = 17;
            reportGroup.Config.StructId = 23;
            reportGroup.Config.Id = 99;
            reportGroup.CreateDate = new DateTime(2014,8,1);
            ReportTemplate template = new ReportTemplate();
            template.Name = "StructureMonthlyReport.docx";
            template.HandleName = "StructureMonthlyReportTemplateHandler";
            reportGroup.Templates.Add(template);
            return reportGroup;
        }
    }
}
