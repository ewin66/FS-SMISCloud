namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Factor
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "FindFactorsByStruct",
                routeTemplate: "struct/{structId}/factors",
                defaults: new { controller = "Factor", action = "FindFactorsByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "FindFactorStatusByStruct",
                routeTemplate: "struct/{structs}/factor-status",
                defaults: new { controller = "Factor", action = "FindFactorStatusByStruct" },
                constraints: new { structs = @"^\d+(,\d+)*$" });

            routes.MapHttpRoute(
                name: "FindFactorsByStructType",
                routeTemplate: "struct/type/{structTypeId}/factors",
                defaults: new { controller = "Factor", action = "FindFactorsByStructType" },
                constraints: new { structTypeId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "FindFactorsConfigByStruct",
                routeTemplate: "struct/{structId}/factor-config",
                defaults: new { controller = "Factor", action = "FindFactorsConfigByStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "ConfigStructFactors",
                routeTemplate: "struct/{structId}/factor/modify",
                defaults: new { controller = "Factor", action = "ConfigStructFactors" },
                constraints: new { structId = @"^\d+$" });


            //获取关联的监测因素
            routes.MapHttpRoute(
                name: "CorrelationFactors",
                routeTemplate: "struct/{structId}/factor/{factorId}/correlation",
                defaults: new {controller = "Correlation", action = "CorrelationFactors"},
                constraints: new {structId = @"^\d+$", factorId = @"^\d+$"});

            //获取监测因素对应的产品列表
            routes.MapHttpRoute(
                name: "GetFactorCorrelateProduct",
                routeTemplate: "factor/{factorId}/correlate-product-type/info",
                defaults: new { controller = "Correlation", action = "GetFactorCorrelateProduct" },
                constraints: new { factorId = @"^\d+$" });

            //获取组合传感器对应的关联传感器列表
            routes.MapHttpRoute(
                name: "GetCombinedSensorList",
                routeTemplate: "combined-sensor/{structId}/{productTypeId}/sensorList/info",
                defaults: new { controller = "Correlation", action = "GetCombinedSensorList" },
                constraints: new { structId = @"^\d+$" });

            //获取监测因素下可配置的单位列表
            routes.MapHttpRoute(
                name: "GetFactorUnitList",
                routeTemplate: "factor/{structId}/unitList/info",
                defaults: new { controller = "Correlation", action = "GetFactorUnitList" },
                constraints: new { structId = @"^\d+$"});
            
            //配置监测因素的展示单位
            routes.MapHttpRoute(
                name: "AddFactorUnit",
                routeTemplate: "factor/{structId}/unit/add",
                defaults: new { controller = "Correlation", action = "AddFactorUnit" },
                constraints: new { structId = @"^\d+$" });

            //获取单个监测因素已配置的单位
            routes.MapHttpRoute(
                name: "GetSubFactorUnit",
                routeTemplate: "factor/{structId}/{factorId}/{valueIndex}/unit/info",
                defaults: new { controller = "Correlation", action = "GetSubFactorUnit" },
                constraints: new { structId = @"^\d+$", factorId = @"^\d+$", valueIndex = @"^\d+$" });

            //获取单个结构物已配置的单位个数
            routes.MapHttpRoute(
                name: "GetUnitCount",
                routeTemplate: "factor/{structId}/unit/count",
                defaults: new { controller = "Correlation", action = "GetUnitCount" },
                constraints: new { structId = @"^\d+$" });
        }
    }
}