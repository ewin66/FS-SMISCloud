using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite.Serializer
{
    [ProtocolDataSerializer(Protocols = new ProtocolType[] { ProtocolType.TempHumidity,ProtocolType.TempHumidity_OLD })]
    internal class TempHumidityDataSerializer : SQLiteDataSerializer
    {
        public TempHumidityDataSerializer()
            : base(new DataMetaInfo
            {
                TableName = "D_OriginalTempHumiData",
                ThemeColums = new[] {"TEMPERATURE_VALUE", "HUMILITY_VALUE"},
                DataType = typeof (TempHumidityData),
                OriginalDataCount = 2,
                ThemesDataOffset = 0,
            })
        {
        }
    }
}
