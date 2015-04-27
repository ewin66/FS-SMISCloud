using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.User.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    public class RoleController : ApiController
    {
        [AcceptVerbs("Get")]
        [LogInfo("获取角色列表", false)]
        public object GetRoles()
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = db.T_DIM_ROLE.Where(r => r.ROLE_CODE != "supadmin").Select(r => new { roleId = r.ROLE_ID, roleName = r.ROLE_DESCRIPTION });

                return query.ToList();
            }            
        }
    }
}
