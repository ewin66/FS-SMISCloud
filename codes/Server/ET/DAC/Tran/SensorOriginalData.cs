using System;

namespace FS.SMIS_Cloud.DAC.Tran
{
    public class SensorOriginalData 
    {
        public int ID { get; set; }
        public int SID { get; set; }
        public uint Type { get; set; }
        public int ModuleNo { get; set; }
        public int ChannelNo { get; set; }
        public DateTime AcqTime { get; set; }
        public double[] Values { get; set; }
    }
}
