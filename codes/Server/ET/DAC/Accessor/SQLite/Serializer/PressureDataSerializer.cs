using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.Pressure_HS,ProtocolType.Pressure_MPM })]
    class PressureDataSerializer : SQLiteDataSerializer
    {

        public PressureDataSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalPressureData",
                ThemeColums = new[] { "ColPressureValue", "CulcPressureValue" },
                DataType = typeof(PressureData),
                OriginalDataCount = 2,
                ThemesDataOffset = 0,
            })
        {
        }
         
    }
}
