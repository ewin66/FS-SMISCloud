using System;

namespace FS.SMIS_Cloud.Services.Messages
{
    [Serializable]
    public class DataContinuWarningMsg : RequestWarningReceivedMessage
    {
        public bool DataStatus { get; set; }
    }
}