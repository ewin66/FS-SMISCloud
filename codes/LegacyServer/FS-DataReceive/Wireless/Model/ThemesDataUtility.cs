using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Reflection;
using log4net;

namespace DataCenter.Model
{
    public struct TableDataInfo
    {
        public PropertyInfo ColumnProperty;

        public ColumnAttribute ColumnAttr;
    }

    public class ThemesDataUtility
    {
        private static Dictionary<string, List<TableDataInfo>> AllThemesTableInfo;

        // private static Dictionary<string, Type> TableNameType;

        private static readonly ILog Log = LogManager.GetLogger(typeof(ThemesDataUtility));

        private static List<Type> AllTableTypes; 


        static ThemesDataUtility()
        {
            if (AllThemesTableInfo == null)
            {
                InitializeAllThemesTableInfo();
            }
        }


        /// <summary>
        ///  初 始 化 所 有 实 体 类 信 息
        /// </summary>
        private static void InitializeAllThemesTableInfo()
        {
            if (AllThemesTableInfo == null)
            {
                AllThemesTableInfo = new Dictionary<string, List<TableDataInfo>>();
            }
            else
            {
                AllThemesTableInfo.Clear();
            }

            List<Type> allType = GetAllThemesType();
            List<TableDataInfo> tableInfos;
            TableDataInfo tableInfo;
            string tableName;
            foreach (Type i in allType)
            {
                var tableattr = (TableAttribute[])i.GetCustomAttributes(typeof(TableAttribute), false);
                if (tableattr.Length != 1)
                {
                    continue;
                }

                tableName = tableattr[0].Name;
                PropertyInfo[] infos = i.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                tableInfos = new List<TableDataInfo>();
                foreach (PropertyInfo j in infos)
                {
                    try
                    {
                        tableInfo = new TableDataInfo();
                        tableInfo.ColumnProperty = j;

                        var colattr = (ColumnAttribute[])j.GetCustomAttributes(typeof(ColumnAttribute), false);

                        if (colattr.Length == 1)
                        {
                            tableInfo.ColumnAttr = colattr[0];
                            tableInfos.Add(tableInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                       Log.Warn(ex.Message);
                    }
                }

                if (tableInfos.Count == 0)
                {
                    continue;
                }

                AllThemesTableInfo.Add(tableName, tableInfos);
            }
        }

        /// <summary>
        ///  所 有 主 题 实 体 类 的 Type 集 合
        /// </summary>
        /// <returns>
        ///  返 回 所 有 主题实体类的Type集合
        /// </returns>
        public static List<Type> GetAllThemesType()
        {
            if (AllTableTypes == null)
            {
                AllTableTypes = new List<Type>();

                // var allType = new List<Type>();
                string assemblyName = System.Configuration.ConfigurationManager.AppSettings["DataModelAssembly"];
                string spaceName = System.Configuration.ConfigurationManager.AppSettings["DataModelSpace"];

                Assembly ass;
                try
                {
                    ass = Assembly.Load(assemblyName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    return AllTableTypes;
                }

                Type[] t = ass.GetTypes();
                for (int i = 0; i < t.Length; i++)
                {
                    var ns = t[i].Namespace;
                    if (ns != null && (ns.Equals(spaceName) && t[i].IsClass))
                    {
                        AllTableTypes.Add(t[i]);
                    }
                }
            }

            return AllTableTypes;
        }

        /// <summary>
        /// 根据实体类的Type类型获取表名称
        /// </summary>
        /// <param name="Data">实体类Type</param>
        public static string GetThemeDataName(Type Data)
        {
            string retVal = null;
            var tableattr = (TableAttribute[])Data.GetCustomAttributes(typeof(TableAttribute), false);
            if (tableattr.Length == 1)
            {
                retVal = tableattr[0].Name;
            }

            return retVal;
        }

        /// <summary>
        /// The get theme class type.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public static Type GetThemeClassType(string tableName)
        {
            Type type = null;
            List<Type> types = GetAllThemesType();
            foreach (var typ in types)
            {
                try
                {
                    var x = (TableAttribute[])typ.GetCustomAttributes(typeof(TableAttribute), false);
                    if (x.Length == 1 && x[0].Name == tableName)
                    {
                        type = typ;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    throw;
                }
            }

            return type;
        }

        /// <summary>
        /// 获取某个表的列属性
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="Data">返回结果</param>
        public static void GetOneThemeTableColInfo(string tableName, out List<TableDataInfo> Data)
        {
            Data = null;
            try
            {
                AllThemesTableInfo.TryGetValue(tableName, out Data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取某个表的列特性
        /// </summary>
        /// <param name="tableName">表名称</param>
        public static List<ColumnAttribute> GetThemesDataTableColumnAttribute(string tableName)
        {
            List<TableDataInfo> Data;
            try
            {
                GetOneThemeTableColInfo(tableName, out Data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }

            if (null == Data)
            {
                return null;
            }

            var ret = new List<ColumnAttribute>(Data.Count);
            foreach (TableDataInfo i in Data)
            {
                ret.Add(i.ColumnAttr);
            }

            return ret;
        }




    }
}