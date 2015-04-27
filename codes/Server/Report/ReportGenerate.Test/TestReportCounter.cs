/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：TestReportCounter.cs
// 功能描述：
// 
// 创建标识： 2015/3/5 13:36:16
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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;

namespace ReportGenerate.Test
{
    [TestFixture]
    public class TestReportCounter
    {
        [Test]
        public void TestCreateXmlFile()
        {
            var fileName = "test.xml";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("root"));
            xml.Save(fileName);               
            Assert.IsTrue(File.Exists(fileName));
        }
    }
}
