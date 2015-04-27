// // --------------------------------------------------------------------------------------------
// // <copyright file="ConfigTableDal.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140619
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
using System.Security.Cryptography.X509Certificates;
using FSDE.IDAL;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class ConfigTableDal:IConfigTable
    {

        public int Add(ConfigTable configTable)
        {
            using (DbConnection conn = new DbConnection() )
            {
                return configTable.Save();
            }
        }

        public bool Update(ConfigTable configTable)
        {
            using (DbConnection conn = new DbConnection())
            {
                if (configTable.Save()>0)
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
                    ConfigTable.Delete(Where.Equal("ID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public System.Collections.Generic.IList<ConfigTable> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<ConfigTable> adapter = TableAdapter<ConfigTable>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}