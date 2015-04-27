namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.HotSpot
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "GetHotSpotInfoByStruct",
                routeTemplate: "struct/{structId}/hotspots",
                defaults: new { controller = "HotSpot", action = "GetHotSpotInfoByStruct" },
                constraints: new { structId = @"^\d+$" });
            //网壳热点
            routes.MapHttpRoute(
                name: "GetHotSpotInfoByStructRShell",
                routeTemplate: "struct/{structId}/hotspotsRShell",
                defaults: new { controller = "HotSpot", action = "GetHotSpotInfoByStructRShell" },
                constraints: new { structId = @"^\d+$" });
            //网壳结构物类型
            routes.MapHttpRoute(
                name: "GetStructTypeRShell",
                routeTemplate: "structTypeRShell/{structId}",
                defaults: new { controller = "HotSpot", action = "GetStructTypeRShell" },
                constraints: new { structId = @"^\d+$" });

            //网壳结构物热点数据
            routes.MapHttpRoute(
                name: "GetRShellHotspotData",
                routeTemplate: "structRShellHotspot/{sensorId}/{factorId}/data",
                defaults: new { controller = "HotSpot", action = "GetRShellHotspotData" },
                constraints: new { sensorId = @"^\d+$", factorId = @"^\d+$" }
                );

            routes.MapHttpRoute(
                name: "GetRealTimeHotSpotInfoByStruct",
                routeTemplate: "struct/{structId}/rt-sensors",
                defaults: new { controller = "HotSpot", action = "GetRealTimeHotSpotInfoByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "GetHotSpotConfigByStruct",
                routeTemplate: "struct/{structId}/hotspot-config",
                defaults: new { controller = "HotSpot", action = "GetHotSpotConfigByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "GetHotSpotNonConfigByStruct",
                routeTemplate: "struct/{structId}/non-hotspot",
                defaults: new { controller = "HotSpot", action = "GetHotSpotNonConfigByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 获取未配置热点的传感器(根据传感器类型分组)
            routes.MapHttpRoute(
                name: "GetProductHotspotNonConfigByStruct",
                routeTemplate: "struct/{structId}/product/non-hotspot",
                defaults: new { controller = "HotSpot", action = "GetProductHotspotNonConfigByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "AddHotSpotConfig",
                routeTemplate: "hotspot-config/add",
                defaults: new { controller = "HotSpot", action = "AddHotSpotConfig" });

            // 修改结构物或施工截面的传感器热点配置
            routes.MapHttpRoute(
                name: "ModifyHotSpotConfig",
                routeTemplate: "hotspot-config/{hotspotId}/modify",
                defaults: new { controller = "HotSpot", action = "ModifyHotSpotConfig" },
                constraints: new { hotspotId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "RemoveHotspotConfig",
                routeTemplate: "hotspot-config/remove/{hotspots}",
                defaults: new { controller = "HotSpot", action = "RemoveHotspotConfig" },
                constraints: new { hotspots = @"^\d+(,\d+)*$" });


            //获取施工线路信息--许凤琴
            routes.MapHttpRoute(
                    name: "GetScheduleConfig",
                    routeTemplate: "struct/{structId}/constructInfo/list",
                    defaults: new { controller = "Progress", action = "GetScheduleConfig" },
                    constraints: new { structId = @"^\d+$" });
            //新增施工线路
            routes.MapHttpRoute(
                name: "AddScheduleConfig",
                routeTemplate: "struct/{structId}/constructLine/add",
                defaults: new { controller = "Progress", action = "AddScheduleConfig" },
                constraints: new { structId = @"^\d+$" });

            //获取施工进度的记录

            routes.MapHttpRoute(
                   name: "GetProgressConfig",
                   routeTemplate: "struct/{lineId}/progressInfo/list",
                   defaults: new { controller = "Progress", action = "GetProgressConfig" },
                   constraints: new { lineId = @"^\d+$" });

         
            //修改线路配置信息
            routes.MapHttpRoute(
                name: "ModifyScheduleConfig",
                routeTemplate: "constructLine/modify/{LineId}",
                defaults: new { controller = "Progress", action = "ModifyScheduleConfig" },
                constraints: new { LineId = @"^\d+$" });

            //删除线路配置信息
            routes.MapHttpRoute(
                name: "RemoveScheduleConfig",
                routeTemplate: "constructLine/remove/{LineId}",
                defaults: new { controller = "Progress", action = "RemoveScheduleConfig" },
                constraints: new { LineId = @"^\d+$" });

            //新增进度信息
            routes.MapHttpRoute(
                name: "addProgressConfig",
                routeTemplate: "struct/{lineId}/progress/add",
                defaults: new { controller = "Progress", action = "addProgressConfig" },
                constraints: new { lineId = @"^\d+$" });

            //获取当前进度的信息

            routes.MapHttpRoute(
                   name: "GetNowProgressConfig",
                   routeTemplate: "struct/{lineId}/{progressId}/progressInfo",
                   defaults: new { controller = "Progress", action = "GetNowProgressConfig" },
                   constraints: new { progressId = @"^\d+$" });

            //修改进度信息
            routes.MapHttpRoute(
                name: "ModifyProgress",
                routeTemplate: "progress/modify/{progressId}",
                defaults: new { controller = "Progress", action = "ModifyProgress" },
                constraints: new { progressId = @"^\d+$" });

            //删除进度信息
            routes.MapHttpRoute(
                name: "RemoveProgress",
                routeTemplate: "progress/remove/{progressId}",
                defaults: new { controller = "Progress", action = "RemoveProgress" },
                constraints: new { progressId = @"^\d+$" });

            
            // 获取施工截面的传感器热点配置
            routes.MapHttpRoute(
                name: "GetHotSpotConfigBySection",
                routeTemplate: "section/{sectionId}/hotspot-config",
                defaults: new { controller = "HotSpot", action = "GetHotSpotConfigBySection" },
                constraints: new { sectionId = @"^\d+$" });

            // 添加施工截面的传感器热点配置
            routes.MapHttpRoute(
                name: "AddHotSpotConfigBySection",
                routeTemplate: "section/{sectionId}/hotspot-config/add",
                defaults: new { controller = "HotSpot", action = "AddHotSpotConfigBySection" },
                constraints: new { sectionId = @"^\d+$" });

            // 添加结构物下施工截面以及截面的多个传感器热点配置
            routes.MapHttpRoute(
                name: "AddSectionAndHotSpotsConfig",
                routeTemplate: "struct/{structId}/section/hotspots-config/add",
                defaults: new { controller = "HotSpot", action = "AddSectionAndHotSpotsConfig" },
                constraints: new { structId = @"^\d+$" });

            // 删除施工截面的传感器热点配置
            routes.MapHttpRoute(
                name: "RemoveHotSpotConfigBySection",
                routeTemplate: "section/hotspot-config/{hotspotId}/remove",
                defaults: new { controller = "HotSpot", action = "RemoveHotSpotConfigBySection" },
                constraints: new { hotspotId = @"^\d+$" });

            // 获取施工截面的传感器热点信息
            routes.MapHttpRoute(
                name: "GetHotSpotInfoBySection",
                routeTemplate: "section/{sectionId}/hotspots",
                defaults: new { controller = "HotSpot", action = "GetHotSpotInfoBySection" },
                constraints: new { sectionId = @"^\d+$" });
        }
    }
}