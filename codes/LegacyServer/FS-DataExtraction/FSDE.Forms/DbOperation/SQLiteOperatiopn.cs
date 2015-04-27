// // --------------------------------------------------------------------------------------------
// // <copyright file="SQLiteOperatiopn.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140605
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using FSDE.BLL.Config;
using FSDE.Forms.Views;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.Forms.DbOperation
{
    public class SQLiteOperatiopn
    {

        //读取C_PRODUCT_CATEGORY表，得其中的数据
        public static IList<ProductCategory> GetDataTypeList()
        {
            DAL.Common.Initialise();
            using (DbConnection conn = new DbConnection())
            {
                {
                    var bll = new ProductCategoryBll();
                    return bll.SelectList().ToList();
                }
            }
        }

        //获取SQLite数据库中所有表的名字
        public static string[] GetSQLiteTableNames()
        {
            using (SQLiteConnection conn = new SQLiteConnection(FrmOther.SqliteConnStr))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select name from sqlite_master where type='table' order by name;";
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<string> listNames = new List<string>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listNames.Add(reader.GetString(0));
                    }
                }
                return listNames.ToArray();
            }
        }

        public static List<string> GetSQLiteFieldNames(string tableName)
        {
            using (SQLiteConnection conn = new SQLiteConnection(FrmOther.SqliteConnStr))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "PRAGMA table_info(["+tableName+"]);";
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<string> listNames = new List<string>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listNames.Add(reader.GetString(1));
                    }
                }
                return listNames;
            }
        }

        //public static List<string> 
        
    }
}