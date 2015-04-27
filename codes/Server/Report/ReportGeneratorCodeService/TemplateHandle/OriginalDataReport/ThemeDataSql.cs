using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReportGeneratorService.TemplateHandle
{
    using System.Collections;
    using System.Reflection;
    using System.Xml.Linq;

    using log4net;

    public static class ThemeDataSql
    {
        private static Dictionary<int, string> themeSql;

        private static string fileName;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static ThemeDataSql()
        {
            themeSql = FromXml(fileName);
        }

        public static string GetSql(int factorId, List<int> sensorIds)
        {
            StringBuilder sb = new StringBuilder();
            string sql;
            if (!themeSql.TryGetValue(factorId, out sql))
            {
                return string.Empty;
            }

            sb.Append(sql);
            sb.Append(GetSqlCondition(sensorIds));
            return sb.ToString();
        }

        private static Dictionary<int, string> FromXml(string fileName)
        {
            Dictionary<int, string> sql = new Dictionary<int, string>();
            
            try
            {
                XDocument doc = XDocument.Load(fileName);
                var nodes =
                    from t in
                        doc.Descendants("Factor")
                    select new { Id = t.Element("Id").Value, Sql = t.Element("Sql").Value };

                foreach (var node in nodes)
                {
                    if (node.Id.Trim() == string.Empty)
                    {
                        continue;
                    }
                    string[] ids = node.Id.Split(',');
                    foreach (var id in ids)
                    {
                        sql.Add(Convert.ToInt32(id), node.Sql);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("获取原始数据Sql配置信息失败！", e);
            }
            return sql;

        }

        private static string GetSqlCondition(List<int> sensors)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (int i=0; i<sensors.Count; i++)
            {
                sb.Append(sensors[i]);
                if (i != sensors.Count - 1)
                {
                     sb.Append(",");
                }
                else
                {
                    sb.Append(")");
                }
            }
            return sb.ToString();
        }
    }
}
