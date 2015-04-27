namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor
{
    using System.Web.Mvc;

    public class SensorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sensor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
