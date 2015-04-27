namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Section.Controllers
{
    using System;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SectionController : ApiController
    {
        /// <summary>
        /// 获取结构物的施工截面
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 施工截面列表 </returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Structure_Construct)]
        [Authorization(AuthorizationCode.U_Common)]
        public object GetSectionByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SECTION
                    where s.StructId == structId
                    select new
                    {
                        structName = s.T_DIM_STRUCTURE.STRUCTURE_NAME_CN,
                        sectionId = s.SectionId,
                        sectionName = s.SectionName,
                        sectionStatus = s.SectionStatus,
                        heapMapName = s.HeapMapName
                    };
                var list =
                    query.ToList().GroupBy(g => g.structName).Select(s =>
                        new JObject(
                            new JProperty("structName", s.Key),
                            new JProperty("sections", s.Select(v =>
                                new JObject(
                                    new JProperty("sectionId", v.sectionId),
                                    new JProperty("sectionName", v.sectionName),
                                    new JProperty("sectionStatus", v.sectionStatus),
                                    new JProperty("heapMapName", v.heapMapName))))));
                return list.FirstOrDefault(); // object or null.
            }
        }

        /// <summary>
        /// 获取单个施工截面信息
        /// </summary>
        /// <param name="sectionId"> 施工截面编号 </param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public object GetSection(int sectionId) 
        {
            using(var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SECTION
                            where s.SectionId == sectionId
                            select new 
                            {
                                sectionName = s.SectionName,
                                sectionStatus = s.SectionStatus,
                                heapMapName = s.HeapMapName
                            };
                var section = query.FirstOrDefault();
                if (section == null)
                {
                    return null;
                }
                return section;
            }
        }

        /// <summary>
        /// 新增结构物的施工截面
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="model"></param>
        /// <returns> 新增结果 </returns>
        [AcceptVerbs("Post")]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Add)]
        public object AddSection([FromUri] int structId, [FromBody] Section model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var structure = entity.T_DIM_STRUCTURE.FirstOrDefault(s => s.ID == structId);
                if (structure == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("结构物不存在，新增截面失败"));
                }
                if (entity.T_DIM_SECTION.Where(w => w.StructId == structId).Any(s => s.SectionName == model.SectionName)) 
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("该结构物已存在此截面名称，新增截面失败"));
                }

                var section = new T_DIM_SECTION
                {
                    SectionName = model.SectionName,
                    SectionStatus = model.SectionStatus,
                    HeapMapName = model.HeapMapName,
                    StructId = structId
                };

                var entry = entity.Entry(section);
                entry.State = EntityState.Added;

                try
                {
                    entity.SaveChanges();
                    return
                        new JObject(new JProperty("sectionId", section.SectionId)); // 200: "OK" (新增施工截面成功)
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("新增施工截面失败"));
                }
            }
        }

        /// <summary>
        /// 修改施工截面
        /// </summary>
        /// <param name="sectionId"> 施工截面编号 </param>
        /// <param name="model"> 施工截面待修改信息 </param>
        /// <returns> 修改结果 </returns>
        [AcceptVerbs("Post")]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public HttpResponseMessage ModifySection([FromUri] int sectionId, [FromBody] Section model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var section = entity.T_DIM_SECTION.FirstOrDefault(s => s.SectionId == sectionId);
                if (section == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("结构物不存在，修改施工截面信息失败"));
                }
                if (model.SectionName != default(string))
                {
                    section.SectionName = model.SectionName;
                }
                if (model.SectionStatus != default(int?))
                {
                    section.SectionStatus = model.SectionStatus;
                }
                if (model.HeapMapName != default(string))
                {
                    section.HeapMapName = model.HeapMapName;
                }

                var entry = entity.Entry(section);
                entry.State = EntityState.Modified;

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("修改施工截面信息成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改施工截面信息失败"));
                }
            }
        }

        /// <summary>
        /// 删除施工截面
        /// </summary>
        /// <param name="sectionId"> 施工截面编号 </param>
        /// <returns> 删除结果 </returns>
        [AcceptVerbs("Post")]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public HttpResponseMessage RemoveSection([FromUri] int sectionId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var section = entity.T_DIM_SECTION.FirstOrDefault(s => s.SectionId == sectionId);
                if (section == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("施工截面不存在，删除施工截面失败"));
                }

                // 删除传感器热点
                var hotspots = entity.T_DIM_HOTSPOT.Where(w => w.SECTION_ID == sectionId);
                foreach (var hotspot in hotspots)
                {
                    var entry1 = entity.Entry(hotspot);
                    entry1.State = EntityState.Deleted;
                }
                // 删除施工截面热点
                var sectionHotspot = entity.T_DIM_HOTSPOT_SECTION.Where(w => w.SectionId == sectionId);
                foreach (var shs in sectionHotspot)
                {
                    var entry2 = entity.Entry(shs);
                    entry2.State = EntityState.Deleted;
                }
                // 删除施工截面
                var entry = entity.Entry(section);
                entry.State = EntityState.Deleted;

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("删除施工截面成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除施工截面失败"));
                }
            }
        }

        /// <summary>
        /// 获取结构物下已配置热点的施工截面
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 已配置热点的施工截面列表 </returns>
        [AcceptVerbs("Get")]
        public object GetSectionHotSpotConfigByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SECTION
                    from hs in entity.T_DIM_HOTSPOT_SECTION
                    where s.StructId == structId
                        && s.SectionId == hs.SectionId
                    select new
                    {
                        hotspotId = hs.SpotId,
                        sectionId = s.SectionId,
                        sectionName = s.SectionName,
                        sectionStatus = s.SectionStatus,
                        heapMapName = s.HeapMapName,
                        sectionSpotX = hs.Spot_X_Axis,
                        sectionSpotY = hs.Spot_Y_Axis,
                        sectionSpotPath = hs.SpotPath,
                        structName = s.T_DIM_STRUCTURE.STRUCTURE_NAME_CN
                    };
                return query.ToList();
            }
        }

        /// <summary>
        /// 获取结构物下未配置热点的施工截面
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 未配置热点的施工截面列表 </returns>
        [AcceptVerbs("Get")]
        public object GetSectionHotSpotNonConfigByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SECTION
                    join hs in entity.T_DIM_HOTSPOT_SECTION on s.SectionId equals hs.SectionId into nhs
                    from ish in nhs.DefaultIfEmpty()
                    where s.StructId == structId
                        && (ish.Spot_X_Axis == null || ish.Spot_Y_Axis == null)
                    select new
                    {
                        sectionId = s.SectionId,
                        sectionName = s.SectionName
                    };
                return query.ToList();
            }
        }

        /// <summary>
        /// 添加结构物的施工截面热点配置
        /// </summary>
        /// <param name="config"> 施工截面热点配置 </param>
        /// <returns> 添加结果 </returns>
        [AcceptVerbs("Post")]
        public HttpResponseMessage AddSectionHotSpotConfig([FromBody]SectionHotSpotConfig config)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var section = entity.T_DIM_SECTION.FirstOrDefault(s => s.SectionId == config.SectionId);
                if (section == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("施工截面不存在，新增截面热点失败"));
                }

                var hotspot = new T_DIM_HOTSPOT_SECTION
                {
                    SectionId = config.SectionId,      
                    Spot_X_Axis = config.SectionSpotX, // xAxis
                    Spot_Y_Axis = config.SectionSpotY, // yAxis
                    SpotPath = config.SectionSpotPath 
                };

                var entry = entity.Entry(hotspot);
                entry.State = EntityState.Added;

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        new JObject(new JProperty("hotspotId", hotspot.SpotId)).ToString());
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("新增施工截面热点失败"));
                }
            }
        }

        /// <summary>
        /// 修改结构物的施工截面热点配置
        /// </summary>
        /// <param name="hotspotId"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public object ModifySectionHotSpotConfig([FromUri]int hotspotId, [FromBody]SectionHotSpotConfig config) 
        {
            using(var entity = new SecureCloud_Entities())
            {
                var hotspot = entity.T_DIM_HOTSPOT_SECTION.FirstOrDefault(s => s.SpotId == hotspotId);
                if (hotspot == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("截面热点不存在，修改失败"));
                }

                if (config.SectionId != default(int))
                {
                    hotspot.SectionId = config.SectionId;
                }
                if (config.SectionSpotX != default(decimal?))
                {
                    hotspot.Spot_X_Axis = config.SectionSpotX;
                }
                if (config.SectionSpotY != default(decimal?))
                {
                    hotspot.Spot_Y_Axis = config.SectionSpotY;
                }
                hotspot.SpotPath = config.SectionSpotPath == string.Empty ? null : config.SectionSpotPath;

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        new JObject(new JProperty("hotspotId", hotspotId)).ToString());
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改截面热点失败"));
                }
            }
        }

        /// <summary>
        /// 删除结构物的施工截面热点配置
        /// </summary>
        /// <param name="hotspots"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public HttpResponseMessage RemoveSectionHotspotConfig(string hotspots)
        {
            int[] arrHotspotId = hotspots.Split(',').Select(s => Convert.ToInt32(s)).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("共{0}条: ", arrHotspotId.Length);

            using (var entity = new SecureCloud_Entities())
            {
                foreach (var hotspotId in arrHotspotId)
                {
                    var hotspot = entity.T_DIM_HOTSPOT_SECTION.FirstOrDefault(s => s.SpotId == hotspotId);
                    if (hotspot == null)
                    {
                        return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("施工截面热点配置不存在，删除施工截面热点配置失败"));
                    }

                    #region 日志信息
                    sb.AppendFormat(" 截面:{0};", hotspot.T_DIM_SECTION.SectionName);
                    #endregion

                    DbEntityEntry<T_DIM_HOTSPOT_SECTION> entry = entity.Entry(hotspot);
                    entry.State = EntityState.Deleted;
                }

                #region 日志信息
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("删除施工截面热点配置成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除施工截面热点配置失败"));
                }
            }
        }
    }

    public class Section
    {
        public string SectionName { get; set; }

        public int? SectionStatus { get; set; }

        public string HeapMapName { get; set; }
    }

    public class SectionHotSpotConfig
    {
        public int SectionId { get; set; }

        public decimal? SectionSpotX { get; set; }

        public decimal? SectionSpotY { get; set; }

        public string SectionSpotPath { get; set; }
    }
}
