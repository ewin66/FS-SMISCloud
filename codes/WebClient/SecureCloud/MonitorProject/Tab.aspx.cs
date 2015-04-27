using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureCloud.MonitorProject
{
    public partial class Tab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HiddenThemeNo.Value = Request.QueryString["themeId"];
            if (Request.QueryString.AllKeys.Contains("factorId"))
            {
                HiddenFactorId.Value = Request.QueryString["factorId"];
            }
            else
            {
                HiddenFactorId.Value = string.Empty;
            }
            if (Request.QueryString.AllKeys.Contains("sensorId"))
            {
                HiddenSensorId.Value = Request.QueryString["sensorId"];
            }
            else
            {
                HiddenSensorId.Value = string.Empty;
            }
        }
    }
}