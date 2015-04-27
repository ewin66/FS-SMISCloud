
using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    [Serializable]
    [Table(Name = "T_THEMES_ENVI_TEMP_HUMI")]
    internal class ThemesEnviTempHumi : ThemesDataBase
    {
        [Column(Name = "TEMPERATURE_VALUE", DbType = "numeric(18,6)")]
        public float TemperatureValue { get; set; }

        public float PhysicalQuantityTemperatureValue { get; set; }

        [Column(Name = "HUMILITY_VALUE", DbType = "numeric(18,6)")]
        public float HumilityValue { get; set; }

        public float PhysicalQuantityHumilityValue { get; set; }
    }
}
