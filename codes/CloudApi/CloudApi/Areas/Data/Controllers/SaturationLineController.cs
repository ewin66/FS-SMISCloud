using System;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json.Linq;

    public class SaturationLineController : ApiController
    {
        /// <summary>
        /// 获取浸润线数据
        /// </summary>
        /// <param name="groupIds">分组</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取浸润线数据", false)]
        public object GetData(string groupIds, DateTime startDate, DateTime endDate)
        {
            int[] groups = groupIds.Split(',').Select(g => Convert.ToInt32(g)).ToArray();
            using (SecureCloud_Entities db = new SecureCloud_Entities())
            {
                var query = from s in db.T_DIM_SENSOR
                            from g in db.T_DIM_GROUP
                            from sg in db.T_DIM_SENSOR_GROUP_JINRUNXIAN
                            join d1 in (from d in db.T_THEMES_ENVI_SATURATION_LINE
                                        where d.ACQUISITION_DATETIME >= startDate && d.ACQUISITION_DATETIME <= endDate
                                        select d) on s.SENSOR_ID equals d1.SENSOR_ID into data
                            from d2 in data.DefaultIfEmpty()
                            where
                                s.SENSOR_ID == sg.SENSOR_ID && sg.GROUP_ID == g.GROUP_ID && !s.IsDeleted
                                && s.SAFETY_FACTOR_TYPE_ID == 34 && groups.Contains(sg.GROUP_ID)
                                && s.Identification != 1
                            select
                                new
                                    {
                                        GroupId = g.GROUP_ID,
                                        GroupName = g.GROUP_NAME,
                                        SensorId = s.SENSOR_ID,
                                        Location = s.SENSOR_LOCATION_DESCRIPTION,
                                        DataId = (int?)d2.ID,
                                        HoleDis = (decimal?)d2.HOLE_DIS,
                                        Height = (decimal?)d2.HEIGHT,
                                        AcquisitionTime = (DateTime?)d2.ACQUISITION_DATETIME
                                    };

                var list = query.ToList();

                var rslt =
                    list.GroupBy(g => new { g.GroupId, g.GroupName })
                        .Select(
                            g =>
                            new JObject(
                                new JProperty("groupId", g.Key.GroupId),
                                new JProperty("groupName", g.Key.GroupName),
                                new JProperty(
                                "items",
                                g.GroupBy(i => new { i.SensorId, i.Location })
                                .Select(
                                    d =>
                                    new JObject(
                                        new JProperty("sensorId", d.Key.SensorId),
                                        new JProperty("location", d.Key.Location),
                                        new JProperty(
                                        "data",
                                        d.FirstOrDefault().DataId == null
                                            ? new List<JObject>()
                                            : d.OrderBy(v => v.AcquisitionTime)
                                                  .Select(
                                                      v =>
                                                      new JObject(
                                                          new JProperty("holeDis", v.HoleDis),
                                                          new JProperty("height", v.Height),
                                                          new JProperty("acquisitiontime", v.AcquisitionTime)))))))));

                return rslt;
            }
        }

        /// <summary>
        /// 获取浸润线高度
        /// </summary>
        /// <param name="structId">结构物</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取浸润线高度", false)]
        public object GetHeight(int structId)
        {
            using (SecureCloud_Entities db = new SecureCloud_Entities())
            {
                // 传感器信息
                var sensors = (from s in db.T_DIM_SENSOR
                               from g in db.T_DIM_GROUP
                               from sg in db.T_DIM_SENSOR_GROUP_JINRUNXIAN
                               from st in db.T_DIM_STRUCTURE
                               where
                                   s.SENSOR_ID == sg.SENSOR_ID && sg.GROUP_ID == g.GROUP_ID && s.STRUCT_ID == st.ID
                                   && st.ID == structId && !s.IsDeleted && st.IsDelete == 0
                                   && s.SAFETY_FACTOR_TYPE_ID == 34 && s.Identification != 1
                               select
                                   new
                                       {
                                           SensorId = s.SENSOR_ID,
                                           GroupName = g.GROUP_NAME,
                                           Depth = sg.HEIGHT,
                                           Location = s.SENSOR_LOCATION_DESCRIPTION
                                       }).ToList();

                // 传感器Id
                var sensorIds = sensors.Select(s => s.SensorId).ToArray();

                // 最新数据Id
                var dataIds = from d in db.T_THEMES_ENVI_SATURATION_LINE
                              where sensorIds.Contains(d.SENSOR_ID)
                              group d by d.SENSOR_ID
                              into g
                              select g.Select(o => o.ID).Max();

                // 最新高程
                var data = (from d in db.T_THEMES_ENVI_SATURATION_LINE
                            where sensorIds.Contains(d.SENSOR_ID) && dataIds.Contains(d.ID)
                            select new { SensorId = d.SENSOR_ID, d.HEIGHT, d.ACQUISITION_DATETIME }).ToList();

                // 阈值
                var threshold = (from t in db.T_FACT_SENSOR_THRESHOLD
                                 where sensorIds.Contains(t.SensorId)
                                 select new { t.SensorId, ThresholdId = t.Id, ThresholdValue = t.ThresholdDownValue })
                    .ToList();


                var date = data.Max(l => l.ACQUISITION_DATETIME);
                var rslt = new JObject(
                    new JProperty("collectTime", date),
                    new JProperty(
                        "data",
                        sensors.GroupBy(s => s.GroupName)
                            .Select(
                                g =>
                                new JObject(
                                    new JProperty("groupName", g.Key),
                                    new JProperty(
                                    "values",
                                    g.GroupBy(v => new { v.SensorId, v.Location, v.Depth })
                                    .Select(
                                        v =>
                                        new JObject(
                                            new JProperty("location", v.Key.Location),
                                            new JProperty("depth", v.Key.Depth),
                                            new JProperty(
                                            "height",
                                            data.Where(d => d.SensorId == v.Key.SensorId && d.ACQUISITION_DATETIME >= date.AddDays(-1))
                                            .Select(d => d.HEIGHT)
                                            .FirstOrDefault()))))))),
                    new JProperty(
                        "threshold",
                        sensors.GroupBy(s => s.GroupName)
                            .Select(
                                g =>
                                new JObject(
                                    new JProperty("groupName", g.Key),
                                    new JProperty(
                                    "values",
                                    g.GroupBy(v => new { v.SensorId, v.Location, v.Depth })
                                    .Select(
                                        v =>
                                        new JObject(
                                            new JProperty("location", v.Key.Location),
                                            new JProperty("depth", v.Key.Depth),
                                            new JProperty(
                                            "height",
                                            threshold.Where(t => t.SensorId == v.Key.SensorId)
                                            .Select(d => d.ThresholdValue)
                                            .FirstOrDefault()))))))));

                return rslt;
            }
        }
    }
}
