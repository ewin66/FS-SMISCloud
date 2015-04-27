
using System;
using FS.SMIS_Cloud.DAC.Node;

namespace FS.SMIS_Cloud.ET
{
    public class SensorStatus
    {
        public uint StructureId { get; set; }

        public string DtuCode { get; set; }

        public uint SensorId { get; set; }

        public string Location { get; set; }

        public int DacErrcode { get; set; }

        public string ErrMsg { get; set; }

        public bool IsOK
        {
            get { return DacErrcode == (int)Errors.SUCCESS; }
        }

        public DateTime AcqTime { get; set; }
    }
}
