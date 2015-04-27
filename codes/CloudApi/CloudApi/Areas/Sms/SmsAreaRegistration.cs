using System.Web.Mvc;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sms
{   
    public class SmsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sms";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
