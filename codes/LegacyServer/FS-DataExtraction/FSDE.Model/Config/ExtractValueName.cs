// // --------------------------------------------------------------------------------------------
// // <copyright file="ExtractValueName.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140605
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

    [Table(Name = "ExtractValueName")]
    public class ExtractValueName
    {
        [PrimaryKey(Name = "ID")]
        public long Id { get; set; }

        [Field(Name = "ValueName")]
        public string ValueName { get; set; }

        [Field(Name = "ValueCode")]
        public string ValueCode { get; set; }

        [Field(Name = "CATEGORYID")]
        public int CatagoryId { get; set; }
    }
}