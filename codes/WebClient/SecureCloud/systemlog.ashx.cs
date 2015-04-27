using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace SecureCloud
{
    /// <summary>
    /// systemlog 的摘要说明
    /// </summary>
    public class systemlog : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //先取总长度
            string Url_count = context.Request.QueryString["Url_count"];
            string json_count = HttpGet(Url_count);
            SystemLogCount systemlogcount = JsonConvert.DeserializeObject<SystemLogCount>(json_count);

            //搜索条件
            string sSearch = context.Request.QueryString["sSearch"];
            string josn_count_sSearch = HttpGet(Url_count + "&keyWords=" + sSearch);
            SystemLogCount systemlogcount_sSearch = JsonConvert.DeserializeObject<SystemLogCount>(josn_count_sSearch);

            string iDisplayStart = context.Request.QueryString["iDisplayStart"];
            string iDisplayLength = context.Request.QueryString["iDisplayLength"];

            int sEcho = Convert.ToInt32(context.Request.QueryString["sEcho"]);
            int startRow = Convert.ToInt32(iDisplayStart) + 1;
            int endRow;
            if (Convert.ToInt32(iDisplayLength) == -1)
            {
                endRow = startRow + systemlogcount.count - 1;
            }
            else
            {
                endRow = startRow + Convert.ToInt32(iDisplayLength) - 1;
            }

            string Url = context.Request.QueryString["Url"];
            Url += "&startRow=" + startRow + "&endRow=" + endRow + "&keyWords=" + sSearch;

            string json = HttpGet(Url);
            IList<SystemContent> SystemLogLists = JsonConvert.DeserializeObject<IList<SystemContent>>(json);

            List<SystemContent2> SystemLoglists2 = new List<SystemContent2>();

            for (int i = 0; i < SystemLogLists.Count; i++) 
            {
                SystemContent2 systemlog = new SystemContent2();
                systemlog.systemlog_time = SystemLogLists[i].logTime.ToString();
                systemlog.systemrlog_level = SystemLogLists[i].logLevel;
                systemlog.systemrlog_processname = SystemLogLists[i].processName;
                systemlog.systemrlog_filename = SystemLogLists[i].fileName;
                systemlog.systemrlog_codenum = SystemLogLists[i].lineNo.ToString();
                systemlog.systemrlog_msg = SystemLogLists[i].message;
                systemlog.systemrlog_exception = SystemLogLists[i].exception;
                SystemLoglists2.Add(systemlog);
            }

            SystemLogEnd systemlogend = new SystemLogEnd
            {
                aaData = SystemLoglists2,
                iTotalRecords =  systemlogcount.count,
                iTotalDisplayRecords = systemlogcount_sSearch.count,
                sEcho = sEcho
            };

            string json_systemlog = JsonConvert.SerializeObject(systemlogend);
            context.Response.Write(json_systemlog);

        }

        public string HttpGet(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class SystemContent
    {
        public DateTime logTime { get; set; }
        public string logLevel { get; set; }
        public string processName { get; set; }
        public string fileName { get; set; }
        public int lineNo { get; set; }
        public string message { get; set; }
        public string exception { get; set; }
    }

    public class SystemContent2
    {
        public string systemlog_time { get; set; }
        public string systemrlog_level { get; set; }
        public string systemrlog_processname { get; set; }
        public string systemrlog_filename { get; set; }
        public string systemrlog_codenum { get; set; }
        public string systemrlog_msg { get; set; }
        public string systemrlog_exception { get; set; }
    }

    public class SystemLogCount
    {
        public int count { get; set; }
    }

    public class SystemLogEnd
    {
        public List<SystemContent2> aaData { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public int sEcho { get; set; }
    }
}