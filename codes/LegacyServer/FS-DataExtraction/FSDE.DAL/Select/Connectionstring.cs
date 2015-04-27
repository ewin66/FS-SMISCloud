#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="Connectionstring.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140603 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System.Linq;

namespace FSDE.DAL.Select
{
    using System;

    using FSDE.Model;
    using FSDE.Model.Config;

    public class Connectionstring
    {
        private const string OldAccess = "Microsoft.Jet.OLEDB.4.0";

        private const string NewAccess = "Microsoft.Ace.OleDb.12.0";

        public static string GetConnectionString(DataBaseName database)
        {
            switch (database.DataBaseType)
            {
                case (int)DataBaseType.SQLite:
                    return GetSQLiteConnectionString(database);
                case (int)DataBaseType.SQLServer:
                    return GetSQLServerConnectionString(database);
                case (int)DataBaseType.ACCESSOld:
                case (int)DataBaseType.ACCESSNew:
                    return GetAccessConnectionString(database);
                default:
                    return string.Empty;
            }
        }


        private static string GetSQLiteConnectionString(DataBaseName database)
        {
            return string.Format("Data Source={0};Version=3;Pooling=False;Max Pool Size=100", database.Location);
        }

        private static string GetSQLServerConnectionString(DataBaseName database)
        {
            string ip = null;
            if (database.Location != "")
            {
                string[] str = database.Location.Split(@":".ToCharArray());
                ip = str[0];
            }
            
            return string.Format(
                "Data Source={0},1433;Initial Catalog={1};User ID={2};Password={3}",
                ip,
                database.DataBaseCode,
                database.UserId,
                database.Password);
        }

        private static string GetAccessConnectionString(DataBaseName database)
        {
            string accessstr;
            if (database.DataBaseCode.Contains(".mdb"))
            {
                accessstr = OldAccess;
            }
            else if (database.DataBaseCode.Contains(".accdb"))
            {
                accessstr = NewAccess;
            }
            else
            {
                throw new Exception("数据库类型不匹配");
            }
            return string.Format(
                "Provider={0};Persist Security Info=False;Data Source={1}",
                accessstr,
                database.Location);
        }
    }
}