using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.MagneticFlux })]
    class MageneticFluxDataSerializer : SQLiteDataSerializer
    {

        public MageneticFluxDataSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalMagneticFluxData",
                ThemeColums = new[] { "OrgVoltage", "HUMILITY_VALUE", "Mechan_Value" },
                DataType = typeof(MagneticFluxData),
                OriginalDataCount = 3,
                ThemesDataOffset = 0,
            })
        {
        }
    }
}
