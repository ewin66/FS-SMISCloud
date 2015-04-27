using System.Web.Mvc;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Assist
{
    public class AssistAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Assist";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
