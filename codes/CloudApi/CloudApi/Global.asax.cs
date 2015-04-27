namespace FreeSun.FS_SMISCloud.Server.CloudApi
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.Service;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // CorsConfig.RegisterCors(GlobalConfiguration.Configuration);
            log4net.Config.XmlConfigurator.Configure();

            WebClientService.TryInit(this.Server.MapPath("~/bin/"));
        }

        protected void Application_End()
        {
            WebClientService.TryStop();
        }
    }
}