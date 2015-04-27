namespace FS.SMIS_Cloud.NGDAC.DAC
{
    using System;
    using System.ComponentModel.Composition;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SensorAdapter : ExportAttribute
    {
        public SensorAdapter() : base(typeof(ISensorAdapter)) { }
        public uint Protocol { get; set; }
        public string ProtocolName { get; set; }
    }

}
