namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data
{
    using System.Web.Mvc;

    public class DataAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Data";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
