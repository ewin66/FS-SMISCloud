namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Log
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
               "GetLogByUserId",
               "user/{userId}/log",
               new { controller = "Log", action = "GetLogByUserId" },
               new { userId = @"^\d+$" });

            routes.MapHttpRoute(
               "GetLogCountById",
               "user/{userId}/log-count",
               new { controller = "Log", action = "GetLogCountById" },
               new { userId = @"^\d+$" });

            routes.MapHttpRoute(
               "GetSysLog",
               "syslog",
               new { controller = "SysLog", action = "GetSysLog" });

            routes.MapHttpRoute(
               "GetSysLogCount",
               "syslog-count",
               new { controller = "SysLog", action = "GetSysLogCount" });
        }
    }
}