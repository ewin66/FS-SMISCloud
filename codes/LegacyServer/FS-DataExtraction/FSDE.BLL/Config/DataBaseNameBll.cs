// // --------------------------------------------------------------------------------------------
// // <copyright file="DataBaseNameBll.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System.Collections;
using System.Collections.Generic;
using FSDE.DALFactory;
using FSDE.IDAL;
using FSDE.Model.Config;

namespace FSDE.BLL.Config
{
    public class DataBaseNameBll
    {
        private readonly IDataBaseName Dal = DataAccess.CreateDataBaseNameDal();

        public int AddDataBaseName(DataBaseName dataBaseName)
        {
            return Dal.AddDataBaseName(dataBaseName);
        }

        public IList<DataBaseName> SelectList()
        {
            return Dal.SelectList();
        }

        public bool Delete(int id)
        {
            return Dal.Delete(id);
        }

    }
}