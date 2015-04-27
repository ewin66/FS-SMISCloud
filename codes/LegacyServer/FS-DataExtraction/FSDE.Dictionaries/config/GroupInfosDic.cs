#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="GroupInfosDic.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140527 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Text.RegularExpressions;
using FSDE.BLL.Config;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.Model.Config;

    public class GroupInfosDic
    {
        private Dictionary<int, GroupInfo> groupInfos;

        private static GroupInfosDic groupInfoDic =new GroupInfosDic();


        public static GroupInfosDic GetGroupInfoDic()
        {
            return groupInfoDic;
        }

        private GroupInfosDic()
        {
            if (null == this.groupInfos)
            {
                this.groupInfos = new Dictionary<int, GroupInfo>();
                var bll = new GroupInfoBll();
                IList<GroupInfo> list = bll.SelectList();
                foreach (GroupInfo groupInfo in list)
                {
                    groupInfos.Add(Convert.ToInt32(groupInfo.GroupID), groupInfo);
                }
            }
        }

        public bool Add(GroupInfo groupInfo)
        {
            var bll = new GroupInfoBll();
            int id = bll.AddGrouopInfo(groupInfo);
            if (id > 0)
            {
                groupInfo.GroupID= id;
                groupInfoDic.groupInfos.Add(id, groupInfo);
                return true;
            }
            return false;
        }

        public int CheckAdd(GroupInfo groupInfo)
        {
            bool flag = true;
            int groupId = 0;
            List<GroupInfo> groupInfosList = groupInfos.Values.ToList();
            for (int i = 0; i < groupInfosList.Count; i++)
            {
                if (groupInfosList[i].GroupName == groupInfo.GroupName)
                {
                    flag = false;
                    groupId = Convert.ToInt32(groupInfosList[i].GroupID);
                    break;
                }
            }

            if (flag)
            {
                var bll = new GroupInfoBll();
                int id = bll.AddGrouopInfo(groupInfo);
                if (id > 0)
                {
                    groupInfo.GroupID = id;
                    groupInfoDic.groupInfos.Add(id, groupInfo);
                    groupId = id;
                }
            }
            return groupId;
        }

        public bool UpdateSensorGroupInfo(GroupInfo groupInfo)
        {
            var bll = new GroupInfoBll();
            if (bll.UpdateGroupInfo(groupInfo))
            {
                groupInfoDic.groupInfos[(int)groupInfo.GroupID] = groupInfo;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            var bll = new GroupInfoBll();
            groupInfos.Remove(id);
            return bll.Delete(id);
        }

        public bool DeleteSensorGroupInfos(int startId, int endId)
        {
            var bll = new GroupInfoBll();
            return bll.DeleteGroupInfos(startId, endId);
        }

        public static GroupInfosDic GetGroupInfosDic()
        {
            return groupInfoDic;
        }

        public Dictionary<int,GroupInfo> GetDic()
        {
            return groupInfos;
        }

        public List<GroupInfo> GetAllGroups()
        {
            return groupInfos.Values.ToList();
        }

        public List<GroupInfo> GetAllGroups(int factortypeid)
        {
            return this.groupInfos.Values.Where(groupInfo => groupInfo.Safetyfactortypeid == factortypeid).ToList();
        }

        public List<GroupInfo> GetFSUSGroups(int factortypeid)
        {
            return
                this.groupInfos.Values.Where(groupInfo => groupInfo.Safetyfactortypeid == factortypeid)
                    .Where(
                        groupInfo =>
                        DataBaseNameDic.GetDataBaseNameDic()
                            .SelectBaseName(groupInfo.DataBaseID)
                            .DataBaseCode.Contains("FSUSDataValueDB"))
                    .ToList();
        }

        public List<GroupInfo> GetOtherGroups(int factortypeid)
        {
            return
                this.groupInfos.Values.Where(groupInfo => groupInfo.Safetyfactortypeid == factortypeid)
                    .Where(
                        groupInfo =>
                        !DataBaseNameDic.GetDataBaseNameDic()
                             .SelectBaseName(groupInfo.DataBaseID)
                             .DataBaseCode.Contains("FSUSDataValueDB"))
                    .ToList();
        }

        public List<GroupInfo> GetGroups(int factortypeid, int databaseId)
        {
            return this.groupInfos.Values.Where(groupInfo => groupInfo.Safetyfactortypeid == factortypeid && groupInfo.DataBaseID == databaseId).ToList();
        }

        public bool IsBelongFSDB(int groupid)
        {
            int dbid = this.groupInfos[groupid].DataBaseID;
            return (int)DataBaseNameDic.GetDataBaseNameDic().GetFSUSBaseName().ID == dbid;
        }

    }
}