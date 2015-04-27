#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="GroupParam.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141124 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGET.DataCalc.Model
{
    using System.Collections.Generic;

    public class GroupItem
    {
        /// <summary>
        /// 传感器ID
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// 所属DTUID
        /// </summary>
        public uint DTUId { get; set; }

        /// <summary>
        /// 分组配置中
        /// </summary>
        public Dictionary<string, object> Paramters = new Dictionary<string, object>();
    }
}