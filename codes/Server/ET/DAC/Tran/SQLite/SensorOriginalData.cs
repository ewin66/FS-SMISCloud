using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Tran
{
    public class SensorOriginalData 
    {
        public int ID { get; set; }
        public int SID { get; set; }
        public ProtocolType Type { get; set; }
        public int ModuleNo { get; set; }
        public int ChannelNo { get; set; }
        public DateTime AcqTime { get; set; }
        public double[] Values { get; set; }
    }
}
