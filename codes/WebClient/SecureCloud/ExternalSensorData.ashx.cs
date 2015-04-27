using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace SecureCloud
{
    /// <summary>
    /// ExternalSensorData 的摘要说明
    /// </summary>
    public class ExternalSensorData : IHttpHandler
    {
        private string host = ConfigurationManager.AppSettings["ApiURL"];
        //主函数代码会自动调用
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request; //根据API文档获得接口的具体信息
            var response = context.Response;

            var loginUrl = request.UrlReferrer; //获得客户端上次请求的ＵＲＬ,参数不分大小写
            var userName = request.Params["userName"];
            var password = request.Params["passWord"];
            var themeId = request.Params["themeId"];
            var factorId = request.Params["factorId"];
            var sensorId = request.Params["sensorId"];
            var structId = request.Params["structId"];

            // 给接口发送请求
            var url = host + context.Server.HtmlEncode(string.Format("/user/login/{0}/{1}/info", userName, password));
            var proxyRequest = (HttpWebRequest) WebRequest.Create(url);
            proxyRequest.Method = "Post";
            proxyRequest.UserAgent = request.UserAgent;
            proxyRequest.ContentType = "application/x-www-form-urlencoded";
            proxyRequest.ContentLength = 0;
            var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            proxyRequest.CachePolicy = noCachePolicy;

            string content = null;
            try
            {
                var proxyResponse = (HttpWebResponse) proxyRequest.GetResponse();
                var reader = new StreamReader(proxyResponse.GetResponseStream(), Encoding.UTF8);
                content = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                //判断是否已经点过链接
                if (loginUrl != null)
                {
                    response.Redirect(loginUrl.AbsoluteUri);
                    response.End();
                }
            }
            if (content == null)
            {
                response.Redirect(loginUrl.AbsoluteUri);
                response.End();
            }

            var loginInfo = JsonConvert.DeserializeObject<LoginInfoData>(content);
            if (loginInfo.Authorized)
            {
                response.Cookies.Add(new HttpCookie("loginname", HttpUtility.UrlEncode(userName, Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("userId", loginInfo.UserId.ToString()));
                response.Cookies.Add(new HttpCookie("orgId", loginInfo.OrgId.ToString()));
                response.Cookies.Add(new HttpCookie("organization",
                    HttpUtility.UrlEncode(loginInfo.Organization, Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("systemName",
                    HttpUtility.UrlEncode(loginInfo.SystemName, Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("roleId", loginInfo.RoleId.ToString()));
                response.Cookies.Add(new HttpCookie("OrgLogo", HttpUtility.UrlEncode(loginInfo.Logo, Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("token", loginInfo.Token));
                response.Cookies.Add(new HttpCookie("dataStructId", structId));
                if (loginUrl != null)
                {
                    response.Cookies.Add(new HttpCookie("loginUrl", loginUrl.AbsoluteUri));
                }
                //注意:必须用"~"代替".."
                response.Redirect("~/MonitorProject/Tab.aspx?themeId=" + themeId + "&factorId=" + factorId +
                                  "&sensorId="+sensorId);
                response.End();
            }
            else
            {
                if (loginUrl != null)
                {
                    response.Redirect(loginUrl.AbsoluteUri);
                    response.End();
                }
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

    public class LoginInfoData
    {
        public bool Authorized { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public int? UserId { get; set; }

        public int? OrgId { get; set; }

        public string Organization { get; set; }

        public int? RoleId { get; set; }

        public string RoleCode { get; set; }

        public string SystemName { get; set; }

        public string Logo { get; set; }
        private const int StructId = 82;

        public int DataStructId
        {
            get { return StructId; }
            
        }
    }
}