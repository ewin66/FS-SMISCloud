// // --------------------------------------------------------------------------------------------
// // <copyright file="ConnectTestDal.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140527
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;
using FSDE.IDAL;
using System.Data.Common;

namespace FSDE.DAL
{
    public class ConnectTestDal:IConnectTest
    {
        public bool IsConnect(string str)
        {
           if (GetSQLiteTableNames(str).Count() > 0)
            {
                using (DbConnection conn = new SQLiteConnection(str))
                {
                    try
                    {
                        conn.Open();

                        if (conn.State == ConnectionState.Open)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (System.Exception e)
                    {
                        MessageBox.Show(@"数据库连接失败");
                    }
                    finally
                    {
                        conn.Close();
                    }
                    
                }
            }
           else
           {
               MessageBox.Show(@"数据库不存在");
           }

            return false;
        }

        public string[] GetSQLiteTableNames(string connectStr)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectStr))
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
                conn.Close();
                return listNames.ToArray();
            }
        }
    }



}