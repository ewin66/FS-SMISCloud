using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Config
{
    public class SerialSoundAndLightList : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SerialSoundAndLight();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SerialSoundAndLight) element).Name;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
}