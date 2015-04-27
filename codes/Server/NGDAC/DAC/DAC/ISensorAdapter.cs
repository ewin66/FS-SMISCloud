
namespace FS.SMIS_Cloud.NGDAC.DAC
{
    public interface ISensorAdapter 
    {
        // 编码
        void Request(ref SensorAcqResult senAcq);

        // 解码
        void ParseResult(ref SensorAcqResult rawData);
    }
}
