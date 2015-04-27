using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SecureCloud
{
    /// <summary>
    /// reportHandler 的摘要说明
    /// </summary>
    public class reportHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
           //获得总长度
            string Url_count = context.Request.QueryString["Url_count"];
            string json_count = HttpGet(Url_count);
            reportCount reportcount = JsonConvert.DeserializeObject<reportCount>(json_count);
            //搜索条件
            string sSearch = context.Request.QueryString["sSearch"];//全局搜索字段
            string josn_count_sSearch = HttpGet(Url_count + "&keyWords=" + sSearch);
            reportCount reportcount_sSearch = JsonConvert.DeserializeObject<reportCount>(josn_count_sSearch);

            string iDisplayStart = context.Request.QueryString["iDisplayStart"];//从第几行开始
            string iDisplayLength = context.Request.QueryString["iDisplayLength"];

            int sEcho = Convert.ToInt32(context.Request.QueryString["sEcho"]);//DataTables用来生成的信息
            int startRow = Convert.ToInt32(iDisplayStart) + 1;
            int endRow;
            if (Convert.ToInt32(iDisplayLength) == -1)
            {
                endRow = startRow + reportcount.count - 1;
            }
            else
            {
                endRow = startRow + Convert.ToInt32(iDisplayLength) - 1;
            }
            string Url = context.Request.QueryString["Url"];
            Url += "&startRow=" + startRow + "&endRow=" + endRow + "&keyWords=" + sSearch;

            string json = HttpGet(Url);
            IList<reportContent> reportLists = JsonConvert.DeserializeObject<IList<reportContent>>(json);

            List<reportContent2> reportLists2 = new List<reportContent2>();
          
            for (int i = 0; i < reportLists.Count; i++)
            {
                reportContent2 rptContent = new reportContent2();
                rptContent.report_name = reportLists[i].reportName;
                rptContent.report_time = reportLists[i].time;
                rptContent.report_download = "<a class='label label-important label-mini' onclick='DownLoadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(reportLists[i].url) + "\")'>下载<i class='icon-share-alt'></i></a>";
                reportLists2.Add(rptContent);
            }

            reportEnd rptend = new reportEnd
            {
                aaData = reportLists2,
                iTotalRecords = reportcount.count,
                iTotalDisplayRecords = reportcount_sSearch.count,
                sEcho = sEcho
            };

            string jsonReportList = JsonConvert.SerializeObject(rptend);
            context.Response.Write(jsonReportList);

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
    public class reportCount
    {
        public int count { get; set; }
    }

    public class reportContent
    {
        public string reportName { get; set; }
        public string url { get; set; }
        public string time { get; set; }
    }
    public class reportContent2
    {
        public string report_name { get; set; }
        public string report_time { get; set; }
        public string report_download { get; set; }
    }

    public class reportEnd
    {
        public List<reportContent2> aaData { get; set; }//数组的数组，表格中的实际数据
        public int iTotalRecords { get; set; }//实际的行数
        public int iTotalDisplayRecords { get; set; }//过滤之后，实际的行数
        public int sEcho { get; set; }//来自客户端sEcho的没有变化的复制品
    }
}