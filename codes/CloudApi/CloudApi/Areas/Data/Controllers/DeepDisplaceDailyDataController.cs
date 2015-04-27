namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 深部位移日数据Controller
    /// </summary>
    public class DeepDisplaceDailyDataController : ApiController
    {
        /// <summary>
        /// Get deep-displace/{groupid}/daily-data-by-depth/{direct}/{startdate}/{enddate}
        /// </summary>
        /// <param name="groupId">传感器组编号（只能是数字）</param>
        /// <param name="direct">数据方向（只能是x, y, xy）</param>
        /// <param name="startdate">开始时间（ISO时间）</param>
        /// <param name="enddate">结束时间（ISO时间）</param>
        /// <returns>深部位移数据</returns>
        [LogInfo("获取内部位移日数据-按深度分组", false)]
        public object GetByGroupDirectAndDateGroupByDepth(int groupId, string direct, DateTime startdate, DateTime enddate)
        {
            using (SecureCloud_Entities entities = new SecureCloud_Entities())
            {
                var query = from d in entities.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            where d.SENSOR_ID == sg.SENSOR_ID
                                  && sg.GROUP_ID == groupId
                                  && d.ACQUISITION_DATETIME >= startdate && d.ACQUISITION_DATETIME <= enddate
                            select new
                            {
                                depth = sg.DEPTH,
                                xvalue = d.DEEP_DISPLACEMENT_X_VALUE,
                                yvalue = d.DEEP_DISPLACEMENT_Y_VALUE,
                                acquistiontime = d.ACQUISITION_DATETIME
                            };
                var list = query.ToList();
                if (direct == "x")
                {
                    return new JArray(list.GroupBy(d => d.depth).OrderBy(d => d.Key).Select(d =>
                        new JObject(
                            new JProperty("depth", d.Key),
                            new JProperty(
                                "values",
                                new JArray(d.OrderBy(v => v.acquistiontime).Select(v =>
                                    new JObject(
                                        new JProperty("xvalue", v.xvalue),
                                        new JProperty("acquistiontime", v.acquistiontime))))))));
                }

                if (direct == "y")
                {
                    return new JArray(list.GroupBy(d => d.depth).OrderBy(d => d.Key).Select(d =>
                        new JObject(
                            new JProperty("depth", d.Key),
                            new JProperty(
                                "values",
                                new JArray(d.OrderBy(v => v.acquistiontime).Select(v =>
                                    new JObject(
                                        new JProperty("yvalue", v.yvalue),
                                        new JProperty("acquistiontime", v.acquistiontime))))))));
                }

                return new JArray(list.GroupBy(d => d.depth).OrderBy(d => d.Key).Select(d =>
                    new JObject(
                        new JProperty("depth", d.Key),
                        new JProperty(
                            "values",
                            new JArray(d.OrderBy(v => v.acquistiontime).Select(v =>
                                new JObject(
                                    new JProperty("xvalue", v.xvalue),
                                    new JProperty("yvalue", v.yvalue),
                                    new JProperty("acquistiontime", v.acquistiontime))))))));
            }
        }

        /// <summary>
        /// Get deep-displace/{groupid}/daily-data-by-time/{direct}/{startdate}/{enddate}
        /// </summary>
        /// <param name="groupId">传感器组编号（只能是数字）</param>
        /// <param name="direct">数据方向（只能是x, y, xy）</param>
        /// <param name="startdate">开始时间（ISO时间）</param>
        /// <param name="enddate">结束时间（ISO时间）</param>
        /// <returns>深部位移数据</returns>
        [LogInfo("获取内部位移日数据-按时间分组", false)]
        public object GetByGroupDirectAndDateGroupByTime(int groupId, string direct, DateTime startdate, DateTime enddate)
        {
            using (SecureCloud_Entities entities = new SecureCloud_Entities())
            {
                var query = from d in entities.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            where d.SENSOR_ID == sg.SENSOR_ID
                                  && sg.GROUP_ID == groupId
                                  && d.ACQUISITION_DATETIME >= startdate && d.ACQUISITION_DATETIME <= enddate
                            select new
                            {
                                depth = sg.DEPTH,
                                xvalue = d.DEEP_CUMULATIVEDISPLACEMENT_X_VALUE,
                                yvalue = d.DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE,
                                acquistiontime = d.ACQUISITION_DATETIME
                            };
                var list = query.ToList();
                if (direct == "x")
                {
                    return new JArray(list.GroupBy(d => d.acquistiontime).OrderBy(d => d.Key).Select(d =>
                        new JObject(
                            new JProperty("acquistiontime", d.Key),
                            new JProperty(
                                "values",
                                new JArray(d.OrderBy(v => v.depth).Select(v =>
                                    new JObject(
                                        new JProperty("depth", v.depth),
                                        new JProperty("xvalue", v.xvalue))))))));
                }

                if (direct == "y")
                {
                    return new JArray(list.GroupBy(d => d.acquistiontime).OrderBy(d => d.Key).Select(d =>
                        new JObject(
                            new JProperty("acquistiontime", d.Key),
                            new JProperty(
                                "values",
                                new JArray(d.OrderBy(v => v.depth).Select(v =>
                                    new JObject(
                                        new JProperty("depth", v.depth),
                                        new JProperty("yvalue", v.yvalue))))))));
                }

                return new JArray(list.GroupBy(d => d.acquistiontime).OrderBy(d => d.Key).Select(d =>
                    new JObject(
                        new JProperty("acquistiontime", d.Key),
                        new JProperty(
                            "values",
                            new JArray(d.OrderBy(v => v.depth).Select(v =>
                                new JObject(
                                    new JProperty("depth", v.depth),
                                    new JProperty("xvalue", v.xvalue),
                                    new JProperty("yvalue", v.yvalue))))))));
            }
        }
    }
}
