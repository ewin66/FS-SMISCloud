namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Structure
{
    using System.Web.Mvc;

    public class StructureAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Structure";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}
