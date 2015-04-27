// // --------------------------------------------------------------------------------------------
// // <copyright file="ReportConfigDal.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141023
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace ReportGeneratorService.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.ReportModule;

    using log4net;

    public class ReportConfigDal
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static List<ReportGroup> GetReportConfig()
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                //var temp = from c in db.T_REPORT_CONFIG
                //           join ct in db.T_REPORT_CONFIG_TEMPLATE on c.Id equals ct.ReportConfigId
                //           join t in db.T_REPORT_TEMPLATE on ct.ReportTemplateId equals t.Id
                //           where c.IsEnabled
                //           select new {c.Id, c.OrgId, c.StructId, c.ReportName, c.CreateInterval,c.NeedConfirmed, c.DateType, c.GetDataTime, t.Name, t.HandleName, t.FactorId, ct.Para1, ct.Para2, ct.Para3, ct.Para4};
                //var result = from s in temp
                //             group s by new { s.Id, s.OrgId, s.StructId, s.ReportName, s.CreateInterval, s.NeedConfirmed, s.DateType, s.GetDataTime }
                //                 into g
                //                 select new ReportGroup()
                //                     {
                //                         Config = new ReportConfig{Id = g.Key.Id, OrgId = g.Key.OrgId, StructId = g.Key.StructId, ReportName = g.Key.ReportName,CreateInterval = g.Key.CreateInterval, IsNeedConfirmed = g.Key.NeedConfirmed, DateType = (DateType)g.Key.DateType, },
                //                         Templates = (from p in g select new ReportTemplate{Name  = p.Name, HandleName = p.HandleName, Para1 = p.Para1, Para2 = p.Para2, Para3 = p.Para3, Para4 = p.Para4 }).ToList()
                //                         Templates = (from p in g select new ReportTemplate { Name = p.Name, HandleName = p.HandleName, FactorId = p.FactorId, Para = new List<string>(){p.Para1, p.Para2, p.Para3, p.Para4} }).ToList()
                //                     };
                //return result.ToList();
                var query = from u in db.T_REPORT_CONFIG
                            join r in db.T_DIM_ORGANIZATION on u.OrgId equals r.ID into r1
                            from s in r1.DefaultIfEmpty()
                            join st in db.T_DIM_STRUCTURE on u.StructId equals st.ID into uo1
                            from f in uo1.DefaultIfEmpty()
                            join c in db.T_REPORT_CONFIG_TEMPLATE on u.Id equals c.ReportConfigId into ct
                            from t in ct.DefaultIfEmpty()
                            join te in db.T_REPORT_TEMPLATE on t.ReportTemplateId equals te.Id into config
                            from re in config.DefaultIfEmpty()
                            where u.IsEnabled
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
                                    templateHandleName = re.HandleName,
                                    param1 = t.Para1,
                                    param2 = t.Para2,
                                    param3 = t.Para3,
                                    param4 = t.Para4,
                                    factorId = re.FactorId,
                                    templateDes = re.Description,
                                    dateType = u.DateType,
                                    createInterval = u.CreateInterval,
                                    getDataTime = u.GetDataTime,
                                    needConfirm = u.NeedConfirmed,
                                    isEnabled = u.IsEnabled
                                };

                var data = query.ToList();
                return
                    data.GroupBy(
                        s => new { s.id, s.orgId, s.structId, s.reportName, s.createInterval, s.needConfirm, s.dateType, s.getDataTime })
                        .Select(
                            g =>
                                new  ReportGroup()
                                     {
                                         Config = new ReportConfig{Id = g.Key.id, OrgId = g.Key.orgId, StructId = g.Key.structId, ReportName = g.Key.reportName,CreateInterval = g.Key.createInterval, IsNeedConfirmed = g.Key.needConfirm, DateType = (DateType)g.Key.dateType, },
                                         Templates =
                                         g.Select(
                                         p => 
                                             new ReportTemplate
                                             {
                                                 Name = p.templateName,
                                                 HandleName = p.templateHandleName, 
                                                 FactorId = p.factorId, 
                                                 Para = new List<string>(){p.param1, p.param2, p.param3, p.param4}
                                             }).ToList()
                                     }).ToList();
            }
        }

        public static bool DeleteOldReportInfo(string fileName)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                try
                {
                    var files = (from r in db.T_REPORT_COLLECTION
                                 where r.Name.Equals(fileName) 
                                 select r).ToList();
                    foreach (var reportCollection in files)
                    {
                        db.T_REPORT_COLLECTION.Remove(reportCollection);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Log.Warn(string.Format("报表信息数据库删除失败:{0}", fileName), e);
                    return false;
                }
                return true;
            }
        }

        public static bool SaveReportInfo(ReportInfo reportInfo)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                var state = Convert.ToInt16(reportInfo.Statue).ToString();

                //var report = new T_REPORT_COLLECTION
                //{
                //    Id = reportInfo.Id,
                //    Name = reportInfo.Name,
                //    Date = reportInfo.CreatedDate,
                //    FileFullName = reportInfo.FullName,
                //    Status = Convert.ToInt16(reportInfo.Statue).ToString(),
                //    OrgId = reportInfo.OrgId,
                //    StructId = reportInfo.StructureId,
                //    DateType = Convert.ToInt16(reportInfo.DateType)
                //};

                //var entry = db.Entry(report);
                //entry.State = System.Data.EntityState.Added;
                if (state == "1")
                {
                    var report = new T_REPORT_COLLECTION
                    {
                        Id = reportInfo.Id,
                        Name = reportInfo.Name,
                        Date = reportInfo.CreatedDate,
                        FileFullName = reportInfo.FullName,
                        Status = Convert.ToInt16(reportInfo.Statue).ToString(),
                        OrgId = reportInfo.OrgId,
                        StructId = reportInfo.StructureId,
                        DateType = Convert.ToInt16(reportInfo.DateType)
                    };

                    var entry = db.Entry(report);
                    entry.State = System.Data.EntityState.Added;

                }
                else if (state == "0")
                {
                    var report2 = new T_REPORT_COLLECTION
                    {
                        Id = reportInfo.Id,
                        Name = reportInfo.Name,
                        UnconfirmedDate = reportInfo.CreatedDate,
                        UnconfirmedFileUrl = reportInfo.FullName,
                        Status = Convert.ToInt16(reportInfo.Statue).ToString(),
                        OrgId = reportInfo.OrgId,
                        StructId = reportInfo.StructureId,
                        DateType = Convert.ToInt16(reportInfo.DateType)
                    };

                    var entry2 = db.Entry(report2);
                    entry2.State = System.Data.EntityState.Added;
                }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        
                        Log.Warn(string.Format("报表信息插入数据库失败:{0}", reportInfo.Name), e);
                        return false;
                    }
            }

            return true;
        }
    }
}