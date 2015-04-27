namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGDAC.Tran.Db;

    public class LoadDbConfigXml
    {
        private readonly XmlDocument _xmldoc = new XmlDocument();

        /// <summary>
        /// 加载提取数据库提取配置文件
        /// </summary>
        /// <param name="path"></param>
        public LoadDbConfigXml(string path)
        {
            var settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(path, settings);
            this._xmldoc.Load(reader);
        }

        /// <summary>
        /// 获取配置文件中所有表的信息
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public DataSourseTableInfo[] GetDataSourseTableInfo(string node)
        {
            XmlNode xns = this._xmldoc.SelectSingleNode(node);
            XmlNodeList dbnodelst = xns.ChildNodes;
            if (dbnodelst.Count <= 0)
                return null;
            var tableInfos = new List<DataSourseTableInfo>();
            foreach (XmlNode dbNode in dbnodelst)
            {
                var xe = (XmlElement) dbNode;
                string[] dbInfo = this.GetDbInfo(xe);
                var dbtype = (DbType) int.Parse(dbInfo[1].Trim());
                XmlNodeList tablenodelst = xe.ChildNodes;
                if (tablenodelst.Count <= 0)
                    continue;
                foreach (XmlNode tablenode in tablenodelst)
                {
                    var tablexn = (XmlElement) tablenode;
                    DataSourseTableInfo tableInfo = this.GetTableInfo(tablexn);
                    tableInfo.DataBaseName = dbInfo[0];
                    tableInfo.DbType = dbtype;
                    tableInfo.ConnectionString = dbInfo[2];
                    tableInfos.Add(tableInfo);
                }
            }
            return tableInfos.ToArray();
        }

        /// <summary>
        /// 获取数据库信息
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        private string[] GetDbInfo(XmlElement xe)
        {
            string dbnamestr = xe.GetAttribute("name");
            string dbtypestr = xe.GetAttribute("dbType");
            string connectionString = xe.GetAttribute("connectionString");
            return new[] {dbnamestr, dbtypestr, connectionString};
        }
        
        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="tablexn"></param>
        /// <returns></returns>
        private DataSourseTableInfo GetTableInfo(XmlElement tablexn)
        {
            int datacont = 0;
            string[][] columses = this.GetColums(tablexn.ChildNodes, out datacont);
            var tableInfo = new DataSourseTableInfo
            {
                TableName = tablexn.GetAttribute("name"),
                Colums = columses[0],
                StandardFields = columses[1],
                Filter = tablexn.GetAttribute("filter"),
                Type =  uint.Parse(tablexn.GetAttribute("protocolType")),
                DataCount = datacont
            };
            return tableInfo;
        }

        /// <summary>
        /// 根据表节点获取列
        /// </summary>
        /// <param name="columlst"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private string[][] GetColums(XmlNodeList columlst,out int count)
        {
            count = 0;
            if (columlst.Count <= 0)
            {
                return new string[2][];
            }
            var colums = new string[columlst.Count];
            var standardfields = new string[columlst.Count];
            for(int i=0;i< columlst.Count;i++)
            {
                var columxe = (XmlElement)columlst[i];
                colums[i] = columxe.GetAttribute("name").Trim();
                standardfields[i] = columxe.GetAttribute("standardfield").Trim();
                if (standardfields[i].StartsWith("value", StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }
            return new[] { colums, standardfields };
        }

        public string[] GetSqlConnectionStrings(string node)
        {
            XmlNode xns = this._xmldoc.SelectSingleNode(node);
            if (xns != null)
            {
                XmlNodeList dbnodelst = xns.ChildNodes;
                if (dbnodelst.Count <= 0)
                    return null;
                var connectionstrs = new List<string>();
                foreach (var xe in dbnodelst.Cast<XmlElement>())
                {
                    try
                    {
                        string[] dbInfo = this.GetDbInfo(xe);
                        if (dbInfo != null && dbInfo.Length == 3)
                            connectionstrs.Add(dbInfo[2]);
                    }
                    catch (Exception)
                    {
                        // TODO logg输出 
                        continue;
                    }
                }
                return connectionstrs.ToArray();
            }
            return null;
        }
    }
}