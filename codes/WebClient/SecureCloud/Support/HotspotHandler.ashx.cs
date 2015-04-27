using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecureCloud.Auth;

namespace SecureCloud.Support
{
    /// <summary>
    /// HotspotHandler 的摘要说明
    /// </summary>
    [Authorization(AuthCode.S_Org_Logo_Upload)]
    public class HotspotHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                string orgId=context.Request.QueryString["orgId"];
                if (orgId == null || orgId == ""||orgId=="-1")
                {
                    string uploadingStruID = context.Request.Params["uploadingStruID"];
                    string uploadingStruName = context.Request.Params["uploadingStruName"];
                    context.Response.ContentType = "text/html";
                    HttpFileCollection files = HttpContext.Current.Request.Files;

                    //string filename = files[0].FileName.Substring(files[0].FileName.LastIndexOf("\\") + 1);
                    string filename = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."));
                    filename = "topo-" + uploadingStruID + "-" + uploadingStruName + filename;
                    //string filename = "800.jpg";

                    //将文件保存在网站目录中  
                    files[0].SaveAs(context.Server.MapPath("\\resource\\img\\Topo\\") + filename);
                    //返回用json数据格式表示的提示  
                    string result = "[" + "\"" + filename.ToString() + "\"" + "]";

                    context.Response.Write(result);
                }
                else
                {
                    string orgName = context.Request.Params["orgName"];
                    context.Response.ContentType = "text/html";
                    HttpFileCollection files = HttpContext.Current.Request.Files;
                  
                    string fileType = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."));
                    string filename = "logo-" + orgId + "-" + orgName + fileType;

                    files[0].SaveAs(context.Server.MapPath("\\resource\\img\\OrgLogo\\") + filename);

                    string result = "[" + "\"" + filename.ToString() + "\"" + "]";

                    context.Response.Write(result);
                }
                
            }
            catch (Exception e) {
                context.Response.Write(e.Message);
                context.Response.Write(e.StackTrace);
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