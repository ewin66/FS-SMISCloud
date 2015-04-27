#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SQLiteStorge.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141118 by LINGWENLONG .
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
using System.Configuration;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Consumer;
using FS.SMIS_Cloud.DAC.Task;

namespace FS.SMIS_Cloud.DAC.Storage.SQLite
{
    using FS.SMIS_Cloud.DAC.Model;

    public class SQLiteStorge : IDACTaskResultConsumer
    {
        private SQLiteDbAccessor sqliteDbAccessor;
        private const string Xmlpath = ".\\ThemeTables_SQLite.xml";

        public SQLiteStorge()
        {
            var loadxml = new LoadDbConfigXml(Xmlpath);
            try
            {
                sqliteDbAccessor = new SQLiteDbAccessor(loadxml.GetSqlConnectionStrings());
                sqliteDbAccessor.UpdateTables(loadxml.GetTableMaps());

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(" initialization MsDbAccessor is error {0}", ex.StackTrace));
            }
        }

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult source)
        {
            sqliteDbAccessor.SaveDacResult(source);
        }
    }
}