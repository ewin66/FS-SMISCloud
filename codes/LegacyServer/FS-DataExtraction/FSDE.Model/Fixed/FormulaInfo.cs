#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="FormulaInfo.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20140227 created by Win
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
    using System;

    using SqliteORM;

    /// <summary>
    /// The formula info.
    /// </summary>
    [Serializable]
    [Table(Name = "VIEW_FORMULA_INFO")]
    public class FormulaInfo : TableBase<FormulaInfo>
    {
        /// <summary>
        /// 传感器计算公式与参数匹配ID.
        /// </summary>
        [Field(Name = "FORMULA_PARA_ID")]
        public long FormulaParaId { get; set; }

        /// <summary>
        /// 传感器计算公式id.
        /// </summary>
        [Field(Name = "FORMULAID")]
        public long FormulaId { get; set; }

        /// <summary>
          /// 传感器计算公式名.
        /// </summary>
          [Field(Name = "FormulaName")]
        public string FormulaName { get; set; }

        /// <summary>
          /// 传感器计算公式表达式.
        /// </summary>
        [Field(Name = "FormulaExpressionCode")]
        public string FormulaExpression { get; set; }

        /// <summary>
        /// 传感器计算公式参数个数.
        /// </summary>
          [Field(Name = "PARA_COUNT")]
        public int ParaCount { get; set; }

        /// <summary>
          /// 传感器计算公式参数名.
        /// </summary>
          [Field(Name = "PARA_NAME")]
        public string ParaName { get; set; }

        /// <summary>
          /// 传感器计算公式参数符号.
        /// </summary>
          [Field(Name = "PARA_ALIAS")]
        public string ParaAlias { get; set; }

        /// <summary>
          /// 传感器计算公式参数名ID
        /// </summary>
        [Field(Name = "PARA_NAME_ID")]
          public long ParaNameId { get; set; }
    }
}
