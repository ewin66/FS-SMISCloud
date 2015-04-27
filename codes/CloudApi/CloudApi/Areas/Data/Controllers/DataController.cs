namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using FS.Service;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Service;
    using System.Collections.Generic;
    using log4net;
    using System.Web;

    /// <summary>
    /// 数据Controller
    /// </summary>
    public class DataController : ApiController
    {
        static ILog log = LogManager.GetLogger("SensorDataController");

        /// <summary>
        /// 获取传感器数据
        /// </summary>
        /// <param name="sensors"> 传感器数组 </param>
        /// <param name="startdate"> 开始时间 </param>
        /// <param name="enddate"> 结束时间 </param>
        /// <param name="interval">间隔数</param>
        /// <param name="datename">日期单位</param>
        /// <returns> The <see cref="object"/>. </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器数据", false)]
        public object GetBySensorAndDate(string sensors, DateTime startdate, DateTime enddate, int interval, string datename)
        {
            var list =
                new DAL.Data().GetMonitorData(sensors, startdate, enddate, interval, datename).OrderBy(d => d.SensorId);
            return
                new JArray(list.GroupBy(l => new { l.SensorId, l.Location, l.Columns, l.Unit }).Select(d =>
                    new JObject(
                        new JProperty("sensorid", d.Key.SensorId),
                        new JProperty("location", d.Key.Location),
                        new JProperty("columns", d.Key.Columns),
                        new JProperty("unit", d.Key.Unit),
                        new JProperty(
                            "data",
                            new JArray(d.Select(v =>
                                new JObject(
                                    new JProperty("value", v.Values),
                                    new JProperty("acquisitiontime", v.AcquisitionTime))))))));
        }

        /// <summary>
        /// 获取GPRS传感器数据
        /// </summary>
        /// <param name="sensors"> 传感器数组 </param>
        /// <param name="startdate"> 开始时间 </param>
        /// <param name="enddate"> 结束时间 </param>
        /// <param name="interval">间隔数</param>
        /// <param name="datename">日期单位</param>
        /// <returns> The <see cref="object"/>. </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取GPRS传感器数据", false)]
        public object GetGprsSensorData(string sensors, DateTime startdate, DateTime enddate, int interval, string datename)
        {
            var listMonitor =
                new DAL.Data().GetMonitorData(sensors, startdate, enddate, interval, datename).OrderBy(d => d.SensorId);
            // return listMonitor;

            var listOriginal =
                new DAL.Data().GetOriginalData(sensors, startdate, enddate, interval, datename).OrderBy(d => d.SensorId);

            var query = from m in listMonitor
                        from o in listOriginal
                        where m.SensorId == o.SensorId
                        && m.AcquisitionTime == o.AcquisitionTime
                        select new
                        {
                            sensorId = m.SensorId,
                            location = m.Location,
                            originalColumn = o.Columns,
                            originalUnit = o.Unit,
                            originalValue = o.Values,
                            calculatedColumn = m.Columns,
                            calculatedUnit = m.Unit,
                            calculatedValue = m.Values,
                            acquisitiontime = m.AcquisitionTime
                        };
            var list = query.ToList();
            // return list;

            var json = new JArray(list.GroupBy(g => new { g.sensorId, g.location, g.calculatedColumn, g.calculatedUnit, g.originalColumn, g.originalUnit })
                .Select(s => new JObject(
                        new JProperty("sensorid", s.Key.sensorId),
                        new JProperty("location", s.Key.location),
                        new JProperty("columns", new JObject(
                            new JProperty("originalColumn", s.Key.originalColumn),
                            new JProperty("calculatedColumn", s.Key.calculatedColumn)
                        )),
                        new JProperty("units", new JObject(
                            new JProperty("originalUnit", s.Key.originalUnit),
                            new JProperty("calculatedUnit", s.Key.calculatedUnit)
                        )),
                        new JProperty("data", new JArray(s.Select(v => new JObject(
                            new JProperty("originalValue", v.originalValue),
                            new JProperty("calculatedValue", v.calculatedValue),
                            new JProperty("acquisitiontime", v.acquisitiontime) )))))));
            return json;
        }

        /// <summary>
        /// 获取传感器原始数据
        /// </summary>
        /// <param name="sensors"> 传感器数组 </param>
        /// <param name="startdate"> 开始时间 </param>
        /// <param name="enddate"> 结束时间 </param>
        /// <param name="interval">间隔数</param>
        /// <param name="datename">日期单位</param>
        /// <returns> The <see cref="object"/>. </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器原始数据", false)]
        [Authorization(AuthorizationCode.S_DataOriginal)]
        public object GetSensorOriginalData(string sensors, DateTime startdate, DateTime enddate, int interval, string datename)
        {

            var list =
                new DAL.Data().GetOriginalData(sensors, startdate, enddate, interval, datename).OrderBy(d => d.SensorId);


            return new JArray(list.GroupBy(l => new { l.SensorId, l.Location, l.Columns, l.Unit }).Select(d =>
                   new JObject(
                       new JProperty("sensorid", d.Key.SensorId),
                       new JProperty("location", d.Key.Location),
                       new JProperty("columns", d.Key.Columns),
                       new JProperty("unit", d.Key.Unit),
                       new JProperty(
                           "data",
                           new JArray(d.Select(v =>
                               new JObject(
                                   new JProperty("value", v.Values),
                                   new JProperty("acquisitiontime", v.AcquisitionTime))))))));
        }
        

        /// <summary>
        /// 获取传感器即时采集请求任务结果
        /// </summary>
        /// <param name="dtu">DTU主键id</param>
        /// <param name="sensors">传感器数组</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器即时采集请求任务结果", false)]
        [Authorization(AuthorizationCode.S_InstantCollect_Issue)]
        public object GetSensorRealtimeRequest(int dtu, string sensors)
        {
            log.DebugFormat("Instant DAC: DTU={0}, sensors={1}", dtu, sensors);
            Guid guid = Guid.NewGuid();
            FsMessage msg = new FsMessage();
            msg.Header.U = guid;
            msg.Header.R = "/et/dtu/instant/dac"; //request url.
            msg.Header.S = "WebClient";
            msg.Header.M = "GET";
            msg.Header.T = Guid.NewGuid();
            msg.Body = new
            {
                dtu = dtu,
                sensors = ToIntArray(sensors),
            };
            WebClientService.SendToET(msg);

            using (var entity = new SecureCloud_Entities())
            {
                var strGuid = guid.ToString();
                var query = from ti in entity.T_TASK_INSTANT
                            where ti.MSG_ID == strGuid
                            select new
                            {
                                msgid = ti.MSG_ID
                            };
                var result = true;
                var list = query.ToList();
                if (list.Count == 0)
                {
                    result = false;
                }
                var json = new JObject(
                                new JProperty("msgid", guid),
                                new JProperty("result", result));
                return json;
            }
        }

        public static int[] ToIntArray(string p, char separator = ',')
        {
            if (p == null)
            {
                return null;
            }
            List<int> numbers = new List<int>();
            string[] ps = p.Split(separator);
            foreach (string pi in ps)
            {
                numbers.Add(Convert.ToInt32(pi));
            }
            return numbers.ToArray();
        }

        /// <summary>
        /// 获取传感器即时采集数据
        /// </summary>
        /// <param name="messageId">消息id</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器即时采集数据", false)]
        [Authorization(AuthorizationCode.S_InstantCollect_Issue)]
        public object GetSensorRealtimeData(string messageId)
        {
            // 查询 T_TASK_INSTANT 表获取数据:
            using (var entity = new SecureCloud_Entities())
            {
                var query = from ti in entity.T_TASK_INSTANT
                            where ti.MSG_ID == messageId
                            select new
                            {
                                sensors = ti.SENSORS,
                                data = ti.RESULT_JSON,
                                time = ti.FINISHED,
                                status = ti.RESULT_MSG
                            };
                var list = query.ToList();
                if (list.Count == 0)
                {
                    return null;
                }
                var strData = list.Select(s => s.data).FirstOrDefault();
                if (strData == null || strData.Trim() == "")
                {
                    return list.Select(s => new { result = JsonConvert.DeserializeObject(""), s.time, s.status }).FirstOrDefault();
                }
                return list.Select(s => new { result = JsonConvert.DeserializeObject(s.data), s.time, s.status }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取传感器最新数据
        /// </summary>
        /// <param name="sensors"> 传感器数组 </param>
        /// <returns> The <see cref="object"/>. </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器最新数据",false)]
        public object GetLastBySensor(string sensors)
        {
            string[] sens = sensors.Split(',');
            int[] sensorArray = sens.Select(s => Convert.ToInt32(s)).ToArray();
            var list = new DAL.Data().GetLastMonitorData(sensorArray).OrderBy(d => d.SensorId);
            return
                new JArray(list.GroupBy(l => new { l.SensorId, l.Location, l.Columns, l.Unit }).Select(d =>
                    new JObject(
                        new JProperty("sensorid", d.Key.SensorId),
                        new JProperty("location", d.Key.Location),
                        new JProperty("columns", d.Key.Columns),
                        new JProperty("unit", d.Key.Unit),
                        new JProperty(
                            "data",
                            new JArray(d.Select(v =>
                                new JObject(
                                    new JProperty("value", v.Values),
                                    new JProperty("acquisitiontime", v.AcquisitionTime))))))));
        }
    }
}
