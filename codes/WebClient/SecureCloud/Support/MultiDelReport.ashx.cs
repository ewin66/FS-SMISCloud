using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Web;
using log4net;
using Newtonsoft.Json;
using System.Collections.Specialized;
using SecureCloud.Auth;
namespace SecureCloud.Support
{
    /// <summary>
    /// MultiDelReport 的摘要说明
    /// </summary>
    [Authorization(AuthCode.S_Report_Manage_Delete)]
    public class MultiDelReport : IHttpHandler
    {
        private static string host = ConfigurationManager.AppSettings["ApiURL"];
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void ProcessRequest(HttpContext context)
        {
            string multiDelRptParams = context.Server.HtmlDecode(context.Request.QueryString["MultiDelRptParams"]);
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                var rptIdArray = multiDelRptParams.Split('@');
                var retArray = new List<string>();
                foreach (var delRptId in rptIdArray)
                {
                    string json = HttpGet(context, delRptId);
                    if (json == "[]" || json == null || json == "null" || json == string.Empty)
                    {
                        continue;
                    }
                    IList<ReportInfo> jsonArray = JsonConvert.DeserializeObject<IList<ReportInfo>>(json);
                    var data = jsonArray[0];
                    var Status = data.status;
                    var delRptUrl = string.Empty;
                    var ret = string.Empty;
                    if (Status == "1")
                    {
                        delRptUrl = data.ConfirmedUrl;
                        ret = UpdateFileInfoToDb(context, delRptId, delRptUrl);
                        string json2 = HttpGet(context, delRptId);
                        if (json2 == "[]" || json2 == null || json2 == "null" || json2 == string.Empty)
                        {
                            continue;
                        }
                        IList<ReportInfo> jsonArray2 = JsonConvert.DeserializeObject<IList<ReportInfo>>(json2);
                        var data2 = jsonArray2[0];
                        var Status2 = data2.status;
                        if (Status2 == "0")
                        {
                            DeleteFileFromServer(delRptUrl);
                        }
                    }
                    else if (Status == "0" || Status == "2")
                    {
                        ret = DeleteFileInfoFromDb(context, delRptId);
                    }
                    retArray.Add(ret);
                }
                var result = "[";
                for (int i = 0; i < retArray.Count; i++)
                {
                    if (i > 0)
                    {
                        result += ",";
                        result += retArray[i];
                    }
                    else
                    {
                        result += retArray[i];
                    }
                }
                result += "]";
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("批量删除..总耗时..{0} 毫秒\r\n", time));
                context.Response.Write(result);
            }
            catch (WebException e)
            {
                if ((int)((HttpWebResponse)e.Response).StatusCode == 405)
                {
                    context.Response.StatusCode = 202;
                    context.Response.Write(405);
                    context.Response.End();
                }
                else
                {
                    throw e;
                }
            }
            catch (Exception e)
            {
                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //context.Response.Write(e.Message);
                context.Response.ContentType = "text/html";
                context.Response.Write(
                    "<script  language='javascript'>window.alert('删除文件时发生异常!');window.history.go(-2);window.opener.location.reload() " +
                    "</script>");
            }
        }

        public static string HttpGet(HttpContext context, string delRptId)
        {
            try
            {
                //给接口发送请求
                var token = context.Request.Params.Get("token");
                var postData = "token=" + token;
                var delRpturl = host + context.Server.HtmlEncode(string.Format("/report/info/{0}?{1}", delRptId, postData));
                var proxyRequest = (HttpWebRequest) WebRequest.Create(delRpturl);
                proxyRequest.Method = "Get";
                proxyRequest.ContentType = "text/json;charset=UTF-8";
                proxyRequest.UserAgent = context.Request.UserAgent;
                proxyRequest.ContentType = "application/x-www-form-urlencoded";
                proxyRequest.ContentLength = 0;
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                proxyRequest.CachePolicy = noCachePolicy;
                context.Response.ContentType = "application/json";
                using (HttpWebResponse proxyResponse = proxyRequest.GetResponse() as HttpWebResponse)
                {
                    context.Response.StatusCode = (int) proxyResponse.StatusCode;
                    string Result;
                    using (Stream receiveStream = proxyResponse.GetResponseStream())
                    {
                        Encoding Encode = Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(receiveStream, Encode);
                        Result = readStream.ReadToEnd();
                    }
                    return Result;
                }
            }
            catch (Exception e)
            {
                //throw e;
                //context.Response.Write("<script  language='javascript'>window.alert('发生异常!');window.history.go(-2);window.opener.location.reload() " + "</script>");
                return string.Empty;
            }
        }

        public static bool DeleteFileFromServer(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public static string DeleteFileInfoFromDb(HttpContext context, string delRptId)
        {
            try
            {
                //给接口发送请求
                var token = context.Request.Params.Get("token");
                var postData = "token=" + token;
                var delRpturl = host + context.Server.HtmlEncode(string.Format("/report/remove/{0}?{1}", delRptId, postData));
                var proxyRequest = (HttpWebRequest) WebRequest.Create(delRpturl);
                proxyRequest.Method = "Post";
                proxyRequest.UserAgent = context.Request.UserAgent;
                proxyRequest.ContentType = "application/x-www-form-urlencoded";
                proxyRequest.ContentLength = 0;
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                proxyRequest.CachePolicy = noCachePolicy;
                // 构造响应对象
                context.Response.ContentType = "application/json";
                //发出代理请求，并获取响应
                using (HttpWebResponse proxyResponse = proxyRequest.GetResponse() as HttpWebResponse)
                {
                    context.Response.StatusCode = (int) proxyResponse.StatusCode;
                    string Result;
                    using (Stream receiveStream = proxyResponse.GetResponseStream())
                    {
                        Encoding Encode = Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(receiveStream, Encode);
                        Result = readStream.ReadToEnd();
                    }
                    //context.Response.Write("\n");
                    //context.Response.Write(Result);
                    return Result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string UpdateFileInfoToDb(HttpContext context, string rptId, string fileFullName)
        {
            var request = context.Request;
            var response = context.Response;
            try
            {
                var token = context.Request.Params.Get("token");
                var postData = "token=" + token;
                var saveRpturl = host + context.Server.HtmlEncode(string.Format("/report/update/{0}?{1}", rptId, postData));
                var proxyRequest = (HttpWebRequest) WebRequest.Create(saveRpturl);
                proxyRequest.Method = "Post";
                proxyRequest.UserAgent = request.UserAgent;
                proxyRequest.ContentType = "application/x-www-form-urlencoded";
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                proxyRequest.CachePolicy = noCachePolicy;
                NameValueCollection parameters = new NameValueCollection()
                {
                    {"FileFullName", null},
                    {"Date", null},
                    {"Status", "0"}
                };
                //构造参数
                if (parameters.Count > 0)
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
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
                    byte[] tempdata = encode.GetBytes(buffer.ToString());
                    proxyRequest.ContentLength = tempdata.Length;
                    Stream stream = proxyRequest.GetRequestStream();
                    stream.Write(tempdata, 0, tempdata.Length);
                    stream.Close();
                }
                // 构造响应对象
                response.ContentType = "text/html";
                //发出代理请求，并获取响应
                using (HttpWebResponse proxyResponse = proxyRequest.GetResponse() as HttpWebResponse)
                {
                    response.StatusCode = (int) proxyResponse.StatusCode;
                    string Result;
                    using (Stream receiveStream = proxyResponse.GetResponseStream())
                    {
                        Encoding Encode = Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(receiveStream, Encode);
                        Result = readStream.ReadToEnd();
                    }
                    return Result;
                }
            }
            catch (Exception e)
            {
                throw e;
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

    public class ReportInfo
    {
        public string reportId { get; set; }
        public string reportName { get; set; }
        public OrgInfo Org { get; set; }
        public StructInfo Struct { get; set; }
        public string ConfirmedUrl { get; set; }
        public string UnconfirmedUrl { get; set; }
        public string ConfirmedDate { get; set; }
        public string UnconfirmedDate { get; set; }
        public string status { get; set; }
    }

    public class OrgInfo
    {
        public int? OrgId { get; set; }
        public string OrgName { get; set; }
    }

    public class StructInfo
    {
        public int? StructId { get; set; }
        public string StructName { get; set; }
    }
}