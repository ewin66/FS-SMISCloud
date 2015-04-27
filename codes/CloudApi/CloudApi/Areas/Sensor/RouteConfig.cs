namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "VibrationCalc",
                routeTemplate: "vibration/calc/result",
                defaults: new { controller = "Vibration", action = "VibrationCalc" });

            routes.MapHttpRoute(
                name: "FindDeepDisplaceSensorsByStruct",
                routeTemplate: "struct/{structId}/factor/deep-displace/groups",
                defaults: new { controller = "FactorSensor", action = "FindDeepDisplaceSensorsByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "FindSensorsByStructAndFactor",
                routeTemplate: "struct/{structId}/factor/{factorId}/sensors",
                defaults: new { controller = "FactorSensor", action = "FindSensorsByStructAndFactor" },
                constraints: new { structId = @"^\d+$", factorId = @"^\d+$" });
            routes.MapHttpRoute(
                name: "GetSensorInfoByStruct",
                routeTemplate: "sensors/{structId}/info/{factorId}/list",
                defaults: new { controller = "Sensor", action = "GetSensorInfoByStruct" },
                constraints: new { structId = @"^\d+$", factorId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "FindGroupsByStructAndFactor",
                routeTemplate: "struct/{structId}/factor/{factorId}/groups",
                defaults: new { controller = "FactorSensor", action = "FindGroupsByStructAndFactor" },
                constraints: new { structId = @"^\d+$", factorId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "FindSensorsByGroup",
                routeTemplate: "group/{groupId}/sensors",
                defaults: new { controller = "FactorSensor", action = "FindSensorsByGroup" },
                constraints: new { groupId = @"^\d+$" });

            // 配置
            routes.MapHttpRoute(
                name: "FindSensorsByStruct",
                routeTemplate: "struct/{structId}/sensors",
                defaults: new { controller = "Sensor", action = "FindSensorsByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 获取结构物下所有DTU及DTU下所有非虚拟(实体/数据)传感器列表
            routes.MapHttpRoute(
                name: "FindDtuNonVirtualSensorsListByStruct",
                routeTemplate: "struct/{structId}/dtu-sensors/list/non-virtual",
                defaults: new { controller = "Sensor", action = "FindDtuNonVirtualSensorsListByStruct" },
                constraints: new { structId = @"^\d+$" });

            // 获取结构物下所有非虚拟(实体/数据)传感器列表
            routes.MapHttpRoute(
                name: "FindNonVirtualSensorsByStruct",
                routeTemplate: "struct/{structId}/non-virtual/sensors",
                defaults: new { controller = "Sensor", action = "FindNonVirtualSensorsByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
               name: "FindNonVirtualSensorsByStructAndFactor",
               routeTemplate: "struct/{structId}/factor/{factorId}/non-virtual/sensors",
               defaults: new { controller = "FactorSensor", action = "FindNonVirtualSensorsByStructAndFactor" },
               constraints: new { structId = @"^\d+$", factorId = @"^\d+$" });

            // 获取结构物下的传感器类型
            routes.MapHttpRoute(
                name: "FindSensorTypeByStruct",
                routeTemplate: "struct/{structId}/sensorType",
                defaults: new { controller = "Sensor", action = "FindSensorTypeByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "GetSensorProductList",
                routeTemplate: "sensor/product/list",
                defaults: new { controller = "SensorProduct", action = "GetSensorProductList" });

            routes.MapHttpRoute(
                name: "GetSensorProductInfo",
                routeTemplate: "sensor/product/{productId}",
                defaults: new { controller = "SensorProduct", action = "GetSensorProductInfo" },
                constraints: new { productId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "AddSensor",
                routeTemplate: "sensor/add",
                defaults: new { controller = "Sensor", action = "AddSensor" });

            routes.MapHttpRoute(
                name: "GetSensorInfo",
                routeTemplate: "sensor/{sensorId}/info",
                defaults: new { controller = "Sensor", action = "GetSensorInfo" },
                constraints: new { sensorId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "ModifySensor",
                routeTemplate: "sensor/modify/{sensorId}",
                defaults: new { controller = "Sensor", action = "ModifySensor" },
                constraints: new { sensorId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "RemoveSensor",
                routeTemplate: "sensor/remove/{sensorId}",
                defaults: new { controller = "Sensor", action = "RemoveSensor" },
                constraints: new { sensorId = @"^\d+$" });

            ////获取关联传感器--许凤琴
            routes.MapHttpRoute(
                name: "GetCorrentSensorInfo",
                routeTemplate: "correntSensor/{sensorId}/info",
                defaults: new { controller = "Sensor", action = "GetCorrentSensorInfo" },
                constraints: new { sensorId = @"^\d+$" });

            // 阈值
            routes.MapHttpRoute(
                name: "FindThresholdBySensor",
                routeTemplate: "sensor/{sensorId}/threshold",
                defaults: new { controller = "Threshold", action = "FindThresholdBySensor" },
                constraints: new { sensorId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "FindThresholdByStructAndFactor",
                routeTemplate: "struct/{structId}/factor/{factorId}/threshold",
                defaults: new { controller = "Threshold", action = "FindThresholdByStructAndFactor" },
                constraints: new { structId = @"^\d+$", factorId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "ConfigSensorThreshold",
                routeTemplate: "sensor/threshold/config",
                defaults: new { controller = "Threshold", action = "ConfigSensorThreshold" });
            
            routes.MapHttpRoute(
                name: "ConfigAllSensorThreshold",
                routeTemplate: "factor/threshold/config",
                defaults: new { controller = "Threshold", action = "ConfigAllSensorThreshold" });

            //传感器分组
            routes.MapHttpRoute(
                name: "FindGroupType",
                routeTemplate: "struct/{structId}/sensor-group/type",
                defaults: new { controller = "SensorGroup", action = "FindGroupType" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
               name: "FindGroupsCeXieByStruct",
               routeTemplate: "struct/{structId}/sensor-group/cexie",
               defaults: new { controller = "SensorGroup", action = "FindGroupsCeXieByStruct" },
               constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
              name: "FindGroupsChenJiangByStruct",
              routeTemplate: "struct/{structId}/sensor-group/chenjiang",
              defaults: new { controller = "SensorGroup", action = "FindGroupsChenJiangByStruct" },
              constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
              name: "FindGroupsJinRunXianByStruct",
              routeTemplate: "struct/{structId}/sensor-group/jinrunxian",
              defaults: new { controller = "SensorGroup", action = "FindGroupsJinRunXianByStruct" },
              constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
               name: "RemoveGroup",
               routeTemplate: "sensor-group/remove/{groupId}",
               defaults: new { controller = "SensorGroup", action = "RemoveGroup" },
               constraints: new { groupId = @"^\d+$" });

            // 测斜
            routes.MapHttpRoute(
              name: "AddGroupsCeXie",
              routeTemplate: "sensor-group/cexie/add",
              defaults: new { controller = "SensorGroup", action = "AddGroupsCeXie" });            

            routes.MapHttpRoute(
               name: "ModifyGroupCeXie",
               routeTemplate: "sensor-group/cexie/modify/{groupId}",
               defaults: new { controller = "SensorGroup", action = "ModifyGroupCeXie" },
               constraints: new { groupId = @"^\d+$" });

            // 沉降
            routes.MapHttpRoute(
              name: "AddGroupsChenJiang",
              routeTemplate: "sensor-group/settle/add",
              defaults: new { controller = "SensorGroup", action = "AddGroupsChenJiang" });

            routes.MapHttpRoute(
               name: "ModifyGroupChenJiang",
               routeTemplate: "sensor-group/settle/modify/{groupId}",
               defaults: new { controller = "SensorGroup", action = "ModifyGroupChenJiang" },
               constraints: new { groupId = @"^\d+$" });

            // 浸润线
            routes.MapHttpRoute(
              name: "AddGroupsJinRunXian",
              routeTemplate: "sensor-group/saturation-line/add",
              defaults: new { controller = "SensorGroup", action = "AddGroupsJinRunXian" });

            routes.MapHttpRoute(
               name: "ModifyGroupJinRunXian",
               routeTemplate: "sensor-group/saturation-line/modify/{groupId}",
               defaults: new { controller = "SensorGroup", action = "ModifyGroupJinRunXian" },
               constraints: new { groupId = @"^\d+$" });

            //传感器过滤配置
            routes.MapHttpRoute(
               name: "FindFilterConfigByStructAndFactor",
               routeTemplate: "struct/{structId}/factor/{factorId}/data-validate",
               defaults: new { controller = "SensorFilter", action = "FindFilterConfigByStructAndFactor" },
               constraints: new { structId = @"^\d+$", factorId = @"^\d+$" });
            //过滤配置
            routes.MapHttpRoute(
              name: "RegisterFilterInfo",
              routeTemplate: "sensor/data-validate/config",
              defaults: new { controller = "SensorFilter", action = "RegisterFilterInfo" });
        }
    }
}