using System;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class ConfigErrorException : Exception
    {
        public ConfigErrorException(string msg)
            : base(msg)
        {
        }
    }
}