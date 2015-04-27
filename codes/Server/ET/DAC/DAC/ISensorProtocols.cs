using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.DAC
{
    public interface ISensorProtocols
    {
        ProtocolType[] Protocols { get; }
    }
}
