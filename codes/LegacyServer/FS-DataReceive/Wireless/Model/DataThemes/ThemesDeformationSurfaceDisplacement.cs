// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemesDeformationSurfaceDisplacement.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ThemesDeformationSurfaceDisplacement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    [Serializable]
    [Table(Name = "T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT")]
    internal class ThemesDeformationSurfaceDisplacement : ThemesDataBase
    {
        
        public float PhysicalQuantityXValue { get; set; }

        [Column(Name = "SURFACE_DISPLACEMENT_X_VALUE", DbType = "numeric(18,6)")]
        public float SurfaceDisplacementXValue { get; set; }

        public float PhysicalQuantityYValue { get; set; }

        [Column(Name = "SURFACE_DISPLACEMENT_Y_VALUE", DbType = "numeric(18,6)")]
        public float SurfaceDisplacementYValue { get; set; }

        public float PhysicalQuantityZValue { get; set; }

        [Column(Name = "SURFACE_DISPLACEMENT_Z_VALUE", DbType = "numeric(18,6)")]
        public float SurfaceDisplacementZValue { get; set; }
    }
}
