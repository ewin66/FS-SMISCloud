using System;
using ZYB;

namespace DataCenter.Communication.comm
{
   public enum DtuState:byte
    {
        Online=0,
       Offline=1,
       Other
    }

   public class DtuOnOffLineLogEventArgs:EventArgs
    {

       public DtuState State { get; set; }
       public DtuData  DtuInfo { get; set; }
    }
}
