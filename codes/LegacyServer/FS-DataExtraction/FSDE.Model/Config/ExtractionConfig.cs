#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ExtractionConfig.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140603 by WIN .
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

    [Table(Name = "ExtractionConfig")]
    public class ExtractionConfig : TableBase<ExtractionConfig>
    {
        [PrimaryKey(Name = "ID")]
        public long Id { get; set; }

        [Field(Name = "DataBaseId")]
        public int DataBaseId { get; set; }

        [Field(Name = "TableName")]
        public string TableName { get; set; }

        [Field(Name = "Acqtime")]
        public string Acqtime { get; set; }
    }
}