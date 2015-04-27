// // --------------------------------------------------------------------------------------------
// // <copyright file="DataFilterTypeDal.cs" company="江苏飞尚安全监测咨询有限公司">
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
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class DataFilterTypeDal:IDataFilterType
    {

        public IList<DataFilterType> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<DataFilterType> adapter = TableAdapter<DataFilterType>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}