using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor
{
    public interface IDataSerializer 
    {
        string GetInsertSql(SensorAcqResult data);
        DataMetaInfo MetaInfo { get; }
        DataMetaInfo GetMeta(Type type);
    }
}
