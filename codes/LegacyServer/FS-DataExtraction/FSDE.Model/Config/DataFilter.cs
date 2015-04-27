#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataFilter.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140529 by WIN .
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

    [Table(Name = "DataFilter")]
    public class DataFilter : TableBase<DataFilter>
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey(Name = "ID")]
        public long Id { get; set; }

        /// <summary>
        /// 做数据过滤的监测因素ID
        /// </summary>
        [Field(Name = "SafetyFactorType")]
        public int SafetyFactorType { get; set; }

        /// <summary>
        /// 过滤方式
        /// </summary>
        [Field(Name = "FilterType")]
        public int FilterType { get; set; }

        [Field(Name = "DataBaseId")]
        public int DataBaseId { get; set; }
    }
}