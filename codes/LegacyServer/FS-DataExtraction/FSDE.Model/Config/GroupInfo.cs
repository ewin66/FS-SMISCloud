#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="GroupInfo.cs" company="江苏飞尚安全监测咨询有限公司">
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

    [Table(Name = "SensorGroupInfo")]
    public class GroupInfo : TableBase<GroupInfo>
    {
        /// <summary>
        /// 传感器组ID
        /// </summary>
        [PrimaryKey(Name = "GroupID")]
        public long GroupID { get; set; }

        /// <summary>
        /// 传感器组名称
        /// </summary>
        [Field(Name = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 该组所属数据库
        /// </summary>
        [Field(Name = "DataBaseID")]
        public int DataBaseID { get; set; }

        /// <summary>
        /// 该组的安全监测因素
        /// </summary>
        [Field(Name = "SAFETYFACTORTYPEID")]
        public int Safetyfactortypeid { get; set; }

        /// <summary>
        /// 是否组合计算（一组传感器通过组合计算得到一个结果值）
        /// </summary>
        [Field(Name = "CombinationCalculate")]
        public bool CombinationCalculate { get; set; }
    }
}