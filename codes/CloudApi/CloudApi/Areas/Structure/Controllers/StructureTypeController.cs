using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Structure.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    public class StructureTypeController : ApiController
    {
        /// <summary>
        /// 获取结构物类型列表
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物类型列表", false)]
        [Authorization(AuthorizationCode.S_Structure_Add)]
        [Authorization(AuthorizationCode.S_Structure_Modify)]
        public object GetStructType()
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query =
                    entity.T_DIM_STRUCTURE_TYPE.Where(t => t.ORDERBY_COLUMN == 2)
                        .Select(t => new { structTypeId = t.ID, structType = t.NAME_STRUCTURE_TYPE_CN });
                return query.ToList();
            }
        }
    }
}
