namespace FreeSun.FS_SMISCloud.Server.CloudApi
{
    using System.Web.Mvc;

    /// <summary>
    /// 筛选器配置类
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 配置方法
        /// </summary>
        /// <param name="filters">筛选器集合</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}