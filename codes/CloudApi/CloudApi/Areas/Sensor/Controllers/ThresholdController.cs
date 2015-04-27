using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    using System.Configuration;
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;    

    public class ThresholdController : ApiController
    {
        private readonly string gps = ConfigurationManager.AppSettings["GPSBaseStation"];

        /// <summary>
        /// 获取传感器阈值
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <returns>阈值列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器阈值", false)]
        public object FindThresholdBySensor(int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var sq = from s in db.T_DIM_SENSOR
                         from p in db.T_DIM_SENSOR_PRODUCT
                         where s.SENSOR_ID == sensorId && s.IsDeleted == false
                         && s.PRODUCT_SENSOR_ID == p.PRODUCT_ID && p.PRODUCT_NAME != gps
                         select s;
                // 归属的传感器
                var sensor = sq.FirstOrDefault();

                if (sensor == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器id无效"));
                }
                var structId = Config.GetStructId(sensorId);
                //修改
                // 监测因素
                var factors = Config.GetConfigByFactors(new[] { Convert.ToInt32(sensor.SAFETY_FACTOR_TYPE_ID) },structId);

                var sensorDetails = from f in factors
                                    where sensor.SAFETY_FACTOR_TYPE_ID == f.Id
                                    select
                                        new
                                        {
                                            sensorId = sensor.SENSOR_ID,
                                            location = sensor.SENSOR_LOCATION_DESCRIPTION,
                                            items = f.Display
                                        };

                var list = new List<SensorThreshold>();
                foreach (var sensorDetail in sensorDetails)
                {
                    int i = 1;
                    foreach (var item in sensorDetail.items)
                    {
                        list.Add(
                            new SensorThreshold
                            {
                                SensorId = sensorDetail.sensorId,
                                Location = sensorDetail.location,
                                ItemId = i,
                                ItemName = item
                            });
                        i++;
                    }
                }

                // 查询阈值
                var query = from l in list
                            from t in db.T_FACT_SENSOR_THRESHOLD
                            where t.SensorId == l.SensorId && t.ItemId == l.ItemId
                            select
                                new
                                {
                                    l.SensorId,
                                    l.ItemId,
                                    t.ThresholdLevel,
                                    Value =
                            t.ThresholdDownValue == null || t.ThresholdUpValue == null
                                ? null
                                : "("
                                  + (t.ThresholdDownValue == double.MinValue ? "-" : t.ThresholdDownValue.ToString())
                                  + ","
                                  + (t.ThresholdUpValue == double.MaxValue ? "+" : t.ThresholdUpValue.ToString())
                                  + ")"
                                };
                var thresholds = query.ToList();

                // 结果
                var rslt =
                    list.GroupBy(l => new { l.SensorId, l.Location, l.ItemId, l.ItemName })
                        .Select(
                            v =>
                            new SensorThreshold
                                {
                                    SensorId = v.Key.SensorId,
                                    Location = v.Key.Location,
                                    ItemId = v.Key.ItemId,
                                    ItemName = v.Key.ItemName,
                                    Threshold =
                                        new[]
                                            {
                                                new ThresholdModel
                                                    {
                                                        Level = 1,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 1)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    },
                                                new ThresholdModel
                                                    {
                                                        Level = 2,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 2)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    },
                                                new ThresholdModel
                                                    {
                                                        Level = 3,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 3)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    },
                                                new ThresholdModel
                                                    {
                                                        Level = 4,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 4)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    }
                                            }
                                });

                return rslt;
            }
        }

        /// <summary>
        /// 获取结构物下监测因素的传感器阈值
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factorId"> 监测子因素编号 </param>
        /// <returns> 阈值列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下监测因素的传感器阈值", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object FindThresholdByStructAndFactor(int structId, int factorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var sq = from s in db.T_DIM_SENSOR
                         from p in db.T_DIM_SENSOR_PRODUCT
                         where
                             s.STRUCT_ID == structId && s.SAFETY_FACTOR_TYPE_ID == factorId && s.IsDeleted == false
                             && s.PRODUCT_SENSOR_ID == p.PRODUCT_ID && p.PRODUCT_NAME != gps
                         select s;
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
                                                items = f.Display
                                            };

                // 传感器监测项
                var list = new List<SensorThreshold>();
                foreach (var sensorDetail in sensorDetails)
                {
                    int i = 1;
                    foreach (var item in sensorDetail.items)
                    {
                        list.Add(
                            new SensorThreshold
                                {
                                    SensorId = sensorDetail.sensorId,
                                    Location = sensorDetail.location,
                                    ItemId = i,
                                    ItemName = item
                                });
                        i++;
                    }
                }

                // 查询阈值
                var query = from l in list
                            from t in db.T_FACT_SENSOR_THRESHOLD
                            where t.SensorId == l.SensorId && t.ItemId == l.ItemId
                            select
                                new
                                    {
                                        l.SensorId,
                                        l.ItemId,
                                        t.ThresholdLevel,
                                        Value =
                                t.ThresholdDownValue == null || t.ThresholdUpValue == null
                                    ? null
                                    : "("
                                      + (t.ThresholdDownValue == double.MinValue ? "-" : t.ThresholdDownValue.ToString())
                                      + ","
                                      + (t.ThresholdUpValue == double.MaxValue ? "+" : t.ThresholdUpValue.ToString())
                                      + ")"
                                    };
                var thresholds = query.ToList();

                // 结果
                var rslt =
                    list.GroupBy(l => new { l.SensorId, l.Location, l.ItemId, l.ItemName })
                        .Select(
                            v =>
                            new SensorThreshold
                                {
                                    SensorId = v.Key.SensorId,
                                    Location = v.Key.Location,
                                    ItemId = v.Key.ItemId,
                                    ItemName = v.Key.ItemName,
                                    Threshold =
                                        new[]
                                            {
                                                new ThresholdModel
                                                    {
                                                        Level = 1,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 1)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    },
                                                new ThresholdModel
                                                    {
                                                        Level = 2,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 2)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    },
                                                new ThresholdModel
                                                    {
                                                        Level = 3,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 3)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    },
                                                new ThresholdModel
                                                    {
                                                        Level = 4,
                                                        Value =
                                                            String.Join(
                                                                ";",
                                                                thresholds.Where(
                                                                    t =>
                                                                    t.SensorId
                                                                    == v.Key.SensorId
                                                                    && t.ItemId
                                                                    == v.Key.ItemId
                                                                    && t.ThresholdLevel == 4)
                                                            .Select(t => t.Value)
                                                            .ToArray())
                                                    }
                                            }
                                });
                           
                return rslt;
            }
        }

        /// <summary>
        /// 配置传感器阈值
        /// </summary>
        /// <param name="model">阈值列表</param>
        /// <returns>配置结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("配置传感器阈值", true)]
        [Authorization(AuthorizationCode.S_Structure_Threshold)]
        public HttpResponseMessage ConfigSensorThreshold(IList<SensorThreshold> model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var sensorThreshold in model)
                    {
                        //日志信息
                        var sensor = db.T_DIM_SENSOR.FirstOrDefault(s => s.SENSOR_ID == sensorThreshold.SensorId);
                        string sensorName = string.Empty;
                        string itemName = null;
                        if (sensor != null)
                        {
                            var structId = Config.GetStructId(sensorThreshold.SensorId);
                            sensorName = sensor.SENSOR_LOCATION_DESCRIPTION;
                            var factor =
                                Config.GetConfigByFactors(new[] { Convert.ToInt32(sensor.SAFETY_FACTOR_TYPE_ID) },structId)
                                    .FirstOrDefault();
                            if (factor != null && sensorThreshold.ItemId < factor.Display.Count())
                            {
                                itemName = factor.Display[sensorThreshold.ItemId];
                            }
                        }

                        sb.AppendFormat("传感器:{0}-", sensorName);
                        sb.AppendFormat("监测项:{0}_", itemName ?? sensorThreshold.ItemId.ToString());                        

                        foreach (var th in sensorThreshold.Threshold)
                        {
                            SensorThreshold threshold = sensorThreshold;
                            ThresholdModel th1 = th;
                            var old =
                                db.T_FACT_SENSOR_THRESHOLD.Where(
                                    st =>
                                    st.SensorId == threshold.SensorId && st.ItemId == threshold.ItemId
                                    && st.ThresholdLevel == th1.Level);
                            if (old.Any())
                            {
                                List<string> values = new List<string>(old.Count());
                                foreach (var o in old)
                                {
                                    var entry = db.Entry(o);
                                    values.Add(
                                        string.Format(
                                            "({0},{1})",
                                            o.ThresholdDownValue == double.MinValue
                                                ? "-"
                                                : o.ThresholdDownValue.ToString(),
                                            o.ThresholdUpValue == double.MaxValue ? "+" : o.ThresholdUpValue.ToString()));
                                    entry.State = System.Data.EntityState.Deleted;
                                }
                                if (!string.IsNullOrEmpty(th.Value))
                                {
                                    sb.AppendFormat("[{0}级阈值从{1}改为:{2}]", th.Level, string.Join(";", values), th.Value);
                                }
                                else
                                {
                                    sb.AppendFormat("[{0}级阈值从{1}改为空]", th.Level, string.Join(";", values));
                                }
                            }

                            if (String.IsNullOrEmpty(th1.Value))
                            {
                                continue;
                            }
                            
                            foreach (var value in th1.Value.Split(';'))
                            {
                                var t = new T_FACT_SENSOR_THRESHOLD();
                                t.SensorId = sensorThreshold.SensorId;
                                t.ItemId = sensorThreshold.ItemId;
                                t.ThresholdLevel = th1.Level;
                                var v = value.Split(new[] { '(', ',', ')' });
                                t.ThresholdDownValue = v[1] == "-" ? double.MinValue : double.Parse(v[1]);
                                t.ThresholdUpValue = v[2] == "+" ? double.MaxValue : double.Parse(v[2]);
                                var entry = db.Entry(t);
                                entry.State = System.Data.EntityState.Added;
                            }
                        }
                        sb.Append(";");
                    }

                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = sb.ToString();

                    #endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("配置成功"));
                }
                catch (FormatException)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("参数无效"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("配置保存失败"));
                }
            }
        }

        /// <summary>
        /// 配置所有传感器阈值
        /// </summary>
        /// <param name="model">阈值列表</param>
        /// <returns>配置结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("配置监测因素下所有传感器阈值", true)]
        [Authorization(AuthorizationCode.S_Structure_Threshold)]
        public HttpResponseMessage ConfigAllSensorThreshold(FactorThreshold model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    // 归属的传感器
                    var sensors =
                        db.T_DIM_SENSOR.Where(
                            s => s.STRUCT_ID == model.StructId && s.SAFETY_FACTOR_TYPE_ID == model.FactorId).ToList();

                    // 监测因素
                    var factors = Config.GetConfigByFactors(
                        sensors.Select(s => s.SAFETY_FACTOR_TYPE_ID == null ? -1 : Convert.ToInt32(s.SAFETY_FACTOR_TYPE_ID))
                            .Distinct()
                            .Where(f => f > 0)
                            .ToArray(),model.StructId);

                    var sensorDetails = from s in sensors
                                        from f in factors
                                        where s.SAFETY_FACTOR_TYPE_ID == f.Id
                                        select
                                            new
                                            {
                                                sensorId = s.SENSOR_ID,
                                                location = s.SENSOR_LOCATION_DESCRIPTION,
                                                items = f.Display
                                            };

                    // 传感器监测项
                    var list = new List<SensorThreshold>();
                    foreach (var sensorDetail in sensorDetails)
                    {
                        int i = 1;
                        foreach (var item in sensorDetail.items)
                        {
                            list.Add(
                                new SensorThreshold
                                {
                                    SensorId = sensorDetail.sensorId,
                                    ItemId = i                                    
                                });
                            i++;
                        }
                    }

                    foreach (var threshold in model.Threshold) // 遍历所有等级
                    {
                        foreach (var item in list) // 遍历所有传感器
                        {
                            SensorThreshold i = item;
                            ThresholdModel th = threshold;
                            var old =
                                db.T_FACT_SENSOR_THRESHOLD.Where(
                                    t =>
                                    t.SensorId == i.SensorId && t.ItemId == i.ItemId && t.ThresholdLevel == th.Level);

                            if (old.Any()) // 已存在配置
                            {
                                foreach (var o in old)
                                {
                                    var entry = db.Entry(o);
                                    entry.State = System.Data.EntityState.Deleted;
                                }
                            }
                            if (!string.IsNullOrEmpty(threshold.Value)) // 该等级阈值不为空
                            {
                                foreach (var value in threshold.Value.Split(';')) // 遍历阈值内容
                                {
                                    var v = value.Split(new[] { '(', ',', ')' });
                                    var data2Insert = new T_FACT_SENSOR_THRESHOLD();
                                    data2Insert.SensorId = i.SensorId;
                                    data2Insert.ItemId = i.ItemId;
                                    data2Insert.ThresholdLevel = th.Level;
                                    data2Insert.ThresholdDownValue = v[1] == "-" ? double.MinValue : double.Parse(v[1]);
                                    data2Insert.ThresholdUpValue = v[2] == "+" ? double.MaxValue : double.Parse(v[2]);

                                    var entry2 = db.Entry(data2Insert);
                                    entry2.State = System.Data.EntityState.Added;
                                }
                            }
                        }
                    }

                    #region 日志信息

                    var stc =
                        db.T_DIM_STRUCTURE.Where(s => s.ID == model.StructId)
                            .Select(s => s.STRUCTURE_NAME_CN)
                            .FirstOrDefault();

                    var fac =
                        db.T_DIM_SAFETY_FACTOR_TYPE.Where(f => f.SAFETY_FACTOR_TYPE_ID == model.FactorId)
                            .Select(f => f.SAFETY_FACTOR_TYPE_NAME)
                            .FirstOrDefault();

                    string values = "空";
                    if (model.Threshold != null)
                    {
                        values = string.Join(
                            ";",
                            model.Threshold.Select(
                                t => string.Format("[{0}级:{1}]", t.Level, t.Value ?? "空")));
                    }

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = string.Format(
                        "结构物：{0},监测因素：{1},阈值设为{2}",
                        stc,
                        fac,
                        values);
                    #endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("配置成功"));
                }
                catch (FormatException ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("参数无效"+ex.StackTrace));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("配置保存失败"));
                }
            }
        }
    }

    public class ThresholdModel
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class SensorThreshold
    {
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("threshold")]
        public IList<ThresholdModel> Threshold { get; set; }
    }

    public class FactorThreshold
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("factorId")]
        public int FactorId { get; set; }

        [JsonProperty("threshold")]
        public IList<ThresholdModel> Threshold { get; set; }
    }    
}
