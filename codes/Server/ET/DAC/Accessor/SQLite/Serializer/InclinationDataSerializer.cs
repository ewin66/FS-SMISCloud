using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[]
    {
        ProtocolType.Inclinometer_BOX, 
        ProtocolType.Inclinometer_OLD, 
        ProtocolType.Inclinometer_ROD,
        ProtocolType.Inclinometer_MOBIL
    })]
    internal class InclinationDataSerializer : SQLiteDataSerializer
    {
        public InclinationDataSerializer() : base(new DataMetaInfo
        {
            TableName = "D_OriginalInclinationData",
            ThemeColums = new[] {"AngleOriginalX", "AngleOriginalY", "AngleOffsetX", "AngleOffsetY"},
            DataType = typeof (InclinationData),
            OriginalDataCount = 4,
            ThemesDataOffset = 0,
        })
        {
        }
    }
}
