

namespace FS.SMIS_Cloud.DAC.Node
{
    public enum WorkingStatus
    {
        NA,           // 不可用.
        IDLE,         // 空闲
        WORKING_ASYNC,// 等待异步应答;
        WORKING_SYNC, // 等待同步应答.
        CONFIGING,    // 配置中, 和 WORKING 互斥
    }
}
