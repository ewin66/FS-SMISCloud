
namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using System.Collections.Generic;

    // 发送代理.
    public interface ITranDataSendDelegator
    {
        void Init(Dictionary<string, string> pms);
        bool SSend(TranMsg req, int timeoutInSeconds, out TranAckMsg resp);
        bool Connect();
        void Disconnect();
        bool HeartBeat();

        bool IsConnected();
    }
}
