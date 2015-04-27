namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Weight
{
    using System;
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                "GetWeightProgress",
                "org/{orgId}/struct/{structId}/weight/progress",
                new { controller = "Weight", action = "GetWeightProgress" },
                new { orgId = @"^\d+$", structId = @"^\d+$" });

            routes.MapHttpRoute(
                "GetFactorWeight",
                "org/{orgId}/struct/{structId}/factor/weight",
                new { controller = "Weight", action = "GetFactorWeight" },
                new { orgId = @"^\d+$", structId = @"^\d+$" });

            routes.MapHttpRoute(
                "AddFactorWeight",
                "factor/weight/add",
                new { controller = "Weight", action = "AddFactorWeight" });

            routes.MapHttpRoute(
                "GetSubFactorWeight",
                "org/{orgId}/struct/{structId}/factor/{factorId}/sub-factor/weight",
                new { controller = "Weight", action = "GetSubFactorWeight" },
                new { orgId = @"^\d+$", structId = @"^\d+$", factorId = @"^\d+$" });

            routes.MapHttpRoute(
                "AddSubFactorWeight",
                "sub-factor/weight/add",
                new { controller = "Weight", action = "AddSubFactorWeight" });

            routes.MapHttpRoute(
                "GetSensorWeight",
                "org/{orgId}/struct/{structId}/sub-factor/{factorId}/sensor/weight",
                new { controller = "Weight", action = "GetSensorWeight" },
                new { orgId = @"^\d+$", structId = @"^\d+$", factorId = @"^\d+$" });

            routes.MapHttpRoute(
                "AddSensorWeight",
                "sensor/weight/add",
                new { controller = "Weight", action = "AddSensorWeight" });
        }
    }
}