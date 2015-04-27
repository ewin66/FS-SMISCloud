#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorGroup.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150408 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FS.SMIS_Cloud.NGDAC.Model
{
    public class SensorGroup
    {
        public uint GroupId { get; set; }

        public uint DtuId { get; set; }

        public string DtuCode { get; set; }

        public uint DacInterval { get; set; }
    }
}