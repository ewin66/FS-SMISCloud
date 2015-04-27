namespace FreeSun.FS_SMISCloud.Server.CloudApi.Log
{
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;

    using log4net;

    /// <summary>
    /// 异常日志筛选器
    /// </summary>
    internal class ExceptionLoggingFilterAttribute : ExceptionFilterAttribute, IExceptionFilter
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实现异常发生的事件
        /// </summary>
        /// <param name="actionExecutedContext">action上下文</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            this.log.Error(
                string.Format("发生异常：{0}", this.BuildLogEntry(actionExecutedContext.ActionContext)),
                actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }

        /// <summary>
        /// 构造日志数据
        /// </summary>
        /// <param name="actionContext">action上下文</param>
        /// <returns>日志数据</returns>
        private string BuildLogEntry(HttpActionContext actionContext)
        {
            string route = actionContext.Request.GetRouteData().Route.RouteTemplate;
            string method = actionContext.Request.Method.Method;
            string url = actionContext.Request.RequestUri.AbsoluteUri;
            string controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = actionContext.ActionDescriptor.ActionName;
            string ip = actionContext.Request.GetClientIp();

            return string.Format("{5}, {0} {1}, 路由: {2}, 处理方法:{3}.{4}", method, url, route, controllerName, actionName, ip);
        }
    }
}