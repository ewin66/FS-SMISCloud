// // --------------------------------------------------------------------------------------------
// // <copyright file="GroupSensorDal.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Collections.Generic;
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class GroupSensorsDal:IGroupSensors
    {
        public int AddGroupSensorInfo(Model.Config.GroupSensors groupSensors)
        {
            using (DbConnection conn = new DbConnection())
            {
                return groupSensors.Save();
            }
        }

        public bool UpdateGroupSensorInfo(Model.Config.GroupSensors groupSensors)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteGroupSensorInfos(int startId, int endId)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(int id)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    GroupSensors.Delete(Where.Equal("ID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public IList<GroupSensors> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<GroupSensors> adapter = TableAdapter<GroupSensors>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}