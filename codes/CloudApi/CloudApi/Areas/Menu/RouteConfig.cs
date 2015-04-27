namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Menu
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 获取用户权限下菜单
            routes.MapHttpRoute(
                name: "GetUserRoleMenu",
                routeTemplate: "user/{userId}/menuList",
                defaults: new { controller = "Menu", action = "GetUserRoleMenu" });
        }
    }
}