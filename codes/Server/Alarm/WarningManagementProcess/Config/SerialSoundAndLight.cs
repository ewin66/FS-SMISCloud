using System;
using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class Mode : ConfigurationElement
    {
        [ConfigurationProperty("mode", IsRequired = true)]
        public string ModeString
        {
            get { return (string) this["mode"]; }
        }

        public int ModeInt
        {
            get { return Convert.ToInt32(ModeString, 2); }
        }
    }

    public class SerialSoundAndLight : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string) this["name"]; }
        }

        [ConfigurationProperty("baudrate", DefaultValue = "9600", IsRequired = false)]
        public int Baudrate
        {
            get { return (int) this["baudrate"]; }
        }

        [ConfigurationProperty("interval", DefaultValue = "10000", IsRequired = false)]
        public int Interval
        {
            get { return (int) this["interval"]; }
        }

        [ConfigurationProperty("organizationids", IsRequired = false)]
        public string Organizationids
        {
            get { return (string) this["organizationids"]; }
        }

        [ConfigurationProperty("warning", IsRequired = true)]
        public Mode WarningMode
        {
            get { return (Mode) this["warning"]; }
        }

        [ConfigurationProperty("minor", IsRequired = true)]
        public Mode MinorMode
        {
            get { return (Mode) this["minor"]; }
        }

        [ConfigurationProperty("major", IsRequired = true)]
        public Mode MajorMode
        {
            get { return (Mode) this["major"]; }
        }

        [ConfigurationProperty("critical", IsRequired = true)]
        public Mode CriticalMode
        {
            get { return (Mode) this["critical"]; }
        }
    }
}