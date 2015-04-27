#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="LoadXmlTest.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141127 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FS.SMIS_Cloud.DAC.Storage.SQLite;
using NUnit.Framework;

namespace DAC.Storage.SQLite.Test
{
    [TestFixture]
    public class LoadDbConfigXmlTest
    {
        private readonly XmlDocument _xmldoc = new XmlDocument();
        private const string ConfigRootNode = "/database/iSecureCloud";
        private const string NodeConnstr = "/database/iSecureCloud/connectionString";
        private const string NodeTables = "/database/iSecureCloud/tables";
        private const string TableCommon = "/database/iSecureCloud/tables/common";
        private string path = ".\\ThemeTables_SQLite.xml";
        
        [SetUp]
        public void LoadDbConfigXml()
        {
            if (!System.IO.File.Exists(path))
                throw new Exception(string.Format("{0} is not exist !", path));
            var settings = new XmlReaderSettings {IgnoreComments = true};
            XmlReader reader = XmlReader.Create(path, settings);
            _xmldoc.Load(reader);
        }

         [Test]
        public void TestLoadDbConfigXml()
        {
             try
             {
                 var loadDbConfig = new LoadDbConfigXml(string.Empty);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message);
                 Assert.AreEqual(" is not exist !", ex.Message);
             }
        }

        [Test]
        public void TestGetConnstr()
        {
            var settings = new XmlReaderSettings {IgnoreComments = true};
            XmlReader reader = XmlReader.Create(path, settings);
            _xmldoc.Load(reader);
            XmlNode xns = this._xmldoc.SelectSingleNode(ConfigRootNode);
            if (xns != null)
            {
                XmlElement dbnode = xns.ChildNodes.Cast<XmlElement>()
                    .FirstOrDefault();
                if (dbnode != null && dbnode.Name == "connectionString")
                {
                    var xmlElement = dbnode.GetAttribute("connstr");
                    Console.WriteLine(xmlElement);
                    Assert.AreEqual(
                        @"FSUSDB\FSUSDataValueDB.db3",
                        xmlElement);
                }
            }
        }

        [Test]
        public void GetSqlConnectionStrings()
        {
            XmlNode xmlconnstrNode = this._xmldoc.SelectSingleNode(NodeConnstr);
            string connstr = xmlconnstrNode.Attributes["connstr"].Value;
            Console.WriteLine(connstr);
            Assert.AreEqual(
                @"FSUSDB\FSUSDataValueDB.db3", connstr);
        }

        [Test]
        public void GeTableInfos()
        {
            XmlNode xmltablecommNode = this._xmldoc.SelectSingleNode(TableCommon);
            string commcolums = xmltablecommNode.Attributes["colums"].Value;
            XmlNode xmltableNode = this._xmldoc.SelectSingleNode(NodeTables);
            IList<TableInfo> tables = new List<TableInfo>();
            foreach (var node in xmltableNode.ChildNodes.Cast<XmlElement>())
            {
                if (node.Name == "table")
                {
                    string name = node.Attributes["name"].Value;
                    string colums = string.Format("{0}{1}", commcolums, node.Attributes["colums"].Value);
                    tables.Add(new TableInfo(name, colums));
                }
            }
            Assert.AreEqual(9, tables.Count);
        }

        private LoadDbConfigXml loadxml;

        [SetUp]
        public void LoadXml()
        {
            loadxml = new LoadDbConfigXml(path);
        }

        [Test]
        public void TestGetSqlConnectionStrings()
        {
            string connstr = loadxml.GetSqlConnectionStrings();
            Assert.AreEqual(
                @"FSUSDB\FSUSDataValueDB.db3", connstr);
        }

        [Test]
        public void TestGeTableInfos()
        {
            IList<TableInfo> tablelst = loadxml.GeTableInfos();
            Assert.AreEqual(9, tablelst.Count);
        }

        [Test]
        public void TestGetTableMaps()
        {
            IDictionary<uint, TableInfo> tableInfos = loadxml.GetTableMaps();
            Assert.AreEqual(16,tableInfos.Count);
        }

    }
}