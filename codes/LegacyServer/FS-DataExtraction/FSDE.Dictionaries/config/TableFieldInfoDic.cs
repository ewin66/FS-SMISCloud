#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="TableFieldInfoDic.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140604 by WIN .
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
using FSDE.BLL;
using FSDE.BLL.Config;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.Model.Config;

    public class TableFieldInfoDic
    {
        private static TableFieldInfoDic tableFieldInfoDic =new TableFieldInfoDic();

        public static TableFieldInfoDic GetTableFieldInfoDic()
        {
            return tableFieldInfoDic;
        }

        private Dictionary<int, TableFieldInfo> tableFieldInfos;

        private TableFieldInfoDic()
        {
            if (tableFieldInfos == null)
            {
                tableFieldInfos = new Dictionary<int, TableFieldInfo>();
                this.tableFieldInfos = new Dictionary<int, TableFieldInfo>();
                var bll = new TableFieldInfoBll();
                IList<TableFieldInfo> list = bll.SelectList();
                foreach (TableFieldInfo tableField in list)
                {
                    tableFieldInfos.Add(Convert.ToInt32(tableField.Id), tableField);
                }
            }
        }

        public bool Add(TableFieldInfo tableFieldInfo)
        {
            var bll = new TableFieldInfoBll();
            int id = bll.AddTableFieldInfo(tableFieldInfo);
            if (id > 0)
            {
                tableFieldInfo.Id = id;
                tableFieldInfoDic.tableFieldInfos.Add(id, tableFieldInfo);
                return true;
            }
            return false;
        }

        public int CheckAdd(TableFieldInfo tableFieldInfo)
        {
            bool flag = false;
            int Id = 0;
            List<TableFieldInfo> tableFieldInfoList = tableFieldInfos.Values.ToList();
            for (int i = 0; i < tableFieldInfoList.Count; i++)
            {
                if (tableFieldInfoList[i].TableName == tableFieldInfo.TableName)
                {
                    flag = true;
                    Id = Convert.ToInt32(tableFieldInfoList[i].Id);
                }
            }
            if (!flag)
            {
                var bll = new TableFieldInfoBll();
                int id = bll.AddTableFieldInfo(tableFieldInfo);
                if (id > 0)
                {
                    tableFieldInfo.Id = id;
                    tableFieldInfoDic.tableFieldInfos.Add(id, tableFieldInfo);
                    return id;
                }
            }
            return Id;

        }

        public bool UpdateTableFieldInfo(TableFieldInfo tableFieldInfo)
        {
            var bll = new TableFieldInfoBll();
            if (bll.UpdateTableFieldInfo(tableFieldInfo))
            {
                tableFieldInfos[(int) tableFieldInfo.Id] = tableFieldInfo;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            var bll = new TableFieldInfoBll();
            tableFieldInfos.Remove(id);
            return bll.Delete(id);
        }

        public int Count()
        {
            return tableFieldInfoDic.tableFieldInfos.Count;
        }

        public List<TableFieldInfo> GetAllTableFieldInfos()
        {
            return tableFieldInfoDic.tableFieldInfos.Values.ToList();
        }

        public List<TableFieldInfo> GetSameDataBaseTableFieldInfos(int databaseId)
        {
            return this.tableFieldInfos.Values.Where(fieldInfo => fieldInfo.DataBaseId == databaseId).ToList();
        }

        public TableFieldInfo GeTableFieldInfo(int databaseid, string tablename)
        {
            foreach (TableFieldInfo fieldInfo in tableFieldInfos.Values)
            {
                if (fieldInfo.DataBaseId == databaseid && fieldInfo.TableName == tablename)
                {
                    return fieldInfo;
                }
            }

            return new TableFieldInfo();
        }
    }
}