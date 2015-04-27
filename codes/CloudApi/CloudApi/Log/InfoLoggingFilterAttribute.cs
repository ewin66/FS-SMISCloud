namespace FreeSun.FS_SMISCloud.Server.CloudApi.Log
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;

    /// <summary>
    /// 普通日志筛选器
    /// </summary>
    public class InfoLoggingFilterAttribute : ActionFilterAttribute, IActionFilter
    {
        private const string Key = "__action_duration__";

        private SqlLogger logger = new SqlLogger();

        /// <summary>
        /// 实现action执行前的事件
        /// </summary>
        /// <param name="actionContext">action上下文</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // 计时
            var stopWatch = new Stopwatch();
            actionContext.Request.Properties[Key] = stopWatch;
            stopWatch.Start();         

            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// 实现action执行完成的事件
        /// </summary>
        /// <param name="actionExecutedContext">action上下文</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {            
            // 请求信息:
            var logInfo = BuildLoggingObject(actionExecutedContext);

            // 写入日志
            logger.WriteLogAsyn(logInfo);

            base.OnActionExecuted(actionExecutedContext);
        }

        /// <summary>
        /// 构造日志对象
        /// </summary>
        private LoggingObject BuildLoggingObject(HttpActionExecutedContext actionExecutedContext)
        {
            // 计时
            var stopWatch = actionExecutedContext.Request.Properties[Key] as Stopwatch;
            double time = -1;
            if (stopWatch != null)
            {
                stopWatch.Stop();
                time = stopWatch.Elapsed.TotalSeconds;
            }

            // statusCode
            int resposeStatus;
            try
            {
                resposeStatus = (int)actionExecutedContext.Response.StatusCode;
            }
            catch (Exception)
            {
                resposeStatus = (int)HttpStatusCode.InternalServerError;
            }
            // method
            string method = actionExecutedContext.Request.Method.Method;
            // url
            string url = actionExecutedContext.Request.RequestUri.AbsoluteUri;
            // ip
            string ip = actionExecutedContext.Request.GetClientIp();
            // userAgent
            string userAgent = string.Join(";", actionExecutedContext.Request.Headers.UserAgent);
            // clientType
            string clientType = HttpHelper.AnalyzeUserAgent(actionExecutedContext.Request.Headers.UserAgent);                       
            // controller
            string controllerName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            // action
            string actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            // content isVisible
            var attr =
                actionExecutedContext.ActionContext.ActionDescriptor
                    .GetCustomAttributes<LogInfoAttribute>().FirstOrDefault();
            string content;            
            bool isVisible;
            if (attr != null)
            {
                content = attr.Description;
                isVisible = attr.IsVisible;
            }
            else
            {
                content = actionName;
                isVisible = false;
            }
            // parameter
            string parameter = string.Empty;
            // parameterShow
            string parameterStr = string.Empty;
            if (method != "GET")
            {
                if (actionExecutedContext.Request.Properties.ContainsKey("ActionParameter"))
                {
                    parameter = actionExecutedContext.Request.Properties["ActionParameter"] as string;
                }

                if (actionExecutedContext.Request.Properties.ContainsKey("ActionParameterShow"))
                {
                    parameterStr = actionExecutedContext.Request.Properties["ActionParameterShow"] as string;
                }
            }                                     
            // userNo sessionId   
            int userNo = -1;
            string token = string.Empty;
            if (actionExecutedContext.Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                AuthorizationInfo info = actionExecutedContext.Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                if (info != null)
                {
                    userNo = info.UserId;
                    token = info.Token;
                }
            }                     

            var logInfo = new LoggingObject
                              {
                                  LogTime = DateTime.Now,
                                  ClientIp = ip,
                                  UserAgent = userAgent,
                                  ClientType = clientType,
                                  Url = url,
                                  Method = method,
                                  Parameter = parameter.Length > 4000 ? parameter.Substring(3995) + "..." : parameter,
                                  ParameterShow = parameterStr.Length > 4000 ? parameterStr.Substring(3995) + "..." : parameterStr,
                                  Content = content,
                                  Controller = controllerName,
                                  Action = actionName,
                                  StatusCode = resposeStatus,
                                  Duration = Convert.ToDecimal(time),
                                  UserNo = userNo,
                                  SessionId = token,
                                  IsVisible = isVisible
                              };
            return logInfo;
        }        
    }
}