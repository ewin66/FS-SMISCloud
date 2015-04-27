// // --------------------------------------------------------------------------------------------
// // <copyright file="ISensorGroupInfoDal.cs" company="江苏飞尚安全监测咨询有限公司">
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


using FSDE.Model.Config;

namespace FSDE.IDAL
{
    using System.Collections.Generic;
    using FSDE.Model;

    public interface IGroupInfoDal
    {
        int AddGroupInfo(GroupInfo sensorGroupInfo);

        bool UpdateGroupInfo(GroupInfo sensorGroupInfo);

        bool DeleteGroupInfos(int startId, int endId);

        bool Delete(int id);

        IList<GroupInfo> SelectList();
         
    }
}