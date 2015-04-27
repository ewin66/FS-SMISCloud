using System;
using System.ComponentModel.Composition;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Accessor.SQLite
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ProtocolDataSerializer : ExportAttribute
    {
        public ProtocolDataSerializer() : base(typeof (IDataSerializer))
        {
        }
        public ProtocolType[] Protocols { get; set; }
    }

}
