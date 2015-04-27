
namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config
{
    public enum DtuType
    {
        Gprs, // GPRS DTU=>COM口采集;
        Com,  // 本地 COM 口采集
        File, // 本地文件接口
        Ftp,  // 远程FTP/SFTP +File 接口;
        Tcp,  // Tcp Socket
        Udp,  // Udp Socket
        Soap, // Soap
        Http, // Http
        Https, //https
    }
}
