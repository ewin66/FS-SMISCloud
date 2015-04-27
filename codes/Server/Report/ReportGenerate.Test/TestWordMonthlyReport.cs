/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：TestStructureMonthlyReport.cs
// 功能描述：
// 
// 创建标识： 2015/2/28 11:29:34
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
using System.Text;
using NUnit.Framework;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;
using ReportGeneratorService.TemplateHandle;

namespace ReportGenerate.Test
{
    [TestFixture]
    public class TestWordMonthlyReport
    {
        [Test]
        public void TestStructureMonthlyReportTemplateHandler()
        {
            string template = "StructureMonthlyReport.docx";
            string reportName = "结构物(边坡)月报表.docx";
            string handleName = "StructureMonthlyReportTemplateHandler";
            DateTime date = new DateTime(2014, 11, 1, 0, 0, 0, DateTimeKind.Local);
            int factorId = -1;
            int structId = 3;
            int orgId = 15;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }
        //  [Test]
        //public void TestDpWeekReport()
        //{
        //    string fileDirPath = RPTPara.GetTemplateFilePath();
        //    string template = "GfMonthlyReport.docx";
        //    string reportName = "广佛东平站周报.docx";
        //    string handleName = "StructureWeekReportTemplateHandler";
        //    DateTime date = new DateTime(2015, 2, 9, 0, 0, 0, DateTimeKind.Local);
        //    int factorId = -1;
        //    int structId = 96;
        //    int orgId = 38;
        //      TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId,
        //                                                                               template, reportName);
        //      string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
        //      Assert.IsTrue(File.Exists(reportFullName));
        //}
        //[Test]
        //  public void TestDpMonthReport()
        //  {
        //      string fileDirPath = RPTPara.GetTemplateFilePath();
        //      string template = "GfMonthlyReport.docx";
        //      string reportName = "广佛东平站月报.docx";
        //      string handleName = "StructureMonthlyReportTemplateHandler";
        //      DateTime date = new DateTime(2015, 2, 1, 0, 0, 0, DateTimeKind.Local);
        //      int factorId = -1;
        //      int structId = 96;
        //      int orgId = 38;
        //      TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId,
        //                                                                                 template, reportName);
        //      string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
        //      Assert.IsTrue(File.Exists(reportFullName));
        //  }
        [Test]
          public void TestXwzxWeekReport()
          {
              string fileDirPath = RPTPara.GetTemplateFilePath();
              string template = "GfMonthlyReport.docx";
              string reportName = "广佛新闻中心周报.docx";
              string handleName = "StructureWeekReportTemplateHandler";
              DateTime date = new DateTime(2015, 2, 9, 0, 0, 0, DateTimeKind.Local);
              int factorId = -1;
              int structId = 62;
              int orgId = 38;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId,
                                                                                     template, reportName);
              string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
              Assert.IsTrue(File.Exists(reportFullName));
          }
        [Test]
          public void TestXwzxMonthReport()
          {
              string fileDirPath = RPTPara.GetTemplateFilePath();
              string template = "GfMonthlyReport.docx";
              string reportName = "广佛新闻中心月报.docx";
              string handleName = "StructureMonthlyReportTemplateHandler";
              DateTime date = new DateTime(2015, 2, 1, 0, 0, 0, DateTimeKind.Local);
              int factorId = -1;
              int structId = 62;
              int orgId = 38;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId,
                                                                                     template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
          }
    }
}
