using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Menu
{
    public class MenuController : ApiController
    {

        /// <summary>
        /// 获取用户权限下菜单
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns>用户权限下菜单</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户权限下菜单", false)]
        public object GetUserRoleMenu(int userId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var ur = (from r in entity.T_DIM_ROLE
                          from u in entity.T_DIM_USER
                          where u.ROLE_ID == r.ROLE_ID
                           && u.USER_NO == userId
                           && u.USER_IS_ENABLED
                          select u.ROLE_ID);

                if (!ur.ToList().Any())
                {
                    return ur.ToList();
                }
                else
                {
                    if (ur.FirstOrDefault() == 1)
                    {
                        var query = from r in entity.T_DIM_RESOURCE
                                    where r.RESOURCE_MENU != null && r.PARENT_ID == "0"
                                    select new
                                    {
                                        RESOURCE_ID = r.RESOURCE_ID,
                                        RESOURCE_NAME = r.RESOURCE_NAME,
                                        RESOURCE_MENU = r.RESOURCE_MENU,
                                        data = (from cr in entity.T_DIM_RESOURCE
                                                where cr.PARENT_ID == r.RESOURCE_ID && cr.RESOURCE_MENU != null
                                                select new
                                                {
                                                    RESOURCE_ID = cr.RESOURCE_ID,
                                                    RESOURCE_NAME = cr.RESOURCE_NAME,
                                                    RESOURCE_MENU = cr.RESOURCE_MENU
                                                })
                                    };
                        return query.ToList();
                    }
                    else
                    {
                        var query = from rr in entity.T_DIM_ROLE_RESOURCE
                                    from r in entity.T_DIM_RESOURCE
                                    where r.RESOURCE_MENU != null && r.PARENT_ID == "0"
                                    && rr.ROLE_ID == ur.FirstOrDefault()
                                    && rr.RESOURCE_ID == r.RESOURCE_ID
                                    select new
                                    {
                                        RESOURCE_ID = r.RESOURCE_ID,
                                        RESOURCE_NAME = r.RESOURCE_NAME,
                                        RESOURCE_MENU = r.RESOURCE_MENU,
                                        data = (from crr in entity.T_DIM_ROLE_RESOURCE
                                                from cr in entity.T_DIM_RESOURCE
                                                where cr.PARENT_ID == r.RESOURCE_ID && cr.RESOURCE_MENU != null
                                                && crr.ROLE_ID == ur.FirstOrDefault()
                                                && crr.RESOURCE_ID == cr.RESOURCE_ID
                                                select new
                                                {
                                                    RESOURCE_ID = cr.RESOURCE_ID,
                                                    RESOURCE_NAME = cr.RESOURCE_NAME,
                                                    RESOURCE_MENU = cr.RESOURCE_MENU
                                                })
                                    };

                        return query.ToList();
                    }
                }
            }
        }
    }
}
