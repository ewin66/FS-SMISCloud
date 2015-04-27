/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：TestExcelDailyReport.cs
// 功能描述：
// 
// 创建标识： 2015/2/28 13:35:48
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
using ReportGeneratorService.ReportModule;

namespace ReportGenerate.Test
{
    [TestFixture]
    public class TestExcelDailyReport
    {
        [Test]
        public void TestCjDailyReport()
        {
            string template = "沉降监测日模板.xls";
            string reportName = "沉降监测日报表.xls";
            string handleName = "CjDailyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 11;
            int structId = 17;
            int orgId = 12;
            try
            {
                TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
                string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
                Assert.IsTrue(File.Exists(reportFullName));
            }
            catch (Exception e)
            {
                throw e;
            }
           
        }

        [Test]
        public void TestDxsDailyReport()
        {
            string template = "地下水位监测日模板.xls";
            string reportName = "地下水位监测日报表.xls";
            string handleName = "DxsDailyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 17;
            int structId = 16;
            int orgId = 12;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestCxDailyReport()
        {
            string template = "测斜监测日模板.xls";
            string reportName = "测斜监测日报表.xls";
            string handleName = "CxDailyReport";
            DateTime date = new DateTime(2014, 8, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 10;
            int structId = 4;
            int orgId = 12;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestVsDailyReport()
        {
            string template = "竖向位移监测日模板.xls";
            string reportName = "竖向位移监测日报表.xls";
            string handleName = "VsDailyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 9;
            int structId = 2;
            int orgId = 12;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestHsDailyReport()
        {
            string template = "水平位移监测日模板.xls";
            string reportName = "水平位移监测日报表.xls";
            string handleName = "HsDailyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 9;
            int structId = 2;
            int orgId = 12;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestAhlqCxDailyUnifyReport()
        {
            string template = "CxDailyReportAhlq.xls";
            string reportName = "安徽六潜测斜日报表.xls";
            string handleName = "CxDailyUnifyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 10;
            int structId = 2;
            int orgId = 12;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestDwyCxDailyUnifyReport()
        {
            string template = "CxDailyReportNcdwy.xls";
            string reportName = "动物园围护结构测斜日报表.xls";
            string handleName = "CxDailyUnifyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 10;
            int structId = 72;
            int orgId = 22;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestThyCxDailyUnifyReport()
        {
            string template = "CxDailyReportThy.xls";
            string reportName = "桃花源测斜日报表.xls";
            string handleName = "CxDailyUnifyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 10;
            int structId = 26;
            int orgId = 12;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestThySwDailyUnifyReport()
        {
            string template = "SwDailyReportThy.xls";
            string reportName = "桃花源地下水位监测日报表.xls";
            string handleName = "SwDailyUnifyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 17;
            int structId = 26;
            int orgId = 12;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestDwySwDailyUnifyReport()
        {
            string template = "SwDailyReportNcdwy.xls";
            string reportName = "动物园围护结构地下水位监测日报表.xls";
            string handleName = "SwDailyUnifyReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 17;
            int structId = 72;
            int orgId = 22;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestGfmsDailyReport()
        {

            string template = "GfmsDailyReport.xls";
            string reportName = "广佛城际轻轨基坑锚杆受力监测日报表.xls";
            string handleName = "GfmsDailyReport";
            DateTime date = new DateTime(2014, 12, 14, 0, 0, 0, DateTimeKind.Local);
            int factorId = 16;
            int structId = 62;
            int orgId = 38;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestGfcjDailyReport()
        {
            string template = "GfcjDailyReport.xls";
            string reportName = "广佛城际轻轨基坑沉降分组监测日报表.xls";
            string handleName = "GfcjDailyReport";
            DateTime date = new DateTime(2014, 12, 15, 0, 0, 0, DateTimeKind.Local);
            int factorId = 42;
            int structId = 62;
            int orgId = 38;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId,
                template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }
    }
}
