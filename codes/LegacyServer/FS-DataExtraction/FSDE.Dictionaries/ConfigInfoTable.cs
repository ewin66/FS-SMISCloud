#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ConfigInfoTable.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace FSDE.Dictionaries
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;

    using FSDE.BLL.Select;
    using FSDE.Model.Config;
    using FSDE.Model.Fixed;

    public class ConfigInfoTable
    {
                                                 //数据库ID                 sensorId  
        public static readonly ConcurrentDictionary<int, ConcurrentDictionary<int, ConfigTableInfo>> ConfigtableInfoDictionary = new ConcurrentDictionary<int, ConcurrentDictionary<int, ConfigTableInfo>>();

        public static void InitializationConfigtableInfo()
        {
            List<DataBaseName> list = ConfigTableDic.GetConfigTableDic().SeleDataBaseNames();
            if (list != null && list.Count > 0)
            {
                SelectConfigTableInfoBll bll = new SelectConfigTableInfoBll();
                foreach (DataBaseName dataBase in list)
                {
                    DataSet ds = bll.Select(dataBase);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataTable table in ds.Tables)
                        {
                            if (table != null && table.Rows.Count > 0)
                            {
                                foreach (DataRow row in table.Rows)
                                {
                                    ConfigTableInfo configInfo =new ConfigTableInfo();
                                    configInfo.DataBaseId = int.Parse(row[0].ToString().Trim());
                                    configInfo.TableName = row[1].ToString().Trim();
                                    configInfo.SensorId = int.Parse(row[2].ToString().Trim());
                                    configInfo.MoudleNo = row[3].ToString().Trim();
                                    configInfo.ChannelId = 1;
                                    try
                                    {
                                        configInfo.ChannelId = int.Parse(row[4].ToString().Trim());
                                        configInfo.Otherflag = row[5].ToString().Trim();
                                    }
                                    catch
                                    {
                                    }
                                    if (!ConfigtableInfoDictionary.ContainsKey((int)dataBase.ID))
                                    {
                                        ConfigtableInfoDictionary.TryAdd(
                                            (int)dataBase.ID,
                                            new ConcurrentDictionary<int, ConfigTableInfo>());
                                    }
                                    if (!ConfigtableInfoDictionary[(int)dataBase.ID].ContainsKey(configInfo.SensorId))
                                    {
                                        ConfigtableInfoDictionary[(int)dataBase.ID].TryAdd(
                                            configInfo.SensorId,
                                            configInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}