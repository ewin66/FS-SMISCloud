namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Section
{
    using System.Web.Mvc;

    public class SectionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Section";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}