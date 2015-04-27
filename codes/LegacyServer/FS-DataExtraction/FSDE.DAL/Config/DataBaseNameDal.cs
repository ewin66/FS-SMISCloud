// // --------------------------------------------------------------------------------------------
// // <copyright file="DataBaseNameDal.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140528
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class DataBaseNameDal:IDataBaseName
    {

        public int AddDataBaseName(DataBaseName dataBaseName)
        {
            using (DbConnection conn = new DbConnection())
            {
                return dataBaseName.Save();
            }
        }
    
        public IList<DataBaseName> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<DataBaseName> adapter = TableAdapter<DataBaseName>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }


        public bool Delete(int id)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    DataBaseName.Delete(Where.Equal("ID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}