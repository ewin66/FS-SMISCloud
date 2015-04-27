namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.IOS
{
    using System.Web.Mvc;

    public class IOSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "IOS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
