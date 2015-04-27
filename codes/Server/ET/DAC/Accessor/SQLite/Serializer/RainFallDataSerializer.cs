using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.RainFall})]
    class RainFallDataSerializer : SQLiteDataSerializer
    {

        public RainFallDataSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalRainFallData",
                ThemeColums = new[] { "RainFall" },
                DataType = typeof(RainFallData),
                OriginalDataCount = 1,
                ThemesDataOffset = 0,
            })
        {
        }
    }
}
