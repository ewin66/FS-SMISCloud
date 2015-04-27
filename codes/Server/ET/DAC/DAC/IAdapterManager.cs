

using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.DAC 
{
    public interface IAdapterManager
    {
        void Initialize();
        ISensorAdapter GetSensorAdapter(uint protocolType);
        Adapter GetAdapter(uint protocolType);
        void ReLoad();
    }
}
