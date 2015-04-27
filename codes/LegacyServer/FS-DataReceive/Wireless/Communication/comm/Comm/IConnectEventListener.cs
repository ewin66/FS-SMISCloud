namespace DataCenter.Communication.comm.Comm
{
    public interface IConnectEventListener
    {
        void OnConnectionStatusChanged(Connection c, WorkingStatus oldStatus, WorkingStatus newStatus);
    }
}
