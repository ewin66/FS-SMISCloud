namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.HotSpot
{
    using System.Web.Mvc;

    public class HotSpotAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HotSpot";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
