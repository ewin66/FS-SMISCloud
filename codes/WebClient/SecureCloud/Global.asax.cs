using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using SecureCloud.Support;
using System.Linq;
using SecureCloud.Auth;



namespace SecureCloud
{
    public class Global : System.Web.HttpApplication
    {
        private static string host = ConfigurationManager.AppSettings["ApiURL"];
        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_PostMapRequestHandler(object sender, EventArgs e) 
        {
            IHttpHandler handler = this.Request.RequestContext.HttpContext.CurrentHandler;
            var attrs = handler.GetType().GetCustomAttributes(typeof(AuthorizationAttribute), true);

            if (attrs.Length > 0) 
            {
                var code = (attrs[0] as AuthorizationAttribute).AuthCode;
                var token = this.Request.Params.Get("token");
                try
                {
                    //给接口发送请求                    
                    var postData = "token=" + token;
                    var delRpturl = host + this.Server.HtmlEncode(string.Format("/auth/{0}?{1}", code, postData));
                    var proxyRequest = (HttpWebRequest)WebRequest.Create(delRpturl);
                    proxyRequest.Method = "Post";
                    proxyRequest.UserAgent = this.Request.UserAgent;
                    proxyRequest.ContentType = "application/x-www-form-urlencoded";
                    proxyRequest.ContentLength = 0;
                    var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    proxyRequest.CachePolicy = noCachePolicy;
                    //发出代理请求，并获取响应
                    using (HttpWebResponse proxyResponse = proxyRequest.GetResponse() as HttpWebResponse)
                    {
                        string Result;
                        using (Stream receiveStream = proxyResponse.GetResponseStream())
                        {
                            Encoding Encode = Encoding.GetEncoding("utf-8");
                            StreamReader readStream = new StreamReader(receiveStream, Encode);
                            Result = readStream.ReadToEnd();
                        }
                        proxyResponse.Close();
                        if (Result == "false") 
                        {
                            this.Context.Response.ContentType = "text/html";
                            this.Response.StatusCode = 202;
                            this.Response.Write(405);
                            this.Response.End();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

    }
}