using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.LVDT_BL, ProtocolType.LVDT_XW})]
    internal class LVDTDataSerializer : SQLiteDataSerializer
    {
        public LVDTDataSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalLVDTData",
                ThemeColums = new[] {"OriginalDisplayment", "OffsetDisplayment"},
                DataType = typeof (LVDTData),
                OriginalDataCount = 2,
                ThemesDataOffset = 0,
            })
        {
        }
    }
}
