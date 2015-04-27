// --------------------------------------------------------------------------------------------------------------------
// <copyright file="THEMES_CABLE_FORCE.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the THEMES_CABLE_FORCE type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    /// <summary>
    /// The theme s_ cabl e_ force.
    /// </summary>
    [Serializable]
    [Table(Name = "T_THEMES_CABLE_FORCE")]
    public class ThemesCableForce : ThemesDataBase
    {
        /// <summary>
        /// Gets or sets the cable force value.
        /// </summary>
        [Column(Name = "CABLE_FORCE_VALUE", DbType = "numeric(18,6)")]
        public float CableForceValue { get; set; }
    }
}