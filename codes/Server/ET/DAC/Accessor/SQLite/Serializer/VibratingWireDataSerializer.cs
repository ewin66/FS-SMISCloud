using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.VibratingWire,ProtocolType.VibratingWire_OLD })]
    internal class VibratingWireDataSerializer : SQLiteDataSerializer
    {
        public VibratingWireDataSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalVibratingWireData",
                ThemeColums = new[] {"Frequency_VALUE", "TEMPERATURE_VALUE", "PhysicalValue"},
                DataType = typeof (VibratingWireData),
                OriginalDataCount = 3,
                ThemesDataOffset = 0,
            })
        {
        }
    }
}
