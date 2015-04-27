#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataBaseNameDic.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140527 by WIN .
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
using System.Security.Permissions;
using FSDE.BLL;
using FSDE.BLL.Config;
using FSDE.Model;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.Model.Config;

    public class DataBaseNameDic
    {
        public ConcurrentDictionary<int, DataBaseName> dataBaseNames;

        private static DataBaseNameDic dataBaseNameDic = new DataBaseNameDic();

        public static DataBaseNameDic GetDataBaseNameDic()
        {
            return dataBaseNameDic;
        }

        private DataBaseNameDic()
        {
            if (null == dataBaseNames)
            {
                this.dataBaseNames = new ConcurrentDictionary<int, DataBaseName>();
                var bll = new DataBaseNameBll();
                List<DataBaseName> list = bll.SelectList().ToList();
                foreach (DataBaseName dataBaseName in list)
                {
                    dataBaseNames.TryAdd(Convert.ToInt32(dataBaseName.ID),dataBaseName);
                }
            }  
        }

        public bool Delete(int id)
        {
            var bll = new DataBaseNameBll();
            var db = new DataBaseName();
            dataBaseNames.TryRemove(id,out db);
            return bll.Delete(id);
        }

        public bool Add(DataBaseName dataBaseName)
        {
            var bll = new DataBaseNameBll();
            int id = bll.AddDataBaseName(dataBaseName);
            if (id > 0)
            {
                dataBaseName.ID = id;
                this.dataBaseNames.TryAdd(id, dataBaseName);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 返回一个数据库对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataBaseName SelectBaseName(int id)
        {
            if (dataBaseNames.ContainsKey(id))
            {
                return dataBaseNames[id];
            }

            return new DataBaseName(); 
        }


        public List<DataBaseName> GetAllBaseNames()
        {
            return dataBaseNames.Values.ToList();
        }

        /// <summary>
        /// 返回统一采集软件的数据数据库
        /// </summary>
        /// <returns></returns>
        public DataBaseName GetFSUSBaseName()
        {
            foreach (DataBaseName dataBaseName in dataBaseNames.Values)
            {
                if (dataBaseName.DataBaseCode.Contains("FSUSDataValueDB"))
                {
                    return dataBaseName;
                }
            }

            return new DataBaseName();
        }

        public List<DataBaseName> GetOtherDataBaseName()
        {
            List<DataBaseName> list = new List<DataBaseName>();
            foreach (DataBaseName dataBaseName in dataBaseNames.Values)
            {
                if (!dataBaseName.DataBaseCode.Contains("FSUSDataValueDB"))
                {
                    if (dataBaseName.DataBaseType == (int)DataBaseType.Shake
                        || dataBaseName.DataBaseType == (int)DataBaseType.Fiber || dataBaseName.DataBaseType == (int)DataBaseType.Vibration)
                    {
                        continue;
                    }
                    list.Add(dataBaseName);
                }
            }
            return list;
        }

        public List<DataBaseName> GetTextBaseNames()
        {
            List<DataBaseName> list = new List<DataBaseName>();
            foreach (DataBaseName dataBaseName in dataBaseNames.Values)
            {
                if (dataBaseName.DataBaseType == (int)DataBaseType.Shake
                        || dataBaseName.DataBaseType == (int)DataBaseType.Fiber || dataBaseName.DataBaseType == (int)DataBaseType.Vibration)
                {
                    list.Add(dataBaseName);
                }
            }
            return list;
        }


    }
}