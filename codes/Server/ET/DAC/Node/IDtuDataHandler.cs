
namespace FS.SMIS_Cloud.DAC.Node
{
    public interface IDtuDataHandler
    {
         void OnDataReceived(IDtuConnection sender, DtuMsg msg);
    }
}
