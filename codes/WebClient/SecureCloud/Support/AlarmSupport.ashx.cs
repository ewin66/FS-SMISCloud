using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace SecureCloud.Support
{
    /// <summary>
    /// AlarmSupport 的摘要说明
    /// </summary>
    public class AlarmSupport : IHttpHandler
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
                string urlParams = context.Request.QueryString["Url_params"];
                var jsonParams = JsonConvert.DeserializeObject<AlarmModel>(urlParams);
                string url = context.Request.QueryString["Url"];
                string urlCount = context.Request.QueryString["Url_count"];
                string json_count = HttpPost(urlCount, context, jsonParams);
                WaringCount warningCount = JsonConvert.DeserializeObject<WaringCount>(json_count);

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
                //获得分页的查询结果

                var listWanings = new List<Warning>();
                if (warningCount.count > 0)
                {
                    url += "&startRow=" + startRow + "&endRow=" + endRow;
                    string json = HttpPost(url, context, jsonParams);
                    if (urlStructOrSensor == "struct")
                    {
                        listWanings = HandleStructWarnings(json, urlRole);
                    }
                    else if (urlStructOrSensor == "sensor")
                    {
                        listWanings = HandleSensorWarnings(json, urlRole);
                    }
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

        public List<Warning> ClientHandleStructWarnings(string json)
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
                    listWanings.Add(warn);
                }
            }

            return listWanings;
        }

        public List<Warning> SupportHandleStructWarnings(string json)
        {
            StructWarn structWarnLists = JsonConvert.DeserializeObject<StructWarn>(json);

            List<Warning> listWanings = new List<Warning>();
            for (int j = 0; j < structWarnLists.warnings.Count; j++)
            {
                Warning warn = new Warning();
                warn.warning_source = structWarnLists.warnings[j].source;
                switch (structWarnLists.warnings[j].level)
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
                warn.warning_time = structWarnLists.warnings[j].time.ToString();
                warn.warning_reason = structWarnLists.warnings[j].reason;
                warn.warning_information = structWarnLists.warnings[j].content;
                var dealFlag = structWarnLists.warnings[j].dealFlag;
                switch (dealFlag)
                {
                    case WarningSupportProcessed:
                        warn.warning_dealFlag = "<span style='color: green;'>已确认</span>";
                        if (structWarnLists.warnings[j].confirmor != "")
                        {
                            warn.warning_confirmInfo = "确认人:" + structWarnLists.warnings[j].confirmor
                                + "; 确认内容:" + structWarnLists.warnings[j].suggestion
                                + "; 确认时间:" + Convert.ToString(structWarnLists.warnings[j].confirmTime);
                        }
                        else
                        {
                            warn.warning_confirmInfo = "N/A";
                        }
                        warn.warning_send = "<a style='cursor:pointer; white-space: nowrap;text-decoration: none;' onclick='issueAlert(" + structWarnLists.warnings[j].warningId + ")'>下发至用户</a>";
                        break;
                    case WarningSupportUnprocessed:
                        warn.warning_dealFlag = "<span style='color: red;'>未确认</span>";
                        warn.warning_confirmInfo = "N/A";
                        warn.warning_send = "<a style='cursor:pointer; white-space: nowrap;text-decoration: none;' onclick='confirmAlert(" + structWarnLists.warnings[j].warningId + ")'>确认</a>";
                        warn.warning_send += "&nbsp;|&nbsp; ";
                        warn.warning_send += "<a style=' cursor:pointer; white-space: nowrap;text-decoration: none;' onclick='issueAlert(" + structWarnLists.warnings[j].warningId + ")'>下发至用户</a>";
                        break;
                    case WarningClientProcessed:
                    case WarningClientUnprocessed:
                        warn.warning_dealFlag = "<span style='color: blue;'>已下发</span>";
                        warn.warning_confirmInfo = "N/A";
                        warn.warning_send = "N/A";
                        break;
                    default:
                        warn.warning_dealFlag = "<a></a>";
                        break;
                }
                listWanings.Add(warn);
            }
            return listWanings;
        }

        public List<Warning> HandleStructWarnings(string json, string urlRole)
        {
            List<Warning> listWanings = new List<Warning>();
            if (urlRole == "support")
            {
                listWanings = SupportHandleStructWarnings(json);
            }
            else
            {
                listWanings = ClientHandleStructWarnings(json);
            }

            return listWanings;
        }

        public List<Warning> HandleSensorWarnings(string json, string urlRole)
        {
            var listWanings = new List<Warning>();
            if (urlRole == "support")
            {
                listWanings = SupportHandleSensorWarnings(json);
            }
            else
            {
                listWanings = ClientHandleSensorWarnings(json);
            }
            return listWanings;
        }

        public List<Warning> SupportHandleSensorWarnings(string json)
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

                    case WarningSupportProcessed:
                        warn.warning_dealFlag = "<a class='btn green' style='cursor: text;'>已确认</a>";
                        break;

                    case WarningSupportUnprocessed:
                        warn.warning_dealFlag = "<a class='btn red' onclick='confirmAlert(" + sensorWarnLists[i].warningId + ")'>未确认</a>";
                        warn.warning_send = "<a class='btn red' onclick='confirmAlert(" + sensorWarnLists[i].warningId + ")'>确认</a>";
                        warn.warning_send += "&nbsp; | &nbsp; ";
                        warn.warning_send += "<a class='btn blue' onclick='issueAlert(" + sensorWarnLists[i].warningId + ")'>下发至用户</a>";
                        break;
                    case WarningClientProcessed:
                    case WarningClientUnprocessed:
                        warn.warning_dealFlag = "<a class='btn red' style='cursor: text;'>已下发</a>";
                        warn.warning_send = "N/A";
                        break;
                    default:
                        warn.warning_dealFlag = "<a></a>";
                        break;
                }
                listWanings.Add(warn);
            }
            return listWanings;
        }

        public List<Warning> ClientHandleSensorWarnings(string json)
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
                listWanings.Add(warn);
            }
            return listWanings;
        }

        public static string ParamToString(AlarmModel alarmParams)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("{0}={1}", "FilteredDeviceType", alarmParams.FilteredDeviceType);
            buffer.AppendFormat("&{0}={1}", "FilteredStatus", alarmParams.FilteredStatus);
            buffer.AppendFormat("&{0}={1}", "FilteredLevel", alarmParams.FilteredLevel);
            buffer.AppendFormat("&{0}={1}", "FilteredStartTime", alarmParams.FilteredStartTime);
            buffer.AppendFormat("&{0}={1}", "FilteredEndTime", alarmParams.FilteredEndTime);
            buffer.AppendFormat("&{0}={1}", "OrderedDevice", alarmParams.OrderedDevice);
            buffer.AppendFormat("&{0}={1}", "OrderedLevel", alarmParams.OrderedLevel);
            buffer.AppendFormat("&{0}={1}", "OrderedTime", alarmParams.OrderedTime);
            return buffer.ToString();
        }

        public static string HttpPost(string url, HttpContext context, AlarmModel alarmParams)
        {
            var request = context.Request;
            var response = context.Response;
            try
            {
                var alarmUrl = context.Server.HtmlEncode(url);
                var proxyRequest = (HttpWebRequest)WebRequest.Create(alarmUrl);
                proxyRequest.Method = "Post";
                proxyRequest.UserAgent = request.UserAgent;
                proxyRequest.ContentType = "application/x-www-form-urlencoded";
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                proxyRequest.CachePolicy = noCachePolicy;

                string buffer = ParamToString(alarmParams);
                Encoding encode = Encoding.GetEncoding("utf-8");
                byte[] tempdata = encode.GetBytes(buffer);
                proxyRequest.ContentLength = tempdata.Length;
                Stream stream = proxyRequest.GetRequestStream();
                stream.Write(tempdata, 0, tempdata.Length);
                stream.Close();

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

    public class AlarmCount
    {
        public int count { get; set; }
    }
    public class AlarmModel
    {
        public string FilteredDeviceType { get; set; }
        public string FilteredStatus { get; set; }
        public string FilteredLevel { get; set; }
        public string FilteredStartTime { get; set; }
        public string FilteredEndTime { get; set; }
        public string OrderedDevice { get; set; }
        public string OrderedLevel { get; set; }
        public string OrderedTime { get; set; }
    }
}