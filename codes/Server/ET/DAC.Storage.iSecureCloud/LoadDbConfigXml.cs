using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace FS.SMIS_Cloud.DAC.Storage.iSecureCloud
{
    public class LoadDbConfigXml
    {
        private readonly XmlDocument _xmldoc = new XmlDocument();
        private const string ConfigRootNode = "/database/iSecureCloud";
        private const string NodeConnstr = "/database/iSecureCloud/connectionString";
        private const string NodeTables = "/database/iSecureCloud/tables";
        private const string TableCommon = "/database/iSecureCloud/tables/common";
        private const string Rawdatatable = "/database/iSecureCloud/tables/rawdatatable";

        /// <summary>
        /// 初始化加载提取数据库提取配置文件
        /// </summary>
        /// <param name="path"></param>
        public LoadDbConfigXml(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new Exception(string.Format("{0} is not exist !", path));
            var settings = new XmlReaderSettings { IgnoreComments = true };
            XmlReader reader = XmlReader.Create(path, settings);
            _xmldoc.Load(reader);
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetSqlConnectionStrings()
        {
            return this._xmldoc.SelectSingleNode(NodeConnstr).Attributes["connstr"].Value;
        }

        /// <summary>
        /// 获取各种数据表的结构
        /// </summary>
        /// <returns></returns>
        public IList<TableInfo> GeTableInfos()
        {
            string commcolums = this._xmldoc.SelectSingleNode(TableCommon).Attributes["colums"].Value; 
            XmlNode xmltableNode = this._xmldoc.SelectSingleNode(NodeTables);
            IList<TableInfo> tables = new List<TableInfo>();
            XmlNode rawdatatable = this._xmldoc.SelectSingleNode(Rawdatatable);
            tables.Add(new TableInfo(rawdatatable.Attributes["name"].Value, rawdatatable.Attributes["colums"].Value));
            foreach (var node in xmltableNode.ChildNodes.Cast<XmlElement>().Where(node => node.Name == "table"))
            {
                try
                {
                    string name = node.Attributes["name"].Value;
                    string colums = string.Format("{0}{1}", commcolums, node.Attributes["colums"].Value);
                    tables.Add(new TableInfo(name, colums));
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return tables;
        }
    }
}