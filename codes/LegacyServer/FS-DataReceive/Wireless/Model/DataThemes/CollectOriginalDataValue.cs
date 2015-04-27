#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="ColThemesDeformationSurfaceDisplacement.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2013 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20131224 created by Win
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    /// <summary>
    /// The col themes deformation surface displacement.
    /// </summary>
    [Serializable]
    [Table(Name = "T_COL_ORIGINAL_DATAVALUE")]
    public class CollectOriginalDataValue : ThemesDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectOriginalDataValue"/> class.
        /// </summary>
        public CollectOriginalDataValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectOriginalDataValue"/> class.
        /// </summary>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="safetyFactorTypeID">
        /// The safety factor type id.
        /// </param>
        /// <param name="acqtime">
        /// The acqtime.
        /// </param>
        /// <param name="structureID">
        /// The structure id.
        /// </param>
        public CollectOriginalDataValue(int sensorId, int safetyFactorTypeID, DateTime acqtime, int structureID)
            : base(sensorId, safetyFactorTypeID, acqtime)
        {
            this.StructureID = structureID;
        }

        /// <summary>
        ///  Gets or sets 结构物ID.
        /// </summary>
        [Column(Name = "STRUCTURE_ID", DbType = "int")]
        public int StructureID { get; set; }

        /// <summary>
        /// Gets or sets the deflection value.
        /// </summary>
        [Column(Name = "CollectOriginalValue1", DbType = "numeric(18,6)", CanBeNull = true)]
        public float? CollectOriginalValue1 { get; set; }

        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        [Column(Name = "CollectOriginalValue2", DbType = "numeric(18,6)", CanBeNull = true)]
        public float? CollectOriginalValue2 { get; set; }

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        [Column(Name = "CollectOriginalValue3", DbType = "numeric(18,6)", CanBeNull = true)]
        public float? CollectOriginalValue3 { get; set; }

        /// <summary>
        /// Gets or sets the z value.
        /// </summary>
        [Column(Name = "CollectOriginalValue4", DbType = "numeric(18,6)", CanBeNull = true)]
        public float? CollectOriginalValue4 { get; set; }
    }
}