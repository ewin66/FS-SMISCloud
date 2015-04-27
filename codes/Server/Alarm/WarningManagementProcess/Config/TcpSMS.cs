using System;
using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class TcpMode : ConfigurationElement
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

    public class TcpSoundAndLight : ConfigurationElement
    {
        [ConfigurationProperty("ip", IsKey = true, IsRequired = false)]
        public string Ip
        {
            get { return (string) this["ip"]; }
        }

        [ConfigurationProperty("port", DefaultValue = "6667", IsRequired = false)]
        public int Port
        {
            get { return (int) this["port"]; }
        }

        [ConfigurationProperty("type", DefaultValue = "Server", IsRequired = false)]
        public TcpType Type
        {
            get { return (TcpType) this["type"]; }
        }

        [ConfigurationProperty("mode", DefaultValue = "0", IsRequired = false)]
        public int Mode
        {
            get { return (int) this["mode"]; }
        }

        [ConfigurationProperty("interval", DefaultValue = "1000", IsRequired = false)]
        public int Interval
        {
            get { return (int) this["interval"]; }
        }

        [ConfigurationProperty("listtimeout", DefaultValue = "200", IsRequired = false)]
        public int Listtimeout
        {
            get { return (int) this["listtimeout"]; }
        }

        [ConfigurationProperty("organizationids", IsRequired = false)]
        public string Organizationids
        {
            get { return (string) this["organizationids"]; }
        }

        [ConfigurationProperty("warning", IsRequired = false)]
        public TcpMode Warning
        {
            get { return (TcpMode) this["warning"]; }
        }

        [ConfigurationProperty("minor", IsRequired = false)]
        public TcpMode Minor
        {
            get { return (TcpMode) this["minor"]; }
        }

        [ConfigurationProperty("major", IsRequired = false)]
        public TcpMode Major
        {
            get { return (TcpMode) this["major"]; }
        }

        [ConfigurationProperty("critical", IsRequired = false)]
        public TcpMode Critical
        {
            get { return (TcpMode) this["critical"]; }
        }
    }
}