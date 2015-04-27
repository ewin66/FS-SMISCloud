#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DistinguishCalcType.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140612 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Core
{
    using System.Collections.Generic;
    using FSDE.Dictionaries.config;
    using FSDE.Model;
    using FSDE.Model.Config;

    //public class DistinguishCalcType
    //{
    //    public Dictionary<int, IList<Data>> DistinguishCalc(IList<Data> list,int databaseId)
    //    {
    //        var rslt = new Dictionary<int, IList<Data>>
    //                       {
    //                           { 0, new List<Data>() },
    //                           { 1, new List<Data>() }
    //                       };

    //        // 这个数据库里所有分组
    //        List<GroupInfo> groupInfos = GroupInfosDic.GetGroupInfosDic().GetGroups((int)SafetyfactortypeEnum.DeepDisplacement, databaseId);

    //        // 所有分组中的传感器
    //        List<GroupSensors> sensorses = GroupSensorInfoDic.GetGroupInfosDic().GetGroupSensorses(groupInfos);

    //        // 所有需要均值的监测因素
    //        List<DataFilter> filters = DataFilterDic.GetDataFilterDic().GetyDataFilters(databaseId);

    //        foreach (Data data in list)
    //        {
    //            bool iscalc = false;

    //            foreach (DataFilter filter in filters)
    //            {
    //                if (filter.SafetyFactorType == data.SafeTypeId)
    //                {
    //                    rslt[1].Add(data);
    //                    iscalc = true;
    //                    break;
    //                }
    //                if (!iscalc)
    //                {
    //                    foreach (GroupSensors sensor in sensorses)
    //                    {
    //                        if (sensor.SensorID == data.SensorId)
    //                        {
    //                            rslt[1].Add(data);
    //                            iscalc = true;
    //                            break;
    //                        }
    //                    }
    //                }
    //            }

    //            if (!iscalc)
    //            {
    //                rslt[0].Add(data);
    //            }
    //        }
    //        return rslt;
    //    }
        
    //    public Dictionary<int, IList<Data>> DistinguishCalc(IList<Data> list, int databaseId, CalcType type)
    //    {
    //        var rslt = new Dictionary<int, IList<Data>>() { { 0, new List<Data>() }, { 1, new List<Data>() } };

    //        // 这个数据库里所有分组
    //        List<GroupInfo> groupInfos = GroupInfosDic.GetGroupInfosDic().GetGroups((int)SafetyfactortypeEnum.DeepDisplacement, databaseId);

    //        // 所有分组中的传感器
    //        List<GroupSensors> sensorses = GroupSensorInfoDic.GetGroupInfosDic().GetGroupSensorses(groupInfos);

    //        // 所有需要均值的监测因素
    //        List<DataFilter> filters = DataFilterDic.GetDataFilterDic().GetyDataFilters(databaseId);
            
            
    //            switch (type)
    //            {
    //                case CalcType.Average:
    //                    foreach (Data data in list)
    //                    {
    //                        bool iscalc = false;
    //                        foreach (DataFilter filter in filters)
    //                        {
    //                            if (filter.SafetyFactorType == data.SafeTypeId)
    //                            {
    //                                rslt[1].Add(data);
    //                                iscalc = true;
    //                                break;
    //                            }
    //                        }
    //                        if (!iscalc)
    //                        {
    //                            rslt[0].Add(data);
    //                        }
    //                    }
    //                    return rslt;
    //                case CalcType.GroupCx:
    //                    foreach (Data data in list)
    //                    {
    //                        bool iscalc = false;
    //                        foreach (GroupSensors sensor in sensorses)
    //                        {
    //                            if (sensor.SensorID == data.SensorId)
    //                            {
    //                                rslt[1].Add(data);
    //                                iscalc = true;
    //                                break;
    //                            }
    //                        }
    //                        if (!iscalc)
    //                        {
    //                            rslt[0].Add(data);
    //                        }
    //                    }
    //                    return rslt;
    //            }
    //        return rslt;
    //    }
    //}

    public enum CalcType
    {
        GroupCx,
        Average,
        None
    }
}