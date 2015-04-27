// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemesVibrationDeckVibration.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ThemesVibrationDeckVibration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    [Serializable]
    [Table(Name = "T_THEMES_VIBRATION_DECK_VIBRATION")]
    public class ThemesVibrationDeckVibration : ThemesDataBase
    {
       // [Column(Name = "DECK_VIBRATION_PHYSICAL_QUANTITY_VALUE", DbType = "numeric(18,6)")]
        public float DeckVibrationPhysicalQuantityValue { get; set; }

        [Column(Name = "DECK_VIBRATION_VALUE", DbType = "numeric(18,6)")]
        public float DeckVibrationValue { get; set; }
    }


}
