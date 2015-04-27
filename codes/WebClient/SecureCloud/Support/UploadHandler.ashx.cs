using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using SecureCloud.Auth;
namespace SecureCloud.Support
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    [Authorization(AuthCode.S_Report_Manage_Upload)]
    public class UploadHandler : IHttpHandler
    {
        private static string host = ConfigurationManager.AppSettings["ApiURL"];
        private string fileRootPath = ConfigurationManager.AppSettings["ConfirmedReportPath"];
        private string manualFileRootPath = ConfigurationManager.AppSettings["ManualReportPath"];

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/html";
                var type = context.Request.QueryString["RptStatus"];
                var rptId = context.Server.HtmlDecode(context.Request.QueryString["RptId"]);
                var rptUrl = context.Server.HtmlDecode(context.Request.QueryString["RptUrl"]);
                //保存报表到服务器
                if (type != "0")
                {
                    if (File.Exists(rptUrl))
                    {
                        File.Delete(rptUrl);
                    }
                }
                string fileName = new FileInfo(rptUrl).Name;
                var now = DateTime.Now;
                var filePath = string.Empty;
                if (type == "2")
                {
                    filePath = manualFileRootPath;
                }
                else
                {
                    filePath = fileRootPath + now.Year.ToString() + "\\" + string.Format( "{0:D2}", now.Month) + "\\" +
                             string.Format( "{0:D2}", now.Day) + "\\";
                }
                var flag = SaveFileToServer(filePath, fileName);
                var fileFullName =  filePath + fileName;
                if (flag)
                {
                    //保存报表信息到数据库
                    var Result = SaveFileInfoToDb(context, rptId, fileFullName, type);
                    context.Response.Write(Result);
                }
                else
                {
                    string ret = fileFullName + "保存到服务器失败";
                    string result = "[" + "\"" + ret + "\"" + "]";
                    context.Response.Write(result);
                }
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
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.Write(e.Message);
                context.Response.Write(e.StackTrace);
            }
        }

        public static bool SaveFileToServer(string filePath, string fileName)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            try
            {
                var fileFullName = Path.Combine(filePath, fileName);
                var flag = CreateDirectory(filePath);
                if (flag)
                {
                    files[0].SaveAs(fileFullName);
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string SaveFileInfoToDb(HttpContext context, string rptId, string fileFullName, string type)
        {
            var request = context.Request;
            var response = context.Response;
            var token = context.Request.Params.Get("token");
            var postData = "token=" + token;
            var saveRpturl = host + context.Server.HtmlEncode(string.Format("/report/update/{0}?{1}", rptId, postData));
            var proxyRequest = (HttpWebRequest) WebRequest.Create(saveRpturl);
            proxyRequest.Method = "Post";
            proxyRequest.UserAgent = request.UserAgent;
            proxyRequest.ContentType = "application/x-www-form-urlencoded";
            var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            proxyRequest.CachePolicy = noCachePolicy;
            var status = "1";
            if (type == "2")
            {
                status = "2";
            }
            NameValueCollection parameters = new NameValueCollection()
            {
                {"FileFullName", fileFullName},
                {"Date", DateTime.Now.ToString()},
                {"Status", status}

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

        public static bool CreateDirectory(string directoryName)
        {
            char separator = Path.DirectorySeparatorChar;
            string path = directoryName;
            string rootPath = Path.GetPathRoot(path);

            if (!Directory.Exists(rootPath))
                return false;

            if (Directory.Exists(path))
                return true;
            string[] directorys = path.Split(separator);
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var directory in directorys)
                {
                    sb.Append(directory);
                    sb.Append(separator);
                    ////根路径不需要创建
                    if (sb.ToString() == rootPath)
                        continue;

                    if (!Directory.Exists(sb.ToString()))
                    {
                        Directory.CreateDirectory(sb.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
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