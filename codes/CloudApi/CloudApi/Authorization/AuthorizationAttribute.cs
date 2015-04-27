// --------------------------------------------------------------------------------------------
// <copyright file="AuthorizationAttribute.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141008
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;
using System.IO;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Authorization
{
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;    
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;

    public class AuthorizationAttribute:ActionFilterAttribute
    {
        private IAuthorization authorizationProvider;

        private string _strAuthorizationCode;

        public string StrAuthorizationCode
        {
            get { return _strAuthorizationCode; }
        }

        public AuthorizationAttribute()
        {
            this.authorizationProvider = new TokenAuthorizationProvider();
        }

        public AuthorizationAttribute(IAuthorization authorizationProvider)
        {
            this.authorizationProvider = authorizationProvider ?? new TokenAuthorizationProvider();
        }

        public AuthorizationAttribute(string code)
        {
            this._strAuthorizationCode = code;
            this.authorizationProvider = new TokenAuthorizationProvider();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            HttpActionDescriptor actionDescriptor = request.GetActionDescriptor();
            
            if (ConfigurationManager.AppSettings["UseAuthorization"] != null && bool.Parse(ConfigurationManager.AppSettings["UseAuthorization"]))
            {
                // 根据HttpActionDescriptor得到应用的AuthorityEntrance特性
                if (!actionDescriptor.GetCustomAttributes<NonAuthorizationAttribute>().Any())
                {
                    HttpStatusCode status;
                    // 验证
                    if (!this.authorizationProvider.IsAuthorized(request, out status))
                    {
                        var response = request.CreateResponse(
                            status,
                            StringHelper.GetMessageString("您尚未通过验证 或者 请求的资源不属于您,确认您是否有权限。"));
                        actionContext.Response = response;
                        return;
                    }                    
                }

                var token = request.GetQueryString("token");
                if (token == null && actionContext.ControllerContext.RouteData.Values.ContainsKey("token"))
                {
                    token = actionContext.ControllerContext.RouteData.Values["token"].ToString();
                }
                if (token != null)
                {
                    request.Properties["AuthorizationInfo"] = authorizationProvider.GetAuthorizationInfo(token);
                }
            }

            base.OnActionExecuting(actionContext);
        }
    }
}