// // --------------------------------------------------------------------------------------------
// // <copyright file="SensorGroupInfoBll.cs" company="江苏飞尚安全监测咨询有限公司">
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

using FSDE.Model;

namespace FSDE.BLL.Config
{
    using System;
    using System.Collections.Generic;

    using FSDE.DALFactory;
    using FSDE.IDAL;
    using FSDE.Model.Config;

    public class GroupInfoBll
    {
        private readonly IGroupInfoDal Dal = DataAccess.CreateGroupInfoDal();

        public int AddGrouopInfo(GroupInfo groupInfo)
        {
            return Dal.AddGroupInfo(groupInfo);
        }

        public bool UpdateGroupInfo(GroupInfo groupInfo)
        {
            return Dal.UpdateGroupInfo(groupInfo);
        }

        public bool Delete(int id)
        {
            return Dal.Delete(id);
        }

        public bool DeleteGroupInfos(int startId, int endId)
        {
            return Dal.DeleteGroupInfos(startId, endId);
        }

        public IList<GroupInfo> SelectList()
        {
            return Dal.SelectList();
        }
    }
}