using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class SerialSMS : ConfigurationElement
    {
        [ConfigurationProperty("com", IsKey = true, IsRequired = false)]
        public int Com
        {
            get { return (int) this["com"]; }
        }

        [ConfigurationProperty("baudrate", DefaultValue = "9600", IsRequired = false)]
        public uint Baudrate
        {
            get { return (uint) this["baudrate"]; }
        }

        [ConfigurationProperty("top", DefaultValue = "100", IsRequired = false)]
        public int Top
        {
            get { return (int) this["top"]; }
        }

        [ConfigurationProperty("interval", DefaultValue = "10000", IsRequired = false)]
        public int Interval
        {
            get { return (int) this["interval"]; }
        }

        [ConfigurationProperty("maxCapacity", DefaultValue = "200", IsRequired = false)]
        public int MaxCapacity
        {
            get { return (int) this["maxCapacity"]; }
        }

        [ConfigurationProperty("dispatchInterval", DefaultValue = "300", IsRequired = false)]
        public int DispatchInterval
        {
            get { return (int) this["dispatchInterval"]; }
        }

        [ConfigurationProperty("commitCount", DefaultValue = "100", IsRequired = false)]
        public int CommitCount
        {
            get { return (int) this["commitCount"]; }
        }

        [ConfigurationProperty("sendInterval", DefaultValue = "100")]
        public int SendInterval
        {
            get { return (int) this["sendInterval"]; }
        }

        [ConfigurationProperty("organizationids", IsRequired = false)]
        public string Organizationids
        {
            get { return (string) this["organizationids"]; }
        }
    }
}