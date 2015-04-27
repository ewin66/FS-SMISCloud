using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerate.Test
{
    using System.Configuration;

    using NUnit.Framework;

    using ReportGeneratorService;
    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.Interface;
    using ReportGeneratorService.ReportModule;
    using ReportGeneratorService.TemplateHandle;

    [TestFixture]
    public class ReportCreateTest
    {
        //private static string ConfirmedReportPath = ConfigurationManager.AppSettings["ConfirmedReportPath"];
       // private static string UnconfirmedReportPath = ConfigurationManager.AppSettings["UnconfirmedReportPath"];
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
            if (result.Result != Result.Successful) return false;

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

        [Test]
        public void StructureMonthlyReportTemplateHandlerTest()
        {
            TemplateHandleBase handleBase = new StructureMonthlyReportTemplateHandler(CreateTemplateHandlerPrams());
          //  handleBase.TemplateHandlerPrams = CreateTemplateHandlerPrams();
            bool ret = handleBase.WriteFile();
            Assert.IsTrue(ret);
        }

        private TemplateHandlerPrams CreateTemplateHandlerPrams()
        {
            TemplateHandlerPrams paPrams = new TemplateHandlerPrams();
            string date = "2014,11,1";
            var d = date.Split(',');
            var dateTime = new DateTime(int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2]), 3, 0, 0);

            paPrams.Date = dateTime;
            string fileDirPath = RPTPara.GetTemplateFilePath();
            paPrams.TemplateFileName = Path.Combine(fileDirPath, "StructureMonthlyReport.docx");
            Organization org = DataAccess.GetOrganizationInfo(15);
            paPrams.Organization = org;
            Structure structure = DataAccess.GetStructureInfo(3);
            paPrams.Structure = structure;

            var sb = new StringBuilder(100);
            sb.Append(paPrams.Structure.Name);
            var reportDate = paPrams.Date.AddMonths(-1);
            sb.Append("_月报表").AppendFormat("_{0}年{1}月", reportDate.Year.ToString(), reportDate.Month.ToString());
            sb.Append(".docx");
            var path = Path.Combine( Path.GetTempPath() , sb.ToString());
            paPrams.FileFullName = path;

            return paPrams;
        }    
    }
}
