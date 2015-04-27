using System;
using System.Web;

namespace SecureCloud
{
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Cache;
    using System.Text;

    using Newtonsoft.Json;

    /// <summary>
    /// ExternalLogin 的摘要说明
    /// </summary>
    public class ExternalLogin : IHttpHandler
    {
        private string host = ConfigurationManager.AppSettings["ApiURL"];

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            // 登录页网址
            var loginUrl = request.UrlReferrer;

            var userName = request.Params["userName"];
            var password = request.Params["password"];

            // 给接口发送请求
            var url = host + context.Server.HtmlEncode(string.Format("/user/login/{0}/{1}/info", userName, password));
            var proxyRequest = (HttpWebRequest)WebRequest.Create(url);
            proxyRequest.Method = "Post";
            proxyRequest.UserAgent = request.UserAgent;
            proxyRequest.ContentType = "application/x-www-form-urlencoded";
            proxyRequest.ContentLength = 0;
            var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            proxyRequest.CachePolicy = noCachePolicy;

            string content = null;
            try
            {
                var proxyResponse = (HttpWebResponse)proxyRequest.GetResponse();
                var reader = new StreamReader(proxyResponse.GetResponseStream(), Encoding.UTF8);
                content = reader.ReadToEnd();                                
            }
            catch (Exception e)
            {
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

            var loginInfo = JsonConvert.DeserializeObject<LoginInfo>(content);
            if (loginInfo.Authorized)
            {
                response.Cookies.Add(new HttpCookie("loginname", HttpUtility.UrlEncode(userName,Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("userId", loginInfo.UserId.ToString()));
                response.Cookies.Add(new HttpCookie("orgId", loginInfo.OrgId.ToString()));
                response.Cookies.Add(new HttpCookie("organization", HttpUtility.UrlEncode(loginInfo.Organization, Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("systemName", HttpUtility.UrlEncode(loginInfo.SystemName, Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("roleId", loginInfo.RoleId.ToString()));
                response.Cookies.Add(new HttpCookie("OrgLogo", HttpUtility.UrlEncode(loginInfo.Logo, Encoding.UTF8)));
                response.Cookies.Add(new HttpCookie("token", loginInfo.Token));
                if (loginUrl != null)
                {
                    response.Cookies.Add(new HttpCookie("loginUrl", loginUrl.AbsoluteUri));
                }

                response.Redirect("~/index.aspx");
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
            get
            {
                return false;
            }
        }
    }

    public class LoginInfo
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
    }
}