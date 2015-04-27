using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace SecureCloud
{
    /// <summary>
    /// datable 的摘要说明
    /// </summary>
    public class datable : IHttpHandler
    {
        private const int WarningSupportUnprocessed = 1;
        private const int WarningSupportProcessed = 2;
        private const int WarningClientUnprocessed = 3;
        private const int WarningClientProcessed = 4;

        public void ProcessRequest(HttpContext context)
        {
            string logPath = context.Server.MapPath("Test.txt");
            if (!File.Exists(logPath))
            {
                File.Create(logPath);
            }
            try
            {
                var urlRole = context.Request.QueryString["role"];
                var urlStructOrSensor = context.Request.QueryString["ss"];
                string urlCount = context.Request.QueryString["Url_count"];
                string jsonCount = HttpGet(urlCount);
                WaringCount warningCount = JsonConvert.DeserializeObject<WaringCount>(jsonCount);

                string iDisplayStart = context.Request.QueryString["iDisplayStart"];
                string iDisplayLength = context.Request.QueryString["iDisplayLength"];
                int sEcho = Convert.ToInt32(context.Request.QueryString["sEcho"]);
                int startRow = Convert.ToInt32(iDisplayStart) + 1;
                int endRow;
                if (Convert.ToInt32(iDisplayLength) == -1)
                {
                    endRow = startRow + warningCount.count - 1;
                }
                else
                {
                    endRow = startRow + Convert.ToInt32(iDisplayLength) - 1;
                }
                string url = context.Request.QueryString["Url"];
                url += "&startRow=" + startRow + "&endRow=" + endRow;

                string json = HttpGet(url);

                var listWanings = new List<Warning>();
                if (urlStructOrSensor == "struct")
                {
                    listWanings = HandleStructWarnings(json, urlRole);
                }
                else if (urlStructOrSensor == "sensor")
                {
                    listWanings = HandleSensorWarnings(json, urlRole);
                }

                TestWarning testWarning = new TestWarning
                {
                    aaData = listWanings,
                    iTotalRecords = warningCount.count,
                    iTotalDisplayRecords = warningCount.count,
                    sEcho = sEcho
                };

                string jsonWarning = JsonConvert.SerializeObject(testWarning);
                context.Response.Write(jsonWarning);
            }
            catch (Exception e) 
            {
                using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(e.Message);
                    }
                }
            }
        }

        public List<Warning> HandleStructWarnings(string json, string urlRole)
        {
            IList<StructWarn> structWarnLists = JsonConvert.DeserializeObject<IList<StructWarn>>(json);

            List<Warning> listWanings = new List<Warning>();
            for (int i = 0; i < structWarnLists.Count; i++)
            {
                for (int j = 0; j < structWarnLists[i].warnings.Count; j++)
                {
                    Warning warn = new Warning();
                    warn.warning_checkbox = "<input type='checkbox' class='checkboxes' value='" + structWarnLists[i].warnings[j].warningId + "' />";
                    warn.warning_source = structWarnLists[i].warnings[j].source;
                    switch (structWarnLists[i].warnings[j].level)
                    {
                        case 1:
                            warn.warning_level = "<span class='label label-red'>一级</span>";
                            break;
                        case 2:
                            warn.warning_level = "<span class='label label-orange'>二级</span>";
                            break;
                        case 3:
                            warn.warning_level = "<span class='label label-purple'>三级</span>";
                            break;
                        default:
                            warn.warning_level = "<span class='label label-blue'>四级</span>";
                            break;
                    }
                    warn.warning_time = structWarnLists[i].warnings[j].time.ToString();
                    warn.warning_typeID = structWarnLists[i].warnings[j].warningTypeId;
                    warn.warning_reason = structWarnLists[i].warnings[j].reason;
                    warn.warning_information = structWarnLists[i].warnings[j].content;
                    if (structWarnLists[i].warnings[j].confirmor != "")
                    {
                        warn.warning_confirmInfo = "确认人:" + structWarnLists[i].warnings[j].confirmor
                            + "; 确认内容:" + structWarnLists[i].warnings[j].suggestion
                            + "; 确认时间:" + Convert.ToString(structWarnLists[i].warnings[j].confirmTime);
                    }
                    else 
                    {
                        warn.warning_confirmInfo = "N/A";
                    }
                    
                    var dealFlag = structWarnLists[i].warnings[j].dealFlag;
                    switch (dealFlag)
                    {
                        case WarningClientProcessed:
                        case WarningSupportProcessed:
                            warn.warning_dealFlag = "<a class='btn green' style='cursor: text;'>已确认</a>";
                            break;
                        case WarningClientUnprocessed:
                        case WarningSupportUnprocessed:
                            warn.warning_dealFlag = "<a class='btn red' onclick='confirmAlert(" + structWarnLists[i].warnings[j].warningId + ")'>未确认</a>";
                            break;
                        default:
                            warn.warning_dealFlag = "<a></a>";
                            break;
                    }
                    if (urlRole == "support")
                    {
                        warn.warning_send = "<a class='btn blue' onclick='issueAlert(" + structWarnLists[i].warnings[j].warningId + ")'>下发</a>";
                    }
                    listWanings.Add(warn);
                }
            }
            return listWanings;
        }

        public List<Warning> HandleSensorWarnings(string json, string urlRole)
        {
            var sensorWarnLists = JsonConvert.DeserializeObject<IList<Warn>>(json);

            var listWanings = new List<Warning>();
            for (int i = 0; i < sensorWarnLists.Count; i++)
            {
                var warn = new Warning
                {
                    warning_checkbox =
                        "<input type='checkbox' class='checkboxes' value='" + sensorWarnLists[i].warningId +
                        "' />",
                    warning_source = sensorWarnLists[i].Location + "-" + sensorWarnLists[i].ProductName
                };
                switch (sensorWarnLists[i].level)
                {
                    case 1:
                        warn.warning_level = "<span class='label label-red'>一级</span>";
                        break;
                    case 2:
                        warn.warning_level = "<span class='label label-orange'>二级</span>";
                        break;
                    case 3:
                        warn.warning_level = "<span class='label label-purple'>三级</span>";
                        break;
                    default:
                        warn.warning_level = "<span class='label label-blue'>四级</span>";
                        break;
                }
                warn.warning_time = sensorWarnLists[i].time.ToString();
                warn.warning_typeID = sensorWarnLists[i].warningTypeId;
                warn.warning_reason = sensorWarnLists[i].reason;
                warn.warning_information = sensorWarnLists[i].content;
                if (sensorWarnLists[i].confirmor != "")
                {
                    warn.warning_confirmInfo = "确认人:" + sensorWarnLists[i].confirmor
                        + "; 确认内容:" + sensorWarnLists[i].suggestion
                        + "; 确认时间:" + Convert.ToString(sensorWarnLists[i].confirmTime);
                }
                else
                {
                    warn.warning_confirmInfo = "N/A";
                }
                var dealFlag = sensorWarnLists[i].dealFlag;
                switch (dealFlag)
                {
                    case WarningClientProcessed:
                    case WarningSupportProcessed:
                        warn.warning_dealFlag = "<a class='btn green' style='cursor: text;'>已确认</a>";
                        break;
                    case WarningClientUnprocessed:
                    case WarningSupportUnprocessed:
                        warn.warning_dealFlag = "<a class='btn red' onclick='confirmAlert(" + sensorWarnLists[i].warningId + ")'>未确认</a>";
                        break;
                    default:
                        warn.warning_dealFlag = "<a></a>";
                        break;
                }
                if (urlRole == "support")
                {
                    warn.warning_send = "<a class='btn blue' onclick='issueAlert(" + sensorWarnLists[i].warningId + ")'>下发</a>";
                }
                listWanings.Add(warn);
            }
            return listWanings;
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

   

    public class TestWarning
    {
        public List<Warning> aaData { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public int sEcho { get; set; }
    }

    public class StructWarn 
    {
        public int structId { get; set; }

        public string structName { get; set; }

        public List<Warn> warnings { get; set; }
    }

    public class Warn 
    {
        public int warningId { get; set; }

        public string warningTypeId { get; set; }

        public string source { get; set; }

        // use for sensor: source = location + productName .
        public string Location { get; set; }
        public string ProductName { get; set; }

        public int level { get; set; }

        public string content { get; set; }

        public string reason { get; set; }

        public DateTime time { get; set; }

        public int dealFlag { get; set; }

        public string confirmor { get; set; }

        public string suggestion { get; set; }

        public DateTime confirmTime { get; set; }
    }

    public class Warning
    {
        public string warning_checkbox { get; set; }
        public string warning_source { get; set; }
        public string warning_level { get; set; }
        public string warning_time { get; set; }
        public string warning_typeID { get; set; }
        public string warning_reason { get; set; }
        public string warning_information { get; set; }
        public string warning_confirmInfo { get; set; }
        public string warning_dealFlag { get; set; }
        public string warning_send { get; set; }
    }

    public class WaringCount
    {
        public int count { get; set; }
    }
}