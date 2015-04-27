#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ExtractionConfigDal.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140624 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.DAL.Select
{
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.IDAL;
    using FSDE.Model.Config;

    using SqliteORM;

    public class ExtractionConfigDal:IExtractionConfigDal
    {
        public IList<ExtractionConfig> SelectConfigs()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<ExtractionConfig> adapter = TableAdapter<ExtractionConfig>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }

        public int AddNewConfig(ExtractionConfig config)
        {
            using (DbConnection conn = new DbConnection())
            {
                return config.Save();
            }
        }

        public bool UpdateConfig(ExtractionConfig config)
        {
            using (DbConnection conn = new DbConnection())
            {
                if (config.Save() > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}