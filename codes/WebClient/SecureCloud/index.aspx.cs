using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureCloud
{
	public partial class index : System.Web.UI.Page
	{      
		protected void Page_Load(object sender, EventArgs e)
		{          
            reportpath.Value = ConfigurationManager.AppSettings["ReportsPath"];
		}
	}
}