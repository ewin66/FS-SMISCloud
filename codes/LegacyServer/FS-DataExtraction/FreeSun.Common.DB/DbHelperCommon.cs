#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DbHelperCommon.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140605 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FreeSun.Common.DB
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class DbHelperCommon
    {
        public static void AddDataSetTableNames(string sqlstring, ref DataSet ds)
        {
            string[] sqlstr = sqlstring.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> tableNames = new List<string>();
            foreach (string s in sqlstr)
            {
                string[] ss = s.Split(new[] { "FROM", "from", "From" }, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length == 2)
                {
                    string name = ss[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    tableNames.Add(name.Trim());
                }
            }

            if (tableNames.Count == ds.Tables.Count)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    ds.Tables[i].TableName = tableNames[i];
                }
            }
        }
    }
}