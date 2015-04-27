#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorGroup.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150414 by LINGWENLONG .
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

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config
{
    [Serializable]
    public class SensorGroup
    {
        public int GroupId { get; set; }

        public int DtuId { get; set; }

        public string DtuCode { get; set; }

        public int DacInterval { get; set; }
    }
}