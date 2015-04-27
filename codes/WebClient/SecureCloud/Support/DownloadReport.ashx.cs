using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SecureCloud.Support
{
    /// <summary>
    /// DownloadReport 的摘要说明
    /// </summary>
    public class DownloadReport : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string fileName = context.Server.HtmlDecode(context.Request.QueryString["fileFullName"]);
            try
            {
                var speed = 0;
                Int32.TryParse(ConfigurationManager.AppSettings["DownloadSpeed"], out speed);

                if (File.Exists(fileName))
                {
                    DownLoad.DownloadFile(context, fileName, speed * 1024);
                }
                else
                {
                    context.Response.Write("<script  language='javascript'>window.alert('待下载的文件被移动或删除！');window.close(); </script>");
                }
            }
            catch (Exception)
            {
                context.Response.Write("<script  language='javascript'>window.alert('下载文件时发生异常!');window.close(); </script>");
               
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