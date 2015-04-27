/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：TestGetTemplateHandlePath.cs
// 功能描述：
// 
// 创建标识： 2015/2/28 11:00:14
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
using NUnit.Framework;
using ReportGeneratorService;

namespace ReportGenerate.Test
{
    [TestFixture]
   public  class TestGetTemplateHandlePath
    {
        private string TemplateHandleRootDirPath = ConfigurationManager.AppSettings["TemplateHandleRootDirPath"];
        [Test]
        public void TestGetTemplateHandleFullPath()
        {
            string templateHandleRootDirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplateHandleRootDirPath));
            string handlePath = MonitorReportFile.GetTemplateHandlePath(templateHandleRootDirPath, "PhShouLianWeeklyReport");
            Assert.IsTrue(File.Exists(handlePath));
        }
    }
}
