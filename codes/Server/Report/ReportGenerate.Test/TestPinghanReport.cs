/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：PinghanReportTest.cs
// 功能描述：
// 
// 创建标识： 2015/2/28 10:54:30
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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using FS.DynamicScript;
using NUnit.Framework;
using ReportGeneratorService;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;

namespace ReportGenerate.Test
{
    [TestFixture]
    public class TestPinghanReport
    {
        public static string DependPath = ConfigurationManager.AppSettings["DependPath"];
        public static string TemplateHandleRootDirPath = ConfigurationManager.AppSettings["TemplateHandleRootDirPath"];

        [Test]
        public void TestPhShouLianWeeklyReport()
        {
            string template = "坪汉净空收敛监测周模板.xls";
            string reportName = "坪汉净空收敛监测_周报表.xls";
            string handleName = "PhShouLianWeeklyReport";
            DateTime date = new DateTime(2015, 1, 19, 0, 0, 0, DateTimeKind.Local);
            int factorId = 41; // 净空收敛
            int structId = 82; // 坪汉 
            int orgId = 48;
            TemplateHandlerPrams para = CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestPhSettlementWeeklyReport()
        {
            string template = "坪汉拱顶沉降监测周模板.xls";
            string reportName = "坪汉沉降监测_周报表.xls";
            string handleName = "PhSettlementWeeklyReport";
            DateTime date = new DateTime(2015, 1, 19, 0, 0, 0, DateTimeKind.Local);
            int factorId = 40; // 拱顶沉降
            int structId = 82; // 坪汉 
            int orgId = 48;
            TemplateHandlerPrams para = CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestPhCjWeeklyReport()
        {
            string template = "PhCjWeeklyReport.xls";
            string reportName = "坪汉沉降监测_周报表(自定义模板).xls";
            string handleName = "PhSettlementWeeklyReport";
            DateTime date = new DateTime(2015, 1, 19, 0, 0, 0, DateTimeKind.Local);
            int factorId = 40; // 拱顶沉降
            int structId = 82; // 坪汉 
            int orgId = 48;
            TemplateHandlerPrams para = CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        [Test]
        public void TestPhCrackWeeklyReport()
        {
            string template = "PhCrackWeeklyReport.xls";
            string reportName = "坪汉净空收敛监测_周报表(自定义模板).xls";
            string handleName = "PhShouLianWeeklyReport";
            DateTime date = new DateTime(2015, 1, 19, 0, 0, 0, DateTimeKind.Local);
            int factorId = 41; // 净空收敛
            int structId = 82; // 坪汉 
            int orgId = 48;
            TemplateHandlerPrams para = CreateTemplateHandlerPrams(date, factorId, structId, orgId, template, reportName);
            string reportFullName = CallTemplateHandle(para, handleName);
            Assert.IsTrue(File.Exists(reportFullName));
        }

        public  static string CallTemplateHandle(TemplateHandlerPrams para, string handleName)
        {
            const string method = "WriteFile";
            string dependPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DependPath));
            string templateHandleRootDirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplateHandleRootDirPath));
            string templateHandlePath = MonitorReportFile.GetTemplateHandlePath(templateHandleRootDirPath, handleName);
            var cp = new object[] { para };
            CrossDomainCompiler.Call(templateHandlePath, dependPath, typeof(TemplateHandleBase), method, ref cp);
            return para.FileFullName;
        }

        public static TemplateHandlerPrams CreateTemplateHandlerPrams(DateTime date, int factorId, int structId, int orgId, string template, string reportName)
        {
            string fileDirPath = RPTPara.GetTemplateFilePath();
            TemplateHandlerPrams paPrams = new TemplateHandlerPrams()
            {
                Date = date,
                Factor = DataAccess.GetFactorInfoById(factorId),
                Structure = DataAccess.GetStructureInfo(structId),
                // Organization = DataAccess.GetOrganizationInfo(orgId),
                Organization = new Organization { Id = orgId, Name = "组织名称", SystemName = "系统名称" },
                FileFullName = Path.Combine(Path.GetTempPath(), reportName),
                TemplateFileName = Path.Combine(fileDirPath, template)
            };
            return paPrams;
        }

    }
}
