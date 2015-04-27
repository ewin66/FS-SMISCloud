using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite 
{
    public abstract class SQLiteDataSerializer : BaseDataSerializer
    {
        protected SQLiteDataSerializer(DataMetaInfo meta) : base(meta)
        {
        }

        protected override void GetCommColumnValue(SensorAcqResult sr, out string col, out string val)
        {
            col =
                "[SENSOR_Set_ID],[ModuleNo],[ChannelID],[LOCATION_DESCRIPTION],[ACQUISITION_DATETIME],[SAFETY_FACTOR_TYPE_ID]";
            Sensor s = sr.Sensor;
            val = string.Format("{0},{1},{2},'{3}','{4:yyyy-M-d HH:mm:ss}',{5}",
                s.SensorID, s.ModuleNo, s.ChannelNo, s.Name, sr.Data.AcqTime, s.FactorType);
        }
    }
}
