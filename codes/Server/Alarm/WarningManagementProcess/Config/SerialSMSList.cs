using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class SerialSMSList : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SerialSMS();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SerialSMS) element).Com;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
}