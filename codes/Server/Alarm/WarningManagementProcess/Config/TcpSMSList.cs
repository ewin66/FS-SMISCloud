using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class TcpSoundAndLightList : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TcpSoundAndLight();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TcpSoundAndLight) element).Ip;
        }
    }
}