using System;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.Voltage })]
    internal class VoltageSerializer : SQLiteDataSerializer
    {

        public VoltageSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalVoltageData",
                ThemeColums = new[] { "OrgVoltage", "displayment" },
                DataType = typeof(VoltageData),
                OriginalDataCount = 2,
                ThemesDataOffset = 0,
            })
        {
        }
    }
}
