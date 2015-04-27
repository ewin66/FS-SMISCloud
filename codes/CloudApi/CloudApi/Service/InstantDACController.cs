using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Service
{

    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;


    public class InstantDACController : ApiController
    {

        [AcceptVerbs("Get")]
        [LogInfo("获取DTU列表", false)]
        public object GetDtu(int structId)
        {
            return null;
        }
    }
}
