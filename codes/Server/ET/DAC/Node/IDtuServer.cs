
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.Node
{
    public delegate void ConnectStatusEventHandler(IDtuConnection c, WorkingStatus oldStatus, 
        WorkingStatus newStatus);

    public interface IDtuServer
    {
        IDtuConnection GetConnection(DtuNode node);
        ConnectStatusEventHandler OnConnectStatusChanged { get; set; }
        void Start();
        void Stop();
        bool Send(string dtuid, byte[] buffer);

        bool Disconnect(object dtu);
    }
}
