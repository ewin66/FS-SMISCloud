﻿// // --------------------------------------------------------------------------------------------
// // <copyright file="DataFilterTypeBll.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140603
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using FSDE.DALFactory;
using FSDE.IDAL;
using FSDE.Model.Config;

namespace FSDE.BLL.Config
{
    public class DataFilterTypeBll
    {
        private readonly IDataFilterType Dal = DataAccess.CreateDataFilterTypeDal();

        public IList<DataFilterType> SelectList()
        {
            return Dal.SelectList();
        }
    }
    
}