using System.Linq;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Assist.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    public class AddressController : ApiController
    {
        /// <summary>
        /// 获取省份列表
        /// </summary>
        /// <returns> 省份列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取省份列表", false)]
        [Authorization(AuthorizationCode.S_Org)]
        [Authorization(AuthorizationCode.S_Structure)]
        public object GetProvinces()
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query =
                    (from r in entity.T_DIM_REGION
                     where r.REGION_TYPE_ID == 4
                     select new { id = r.REGION_ID, value = r.REGION_NAME_CN }).Concat(
                         from r in entity.T_DIM_REGION
                         where r.REGION_TYPE_ID == 3
                         select new { id = r.REGION_ID, value = r.REGION_NAME_CN });

                return query.ToList();
            }
        }

        /// <summary>
        /// 获取市列表
        /// </summary>
        /// <param name="provinceCode"> 省份编号 </param>
        /// <returns> 市列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取市列表", false)]
        [Authorization(AuthorizationCode.S_Org)]
        [Authorization(AuthorizationCode.S_Structure)]
        public object GetCities(int provinceCode)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var city = (from r in entity.T_DIM_REGION
                           where r.REGION_ID == provinceCode && r.REGION_TYPE_ID == 4
                           select r.REGION_NAME_CN).FirstOrDefault();
                if (city != null)
                {
                    return new[] { new { id = provinceCode, value = city } };
                }

                var query = from r in entity.T_DIM_REGION
                            where r.PARENT_REGION_ID == provinceCode
                            select new { id = r.REGION_ID, value = r.REGION_NAME_CN };

                return query.ToList();
            }
        }

        /// <summary>
        /// 获取县区列表
        /// </summary>
        /// <param name="cityCode"> 市编号 </param>
        /// <returns> 县区列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取县区列表", false)]
        [Authorization(AuthorizationCode.S_Org)]
        [Authorization(AuthorizationCode.S_Structure)]
        public object GetCountries(int cityCode)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from r in entity.T_DIM_REGION
                            where r.PARENT_REGION_ID == cityCode
                            select new { id = r.REGION_ID, value = r.REGION_NAME_CN };

                return query.ToList();
            }
        }
    }
}
