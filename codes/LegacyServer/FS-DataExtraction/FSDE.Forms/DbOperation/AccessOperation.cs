// // --------------------------------------------------------------------------------------------
// // <copyright file="AccessOperation.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows.Forms;
using FSDE.BLL.Config;
using FSDE.Forms.Views;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.Forms.DbOperation
{
    public class AccessOperation
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

        /// 取所有表名
        /// </summary>
        /// <returns></returns>
        public static string[] GetAccessTableNames()
        {
            try
            {
                //获取数据表
                OleDbConnection conn = new OleDbConnection();
                conn.ConnectionString = FrmOther.AccessConnStr;
                conn.Open();
                DataTable shemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                int n = shemaTable.Rows.Count;
                string[] strTable = new string[n];
                int m = shemaTable.Columns.IndexOf("TABLE_NAME");
                for (int i = 0; i < n; i++)
                {
                    DataRow m_DataRow = shemaTable.Rows[i];
                    strTable[i] = m_DataRow.ItemArray.GetValue(m).ToString();
                }
                conn.Close();
                conn.Dispose();
                return strTable;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("指定的限制集无效:\n" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 取指定表所有字段名称
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTableFieldNameList(string TableName)
        {
            List<string> list = new List<string>();
            OleDbConnection Conn = new OleDbConnection(FrmOther.AccessConnStr);
            try
            {
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 * FROM [" + TableName + "]";
                    cmd.Connection = Conn;
                    OleDbDataReader dr = cmd.ExecuteReader();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        list.Add(dr.GetName(i));
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                MessageBox.Show(@"获取表字段失败");
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
                Conn.Dispose();
            }
            return null;
        }
    }
}