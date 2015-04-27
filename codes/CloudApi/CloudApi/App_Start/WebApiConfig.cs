namespace FreeSun.FS_SMISCloud.Server.CloudApi
{
    using System.Configuration;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json;

    /// <summary>
    /// WebApi配置类
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 配置方法
        /// </summary>
        /// <param name="config">全局配置</param>
        public static void Register(HttpConfiguration config)
        {
            // 支持jsonp
            config.Formatters.Insert(0, new JsonpMediaTypeFormatter());

            // 只返回json格式
            config.Formatters.Remove(config.Formatters.XmlFormatter); 
       
            // 返回Json时间格式
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;

            // 将所有日期转换成UTC格式
            // var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            // json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // 有缩进
            if (bool.Parse(ConfigurationManager.AppSettings["ResutlIntended"]))
            {
                config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            }

            // 验证
            config.Filters.Add(new AuthorizationAttribute());

            // 异常记录到文件
            config.Filters.Add(new ExceptionLoggingFilterAttribute());

            // 普通请求操作记录到数据库
            config.Filters.Add(new InfoLoggingFilterAttribute());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;            
        }
    }
}