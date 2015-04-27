using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SecureCloud.Auth;

namespace SecureCloud.Support
{
    /// <summary>
    /// SectionHeapMapHandler 的摘要说明
    /// </summary>
    [Authorization(AuthCode.S_Structure_Construct_Section_Add)]
    public class SectionHeapMapHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                string structId = context.Request.Params["uploadingStructId"];
                string guid = context.Request.Params["uploadingGuid"];
                string sectionName = context.Request.Params["uploadingSectionName"];

                context.Response.ContentType = "text/html";
                HttpFileCollection files = HttpContext.Current.Request.Files;
                string fileSuffix = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."));
                string fileName = "topo-" + structId + "-" + guid + "-" + sectionName + fileSuffix; // 截面图片命名规则：topo-结构物id-Guid-截面名称.后缀
                //将文件保存在网站目录中  
                files[0].SaveAs(context.Server.MapPath("\\resource\\img\\Topo\\") + fileName);
                //返回用json数据格式表示的提示  
                string result = "[" + "\"" + fileName + "\"" + "]";

                context.Response.Write(result);
            }
            catch (Exception e)
            {
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