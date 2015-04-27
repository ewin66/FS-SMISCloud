using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    [Serializable]
    [Table(Name = "T_THEMES_FORCE_STEELBAR")]
    public class ThemesForceSteelbar : ThemesDataBase
    {
        public float PhysicalQuantityValue { get; set; }

        [Column(Name = "STEELBAR_FORCE_VALUE", DbType = "numeric(18,6)")]
        public float SteelbarForceValue { get; set; }

        public float TemperatureValue { get; set; }
    }
}
