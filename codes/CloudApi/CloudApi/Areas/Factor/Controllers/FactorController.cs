namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Factor.Controllers
{
    using System;
    using System.Collections.Generic;    
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class FactorController : ApiController
    {
        /// <summary>
        /// Get struct/{structid}/factors
        /// </summary>
        /// <param name="structId">结构物编号（只能是数字）</param>
        /// <returns>监测因素</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物监测因素", false)]
        public object FindFactorsByStruct(int structId)
        {
            var list = new List<T_DIM_SAFETY_FACTOR_TYPE>();
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from stc in entity.T_DIM_STRUCTURE
                            from sf in entity.T_DIM_STRUCTURE_FACTOR
                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                            where stc.ID == structId
                                  && stc.ID == sf.STRUCTURE_ID
                                  && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                            select f;
                var queryAll = from q in query
                               join f in entity.T_DIM_SAFETY_FACTOR_TYPE on q.SAFETY_FACTOR_TYPE_PARENT_ID equals f.SAFETY_FACTOR_TYPE_ID
                               select f;
                list = query.Union(queryAll).ToList();
            }

            return
                new JArray(
                    list.Where(l => l.SAFETY_FACTOR_TYPE_PARENT_ID == 0)
                        .OrderBy(l => l.SAFETY_FACTOR_TYPE_ID)
                        .Select(
                            l => new JObject(
                                     new JProperty("factorId", l.SAFETY_FACTOR_TYPE_ID),
                    new JProperty("factorName", l.SAFETY_FACTOR_TYPE_NAME),
                    new JProperty(
                        "children",
                        new JArray(
                                     list.Where(c => l.SAFETY_FACTOR_TYPE_ID == c.SAFETY_FACTOR_TYPE_PARENT_ID)
                                     .OrderBy(c => c.SAFETY_FACTOR_TYPE_ID)
                                     .Select(
                                         c => new JObject(
                                    new JProperty("factorId", c.SAFETY_FACTOR_TYPE_ID),
                                    new JProperty("factorName", c.SAFETY_FACTOR_TYPE_NAME),
                                    new JProperty("valueNumber", c.FACTOR_VALUE_COLUMN_NUMBER))))))));
        }

        /// <summary>
        /// 获取结构物监测因素状态
        /// </summary>
        /// <param name="structs">结构物数组</param>
        /// <returns>因素状态</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物监测因素状态", false)]
        public object FindFactorStatusByStruct(string structs)
        {
            int[] strcts = structs.Split(',').Select(s => Convert.ToInt32(s)).ToArray();

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                #region 因子评分产生状态-使用中

                Debug.WriteLine("==========================================");
                
                // 查询结构物和包含的监测主题
                var query = (from stc in entity.T_DIM_STRUCTURE
                             from sf in entity.T_DIM_STRUCTURE_FACTOR
                             from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                             from f2 in entity.T_DIM_SAFETY_FACTOR_TYPE
                             from orgStc in entity.T_DIM_ORG_STUCTURE
                             where
                                 strcts.Contains(stc.ID) && stc.ID == sf.STRUCTURE_ID
                                 && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                 && f.SAFETY_FACTOR_TYPE_PARENT_ID == f2.SAFETY_FACTOR_TYPE_ID
                                 && orgStc.STRUCTURE_ID == stc.ID
                             select
                                 new FactorList
                                     {
                                         StructId = stc.ID,
                                         OrgStc = orgStc.ORG_STRUC_ID,
                                         FactorId = f2.SAFETY_FACTOR_TYPE_ID,
                                         FactorName = f2.SAFETY_FACTOR_TYPE_NAME
                                     }).Distinct();

                var themeScore = from ts in entity.T_FACT_SAFETY_FACTOR_SCORE
                                 from q in query
                                 where ts.ORG_STRUC_ID == q.OrgStc && ts.SAFETY_FACTOR_TYPE_ID == q.FactorId
                                 select
                                     new
                                         {
                                             Id = ts.ID,
                                             q.OrgStc,
                                             StructId = q.StructId,
                                             FactorId = q.FactorId,
                                             FactorName = q.FactorName,
                                             Score = ts.SAFETY_FACTOR_SCORE,
                                             Date = ts.EVALUATION_DATETIME
                                         };
                
                // 查询最后评分ID
                var ids =
                    themeScore.GroupBy(r => new { r.StructId, r.FactorId, r.FactorName })
                        .Select(i => i.Select(s => s.Id).Max())
                        .ToArray();

                var scores = themeScore.Where(r => ids.Contains(r.Id));

                // 查询主题评分
                var rslt = from q in query
                           join w in scores on new { q.OrgStc, q.FactorId } equals
                               new { OrgStc = w.OrgStc, FactorId = w.FactorId } into score
                           from s in score.DefaultIfEmpty()
                           select
                               new
                                   {                                       
                                       StructId = q.StructId,
                                       FactorId = q.FactorId,
                                       FactorName = q.FactorName,
                                       Score = s.Score,
                                       Date = s.Date
                                   };
                
                // 分数对应的文字
                var scoreWeight =
                    entity.T_FACT_WEIGHT_SCORE.Select(
                        w =>
                        new { Low = w.SCORE_VALUE_LOWER_LIMIT, Up = w.SCORE_VALUE_UPPER_LIMIT, Name = w.DESCRIPTION })
                        .ToList();
                
                var json =
                    new JArray(
                        rslt.ToList()
                            .GroupBy(g => g.StructId)
                            .Select(
                                s =>
                                new JObject(
                                    new JProperty("structId", s.Key),
                                    new JProperty(
                                    "entry",
                                    new JArray(
                                    s.GroupBy(e => new { e.FactorId, e.FactorName })
                                    .Select(
                                        e => new JObject(
                                                 new JProperty("factorId", e.Key.FactorId),
                                            new JProperty("factorName", e.Key.FactorName),
                                            new JProperty(
                                            "status",
                                            scoreWeight.Where(
                                                w =>
                                                w.Low
                                                <= s.Where(
                                                    d => d.Date == e.Max(v => v.Date) && d.FactorId == e.Key.FactorId)
                                                       .Select(d => d.Score)
                                                       .FirstOrDefault()
                                                && w.Up
                                                >= s.Where(
                                                    d => d.Date == e.Max(v => v.Date) && d.FactorId == e.Key.FactorId)
                                                       .Select(d => d.Score)
                                                       .FirstOrDefault()).Select(w => w.Name).FirstOrDefault()))))))));
                return json;
                #endregion

                #region 告警产生状态-非使用中

                //var query = from sf in entity.T_DIM_STRUCTURE_FACTOR
                //            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                //            from s in entity.T_DIM_SENSOR
                //            from fp in entity.T_DIM_SAFETY_FACTOR_TYPE
                //            where
                //                strcts.Contains(sf.STRUCTURE_ID) && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                //                && s.STRUCT_ID == sf.STRUCTURE_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                //                && f.SAFETY_FACTOR_TYPE_PARENT_ID == fp.SAFETY_FACTOR_TYPE_ID
                //            select
                //                new
                //                    {
                //                        structId = s.STRUCT_ID,
                //                        factorId = fp.SAFETY_FACTOR_TYPE_ID,
                //                        factorName = fp.SAFETY_FACTOR_TYPE_NAME,
                //                        sensorId = s.SENSOR_ID
                //                    };
                //var device = query.ToList();

                //var sensors = device.Select(d => d.sensorId).ToArray();
                //var warnings = new DAL.Warning().GetWarningsBySensor(
                //    sensors,
                //    DateTime.Now.AddYears(-1),
                //    DateTime.Now,
                //    false);

                //var statusMap = new Dictionary<int, string>() { { 1, "差" }, { 2, "劣" }, { 3, "中" }, { 4, "良" } };

                //var rslt =
                //    device.GroupBy(s => s.structId)
                //        .Select(
                //            s =>
                //            new
                //                {
                //                    structId = s.Key,
                //                    entry =
                //                s.GroupBy(f => new { f.factorId, f.factorName })
                //                .Select(
                //                    f =>
                //                    new
                //                        {
                //                            factorId = f.Key.factorId,
                //                            factorName = f.Key.factorName,
                //                            status =
                //                        (warnings.AsEnumerable()
                //                            .Where(
                //                                w =>
                //                                f.Select(sen => sen.sensorId)
                //                                    .ToArray()
                //                                    .Contains(w.Field<int>("SensorId")))
                //                            .Min(w => w.Field<int?>("Level"))) == null
                //                            ? "优"
                //                            : statusMap[
                //                                (warnings.AsEnumerable()
                //                                  .Where(
                //                                      w =>
                //                                      f.Select(sen => sen.sensorId)
                //                                          .ToArray()
                //                                          .Contains(w.Field<int>("SensorId")))
                //                                  .Min(w => w.Field<int>("Level")))]
                //                        })
                //                });
                //return rslt;

                #endregion
            }
        }

        /// <summary>
        /// 根据结构物类型获取监测因子
        /// </summary>
        /// <param name="structTypeId"> 结构物类型编号 </param>
        /// <returns> 监测因子 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物类型下的监测因素", false)]
        public object FindFactorsByStructType(string structTypeId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from sf in entity.T_DIM_STRUCTURE_TYPE_FACTOR
                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                            where sf.STRUCTURE_TYPE_ID == structTypeId && sf.FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                            select f;
                var queryAll = from q in query
                               join f in entity.T_DIM_SAFETY_FACTOR_TYPE on q.SAFETY_FACTOR_TYPE_PARENT_ID equals
                                   f.SAFETY_FACTOR_TYPE_ID
                               select f;
                var list = query.Union(queryAll).ToList();

                return
                    new JArray(
                        list.Where(l => l.SAFETY_FACTOR_TYPE_PARENT_ID == 0)
                            .Select(
                                l =>
                                new JObject(
                                    new JProperty("factorId", l.SAFETY_FACTOR_TYPE_ID),
                                    new JProperty("factorName", l.SAFETY_FACTOR_TYPE_NAME),
                                    new JProperty(
                                    "children",
                                    new JArray(
                                    list.Where(c => l.SAFETY_FACTOR_TYPE_ID == c.SAFETY_FACTOR_TYPE_PARENT_ID)
                                    .Select(
                                        c =>
                                        new JObject(
                                            new JProperty("factorId", c.SAFETY_FACTOR_TYPE_ID),
                                            new JProperty("factorName", c.SAFETY_FACTOR_TYPE_NAME),
                                            new JProperty("description", c.DESCRIPTION ?? c.SAFETY_FACTOR_TYPE_NAME))))))));
            }
        }

        /// <summary>
        /// 根据结构物获取监测因子配置
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 因子配置 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物监测因素配置", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object FindFactorsConfigByStruct(int structId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_STRUCTURE
                            from stf in entity.T_DIM_STRUCTURE_TYPE_FACTOR
                            join sf in entity.T_DIM_STRUCTURE_FACTOR on new { s = s.ID, f = stf.FACTOR_TYPE_ID, } equals
                                new { s = sf.STRUCTURE_ID, f = sf.SAFETY_FACTOR_TYPE_ID } into config
                            from c in config.DefaultIfEmpty()
                            from f1 in entity.T_DIM_SAFETY_FACTOR_TYPE 
                            from f2 in entity.T_DIM_SAFETY_FACTOR_TYPE
                            where s.STRUCTURE_TYPE_ID == stf.STRUCTURE_TYPE_ID && s.ID == structId
                            && stf.FACTOR_TYPE_ID == f1.SAFETY_FACTOR_TYPE_ID
                            && f1.SAFETY_FACTOR_TYPE_PARENT_ID == f2.SAFETY_FACTOR_TYPE_ID
                            select new
                                       {
                                           parentId = f2.SAFETY_FACTOR_TYPE_ID,
                                           parentName = f2.SAFETY_FACTOR_TYPE_NAME,
                                           factorId = stf.FACTOR_TYPE_ID, 
                                           factorName = f1.SAFETY_FACTOR_TYPE_NAME,
                                           description = f1.DESCRIPTION,
                                           choose = (int?)c.ID != null
                                       };
                return
                    query.ToList()
                        .GroupBy(f => new { f.parentId, f.parentName })
                        .Select(
                            p =>
                            new JObject(
                                new JProperty("factorId", p.Key.parentId),
                                new JProperty("factorName", p.Key.parentName),
                                new JProperty(
                                "children",
                                p.Select(
                                    c =>
                                    new JObject(
                                        new JProperty("factorId", c.factorId),
                                        new JProperty("factorName", c.factorName),
                                        new JProperty("description", c.description),
                                        new JProperty("choose", c.choose))))));
            }
        }

        /// <summary>
        /// 配置结构物监测因子
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factors"> 监测因子列表 </param>
        /// <returns> 配置结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("配置结构物监测因素", true)]
        [Authorization(AuthorizationCode.S_Structure_Factor_Modify)]
        public HttpResponseMessage ConfigStructFactors([FromUri] int structId, [FromBody] string array)
        {
            int[] factors = array.Split(',').Select(f => Convert.ToInt32(f)).ToArray();

            using (var entity = new SecureCloud_Entities())
            {
                var ori =
                    entity.T_DIM_STRUCTURE_FACTOR.Where(s => s.STRUCTURE_ID == structId)
                        .Select(s => s.SAFETY_FACTOR_TYPE_ID).ToList();

                var arrToAdd = factors.Except(ori);
                var arrToDel = ori.Except(factors);

                var sens =
                    entity.T_DIM_SENSOR.Where(s => s.STRUCT_ID == structId && arrToDel.Contains((int)s.SAFETY_FACTOR_TYPE_ID) && !s.IsDeleted);
                if (sens.Any())
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Conflict,
                        StringHelper.GetMessageString("请先删除该监测因素下的传感器"));
                }

                foreach (int factorId in arrToAdd)
                {
                    var sf = new T_DIM_STRUCTURE_FACTOR();
                    sf.STRUCTURE_ID = structId;
                    sf.SAFETY_FACTOR_TYPE_ID = factorId;

                    var entry = entity.Entry(sf);
                    entry.State = System.Data.EntityState.Added;
                }

                var queryDelete =
                    entity.T_DIM_STRUCTURE_FACTOR.Where(
                        s => s.STRUCTURE_ID == structId && arrToDel.Contains(s.SAFETY_FACTOR_TYPE_ID));
                foreach (var config in queryDelete)
                {
                    var entry = entity.Entry(config);
                    entry.State = System.Data.EntityState.Deleted;
                }

                #region 日志信息

                var stc =
                    entity.T_DIM_STRUCTURE.Where(s => s.ID == structId)
                        .Select(s => s.STRUCTURE_NAME_CN)
                        .FirstOrDefault();

                var fac =
                    entity.T_DIM_SAFETY_FACTOR_TYPE.Where(f => factors.Contains(f.SAFETY_FACTOR_TYPE_ID))
                        .Select(f => f.SAFETY_FACTOR_TYPE_NAME);

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(array);
                this.Request.Properties["ActionParameterShow"] = string.Format(
                    "结构物:{0},监测因素修改为:{1}",
                    stc ?? string.Empty,
                    string.Join(",", fac));
                #endregion

                try
                {
                    entity.Configuration.AutoDetectChangesEnabled = false;
                    entity.Configuration.ValidateOnSaveEnabled = false;
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("配置成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("配置失败"));
                }
                finally
                {
                    entity.Configuration.AutoDetectChangesEnabled = true;
                    entity.Configuration.ValidateOnSaveEnabled = true;
                }
            }
        }
    }

    class FactorList
    {
        public int StructId { get; set; }

        public int? OrgStc { get; set; }

        public int? FactorId { get; set; }

        public string FactorName { get; set; }
    }
}
