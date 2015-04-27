using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureCloud
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// ExcelDownload 的摘要说明
    /// </summary>
    public class ExcelDownload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string Url = context.Request.QueryString["Url"];
            string json = HttpGet(Url);
            json = json.Replace("logTime", "操作时间").Replace("clientType", "设备类型").Replace("content", "操作内容").Replace("parameter", "操作参数").Replace("logLevel", "日志等级").Replace("fileName","文件名").Replace("processName", "进程名").Replace("lineNo", "代码行号").Replace("message", "操作信息").Replace("exception", "异常");

            //DataTable dt = ToDataTable(json);
            JsonToCsv(json);
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

        #region 把json导出到Csv的函数
        /// <summary>
        /// 把json导出到Csv的函数
        /// </summary>
        /// <param name="json"></param>
        public static void JsonToCsv(string json)
        {
            HttpContext curContext = HttpContext.Current;
            IEnumerable<JObject> data = JsonConvert.DeserializeObject<IEnumerable<JObject>>(json);

            if (data.Any())
            {
                curContext.Response.Clear();
                curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=Log.csv");
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}