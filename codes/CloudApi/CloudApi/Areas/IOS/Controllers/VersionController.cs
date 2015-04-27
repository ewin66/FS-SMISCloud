namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.IOS.Controllers
{
    using System.Linq;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    public class VersionController : ApiController
    {
        /// <summary>
        /// 获取IOS客户端最新版本
        /// </summary>
        /// <returns>最新版本</returns>
        [LogInfo("获取IOS客户端最新版本", false)]
        public object GetIosLastVersion()
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from version in entity.T_IOS_VERSION
                    where version.ReleaseTime ==
                          (from v in entity.T_IOS_VERSION
                              select v.ReleaseTime).Max()
                    select new
                    {
                        versionNo = version.VersionNo,
                        upgradeUrl = version.UpgradeUrl
                    };
                return query.ToList().FirstOrDefault();
            }
        }
    }
}
