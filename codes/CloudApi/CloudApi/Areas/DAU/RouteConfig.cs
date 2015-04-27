namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.DAU
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {        
            // DTU产品
            routes.MapHttpRoute(
                name: "GetDtuProducts",
                routeTemplate: "dtu/product",
                defaults: new { controller = "DtuProduct", action = "GetDtuProducts" });

            // DTU
            routes.MapHttpRoute(
                name: "GetDTU",
                routeTemplate: "struct/{structId}/dtu",
                defaults: new { controller = "Dtu", action = "GetDtu" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "GetDtuInfo",
                routeTemplate: "dtu/{dtuId}/info",
                defaults: new { controller = "Dtu", action = "GetDtuInfo" },
                constraints: new { dtuId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "GetOrgDTU",
                routeTemplate: "struct/{structId}/org-dtu",
                defaults: new { controller = "Dtu", action = "GetOrgDtu" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "AddDTU",
                routeTemplate: "dtu/add",
                defaults: new { controller = "Dtu", action = "Add" });

            routes.MapHttpRoute(
                name: "AddDTUMap",
                routeTemplate: "dtu-map/add/{dtuId}/{structId}",
                defaults: new { controller = "Dtu", action = "AddMap" });

            routes.MapHttpRoute(
                name: "ModifyDTU",
                routeTemplate: "dtu/modify/{dtuId}",
                defaults: new { controller = "Dtu", action = "Modify" },
                constraints: new { dtuId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "RemoveDTU",
                routeTemplate: "dtu/remove/{dtuId}/{structId}",
                defaults: new { controller = "Dtu", action = "Remove" },
                constraints: new { dtuId = @"^\d+$", structId = @"^\d+$" });

            // 获取DTU远程配置
            routes.MapHttpRoute(
                name: "GetDtuRometeConfig",
                routeTemplate: "dtu/{dtuId}/remote-config",
                defaults: new { controller = "Dtu", action = "GetDtuRometeConfig" },
                constraints: new { dtuId = @"^\d+$" });

            // 修改DTU远程配置请求
            routes.MapHttpRoute(
                name: "ModifyDtuRemoteConfigRequest",
                routeTemplate: "dtu/{dtuId}/remote-config/modify-request",
                defaults: new { controller = "Dtu", action = "ModifyDtuRemoteConfigRequest" },
                constraints: new { dtuId = @"^\d+$" });

            // 修改DTU远程配置
            routes.MapHttpRoute(
                name: "ModifyDtuRemoteConfig",
                routeTemplate: "messageId/{messageId}/dtu/{dtuId}/remote-config/modify",
                defaults: new { controller = "Dtu", action = "ModifyDtuRemoteConfig" },
                constraints: new { messageId = @"^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", dtuId = @"^\d+$" });

            // 获取DTU重启请求
            routes.MapHttpRoute(
                name: "GetDtuRestartRequest",
                routeTemplate: "dtu/{dtuId}/restart-request",
                defaults: new { controller = "Dtu", action = "GetDtuRestartRequest" },
                constraints: new { dtuId = @"^\d+$" });

            // 获取DTU重启结果
            routes.MapHttpRoute(
                name: "GetDtuRestartResult",
                routeTemplate: "messageId/{messageId}/dtu/restart-result",
                defaults: new { controller = "Dtu", action = "GetDtuRestartResult" },
                constraints: new { messageId = @"^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$" });
        }
    }
}