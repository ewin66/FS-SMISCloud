// // --------------------------------------------------------------------------------------------
// // <copyright file="SFormulaidSetDal.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140609
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class SFormulaidSetDal : ISFormulaidSet
    {
        public int AddSFormulaidSet(SFormulaidSet sformulaidSet)
        {
            using (DbConnection conn = new DbConnection())
            {
                return sformulaidSet.Save();
            }
        }

        public bool UpdateSFormulaidSet(SFormulaidSet sformulaidSet)
        {
            using (DbConnection conn = new DbConnection())
            {
                if (sformulaidSet.Save() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool DeleteSFormulaidSet(int startId, int endId)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(int id)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    SFormulaidSet.Delete(Where.Equal("FORMULAID_SET_ID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public IList<SFormulaidSet> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<SFormulaidSet> adapter = TableAdapter<SFormulaidSet>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}