// // --------------------------------------------------------------------------------------------
// // <copyright file="ConfigTableDic.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140619
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
using System.Linq;
using FSDE.BLL.Config;
using FSDE.Model.Config;

namespace FSDE.Dictionaries
{
    using FSDE.Dictionaries.config;

    public class ConfigTableDic
    {
        private Dictionary<int, ConfigTable> configTables;

        private static ConfigTableDic configTableDic = new ConfigTableDic();

        public static ConfigTableDic GetConfigTableDic()
        {
            return configTableDic;
        }

        private ConfigTableDic()
        {
            if (null == configTables)
            {
                configTables = new Dictionary<int, ConfigTable>();
                var bll = new ConfigTableBll();
                IList<ConfigTable> list = bll.SelectList();
                foreach (var table in list)
                {
                    configTables.Add(Convert.ToInt32(table.ID),table);
                }
            }
        }

        public bool Add(ConfigTable configTable)
        {
            var bll = new ConfigTableBll();
            int id = bll.Add(configTable);
            if (id > 0)
            {
                configTable.ID = id;
                configTableDic.configTables.Add(id,configTable);
                return true;
            }
            return false;
        }

        public int CheckAdd(ConfigTable configTable)
        {
            bool flag = false;
            List<ConfigTable> list = configTables.Values.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (configTable.ChannelId == list[i].ChannelId
                    && configTable.TableName == list[i].TableName
                    && configTable.ModuleNo == list[i].ModuleNo)
                {
                    flag = true;
                    return Convert.ToInt32(list[i].ID);
                }
            }

            if (!flag)
            {
                var bll = new ConfigTableBll();
                int id = bll.Add(configTable);
                if (id > 0)
                {
                    configTable.ID = id;
                    configTableDic.configTables.Add(id, configTable);
                    return id;
                }
            }
            return -1;
        }

        public bool Delete(int id)
        {
            var bll = new ConfigTableBll();
            configTables.Remove(id);
            return bll.Delete(id);
        }

        public List<ConfigTable> SelectList()
        {
            return configTables.Values.ToList();
        }
        
        /// <summary>
        /// 查找同一数据库的所有配置表
        /// </summary>
        /// <param name="dataBaseId"></param>
        /// <returns></returns>
        public List<ConfigTable> SelecConfigTables(int dataBaseId)
        {
            List<ConfigTable> list=new List<ConfigTable>();
            foreach (ConfigTable config in configTables.Values)
            {
                if (config.DataBaseId == dataBaseId)
                {
                    list.Add(config);
                }
            }
            return list;
        }
        
        /// <summary>
        /// 查找所有有配置表的数据库
        /// </summary>
        /// <returns></returns>
        public List<DataBaseName> SeleDataBaseNames()
        {
            List<DataBaseName> list =new List<DataBaseName>();
            foreach (ConfigTable config in configTables.Values)
            {
               // config.DataBaseId
                bool ishas = false;
                foreach (DataBaseName dataBase in list)
                {
                    if ((int)dataBase.ID == config.DataBaseId)
                    {
                        ishas = true;
                        break;
                    }
                }
                if (!ishas)
                {
                    list.Add(DataBaseNameDic.GetDataBaseNameDic().SelectBaseName(config.DataBaseId));
                }
            }

            return list;
        }

        public ConfigTable SelecConfigTable(int databaseId, string tableName)
        {
            foreach (ConfigTable table in configTables.Values)
            {
                if (table.DataBaseId == databaseId && table.TableName == tableName)
                {
                    return table;
                }
            }
            return null;
        }

        public ConfigTable SelecConfigTable(int databaseId)
        {
            return configTables.Values.FirstOrDefault(table => table.DataBaseId == databaseId);
        }
    }
}