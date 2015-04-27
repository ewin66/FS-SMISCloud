using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Weight.Controllers
{
    using System.Configuration;
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class WeightController : ApiController
    {
        /// <summary>
        /// <para> 获取权重配置进度 </para>
        /// <para> Get org/{orgId}/struct/{structId}/weight/progress </para>
        /// </summary>
        /// <param name="orgId"> 组织编号 </param>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns>权重配置进度</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物权重配置进度", false)]
        [Authorization(AuthorizationCode.S_Weight)]
        public object GetWeightProgress(int orgId, int structId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var factors = (from sf in entity.T_DIM_STRUCTURE_FACTOR
                               from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                               where sf.STRUCTURE_ID == structId && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                               select f.SAFETY_FACTOR_TYPE_PARENT_ID).ToList().Select(f => (int?)f).ToArray();

                // 主题总百分比
                var queryFac = from os in entity.T_DIM_ORG_STUCTURE
                               from fw in entity.T_FACT_SAFETY_FACTOR_WEIGHTS
                               where
                                   os.ORGANIZATION_ID == orgId && os.STRUCTURE_ID == structId
                                   && os.ORG_STRUC_ID == fw.ORG_STRUC_ID && factors.Contains(fw.SAFETY_FACTOR_TYPE_ID)
                               select fw.SAFETY_FACTOR_WEIGHTS;

                var facTotal = queryFac.ToList().Select(f => Convert.ToInt32(f)).Sum();

                // 子因素总百分比
                var querySub = from os in entity.T_DIM_ORG_STUCTURE
                               from sf in entity.T_DIM_STRUCTURE_FACTOR
                               from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                               join fw in entity.T_FACT_SUB_SAFETY_FACTOR_WEIGHTS on
                                   new
                                       {
                                           ORG_STRUC_ID = (int?)os.ORG_STRUC_ID,
                                           SAFETY_FACTOR_TYPE_ID = (int?)f.SAFETY_FACTOR_TYPE_ID
                                       } equals
                                   new { fw.ORG_STRUC_ID, fw.SAFETY_FACTOR_TYPE_ID } into w
                               from wei in w.DefaultIfEmpty()
                               where
                                   os.ORGANIZATION_ID == orgId && os.STRUCTURE_ID == structId
                                   && sf.STRUCTURE_ID == structId && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                               select
                                   new
                                       {
                                           factorId = f.SAFETY_FACTOR_TYPE_PARENT_ID,
                                           weight = wei.SUB_SAFETY_FACTOR_WEIGHTS ?? 0
                                       };

                var subTotal =
                    querySub.ToList()
                        .GroupBy(s => s.factorId)
                        .Select(
                            g => new { factorId = g.Key, subTotal = g.Select(k => Convert.ToInt32(k.weight)).Sum() });

                // 传感器总百分比
                var querySensor = from os in entity.T_DIM_ORG_STUCTURE
                                  from s in entity.T_DIM_SENSOR
                                  from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                                  from p in entity.T_DIM_SENSOR_PRODUCT
                                  join sw in entity.T_FACT_SENSOR_WEIGHTS on
                                      new { ORG_STRUC_ID = (int?)os.ORG_STRUC_ID, SENSOR_ID = (int?)s.SENSOR_ID } equals
                                      new { sw.ORG_STRUC_ID, sw.SENSOR_ID } into wei
                                  from w in wei.DefaultIfEmpty()
                                  where
                                      os.ORGANIZATION_ID == orgId && os.STRUCTURE_ID == structId
                                      && s.STRUCT_ID == os.STRUCTURE_ID
                                      && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                      && s.PRODUCT_SENSOR_ID == p.PRODUCT_ID && p.PRODUCT_NAME != gpsBase
                                      && !s.IsDeleted && s.Identification != 1
                                  select
                                      new
                                          {
                                              factorId = f.SAFETY_FACTOR_TYPE_PARENT_ID,
                                              subId = f.SAFETY_FACTOR_TYPE_ID,
                                              subName=f.SAFETY_FACTOR_TYPE_NAME,
                                              weight = w.SENSOR_WEIGHTS ?? 0
                                          };

                var sensorTotal =
                    querySensor.ToList()
                        .GroupBy(st => new { st.factorId, st.subId, st.subName })
                        .Select(
                            g =>
                            new
                                {
                                    g.Key.factorId,
                                    g.Key.subId,
                                    g.Key.subName,
                                    sensorTotal = g.Select(k => Convert.ToInt32(k.weight)).Sum()
                                });

                return new JObject(
                    new JProperty("facTotal", facTotal),
                    new JProperty(
                        "factor",
                        subTotal.OrderBy(sub => sub.factorId)
                            .Select(
                                sub =>
                                new JObject(
                                    new JProperty("factorId", sub.factorId),
                                    new JProperty("subTotal", sub.subTotal),
                                    new JProperty(
                                    "sub",
                                    sensorTotal.Where(s => s.factorId == sub.factorId)
                                    .OrderBy(s => s.subId)
                                    .Select(
                                        sen =>
                                        new JObject(
                                            new JProperty("subId", sen.subId),
                                              new JProperty("subName", sen.subName),
                                            new JProperty("sensorTotal", sen.sensorTotal))))))));
            }
        }

        /// <summary>
        /// 获取主题因素权重
        /// </summary>
        /// <param name="orgId"> 组织编号 </param>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 主题权重列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取监测主题权重", false)]
        [Authorization(AuthorizationCode.S_Weight)]
        public object GetFactorWeight(int orgId, int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from os in entity.T_DIM_ORG_STUCTURE
                            from sf in entity.T_DIM_STRUCTURE_FACTOR
                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE 
                            from f2 in entity.T_DIM_SAFETY_FACTOR_TYPE
                            join fw in entity.T_FACT_SAFETY_FACTOR_WEIGHTS on
                                new
                                    {
                                        ORG_STRUC_ID = (int?)os.ORG_STRUC_ID,
                                        SAFETY_FACTOR_TYPE_ID = (int?)f2.SAFETY_FACTOR_TYPE_ID
                                    } 
                            equals
                                new { fw.ORG_STRUC_ID, fw.SAFETY_FACTOR_TYPE_ID }
                            into w 
                            from wei in w.DefaultIfEmpty()
                            where
                                os.ORGANIZATION_ID == orgId && os.STRUCTURE_ID == structId
                                && sf.STRUCTURE_ID == structId && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                && f.SAFETY_FACTOR_TYPE_PARENT_ID == f2.SAFETY_FACTOR_TYPE_ID
                            select
                                new
                                    {
                                        factorId = f2.SAFETY_FACTOR_TYPE_ID,
                                        factorName = f2.SAFETY_FACTOR_TYPE_NAME,
                                        weight = wei.SAFETY_FACTOR_WEIGHTS ?? 0
                                    };
                return query.Distinct().ToList();
            }
        }

        /// <summary>
        /// 配置主题因素权重
        /// </summary>
        /// <param name="config"> 配置内容 </param>
        /// <returns> 配置结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("配置监测主题权重", true)]
        [Authorization(AuthorizationCode.S_Weight_Config)]
        public HttpResponseMessage AddFactorWeight([FromBody]FactorWeightModel[] config)
        {
            var sb = new StringBuilder(50);

            using (var entity = new SecureCloud_Entities())
            {
                foreach (FactorWeightModel m in config)
                {
                    FactorWeightModel model = m;
                    var orgStc = (from os in entity.T_DIM_ORG_STUCTURE
                                    where os.ORGANIZATION_ID == model.OrgId && os.STRUCTURE_ID == model.StructId
                                    select os).FirstOrDefault();
                    if (orgStc == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("组织下不存在该结构物"));
                    }
                    // 组织结构物代理键
                    var orgStcId = orgStc.ORG_STRUC_ID;
                    // 对应权重数据
                    var wei = (from fw in entity.T_FACT_SAFETY_FACTOR_WEIGHTS
                               where fw.ORG_STRUC_ID == orgStcId && fw.SAFETY_FACTOR_TYPE_ID == model.FactorId
                               select fw).FirstOrDefault();
                    if (wei == null) 
                    {
                        // 数据不存在,新增
                        var weight = new T_FACT_SAFETY_FACTOR_WEIGHTS();
                        weight.SAFETY_FACTOR_TYPE_ID = model.FactorId;
                        weight.SAFETY_FACTOR_WEIGHTS = (byte)model.Weight;
                        weight.ORG_STRUC_ID = orgStcId;                        

                        var entry = entity.Entry(weight);
                        entry.State = System.Data.EntityState.Added;
                    }
                    else 
                    {
                        // 数据已存在,修改
                        wei.SAFETY_FACTOR_WEIGHTS = (byte)model.Weight;
                        var entry = entity.Entry(wei);
                        entry.State = System.Data.EntityState.Modified;
                    }

                    #region 日志信息
                    var org =
                                    entity.T_DIM_ORGANIZATION.Where(o => o.ID == m.OrgId)
                                        .Select(o => o.ABB_NAME_CN)
                                        .FirstOrDefault();

                    var stc =
                        entity.T_DIM_STRUCTURE.Where(s => s.ID == m.StructId)
                            .Select(s => s.STRUCTURE_NAME_CN)
                            .FirstOrDefault();

                    var factor =
                        entity.T_DIM_SAFETY_FACTOR_TYPE.Where(f => f.SAFETY_FACTOR_TYPE_ID == m.FactorId)
                            .Select(f => f.SAFETY_FACTOR_TYPE_NAME)
                            .FirstOrDefault();

                    sb.AppendFormat(
                        "组织：{0},结构物：{1},监测主题：{2}，权重：{3};",
                        org ?? string.Empty,
                        stc ?? string.Empty,
                        factor ?? string.Empty,
                        m.Weight); 
                    #endregion
                }

                #region 日志信息

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                try
                {
                    entity.Configuration.AutoDetectChangesEnabled = false;
                    entity.Configuration.ValidateOnSaveEnabled = false;
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("配置成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("配置失败"));
                }
                finally
                {
                    entity.Configuration.AutoDetectChangesEnabled = true;
                    entity.Configuration.ValidateOnSaveEnabled = true;
                }
            }
        }

        /// <summary>
        /// 获取子因素权重
        /// </summary>
        /// <param name="orgId"> 组织编号 </param>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factorId"> 主题编号 </param>
        /// <returns> 子因素权重列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取监测因素权重", false)]
        [Authorization(AuthorizationCode.S_Weight)]
        public object GetSubFactorWeight(int orgId, int structId, int factorId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from os in entity.T_DIM_ORG_STUCTURE
                            from sf in entity.T_DIM_STRUCTURE_FACTOR
                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                            join fw in entity.T_FACT_SUB_SAFETY_FACTOR_WEIGHTS
                            on new {
                                ORG_STRUC_ID = (int?)os.ORG_STRUC_ID,
                                SAFETY_FACTOR_TYPE_ID = (int?)f.SAFETY_FACTOR_TYPE_ID
                            }
                            equals new { fw.ORG_STRUC_ID, fw.SAFETY_FACTOR_TYPE_ID }
                            into w
                            from wei in w.DefaultIfEmpty()
                            where
                                os.ORGANIZATION_ID == orgId && os.STRUCTURE_ID == structId
                                && sf.STRUCTURE_ID == structId && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                && f.SAFETY_FACTOR_TYPE_PARENT_ID == factorId 
                            select
                                new
                                    {
                                        subFactorId = f.SAFETY_FACTOR_TYPE_ID,
                                        subFactorName = f.SAFETY_FACTOR_TYPE_NAME,
                                        weight = wei.SUB_SAFETY_FACTOR_WEIGHTS ?? 0
                                    };
                return query.ToList();
            }
        }

        /// <summary>
        /// 配置子因素权重
        /// </summary>
        /// <param name="config"> 配置内容 </param>
        /// <returns> 配置结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("配置监测因素权重", true)]
        [Authorization(AuthorizationCode.S_Weight_Config)]
        public HttpResponseMessage AddSubFactorWeight([FromBody]SubFactorWeightModel[] config)
        {
            var sb = new StringBuilder(50);
            using (var entity = new SecureCloud_Entities())
            {
                foreach (SubFactorWeightModel m in config)
                {
                    SubFactorWeightModel model = m;
                    var orgStc = (from os in entity.T_DIM_ORG_STUCTURE
                                  where os.ORGANIZATION_ID == model.OrgId && os.STRUCTURE_ID == model.StructId
                                  select os).FirstOrDefault();
                    if (orgStc == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("组织下不存在该结构物"));
                    }
                    // 组织结构物代理键
                    var orgStcId = orgStc.ORG_STRUC_ID;
                    // 对应权重数据
                    var wei = (from fw in entity.T_FACT_SUB_SAFETY_FACTOR_WEIGHTS
                               where fw.ORG_STRUC_ID == orgStcId && fw.SAFETY_FACTOR_TYPE_ID == model.SubFactorId
                               select fw).FirstOrDefault();
                    if (wei == null) 
                    {
                        // 数据不存在,新增
                        var weight = new T_FACT_SUB_SAFETY_FACTOR_WEIGHTS();
                        weight.SAFETY_FACTOR_TYPE_ID = model.SubFactorId;
                        weight.SUB_SAFETY_FACTOR_WEIGHTS = (byte)model.Weight;
                        weight.ORG_STRUC_ID = orgStcId;

                        var entry = entity.Entry(weight);
                        entry.State = System.Data.EntityState.Added;
                    }
                    else
                    {
                        // 数据已存在,修改
                        wei.SUB_SAFETY_FACTOR_WEIGHTS = (byte)model.Weight;
                        var entry = entity.Entry(wei);
                        entry.State = System.Data.EntityState.Modified;
                    }

                    #region 日志信息
                    var org =
                                    entity.T_DIM_ORGANIZATION.Where(o => o.ID == m.OrgId)
                                        .Select(o => o.ABB_NAME_CN)
                                        .FirstOrDefault();

                    var stc =
                        entity.T_DIM_STRUCTURE.Where(s => s.ID == m.StructId)
                            .Select(s => s.STRUCTURE_NAME_CN)
                            .FirstOrDefault();

                    var factor =
                        entity.T_DIM_SAFETY_FACTOR_TYPE.Where(f => f.SAFETY_FACTOR_TYPE_ID == m.SubFactorId)
                            .Select(f => f.SAFETY_FACTOR_TYPE_NAME)
                            .FirstOrDefault();

                    sb.AppendFormat(
                        "组织：{0},结构物：{1},监测因素：{2}，权重：{3};",
                        org ?? string.Empty,
                        stc ?? string.Empty,
                        factor ?? string.Empty,
                        m.Weight);
                    #endregion
                }

                #region 日志信息

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                try
                {
                    entity.Configuration.AutoDetectChangesEnabled = false;
                    entity.Configuration.ValidateOnSaveEnabled = false;
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("配置成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("配置失败"));
                }
                finally
                {
                    entity.Configuration.AutoDetectChangesEnabled = true;
                    entity.Configuration.ValidateOnSaveEnabled = true;
                }
            }
        }

        /// <summary>
        /// 获取传感器权重
        /// </summary>
        /// <param name="orgId"> 组织编号 </param>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factorId"> 子因素编号 </param>
        /// <returns> 传感器权重列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器权重", false)]
        [Authorization(AuthorizationCode.S_Weight)]
        public object GetSensorWeight(int orgId, int structId, int factorId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (var entity = new SecureCloud_Entities())
            {
                var query = from os in entity.T_DIM_ORG_STUCTURE                            
                            from s in entity.T_DIM_SENSOR
                            from p in entity.T_DIM_SENSOR_PRODUCT
                            join sw in entity.T_FACT_SENSOR_WEIGHTS
                            on new { ORG_STRUC_ID = (int?)os.ORG_STRUC_ID, SENSOR_ID = (int?)s.SENSOR_ID }
                            equals new { sw.ORG_STRUC_ID, sw.SENSOR_ID }
                            into wei
                            from w in wei.DefaultIfEmpty()
                            where
                                os.ORGANIZATION_ID == orgId && os.STRUCTURE_ID == structId
                                && s.STRUCT_ID == os.STRUCTURE_ID
                                && s.SAFETY_FACTOR_TYPE_ID == factorId
                                && s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                && p.PRODUCT_NAME != gpsBase
                                && !s.IsDeleted && s.Identification != 1
                            select
                                new
                                {
                                    sensorId = s.SENSOR_ID,
                                    location = s.SENSOR_LOCATION_DESCRIPTION,
                                    weight = w.SENSOR_WEIGHTS ?? 0
                                };
                return query.ToList();
            }
        }

        /// <summary>
        /// 配置传感器权重
        /// </summary>
        /// <param name="config"> 配置内容 </param>
        /// <returns> 配置结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("配置传感器权重", true)]
        [Authorization(AuthorizationCode.S_Weight_Config)]
        public HttpResponseMessage AddSensorWeight([FromBody]SensorWeightModel[] config)
        {
            var sb = new StringBuilder(50);
            using (var entity = new SecureCloud_Entities())
            {
                foreach (SensorWeightModel m in config)
                {
                    SensorWeightModel model = m;
                    var orgStc = (from os in entity.T_DIM_ORG_STUCTURE
                                  where os.ORGANIZATION_ID == model.OrgId && os.STRUCTURE_ID == model.StructId
                                  select os).FirstOrDefault();
                    if (orgStc == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("组织下不存在该结构物"));
                    }
                    // 组织结构物代理键
                    var orgStcId = orgStc.ORG_STRUC_ID;
                    // 对应权重数据
                    var wei = (from sw in entity.T_FACT_SENSOR_WEIGHTS
                               where sw.ORG_STRUC_ID == orgStcId && sw.SENSOR_ID == model.SensorId
                               select sw).FirstOrDefault();
                    if (wei == null) 
                    {
                        // 数据不存在,新增
                        var weight = new T_FACT_SENSOR_WEIGHTS();
                        weight.SENSOR_ID = model.SensorId;
                        weight.SENSOR_WEIGHTS = (byte)model.Weight;
                        weight.ORG_STRUC_ID = orgStcId;

                        var entry = entity.Entry(weight);
                        entry.State = System.Data.EntityState.Added;
                    }
                    else
                    {
                        // 数据已存在,修改
                        wei.SENSOR_WEIGHTS = (byte)model.Weight;
                        var entry = entity.Entry(wei);
                        entry.State = System.Data.EntityState.Modified;
                    }

                    #region 日志信息
                    var org =
                                    entity.T_DIM_ORGANIZATION.Where(o => o.ID == m.OrgId)
                                        .Select(o => o.ABB_NAME_CN)
                                        .FirstOrDefault();

                    var stc =
                        entity.T_DIM_STRUCTURE.Where(s => s.ID == m.StructId)
                            .Select(s => s.STRUCTURE_NAME_CN)
                            .FirstOrDefault();

                    var sensor =
                        entity.T_DIM_SENSOR.Where(f => f.SENSOR_ID == m.SensorId)
                            .Select(f => f.SENSOR_LOCATION_DESCRIPTION)
                            .FirstOrDefault();

                    sb.AppendFormat(
                        "组织：{0},结构物：{1},传感器：{2}，权重：{3};",
                        org ?? string.Empty,
                        stc ?? string.Empty,
                        sensor ?? string.Empty,
                        m.Weight);
                    #endregion
                }

                #region 日志信息

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                try
                {
                    entity.Configuration.AutoDetectChangesEnabled = false;
                    entity.Configuration.ValidateOnSaveEnabled = false;
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("配置成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
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

    public class FactorWeightModel
    {
        public int OrgId { get; set; }

        public int StructId { get; set; }

        public int FactorId { get; set; }

        public int Weight { get; set; }
    }

    public class SubFactorWeightModel
    {
        public int OrgId { get; set; }

        public int StructId { get; set; }

        public int SubFactorId { get; set; }

        public int Weight { get; set; }
    }

    public class SensorWeightModel
    {
        public int OrgId { get; set; }

        public int StructId { get; set; }

        public int SensorId { get; set; }

        public int Weight { get; set; }
    }
}
