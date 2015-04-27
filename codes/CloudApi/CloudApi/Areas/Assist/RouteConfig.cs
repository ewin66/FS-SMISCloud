namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Assist
{
    using System.Web.Http;
    using System.Web.Routing;

    /// <summary>
    /// 辅助接口路由配置
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 辅助接口路由配置
        /// </summary>
        /// <param name="routes">路由集合</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 地址
            routes.MapHttpRoute(
                "GetProvinces",
                "assist/province",
                new { controller = "Address", action = "GetProvinces" });

            routes.MapHttpRoute(
                "GetCities",
                "assist/city/{provinceCode}",
                new { controller = "Address", action = "GetCities" },
                new { provinceCode = @"^\d+$" });

            routes.MapHttpRoute(
                "GetCountries",
                "assist/country/{cityCode}",
                new { controller = "Address", action = "GetCountries" },
                new { cityCode = @"^\d+$" });
        }
    }
}