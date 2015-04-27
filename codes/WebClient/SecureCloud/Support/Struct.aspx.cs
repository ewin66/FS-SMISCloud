using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureCloud.Support
{
    public partial class Struct : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HiddenFlag.Value = Request.QueryString["flag"];
            HiddenOrgName.Value = Request.QueryString["orgName"];
            HiddenOrgId.Value = Request.QueryString["orgId"];
        }
        public static void ShowMsg(System.Web.UI.Page page, string msg)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script language='javascript' defer>alert('" + msg.ToString() + "');</script>");
        }
       
    }
}