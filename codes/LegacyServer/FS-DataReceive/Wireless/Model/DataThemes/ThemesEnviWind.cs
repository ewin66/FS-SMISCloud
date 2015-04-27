using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    [Serializable]
    [Table(Name = "T_THEMES_ENVI_WIND")]
    internal class ThemesEnviWind : ThemesDataBase
    {

        public float SpeedPhysicalQuantityValue { get; set; }

        public float DirectionPhysicalQuantityValue { get; set; }

        public float ElevationPhysicalQuantityValue { get; set; }

        [Column(Name = "WIND_SPEED_VALUE", DbType = "numeric(18,6)")]
        public float WindSpeedValue { get; set; }

        [Column(Name = "WIND_DIRECTION_VALUE", DbType = "numeric(18,6)")]
        public float WindDirectionValue { get; set; }

        [Column(Name = "WIND_ELEVATION_VALUE", DbType = "numeric(18,6)")]
        public float WindElevationValue { get; set; }
    }
}
