namespace FS.SMIS_Cloud.NGDAC.Node
{
    using FS.SMIS_Cloud.NGDAC.Model;

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
