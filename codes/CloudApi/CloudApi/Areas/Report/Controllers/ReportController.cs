using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
using Newtonsoft.Json;
using System.Data.Objects.SqlClient;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Report.Controllers
{
    public class ReportController : ApiController
    {
        private Dictionary<string, int> dateMap = new Dictionary<string, int>
        {
            {"day", 1},
            {"week", 2},
            {"month", 3},
            {"year", 4}
        };

        /// <summary>
        /// 获取报表记录数量org/{orgId}/report-count/{dateType}
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="dateType"></param>
        /// <param name="keyWords"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取组织报表记录数量", false)]
        public object GetReportCountByOrgId(int orgId, string dateType)
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    int dateTypeId = dateMap[dateType];
                    var query =
                        db.T_REPORT_COLLECTION.Where(r => r.OrgId == orgId && r.DateType == dateTypeId && r.Status != "0");
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query =
                            query.Where(
                                l =>
                                    l.Name.ToLower().Contains(keyWords)
                            || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("yyyy", l.Date)).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", l.Date)).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("dd", l.Date)).Contains(keyWords)
                                );
                    }
                    var count = query.Count();
                    return new Data { Count = count };
                }
            }
            catch (Exception)
            {
                return new Data { Count = 0 };
            }

        }

        /// <summary>
        /// 根据组织编号获取报表列表org/{orgId}/report/{dateType}
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="dateType"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取组织报表", false)]
        public object GetReportListByOrgId(int orgId, string dateType)
        {
            int start = 0, end = 0;
            if (this.Request.GetQueryString("startRow") != null && this.Request.GetQueryString("endRow") != null)
            {
                start = int.Parse(this.Request.GetQueryString("startRow"));
                end = int.Parse(this.Request.GetQueryString("endRow"));
            }
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    int dateTypeId = dateMap[dateType];
                    var query =
                        db.T_REPORT_COLLECTION.Where(r => r.OrgId == orgId && r.DateType == dateTypeId && r.Status != "0");
                    var query2 = from q in query
                                 from s in db.T_DIM_STRUCTURE
                                 from o in db.T_DIM_ORGANIZATION
                                 where o.ID == q.OrgId
                                 where q.StructId == s.ID && s.IsDelete == 0
                                 select new
                                 {
                                     Name = q.Name,
                                     Date = q.Date,
                                     FileFullName = q.FileFullName,
                                     orgName = o.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Status = q.Status,
                                     ManualFileName = q.ManualFileName
                                 };
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query2 =
                            query2.Where(
                                l =>
                                    l.Name.ToLower().Contains(keyWords)
                            || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("yyyy", l.Date)).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", l.Date)).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("dd", l.Date)).Contains(keyWords)
                                );
                    }
                    var rslt = query2.OrderByDescending(i => i.Date).Select(q => q);
                    if (start > 0 && end > 0)
                    {
                        rslt = rslt.Skip(start - 1).Take(end - start + 1);
                    }
                    return rslt.ToList().Select(
                        r =>
                            new
                            {
                                reportName = r.Name,
                                time = Convert.ToDateTime(r.Date).ToShortDateString(),
                                url = r.FileFullName,
                                OrgName = r.orgName,
                                StructName = r.structName,
                                DateType = ((ReportDateType)(r.DateType)).ToString(),
                                Status = r.Status,
                                ManualFileName = r.ManualFileName
                            });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 获取结构物报表记录数目 struct/{structId}/report-count/{dateType}
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="dateType"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物报表记录数目", false)]
        public object GetReportCountByStructId(int structId, string dateType)
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    int dateTypeId = dateMap[dateType];
                    var query =
                        db.T_REPORT_COLLECTION.Where(
                            r => r.StructId == structId && r.DateType == dateTypeId && r.Status != "0");
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query =
                            query.Where(
                                l =>
                                    l.Name.ToLower().Contains(keyWords)
                                    || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("yyyy", l.Date)).Contains(keyWords)
                                    || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", l.Date)).Contains(keyWords)
                                    || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("dd", l.Date)).Contains(keyWords)
                                );
                    }
                    var count = query.Count();
                    return new Data { Count = count };
                }
            }
            catch (Exception)
            {
                return new Data { Count = 0 };
            }

        }

        /// <summary>
        /// 根据结构物编号获取报表列表struct/{structId}/report/{dateType}
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="dateType"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物报表", false)]
        public object GetReportListByStructId(int structId, string dateType)
        {
            int dateTypeId = dateMap[dateType];
            int start = 0, end = 0;
            if (this.Request.GetQueryString("startRow") != null && this.Request.GetQueryString("endRow") != null)
            {
                start = int.Parse(this.Request.GetQueryString("startRow"));
                end = int.Parse(this.Request.GetQueryString("endRow"));
            }
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    var query =
                        db.T_REPORT_COLLECTION.Where(
                            r => r.StructId == structId && r.DateType == dateTypeId && r.Status != "0");
                    var query2 = from q in query
                                 from s in db.T_DIM_STRUCTURE
                                 from os in db.T_DIM_ORG_STUCTURE
                                 from o in db.T_DIM_ORGANIZATION
                                 where q.StructId == s.ID && s.IsDelete == 0 && s.ID == os.STRUCTURE_ID && os.ORGANIZATION_ID == o.ID
                                 select new
                                 {
                                     Name = q.Name,
                                     Date = q.Date,
                                     FileFullName = q.FileFullName,
                                     orgName = o.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Status = q.Status,
                                     ManualFileName = q.ManualFileName
                                 }; ;

                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query2 =
                            query2.Where(
                                l =>
                                    l.Name.ToLower().Contains(keyWords)
                                    || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("yyyy", l.Date)).Contains(keyWords)
                                    || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", l.Date)).Contains(keyWords)
                                    || SqlFunctions.StringConvert((double)SqlFunctions.DatePart("dd", l.Date)).Contains(keyWords)
                                );
                    }
                    var rslt = query2.OrderByDescending(i => i.Date).Select(q => q);
                    if (start > 0 && end > 0)
                    {
                        rslt = rslt.Skip(start - 1).Take(end - start + 1);
                    }
                    return rslt.ToList().Select(
                        r =>
                            new
                            {
                                reportName = r.Name,
                                time = Convert.ToDateTime(r.Date).ToShortDateString(),
                                url = r.FileFullName,
                                OrgName = r.orgName,
                                StructName = r.structName,
                                DateType = ((ReportDateType)(r.DateType)).ToString(),
                                Status = r.Status,
                                ManualFileName = r.ManualFileName
                            });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ClientGetManagedRpt(int userId)
        {
            using (var db = new SecureCloud_Entities())
            {
                int start = 0, end = 0;
                if (this.Request.GetQueryString("startRow") != null && this.Request.GetQueryString("endRow") != null)
                {
                    start = int.Parse(this.Request.GetQueryString("startRow"));
                    end = int.Parse(this.Request.GetQueryString("endRow"));
                }
                try
                {
                    var query1 = from org in db.T_DIM_USER_ORG
                                 from q in db.T_REPORT_COLLECTION
                                 from o in db.T_REPORT_CONFIG
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where q.OrgId == org.ORGANIZATION_ID && org.USER_NO == userId
                                 && (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && q.StructId == o.StructId && s.IsDelete == 0
                                 && l.ID == q.OrgId && q.OrgId == o.OrgId
                                 && q.DateType == o.DateType && o.NeedConfirmed && o.IsEnabled
                                 && ((q.Status == "1" && q.FileFullName != null) || (q.Status == "0"
                                 && q.UnconfirmedFileUrl != null))
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = (q.Status ?? "1"),
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = o.NeedConfirmed,
                                     manualFileName = q.ManualFileName
                                 };
                    var query2 = from org in db.T_DIM_USER_ORG
                                 from q in db.T_REPORT_COLLECTION
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where q.OrgId == org.ORGANIZATION_ID && org.USER_NO == userId
                                 && (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && s.IsDelete == 0 && l.ID == q.OrgId
                                 && q.Status == "2"
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = q.Status,
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = true,
                                     manualFileName = q.ManualFileName
                                 };
                    var query = query1.ToList().Union(query2.ToList());
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query =
                           query.Where(
                               l =>
                                   l.Name.ToLower().Contains(keyWords)
                                   || l.orgName.ToLower().Contains(keyWords)
                                   || l.structName.ToLower().Contains(keyWords)
                                   || ((ReportDateType)(l.DateType)).ToString().ToLower().Contains(keyWords)
                                   || ((ReportStatus)(Convert.ToInt32(l.Status))).ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Day.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Day.ToString().Contains(keyWords)
                               );
                    }
                    var ret = query.OrderByDescending(i => i.Date).Select(q => q);
                    if (start > 0 && end > 0)
                    {
                        ret = ret.Skip(start - 1).Take(end - start + 1);
                    }
                    return
                        ret
                            .Select(
                                r =>
                                    new
                                    {
                                        reportId = r.Id,
                                        reportName = r.Name,
                                        OrgName = r.orgName,
                                        StructName = r.structName,
                                        DateType = ((ReportDateType)(r.DateType)).ToString(),
                                        status = r.Status,
                                        ConfirmedDate = Convert.ToDateTime(r.Date).ToShortDateString(),
                                        ConfirmedUrl = r.cornfirmedUrl,
                                        UnconfirmedDate = Convert.ToDateTime(r.unconfirmedDate).ToShortDateString(),
                                        UnconfirmedUrl = r.unconfirmedUrl,
                                        Confirm = r.confirm,
                                        ManualFileName = r.manualFileName
                                    });
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public object SupportGetManagedRpt()
        {
            using (var db = new SecureCloud_Entities())
            {
                int start = 0, end = 0;
                if (this.Request.GetQueryString("startRow") != null && this.Request.GetQueryString("endRow") != null)
                {
                    start = int.Parse(this.Request.GetQueryString("startRow"));
                    end = int.Parse(this.Request.GetQueryString("endRow"));
                }
                try
                {
                    var query1 = from q in db.T_REPORT_COLLECTION
                                 from o in db.T_REPORT_CONFIG
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where  (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && q.StructId == o.StructId && s.IsDelete == 0
                                 && l.ID == q.OrgId && q.OrgId == o.OrgId
                                 && q.DateType == o.DateType && o.NeedConfirmed && o.IsEnabled
                                 && ((q.Status == "1" && q.FileFullName != null) || (q.Status == "0"
                                 && q.UnconfirmedFileUrl != null))
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = (q.Status ?? "1"),
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = o.NeedConfirmed,
                                     manualFileName = q.ManualFileName
                                 };
                    var query2 = from q in db.T_REPORT_COLLECTION
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where  (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && s.IsDelete == 0 && l.ID == q.OrgId
                                 && q.Status == "2"
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = q.Status,
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = true,
                                     manualFileName = q.ManualFileName
                                 };
                    var query = query1.ToList().Union(query2.ToList());
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query =
                           query.Where(
                               l =>
                                   l.Name.ToLower().Contains(keyWords)
                                   || l.orgName.ToLower().Contains(keyWords)
                                   || l.structName.ToLower().Contains(keyWords)
                                   || ((ReportDateType)(l.DateType)).ToString().ToLower().Contains(keyWords)
                                   || ((ReportStatus)(Convert.ToInt32(l.Status))).ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Day.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Day.ToString().Contains(keyWords)
                               );
                    }
                    var ret = query.OrderByDescending(i => i.Date).Select(q => q);
                    if (start > 0 && end > 0)
                    {
                        ret = ret.Skip(start - 1).Take(end - start + 1);
                    }
                    return
                        ret
                            .Select(
                                r =>
                                    new
                                    {
                                        reportId = r.Id,
                                        reportName = r.Name,
                                        OrgName = r.orgName,
                                        StructName = r.structName,
                                        DateType = ((ReportDateType)(r.DateType)).ToString(),
                                        status = r.Status,
                                        ConfirmedDate = Convert.ToDateTime(r.Date).ToShortDateString(),
                                        ConfirmedUrl = r.cornfirmedUrl,
                                        UnconfirmedDate = Convert.ToDateTime(r.unconfirmedDate).ToShortDateString(),
                                        UnconfirmedUrl = r.unconfirmedUrl,
                                        Confirm = r.confirm,
                                        ManualFileName = r.manualFileName
                                    });
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取需要管理的报表 user/{userId}/report/managedRpt-list
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取需要管理的报表", false)]
        [Authorization(AuthorizationCode.S_Report_Manage)]
        public object GetManagedRpt(int userId)
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
                        return SupportGetManagedRpt();
                    }
                    return ClientGetManagedRpt(userId);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ClientGetManagedRptGroupByStatus(int userId)
        {
            using (var db = new SecureCloud_Entities())
            {
                int start = 0, end = 0;
                if (this.Request.GetQueryString("startRow") != null && this.Request.GetQueryString("endRow") != null)
                {
                    start = int.Parse(this.Request.GetQueryString("startRow"));
                    end = int.Parse(this.Request.GetQueryString("endRow"));
                }
                try
                {
                    var query1 = from org in db.T_DIM_USER_ORG
                                 from q in db.T_REPORT_COLLECTION
                                 from o in db.T_REPORT_CONFIG
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where q.OrgId == org.ORGANIZATION_ID && org.USER_NO == userId
                                 && (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && q.StructId == o.StructId && s.IsDelete == 0
                                 && l.ID == q.OrgId && q.OrgId == o.OrgId
                                 && q.DateType == o.DateType && o.NeedConfirmed && o.IsEnabled
                                 && ((q.Status == "1" && q.FileFullName != null) || (q.Status == "0"
                                 && q.UnconfirmedFileUrl != null))
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = (q.Status ?? "1"),
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = o.NeedConfirmed,
                                     manualFileName = q.ManualFileName
                                 };
                    var query2 = from org in db.T_DIM_USER_ORG
                                 from q in db.T_REPORT_COLLECTION
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where q.OrgId == org.ORGANIZATION_ID && org.USER_NO == userId
                                 && (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && s.IsDelete == 0 && l.ID == q.OrgId
                                 && q.Status == "2"
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = q.Status,
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = true,
                                     manualFileName = q.ManualFileName
                                 };
                    var query = query1.ToList().Union(query2.ToList());
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query =
                           query.Where(
                               l =>
                                   l.Name.ToLower().Contains(keyWords)
                                   || l.orgName.ToLower().Contains(keyWords)
                                   || l.structName.ToLower().Contains(keyWords)
                                   || ((ReportDateType)(l.DateType)).ToString().ToLower().Contains(keyWords)
                                   || ((ReportStatus)(Convert.ToInt32(l.Status))).ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Day.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Day.ToString().Contains(keyWords)
                               );
                    }
                    var ret = query.OrderByDescending(i => i.Status).Select(q => q);
                    if (start > 0 && end > 0)
                    {
                        ret = ret.Skip(start - 1).Take(end - start + 1);
                    }
                    return
                        ret.GroupBy(g =>
                            new
                            {
                                g.Status
                            })
                            .Select(
                                s =>
                                    new
                                    {
                                        status = s.Key.Status,
                                        reports = s.Select(
                                         r =>
                                          new
                                          {
                                              reportId = r.Id,
                                              reportName = r.Name,
                                              OrgName = r.orgName,
                                              StructName = r.structName,
                                              DateType = ((ReportDateType)(r.DateType)).ToString(),
                                              ConfirmedDate = Convert.ToDateTime(r.Date).ToShortDateString(),
                                              ConfirmedUrl = r.cornfirmedUrl,
                                              UnconfirmedDate = Convert.ToDateTime(r.unconfirmedDate).ToShortDateString(),
                                              UnconfirmedUrl = r.unconfirmedUrl,
                                              Confirm = r.confirm,
                                              ManualFileName = r.manualFileName
                                          }).OrderByDescending(d => d.ConfirmedDate ?? d.UnconfirmedDate).ToList()
                                    }).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public object SupportGetManagedRptGroupByStatus()
        {
            using (var db = new SecureCloud_Entities())
            {
                int start = 0, end = 0;
                if (this.Request.GetQueryString("startRow") != null && this.Request.GetQueryString("endRow") != null)
                {
                    start = int.Parse(this.Request.GetQueryString("startRow"));
                    end = int.Parse(this.Request.GetQueryString("endRow"));
                }
                try
                {
                    var query1 = from q in db.T_REPORT_COLLECTION
                                 from o in db.T_REPORT_CONFIG
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where  (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && q.StructId == o.StructId && s.IsDelete == 0
                                 && l.ID == q.OrgId && q.OrgId == o.OrgId
                                 && q.DateType == o.DateType && o.NeedConfirmed && o.IsEnabled
                                 && ((q.Status == "1" && q.FileFullName != null) || (q.Status == "0"
                                 && q.UnconfirmedFileUrl != null))
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = (q.Status ?? "1"),
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = o.NeedConfirmed,
                                     manualFileName = q.ManualFileName
                                 };
                    var query2 = from q in db.T_REPORT_COLLECTION
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where  (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && s.IsDelete == 0 && l.ID == q.OrgId
                                 && q.Status == "2"
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = q.Status,
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = true,
                                     manualFileName = q.ManualFileName
                                 };
                    var query = query1.ToList().Union(query2.ToList());
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        query =
                           query.Where(
                               l =>
                                   l.Name.ToLower().Contains(keyWords)
                                   || l.orgName.ToLower().Contains(keyWords)
                                   || l.structName.ToLower().Contains(keyWords)
                                   || ((ReportDateType)(l.DateType)).ToString().ToLower().Contains(keyWords)
                                   || ((ReportStatus)(Convert.ToInt32(l.Status))).ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Day.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Day.ToString().Contains(keyWords)
                               );
                    }
                    var ret = query.OrderByDescending(i => i.Status).Select(q => q);
                    if (start > 0 && end > 0)
                    {
                        ret = ret.Skip(start - 1).Take(end - start + 1);
                    }
                    return
                        ret.GroupBy(g =>
                            new
                            {
                                g.Status
                            })
                            .Select(
                                s =>
                                    new
                                    {
                                        status = s.Key.Status,
                                        reports = s.Select(
                                         r =>
                                          new
                                          {
                                              reportId = r.Id,
                                              reportName = r.Name,
                                              OrgName = r.orgName,
                                              StructName = r.structName,
                                              DateType = ((ReportDateType)(r.DateType)).ToString(),
                                              ConfirmedDate = Convert.ToDateTime(r.Date).ToShortDateString(),
                                              ConfirmedUrl = r.cornfirmedUrl,
                                              UnconfirmedDate = Convert.ToDateTime(r.unconfirmedDate).ToShortDateString(),
                                              UnconfirmedUrl = r.unconfirmedUrl,
                                              Confirm = r.confirm,
                                              ManualFileName = r.manualFileName
                                          }).OrderByDescending(d => d.ConfirmedDate ?? d.UnconfirmedDate).ToList()
                                    }).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取需要管理的报表(按照状态分组) user/{userId}/report/orderManagedRpt-list
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取需要管理的报表(按照状态分组)", false)]
        public object GetManagedRptGroupByStatus(int userId)
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
                            return SupportGetManagedRptGroupByStatus();
                        }
                        return ClientGetManagedRptGroupByStatus(userId);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
        }


        public object SupportGetManagedRptCount()
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    var query1 = from q in db.T_REPORT_COLLECTION
                                 from o in db.T_REPORT_CONFIG
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && q.StructId == o.StructId && s.IsDelete == 0
                                 && l.ID == q.OrgId && q.OrgId == o.OrgId
                                 && q.DateType == o.DateType && o.NeedConfirmed && o.IsEnabled
                                 && ((q.Status == "1" && q.FileFullName != null) || (q.Status == "0"
                                 && q.UnconfirmedFileUrl != null))
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = (q.Status ?? "1"),
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = o.NeedConfirmed
                                 };
                    var query2 = from q in db.T_REPORT_COLLECTION
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && s.IsDelete == 0 && l.ID == q.OrgId
                                 && q.Status == "2"
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = q.Status,
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = true
                                 };
                    var list = (query1.ToList().Union(query2.ToList()));
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        list =
                            list.Where(
                                l =>
                                      l.Name.ToLower().Contains(keyWords)
                                   || l.orgName.ToLower().Contains(keyWords)
                                   || l.structName.ToLower().Contains(keyWords)
                                   || ((ReportDateType)(l.DateType)).ToString().ToLower().Contains(keyWords)
                                   || ((ReportStatus)(Convert.ToInt32(l.Status))).ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Day.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Day.ToString().Contains(keyWords)
                                );
                    }
                    var count = list.Count();
                    return new Data { Count = count };
                }
            }
            catch (Exception)
            {
                return new Data { Count = 0 };
            }
        }

        public object ClientGetManagedRptCount(int userId)
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    var query1 = from org in db.T_DIM_USER_ORG
                                 from q in db.T_REPORT_COLLECTION
                                 from o in db.T_REPORT_CONFIG
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where q.OrgId == org.ORGANIZATION_ID && org.USER_NO == userId
                                 && (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && q.StructId == o.StructId && s.IsDelete == 0
                                 && l.ID == q.OrgId && q.OrgId == o.OrgId
                                 && q.DateType == o.DateType && o.NeedConfirmed && o.IsEnabled
                                 && ((q.Status == "1" && q.FileFullName != null) || (q.Status == "0"
                                 && q.UnconfirmedFileUrl != null))
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = (q.Status ?? "1"),
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = o.NeedConfirmed
                                 };

                    var query2 = from org in db.T_DIM_USER_ORG
                                 from q in db.T_REPORT_COLLECTION
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where q.OrgId == org.ORGANIZATION_ID && org.USER_NO == userId
                                 && (l.IsDeleted == false || l.IsDeleted == null)
                                 && q.StructId == s.ID && s.IsDelete == 0 && l.ID == q.OrgId
                                 && q.Status == "2"
                                 select new
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     DateType = q.DateType,
                                     Date = q.Date,
                                     Status = q.Status,
                                     cornfirmedUrl = q.FileFullName,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     confirm = true
                                 };
                    var list = (query1.ToList().Union(query2.ToList()));
                    if (this.Request.GetQueryString("keyWords") != null)
                    {
                        string keyWords = this.Request.GetQueryString("keyWords").ToLower();
                        list =
                            list.Where(
                                l =>
                                      l.Name.ToLower().Contains(keyWords)
                                   || l.orgName.ToLower().Contains(keyWords)
                                   || l.structName.ToLower().Contains(keyWords)
                                   || ((ReportDateType)(l.DateType)).ToString().ToLower().Contains(keyWords)
                                   || ((ReportStatus)(Convert.ToInt32(l.Status))).ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.Date).Day.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Year.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Month.ToString().Contains(keyWords)
                                   || Convert.ToDateTime(l.unconfirmedDate).Day.ToString().Contains(keyWords)
                                );
                    }
                    var count = list.Count();
                    return new Data { Count = count };
                }
            }
            catch (Exception)
            {
                return new Data { Count = 0 };
            }
        }


        /// <summary>
        /// 获取需要管理的报表记录数量 user/{userId}/report/manualRpt-count
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取需要管理的报表记录数量", false)]
        [Authorization(AuthorizationCode.S_Report_Manage)]
        public object GetManagedRptCount(int userId)
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
                        return SupportGetManagedRptCount();
                    }
                    return ClientGetManagedRptCount(userId);
                }
            }
            catch (Exception)
            {
                return new Data { Count = 0 };
            }
        }

        /// <summary>
        /// 报表重命名 report/rename/{rptId}
        /// </summary>
        /// <param name="rptId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("报表重命名", true)]
        [Authorization(AuthorizationCode.S_Report_Manage_Rename)]
        public HttpResponseMessage RenameReport([FromUri] string rptId, [FromBody] FileName fileName)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var query = db.T_REPORT_COLLECTION.FirstOrDefault(r => r.Id == rptId);
                    if (query == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("报表不存在或已被删除"));
                    }
                    //更新路径
                    var filePath = query.FileFullName;
                    if (File.Exists(filePath))
                    {
                        var dir = Path.GetDirectoryName(filePath);
                        var ext = Path.GetExtension(filePath);
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                        var filename = string.Format("{0}{1}", fileName.Name, ext);
                        if (fileNameWithoutExtension != fileName.Name)
                        {
                            var fileFullName = Path.Combine(dir, filename);
                            File.Copy(filePath, fileFullName);
                            query.FileFullName = fileFullName;
                            var entry0 = db.Entry(query);
                            entry0.State = System.Data.EntityState.Modified;
                            db.SaveChanges();
                            File.Delete(filePath);
                        }
                    }
                    //重命名
                    query.Name = fileName.Name;
                    var entry = db.Entry(query);
                    entry.State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    #region 日志信息
                    this.Request.Properties["ActionParameterShow"] = "报表重命名为：" + fileName.Name;
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("报表重命名成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表重命名失败"));
                }
            }
        }

        /// <summary>
        /// 查询报表详细信息 report/info/{rptId}
        /// </summary>
        /// <param name="rptId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("查询报表详细信息", false)]
        [Authorization(AuthorizationCode.S_Report_Manage)]
        public object GetReportInfoById(string rptId)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var query = db.T_REPORT_COLLECTION.Where(r => r.Id == rptId);
                    if (!query.Any())
                    {
                        return null;
                    }
                    var query2 = from q in query
                                 from s in db.T_DIM_STRUCTURE
                                 from l in db.T_DIM_ORGANIZATION
                                 where q.StructId == s.ID && s.IsDelete == 0 && l.ID == q.OrgId
                                 select new
                                 {
                                     rptId = q.Id,
                                     rptName = q.Name,
                                     orgId = q.OrgId,
                                     structId = q.StructId,
                                     orgName = l.ABB_NAME_CN,
                                     structName = s.STRUCTURE_NAME_CN,
                                     confirmedDate = q.Date,
                                     confirmedUrl = q.FileFullName,
                                     unconfirmedDate = q.UnconfirmedDate,
                                     unconfirmedUrl = q.UnconfirmedFileUrl,
                                     State = q.Status,
                                     dateType = q.DateType,
                                     manualFileName = q.ManualFileName
                                 };
                    var list = query2.ToList();
                    return
                        list
                            .Select(
                                r =>
                                    new
                                    {
                                        reportId = r.rptId,
                                        reportName = r.rptName,
                                        status = r.State,
                                        Org = new
                                        {
                                            OrgId = r.orgId,
                                            OrgName = r.orgName,
                                        },
                                        Struct = new
                                        {
                                            StructId = r.structId,
                                            StructName = r.structName
                                        },
                                        ConfirmedUrl = r.confirmedUrl,
                                        ConfirmedDate = Convert.ToDateTime(r.confirmedDate).ToShortDateString(),
                                        UnconfirmedUrl = r.unconfirmedUrl,
                                        UnconfirmedDate = Convert.ToDateTime(r.unconfirmedDate).ToShortDateString(),
                                        DateType = ((ReportDateType)(r.dateType)).ToString(),
                                        ManualFileName = r.manualFileName
                                    });
                }
                catch (Exception)
                {

                    return null;
                }

            }
        }

        /// <summary>
        /// 删除报表信息  report/remove/{rptId}
        /// </summary>
        /// <param name="rptId"></param>
        /// <returns>删除结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除报表信息", true)]
        [Authorization(AuthorizationCode.S_Report_Manage_Delete)]
        public HttpResponseMessage RemoveReportInfo([FromUri] string rptId)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var query = db.T_REPORT_COLLECTION.FirstOrDefault(r => r.Id == rptId);
                    if (query == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.Accepted,
                            StringHelper.GetMessageString("报表不存在或已被删除"));
                    }
                    var url = string.Empty;
                    if (query.Status == "0")
                    {
                        url = query.UnconfirmedFileUrl;
                    }
                    else
                    {
                        url = query.FileFullName;
                    }
                    var entry = db.Entry(query);
                    entry.State = System.Data.EntityState.Deleted;
                    db.SaveChanges();
                    if (File.Exists(url))
                    {
                        File.Delete(url);
                    }
                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = "rptId:" + rptId;
                    this.Request.Properties["ActionParameterShow"] = "报表名：" + query.Name;
                    #endregion
                    var query2 = db.T_REPORT_COLLECTION.Where(r => r.Name == query.Name);
                    if (query2.Any())
                    {
                        foreach (var item in query2)
                        {
                            if (item.Status != "2")
                            {
                                var entry2 = db.Entry(item);
                                entry2.State = System.Data.EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("报表删除成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表删除失败"));
                }
            }

        }

        /// <summary>
        /// 按名称删除报表信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("按名称删除报表信息", true)]
        [Authorization(AuthorizationCode.S_Report_Manage_Delete)]
        public HttpResponseMessage RemoveReportInfoByName([FromBody] FileName fileName)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var query = db.T_REPORT_COLLECTION.Where(r => r.Name == fileName.Name);
                    if (!query.Any())
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.Accepted,
                            StringHelper.GetMessageString("报表不存在或已被删除"));
                    }
                    foreach (var item in query)
                    {
                        var entry = db.Entry(item);
                        entry.State = System.Data.EntityState.Deleted;
                    }
                    db.SaveChanges();
                    #region 日志信息

                    this.Request.Properties["ActionParameterShow"] = "报表名：" + fileName.Name;

                    #endregion
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("报表删除成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表删除失败"));

                }
            }

        }
        /// <summary>
        /// 增加报表
        /// </summary>
        /// <param name="reportInfo"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("增加报表", true)]
        [Authorization(AuthorizationCode.S_Report_Manage_ManualUpload)]
        public HttpResponseMessage AddReport([FromBody] ReportInfo reportInfo)
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    var query = db.T_REPORT_COLLECTION.Where(r => r.Name == reportInfo.Name && r.OrgId == reportInfo.OrgId && r.StructId == reportInfo.StructId && r.DateType == reportInfo.DateType && r.Date == reportInfo.Date);
                    if (query.Any())
                    {
                        foreach (var rpt in query.ToList())
                        {
                            var url = string.Empty;
                            if (rpt.Status == "0")
                            {
                                url = rpt.UnconfirmedFileUrl;
                            }
                            else
                            {
                                url = rpt.FileFullName;
                            }
                            var entry0 = db.Entry(rpt);
                            entry0.State = System.Data.EntityState.Deleted;
                            db.SaveChanges();
                            if (File.Exists(url))
                            {
                                File.Delete(url);
                            }
                        }
                    }
                    var report = new T_REPORT_COLLECTION
                    {
                        Id = reportInfo.Id,
                        Name = reportInfo.Name,
                        OrgId = reportInfo.OrgId,
                        StructId = reportInfo.StructId,
                        FactorId = reportInfo.FactorId,
                        DateType = reportInfo.DateType,
                        FileFullName = reportInfo.FileFullName,
                        Date = reportInfo.Date,
                        Status = reportInfo.Status,
                        ManualFileName = reportInfo.ManualFileName
                    };
                    var entry = db.Entry(report);
                    query = db.T_REPORT_COLLECTION.Where(r => r.Id == reportInfo.Id);
                    if (query.Any())//更新已有的记录,文件覆盖
                    {
                        entry.State = System.Data.EntityState.Modified;
                    }
                    else
                    {
                        entry.State = System.Data.EntityState.Added;
                    }
                    db.SaveChanges();
                }
                #region 日志信息
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(reportInfo);
                this.Request.Properties["ActionParameterShow"]
                    = string.Format("组织：{0}, 结构物：{1}, 报表名称：{2}, 报表日期类型：{3},  报表存放路径：{4},  报表生成日期: {5}",
                    reportInfo.OrgId,
                    reportInfo.StructId,
                    reportInfo.Name,
                    reportInfo.DateType,
                    reportInfo.FileFullName,
                    reportInfo.Date);
                #endregion
                return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("增加报表成功"));
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("增加报表失败"));
            }
        }

        /// <summary>
        /// 更新报表信息 report/update/{rptId}
        /// </summary>
        /// <returns>保存结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("更新报表信息", true)]
        [Authorization(AuthorizationCode.S_Report_Manage_Upload)]
        public HttpResponseMessage UpdateReportInfo([FromUri] string rptId, [FromBody] Report rpt)
        {
            using (var db = new SecureCloud_Entities())
            {
                var paraShow = new StringBuilder(100);
                try
                {
                    var rptEntity = db.T_REPORT_COLLECTION.FirstOrDefault(u => u.Id == rptId);
                    if (rptEntity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("待确认的报表不存在"));
                    }
                    if (rpt.FileFullName != rptEntity.FileFullName)
                    {
                        rptEntity.FileFullName = rpt.FileFullName;
                        paraShow.AppendFormat("报表全名改为：{0} ", rpt.FileFullName);
                    }
                    if (rpt.Status != rptEntity.Status)
                    {
                        rptEntity.Status = rpt.Status;
                        paraShow.AppendFormat("报表完成状态改为：{0} ", rpt.Status);
                    }
                    if (rpt.Date != rptEntity.Date)
                    {
                        rptEntity.Date = rpt.Date;
                        paraShow.AppendFormat("报表生成时间改为：{0} ", rpt.Date);
                    }
                    var entry1 = db.Entry(rptEntity);
                    entry1.State = System.Data.EntityState.Modified;
                    db.SaveChanges();

                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(rpt);
                    this.Request.Properties["ActionParameterShow"] = paraShow.ToString();
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("报表确认成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("报表确认失败"));
                }

            }
        }
    }

    public class FileName
    {
        public string Name { get; set; }
    }

    public class Report
    {
        public string FileFullName { get; set; }

        public DateTime? Date { get; set; }

        public string Status { get; set; }
    }

    public enum ReportType
    {
        组织报表 = 1,
        结构物报表 = 2,
        监测因素报表 = 3
    }

    public enum ReportDateType
    {
        日报表 = 1,
        周报表 = 2,
        月报表 = 3,
        年报表 = 4
    }

    public enum ReportStatus
    {
        未确认 = 0,
        已确认 = 1,
        人工上传 = 2
    }

    public class Data
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class ReportInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ManualFileName { get; set; }

        public int? OrgId { get; set; }
        public int? StructId { get; set; }
        public int? FactorId { get; set; }

        public int DateType { get; set; }

        public string FileFullName { get; set; }
        public DateTime? Date { get; set; }


        public string Status { get; set; }

    }

    /// <summary>
    /// 需要二次确认报表的详细信息
    /// </summary>
    public class needConfirmed
    {
        public string rptId { get; set; }

        public string rptName { get; set; }

        public string orgName { get; set; }

        public string structName { get; set; }

        public DateTime? date { get; set; }

        public string cornfirmedUrl { get; set; }

        public string unconfirmedUrl { get; set; }

        public DateTime? unconfirmedDate { get; set; }

        public string state { get; set; }

        public int dateType { get; set; }

        public bool confirm { get; set; }

    }

    /// <summary>
    /// 创建一个通用比较的类，实现IEqualityComparer<T>接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class CommonEqualityComparer<T, V> : IEqualityComparer<T>
    {
        private Func<T, V> keySelector;

        public CommonEqualityComparer(Func<T, V> keySelector)
        {
            this.keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            return EqualityComparer<V>.Default.Equals(keySelector(x), keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<V>.Default.GetHashCode(keySelector(obj));
        }
    }

    /// <summary>
    /// 借助上面这个类，重载Distinct扩展方法
    /// </summary>
    public static class DistinctExtensions
    {
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }
    }
}
