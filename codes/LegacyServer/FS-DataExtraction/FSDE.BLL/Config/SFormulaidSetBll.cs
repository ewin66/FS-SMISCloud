// // --------------------------------------------------------------------------------------------
// // <copyright file="SFormulaidSetBll.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using FSDE.DALFactory;
using FSDE.IDAL;
using FSDE.Model.Config;

namespace FSDE.BLL
{
    public class SFormulaidSetBll
    {
        private readonly ISFormulaidSet Dal = DataAccess.CreateSFormulaidSetDal();

        public int Add(SFormulaidSet sFormulaidSet)
        {
            return Dal.AddSFormulaidSet(sFormulaidSet);
        }

        public bool Update(SFormulaidSet sFormulaidSet)
        {
            return Dal.UpdateSFormulaidSet(sFormulaidSet);
        }

        public bool Delete(int id)
        {
            return Dal.Delete(id);
        }

        public IList<SFormulaidSet> SelectList()
        {
            return Dal.SelectList();
        }

    }
}