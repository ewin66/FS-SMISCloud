//  --------------------------------------------------------------------------------------------
//  <copyright file="ThemesDeformationBridgeDeflection.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2013 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20131224
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------

using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    /// <summary>
    /// The themes deformation bridge deflection.
    /// </summary>
    [Serializable]
    [Table(Name = "T_THEMES_DEFORMATION_BRIDGE_DEFLECTION")]
    public class ThemesDeformationBridgeDeflection : ThemesDataBase
    {
        /// <summary>
        /// Gets or sets the deflection value.
        /// </summary>
        [Column(Name = "DEFLECTION_VALUE", DbType = "numeric(18,6)")]
        public float DeflectionValue { get; set; }
    }
}