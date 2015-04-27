using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    [Serializable]
    [Table(Name = "T_THEMES_DEFORMATION_DEEP_DISPLACEMENT")]
    public class ThemesDeformationDeepDisplacement : ThemesDataBase
    {
        // public ThemesDeformationDeepDisplacement(){}

        // public ThemesDeformationDeepDisplacement(int sensorId, int safetyFactorTypeID, DateTime acqtime)
        //    : base(sensorId, safetyFactorTypeID, acqtime)
        // {
        // }

        [Column(Name = "DEEP_DISPLACEMENT_X_VALUE", DbType = "numeric(18,6)")]
        public float PhysicalQuantityXValue { get; set; }
        
        public float DeepDisplacementXValue { get; set; }

         [Column(Name = "DEEP_DISPLACEMENT_Y_VALUE", DbType = "numeric(18,6)")]
        public float PhysicalQuantityYValue { get; set; }
       
        public float DeepDisplacementYValue { get; set; }

        public float TemperatureValue { get; set; }
        
        public float DeepCumulativedisplacementXValue { get; set; }

        public float DeepCumulativedisplacementYValue { get; set; }
    }
}