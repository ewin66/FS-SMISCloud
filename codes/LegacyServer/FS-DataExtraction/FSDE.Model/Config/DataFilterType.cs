// // --------------------------------------------------------------------------------------------
// // <copyright file="DataFilterType.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140603
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using SqliteORM;

namespace FSDE.Model.Config
{

    [Table(Name = "DataFilterType")]
    public class DataFilterType : TableBase<DataFilterType>
    {
        [PrimaryKey(Name = "ID")]
        public long ID { get; set; }

        [Field(Name = "FilterDESCRIPTION")]
        public string FilterDesc { get; set; }
    }
}