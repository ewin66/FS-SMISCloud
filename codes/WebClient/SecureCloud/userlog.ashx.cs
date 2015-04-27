using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace SecureCloud
{
    /// <summary>
    /// userlog 的摘要说明
    /// </summary>
    public class userlog : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //先取总长度
            string Url_count = context.Request.QueryString["Url_count"];
            string json_count = HttpGet(Url_count);
            UserLogCount userlogcount = JsonConvert.DeserializeObject<UserLogCount>(json_count);
            //搜索条件
            string sSearch = context.Request.QueryString["sSearch"];
            string josn_count_sSearch = HttpGet(Url_count + "&keyWords="+sSearch);
            UserLogCount userlogcount_sSearch = JsonConvert.DeserializeObject<UserLogCount>(josn_count_sSearch);

            string iDisplayStart = context.Request.QueryString["iDisplayStart"];
            string iDisplayLength = context.Request.QueryString["iDisplayLength"];
            
            int sEcho = Convert.ToInt32(context.Request.QueryString["sEcho"]);
            int startRow = Convert.ToInt32(iDisplayStart) + 1;
            int endRow;
            if (Convert.ToInt32(iDisplayLength) == -1)
            {
                endRow = startRow + userlogcount.count - 1;
            }
            else {
                endRow = startRow + Convert.ToInt32(iDisplayLength) - 1;
            }          
            string Url = context.Request.QueryString["Url"];
            Url += "&startRow=" + startRow + "&endRow=" + endRow+"&keyWords="+sSearch;
            
            string json = HttpGet(Url);

            IList<UserContent> UserLogLists = JsonConvert.DeserializeObject<IList<UserContent>>(json);

            

            List<UserContent2> UserLogLists2 = new List<UserContent2>();

            for (int i = 0; i < UserLogLists.Count; i++) {
                UserContent2 userlog = new UserContent2();
                userlog.userlog_time = UserLogLists[i].logTime.ToString();
                userlog.userlog_clientType = UserLogLists[i].clientType;
                userlog.userlog_content = UserLogLists[i].content;
                if (UserLogLists[i].parameter == "" || UserLogLists[i].parameter == null || UserLogLists[i].parameter.Length==0)
                {
                    userlog.userlog_parameter = "无";
                }
                else if (UserLogLists[i].parameter.Length > 50)
                {
                    userlog.userlog_parameter = "<span data-toggle='tooltip' data-placement='bottom' title='" + UserLogLists[i].parameter + "'>" + UserLogLists[i].parameter.Substring(0, 50) + "<a href='#' onclick='javascript:showMsg(\"" + UserLogLists[i].parameter + "\");'>...</a></span>";
                    //userlog.userlog_parameter = UserLogLists[i].parameter.Substring(0, 50) + "<a href='#' data-toggle='tooltip' data-placement='bottom' title='" + UserLogLists[i].parameter + "' onclick='javascript:showMsg(\"" + UserLogLists[i].parameter + "\");'>...</a>";
                }
                else {
                    userlog.userlog_parameter = UserLogLists[i].parameter;
                }         
                UserLogLists2.Add(userlog);
            }

            UserLogEnd userlogend = new UserLogEnd
            {
                aaData = UserLogLists2,
                iTotalRecords = userlogcount.count,
                iTotalDisplayRecords = userlogcount_sSearch.count,
                sEcho = sEcho
            };

            string json_userlog = JsonConvert.SerializeObject(userlogend);
            context.Response.Write(json_userlog);

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

    public class UserContent
    {
        public DateTime logTime { get; set; }
        public string clientType { get; set; }
        public string content { get; set; }
        public string parameter { get; set; }
    }

    public class UserContent2
    {
        public string userlog_time { get; set; }
        public string userlog_clientType { get; set; }
        public string userlog_content { get; set; }
        public string userlog_parameter { get; set; }
    }

    public class UserLogCount
    {
        public int count { get; set; }
    }

    public class UserLogEnd
    {
        public List<UserContent2> aaData { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public int sEcho { get; set; }
    }
}