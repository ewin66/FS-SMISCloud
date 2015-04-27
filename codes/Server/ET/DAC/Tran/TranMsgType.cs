
namespace FS.SMIS_Cloud.DAC.Tran
{
    // 數據幀類型.
    public enum TranMsgType
    {
        HeartBeat = (byte)1,  // HeartBeat req.
        Ack = (byte)2,        // Acknowledge
        Vib = (byte)11,       // 仅振动参数文件
        Dac,                  // Data of DAC result
        Com,                  // Data of Equipment Wrapper.

    }
}
