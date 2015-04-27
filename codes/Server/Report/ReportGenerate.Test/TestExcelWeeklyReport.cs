/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：TestExcelWeeklyReport.cs
// 功能描述：
// 
// 创建标识： 2015/2/28 16:55:29
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
    public class TestExcelWeeklyReport
    {
        [Test]
        public void TestCxWeekReport()
        {
            string template = "测斜监测周模板.xls";
            string reportName = "测斜周报表.xls";
            string handleName = "CxWeekReport";
            DateTime date = new DateTime(2014, 8, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 10;
            int structId = 4;
            int orgId = 22;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestDxsWeekReport()
        {
            string template = "地下水位监测周模板.xls";
            string reportName = "地下水位周报.xls";
            string handleName = "DxsWeekReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 17;
            int structId = 16;
            int orgId = 22;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestCjWeekReport()
        {
            string template = "沉降监测周模板.xls";
            string reportName = "沉降监测周报表.xls";
            string handleName = "CjWeekReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 11;
            int structId = 17;
            int orgId = 22;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestVsWeekReport()
        {
            string template = "竖向位移监测周模板.xls";
            string reportName = "竖向位移监测周报表.xls";
            string handleName = "VsWeekReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 9;
            int structId = 2;
            int orgId = 22;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestHsWeekReport()
        {
            string template = "水平位移监测周模板.xls";
            string reportName = "水平位移监测周报表.xls";
            string handleName = "HsWeekReport";
            DateTime date = new DateTime(2014, 5, 22, 0, 0, 0, DateTimeKind.Local);
            int factorId = 9;
            int structId = 2;
            int orgId = 22;
            TemplateHandlerPrams para = TestPinghanReport.CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = TestPinghanReport.CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

    }
}
