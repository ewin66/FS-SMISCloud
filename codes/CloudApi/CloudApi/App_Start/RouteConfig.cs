namespace FreeSun.FS_SMISCloud.Server.CloudApi
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// 路由配置类
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 配置路由
        /// </summary>
        /// <param name="routes">路由集合</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}