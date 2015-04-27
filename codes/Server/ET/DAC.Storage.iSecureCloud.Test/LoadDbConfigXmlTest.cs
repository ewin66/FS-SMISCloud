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
using FS.SMIS_Cloud.DAC.Storage.iSecureCloud;
using NUnit.Framework;

namespace DAC.Storage.iSecureCloud.Test
{
    [TestFixture]
    public class LoadDbConfigXmlTest
    {
        private readonly XmlDocument _xmldoc = new XmlDocument();
        private const string ConfigRootNode = "/database/iSecureCloud";
        private const string NodeConnstr = "/database/iSecureCloud/connectionString";
        private const string NodeTables = "/database/iSecureCloud/tables";
        private const string TableCommon = "/database/iSecureCloud/tables/common";
        private const string Rawdatatable = "/database/iSecureCloud/tables/rawdatatable";
        private string path = ".\\ThemeTables_iSecureCloud.xml";
        
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
                        "Initial Catalog=DW_iSecureCloud_Empty;Data Source=192.168.1.128;User Id=sa;Password=861004",
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
                "Initial Catalog=DW_iSecureCloud_Empty;Data Source=192.168.1.128;User Id=sa;Password=861004", connstr);
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
            Assert.AreEqual(17, tables.Count);
        }

        #region  数据表字段
        private const string T_THEMES_CABLE_FORCE =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,CABLE_FORCE_VALUE";

        private const string T_THEMES_DEFORMATION_BRIDGE_DEFLECTION =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,DEFLECTION_VALUE";

        private const string T_THEMES_DEFORMATION_CRACK =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,CRACK_VALUE";

        private const string T_THEMES_DEFORMATION_DEEP_DISPLACEMENT =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,DEEP_DISPLACEMENT_X_VALUE,DEEP_DISPLACEMENT_Y_VALUE,DEEP_CUMULATIVEDISPLACEMENT_X_VALUE,DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE";

        private const string T_THEMES_DEFORMATION_SETTLEMENT =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,SETTLEMENT_VALUE";

        private const string T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,SURFACE_DISPLACEMENT_X_VALUE,SURFACE_DISPLACEMENT_Y_VALUE,SURFACE_DISPLACEMENT_Z_VALUE";

        private const string T_THEMES_ENVI_RAINFALL =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,RAINFALL_VALUE";

        private const string T_THEMES_ENVI_TEMP_HUMI =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,TEMPERATURE_VALUE,HUMILITY_VALUE";

        private const string T_THEMES_ENVI_WATER_LEVEL =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,WATER_LEVEL_VALUE,WATER_LEVEL_CUMULATIVEVALUE";

        private const string T_THEMES_ENVI_WIND =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,WIND_SPEED_VALUE,WIND_DIRECTION_VALUE,WIND_ELEVATION_VALUE";

        private const string T_THEMES_FORCE_ANCHOR =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,ANCHOR_FORCE_VALUE";

        private const string T_THEMES_FORCE_EARTH_PRESSURE =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,EARTH_PRESSURE_VALUE";

        private const string T_THEMES_FORCE_STEELBAR =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,STEELBAR_FORCE_VALUE";

        private const string T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,PORE_WATER_PRESSURE_VALUE,PORE_WATER_PRESSURE_VALUE_CUMULATIVEVALUE";

        private const string T_THEMES_STRESS_STRAIN_RETAININGWALL =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,STRESS_STRAIN_VALUE";

        private const string T_THEMES_FORCE_AXIAL =
            "SENSOR_ID,SAFETY_FACTOR_TYPE_ID,ACQUISITION_DATETIME,AXIAL_FORCE_VALUE";
        #endregion 数据表字段

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
                "Initial Catalog=DW_iSecureCloud_Empty;Data Source=192.168.1.128;User Id=sa;Password=861004", connstr);
        }

        [Test]
        public void TestGeTableInfos()
        {
            IList<TableInfo> tablelst = loadxml.GeTableInfos();
            Assert.AreEqual(18, tablelst.Count);
            Assert.AreEqual("T_DATA_ORIGINAL", tablelst[0].TableName);
            Assert.AreEqual("SensorId,CollectTime,Value1,Value2,Value3,Value4", tablelst[0].Colums);

            Assert.AreEqual("T_THEMES_CABLE_FORCE", tablelst[1].TableName);
            Assert.AreEqual(T_THEMES_CABLE_FORCE, tablelst[1].Colums);

            Assert.AreEqual("T_THEMES_DEFORMATION_BRIDGE_DEFLECTION", tablelst[2].TableName);
            Assert.AreEqual(T_THEMES_DEFORMATION_BRIDGE_DEFLECTION, tablelst[2].Colums);

            Assert.AreEqual("T_THEMES_DEFORMATION_CRACK", tablelst[3].TableName);
            Assert.AreEqual(T_THEMES_DEFORMATION_CRACK, tablelst[3].Colums);

            Assert.AreEqual("T_THEMES_DEFORMATION_DEEP_DISPLACEMENT", tablelst[4].TableName);
            Assert.AreEqual(T_THEMES_DEFORMATION_DEEP_DISPLACEMENT, tablelst[4].Colums);

            Assert.AreEqual("T_THEMES_DEFORMATION_SETTLEMENT", tablelst[5].TableName);
            Assert.AreEqual(T_THEMES_DEFORMATION_SETTLEMENT, tablelst[5].Colums);

            Assert.AreEqual("T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT", tablelst[6].TableName);
            Assert.AreEqual(T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT, tablelst[6].Colums);

            Assert.AreEqual("T_THEMES_ENVI_RAINFALL", tablelst[7].TableName);
            Assert.AreEqual(T_THEMES_ENVI_RAINFALL, tablelst[7].Colums);

            Assert.AreEqual("T_THEMES_ENVI_TEMP_HUMI", tablelst[8].TableName);
            Assert.AreEqual(T_THEMES_ENVI_TEMP_HUMI, tablelst[8].Colums);

            Assert.AreEqual("T_THEMES_ENVI_WATER_LEVEL", tablelst[9].TableName);
            Assert.AreEqual(T_THEMES_ENVI_WATER_LEVEL, tablelst[9].Colums);

            Assert.AreEqual("T_THEMES_ENVI_WIND", tablelst[10].TableName);
            Assert.AreEqual(T_THEMES_ENVI_WIND, tablelst[10].Colums);

            Assert.AreEqual("T_THEMES_FORCE_ANCHOR", tablelst[11].TableName);
            Assert.AreEqual(T_THEMES_FORCE_ANCHOR, tablelst[11].Colums);

            Assert.AreEqual("T_THEMES_FORCE_EARTH_PRESSURE", tablelst[12].TableName);
            Assert.AreEqual(T_THEMES_FORCE_EARTH_PRESSURE, tablelst[12].Colums);

            Assert.AreEqual("T_THEMES_FORCE_STEELBAR", tablelst[13].TableName);
            Assert.AreEqual(T_THEMES_FORCE_STEELBAR, tablelst[13].Colums);

            Assert.AreEqual("T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE", tablelst[14].TableName);
            Assert.AreEqual(T_THEMES_STRESS_STRAIN_PORE_WATER_PRESSURE, tablelst[14].Colums);

            Assert.AreEqual("T_THEMES_STRESS_STRAIN_RETAININGWALL", tablelst[15].TableName);
            Assert.AreEqual(T_THEMES_STRESS_STRAIN_RETAININGWALL, tablelst[15].Colums);

            Assert.AreEqual("T_THEMES_FORCE_AXIAL", tablelst[16].TableName);
            Assert.AreEqual(T_THEMES_FORCE_AXIAL, tablelst[16].Colums);
        }
    }
}