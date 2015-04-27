using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace SecureCloud.Support
{
    /// <summary>
    /// ManageReport 的摘要说明
    /// </summary>
    public class ManageReport : IHttpHandler
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
            var str1 = "";
            for (int i = 0; i < reportLists.Count; i++)
            {
                for (int j = 0; j < reportLists[i].reports.Count; j++)
                {
                    report rpt = reportLists[i].reports[j];
                    reportContent2 rptContent = new reportContent2();
                    rptContent.reportName = rpt.reportName;
                    rptContent.orgName = rpt.OrgName;
                    rptContent.structName = rpt.StructName;
                    rptContent.reportType = rpt.DateType;
                    rptContent.checkbox = "<input type='checkbox' class='checkboxes' onclick='checkboxClicked()' value='" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "' />";
                    if (reportLists[i].status == "0")//未确认
                    {
                        str1 = "";
                        rptContent.status = "<span class='label label-info'>未确认</span>";
                        rptContent.date = rpt.UnconfirmedDate;
                        str1 = "<a href='#' class='btn blue' onclick='DownloadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.UnconfirmedUrl) + "\")'>下载</a>&nbsp;";
                        str1 += "<a href='#UploadReportModal' class='btn green' data-toggle='modal' onclick='UploadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "\",\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.UnconfirmedUrl) + "\",\"" + reportLists[i].status + "\")'>上传</a>&nbsp;";
                        str1 += "<a href='#deleteRptModal' class='btn ' data-toggle='modal' onclick='DeleteReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "\",\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportName) + "\")'>删除</a>";
                    }
                    else
                    {
                        rptContent.date = rpt.ConfirmedDate;
                        if (reportLists[i].status == "1")//已确认
                        {
                            str1 = "";
                            rptContent.status = "<span class='label label-info'>已确认</span>";
                            str1 = "<a href='#' class='btn blue' onclick='DownloadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.UnconfirmedUrl) + "\")'>下载</a>&nbsp;";
                            str1 += "<a href='#' class='btn blue' onclick='DownloadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.ConfirmedUrl) + "\")'>下载已确认</a>&nbsp;";
                            str1 += "<a href='#UploadReportModal' class='btn green' data-toggle='modal' onclick='UploadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "\",\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.ConfirmedUrl) + "\",\"" + reportLists[i].status + "\")'>重新上传</a>&nbsp;";
                            str1 += "<a href='#deleteRptModal' class='btn ' data-toggle='modal' onclick='DeleteReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "\",\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportName) + "\")'>删除</a>";
                        }
                        if (reportLists[i].status == "2")//人工上传
                        {
                            str1 = "";
                            rptContent.status = "<span class='label label-info'>人工上传</span>";
                            rptContent.date = rpt.ConfirmedDate;
                            str1 = "<a href='#' class='btn blue' onclick='DownloadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.ConfirmedUrl) + "\")'>下载</a>&nbsp;";
                            str1 += "<a href='#UploadReportModal' class='btn green' data-toggle='modal' onclick='UploadReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "\",\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.ConfirmedUrl) + "\",\"" + reportLists[i].status + "\")'>重新上传</a>&nbsp;";
                            str1 += "<a href='#renameRptModal' class='btn purple' data-toggle='modal' onclick='RenameReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "\")'>重命名</a>&nbsp;";
                            str1 += "<a href='#deleteRptModal' class='btn ' data-toggle='modal' onclick='DeleteReport(\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportId) + "\",\"" + Microsoft.JScript.GlobalObject.encodeURIComponent(rpt.reportName) + "\")'>删除</a>";
                        }
                    }
                    rptContent.option = str1;
                    reportLists2.Add(rptContent);
                }
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


        public class reportCount
        {
            public int count { get; set; }
        }
        public class reportContent
        {
            public string status { get; set; }
            public List<report> reports { get; set; }
        }

        public class report
        {
            public string reportId { get; set; }
            public string reportName { get; set; }
            public string OrgName { get; set; }
            public string StructName { get; set; }
            public string DateType { get; set; }
            public string ConfirmedDate { get; set; }
            public string ConfirmedUrl { get; set; }
            public string UnconfirmedDate { get; set; }
            public string UnconfirmedUrl { get; set; }
            public bool Confirm { get; set; }
        }
        public class reportContent2
        {
            public string checkbox { get; set; }
            public string reportName { get; set; }
            public string orgName { get; set; }
            public string structName { get; set; }
            public string reportType { get; set; }
            public string date { get; set; }
            public string status { get; set; }
            public string option { get; set; }
        }

        public class reportEnd
        {
            public List<reportContent2> aaData { get; set; }//数组的数组，表格中的实际数据
            public int iTotalRecords { get; set; }//实际的行数
            public int iTotalDisplayRecords { get; set; }//过滤之后，实际的行数
            public int sEcho { get; set; }//来自客户端sEcho的没有变化的复制品
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