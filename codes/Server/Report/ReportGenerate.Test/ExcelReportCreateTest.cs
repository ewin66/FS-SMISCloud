using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.TemplateHandle;



namespace ReportGenerate.Test
{
     [TestFixture]
    public class ExcelReportCreateTest
    {
         [Test]
         public void ExcelDailyReport()
         {
             string fileDirPath = RPTPara.GetTemplateFilePath();
             TemplateHandleBase cjr = new CjDailyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 11, NameCN = "沉降监测日报表" }, Structure = new Structure { Id = 17 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "沉降监测日模板.xls") });
             TemplateHandleBase dxsr = new DxsDailyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 17, NameCN = "地下水位监测日报表" }, Structure = new Structure { Id = 16 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "地下水位监测日模板.xls") });
             TemplateHandleBase cxr = new CxDailyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 8, 22), Factor = new Factor { Id = 10, NameCN = "测斜监测日报表" }, Structure = new Structure { Id = 4 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "测斜监测日模板.xls") });
             TemplateHandleBase vsr = new VsDailyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 9, NameCN = "竖向位移监测日报表" }, Structure = new Structure { Id = 2 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "竖向位移监测日模板.xls") });
             TemplateHandleBase hsr = new HsDailyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 9, NameCN = "水平位移监测日报表" }, Structure = new Structure { Id = 2 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "水平位移监测日模板.xls") });
             //以下模板涉及到xml计数，在当前测试确保结构物名称不为null
             //TemplateHandleBase cxahlqr = new CxDailyUnifyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 7, 19), Factor = new Factor { Id = 10, NameCN = "测斜监测日报表" }, Structure = new Structure { Id = 2,Name = "安徽六潜测斜"}, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "安徽六潜" }, FileFullName = Path.Combine(Path.GetTempPath(), "安徽六潜测斜日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "CxDailyReportAhlq.xls") });
            // TemplateHandleBase cxncdwyr = new CxDailyUnifyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 7, 19), Factor = new Factor { Id = 10, NameCN = "测斜监测日报表" }, Structure = new Structure { Id = 26,Name = "动物园测斜"}, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "动物园" }, FileFullName = Path.Combine(Path.GetTempPath(), "围护结构测斜日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "CxDailyReportNcdwy.xls") });
            // TemplateHandleBase cxthyr = new CxDailyUnifyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 7, 19), Factor = new Factor { Id = 10, NameCN = "测斜监测日报表" }, Structure = new Structure { Id = 26 ,Name = "桃花源测斜"}, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "桃花源" }, FileFullName = Path.Combine(Path.GetTempPath(), "桃花源测斜日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "CxDailyReportThy.xls") });
            // TemplateHandleBase dxsylr = new SwDailyUnifyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 17, NameCN = "地下水位监测日报表" }, Structure = new Structure { Id = 26,Name = "桃花源地下水"}, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "桃花源" }, FileFullName = Path.Combine(Path.GetTempPath(), "桃花源地下水日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "SwDailyReportThy.xls") });
             //TemplateHandleBase dxncvylr = new SwDailyUnifyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 17, NameCN = "地下水位监测日报表" }, Structure = new Structure { Id = 72,Name = "动物园地下水"}, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "动物园" }, FileFullName = Path.Combine(Path.GetTempPath(), "围护结构地下水日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "SwDailyReportNcdwy.xls") });
           //  Assert.IsTrue(cxahlqr.WriteFile());
            // Assert.IsTrue(cxncdwyr.WriteFile());
            // Assert.IsTrue(cxthyr.WriteFile());
            // Assert.IsTrue(dxsylr.WriteFile());
             //Assert.IsTrue(dxncvylr.WriteFile());
             Assert.IsTrue(cjr.WriteFile());
             Assert.IsTrue(dxsr.WriteFile());
             Assert.IsTrue(cxr.WriteFile());
             Assert.IsTrue(vsr.WriteFile());
             Assert.IsTrue(hsr.WriteFile());
             Assert.IsTrue(ExistOrNot(Path.Combine(Path.GetTempPath(), "日报表.xls")));
         }
         [Test]
         public void ExcelWeekReport()
         {
             string fileDirPath = RPTPara.GetTemplateFilePath();
             TemplateHandleBase cxz = new CxWeekReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 8, 22), Factor = new Factor { Id = 10, NameCN = "测斜监测周报表" }, Structure = new Structure { Id = 4 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(),"周报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "测斜监测周模板.xls") });
             TemplateHandleBase dxsz = new DxsWeekReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 17, NameCN = "地下水位监测周报表" }, Structure = new Structure { Id = 16 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "周报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"地下水位监测周模板.xls") });
             TemplateHandleBase cjz = new CjWeekReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 11, NameCN = "沉降监测周报表" }, Structure = new Structure { Id = 17 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "周报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"沉降监测周模板.xls") });
             TemplateHandleBase vsz = new VsWeekReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 9, NameCN = "竖向位移监测周报表" }, Structure = new Structure { Id = 2 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "周报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"竖向位移监测周模板.xls") });
             TemplateHandleBase hsz = new HsWeekReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 5, 22), Factor = new Factor { Id = 9, NameCN = "水平位移监测周报表" }, Structure = new Structure { Id = 2 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "周报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"水平位移监测周模板.xls") });
             Assert.IsTrue(cxz.WriteFile());
             Assert.IsTrue(dxsz.WriteFile());
             Assert.IsTrue(cjz.WriteFile());
             Assert.IsTrue(vsz.WriteFile());
             Assert.IsTrue(hsz.WriteFile());
             Assert.IsTrue(ExistOrNot(Path.Combine(Path.GetTempPath(),"周报表.xls")));
         }
         [Test]
         public void ExcelMonthReport()
         {
             string fileDirPath = RPTPara.GetTemplateFilePath();
            TemplateHandleBase cjy = new CjMonthReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2015, 1, 22),  Factor = new Factor { Id = 11, NameCN = "沉降监测月报表" }, Structure = new Structure { Id = 17 },  Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(),"月报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"沉降监测月模板.xls") });
            TemplateHandleBase dxsy = new DxsMonthReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2015, 2, 22), Factor = new Factor { Id = 17, NameCN = "地下水监测月报表" }, Structure = new Structure { Id = 16 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "月报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"地下水位监测月模板.xls") });
             TemplateHandleBase cxy = new CxMonthReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2015,3, 22), Factor = new Factor { Id = 10, NameCN = "测斜监测月报表" }, Structure = new Structure { Id = 4 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "月报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"测斜监测月模板.xls") });
             TemplateHandleBase vsy = new VsMonthReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2015, 12, 22), Factor = new Factor { Id = 9, NameCN = "竖向位移监测月报表" }, Structure = new Structure { Id = 2 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "月报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"竖向位移监测月模板.xls") });
             TemplateHandleBase hsy = new HsMonthReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2015, 11, 22), Factor = new Factor { Id = 9, NameCN = "水平位移监测月报表" }, Structure = new Structure { Id = 2 }, Organization = new Organization { Id = 12, Name = "组织名称", SystemName = "系统名称" }, FileFullName = Path.Combine(Path.GetTempPath(), "月报表.xls"), TemplateFileName = Path.Combine(fileDirPath,"水平位移监测月模板.xls") });
             Assert.IsTrue(cjy.WriteFile());
             Assert.IsTrue(dxsy.WriteFile());
            Assert.IsTrue(cxy.WriteFile());
             Assert.IsTrue(vsy.WriteFile());
            Assert.IsTrue(hsy.WriteFile());
             Assert.IsTrue(ExistOrNot(Path.Combine(Path.GetTempPath(),"月报表.xls")));
         }
         [Test]
         public void ExcelgfDailyReport()
         {
             string fileDirPath = RPTPara.GetTemplateFilePath();
             TemplateHandleBase gfms = new GfmsDailyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 12, 14), Factor = new Factor { Id = 16, NameCN = "锚杆受力监测" }, Structure = new Structure { Id = 62 }, Organization = new Organization { Id = 38, Name = "组织名称", SystemName = "广佛城际轻轨基坑监测" }, FileFullName = Path.Combine(Path.GetTempPath(), "日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "GfmsDailyReport.xls") });
             TemplateHandleBase gfcj = new GfcjDailyReport(new ReportGeneratorService.ReportModule.TemplateHandlerPrams { Date = new DateTime(2014, 12, 15), Factor = new Factor { Id = 42, NameCN = "沉降分组监测" }, Structure = new Structure { Id = 62 }, Organization = new Organization { Id = 38, Name = "组织名称", SystemName = "广佛城际轻轨基坑监测" }, FileFullName = Path.Combine(Path.GetTempPath(), "日报表.xls"), TemplateFileName = Path.Combine(fileDirPath, "GfcjDailyReport.xls") });
             Assert.IsTrue(gfms.WriteFile());
             Assert.IsTrue(gfcj.WriteFile());
             Assert.IsTrue(ExistOrNot(Path.Combine(Path.GetTempPath(), "日报表.xls")));
         }

         public bool ExistOrNot(string path)
         {
             if (!File.Exists(path))
             {
                 return false;
             }
             else
             {
                 return true;
             }
         }
    }
}
