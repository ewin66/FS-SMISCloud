// // --------------------------------------------------------------------------------------------
// // <copyright file="TableFieldInfoBll.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140605
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

namespace FSDE.BLL
{
    public class TableFieldInfoBll
    {
        private readonly ITableFieldInfo Dal = DataAccess.CreateTableFieldInfoDal();
        public int AddTableFieldInfo(TableFieldInfo tableFieldInfo)
        {
            return Dal.AddTableFieldInfo(tableFieldInfo);
        }

        public bool UpdateTableFieldInfo(TableFieldInfo tableFieldInfo)
        {
            return Dal.UpdateTableFieldInfo(tableFieldInfo);
        }

        public bool Delete(int id)
        {
            return Dal.Delete(id);
        }

        public IList<TableFieldInfo> SelectList()
        {
            return Dal.SelectList();
        }
    }
}