namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The wind data controller.
    /// </summary>
    public class WindDataController : ApiController
    {
        private WindData windDal = new WindData();

        /// <summary>
        /// 风玫瑰图统计 GET wind/{sensorId}/stat-data/{startdate}/{enddate}
        /// </summary>
        /// <param name="sensorId"> The sensors. </param>
        /// <param name="startDate"> The startdate. </param>
        /// <param name="endDate"> The enddate. </param>
        /// <returns> The stat data. <see cref="object"/>.
        /// </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取风玫瑰图统计数据", false)]
        public object GetWindStatDataBySensorAndDate(int sensorId, DateTime startDate, DateTime endDate)
        {
            IList<WindStatData> data = this.windDal.GetWindStatData(sensorId, startDate, endDate);

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var sensor =
                    entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == sensorId)
                        .Select(s => new { SensorId = s.SENSOR_ID, Location = s.SENSOR_LOCATION_DESCRIPTION }).FirstOrDefault();

                return new JObject(
                    new JProperty("sensorId", sensorId),
                    new JProperty("location", sensor == null ? default(string) : sensor.Location),
                    new JProperty(
                        "value",
                        new JArray(
                            data.Select(
                                v =>
                                new JObject(
                                    new JProperty("direct", v.Direct),
                                    new JProperty("percent1", v.Percent1),
                                    new JProperty("percent2", v.Percent2),
                                    new JProperty("percent3", v.Percent3),
                                    new JProperty("percent4", v.Percent4),
                                    new JProperty("percent5", v.Percent5),
                                    new JProperty("percent6", v.Percent6),
                                    new JProperty("percent7", v.Percent7),
                                    new JProperty("totalPercent", v.TotalPercent))))));
            }
        }
    }
}
