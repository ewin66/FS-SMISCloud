namespace FreeSun.FS_SMISCloud.Server.CloudApi.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    /// <summary>
    /// 上下文工具类
    /// </summary>
    public static class HttpRequestMessageExtension
    {
        /// <summary> 获取用户IP </summary>
        /// <param name="request">请求上下文</param>
        /// <returns>用户ip</returns>
        public static string GetClientIp(this HttpRequestMessage request)
        {
            //if (request.GetQueryString("_c_ip") != null)
            //{
            //    return request.GetQueryString("_c_ip");
            //}
            //else 
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else
            {
                return null;
            }
        }

        /// <summary> A HttpRequestMessage extension method that gets query strings. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="request"> 请求上下文. </param>
        /// <returns> The query strings. </returns>
        public static Dictionary<string, string> GetQueryStrings(this HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs()
                          .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary> A HttpRequestMessage extension method that gets query string. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="request"> 请求上下文. </param>
        /// <param name="key">     The key. </param>
        /// <returns> The query string. </returns>
        public static string GetQueryString(this HttpRequestMessage request, string key)
        {
            // IEnumerable<KeyValuePair<string,string>> - right!
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            var match = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, key, true) == 0);
            if (string.IsNullOrEmpty(match.Value))
                return null;

            return match.Value;
        }

        /// <summary> A HttpRequestMessage extension method that gets action descriptor. </summary>
        /// <remarks> Liuxinyi, 2014-1-6. </remarks>
        /// <param name="request"> 请求上下文. </param>
        /// <returns> The action descriptor. </returns>
        public static HttpActionDescriptor GetActionDescriptor(this HttpRequestMessage request)
        {
            HttpConfiguration configuration = request.GetConfiguration();
            HttpControllerDescriptor controllerDescriptor = configuration.Services.GetHttpControllerSelector().SelectController(request);
            HttpControllerContext controllerContext = new HttpControllerContext(request.GetConfiguration(),
                request.GetRouteData(), request)
            {
                ControllerDescriptor = controllerDescriptor
            };
            HttpActionDescriptor actionDescriptor = configuration.Services.GetActionSelector().SelectAction(controllerContext);
            return actionDescriptor;
        }
    }
}