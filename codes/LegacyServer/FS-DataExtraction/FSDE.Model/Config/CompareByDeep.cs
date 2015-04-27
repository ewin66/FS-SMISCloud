#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="CompareByDeep.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述： 用于杆式测斜分组计算时根据安装深度排序时使用
// 
//  创建标识：Created in 20140528 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model.Config
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 比较器
    /// </summary>
    public class CompareByDeep : IComparer<GroupSensors>
    {
        public int Compare(GroupSensors x, GroupSensors y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            int xdeep = Convert.ToInt32(x.SensorFlag);
            int ydeep = Convert.ToInt32(y.SensorFlag);
            int result = ydeep.CompareTo(xdeep);
            return result;
        }
    }
}