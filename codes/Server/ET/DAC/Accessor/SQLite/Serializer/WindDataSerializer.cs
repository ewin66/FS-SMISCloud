using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.Wind_OSL})]
    internal class WindDataSerializer:SQLiteDataSerializer
    {
        public WindDataSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalWindData",
                ThemeColums = new[] { "WIND_SPEED_VALUE", "WIND_DIRECTION_VALUE", "WIND_ELEVATION_VALUE" },
                DataType = typeof(Wind3dData),
                OriginalDataCount = 3,
                ThemesDataOffset = 0,
            })
        {
        }
    }
}
