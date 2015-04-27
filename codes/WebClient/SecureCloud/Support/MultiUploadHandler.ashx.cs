using System.Diagnostics;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using SecureCloud.Auth;
namespace SecureCloud.Support
{
    /// <summary>
    /// MultiUploadHandler 的摘要说明
    /// </summary>
    [Authorization(AuthCode.S_Report_Manage_Upload)]
    public class MultiUploadHandler : IHttpHandler
    {
        private static string host = ConfigurationManager.AppSettings["ApiURL"];
        private string fileRootPath = ConfigurationManager.AppSettings["ConfirmedReportPath"];
        private string manualFileRootPath = ConfigurationManager.AppSettings["ManualReportPath"];
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void ProcessRequest(HttpContext context)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string multiUploadRptParams = context.Server.HtmlDecode(context.Request.QueryString["MultiUploadRptParams"]);
            var multiUploadRpt = multiUploadRptParams.Split('@');
            context.Response.ContentType = "text/html";
            HttpFileCollection files = HttpContext.Current.Request.Files;
            var retArray = new List<string>();
            try
            {
                for (int i = 0; i < multiUploadRpt.Count(); i++)
                    {
                        var RptId = multiUploadRpt[i];                     
                        string json = MultiDelReport.HttpGet(context, RptId);
                        if (json == "[]" || json == null || json == "null" || json == string.Empty)
                        {
                            continue;
                        }
                        IList<ReportInfo> jsonArray = JsonConvert.DeserializeObject<IList<ReportInfo>>(json);
                        var data = jsonArray[0];
                        var Status = data.status;
                        var RptUrl = string.Empty;
                        if (Status == "0")
                        {
                            RptUrl = data.UnconfirmedUrl;
                        }
                        else
                        {
                            RptUrl = data.ConfirmedUrl;
                        }
                        string fileName = new FileInfo(RptUrl).Name;
                        var now = DateTime.Now;
                        var filePath = string.Empty;
                        if (Status == "2")
                        {
                            filePath = manualFileRootPath;
                        }
                        else
                        {
                            filePath = fileRootPath + now.Year.ToString() + "\\" + string.Format("{0:D2}", now.Month) + "\\" +
                                     string.Format("{0:D2}", now.Day) + "\\";
                        }
                        var fileFullName = Path.Combine(filePath, fileName);
                        var dirRet = UploadHandler.CreateDirectory(filePath);
                        if (dirRet)
                        {
                            files[i].SaveAs(fileFullName);
                            var ret = UpdateFileInfoToDb(context, RptId, fileFullName, Status);
                            retArray.Add(ret);
                        }
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
                    logger.Debug(string.Format("批量上传..总耗时..{0} 毫秒\r\n", time));
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
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Write(e.Message);
            }
        }

        public string UpdateFileInfoToDb(HttpContext context, string rptId, string fileFullName, string state)
        {
            var request = context.Request;
            var response = context.Response;
            var token = context.Request.Params.Get("token");
            var postData = "token=" + token;
            var saveRpturl = host + context.Server.HtmlEncode(string.Format("/report/update/{0}?{1}", rptId, postData));
            var proxyRequest = (HttpWebRequest)WebRequest.Create(saveRpturl);
            proxyRequest.Method = "Post";
            proxyRequest.UserAgent = request.UserAgent;
            proxyRequest.ContentType = "application/x-www-form-urlencoded";
            var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            proxyRequest.CachePolicy = noCachePolicy;
            var status = "1";
            if (state == "2")
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
            try
            {
                using (HttpWebResponse proxyResponse = proxyRequest.GetResponse() as HttpWebResponse)
                {
                    response.StatusCode = (int)proxyResponse.StatusCode;
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


}