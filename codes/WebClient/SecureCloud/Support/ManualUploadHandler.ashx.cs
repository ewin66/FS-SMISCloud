using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Web;
using log4net;
using Newtonsoft.Json;
using SecureCloud.Auth;
namespace SecureCloud.Support
{
    /// <summary>
    /// ManualUploadHandler 的摘要说明
    /// </summary>
    [Authorization(AuthCode.S_Report_Manage_ManualUpload)]
    public class ManualUploadHandler : IHttpHandler
    {
        private static readonly string host = ConfigurationManager.AppSettings["ApiURL"];
        private readonly string fileRootPath = ConfigurationManager.AppSettings["ManualReportPath"];
        //文件名重名检查
        private long _counter = DateTime.Now.Ticks / 10000;
        string manualFileName = string.Empty;
        private int multiFatorId;
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;
                manualFileName = files[0].FileName;
                context.Response.ContentType = "text/html";
                var rptParams = context.Server.HtmlDecode(context.Request.QueryString["RptParam"]);
                multiFatorId = Convert.ToInt32(context.Server.HtmlDecode(context.Request.QueryString["MultiFactorId"]));
                var json = JsonConvert.DeserializeObject<ReportParam>(rptParams);
                if (! CreateMultiDirectory(fileRootPath))
                {
                    throw new ArgumentNullException("创建存储报表的多层目录失败!");
                }
                var fileFullName = GetFileFullName(json);
                var fileId = GetFileId(json);
                string filePath = Path.GetDirectoryName(fileFullName);
                if (filePath != string.Empty && !Directory.Exists(filePath))
                {
                    if (!CreateMultiDirectory(filePath))
                    {
                        throw new ArgumentNullException("创建存储报表的路径失败!");
                    }
                }
                if (File.Exists(fileFullName))//出现重名文件
                {
                   fileFullName = GetNewPathForDupes(fileFullName);
                }
                //保存报表到服务器  保存报表信息到数据库
                var reportInfo = new ReportInfo
                {
                    Id = fileId,
                    Name = Path.GetFileNameWithoutExtension(manualFileName),
                    OrgId = json.OrgId,
                    StructId = json.StructId,
                    FactorId = json.FactorId,
                    RptDateType = json.RptType,
                    ConfirmedUrl = fileFullName,
                    ConfirmedDate = Convert.ToDateTime(json.Date),
                    Status = "2",
                    ManualFileName = manualFileName
                };
                bool ret = AddNewFileToServer(context, reportInfo, json);
                string str = string.Empty;
                if (ret)
                {
                    str = fileFullName + "...上传成功";
                }
                else
                {
                    str = fileFullName + "...上传失败";
                }
                string result = "[" + "\"" + str + "\"" + "]";
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
                context.Response.Write(e.StackTrace);
            }
        }
        /// <summary>
        /// 在服务器上存储报表文件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reportInfo"> 报表记录信息 </param>
        /// <param name="param">报表参数</param>
        /// <returns></returns>
        protected bool AddNewFileToServer(HttpContext context, ReportInfo reportInfo, ReportParam param)
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            try
            {
                var fileFullName = GetFileFullName(param);
                var flag = CreateMultiDirectory(fileRootPath);
                if (flag)
                {
                    files[0].SaveAs(fileFullName);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return AddNewFileToDb(context, reportInfo);
        }
        /// <summary>
        /// 在数据库中增加报表记录
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reportInfo"></param>
        /// <returns></returns>
        protected bool AddNewFileToDb(HttpContext context, ReportInfo reportInfo)
        {
            var request = context.Request;
            var response = context.Response;
            try
            {
                var token = context.Request.Params.Get("token");
                var postData = "token=" + token;
                var saveRpturl = host + context.Server.HtmlEncode(string.Format("/report/add?{0}", postData));
                var proxyRequest = (HttpWebRequest)WebRequest.Create(saveRpturl);
                proxyRequest.Method = "Post";
                proxyRequest.UserAgent = request.UserAgent;
                proxyRequest.ContentType = "application/x-www-form-urlencoded";
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                proxyRequest.CachePolicy = noCachePolicy;
                var factorId = Convert.ToInt32(reportInfo.FactorId);
                var fatorStr = string.Empty;
                if (factorId != multiFatorId)
                {
                    fatorStr = reportInfo.FactorId.ToString();
                }
                NameValueCollection parameters = new NameValueCollection()
            {
                {"Id", reportInfo.Id},
                {"Name", reportInfo.Name},
                {"ManualFileName", manualFileName},
                {"OrgId", reportInfo.OrgId.ToString()},
                {"StructId", reportInfo.StructId.ToString()},             
                {"FactorId", fatorStr},             
                {"DateType", reportInfo.RptDateType.ToString()},
                {"FileFullName", reportInfo.ConfirmedUrl},
                {"Date", reportInfo.ConfirmedDate.ToString()},
                {"Status", reportInfo.Status}            
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
                    response.StatusCode = (int)proxyResponse.StatusCode;
                    string Result;
                    using (Stream receiveStream = proxyResponse.GetResponseStream())
                    {
                        Encoding Encode = Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(receiveStream, Encode);
                        Result = readStream.ReadToEnd();
                    }
                    // return Result;
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
       
        /// <summary>
        /// 获取报表编号: 6位结构物Id + 6位监测因素Id + 2位报表日期类型 +  (创建时间 - “1970-01-01”经过的毫秒数)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string GetFileId(ReportParam param)
        {
            var sb = new StringBuilder(50);
            sb.AppendFormat("{0:D6}", param.StructId);
            sb.AppendFormat("-{0:D6}", param.FactorId);
            sb.AppendFormat("-{0:D2}", param.RptType);
            sb.Append("-").Append((long)(Convert.ToDateTime(param.Date) - new DateTime(1970, 1, 1)).TotalMilliseconds);
            return sb.ToString();
        }
        
        /// <summary>
        /// 创建多层文件夹
        /// </summary>
        /// <param name="directoryName">文件夹名称</param>
        /// <returns></returns>
        public static bool CreateMultiDirectory(string directoryName)
        {
            char separator = Path.DirectorySeparatorChar;
            string path = directoryName;
            string rootPath = Path.GetPathRoot(path);
            if (!Directory.Exists(rootPath))
                return false;
            if (Directory.Exists(path))
                return true;
            string[] directorys = path.Split(separator);
            var sb = new StringBuilder();
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
            catch (Exception)
            {
                return false;
            }
            return true;
        }
       
        /// <summary>
        /// 获取文件全路径
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string GetFileFullName(ReportParam param)
        {
            string reportPath = fileRootPath;
            string fileName = manualFileName;
            string fileFullName = Path.Combine(reportPath, fileName);
            string  renameFilename = GetNewPathForDupes(fileFullName);
            return renameFilename;
        }

        /// <summary>
        /// Generates a new path for duplicate filenames.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string GetNewPathForDupes(string path)
        {
            if (path != null)
            {
                string directory = Path.GetDirectoryName(path);
                string filename = Path.GetFileNameWithoutExtension(path);
                if (filename.Contains("("))
                {
                   filename  =  filename.Split('(')[0];
                }
                string extension = Path.GetExtension(path);
                string newFullPath = path;
                if (File.Exists(newFullPath))
                {
                    string newFilename = string.Format("{0}({1}){2}", filename, _counter, extension);
                    try
                    {
                        newFullPath = Path.Combine(directory, newFilename);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                return newFullPath;
            }
            else
            {
                return null;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class ReportInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string ManualFileName { get; set; }
            public int? OrgId { get; set; }
            public int? StructId { get; set; }
            public int? FactorId { get; set; }
            public int RptDateType { get; set; }
            public string ConfirmedUrl { get; set; }
            public DateTime ConfirmedDate { get; set; }
            public string Status { get; set; }
        }

        public class ReportParam
        {
            public int OrgId { get; set; }
            public int StructId { get; set; }
            public string StructName { get; set; }
            public int FactorId { get; set; }
            public string FactorName { get; set; }
            public int RptType { get; set; }
            public string Date { get; set; }
            public string ExtName { get; set; }
        }

        public enum DateType
        {
            Day = 1,
            Week = 2,
            Month = 3,
            Year = 4
        }

    }
}