using System.Web.Mvc;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.DAU
{
    public class DAUAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DAU";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
