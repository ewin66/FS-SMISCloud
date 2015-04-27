#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SelectConfigTableInfoDal.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140625 by WIN .
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
using System.Reflection;
using log4net;

namespace FSDE.DAL.Select
{
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    using FreeSun.Common.DB;

    using FSDE.Dictionaries;
    using FSDE.IDAL;
    using FSDE.Model;
    using FSDE.Model.Config;

    public class SelectConfigTableInfoDal : ISelectTablesDal
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //读取数据库中多有表的数据到内存中
        public DataSet Select(DataBaseName database)
        {
            string connstr = Connectionstring.GetConnectionString(database);
            StringBuilder sqlstr = new StringBuilder();
            List<ConfigTable> tables = ConfigTableDic.GetConfigTableDic().SelecConfigTables((int) database.ID);
            for (int i = 0; i < tables.Count; i++)
            {
                string str = string.Format(
                    "select {0} as DataBaseID, '{1}' as TableName, ",
                    (int) database.ID,
                    tables[i].TableName);
                sqlstr.Append(str)
                    .Append("[")
                    .Append(tables[i].SensorId)
                    .Append("],[")
                    .Append(tables[i].ModuleNo);

                if (!string.IsNullOrEmpty(tables[i].ChannelId))
                {
                    sqlstr.Append("],[").Append(tables[i].ChannelId);
                }
                if (!string.IsNullOrEmpty(tables[i].OtherFlag))
                {
                    sqlstr.Append("],[").Append(tables[i].OtherFlag);
                }
                sqlstr.Append("] from ").Append(tables[i].TableName);
                if (i != tables.Count - 1)
                {
                    sqlstr.Append(";");
                }
            }
            switch (database.DataBaseType)
            {
                case (int) DataBaseType.SQLite:
                    try
                    {
                        var dbHelper = new FreeSun.Common.DB.DbHelperSqLiteP(connstr);
                        return dbHelper.Query(sqlstr.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormat("{0} :{1}", ex.Message, sqlstr);
                    }
                    return new DataSet();
                case (int) DataBaseType.SQLServer:
                    try
                    {
                        var sqlhelper = new DbHelperSQLP(connstr);
                        return sqlhelper.Query(sqlstr.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormat("{0} :{1}", ex.Message, sqlstr);
                    }
                    return new DataSet();
                case (int) DataBaseType.ACCESSOld:
                case (int) DataBaseType.ACCESSNew:
                    var oledbhelper = new DbHelperOleDbP(connstr);
                    string[] sqlstrings = sqlstr.ToString().Split(';');
                    DataSet[] ds = new DataSet[sqlstrings.Length];
                    for (int i = 0; i < sqlstrings.Length; i++)
                    {
                        try
                        {
                            ds[i] = oledbhelper.Query(sqlstrings[i]);
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorFormat("{0} :{1}", ex.Message, sqlstrings[i]);
                        }
                    }
                    DataSet retSet = new DataSet();
                    for (int i = 0; i < sqlstrings.Length; i++)
                    {
                        retSet.Merge(ds[i]);
                    }
                    return retSet;
                default:
                    return new DataSet();
            }
        }
    }
}