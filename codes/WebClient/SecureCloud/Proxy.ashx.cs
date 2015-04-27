using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;

namespace ProxyDemo
{
    using System.Configuration;

    using log4net;

    /// <summary>
    /// Proxy 的摘要说明
    /// </summary>
    public class Proxy : IHttpHandler
    {
        private string host = ConfigurationManager.AppSettings["ApiURL"];

        private ILog log = LogManager.GetLogger("Proxy");

        public void ProcessRequest(HttpContext context)
        {
            // 获取请求信息
            var orgRequest = context.Request;
            var rawUrl = orgRequest.RawUrl;
            string url = host + rawUrl.Substring(rawUrl.IndexOf("?path=") + 6);
            string method = orgRequest.HttpMethod;
            string userAgent = orgRequest.UserAgent;
            string contentType = orgRequest.ContentType;
            int contentLen = orgRequest.ContentLength;
            string clientIp = orgRequest.UserHostAddress;
            if (url.Contains("?"))
            {
                url += "&_c_ip=" + clientIp;
            }
            else
            {
                url += "?_c_ip=" + clientIp;
            }
            NameValueCollection parameters = orgRequest.Form;

            //构造代理请求
            var proxyRequest = (HttpWebRequest)WebRequest.Create(url);
            
            proxyRequest.Method = method;
            proxyRequest.UserAgent = userAgent;
            proxyRequest.ContentType = contentType;
            proxyRequest.ContentLength = contentLen;
            proxyRequest.CookieContainer = this.GetCookies(orgRequest);// 构造cookie
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            proxyRequest.CachePolicy = noCachePolicy;

            //非get请求，构造参数
            if (method.ToUpper() != ("GET"))
            {
                if (parameters.Count > 0)
                {                    
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (key == "path" || key == "_c_ip") continue;
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        }
                        i++;
                    }
                    Encoding encode = Encoding.GetEncoding("utf-8");
                    byte[] data = encode.GetBytes(buffer.ToString());
                    proxyRequest.ContentLength = data.Length;
                    Stream stream = proxyRequest.GetRequestStream();
                    stream.Write(data, 0, data.Length);

                    stream.Close();
                }
            }

            // 构造响应对象
            var orgResponse = context.Response;
            orgResponse.ContentType = "application/json";

            //发出代理请求，并获取响应
            try
            {
                log.Info(proxyRequest.RequestUri);
                using (HttpWebResponse proxyResponse = proxyRequest.GetResponse() as HttpWebResponse)
                {
                    orgResponse.StatusCode = (int)proxyResponse.StatusCode;
                    string result;
                    using (Stream receiveStream = proxyResponse.GetResponseStream())
                    {
                        Encoding encode = Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(receiveStream, encode);
                        result = readStream.ReadToEnd();
                    }
                    // 设置响应的cookie
                    foreach (Cookie cookie in proxyResponse.Cookies)
                    {
                        HttpCookie c = new HttpCookie(cookie.Name, cookie.Value);
                        c.Path = cookie.Path;
                        c.Expires = cookie.Expires;
                        orgResponse.Cookies.Add(c);
                    }

                    orgResponse.Write("\n");
                    orgResponse.Write(result);
                }
            }
            catch (WebException webExcpetion)
            {
                if (webExcpetion.Status == WebExceptionStatus.ProtocolError)
                {
                    orgResponse.StatusCode = (int)((HttpWebResponse)webExcpetion.Response).StatusCode;
                    string result;
                    using (Stream receiveStream = webExcpetion.Response.GetResponseStream())
                    {
                        Encoding encode = Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(receiveStream, encode);
                        result = readStream.ReadToEnd();
                    }
                    orgResponse.Write("\n");
                    orgResponse.Write(result);
                }
                log.Error("webExcpetion", webExcpetion);
            }
            catch (Exception e)
            {
                orgResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                orgResponse.Write("代理出现异常:" + e.Message);
                log.Error("proxy error", e);
            }

            // 结束响应
            orgResponse.End();
        }

        private CookieContainer GetCookies(HttpRequest request)
        {
            CookieContainer myCookieContainer = new CookieContainer();

            HttpCookie requestCookie;
            int requestCookiesCount = request.Cookies.Count;
            for (int i = 0; i < requestCookiesCount; i++)
            {
                requestCookie = request.Cookies[i];
                Cookie clientCookie = new Cookie(requestCookie.Name, requestCookie.Value, requestCookie.Path, requestCookie.Domain == null ? request.Url.Host : requestCookie.Domain);
                myCookieContainer.Add(clientCookie);
            }
            return myCookieContainer;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}