namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sms
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {          
            // 告警通知用户管理
            routes.MapHttpRoute(
                name: "GetList",
                routeTemplate: "sms-user/list/{userId}",
                defaults: new { controller = "SmsUser", action = "GetList" });            

            routes.MapHttpRoute(
                name: "AddSmsUser",
                routeTemplate: "sms-user/add",
                defaults: new { controller = "SmsUser", action = "Add" });

            routes.MapHttpRoute(
                name: "RemoveSmsUser",
                routeTemplate: "sms-user/remove/{receiverId}",
                defaults: new { controller = "SmsUser", action = "Remove" },
                constraints: new { receiverId = @"^\d+$" });
        }
    }
}