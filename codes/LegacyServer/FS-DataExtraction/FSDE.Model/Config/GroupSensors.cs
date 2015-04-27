#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="GroupSensors.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace FSDE.Model.Config
{
    using SqliteORM;

    [Table(Name = "GroupSensor")]
    public class GroupSensors : TableBase<GroupSensors>
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey(Name = "ID")]
        public long ID { get; set; }

        /// <summary>
        /// 传感器组ID
        /// </summary>
        [Field(Name = "GroupID")]
        public int GroupID { get; set; }

        /// <summary>
        ///  传感器配置ID
        /// </summary>
        [Field(Name = "SensorID")]
        public int SensorID { get; set; }

        /// <summary>
        /// 传感器标记(当ID不能确定唯一传感器时使用)
        /// </summary>
        [Field(Name = "SensorFlag")]
        public string SensorFlag { get; set; }

        /// <summary>
        /// 预留字段
        /// </summary>
        [Field(Name = "Reserved")]
        public string Reserved { get; set; }

    }
}