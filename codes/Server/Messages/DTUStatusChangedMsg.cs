using System;

namespace FS.SMIS_Cloud.Services.Messages
{
    [Serializable]
    public class DTUStatusChangedMsg : MessageBase
    {
        /// <summary>
        /// Gets or sets 告警类型ID
        /// </summary>
        public string WarningTypeId { get; set; }

        /// <summary>
        /// Gets or sets 设备类型ID
        /// </summary>
        public int DeviceTypeId { get; set; }

        public string DTUID { get; set; }

        public bool IsOnline { get; set; }
        

        /// <summary>
        /// Gets or sets 告警内容
        /// </summary>
        public string WarningContent { get; set; }

        public DateTime TimeStatusChanged { get; set; }
    }
}