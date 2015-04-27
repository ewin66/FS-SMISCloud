namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Dashboard
{
    using System;
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 统计用户下项目状态
            routes.MapHttpRoute(
                name: "GetProjectsStatusStatisticsByUser",
                routeTemplate: "statistics/user/{userId}/projects/status/{startTime}/{endTime}",
                defaults: new { controller = "Dashboard", action = "GetProjectsStatusStatisticsByUser" },
                constraints: new { userId = @"^\d+$" });

            // 统计用户项目下结构物状态
            routes.MapHttpRoute(
                name: "GetStructsStatusStatisticsByProject",
                routeTemplate: "statistics/user/{userId}/project/{projectId}/structs/status/{status}/{startTime}/{endTime}",
                defaults: new { controller = "Dashboard", action = "GetStructsStatusStatisticsByProject" },
                constraints: new { userId = @"^\d+$", projectId = @"^\d+$", status = @"^(abnormal)|(normal)|(all)$" });

            // 统计结构物下告警、DTU、传感器状态
            routes.MapHttpRoute(
                name: "GetAlarmsDtusSensorsStatusStatisticsByStruct",
                routeTemplate: "statistics/struct/{structId}/status/{startTime}/{endTime}",
                defaults: new { controller = "Dashboard", action = "GetAlarmsDtusSensorsStatusStatisticsByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 统计结构物下各等级未确认告警数
            routes.MapHttpRoute(
                name: "GetEachAlarmCountStatisticsByStruct",
                routeTemplate: "statistics/struct/{structId}/alarm-count/{status}/{startTime}/{endTime}",
                defaults: new { controller = "Dashboard", action = "GetEachAlarmCountStatisticsByStruct", startTime = DateTime.Now.AddHours(-24), endTime = DateTime.Now },
                constraints: new { structId = @"^\d+$", status = @"^(unprocessed)|(processed)|(all)$" });

            // 获取结构物下DTU当前状态
            routes.MapHttpRoute(
                name: "GetDtusLatestStatusByStruct",
                routeTemplate: "struct/{structId}/dtus/{status}",
                defaults: new { controller = "Dashboard", action = "GetDtusLatestStatusByStruct" },
                constraints: new { structId = @"^\d+$", status = @"^(offline)|(neverUpline)|(online)|(all)$" });

            // 获取DTU最新状态
            routes.MapHttpRoute(
                name: "GetDtuLatestStatus",
                routeTemplate: "dtu/{dtuId}/status",
                defaults: new { controller = "Dashboard", action = "GetDtuLatestStatus" },
                constraints: new { dtuId = @"^\d+$" });

            // 获取DTU历史状态
            routes.MapHttpRoute(
                name: "GetDtuHistoryStatus",
                routeTemplate: "dtu/{dtuId}/history-status/{startTime}/{endTime}",
                defaults: new { controller = "Dashboard", action = "GetDtuHistoryStatus" },
                constraints: new { dtuId = @"^\d+$" });

            // 获取DTU基本信息
            routes.MapHttpRoute(
                name: "GetDtuDetails",
                routeTemplate: "dtu/{dtuId}/details",
                defaults: new { controller = "Dashboard", action = "GetDtuDetails" },
                constraints: new { dtuId = @"^\d+$" });

            // 获取指定结构物的指定DTU下传感器状态
            routes.MapHttpRoute(
                name: "GetSensorsStatusByStructAndDtu",
                routeTemplate: "struct/{structId}/dtu/{dtuId}/sensors/status",
                defaults: new { controller = "Dashboard", action = "GetSensorsStatusByStructAndDtu" },
                constraints: new { dtuId = @"^\d+$" });

            // 统计结构物下传感器状态
            routes.MapHttpRoute(
                name: "GetSensorsStatusStatisticsByStruct",
                routeTemplate: "statistics/struct/{structId}/sensors/status",
                defaults: new { controller = "Dashboard", action = "GetSensorsStatusStatisticsByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 获取传感器最新状态--修改xu
            routes.MapHttpRoute(
                name: "GetSensorLatestStatus",
                routeTemplate: "sensor/{sensorId}/{structId}/status",
                defaults: new { controller = "Dashboard", action = "GetSensorLatestStatus" },
                constraints: new { sensorId = @"^\d+$" });


            //获取未知状态的传感器--xu
            routes.MapHttpRoute(
                name: "GetUnknowSensor",
                routeTemplate: "statistics/struct/{structId}/{sensorId}/sensors",
                defaults: new { controller = "Dashboard", action = "GetUnknowSensor" },
                constraints: new { structId = @"^\d+$" });

            //获取禁用的传感器信息
            routes.MapHttpRoute(
               name: "GetUnableSensor",
               routeTemplate: "statistics/struct/unable/{structId}/sensors",
               defaults: new { controller = "Dashboard", action = "GetUnableSensor" },
               constraints: new { structId = @"^\d+$" });

            // 统计传感器历史异常次数
            routes.MapHttpRoute(
                name: "GetSensorHistoryAbnormalCountStatistics",
                routeTemplate: "statistics/sensor/{sensorId}/abnormal/{startTime}/{endTime}",
                defaults: new { controller = "Dashboard", action = "GetSensorHistoryAbnormalCountStatistics" },
                constraints: new { sensorId = @"^\d+$" });

            // 获取传感器指定告警类型下历史状态
            routes.MapHttpRoute(
                name: "GetSensorHistoryStatusByAlarmType",
                routeTemplate: "sensor/{sensorId}/alarm-type/{alarmTypeId}/history-status/{startTime}/{endTime}",
                defaults: new { controller = "Dashboard", action = "GetSensorHistoryStatusByAlarmType" },
                constraints: new { sensorId = @"^\d+$" });

            // 获取用户下组织结构物列表
            routes.MapHttpRoute(
                name: "GetOrgStructsListByUser",
                routeTemplate: "user/{userId}/org-structs/list",
                defaults: new { controller = "Dashboard", action = "GetOrgStructsListByUser" },
                constraints: new { userId = @"^\d+$" });
        }
    }
}