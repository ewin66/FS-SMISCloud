namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Log
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class LogAreaRegistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public override string AreaName
        {
            get
            {
                return "Log";
            }
        }        
    }
}
