using System;

namespace FS.SMIS_Cloud.DAC.Node
{
    public class DTUConnectionStatusChangedMsg
    {
        public string DTUID { get; set; }

        public bool IsOnline { get; set; }

        public DateTime TimeStatusChanged { get; set; }
    }
}