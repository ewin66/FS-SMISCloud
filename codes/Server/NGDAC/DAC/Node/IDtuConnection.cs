namespace FS.SMIS_Cloud.NGDAC.Node
{
    public interface IDtuConnection
    {
        bool IsOnline { get;}
        DtuMsg Ssend(byte[] buffer, int timeout); // Synchronized send
        bool Asend(byte[] buff);                  // Asynchronized send
        string DtuID { get; }
        WorkingStatus Status { get; }
        bool Connect();
        void Disconnect();
        bool IsAvaliable();
    }
}
