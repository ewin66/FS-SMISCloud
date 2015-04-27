#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="GroupSensorInfoDic.cs" company="江苏飞尚安全监测咨询有限公司">
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using FSDE.BLL;
using FSDE.BLL.Config;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;
    using System.Security;

    using FSDE.Model.Config;

    public class GroupSensorInfoDic
    {
        private Dictionary<int, GroupSensors> groupSensorses;

        private static GroupSensorInfoDic groupInfosDic = new GroupSensorInfoDic();

        public static GroupSensorInfoDic GetGroupInfosDic()
        {
            return groupInfosDic;
        }

        private GroupSensorInfoDic()
        {
            if (null == groupSensorses)
            {
                groupSensorses = new Dictionary<int, GroupSensors>();
                var bll = new GroupSensorsBll();
                IList<GroupSensors> list = bll.SelectList();
                foreach (GroupSensors groupSensorse in list)
                {
                    groupSensorses.Add(Convert.ToInt32(groupSensorse.ID), groupSensorse);
                }
            }
        }

        public Dictionary<int,GroupSensors> GetDic()
        {
            return groupSensorses;
        }

        public bool Add(GroupSensors groupSensors)
        {
            bool flag = false;
            var b = new GroupSensorsBll();
            List<GroupSensors> groupSensorsesList = b.SelectList().ToList();
            for (int i = 0; i < groupSensorsesList.Count; i++)
            {
                if (groupSensorsesList[i].SensorID == groupSensors.SensorID)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                var bll = new GroupSensorsBll();
                int id = bll.AddGroupSensorInfo(groupSensors);
                groupSensors.ID = id;
                groupInfosDic.groupSensorses.Add(id, groupSensors);
                return true;
            }
            return false;
        }

        public List<GroupSensors> GetGroupSensorses(int groupId)
        {
            List<GroupSensors> list = new List<GroupSensors>();
            foreach (GroupSensors groupSensorse in groupSensorses.Values)
            {
                if (groupSensorse.GroupID == groupId)
                {
                    list.Add(groupSensorse);
                }
            }

            return list;
        }

        public bool Delete(int id)
        {
            var bll = new GroupSensorsBll();
            groupInfosDic.groupSensorses.Remove(id);
            return bll.Delete(id);
        }

        public List<GroupSensors> GetGroupSensorses(List<GroupInfo> groups)
        {
            List<GroupSensors> list = new List<GroupSensors>();
            foreach (GroupInfo group in groups)
            {
                foreach (GroupSensors sensor in groupSensorses.Values)
                {
                    if (sensor.GroupID == group.GroupID)
                    {
                        list.Add(sensor);
                    }
                }
            }

            return list;
        }
    }
}