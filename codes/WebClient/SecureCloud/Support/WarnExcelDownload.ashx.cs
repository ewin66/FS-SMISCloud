using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SecureCloud.Support
{
    /// <summary>
    /// WarnExcelDownload 的摘要说明
    /// </summary>
    public class WarnExcelDownload : IHttpHandler
    {
        private const int WarningSupportUnprocessed = 1;
        private const int WarningSupportProcessed = 2;
        private const int WarningClientUnprocessed = 3;
        private const int WarningClientProcessed = 4;
        public void ProcessRequest(HttpContext context)
        {
            string url = context.Request.QueryString["Url"];
            string urlParams = context.Request.QueryString["Url_params"];
            var jsonParams = JsonConvert.DeserializeObject<AlarmModel>(urlParams);
            string json = AlarmSupport.HttpPost(url, context, jsonParams);
            StructWarn structWarnLists = JsonConvert.DeserializeObject<StructWarn>(json);
            List<Warn> structWarn = structWarnLists.warnings;
            List<WarningInfo>  warnList = new List<WarningInfo>();
            foreach (Warn warn in structWarn)
            {
                WarningInfo warnInfo = new WarningInfo();
                warnInfo.warning_source = warn.source;
                warnInfo.warning_level = warn.level.ToString();
                warnInfo.warning_time = warn.time.ToString();
                warnInfo.warning_reason = warn.reason;
                warnInfo.warning_information = warn.content;
                switch (warn.dealFlag)
                {
                    case WarningSupportUnprocessed:
                        warnInfo.warning_dealFlag = "未处理";
                        warnInfo.warning_confirmInfo = "N/A";
                        warnInfo.warning_send = "确认 | 下发至用户";
                        break;
                    case WarningSupportProcessed:
                        warnInfo.warning_dealFlag = "已确认";
                        warnInfo.warning_confirmInfo = "确认人:" + warn.confirmor + "  确认信息: " + warn.suggestion + "  确认时间: " +
                                                       warn.confirmTime.ToString();
                        warnInfo.warning_send = "下发至用户";
                        break;
                    case  WarningClientProcessed:
                    case  WarningClientUnprocessed:
                        warnInfo.warning_dealFlag = "已下发";
                        warnInfo.warning_confirmInfo = "N/A";
                        warnInfo.warning_send = "N/A";
                        break;
                    default:
                        warnInfo.warning_dealFlag = "";
                        warnInfo.warning_confirmInfo = "";
                        warnInfo.warning_send = "";
                        break;
                }
               warnList.Add(warnInfo);
            }

            var ListToJson = new JArray(
                warnList.Select(
                 d => new JObject(
                     new JProperty("告警源", d.warning_source),
                     new JProperty("等级", d.warning_level),
                     new JProperty("产生时间", d.warning_time),
                     new JProperty("可能原因", d.warning_reason),
                     new JProperty("告警信息", d.warning_information),
                     new JProperty("状态", d.warning_dealFlag),
                     new JProperty("确认信息", d.warning_confirmInfo),
                     new JProperty("操作", d.warning_send)
                     )));
            JsonToCsv(ListToJson.ToString());
        }

        #region 把json导出到Csv的函数
        /// <summary>
        /// 把json导出到Csv的函数
        /// </summary>
        /// <param name="json"></param>
        public  void JsonToCsv(string json)
        {
            HttpContext curContext = HttpContext.Current;
            IEnumerable<JObject> data = JsonConvert.DeserializeObject<IEnumerable<JObject>>(json);

            if (data.Any())
            {
                curContext.Response.Clear();
                curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=告警管理.csv");
                curContext.Response.ContentType = "application/ms-excel";//"application/vnd.ms-excel";
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("GB2312");

                //导出Excel文件
                StringWriter sw = new StringWriter();

                // 写入表头
                StringBuilder sb = new StringBuilder();
                foreach (var prop in data.First())
                {
                    var key = prop.Key;
                    sb.Append(key.Replace(",", ";")).Append(",");
                }
                sw.WriteLine(sb);

                // 写入内容
                int i = 0;
                foreach (var o in data)
                {
                    sb = new StringBuilder();
                    foreach (var prop in o)
                    {
                        var value = prop.Value != null ? (prop.Value as JValue).Value : null;
                        sb.Append(value.ToString().Replace(",", ";")).Append(",");
                    }
                    sw.WriteLine(sb);
                    i++;
                }

                curContext.Response.Write(sw);
                curContext.Response.End();
            }
            else
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('没有可导出的数据');window.close();</script>");
            }
        }
        #endregion

        public class WarningInfo
        {
            public string warning_source { get; set; }
            public string warning_level { get; set; }
            public string warning_time { get; set; }
            public string warning_reason { get; set; }
            public string warning_information { get; set; }
            public string warning_dealFlag { get; set; }
            public string warning_confirmInfo { get; set; }
            public string warning_send { get; set; }
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