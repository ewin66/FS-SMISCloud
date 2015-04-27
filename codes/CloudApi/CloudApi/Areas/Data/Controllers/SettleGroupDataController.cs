namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json.Linq;

    public class SettleGroupDataController : ApiController
    {
        /// <summary>
        /// 沉降组对比数据 settle/{groupId}/daily-data/{startDate}/{endDate}
        /// </summary>
        /// <param name="groupId">组编号</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>数据</returns>
        [LogInfo("获取沉降日对比数据", false)]
        public object GetDailyDataByGroupAndDate(int groupId, DateTime startDate, DateTime endDate)
        {
            string unit = "mm";
            int decimalPlace = 2;
            FactorConfig config =
                Config.GetConfigByFactors(
                    new[] { int.Parse(ConfigurationManager.AppSettings["SettleFactorId"]) },0)///修改
                        .FirstOrDefault();
            if (config != default(FactorConfig) && config.Unit.Length > 0)
            {
                unit = config.Unit[0];
            }

            if (config != default(FactorConfig) && config.DecimalPlaces.Length > 0)
            {
                decimalPlace = config.DecimalPlaces[0];
            }

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from data in entity.T_THEMES_DEFORMATION_SETTLEMENT
                            from sensor in entity.T_DIM_SENSOR
                            from sensorGrp in entity.T_DIM_SENSOR_GROUP_CHENJIANG
                            where data.SENSOR_ID == sensor.SENSOR_ID
                                  && sensor.SENSOR_ID == sensorGrp.SENSOR_ID
                                  && sensorGrp.GROUP_ID == groupId
                                  && !sensor.IsDeleted
                                  && sensor.Identification != 1
                                  && data.ACQUISITION_DATETIME >= startDate
                                  && data.ACQUISITION_DATETIME <= endDate
                            select new
                            {
                                Acquistiontime = data.ACQUISITION_DATETIME,
                                SensorId = data.SENSOR_ID,
                                Location = sensor.SENSOR_LOCATION_DESCRIPTION,
                                Len = sensorGrp.LENGTH,
                                Value = data.SETTLEMENT_VALUE
                            };
                var unitList = Config.GetUnitBySensorID(Convert.ToInt32(query.Select(m => m.SensorId).FirstOrDefault()));
                unit = unitList[0] ?? "mm";

                return new JObject(
                    new JProperty("unit", unit),
                    new JProperty("data",
                        new JArray(query.ToList().GroupBy(g => new
                        {
                            Year = g.Acquistiontime.GetValueOrDefault().Year,
                            Month = g.Acquistiontime.GetValueOrDefault().Month,
                            Day = g.Acquistiontime.GetValueOrDefault().Day
                        }).Select(d =>
                            new JObject(
                                new JProperty("acquistiontime", new DateTime(d.Key.Year, d.Key.Month, d.Key.Day)),
                                new JProperty("values",
                                    new JArray(
                                        d.GroupBy(g => new { g.SensorId, g.Len, g.Location })
                                            .OrderBy(g => g.Key.Len)
                                            .Select(v =>
                                                new JObject(
                                                    new JProperty("sensorId", v.Key.SensorId),
                                                    new JProperty("location", v.Key.Location),
                                                    new JProperty("len", v.Key.Len),
                                                    new JProperty("value", Math.Round(v.Select(value =>
                                                        value.Value.GetValueOrDefault())
                                                        .Average(), decimalPlace)))))))))));
            }
        }

        /// <summary>
        /// 获取日报表信息settlement/{groupId}/daily-report/{startDate}/{endDate}/{algorithm}/info
        /// </summary>
        /// <param name="groupId">结构物编号</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="algorithm">算法</param>
        /// <returns>数据</returns>
        public object GetSettlementDailyReportByTime(int groupId, DateTime startDate, DateTime endDate, int algorithm)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {

                var query = from data in entity.T_THEMES_DEFORMATION_SETTLEMENT
                    from sensor in entity.T_DIM_SENSOR
                    from sensorGrp in entity.T_DIM_SENSOR_GROUP_CHENJIANG
                    where data.SENSOR_ID == sensor.SENSOR_ID
                          && sensorGrp.GROUP_ID == groupId
                          && sensor.SENSOR_ID == sensorGrp.SENSOR_ID //根据分组进行传感器筛选
                          && !sensor.IsDeleted && sensor.Identification != 1
                          && data.ACQUISITION_DATETIME >= startDate
                          && data.ACQUISITION_DATETIME <= endDate && data.SETTLEMENT_VALUE != null
                    select new
                    {
                        SensorId = data.SENSOR_ID,
                        Location = sensor.SENSOR_LOCATION_DESCRIPTION,
                        Value = (decimal) data.SETTLEMENT_VALUE,
                        Acquistiontime = (DateTime)data.ACQUISITION_DATETIME
                    };

                if (algorithm == 1)
                {
                    //对query进行分组取第一条不为空的数据 　
                    var groupBySensorId = query.GroupBy(q => q.SensorId).Select(g => new
                    {
                        sensorId = g.Key,
                        minData = g.Min(q => q.Acquistiontime)
                    });

                    var queryData = from q in query
                        from g in groupBySensorId
                        where q.Acquistiontime == g.minData && q.SensorId == g.sensorId
                        select new
                        {
                            SensorId = q.SensorId,
                            Location = q.Location,
                            Value = q.Value,
                        };
                    var list = queryData.ToList();
                    return list;
                }
                else if (algorithm==2)
                {
                     //求平均
                    var groupBySensorId = query.GroupBy(q => q.SensorId).Select(g => new
                    {
                        sensorId = g.Key,
                        value = Math.Round(g.Average(p => p.Value),2)
                    });
                    var queryData = (from q in query
                        from g in groupBySensorId
                        where q.SensorId == g.sensorId
                        select new
                        {
                            SensorId = g.sensorId,
                            Location = q.Location,
                            Value = g.value,
                        }).Distinct();
                    var avergList = queryData.ToList();
                    return avergList;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
