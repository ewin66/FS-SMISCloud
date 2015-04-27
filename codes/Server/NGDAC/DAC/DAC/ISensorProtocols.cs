namespace FS.SMIS_Cloud.NGDAC.DAC
{
    using FS.SMIS_Cloud.NGDAC.Model;

    public interface ISensorProtocols
    {
        ProtocolType[] Protocols { get; }
    }
}
