#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorInfo.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140526 by WIN .
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

    [Table(Name = "SensorInfo")]
    public class SensorInfo : TableBase<SensorInfo>
    {
        /// <summary>
        /// 传感器配置ID
        /// </summary>
        [PrimaryKey(Name = "ID")]
        public long ID { get; set; }

        /// <summary>
        /// 传感器所属模块号
        /// </summary>
        [Field(Name = "ModuleNo")]
        public string ModuleNo { get; set; }

        /// <summary>
        /// 传感器所在通道号
        /// </summary>
        [Field(Name = "ChannelID")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 预留字段
        /// </summary>
        [Field(Name = "Reserved")]
        public string Reserved { get; set; }

        /// <summary>
        /// 该传感器的安全监测因素
        /// </summary>
        [Field(Name = "SAFETYFACTORTYPEID")]
        public int Safetyfactortypeid { get; set; }

        /// <summary>
        /// 该传感器的公式参数配置ID(如果没有默认为0)
        /// </summary>
        [Field(Name = "FORMULAID_SET_ID")]
        public int FormulaidSetId { get; set; }

        public SensorInfo()
        {
            FormulaidSetId = 0;
        }

        /// <summary>
        /// 该组所属数据库
        /// </summary>
        [Field(Name = "DataBaseID")]
        public int DataBaseId { get; set; }
    }
}