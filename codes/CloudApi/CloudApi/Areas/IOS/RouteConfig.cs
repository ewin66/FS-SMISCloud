namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.IOS
{
    using System.Web.Http;
    using System.Web.Routing;

    /// <summary>
    /// DATA路由配置
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 路由配置
        /// </summary>
        /// <param name="routes">路由集合</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "GetIosLastVersion",
                routeTemplate: "ios/last-version",
                defaults: new { controller = "Version", action = "GetIosLastVersion" });
        }
    }
}