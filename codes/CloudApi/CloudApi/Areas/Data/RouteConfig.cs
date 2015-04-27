namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data
{
    using System.Web.Http;
    using System.Web.Routing;

    /// <summary>
    /// DATA路由配置
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// DATA路由配置
        /// </summary>
        /// <param name="routes">路由集合</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "GetVibrationData",
                routeTemplate: "vibration/{structid}/{collectTime}/microsetsmic",
                defaults: new { controller = "VibrationData", action = "GetVibrationData" });
            //保存震动震源数据
            routes.MapHttpRoute(
                name: "SaveVibrationData",
                routeTemplate: "vibration/save/data",
                defaults: new { controller = "VibrationData", action = "SaveVibrationData" });
            // 总数据接口
            routes.MapHttpRoute(
                name: "GetDataBySensorAndDate",
                routeTemplate: "sensor/{sensors}/data/{startdate}/{enddate}/{interval}/{datename}",
                defaults: new { controller = "Data", action = "GetBySensorAndDate", interval = 1, datename = "second" },
                constraints: new { sensors = @"^\d+(,\d+)*$", interval = @"^\d+$", datename = @"^(year)|(month)|(day)|(hour)|(minute)|(second)$" });

            routes.MapHttpRoute(
                name: "GetGprsSensorData",
                routeTemplate: "gprs/sensor/{sensors}/data/{startdate}/{enddate}/{interval}/{datename}",
                defaults: new { controller = "Data", action = "GetGprsSensorData", interval = 1, datename = "second" },
                constraints: new { sensors = @"^\d+(,\d+)*$", interval = @"^\d+$", datename = @"^(year)|(month)|(day)|(hour)|(minute)|(second)$" });

            //获取传感器数据接口
            routes.MapHttpRoute(
                name: "GetSensorOriginalData",
                routeTemplate: "sensor/{sensors}/originaldata/{startdate}/{enddate}/{interval}/{datename}",
                defaults: new { controller = "Data", action = "GetSensorOriginalData", interval = 5, datename = "second" },
                constraints: new { sensors = @"^\d+(,\d+)*$", interval = @"^\d+$", datename = @"^(year)|(month)|(day)|(hour)|(minute)|(second)$" });

            // 获取传感器即时采集请求任务结果
            routes.MapHttpRoute(
                name: "GetSensorRealtimeRequest",
                routeTemplate: "dtu/{dtu}/sensor/{sensors}/realtime-request",
                defaults: new { controller = "Data", action = "GetSensorRealtimeRequest" },
                constraints: new { dtu = @"^\d+(,\d+)*$", sensors = @"^\d+(,\d+)*$" });

            // 获取传感器即时采集数据
            routes.MapHttpRoute(
                name: "GetSensorRealtimeData",
                routeTemplate: "messageId/{messageId}/sensor/realtime-data",
                defaults: new { controller = "Data", action = "GetSensorRealtimeData" },
                constraints: new { messageId = @"^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$" });

            routes.MapHttpRoute(
                name: "GetLastDataBySensor",
                routeTemplate: "sensor/{sensors}/last-data",
                defaults: new { controller = "Data", action = "GetLastBySensor" },
                constraints: new { sensors = @"^\d+(,\d+)*$" });

            // 深部位移数据
            routes.MapHttpRoute(
                name: "GetDeepDisplaceDataByGroupDirectAndDateGroupByDepth",
                routeTemplate: "deep-displace/{groupId}/data-by-depth/{direct}/{startdate}/{enddate}",
                defaults: new { controller = "DeepDisplaceData", action = "GetByGroupDirectAndDateGroupByDepth" },
                constraints: new { groupId = @"^\d+$", direct = @"^x|y|(xy)$" });

            routes.MapHttpRoute(
                name: "GetDeepDisplaceDataByGroupDirectAndDateGroupByDate",
                routeTemplate: "deep-displace/{groupId}/data-by-time/{direct}/{startdate}/{enddate}",
                defaults: new { controller = "DeepDisplaceData", action = "GetByGroupDirectAndDateGroupByTime" },
                constraints: new { groupId = @"^\d+$", direct = @"^x|y|(xy)$" });

            routes.MapHttpRoute(
                name: "GetLastDataByGroup",
                routeTemplate: "deep-displace/{groupId}/last-data/{direct}",
                defaults: new { controller = "DeepDisplaceData", action = "GetLastDataByGroup" },
                constraints: new { groupId = @"^\d+$", direct = @"^x|y|(xy)$" });

            // 深部位移日数据
            routes.MapHttpRoute(
                name: "GetDeepDisplaceDailyDataByStructDirectAndDateGroupByDepth",
                routeTemplate: "deep-displace/{groupId}/daily-data-by-depth/{direct}/{startdate}/{enddate}",
                defaults: new { controller = "DeepDisplaceDailyData", action = "GetByGroupDirectAndDateGroupByDepth" },
                constraints: new { groupId = @"^\d+$", direct = @"^x|y|(xy)$" });

            routes.MapHttpRoute(
                name: "GetDeepDisplaceDailyDataBySensorsDirectAndDateGroupByDate",
                routeTemplate: "deep-displace/{groupId}/daily-data-by-time/{direct}/{startdate}/{enddate}",
                defaults: new { controller = "DeepDisplaceDailyData", action = "GetByGroupDirectAndDateGroupByTime" },
                constraints: new { groupId = @"^\d+$", direct = @"^x|y|(xy)$" });

            // 沉降
            routes.MapHttpRoute(
                name: "GetSettleDailyDataByGroupAndDate",
                routeTemplate: "settle/{groupId}/daily-data/{startDate}/{endDate}",
                defaults: new { controller = "SettleGroupData" },
                constraints: new { groupId = @"^\d+$"  });
            
            //沉降日报表--许凤琴
            routes.MapHttpRoute(
               name: "GetSettleDailyDataByDate",
               routeTemplate: "settlement/{groupId}/daily-report/{startDate}/{endDate}/{algorithm}/info",
               defaults: new { controller = "SettleGroupData", action = "GetSettlementDailyReportByTime" },
               constraints: new { groupId = @"^\d+$" });

            // 风玫瑰图
            routes.MapHttpRoute(
                name: "GetWindStatDataBySensorAndDate",
                routeTemplate: "wind/{sensorId}/stat-data/{startDate}/{endDate}",
                defaults: new { controller = "WindData", action = "GetWindStatDataBySensorAndDate" },
                constraints: new { sensorId = @"^\d+$" });

            // 振动
            routes.MapHttpRoute(
                name: "GetVibrationDataBatch",
                routeTemplate: "vibration/{sensorId}/data-batch/{startDate}/{endDate}",
                defaults: new { controller = "VibrationData", action = "GetDataBatch" },
                constraints: new { sensorId = @"^\d+$" });

            // 振动时间列
            routes.MapHttpRoute(
                name: "GetVibrationCollectTime",
                routeTemplate: "vibration/{sensorId}/collecttime",
                defaults: new { controller = "VibrationData", action = "GetCollectTime" },
                constraints: new { sensorId = @"^\d+$" });
            
            //振批数据
            routes.MapHttpRoute(
                name: "GetVibrationOriginal",
                routeTemplate: "vibration/{collectTime}/{structId}/{factorId}/batchs",
                defaults: new { controller = "VibrationData", action = "GetOriginalByCollectTime" });

            // 振动-频域
            routes.MapHttpRoute(
                name: "GetVibrationSpectrumData",
                routeTemplate: "vibration/{batchId}/spectrum-data",
                defaults: new { controller = "VibrationData", action = "GetSpectrumData" });

            routes.MapHttpRoute(
                name: "GetVibrationRtSpectrumData",
                routeTemplate: "vibration/{sensorId}/rt-spectrum-data",
                defaults: new { controller = "VibrationData", action = "GetRtSpectrumData" },
                constraints: new { sensorId = @"^\d+$" });

            // 振动-时域
            routes.MapHttpRoute(
                name: "GetVibrationOriginalData",
                routeTemplate: "vibration/{batchId}/original-data",
                defaults: new { controller = "VibrationData", action = "GetOriginalData" });

            routes.MapHttpRoute(
                name: "GetVibrationRtOriginalData",
                routeTemplate: "vibration/{sensorId}/rt-original-data",
                defaults: new { controller = "VibrationData", action = "GetRtOriginalData" },
                constraints: new { sensorId = @"^\d+$" });

            // 网壳振动传感器组-触发时段
             routes.MapHttpRoute(
                name: "GetDataBatchRShell",
                routeTemplate: "vibrationRShell/{sensorId}/data-batch/{startDate}/{endDate}",
                defaults: new { controller = "VibrationDataRShell", action = "GetDataBatchRShell" },
                constraints: new { sensorId = @"^\d+$" });

            // 网壳振动-时域
            routes.MapHttpRoute(
                name: "GetOriginalDataRShell",
                routeTemplate: "vibration-RShell/{collectTime}/{sensorId}/original-data",
                defaults: new { controller = "VibrationDataRShell", action = "GetOriginalDataRShell" });

            routes.MapHttpRoute(
                name: "GetRtOriginalDataRShell",
                routeTemplate: "vibration-RShell/{sensorId}/rt-original-data",
                defaults: new { controller = "VibrationDataRShell", action = "GetRtOriginalDataRShell" },
                constraints: new { sensorId = @"^\d+$" });

            // 网壳振动- 触发时段
            routes.MapHttpRoute(
                name: "GetSpectrumDataRShell",
                routeTemplate: "vibration-RShell/{collectTime}/{sensorId}/spectrum-data",
                defaults: new { controller = "VibrationDataRShell", action = "GetSpectrumDataRShell" });

            routes.MapHttpRoute(
                name: "GetRtSpectrumDataRShell",
                routeTemplate: "vibration-RShell/{sensorId}/rt-spectrum-data",
                defaults: new { controller = "VibrationDataRShell", action = "GetRtSpectrumDataRShell" },
                constraints: new { sensorId = @"^\d+$" });

            // 干滩
            routes.MapHttpRoute(
                name: "GetBeachLastData",
                routeTemplate: "struct/{structId}/beach/last-data",
                defaults: new { controller = "BeachData", action = "GetLastData" },
                constraints: new { structId = @"^\d+$" });

            // 浸润线
            routes.MapHttpRoute(
                name: "GetSaturationLineData",
                routeTemplate: "saturation-line/{groupIds}/data/{startDate}/{endDate}",
                defaults: new { controller = "SaturationLine", action = "GetData" });

            routes.MapHttpRoute(
                name: "GetSaturationLineHeight",
                routeTemplate: "struct/{structId}/saturation-line/height/",
                defaults: new { controller = "SaturationLine", action = "GetHeight" },
                constraints: new { structId = @"^\d+$" });


            // 杆件应力、应变
            routes.MapHttpRoute(
                name: "GetCombinedSensors",
                routeTemplate: "combinedSensors/{sensorId}/info",
                defaults: new { controller = "StressStrainData", action = "GetCombinedSensors" });
        }
    }
}