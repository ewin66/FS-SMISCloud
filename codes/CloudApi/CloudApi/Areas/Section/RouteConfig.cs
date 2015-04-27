namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Section
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 获取结构物的施工截面
            routes.MapHttpRoute(
                name: "GetSectionByStruct",
                routeTemplate: "struct/{structId}/sections",
                defaults: new { controller = "Section", action = "GetSectionByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 获取单个施工截面信息
            routes.MapHttpRoute(
                name: "GetSection",
                routeTemplate: "section/{sectionId}/info",
                defaults: new { controller = "Section", action = "GetSection" },
                constraints: new { sectionId = @"^\d+$" });

            // 新增结构物的施工截面
            routes.MapHttpRoute(
                name: "AddSection",
                routeTemplate: "struct/{structId}/section/add",
                defaults: new { controller = "Section", action = "AddSection" },
                constraints: new { structId = @"^\d+$" });

            // 修改施工截面
            routes.MapHttpRoute(
                name: "ModifySection",
                routeTemplate: "section/{sectionId}/modify",
                defaults: new { controller = "Section", action = "ModifySection" },
                constraints: new { sectionId = @"^\d+$" });

            // 删除施工截面
            routes.MapHttpRoute(
                name: "RemoveSection",
                routeTemplate: "section/{sectionId}/remove",
                defaults: new { controller = "Section", action = "RemoveSection" },
                constraints: new { sectionId = @"^\d+$" });

            // 获取结构物下已配置的施工截面热点
            routes.MapHttpRoute(
                name: "GetSectionHotSpotConfigByStruct",
                routeTemplate: "struct/{structId}/hotspot-config/sections",
                defaults: new { controller = "Section", action = "GetSectionHotSpotConfigByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 获取结构物下未配置的施工截面热点
            routes.MapHttpRoute(
                name: "GetSectionHotSpotNonConfigByStruct",
                routeTemplate: "struct/{structId}/non-hotspot/sections",
                defaults: new { controller = "Section", action = "GetSectionHotSpotNonConfigByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 添加结构物的施工截面热点配置
            routes.MapHttpRoute(
                name: "AddSectionHotSpotConfig",
                routeTemplate: "hotspot-config/section/add",
                defaults: new { controller = "Section", action = "AddSectionHotSpotConfig" });

            // 修改结构物的施工截面热点配置
            routes.MapHttpRoute(
                name: "ModifySectionHotSpotConfig",
                routeTemplate: "hotspot-config/{hotspotId}/section/modify",
                defaults: new { controller = "Section", action = "ModifySectionHotSpotConfig" },
                constraints: new { hotspotId = @"^\d+$" });

            // 删除结构物的施工截面热点配置
            routes.MapHttpRoute(
                name: "RemoveSectionHotspotConfig",
                routeTemplate: "hotspot-config/{hotspots}/section/remove",
                defaults: new { controller = "Section", action = "RemoveSectionHotspotConfig" },
                constraints: new { hotspots = @"^\d+(,\d+)*$" });
        }
    }
}