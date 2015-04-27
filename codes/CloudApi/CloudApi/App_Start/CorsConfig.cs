namespace FreeSun.FS_SMISCloud.Server.CloudApi
{
    using System.Web.Http;

    using Thinktecture.IdentityModel.Http.Cors.WebApi;

    /// <summary>
    /// 跨域配置类
    /// </summary>
    public class CorsConfig
    {
        /// <summary>
        /// 配置方法
        /// </summary>
        /// <param name="httpConfig">全局配置</param>
        public static void RegisterCors(HttpConfiguration httpConfig)
        {
            WebApiCorsConfiguration corsConfig = new WebApiCorsConfiguration();

            corsConfig.RegisterGlobal(httpConfig);
            
            corsConfig.AllowAll();
        }
    }
}