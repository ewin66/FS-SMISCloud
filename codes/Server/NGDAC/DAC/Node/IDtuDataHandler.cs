
namespace FS.SMIS_Cloud.NGDAC.Node
{
    public interface IDtuDataHandler
    {
         void OnDataReceived(IDtuConnection sender, DtuMsg msg);
    }
}
