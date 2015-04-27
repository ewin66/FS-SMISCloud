#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="TableFieldInfo.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140604 by WIN .
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

    [Table(Name = "TableFieldInfos")]
    public class TableFieldInfo : TableBase<TableFieldInfo>
    {
        [PrimaryKey(Name = "ID")]
        public long Id { get; set; }

        [Field(Name = "DataBaseID")]
        public int DataBaseId { get; set; }

        [Field(Name = "TableName")]
        public string TableName { get; set; }

        [Field(Name = "ModuleNo")]
        public string ModuleNo { get; set; }

        [Field(Name = "channelId")]
        public string ChannelId { get; set; }

        [Field(Name = "OtherFlag")]
        public string OtherFlag { get; set; }

        [Field(Name = "AcqTime")]
        public string AcqTime { get; set; }

        [Field(Name = "SensorType")]
        public int SensorType { get; set; }

        [Field(Name = "SensorID")]
        public string SensorID { get; set; }

        [Field(Name = "ValueNameCount")]
        public int ValueNameCount { get; set; }

        [Field(Name = "ExtractValueNameID1")]
        public int ExtractValueNameId1 { get; set; }

        [Field(Name = "ExtractFieldName1")]
        public string ExtractFieldName1 { get; set; }

        [Field(Name = "ExtractValueNameID2")]
        public int ExtractValueNameId2 { get; set; }

        [Field(Name = "ExtractFieldName2")]
        public string ExtractFieldName2 { get; set; }

        [Field(Name = "ExtractValueNameID3")]
        public int ExtractValueNameId3 { get; set; }

        [Field(Name = "ExtractFieldName3")]
        public string ExtractFieldName3 { get; set; }

        [Field(Name = "ExtractValueNameID4")]
        public int ExtractValueNameId4 { get; set; }

        [Field(Name = "ExtractFieldName4")]
        public string ExtractFieldName4 { get; set; }

        [Field(Name = "ExtractValueNameID5")]
        public int ExtractValueNameId5 { get; set; }

        [Field(Name = "ExtractFieldName5")]
        public string ExtractFieldName5 { get; set; }

        [Field(Name = "ExtractValueNameID6")]
        public int ExtractValueNameId6 { get; set; }

        [Field(Name = "ExtractFieldName6")]
        public string ExtractFieldName6 { get; set; }

        [Field(Name = "ExtractValueNameID7")]
        public int ExtractValueNameId7 { get; set; }

        [Field(Name = "ExtractFieldName7")]
        public string ExtractFieldName7 { get; set; }

        public string Reserved2 { get; set; }
    }
}