#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataFilterDic.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140529 by WIN .
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
using FSDE.BLL.Config;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.Model.Config;

    public class DataFilterDic
    {
        private Dictionary<int, DataFilter> dataFilters;

        private static DataFilterDic dataFilterDic = new DataFilterDic();


        private DataFilterDic()
        {
            if (null == this.dataFilters)
            {
                this.dataFilters = new Dictionary<int, DataFilter>();
                var bll = new DataFilterBll();
                IList<DataFilter> list = bll.SelectList();
                foreach (DataFilter filter in list)
                {
                    dataFilters.Add(Convert.ToInt32(filter.Id),filter);
                }
            }
        }
        public bool Add(DataFilter dataFilter)
        {
            bool flag = false;
            List<DataFilter> dataFiltersList = dataFilters.Values.ToList();

            for (int i = 0; i < dataFiltersList.Count; i++)
            {
                if (dataFiltersList[i].FilterType == dataFilter.FilterType
                    && dataFiltersList[i].SafetyFactorType == dataFilter.SafetyFactorType)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                var bll = new DataFilterBll();
                int id = bll.AddDataFilter(dataFilter);
                if (id > 0)
                {
                    dataFilter.Id = id;
                    this.dataFilters.Add(id, dataFilter);
                    return true;
                }
            }
            
            return false;
        }

        public static DataFilterDic GetDataFilterDic()
        {
            return dataFilterDic;
        }

        /// <summary>
        /// 返回所有需要进行去除数据跳变处理的安全监测因素类型
        /// </summary>
        /// <returns></returns>
        public int[] GetSafetyfactortypes()
        {
            return this.dataFilters.Values.Select(dataFilter => dataFilter.SafetyFactorType).ToArray();
        }

        /// <summary>
        /// 根据安全监测因素选择数据过滤算法
        /// </summary>
        /// <param name="safetyFactorTypeId"></param>
        /// <returns></returns>
        public DataFilter GetFilterType(int safetyFactorTypeId)
        {
            foreach (DataFilter dataFilter in dataFilters.Values)
            {
                if (dataFilter.SafetyFactorType == safetyFactorTypeId)
                {
                    return dataFilter;
                }
            }

            return new DataFilter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        public List<DataFilter> GetyDataFilters(int databaseId)
        {
            List<DataFilter> list =new List<DataFilter>();
            foreach (DataFilter dataFilter in dataFilters.Values)
            {
                if (dataFilter.DataBaseId == databaseId)
                {
                    list.Add(dataFilter);
                }
            }
            return list;
        }

    }
}