using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureCloud
{
    public partial class DataWarningTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HiddenSensorId.Value = Request.QueryString["sensorId"];
            HiddenStartTime.Value = Request.QueryString["startTime"];
            HiddenEndTime.Value=Request.QueryString["endTime"];
            HiddenSensorLocation.Value = Request.QueryString["sensorLocation"];

        }
    }
}