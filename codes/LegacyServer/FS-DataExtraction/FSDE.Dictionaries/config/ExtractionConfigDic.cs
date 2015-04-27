#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ExtractionConfigDic.cs" company="江苏飞尚安全监测咨询有限公司">
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

namespace FSDE.Dictionaries.config
{
    using System;
    using System.Collections.Generic;
    using FSDE.BLL.Select;
    using FSDE.Model.Config;

    public class ExtractionConfigDic
    {
        private static ExtractionConfigDic extractionConfigdic = new ExtractionConfigDic();

        public static ExtractionConfigDic GetExtractionConfigDic()
        {
            return extractionConfigdic;
        }

        private Dictionary<int, ExtractionConfig> extractionConfigs; 

        private ExtractionConfigDic()
        {
            if (null == extractionConfigs)
            {
                extractionConfigs = new Dictionary<int, ExtractionConfig>();
                var bll = new ExtractionConfigBll();
                IList<ExtractionConfig> list = bll.SelectConfigs();
                foreach (ExtractionConfig config in list)
                {
                    this.extractionConfigs.Add(Convert.ToInt32(config.Id),config);
                }
            }
        }

        /// <summary>
        /// 根据数据库id和表名获取一个提取时间配置
        /// </summary>
        /// <param name="databaseId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ExtractionConfig GetExtractionConfig(int databaseId, string tableName)
        {
            foreach (ExtractionConfig ec in this.extractionConfigs.Values)
            {
                if (ec.DataBaseId == databaseId && string.Equals(ec.TableName, tableName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return ec;
                }
            }

            return new ExtractionConfig { Acqtime = "00010101000000.000" };
        }

        public ExtractionConfig GetExtractionTxtTime(int databaseId)
        {
            foreach (ExtractionConfig ec in this.extractionConfigs.Values)
            {
                if (ec.DataBaseId == databaseId)
                {
                    return ec;
                }
            }

            return new ExtractionConfig { Acqtime = "00010101000000.000" };
        }

        /// <summary>
        /// 获取同一个数据库的所有提取时间配置
        /// </summary>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        public List<ExtractionConfig> GetExtractionConfig(int databaseId)
        {
            var list = new List<ExtractionConfig>();
            foreach (ExtractionConfig ec in this.extractionConfigs.Values)
            {
                if (ec.DataBaseId == databaseId)
                {
                    list.Add(ec);
                }
            }

            return list;
        }

        /// <summary>
        /// 更新一个提取时间配置
        /// </summary>
        /// <param name="config"></param>
        public void UpdateExtractionConfig(ExtractionConfig config)
        {
            bool ishas = false;
            var bll = new ExtractionConfigBll();
            foreach (ExtractionConfig ec in this.extractionConfigs.Values)
            {
                if (ec.DataBaseId == config.DataBaseId && string.Equals(ec.TableName, config.TableName, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.extractionConfigs[(int)ec.Id].Acqtime = config.Acqtime;
                    config.Id = ec.Id;
                    bll.UpdateConfig(config);
                    ishas = true;
                }
            }
            if (!ishas)
            {
                int id = bll.AddNewConfig(config);
                if (id > 0)
                {
                    config.Id = id;
                    this.extractionConfigs.Add(id,config);
                }
            }
        }
    }
}