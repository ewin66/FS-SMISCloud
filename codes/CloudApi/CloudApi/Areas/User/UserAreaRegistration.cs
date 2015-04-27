namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.User
{
    using System.Web.Mvc;

    public class UserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "User";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
