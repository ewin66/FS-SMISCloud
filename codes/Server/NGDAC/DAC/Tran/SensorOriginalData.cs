namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using System;

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
