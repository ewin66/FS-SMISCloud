using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace ReportGenerate.Test
{
    using ReportGeneratorService;
    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.Interface;

    [TestFixture]
    public class TestDBAccess
    {
        [Test]
        public void TestGetReportConfig()
        {
            List<ReportGroup> data = ReportConfigDal.GetReportConfig();
            Assert.IsNotNull(data); 
        }

        [Test]
        public void TestSaveAndDeleteReportInfo()
        {
            ReportGroup reportGroup = TestReportCreate.GetMonthReportGroup();
            ReportFileBase file = new MonitorReportFile(reportGroup);
            Assert.IsTrue(ReportConfigDal.SaveReportInfo(file.ReportInfo));

            int fileRec;
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                fileRec = db.T_REPORT_COLLECTION.Where(r => r.Name == file.ReportInfo.Name).ToList().Count;
            }
            Assert.IsTrue(fileRec > 0);

            Assert.IsTrue(ReportConfigDal.DeleteOldReportInfo(file.ReportInfo.Name));
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                fileRec = db.T_REPORT_COLLECTION.Where(r => r.FileFullName == file.ReportInfo.FullName).ToList().Count;
            }
            Assert.IsTrue(fileRec == 0);
        }
    }
}
