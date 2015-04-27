using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerate.Test
{
    using System.Configuration;

    using NUnit.Framework;

    using ReportGeneratorService;
    using ReportGeneratorService.Common;
    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.Interface;
    using System.IO;
    [TestFixture]
    public class DirectoryHelperTest
    {
        [Test]
        public void CreateDirectoryTest()
        {
            string temp = GetDirectoryName();
            DirectoryHelper.CreateDirectory(temp);
            Assert.IsTrue(Directory.Exists(temp));
        }

        private static string GetDirectoryName()
        {
            ReportGroup reportGroup = TestReportCreate.GetMonthReportGroup();
            ReportFileBase file = new MonitorReportFile(reportGroup);
            return Path.GetDirectoryName(file.ReportInfo.FullName);
        }

        [Test]
        public void GetTemplateFilePathTest()
        {
            string filePath = RPTPara.GetTemplateFilePath();
            Console.WriteLine(filePath);
            Assert.IsTrue(File.Exists(Path.Combine(filePath, "CxDailyReportAhlq.xls")));
        }
    }
}
