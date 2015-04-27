namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Factor
{
    using System.Web.Mvc;

    public class FactorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Factor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
