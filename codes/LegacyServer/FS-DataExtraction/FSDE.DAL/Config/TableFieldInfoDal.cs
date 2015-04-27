// // --------------------------------------------------------------------------------------------
// // <copyright file="TableFieldInfoDal.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class TableFieldInfoDal : ITableFieldInfo
    {

        public int AddTableFieldInfo(TableFieldInfo tableFieldInfo)
        {
            using (DbConnection conn = new DbConnection())
            {
                return tableFieldInfo.Save();
            }
        }

        public bool UpdateTableFieldInfo(TableFieldInfo tableFieldInfo)
        {
            using (DbConnection conn = new DbConnection())
            {
                if (tableFieldInfo.Save() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool Delete(int id)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    TableFieldInfo.Delete(Where.Equal("ID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public System.Collections.Generic.IList<TableFieldInfo> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<TableFieldInfo> adapter = TableAdapter<TableFieldInfo>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}