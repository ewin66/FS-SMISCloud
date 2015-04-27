// // --------------------------------------------------------------------------------------------
// // <copyright file="GroupSensorsBll.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140530
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
    public class GroupSensorsBll
    {
        private readonly IGroupSensors Dal = DataAccess.CreateGroupSensorsDal();
        public int AddGroupSensorInfo(GroupSensors groupSensorInfo)
        {
            return Dal.AddGroupSensorInfo(groupSensorInfo);
        }

        public bool Delete(int id)
        {
            return Dal.Delete(id);
        }

        public IList<GroupSensors> SelectList()
        {
            return Dal.SelectList();
        }

    }
}