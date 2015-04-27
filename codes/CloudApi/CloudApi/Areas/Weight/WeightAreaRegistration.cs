using System.Web.Mvc;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Weight
{
    public class WeightAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Weight";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
