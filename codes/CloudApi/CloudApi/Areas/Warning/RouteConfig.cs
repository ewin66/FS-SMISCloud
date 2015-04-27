namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Warning
{
    using System;
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 告警数量
            routes.MapHttpRoute(
                "GetWarningCountByUser",
                "user/{userId}/warning-count/{status}/{startDate}/{endDate}",
                new { controller = "Warning", action = "GetWarningCountByUser", startDate = DateTime.Now.AddYears(-10), endDate = DateTime.Now.AddYears(10) },
                new { userId = @"^\d+$", status = @"^(all)|(processed)|(unprocessed)$" });

            routes.MapHttpRoute(
               "GetWarningCountByStruct",
               "struct/{structs}/warning-count/{status}/{startDate}/{endDate}",
               new { controller = "Warning", action = "GetWarningCountByStruct", startDate = DateTime.Now.AddYears(-10), endDate = DateTime.Now.AddYears(10) },
               new { structs = @"^\d+(,\d+)*$", status = @"^(all)|(processed)|(unprocessed)$" });

            routes.MapHttpRoute(
                "GetWarningCountBySensor",
                "sensor/{sensors}/warning-count/{status}/{startDate}/{endDate}",
                new { controller = "Warning", action = "GetWarningCountBySensor", startDate = DateTime.Now.AddYears(-10), endDate = DateTime.Now.AddYears(10) },
                new { sensors = @"^\d+(,\d+)*$", status = @"^(all)|(processed)|(unprocessed)$" });
            routes.MapHttpRoute(
               name: "FindFilteredOrderedAlarmsByStructCount",
               routeTemplate: "struct/{structId}/filtered-ordered/alarms-count",
               defaults: new { controller = "Warning", action = "FindFilteredOrderedAlarmsByStructCount" },
               constraints: new { structId = @"^\d+$" }); 

            // 告警内容
            routes.MapHttpRoute(
                "FindWarningsByUser", 
                "user/{userId}/warnings/{status}/{startDate}/{endDate}",
                new { controller = "Warning", action = "FindWarningsByUser", startDate = DateTime.Now.AddYears(-10), endDate = DateTime.Now.AddYears(10) },
                new { userId = @"^\d+$", status = @"^(all)|(processed)|(unprocessed)$" });

            // 获取结构物下"全部/已确认/未确认/已下发"告警内容
            routes.MapHttpRoute(
                "FindWarningsByStruct",
                "struct/{structs}/warnings/{status}/{startDate}/{endDate}",
                new { controller = "Warning", action = "FindWarningsByStruct", startDate = DateTime.Now.AddYears(-10), endDate = DateTime.Now.AddYears(10) },
                new { structs = @"^\d+(,\d+)*$", status = @"^(all)|(processed)|(unprocessed)|(issued)$" });

            // 获取结构物下过滤及排序后的告警内容
            routes.MapHttpRoute(
                name: "FindFilteredOrderedAlarmsByStruct",
                routeTemplate: "struct/{structId}/filtered-ordered/alarms",
                defaults: new { controller = "Warning", action = "FindFilteredOrderedAlarmsByStruct" },
                constraints: new { structId = @"^\d+$" }); 
            
            routes.MapHttpRoute(
                "FindWarningsBySensor",
                "sensor/{sensors}/warnings/{status}/{startDate}/{endDate}",
                new { controller = "Warning", action = "FindWarningsBySensor", startDate = DateTime.Now.AddYears(-10), endDate = DateTime.Now.AddYears(10) },
                new { sensors = @"^\d+(,\d+)*$", status = @"^(all)|(processed)|(unprocessed)$" });

            // 告警统计
            routes.MapHttpRoute(
                "GetWarningStatus",
                "struct/{structs}/warn-number/{status}/{startDate}/{endDate}",
                new { controller = "Warning", action = "GetWarningStatus", startDate = DateTime.Now.AddYears(-10), endDate = DateTime.Now.AddYears(10) },
                new { structs = @"^\d+(,\d+)*$" });

            // 告警下发
            routes.MapHttpRoute(
                "DistributeWarnings",
                "warnings/distribute/{warnIds}",
                new { controller = "Warning", action = "DistributeWarnings" },
                new { warnids = @"^\d+(,\d+)*$" });

            // 确认告警
            routes.MapHttpRoute(
                "ConfirmWarnings",
                "warnings/confirm/{warnIds}",
                new { controller = "Warning", action = "ConfirmWarnings" },
                new { warnids = @"^\d+(,\d+)*$" });
        }
    }
}