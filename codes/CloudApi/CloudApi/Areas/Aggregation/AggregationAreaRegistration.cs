using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Aggregation
{
    using System.Web.Mvc;

    public class AggregationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Aggregation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context.Routes);
        }
    }
}