// // --------------------------------------------------------------------------------------------
// // <copyright file="ConnectTestBll.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140527
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using FSDE.DALFactory;
using FSDE.IDAL;

namespace FSDE.BLL
{
    public class ConnectTestBll : IConnectTest
    {
        private static readonly IConnectTest Dal = DataAccess.CreateConnectTestDal();

        public bool IsConnect(string strsql)
        {
            return Dal.IsConnect(strsql);
        }
    }
}