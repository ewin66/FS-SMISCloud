namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Aggregation
{
    using System;
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 获取某结构物下配置的聚集条件
            routes.MapHttpRoute(
                name: "GetStructureAggConfig",
                routeTemplate: "struct/{structId}/config/aggconfigs",
                defaults: new { controller = "Aggregation", action = "GetStructureAggConfig" },
                constraints: new { structId = @"^\d+$" });

            // 修改某结构物下配置的聚集条件
            routes.MapHttpRoute(
                name: "UpdateStructureAggConfig",
                routeTemplate: "struct/config/aggconfigs/update/{configId}",
                defaults: new { controller = "Aggregation", action = "UpdateStructureAggConfig" },
                constraints: new { configId = @"^\d+$" });

            // 修改某结构物下配置的聚集条件
            routes.MapHttpRoute(
                name: "DeleteStructureAggConfig",
                routeTemplate: "struct/config/aggconfigs/{configId}/delete",
                defaults: new { controller = "Aggregation", action = "DeleteStructureAggConfig" },
                constraints: new { configId = @"^\d+$" });

            // 增加某结构物的聚集条件配置
            routes.MapHttpRoute(
                name: "AddStructureAggConfig",
                routeTemplate: "struct/config/aggconfigs/add",
                defaults: new { controller = "Aggregation", action = "AddStructureAggConfig" });
        }
    }
}