using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
using Newtonsoft.Json;
using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    using System.Text;

    public class SensorFilterController : ApiController
    {
        private readonly string gps = ConfigurationManager.AppSettings["GPSBaseStation"];
        /// <summary>
        /// 获取传感器过滤配置
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <param name="factorId">监测子因素编号</param>
        /// <returns>过滤配置列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下监测因素的传感器过滤配置", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public List<FilterConfig> FindFilterConfigByStructAndFactor(int structId, int factorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var sensor = db.T_DIM_SENSOR.ToList();

                var sq = from s in db.T_DIM_SENSOR 
                              from p in db.T_DIM_SENSOR_PRODUCT
                              where
                                       s.STRUCT_ID == structId && s.SAFETY_FACTOR_TYPE_ID == factorId && s.IsDeleted == false && 
                                       s.PRODUCT_SENSOR_ID == p.PRODUCT_ID && p.PRODUCT_NAME != gps
                              select s;

                var sql = sq.ToString();
                // 归属的传感器
                var sensors = sq.ToList();
                // 监测因素
                var factors = Config.GetConfigByFactors(
                    sensors.Select(s => s.SAFETY_FACTOR_TYPE_ID == null ? -1 : Convert.ToInt32(s.SAFETY_FACTOR_TYPE_ID))
                        .Distinct()
                        .Where(f => f > 0)
                        .ToArray(),structId);
                var sensorDetails = from s in sensors
                                    from f in factors
                                    where s.SAFETY_FACTOR_TYPE_ID == f.Id
                                    select
                                        new
                                        {
                                            sensorId = s.SENSOR_ID,
                                            location = s.SENSOR_LOCATION_DESCRIPTION,
                                            //columns=f.Columns,
                                            items = f.Display
                                        };
                var sensorList = new List<SensorInfo>();
                foreach (var sensorDetail in sensorDetails)
                {
                    int i = 1;
                    foreach (var item in sensorDetail.items)
                    {
                        sensorList.Add(
                            new SensorInfo
                            {
                                SensorId = sensorDetail.sensorId,
                                Location = sensorDetail.location,
                                ItemId = i,
                                ItemName = item
                            });
                        i++;
                    }
                }

                var slr = from s in sensorList.ToList()
                          join r in db.T_DATA_RATIONAL_FILTER_CONFIG
                              on new {SensorId = s.SensorId, ItemId = s.ItemId}
                              equals new {SensorId = r.SensorId, ItemId = r.ItemId}
                              into x
                          from cx in x.DefaultIfEmpty()
                          select new
                          {
                              sensorId = s.SensorId,
                              location = s.Location,
                              itemId = s.ItemId,
                              itemName = s.ItemName,
                              rvEnable = (cx == null ? null : (bool?)cx.Enabled),
                              rvLower =(cx==null ? null : (decimal?)cx.RationalLower),
                              rvUpper =  (cx==null?null:(decimal?)cx.RationalUpper)
                          };
                var sls = from s in sensorList
                          join r in db.T_DATA_STABLE_FILTER_CONFIG
                              on new {SensorId = s.SensorId, ItemId = s.ItemId}
                              equals new {SensorId = r.SensorId, ItemId = r.ItemId}
                              into x
                          from cx in x.DefaultIfEmpty()
                          select new
                          {
                              sensorId = s.SensorId,
                              location = s.Location,
                              itemId = s.ItemId,
                              itemName = s.ItemName,
                              svEnable = (cx==null?null:(bool?)cx.Enabled),
                              svWindowSize = (cx==null? null : (int?)cx.WindowSize),
                              svKt = (cx==null?null:(decimal?)cx.KT),
                              svDt=(cx==null?null:(int?)cx.DT),
                              svRt=(cx==null?null:(int?)cx.RT)
                          };

                var list = from r in slr.ToList()
                           from s in sls.ToList()
                           where r.sensorId == s.sensorId && r.itemId == s.itemId
                           select new FilterConfig()
                           {
                               SensorId = s.sensorId,
                               Location = s.location,
                               ItemId = s.itemId,
                               ItemName = s.itemName,
                               RvEnable = r.rvEnable,
                               RvLower = r.rvLower,
                               RvUpper = r.rvUpper,
                               SvEnabled = s.svEnable,
                               SvWindowSize = s.svWindowSize,
                               SvKt = s.svKt,
                               SvDt = s.svDt,
                               SvRt = s.svRt
                           };
                    
                return list.ToList();
            }
        }

        [AcceptVerbs("Post")]
        [LogInfo("配置传感器过滤", true)]
        [Authorization(AuthorizationCode.S_Structure_DatavalRESOURCE_ID)]
        public HttpResponseMessage RegisterFilterInfo([FromBody] ConfigInfo config)
        {
            using (var db = new SecureCloud_Entities())
            {
                var sensors = db.T_DIM_SENSOR.FirstOrDefault(m => m.SENSOR_ID == config.SensorId);
                if (sensors == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                                                  StringHelper.GetMessageString("添加配置信息失败，传感器不存在"));
                }
                if (config.RvEnabled == true)
                {
                    if (config.RvLower == null || config.RvUpper == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                                                      StringHelper.GetMessageString("添加配置信息失败，上下限不能为空"));
                    }
                }
                if (config.SvEnabled == true)
                {
                    if (config.SvDt == null || config.SvKt == null || config.SvRt == null || config.SvWindowSize == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                                                      StringHelper.GetMessageString("添加配置信息失败，Stable其它参数不能为空"));
                    }
                }
                var structId = Config.GetStructId(config.SensorId);
                try
                {
                    var rational =
                        db.T_DATA_RATIONAL_FILTER_CONFIG.Where(m => m.SensorId == config.SensorId)
                          .FirstOrDefault(m => m.ItemId == config.ItemId);
                    if (rational != null)
                    {
                        rational.Enabled = config.RvEnabled;
                        rational.RationalLower = (config.RvLower ?? null);
                        rational.RationalUpper = (config.RvUpper??null);
                        var entryup = db.Entry(rational);
                        entryup.State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //新增rational过滤
                        var rFilterConfig = new T_DATA_RATIONAL_FILTER_CONFIG();
                        rFilterConfig.SensorId = config.SensorId;
                        rFilterConfig.ItemId = config.ItemId;
                        rFilterConfig.Enabled = config.RvEnabled;
                        rFilterConfig.RationalLower = (config.RvLower??null);
                        rFilterConfig.RationalUpper = (config.RvUpper??null);
                        var entry = db.Entry(rFilterConfig);
                        entry.State = EntityState.Added;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("配置失败"));
                }
                try
                {
                    var rational =
                        db.T_DATA_STABLE_FILTER_CONFIG.Where(m => m.SensorId == config.SensorId)
                          .FirstOrDefault(m => m.ItemId == config.ItemId);
                    if (rational != null)
                    {
                        rational.Enabled = config.SvEnabled;
                        rational.KT = (config.SvKt??null);
                        rational.DT = (config.SvDt??null);
                        rational.RT = (config.SvRt??null);
                        rational.WindowSize = (config.SvWindowSize??null);
                        var entryup = db.Entry(rational);
                        entryup.State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //新增stable过滤
                        var sFilterConfig = new T_DATA_STABLE_FILTER_CONFIG();
                        sFilterConfig.SensorId = config.SensorId;
                        sFilterConfig.ItemId = config.ItemId;
                        sFilterConfig.Enabled = config.SvEnabled;
                        sFilterConfig.WindowSize = (config.SvWindowSize??null);
                        sFilterConfig.DT = (config.SvDt??null);
                        sFilterConfig.KT = (config.SvKt??null);
                        sFilterConfig.RT = (config.SvRt??null);
                        var entry1 = db.Entry(sFilterConfig);
                        entry1.State = EntityState.Added;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("配置失败"));
                }

                #region 日志信息
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                var sensorName = string.Empty;
                var sensor = db.T_DIM_SENSOR.FirstOrDefault(s => s.SENSOR_ID == config.SensorId);
                var item = string.Empty;
                if (sensor != null)
                {
                    sensorName = sensor.SENSOR_LOCATION_DESCRIPTION;
                    if (sensor.SAFETY_FACTOR_TYPE_ID != null)
                    {
                        var factor =
                            Config.GetConfigByFactors(new[] { (int)sensor.SAFETY_FACTOR_TYPE_ID }, structId).FirstOrDefault();
                        if (factor != null && config.ItemId >= 0 && config.ItemId < factor.Display.Length)
                        {
                            item = factor.Display[config.ItemId];
                        }
                    }
                }
                this.Request.Properties["ActionParameterShow"] = string.Format(
                    "传感器：{0}，监测项：{1}，启用合理范围：{2}，合理范围下限：{3}，合理范围上限：{4}；启用滑窗：{5}，窗口大小：{6}，DT：{7}，KT：{8}，RT：{9}",
                        sensorName,
                        item,
                        config.RvEnabled,
                        config.RvLower,
                        config.RvUpper,
                        config.SvEnabled,
                        config.SvWindowSize,
                        config.SvDt,
                        config.SvKt,
                        config.SvRt);
                #endregion
                return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("配置成功"));
                
            }
        }
    }

    public class ConfigInfo
    {
        public int SensorId { get; set; }
        public int ItemId { get; set; }
        public bool RvEnabled { get; set; }
        public decimal? RvLower { get; set; }
        public decimal? RvUpper { get; set; }
        public bool SvEnabled { get; set; }
        public int?  SvWindowSize { get; set; }
        public decimal? SvKt { get; set; }
        public int?  SvDt { get; set; }
        public int? SvRt { get; set; }
    }

    public class FilterConfig
    {
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("rvEnabled")]
        public bool? RvEnable { get; set; }

        [JsonProperty("rvLower")]
        public decimal? RvLower { get; set; }

        [JsonProperty("rvUpper")]
        public decimal? RvUpper { get; set; }

        [JsonProperty("svEnabled")]
        public bool? SvEnabled { get; set; }

        [JsonProperty("svWindowSize")]
        public decimal? SvWindowSize { get; set; }

        [JsonProperty("svKt")]
        public decimal? SvKt { get; set; }

        [JsonProperty("svDt")]
        public decimal? SvDt { get; set; }

        [JsonProperty("svRt")]
        public decimal? SvRt { get; set; }
    }

    public class SensorInfo
    {
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }
    }
}
