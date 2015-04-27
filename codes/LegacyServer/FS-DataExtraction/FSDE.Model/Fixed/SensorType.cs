#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorType.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace FSDE.Model.Fixed
{
    using SqliteORM;

    [Table(Name = "SensorType")]
    public class SensorType : TableBase<SensorType>
    {
        /// <summary>
        /// 传感器类型ID
        /// </summary>
        [PrimaryKey(Name = "ID")]
        public long Id { get; set; }

        /// <summary>
        /// 传感器类型
        /// </summary>
        [Field(Name = "SensorTypeCode")]
        public string SensorTypeCode { get; set; }

        /// <summary>
        /// 可以监测的安全因素
        /// </summary>
        [Field(Name = "SafetyFactorTypes")]
        public string SafetyFactorTypes { get; set; }

        /// <summary>
        /// 计算公式ID
        /// </summary>
        [Field(Name = "FORMULA_ID")]
        public int FormulaId { get; set; }
    }
}