// // --------------------------------------------------------------------------------------------
// // <copyright file="SensorGroupInfo.cs" company="江苏飞尚安全监测咨询有限公司">
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
using System.Linq;
using FSDE.IDAL;
using FSDE.Model;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class GroupInfoDal : IGroupInfoDal
    {

        public int AddGroupInfo(GroupInfo sensorGroupInfo)
        {
            using (DbConnection conn = new DbConnection())
            {
                return sensorGroupInfo.Save();
            }
        }

        public bool UpdateGroupInfo(GroupInfo sensorGroupInfo)
        {
            using (DbConnection conn = new DbConnection())
            {
                if (sensorGroupInfo.Save() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool DeleteGroupInfos(int startId, int endId)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    GroupInfo.Delete(Where.And(Where.GreaterOrEqual("ID", startId), Where.LessOrEqual("ID", endId)));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool Delete(int id)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    GroupInfo.Delete(Where.Equal("GroupID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public System.Collections.Generic.IList<GroupInfo> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<GroupInfo> adapter = TableAdapter<GroupInfo>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}