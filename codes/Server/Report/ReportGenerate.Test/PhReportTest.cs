/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：PhReportTest.cs
// 功能描述：
// 
// 创建标识： 2015/1/22 17:12:59
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
using System.IO;
using NUnit.Framework;
using ReportGeneratorService.Dal;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;
using ReportGeneratorService.TemplateHandle;

namespace ReportGenerate.Test
{
    [TestFixture]
    public class PhReportTest
    {
        [Test]
        public void PhCjWeeklyReportTest()
        {
            string template = "PhCjWeeklyReport.xls";
            string fullName = "坪汉沉降监测_周报表(自定义模板).xls";
            TemplateHandleBase handleBase = new PhCjWeeklyReport(CreateCjTemplateHandlerPrams(template, fullName));
            bool ret = handleBase.WriteFile();
            Assert.IsTrue(ret);
        }

        [Test]
        public void PhCrackWeeklyReportTest()
        {
            string template = "PhCrackWeeklyReport.xls";
            string fullName = "坪汉净空收敛监测_周报表(自定义模板).xls";
            TemplateHandleBase handleBase = new PhCrackWeeklyReport(CreateCrackTemplateHandlerPrams(template, fullName));
            bool ret = handleBase.WriteFile();
            Assert.IsTrue(ret);

        }

        [Test]
        public void PhSettlementWeeklyReportTest()
        {
            string template = "坪汉拱顶沉降监测周模板.xls";
            string fullName = "坪汉沉降监测_周报表.xls";
            TemplateHandleBase handleBase = new PhSettlementWeeklyReport(CreateCjTemplateHandlerPrams(template, fullName));
            bool ret = handleBase.WriteFile();
            Assert.IsTrue(ret);
        }

        [Test]
        public void PhShouLianWeeklyReportTest()
        {
            string template = "坪汉净空收敛监测周模板.xls";
            string fullName = "坪汉净空收敛监测_周报表.xls";
            TemplateHandleBase handleBase = new PhShouLianWeeklyReport(CreateCrackTemplateHandlerPrams(template, fullName));
            bool ret = handleBase.WriteFile();
            Assert.IsTrue(ret);

        }

        private TemplateHandlerPrams CreateCjTemplateHandlerPrams(string template, string fullName)
        {
            string fileDirPath = RPTPara.GetTemplateFilePath();
           
            TemplateHandlerPrams paPrams = new TemplateHandlerPrams()
            {
                Date = new DateTime(2015, 1, 19, 0, 0, 0, DateTimeKind.Local),
                Factor = DataAccess.GetFactorInfoById(40),
                Structure = DataAccess.GetStructureInfo(82),
                Organization = DataAccess.GetOrganizationInfo(48),
                FileFullName = Path.Combine(Path.GetTempPath(), fullName),
                TemplateFileName = Path.Combine(fileDirPath, template)
            };
            return paPrams;
        }

        private TemplateHandlerPrams CreateCrackTemplateHandlerPrams(string template, string fullName)
        {
            string fileDirPath = RPTPara.GetTemplateFilePath();

            TemplateHandlerPrams paPrams = new TemplateHandlerPrams()
            {
                Date = new DateTime(2015, 1, 19, 0, 0, 0, DateTimeKind.Local),
                Factor = DataAccess.GetFactorInfoById(41),
                Structure = DataAccess.GetStructureInfo(82),
                Organization = DataAccess.GetOrganizationInfo(48),
                FileFullName = Path.Combine(Path.GetTempPath(), fullName),
                TemplateFileName = Path.Combine(fileDirPath, template)
            };
            return paPrams;
        }

    }


}
