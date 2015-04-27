using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
using System.Text;
using NetMQ.zmq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Quartz;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Report.Controllers
{
    public class RptConfigController : ApiController
    {
        private Dictionary<string, int> dateMap = new Dictionary<string, int>
        {
            {"day", 1},
            {"week", 2},
            {"month", 3},
            {"year", 4},
            {"all", 5}
        };
        /// <summary>
        /// 获取报表模板列表 template/list/{dateType}
        /// </summary>
        /// <param name="dateType"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取报表模板列表", false)]
        [Authorization(AuthorizationCode.S_Report)]
        public object GetReportTemplateList(string dateType)
        {
            using (var db = new SecureCloud_Entities())
            {
                int dateTypeId = dateMap[dateType];
                var query = from u in db.T_REPORT_TEMPLATE
                            select
                                new
                                {
                                    id = u.Id,
                                    name = u.Name,
                                    handlerName = u.HandleName,
                                    factorId = u.FactorId,
                                    type = u.Type,
                                    des = u.Description
                                };
                if (dateTypeId != 5)
                {
                    query = query.Where(
                        l => l.type == dateTypeId
                        );
                }
                var data = query.ToList();
                return
                    data.OrderBy(o => o.id)
                        .Select(
                        r =>
                            new
                            {
                                Id = r.id,
                                Name = r.name,
                                HandleName = r.handlerName,
                                FactorId = r.factorId,
                                Type = r.type,
                                Des = r.des
                            }
                        );
            }

        }

        public object SupportGetReportConfigList()
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                     var query =
                            from o in db.T_REPORT_CONFIG
                            from r in db.T_DIM_ORGANIZATION
                            from st in db.T_DIM_STRUCTURE
                            from c in db.T_REPORT_CONFIG_TEMPLATE
                            from te in db.T_REPORT_TEMPLATE
                            where (r.IsDeleted == false || r.IsDeleted == null)
                                   && o.OrgId == r.ID
                                  && o.StructId == st.ID && st.IsDelete == 0
                                  && o.Id == c.ReportConfigId && c.ReportTemplateId == te.Id
                            select
                                new
                                {
                                    id = o.Id,
                                    orgId = (int?)r.ID,
                                    orgName = r.ABB_NAME_CN,
                                    structId = (int?)st.ID,
                                    structName = st.STRUCTURE_NAME_CN,
                                    reportName = o.ReportName,
                                    templateId = (int?)te.Id,
                                    templateName = te.Name,
                                    templateDes = te.Description,
                                    dateType = o.DateType,
                                    createInterval = o.CreateInterval,
                                    getDataTime = o.GetDataTime,
                                    needConfirm = o.NeedConfirmed,
                                    isEnabled = o.IsEnabled
                                };
                        if (!query.Any())
                        {
                            return null;
                        }
                        var data = query.ToList();
                        return
                            data.GroupBy(
                                d =>
                                    new
                                    {
                                        d.id,
                                        d.reportName,
                                        d.dateType,
                                        d.createInterval,
                                        d.isEnabled,
                                        d.needConfirm,
                                        d.getDataTime
                                    })
                                .Select(
                                    g =>
                                        new
                                        {
                                            Id = g.Key.id,
                                            ReportName = g.Key.reportName,
                                            DateType = g.Key.dateType,
                                            DateTypeLabel = ((ReportDateType)g.Key.dateType).ToString(),
                                            CreateInterval = g.Key.createInterval,
                                            GetDataTime = g.Key.getDataTime,
                                            NeedConfirmed = g.Key.needConfirm,
                                            IsEnabled = g.Key.isEnabled,
                                            Org = g.Select(o => o.orgId).FirstOrDefault() != null
                                                ? g.Select(o => new { id = o.orgId, name = o.orgName }).Distinct()
                                                : null,
                                            Struct =
                                                g.Select(o => o.structId).FirstOrDefault() != null
                                                    ? g.Select(s => new { id = s.structId, name = s.structName })
                                                        .Distinct()
                                                    : null,
                                            Template = g.Select(o => o.templateId).FirstOrDefault() != null
                                                ? g.Select(
                                                    o =>
                                                        new
                                                        {
                                                            id = o.templateId,
                                                            name = o.templateName,
                                                            des = o.templateDes
                                                        }).Distinct()
                                                : null
                                        });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ClientGetReportConfigList(int userId)
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    var query = from org in db.T_DIM_USER_ORG
                                from o in db.T_REPORT_CONFIG
                                from r in db.T_DIM_ORGANIZATION
                                from st in db.T_DIM_STRUCTURE
                                from c in db.T_REPORT_CONFIG_TEMPLATE
                                from te in db.T_REPORT_TEMPLATE
                                where org.USER_NO == userId && (r.IsDeleted == false || r.IsDeleted == null)
                                      && org.ORGANIZATION_ID == o.OrgId && o.OrgId == r.ID
                                      && o.StructId == st.ID && st.IsDelete == 0
                                      && o.Id == c.ReportConfigId && c.ReportTemplateId == te.Id
                                select
                                    new
                                    {
                                        id = o.Id,
                                        orgId = (int?)r.ID,
                                        orgName = r.ABB_NAME_CN,
                                        structId = (int?)st.ID,
                                        structName = st.STRUCTURE_NAME_CN,
                                        reportName = o.ReportName,
                                        templateId = (int?)te.Id,
                                        templateName = te.Name,
                                        templateDes = te.Description,
                                        dateType = o.DateType,
                                        createInterval = o.CreateInterval,
                                        getDataTime = o.GetDataTime,
                                        needConfirm = o.NeedConfirmed,
                                        isEnabled = o.IsEnabled
                                    };
                    if (!query.Any())
                    {
                        return null;
                    }
                    var data = query.ToList();
                    return
                        data.GroupBy(
                            d =>
                                new
                                {
                                    d.id,
                                    d.reportName,
                                    d.dateType,
                                    d.createInterval,
                                    d.isEnabled,
                                    d.needConfirm,
                                    d.getDataTime
                                })
                            .Select(
                                g =>
                                    new
                                    {
                                        Id = g.Key.id,
                                        ReportName = g.Key.reportName,
                                        DateType = g.Key.dateType,
                                        DateTypeLabel = ((ReportDateType)g.Key.dateType).ToString(),
                                        CreateInterval = g.Key.createInterval,
                                        GetDataTime = g.Key.getDataTime,
                                        NeedConfirmed = g.Key.needConfirm,
                                        IsEnabled = g.Key.isEnabled,
                                        Org = g.Select(o => o.orgId).FirstOrDefault() != null
                                            ? g.Select(o => new { id = o.orgId, name = o.orgName }).Distinct()
                                            : null,
                                        Struct =
                                            g.Select(o => o.structId).FirstOrDefault() != null
                                                ? g.Select(s => new { id = s.structId, name = s.structName })
                                                    .Distinct()
                                                : null,
                                        Template = g.Select(o => o.templateId).FirstOrDefault() != null
                                            ? g.Select(
                                                o =>
                                                    new
                                                    {
                                                        id = o.templateId,
                                                        name = o.templateName,
                                                        des = o.templateDes
                                                    }).Distinct()
                                            : null
                                    });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取报表配置列表  user/{userId}/reportConfig/list
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取报表配置列表", false)]
        [Authorization(AuthorizationCode.S_Report)]
        public object GetReportConfigList(int userId)
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    var ur = (from r in db.T_DIM_ROLE
                              from u in db.T_DIM_USER
                              where u.ROLE_ID == r.ROLE_ID
                               && u.USER_NO == userId
                               && u.USER_IS_ENABLED
                              select u.ROLE_ID);
                    if (!ur.ToList().Any())
                    {
                        return null;
                    }
                    if (ur.FirstOrDefault() == 1)
                    {
                        return SupportGetReportConfigList();
                    }
                     return ClientGetReportConfigList(userId);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 删除报表配置信息  reportConfig/remove/{Id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除报表配置信息", true)]
        [Authorization(AuthorizationCode.S_Report_Config_Delete)]
        public HttpResponseMessage RemoveReportConfigInfo([FromUri] int id)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var config = db.T_REPORT_CONFIG.FirstOrDefault(u => u.Id == id);

                    if (config == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表配置不存在或已被删除"));
                    }

                    var entry = db.Entry(config);
                    entry.State = System.Data.EntityState.Deleted;


                    IQueryable<T_REPORT_CONFIG_TEMPLATE> congigTemp = from q in db.T_REPORT_CONFIG_TEMPLATE
                                                                      where q.ReportConfigId == id
                                                                      select q;
                    foreach (var item in congigTemp)
                    {
                        var entry2 = db.Entry(item);
                        entry2.State = System.Data.EntityState.Deleted;
                    }


                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = "Id:" + id;
                    this.Request.Properties["ActionParameterShow"] =
                        string.Format("组织编号：{0}, 结构物编号： {1}, 报表名称：{2}",
                        config.OrgId,
                        config.StructId,
                        config.ReportName
                        );
                    #endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("报表配置删除成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表配置删除失败"));
                }
            }
        }

        /// <summary>
        /// 增加报表配置信息  reportConfig/add
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        [LogInfo("增加报表配置信息", true)]
        [Authorization(AuthorizationCode.S_Report_Config_Add)]
        public HttpResponseMessage AddReportConfigInfo([FromBody] ReportConfig rptConfig)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var us = new T_REPORT_CONFIG
                    {
                        OrgId = rptConfig.OrgId,
                        StructId = rptConfig.StructId,
                        ReportName = rptConfig.ReportName,
                        DateType = rptConfig.DateType,
                        CreateInterval = rptConfig.CreateInterval,
                        GetDataTime = rptConfig.GetDataTime,
                        NeedConfirmed = rptConfig.NeedConfirmed,
                        IsEnabled = true

                    };
                    IQueryable<T_REPORT_CONFIG> congigTemp = from q in db.T_REPORT_CONFIG
                                                             where q.OrgId == rptConfig.OrgId
                                                             && q.StructId == rptConfig.StructId
                                                             && q.ReportName == rptConfig.ReportName
                                                             && q.DateType == rptConfig.DateType
                                                             && q.CreateInterval == rptConfig.CreateInterval
                                                             && q.GetDataTime == rptConfig.GetDataTime
                                                             && q.NeedConfirmed == rptConfig.NeedConfirmed
                                                             && q.IsEnabled == rptConfig.IsEnabled
                                                             select q;

                    var stc = db.T_REPORT_CONFIG.Where(s => s.ReportName == rptConfig.ReportName && s.DateType == rptConfig.DateType);

                    var intervalCheck = CronExpression.IsValidExpression(rptConfig.CreateInterval);

                    if (!intervalCheck || congigTemp.Any() || rptConfig.Templates == null || stc.Any())
                    {
                        if (!intervalCheck)
                        {
                            response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                           new JProperty("interval", false),
                           new JProperty("exist", false),
                           new JProperty("template", true),
                           new JProperty("name", true)//无重名
                           ));
                        }
                        else if (congigTemp.Any())
                        {
                            response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                          new JProperty("interval", true),
                          new JProperty("exist", true),
                          new JProperty("template", true),
                          new JProperty("name", true)
                          ));
                        }
                        else if (rptConfig.Templates == null)
                        {
                            response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                         new JProperty("interval", true),
                         new JProperty("exist", false),
                         new JProperty("template", false),
                         new JProperty("name", true)
                         ));
                        }
                        else if (stc.Any())
                        {
                            response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                         new JProperty("interval", true),
                         new JProperty("exist", false),
                         new JProperty("template", true),
                         new JProperty("name", false)
                         ));
                        }
                    }

                    if (intervalCheck && !congigTemp.Any() && rptConfig.Templates != null && !stc.Any())
                    {
                        db.T_REPORT_CONFIG.Add(us);
                        db.SaveChanges();
                        int configId = us.Id;
                        string templateStr = string.Empty;
                        if (rptConfig.Templates != null)
                        {
                            var templates = rptConfig.Templates.Split(',').Select(o => Convert.ToInt32(o));
                            var template = db.T_REPORT_TEMPLATE.Where(o => templates.Contains(o.Id)).Select(o => o.Name);

                            templateStr = string.Join(",", template);
                            foreach (var templateId in templates)
                            {
                                var uo = new T_REPORT_CONFIG_TEMPLATE { ReportConfigId = configId, ReportTemplateId = templateId };
                                var entry = db.Entry(uo);
                                entry.State = System.Data.EntityState.Added;
                            }

                        }

                        db.SaveChanges();
                        #region 日志信息
                        this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(rptConfig);
                        this.Request.Properties["ActionParameterShow"]
                            = string.Format("组织：{0}, 结构物：{1}, 报表名称：{2}, 日期类型：{3},  生成周期：{4},  模板: {5}",
                            us.OrgId,
                            us.StructId,
                            us.ReportName,
                            us.DateType,
                            us.CreateInterval,
                            templateStr);

                        #endregion
                        response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                            new JProperty("interval", true),
                            new JProperty("exist", false),
                            new JProperty("template", true),
                            new JProperty("name", true)
                            ));
                    }

                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表配置添加失败"));
                }
            }
        }

        /// <summary>
        /// 修改报表配置信息  reportConfig/modify-info/{Id}
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        [LogInfo("修改报表配置信息", true)]
        [Authorization(AuthorizationCode.S_Report_Config_Update)]
        public HttpResponseMessage ModifyReportConfigInfo([FromUri] int id, [FromBody] ReportConfig rptConfig)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var db = new SecureCloud_Entities())
            {
                var paraShow = new StringBuilder(100);
                try
                {
                    var rptConfigEntity = db.T_REPORT_CONFIG.ToList().Find(u => u.Id == id);
                    var intervalCheck = CronExpression.IsValidExpression(rptConfig.CreateInterval);
                    if (!intervalCheck || rptConfigEntity == null || rptConfig.Templates == null)
                    {
                        if (!intervalCheck)
                        {
                            response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                            new JProperty("interval", false),
                            new JProperty("exist", true),
                            new JProperty("template", true)
                           ));
                        }
                        else if (rptConfigEntity == null)
                        {
                            response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                           new JProperty("interval", true),
                           new JProperty("exist", false),
                           new JProperty("template", true)
                          ));
                        }
                        else if (rptConfig.Templates == null)
                        {
                            response = this.Request.CreateResponse(HttpStatusCode.OK,
                           new JObject(
                           new JProperty("interval", true),
                           new JProperty("exist", true),
                           new JProperty("template", false)
                         ));
                        }
                    }

                    if (intervalCheck && rptConfigEntity != null && rptConfig.Templates != null)
                    {

                        if (rptConfig.DateType != rptConfigEntity.DateType)
                        {
                            rptConfigEntity.DateType = rptConfig.DateType;
                            paraShow.AppendFormat("报表日期类型改为： {0} ", rptConfig.DateType);
                        }

                        if (rptConfig.CreateInterval != rptConfigEntity.CreateInterval)
                        {
                            rptConfigEntity.CreateInterval = rptConfig.CreateInterval;
                            paraShow.AppendFormat("报表生成周期改为： {0} ", rptConfig.CreateInterval);
                        }
                        if (rptConfig.GetDataTime != default(string) && rptConfig.GetDataTime != rptConfigEntity.GetDataTime)
                        {
                            rptConfigEntity.GetDataTime = rptConfig.GetDataTime;
                            paraShow.AppendFormat("报表数据获取时间改为： {0} ", rptConfig.GetDataTime);
                        }

                        if (rptConfig.IsEnabled != rptConfigEntity.IsEnabled)
                        {
                            rptConfigEntity.IsEnabled = rptConfig.IsEnabled;
                            paraShow.AppendFormat("报表启用状态改为: {0} ", rptConfig.IsEnabled);
                        }

                        if (rptConfig.NeedConfirmed != rptConfigEntity.NeedConfirmed)
                        {
                            rptConfigEntity.NeedConfirmed = rptConfig.NeedConfirmed;
                            paraShow.AppendFormat("报表是否需要确认状态改为： {0} ", rptConfig.NeedConfirmed);
                        }
                        var entry1 = db.Entry(rptConfigEntity);
                        entry1.State = System.Data.EntityState.Modified;

                        var queryTemp = from uo in db.T_REPORT_CONFIG_TEMPLATE where uo.ReportConfigId == id select uo;
                        foreach (var configTemp in queryTemp)
                        {
                            var entry2 = db.Entry(configTemp);
                            entry2.State = System.Data.EntityState.Deleted;
                        }

                        if (rptConfig.Templates.Length != 0)
                        {
                            var templates = rptConfig.Templates.Split(',').Select(o => Convert.ToInt32(o));
                            var temp = db.T_REPORT_TEMPLATE.Where(o => templates.Contains(o.Id)).Select(o => o.Name);
                            paraShow.AppendFormat("模板改为: {0} ", string.Join(",", temp));

                            foreach (var tempId in templates)
                            {
                                var uo = new T_REPORT_CONFIG_TEMPLATE { ReportConfigId = id, ReportTemplateId = tempId };
                                var entry3 = db.Entry(uo);
                                entry3.State = System.Data.EntityState.Added;
                            }
                        }
                        #region 日志信息
                        this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(rptConfig);
                        this.Request.Properties["ActionParameterShow"] = paraShow.ToString();

                        #endregion

                        db.SaveChanges();
                        response = this.Request.CreateResponse(HttpStatusCode.OK, new JObject(
                             new JProperty("interval", true),
                             new JProperty("exist", true),
                             new JProperty("template", true)
                            ));
                    }

                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表配置信息修改失败"));
                }

            }
        }

        /// <summary>
        /// 获取单个配置的详细信息 reportConfig/info/{Id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取单个配置的详细信息", false)]
        public object GetReportConfigInfo([FromUri] int id)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from u in db.T_REPORT_CONFIG
                            join r in db.T_DIM_ORGANIZATION on u.OrgId equals r.ID into r1
                            from s in r1.DefaultIfEmpty()
                            join st in db.T_DIM_STRUCTURE on u.StructId equals st.ID into uo1
                            from f in uo1.DefaultIfEmpty()
                            join c in db.T_REPORT_CONFIG_TEMPLATE on u.Id equals c.ReportConfigId into ct
                            from t in ct.DefaultIfEmpty()
                            join te in db.T_REPORT_TEMPLATE on t.ReportTemplateId equals te.Id into config
                            from re in config.DefaultIfEmpty()
                            where u.Id == id
                            select
                                new
                                {
                                    id = u.Id,
                                    orgId = (int?)s.ID,
                                    orgName = s.ABB_NAME_CN,
                                    structId = (int?)f.ID,
                                    structName = f.STRUCTURE_NAME_CN,
                                    reportName = u.ReportName,
                                    templateId = (int?)re.Id,
                                    templateName = re.Name,
                                    templateDes = re.Description,
                                    dateType = u.DateType,
                                    createInterval = u.CreateInterval,
                                    isEnabled = u.IsEnabled
                                };

                var data = query.ToList();
                return
                    data.GroupBy(
                        d => new { d.id, d.reportName, d.dateType, d.createInterval, d.isEnabled })
                        .Select(
                            g =>
                                new
                                {
                                    Id = g.Key.id,
                                    ReportName = g.Key.reportName,
                                    DateType = ((ReportDateType)g.Key.dateType).ToString(),
                                    CreateInterVal = g.Key.createInterval,
                                    IsEnabled = g.Key.isEnabled,
                                    Org = g.Select(o => o.orgId).FirstOrDefault() != null
                                          ? g.Select(o => new { id = o.orgId, name = o.orgName }).Distinct()
                                          : null,
                                    Struct =
                                     g.Select(o => o.structId).FirstOrDefault() != null
                                     ? g.Select(s => new { id = s.structId, name = s.structName }).Distinct()
                                     : null,
                                    Template = g.Select(o => o.templateId).FirstOrDefault() != null
                                    ? g.Select(o => new { id = o.templateId, name = o.templateName, des = o.templateDes }).Distinct()
                                    : null
                                }).FirstOrDefault();
            }
        }
    }

    /// <summary>
    /// 报表配置模型
    /// </summary>
    public class ReportConfig
    {
        public int? OrgId { get; set; }

        public int? StructId { get; set; }

        public int DateType { get; set; }

        public string ReportName { get; set; }

        public string CreateInterval { get; set; }

        public bool IsEnabled { get; set; }

        public bool NeedConfirmed { get; set; }

        public string Templates { get; set; }

        public string GetDataTime { get; set; }

    }

    public class ReportConfigListModal
    {
        public int id { get; set; }
        public int? orgId { get; set; }
        public int? structId { get; set; }
        public string structName { get; set; }
        public string reportName { get; set; }
        public int? templateId { get; set; }
        public string templateName { get; set; }
        public string templateDes { get; set; }
        public int dateType { get; set; }
        public string getDataTime { get; set; }
        public string createInterval { get; set; }
        public bool isEnabled { get; set; }
        public bool needConfirm { get; set; }
    }
}