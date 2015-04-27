using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model.DataThemes
{
    [Serializable]
    [Table(Name = "T_THEMES_DEFORMATION_CRACK")]
    public class ThemesDeformationCrack : ThemesDataBase
    {
        public float CrackPhysicalQuantityValue { get; set; }

        [Column(Name = "CRACK_VALUE", DbType = "numeric(18,6)")]
        public float CrackValue { get; set; }
    }
}
