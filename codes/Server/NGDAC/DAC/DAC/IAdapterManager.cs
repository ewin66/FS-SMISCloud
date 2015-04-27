

namespace FS.SMIS_Cloud.NGDAC.DAC 
{
    using FS.SMIS_Cloud.NGDAC.Model;

    public interface IAdapterManager
    {
        void Initialize();
        ISensorAdapter GetSensorAdapter(uint protocolType);
        Adapter GetAdapter(uint protocolType);
        void ReLoad();
    }
}
