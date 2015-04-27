namespace FS.SMIS_Cloud.NGDAC.Node
{
    using System;

    public class DTUConnectionStatusChangedMsg
    {
        public string DTUID { get; set; }

        public bool IsOnline { get; set; }

        public DateTime TimeStatusChanged { get; set; }
    }
}