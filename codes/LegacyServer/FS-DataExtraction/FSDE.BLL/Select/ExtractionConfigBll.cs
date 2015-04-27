#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ExtractionConfigBll.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace FSDE.BLL.Select
{
    using System.Collections.Generic;

    using FSDE.DALFactory;
    using FSDE.IDAL;
    using FSDE.Model.Config;

    public class ExtractionConfigBll
    {
        private static readonly IExtractionConfigDal Dal = DataAccess.CreatExtractionConfigDal();

        public IList<ExtractionConfig> SelectConfigs()
        {
           return Dal.SelectConfigs();
        }

        public int AddNewConfig(ExtractionConfig config)
        {
            return Dal.AddNewConfig(config);
        }

        public bool UpdateConfig(ExtractionConfig config)
        {
            return Dal.UpdateConfig(config);
        }
    }
}