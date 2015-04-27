using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureCloud.MonitorProject
{
    public partial class OneChar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HiddenFactorNo.Value = Request.QueryString["factorid"];
            HiddenSensorId.Value = Request.QueryString["sensorId"];
        }
    }
}