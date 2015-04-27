using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class ForwarderServiceConfig : ConfigurationSection
    {
        [ConfigurationProperty("serialSMSList")]
        [ConfigurationCollection(typeof (SerialSMS), AddItemName = "serialSMS")]
        public SerialSMSList SerialSmSList
        {
            get { return (SerialSMSList) this["serialSMSList"]; }
        }

        [ConfigurationProperty("tcpSoundAndLightList")]
        [ConfigurationCollection(typeof (TcpSoundAndLight), AddItemName = "tcpSoundAndLight")]
        public TcpSoundAndLightList TcpSmsList
        {
            get { return (TcpSoundAndLightList) this["tcpSoundAndLightList"]; }
        }

        [ConfigurationProperty("serialSoundAndLightList")]
        [ConfigurationCollection(typeof (SerialSoundAndLight), AddItemName = "serialSoundAndLight")]
        public SerialSoundAndLightList SerialSoundAndLightList
        {
            get { return (SerialSoundAndLightList) this["serialSoundAndLightList"]; }
        }
    }
}