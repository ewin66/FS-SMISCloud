#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SelectOtherDBDal.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System.Configuration;
using System.Globalization;
using System.Linq;

namespace FSDE.DAL.Select
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.Text;

    using FreeSun.Common.DB;

    using FSDE.Dictionaries;
    using FSDE.Dictionaries.config;
    using FSDE.IDAL;
    using FSDE.Model;
    using FSDE.Model.Config;

    public class SelectOtherDBDal : ISelectTablesDal
    {
        private const string FieldNamestr = "ExtractFieldName";

        private const string acqtime = "AcqTime";

        private const string acqendtime = "EndTime";

        // 延迟时间(分钟)(仅提取到当前时间之前的某一时刻)
        private int delaytime = 0;

        public DataSet Select(DataBaseName database)
        {
            var file = new ExeConfigurationFileMap { ExeConfigFilename = @".\config\Params.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            string selectByDay = config.AppSettings.Settings["otherDBSelectByDay"].Value;
            var strdelaytime = config.AppSettings.Settings["otherDBDelayMinute"].Value;
            int.TryParse(strdelaytime, out delaytime);
            bool isSelectedByDay = false;
            if (!Boolean.TryParse(selectByDay, out isSelectedByDay))
            {
                isSelectedByDay = false;
            }
            if (isSelectedByDay)
            {
                return SelectByDay(database);
            }
            else
            {
                return SelectAll(database);
            }

        }

        #region SELECTALL
        private DataSet SelectAll(DataBaseName database)
        {
            string connstr = Connectionstring.GetConnectionString(database);
            var sqlstr = new StringBuilder();
            List<TableFieldInfo> tables =
                TableFieldInfoDic.GetTableFieldInfoDic().GetSameDataBaseTableFieldInfos((int)database.ID);

            for (int i = 0; i < tables.Count; i++)
            {
                // Select CONVERT(varchar(100), GETDATE(), 25)
               string str = string.Format(
                    "select {0} as ProjectCode,{1} as DataBaseNameID,{2} as sensorType,{3} as ACQUISITION_DATETIME,",
                    ProjectInfoDic.GetInstance().GetProjectInfo().ProjectCode,
                    database.ID,
                    tables[i].SensorType,
                    tables[i].AcqTime);
               sqlstr.Append(str);

                if (ConfigInfoTable.ConfigtableInfoDictionary.ContainsKey((int)database.ID))
                {
                    sqlstr.Append(tables[i].SensorID).Append(",");
                }
                else
                {
                    if (!string.IsNullOrEmpty(tables[i].ModuleNo))
                    {
                        sqlstr.Append(tables[i].ModuleNo).Append(",");
                    }
                    
                    if (!string.IsNullOrEmpty(tables[i].ChannelId))
                    {
                        sqlstr.Append(tables[i].ChannelId).Append(",");
                    }
                    else
                    {
                        sqlstr.Append(string.Format("{0} as channelId", 1)).Append(",");
                    }

                    if (!string.IsNullOrEmpty(tables[i].OtherFlag))
                    {
                        sqlstr.Append(tables[i].OtherFlag).Append(",");
                    }
                }
                
                Type t = tables[i].GetType();
                for (int j = 1; j <= tables[i].ValueNameCount; j++)
                {
                    System.Reflection.PropertyInfo propertyInfo = t.GetProperty(FieldNamestr + j);
                    if (!string.IsNullOrEmpty(propertyInfo.GetValue(tables[i], null).ToString()))
                        sqlstr.Append(propertyInfo.GetValue(tables[i], null)).Append(",");
                }
                sqlstr.Replace(',', ' ', sqlstr.Length - 1, 1);
                sqlstr.Append("from ")
                    .Append(tables[i].TableName)
                    .Append(" where ")
                    .Append(tables[i].AcqTime)
                    .Append(" >@")
                    .Append(acqtime)
                    .Append(i);
                if (i != tables.Count - 1)
                {
                    sqlstr.Append(";");
                }
            }
            
            List<ExtractionConfig> list = ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID);
            switch (database.DataBaseType)
            {
                case (int)DataBaseType.SQLite:
                    var sqlitepara =
                        new SQLiteParameter[tables.Count];
                    if (sqlitepara.Length > 0)
                    {
                        for (int i = 0; i < sqlitepara.Length; i++)
                        {
                            var str = new StringBuilder();
                            str.Append("@").Append(acqtime).Append(i);
                            sqlitepara[i] = new SQLiteParameter(str.ToString(),
                                this.GetLastTime(list, tables[i].TableName));
                        }
                        var sqlitehelper = new DbHelperSqLiteP(connstr);
                        try
                        {
                            return sqlitehelper.Query(sqlstr.ToString(), sqlitepara);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    break;
                case (int)DataBaseType.SQLServer:

                      var sqlpara =
                        new SqlParameter[tables.Count];
                    if (sqlpara.Length > 0)
                    {
                        for (int i = 0; i < sqlpara.Length; i++)
                        {
                            var str = new StringBuilder();
                            str.Append("@").Append(acqtime).Append(i);
                            string time = this.GetLastTime(list, tables[i].TableName);
                            //string timestr = null;
                            //string[] sliptime = time.Split(new char[]{'/',' ',':'});
                            //if (sliptime.Count() > 1)
                            //{
                            //    for (int n = 0; n < sliptime.Count(); n++)
                            //    {
                            //        if (sliptime[n].Length < 8 && sliptime[n]!="")
                            //        {
                            //            if (Convert.ToInt32(sliptime[n]) < 10 && sliptime[n].Length == 1)
                            //            {
                            //                sliptime[n] = "0" + sliptime[n];
                            //            }
                            //        }
                                    
                            //    }
                            //}
                            
                            //for (int m = 0; m < sliptime.Count(); m++)
                            //{
                            //    timestr += sliptime[m].Trim();
                            //}
                            //string[] timeformats =
                            //    {
                            //        "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff",
                            //        "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.fff", "yyyyMMddHHmmss",
                            //        "yyyyMMddHHmmss.fff", "yyyy-MM-dd h:mm:ss"
                            //    };
                            DateTime timeTemp = Convert.ToDateTime(time).AddMilliseconds(999);
                            //bool isSuccess = DateTime.TryParseExact(
                            //timestr,
                            //timeformats,
                            //CultureInfo.CurrentCulture,
                            //DateTimeStyles.None,
                            //out timeTemp); //AssumeLocal
                            //if (!isSuccess)
                            //{
                            //    timeTemp = Convert.ToDateTime(timestr);
                            //}
                            sqlpara[i] = new SqlParameter(str.ToString(), timeTemp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        }
                        var sqlhelper = new DbHelperSQLP(connstr);
                        try
                        {
                            return sqlhelper.Query(sqlstr.ToString(),300, sqlpara);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        
                    }
                    break;
                case (int)DataBaseType.ACCESSOld:
                case (int)DataBaseType.ACCESSNew:
                      var olepara =
                        new OleDbParameter[tables.Count];
                    if (olepara.Length > 0)
                    {
                        for (int i = 0; i < olepara.Length; i++)
                        {
                            var str = new StringBuilder();
                            str.Append("@").Append(acqtime).Append(i);
                            try
                            {
                               string timestr = this.GetLastTime(list, tables[i].TableName);
                               DateTime timeTemp = Convert.ToDateTime(timestr);
                               // string[] timeformats =
                               //     {
                               //         "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff",
                               //         "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.fff",
                               //         "yyyyMMddHHmmss", "yyyyMMddHHmmss.fff", "yyyy-MM-dd h:mm:ss"
                               //     };
                               //bool isSuccess = DateTime.TryParseExact(
                               //timestr,
                               //timeformats,
                               //CultureInfo.CurrentCulture,
                               //DateTimeStyles.None,
                               //out timeTemp); //AssumeLocal
                               // if (!isSuccess)
                               // {
                               //     timeTemp = Convert.ToDateTime(timestr);
                               // }
                                olepara[i] = new OleDbParameter(str.ToString(),
                                    timeTemp);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        var oledbhelper = new DbHelperOleDbP(connstr);

                        string[] sqlstrings = sqlstr.ToString().Split(';');

                        DataSet[] ds = new DataSet[olepara.Length];
                        for (int i = 0; i < olepara.Length; i++)
                        {
                            ds[i] = new DataSet();
                        }
                        for (int i = 0; i < olepara.Length; i++)
                        {
                            ds[i] = oledbhelper.Query(sqlstrings[i], olepara[i]);
                        }
                        DataSet retSet = new DataSet();
                        for (int i = 0; i < olepara.Length; i++)
                        {
                            retSet.Merge(ds[i]);
                        }
                        return retSet;
                        //return oledbhelper.Query(sqlstr.ToString(), olepara);
                    }
                    break;
                default:
                    return new DataSet();
            }
            return new DataSet();
        }

        private string GetLastTime(List<ExtractionConfig> list, string tablename)
        {
            string acqtime = "2013-01-01 00:00:00";
            foreach (ExtractionConfig extractionConfig in list)
            {
                if (string.Equals(extractionConfig.TableName, tablename, StringComparison.CurrentCultureIgnoreCase))
                {
                    acqtime = extractionConfig.Acqtime;
                    return acqtime;
                }
            }

            return acqtime;
        }
        #endregion

        #region SELECT DAY BY DAY
        /// <summary>
        /// 当待提取数据超过1天(提取历史数据)时 每次提取一天的数据
        /// </summary>
        private DataSet SelectByDay(DataBaseName database)
        {
            var connstr = Connectionstring.GetConnectionString(database);
            var sqlstr = new StringBuilder();
            var tables = TableFieldInfoDic.GetTableFieldInfoDic().GetSameDataBaseTableFieldInfos((int)database.ID);

            for (var i = 0; i < tables.Count; i++)
            {
                string str = string.Format(
                     "select {0} as ProjectCode,{1} as DataBaseNameID,{2} as sensorType,{3} as ACQUISITION_DATETIME,",
                     ProjectInfoDic.GetInstance().GetProjectInfo().ProjectCode,
                     database.ID,
                     tables[i].SensorType,
                     tables[i].AcqTime);
                sqlstr.Append(str);

                if (ConfigInfoTable.ConfigtableInfoDictionary.ContainsKey((int)database.ID))
                {
                    sqlstr.Append(tables[i].SensorID).Append(",");
                }
                else
                {
                    if (!string.IsNullOrEmpty(tables[i].ModuleNo))
                    {
                        sqlstr.Append(tables[i].ModuleNo).Append(",");
                    }

                    if (!string.IsNullOrEmpty(tables[i].ChannelId))
                    {
                        sqlstr.Append(tables[i].ChannelId).Append(",");
                    }
                    else
                    {
                        sqlstr.Append(string.Format("{0} as channelId", 1)).Append(",");
                    }

                    if (!string.IsNullOrEmpty(tables[i].OtherFlag))
                    {
                        sqlstr.Append(tables[i].OtherFlag).Append(",");
                    }
                }

                Type t = tables[i].GetType();
                for (int j = 1; j <= tables[i].ValueNameCount; j++)
                {
                    System.Reflection.PropertyInfo propertyInfo = t.GetProperty(FieldNamestr + j);
                    if (!string.IsNullOrEmpty(propertyInfo.GetValue(tables[i], null).ToString()))
                        sqlstr.Append(propertyInfo.GetValue(tables[i], null)).Append(",");
                }
                sqlstr.Replace(',', ' ', sqlstr.Length - 1, 1);
                sqlstr.Append("from ")
                    .Append(tables[i].TableName)
                    .Append(" where ")
                    .Append(tables[i].AcqTime)
                    .Append(" >@")
                    .Append(acqtime)
                    .Append(i)
                    .Append(" and ")
                    .Append(tables[i].AcqTime)
                    .Append(" <=@")
                    .Append(acqendtime)
                    .Append(i);
                if (i != tables.Count - 1)
                {
                    sqlstr.Append(";");
                }
            }

            List<ExtractionConfig> list = ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID);
            switch (database.DataBaseType)
            {
                case (int)DataBaseType.SQLite:
                    var sqlitepara =
                        new SQLiteParameter[tables.Count * 2];
                    if (sqlitepara.Length > 0)
                    {
                        var endtime1 = new DateTime[tables.Count];
                        for (int i = 0; i < tables.Count; i++)
                        {
                            string time = this.GetLastTimeOrDefaultMin(list, tables[i], database);
                            DateTime timeTemp = Convert.ToDateTime(time).AddSeconds(1);
                            endtime1[i] = GetDayStepEndTime(timeTemp);
                            sqlitepara[i * 2] = new SQLiteParameter(new StringBuilder().Append("@").Append(acqtime).Append(i).ToString(),
                                timeTemp.ToString("yyyy-MM-dd HH:mm:ss"));
                            sqlitepara[i * 2 + 1] = new SQLiteParameter(new StringBuilder().Append("@").Append(acqendtime).Append(i).ToString(),
                                endtime1[i].ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        var sqlitehelper = new DbHelperSqLiteP(connstr);
                        try
                        {
                            var s = sqlitehelper.Query(sqlstr.ToString(), sqlitepara); 
                            for (var i = 0; i < tables.Count; i++)
                            {
                                ExtractionConfigDic.GetExtractionConfigDic()
                               .UpdateExtractionConfig(
                                   new ExtractionConfig
                                   {
                                       DataBaseId = (int)database.ID,
                                       TableName = tables[i].TableName,
                                       Acqtime = endtime1[i].ToString("yyyy-MM-dd HH:mm:ss")
                                   });
                            }
                            return s;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    break;
                case (int)DataBaseType.SQLServer:

                    var sqlpara =
                      new SqlParameter[tables.Count * 2];
                    if (sqlpara.Length > 0)
                    {
                        var endtime2 = new DateTime[tables.Count];
                        for (var i = 0; i < tables.Count; i++)
                        {
                            string time = this.GetLastTimeOrDefaultMin(list, tables[i], database);
                            DateTime timeTemp = Convert.ToDateTime(time).AddSeconds(1);//这里加1秒是防止重复提取(因为时间记录中不包含毫秒数)
                            endtime2[i] = GetDayStepEndTime(timeTemp);
                            sqlpara[i * 2] = new SqlParameter(new StringBuilder().Append("@").Append(acqtime).Append(i).ToString(),
                                timeTemp.ToString("yyyy-MM-dd HH:mm:ss"));
                            sqlpara[i * 2 + 1] = new SqlParameter(new StringBuilder().Append("@").Append(acqendtime).Append(i).ToString(),
                                endtime2[i].ToString("yyyy-MM-dd HH:mm:ss"));

                        }
                        var sqlhelper = new DbHelperSQLP(connstr);
                        try
                        {
                            var s1 = sqlhelper.Query(sqlstr.ToString(), 300, sqlpara);
                            for (var i = 0; i < tables.Count; i++)
                            {
                                ExtractionConfigDic.GetExtractionConfigDic()
                               .UpdateExtractionConfig(
                                   new ExtractionConfig
                                   {
                                       DataBaseId = (int)database.ID,
                                       TableName = tables[i].TableName,
                                       Acqtime = endtime2[i].ToString("yyyy-MM-dd HH:mm:ss")
                                   });
                            }
                            return s1;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    break;
                case (int)DataBaseType.ACCESSOld:
                case (int)DataBaseType.ACCESSNew:
                    var olepara =
                      new OleDbParameter[tables.Count * 2];
                    if (olepara.Length > 0)
                    {
                        var endtime3 = new DateTime[tables.Count];
                        for (var i = 0; i < tables.Count; i++)
                        {
                            try
                            {
                                var timestr = this.GetLastTimeOrDefaultMin(list, tables[i], database);
                                var timeTemp = Convert.ToDateTime(timestr).AddSeconds(1);
                                endtime3[i] = GetDayStepEndTime(timeTemp);
                                olepara[i * 2] = new OleDbParameter(new StringBuilder().Append("@").Append(acqtime).Append(i).ToString(),
                                    timeTemp);
                                olepara[i * 2 + 1] = new OleDbParameter(new StringBuilder().Append("@").Append(acqendtime).Append(i).ToString(),
                                    endtime3[i]);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        var oledbhelper = new DbHelperOleDbP(connstr);

                        string[] sqlstrings = sqlstr.ToString().Split(';');

                        DataSet[] ds = new DataSet[tables.Count];
                        for (int i = 0; i < tables.Count; i++)
                        {
                            ds[i] = new DataSet();
                        }
                        for (int i = 0; i < tables.Count; i++)
                        {
                            //@MODIFY 2015-01-12 参数化SQL存在错误？
                            var sql = sqlstrings[i].Replace("@" + acqtime + i.ToString(), "#" + Convert.ToDateTime(olepara[i * 2].Value).ToString("yyyy-MM-dd HH:mm:ss") + "#");
                            sql = sql.Replace("@" + acqendtime + i.ToString(), "#" + Convert.ToDateTime(olepara[i * 2 + 1].Value).ToString("yyyy-MM-dd HH:mm:ss") + "#");
                            ds[i] = oledbhelper.Query(sql);
                            //ds[i] = oledbhelper.Query(sqlstrings[i], olepara[i * 2], olepara[i * 2 + 1]);
                            ExtractionConfigDic.GetExtractionConfigDic()
                           .UpdateExtractionConfig(
                               new ExtractionConfig
                               {
                                   DataBaseId = (int)database.ID,
                                   TableName = tables[i].TableName,
                                   Acqtime = endtime3[i].ToString("yyyy-MM-dd HH:mm:ss")
                               });
                        }
                        DataSet retSet = new DataSet();
                        for (int i = 0; i < tables.Count; i++)
                        {
                            retSet.Merge(ds[i]);
                        }
                        return retSet;
                        //return oledbhelper.Query(sqlstr.ToString(), olepara);
                    }
                    break;
                default:
                    return new DataSet();
            }//switch
            return new DataSet();
        }

        /// <summary>
        /// 获取上次提取的时间(如果不存在则用目标数据库中数据最小时间)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        private string GetLastTimeOrDefaultMin(List<ExtractionConfig> list,TableFieldInfo tableinfo,DataBaseName database)
        {
            foreach (ExtractionConfig extractionConfig in list)
            {
                if (string.Equals(extractionConfig.TableName, tableinfo.TableName, StringComparison.CurrentCultureIgnoreCase))
                {
                    string lastExtractTime = extractionConfig.Acqtime;
                    return lastExtractTime;
                }
            }

            return GetDefaultMinTime(database, tableinfo);
        }

        /// <summary>
        /// 获取数据表中数据最早时间
        /// </summary>
        /// <param name="database">数据库信息</param>
        /// <param name="tableinfo">表信息</param>
        /// <returns></returns>
        private string GetDefaultMinTime(DataBaseName database, TableFieldInfo tableinfo)
        {
            string retTime = "2013-01-01 00:00:00";
            var connstr = Connectionstring.GetConnectionString(database);
            switch (database.DataBaseType)
            {
                case (int)DataBaseType.SQLite:
                    var db1 = new DbHelperSqLiteP(connstr);
                    var tb1 = db1.Query(string.Format("select min({0}) as mintime from {1}", tableinfo.AcqTime, tableinfo.TableName)).Tables[0];
                    if (tb1 != null && tb1.Rows.Count > 0)
                    {
                        retTime = tb1.Rows[0]["mintime"].ToString();
                    }
                    break;
                case (int)DataBaseType.SQLServer:
                    var db2 = new DbHelperSQLP(connstr);
                    var tb2 = db2.Query(string.Format("select min({0}) as mintime from {1}", tableinfo.AcqTime, tableinfo.TableName)).Tables[0];
                    if (tb2 != null && tb2.Rows.Count > 0)
                    {
                        retTime = tb2.Rows[0]["mintime"].ToString();
                    }
                    break;
                case (int)DataBaseType.ACCESSOld:
                case (int)DataBaseType.ACCESSNew:
                    var db3 = new DbHelperOleDbP(connstr);
                    var tb3 = db3.Query(string.Format("select min({0}) as mintime from {1}", tableinfo.AcqTime, tableinfo.TableName)).Tables[0];
                    if (tb3 != null && tb3.Rows.Count > 0)
                    {
                        retTime = tb3.Rows[0]["mintime"].ToString();
                    }
                    break;
                default:
                    break;
            }
            return retTime;
        }

        private DateTime GetDayStepEndTime(DateTime time)
        {
            if (DateTime.Now.Subtract(time).Days > 1)   // 与当前时间相差超过一天
            {
                return time.AddDays(1);
            }
            else
            {
                return DateTime.Now.AddMinutes(-delaytime);
            }
        }
        #endregion
    }
}