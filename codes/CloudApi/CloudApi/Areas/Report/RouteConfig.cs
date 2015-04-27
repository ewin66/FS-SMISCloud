namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Report
{
    using System.Web.Http;
    using System.Web.Routing;

    /// <summary>
    /// DATA路由配置
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 路由配置
        /// </summary>
        /// <param name="routes">路由集合</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "GetReportListByOrgId",
                routeTemplate: "org/{orgId}/report/{dateType}",
                defaults: new { controller = "Report", action = "GetReportListByOrgId" },
                constraints: new { orgId = @"^\d+$", dateType = @"^(day)|(week)|(month)|(year)$" });

            routes.MapHttpRoute(
                name: "GetReportListByStructId",
                routeTemplate: "struct/{structId}/report/{dateType}",
                defaults: new { controller = "Report", action = "GetReportListByStructId" },
                constraints: new { structId = @"^\d+$", dateType = @"^(day)|(week)|(month)|(year)$" });
            /* 报表管理 */
            //删除报表信息
            routes.MapHttpRoute(
              name: "RemoveReportInfo",
              routeTemplate: "report/remove/{rptId}",
              defaults: new { controller = "Report", action = "RemoveReportInfo" }

              );
            //更新报表信息
            routes.MapHttpRoute(
               name: "UpdateReportInfo",
               routeTemplate: "report/update/{rptId}",
               defaults: new { controller = "Report", action = "UpdateReportInfo" }
              );
            //查询报表详细信息
            routes.MapHttpRoute(
               name: "GetReportInfoById",
               routeTemplate: "report/info/{rptId}",
               defaults: new { controller = "Report", action = "GetReportInfoById" }
              );

            /**************************人工上传报表start*****************************************/
            //根据报表名称删除报表信息
            routes.MapHttpRoute(
              name: "RemoveReportInfoByName",
              routeTemplate: "report/delete",
              defaults: new { controller = "Report", action = "RemoveReportInfoByName" }

              );
            //增加报表
            routes.MapHttpRoute(
               name: "AddReport",
               routeTemplate: "report/add",
               defaults: new { controller = "Report", action = "AddReport" }
              );
            //获取需要管理的报表
            routes.MapHttpRoute(
                name: "GetManagedRpt",
                routeTemplate: "user/{userId}/report/managedRpt-list",
                defaults: new { controller = "Report", action = "GetManagedRpt" },
                constraints: new { userId = @"^\d+$" }
               );
            //获取需要管理的报表(按照状态分组)
            routes.MapHttpRoute(
                name: "GetManagedRptGroupByStatus",
                routeTemplate: "user/{userId}/report/orderManagedRpt-list",
                defaults: new { controller = "Report", action = "GetManagedRptGroupByStatus" },
                constraints: new { userId = @"^\d+$" }
               );
            //获取需要管理的报表记录数目
            routes.MapHttpRoute(
                name: "GetManagedRptCount",
                routeTemplate: "user/{userId}/report/manualRpt-count",
                defaults: new { controller = "Report", action = "GetManagedRptCount" },
                constraints: new { userId = @"^\d+$" }
              );
            //报表重命名
            routes.MapHttpRoute(
               name: "RenameReport",
               routeTemplate: "report/rename/{rptId}",
               defaults: new { controller = "Report", action = "RenameReport" }
               );

            /**************************人工上传报表end******************************************/
            /* 报表配置 */
            //获取报表配置列表
            routes.MapHttpRoute(
                name: "GetReportConfigList",
                routeTemplate: "user/{userId}/reportConfig/list",
                defaults: new { controller = "RptConfig", action = "GetReportConfigList" },
                constraints: new { userId = @"^\d+$" }
               );

            //删除报表配置信息
            routes.MapHttpRoute(
              name: "RemoveReportConfigInfo",
              routeTemplate: "reportConfig/remove/{Id}",
              defaults: new { controller = "RptConfig", action = "RemoveReportConfigInfo" },
              constraints: new { Id = @"^\d+$" }

              );
            //增加报表配置信息
            routes.MapHttpRoute(
               name: "AddReportConfigInfo",
               routeTemplate: "reportConfig/add",
               defaults: new { controller = "RptConfig", action = "AddReportConfigInfo" }
              );

            // 修改报表配置信息
            routes.MapHttpRoute(
                name: "ModifyReportConfigInfo",
                routeTemplate: "reportConfig/modify-info/{Id}",
                defaults: new { controller = "RptConfig", action = "ModifyReportConfigInfo" },
                constraints: new { Id = @"^\d+$" }
                );

            // 获取单个配置的详细信息
            routes.MapHttpRoute(
                name: "GetReportConfigInfo",
                routeTemplate: "reportConfig/info/{Id}",
                defaults: new { controller = "RptConfig", action = "GetReportConfigInfo" },
                constraints: new { Id = @"^\d+$" }
                );
            //获取组织报表记录数目
            routes.MapHttpRoute(
                name: "GetReportCountByOrgId",
                routeTemplate: "org/{orgId}/report-count/{dateType}",
                defaults: new { controller = "Report", action = "GetReportCountByOrgId" },
                constraints: new { orgId = @"^\d+$", dateType = @"^(day)|(week)|(month)|(year)$" });

            //获取结构物报表记录数目
            routes.MapHttpRoute(
                name: "GetReportCountByStructId",
                routeTemplate: "struct/{structId}/report-count/{dateType}",
                defaults: new { controller = "Report", action = "GetReportCountByStructId" },
                constraints: new { structId = @"^\d+$", dateType = @"^(day)|(week)|(month)|(year)$" });

            //获取报表模板列表
            routes.MapHttpRoute(
                name: "GetReportTemplateList",
                routeTemplate: "template/list/{dateType}",
                defaults: new { controller = "RptConfig", action = "GetReportTemplateList" },
                constraints: new { dateType = @"^(day)|(week)|(month)|(year)|(all)$" });

        }
    }
}