namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Warning
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class WarnningAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Warnning";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
