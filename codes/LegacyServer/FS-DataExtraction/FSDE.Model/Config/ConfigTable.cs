// // --------------------------------------------------------------------------------------------
// // <copyright file="ConfigTable.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140619
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace FSDE.Model.Config
{
    using SqliteORM;

    [Table(Name = "ConfigTable")]
    public class ConfigTable:TableBase<ConfigTable>
    {
        [PrimaryKey(Name = "ID")]
        public long ID { get; set; }

        [Field(Name = "SensorID")]
        public string SensorId { get; set; }

        /// <summary>
        /// 传感器所属模块号
        /// </summary>
        [Field(Name = "ModuleNo")]
        public string ModuleNo { get; set; }

        [Field(Name = "ChannelId")]
        public string ChannelId { get; set; }

        [Field(Name = "OtherFlag")]
        public string OtherFlag { get; set; }

        [Field(Name = "TableName")]
        public string TableName { get; set; }

        [Field(Name = "DataBaseID")]
        public int DataBaseId { get; set; }
    }
}