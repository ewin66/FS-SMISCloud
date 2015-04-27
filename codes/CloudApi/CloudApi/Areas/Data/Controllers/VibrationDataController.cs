using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class VibrationDataController : ApiController
    {
         //<summary>
         //获取计算后的震动数据
         //</summary>
         //<param name="batchId">传感器编号</param>
         //<returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("所有传感器震动数据", false)]
        public object GetVibrationData(int structid,DateTime collectTime)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from b1 in db.T_THEMES_VIBRATION_MICROSEISMIC
                            where b1.CollectTime == collectTime && b1.struct_Id == structid 
                            select
                                new MicroseismicData
                                {
                                    Id = b1.Id,
                                    StructId = b1.struct_Id,
                                    CollectTime = b1.CollectTime,
                                    X = b1.Coordinate_X,
                                    Y=b1.Coordinate_Y,
                                    Z=b1.Coordinate_Z,
                                    OccurrenceTime=b1.OccurrenceTime
                                };
                return query.ToList();
            }
        }
        public class MicroseismicData
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("structId")]
            public int StructId { set; get; }

            [JsonProperty("collectTime")]
            public DateTime CollectTime { set; get; }

            [JsonProperty("x")]
            public decimal X { set; get; }

            [JsonProperty("y")]
            public decimal Y { set; get; }

            [JsonProperty("z")]
            public decimal Z { set; get; }

            [JsonProperty("occurrenceTime")]
            public DateTime OccurrenceTime { set; get; }

          
        }

        [AcceptVerbs("Post")]
        [LogInfo("微震震源数据保存", false)]
        public void SaveVibrationData([FromBody]SensorCond form)
        {
            using (var db = new SecureCloud_Entities())
            {
                var stringId = int.Parse(form.structId);
                var vibration = (from m1 in db.T_THEMES_VIBRATION_MICROSEISMIC
                                 where m1.struct_Id == stringId
                           && m1.CollectTime == form.collectTime
                                 select m1).FirstOrDefault();
                if (vibration != null)
                {
                    var selectPt = from m2 in db.T_THEMES_VIBRATION_MICROSEISMIC_PTSELECT
                                   where m2.msId == vibration.Id
                                   select m2;
                    foreach (var item in selectPt)
                    {
                        db.T_THEMES_VIBRATION_MICROSEISMIC_PTSELECT.Remove(item);
                    }
                    db.T_THEMES_VIBRATION_MICROSEISMIC.Remove(vibration);
                }

                T_THEMES_VIBRATION_MICROSEISMIC MS = new T_THEMES_VIBRATION_MICROSEISMIC();
                MS.struct_Id = int.Parse(form.structId);
                MS.CollectTime = form.collectTime;
                MS.Coordinate_X = decimal.Parse(form.xyzt[0].ToString());
                MS.Coordinate_Y = decimal.Parse(form.xyzt[1].ToString());
                MS.Coordinate_Z = decimal.Parse(form.xyzt[2].ToString());
                MS.Intensity = 0;
                long a = (long)form.xyzt[3] * 1000 * 10000;
                long ticks = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).Ticks + a;
                DateTime dt = new DateTime(ticks);
                MS.OccurrenceTime = dt;
                db.T_THEMES_VIBRATION_MICROSEISMIC.Add(MS);


                foreach (var item in form.items)
                {
                    T_THEMES_VIBRATION_MICROSEISMIC_PTSELECT tp = new T_THEMES_VIBRATION_MICROSEISMIC_PTSELECT();
                    tp.msId = MS.Id;
                    tp.SelectPt = int.Parse(item.sensorid);
                    tp.SelectTime = decimal.Parse(item.t.ToString().Substring(0, 13)) / 1000;
                    tp.WaveSpeed = decimal.Parse(item.speed.ToString());
                    db.T_THEMES_VIBRATION_MICROSEISMIC_PTSELECT.Add(tp);
                }
                db.SaveChanges();
            }
        }


        /// <summary>
        /// 获取振动批次下所有传感器震动数据
        /// </summary>
        /// <param name="batchId">传感器编号</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("所有传感器震动数据", false)]
        public IEnumerable<DataOriginal> GetOriginalByCollectTime(DateTime collectTime, int structId, int factorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from b1 in db.T_THEMES_VIBRATION_BATCH
                            join b2 in db.T_THEMES_VIBRATION_ORIGINAL on b1.BatchId equals b2.BatchId
                            join s1 in db.T_DIM_SENSOR on b1.SensorId equals s1.SENSOR_ID
                            join u1 in db.T_DIM_USER_STRUCTURE on s1.STRUCT_ID equals u1.STRUCTURE_ID
                            where b1.CollectTime == collectTime
                                  && s1.STRUCT_ID == structId && s1.SAFETY_FACTOR_TYPE_ID == factorId
                            orderby b1.SensorId,b2.CollectTime
                            select
                                new DataOriginal
                                {
                                    Id=b2.Id,
                                    SensorId=b1.SensorId,
                                    BatchId = b1.BatchId,
                                    Speed=b2.Speed,
                                    CollectTime = b2.CollectTime
                                };

                return query.ToList();
            }
        }
        /// <summary>
        /// 获取振动批次
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取振动批次", false)]
        public IEnumerable<DataBatch> GetCollectTime(int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from b in db.T_THEMES_VIBRATION_BATCH
                            where b.SensorId == sensorId 
                            orderby b.CollectTime
                            select
                                new DataBatch
                                {
                                    BatchId = b.BatchId,
                                    CollectTime = b.CollectTime,
                                    MaxFrequency = b.MaxFrequency
                                };

                return query.ToList();
            }
        }

        /// <summary>
        /// 获取振动批次
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取振动批次", false)]
        public IEnumerable<DataBatch> GetDataBatch(int sensorId, DateTime startDate, DateTime endDate)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from b in db.T_THEMES_VIBRATION_BATCH
                            where b.SensorId == sensorId && b.CollectTime >= startDate && b.CollectTime <= endDate
                            orderby b.CollectTime
                            select
                                new DataBatch
                                {
                                    BatchId = b.BatchId,
                                    CollectTime = b.CollectTime,
                                    MaxFrequency = b.MaxFrequency
                                };

                return query.ToList();
            }
        }



        /// <summary>
        /// 获取振动频谱数据
        /// </summary>
        /// <param name="batchId">批次编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取振动频谱数据", false)]
        public object GetSpectrumData(Guid batchId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from d in db.T_THEMES_VIBRATION
                            where d.BatchId == batchId
                            orderby d.Frequency
                            select new VibrationData { Value = d.Value, Frequency = d.Frequency };

                var data = query.ToList();

                var query2 = from b in db.T_THEMES_VIBRATION_BATCH
                             from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             where b.SensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                             && b.BatchId == batchId && s.Identification != 1 && !s.IsDeleted
                             select
                                 new
                                 {
                                     SensorId = s.SENSOR_ID,
                                     Location = s.SENSOR_LOCATION_DESCRIPTION,
                                     Columns = f.FACTOR_VALUE_COLUMNS,
                                     //Unit = f.FACTOR_VALUE_UNIT
                                    // Unit = Config.GetUnitBySensorID(s.SENSOR_ID)
                                 };
                var unit = "";
                foreach (var item in query2)
                {
                     unit = Config.GetUnitBySensorID(item.SensorId)[0];
                }

                var info = query2.FirstOrDefault();

                if (info != null)
                {
                    return
                        new JArray(
                            new JObject(
                                new JProperty("sensorId", info.SensorId),
                                new JProperty("location", info.Location),
                                new JProperty("columns", info.Columns.Split(',')),
                                //new JProperty("unit", info.Unit.Split(',')),
                                new JProperty("unit", unit),
                                new JProperty(
                                    "data",
                                    data.Count > 0
                                        ? data.Select(
                                            d =>
                                            new JObject(
                                                new JProperty("value", new JArray(d.Value)),
                                                new JProperty("frequency", d.Frequency)))
                                        : new List<JObject>())));
                }

                return null;
            }
        }

        /// <summary>
        /// 获取振动实时频谱数据
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取振动实时频谱数据", false)]
        public object GetRtSpectrumData(int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                if (!(from b in db.T_THEMES_VIBRATION_BATCH where b.SensorId == sensorId select b.CollectTime).Any())
                {
                    return null;
                }

                var lastDate =
                    (from b in db.T_THEMES_VIBRATION_BATCH where b.SensorId == sensorId select b.CollectTime).Max();

                var lastBatch =
                    (from i in db.T_THEMES_VIBRATION_BATCH
                     where i.SensorId == sensorId && i.CollectTime == lastDate
                     select i.BatchId).ToList().Max();

                var maxFrequency =
                    (from b in db.T_THEMES_VIBRATION_BATCH where b.BatchId == lastBatch select b.MaxFrequency)
                        .FirstOrDefault();

                var query = from d in db.T_THEMES_VIBRATION
                            where d.BatchId == lastBatch
                            orderby d.Frequency
                            select new VibrationData { Value = d.Value, Frequency = d.Frequency };

                var data = query.ToList();

                var query2 = from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             where sensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                             && !s.IsDeleted && s.Identification != 1
                             select
                                 new
                                 {
                                     SensorId = s.SENSOR_ID,
                                     Location = s.SENSOR_LOCATION_DESCRIPTION,
                                     Columns = f.FACTOR_VALUE_COLUMNS,
                                     //Unit = f.FACTOR_VALUE_UNIT
                                    // Unit = Config.GetUnitBySensorID(s.SENSOR_ID)[0]
                                 };

                var unit = "";
                foreach (var item in query2)
                {
                    unit = Config.GetUnitBySensorID(item.SensorId)[0];
                }

                var info = query2.FirstOrDefault();

                if (info != null)
                {
                    return
                        new JArray(
                            new JObject(
                                new JProperty("collectTime", lastDate),
                                new JProperty("maxFrequency", maxFrequency),
                                new JProperty("sensorId", info.SensorId),
                                new JProperty("location", info.Location),
                                new JProperty("columns", info.Columns.Split(',')),
                                new JProperty("unit", unit),
                                new JProperty(
                                    "data",
                                    data.Count > 0
                                        ? data.Select(
                                            d =>
                                            new JObject(
                                                new JProperty("value", new JArray(d.Value)),
                                                new JProperty("frequency", d.Frequency)))
                                        : new List<JObject>())));
                }

                return null;
            }
        }

        /// <summary>
        /// 获取振动时域数据
        /// </summary>
        /// <param name="batchId">批次编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取振动时域数据", false)]
        public object GetOriginalData(Guid batchId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from d in db.T_THEMES_VIBRATION_ORIGINAL
                            where d.BatchId == batchId
                            orderby d.CollectTime
                            select new { d.CollectTime, d.Speed };

                var data = query.ToList();

                var query2 = from b in db.T_THEMES_VIBRATION_BATCH
                             from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             where b.SensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                             && b.BatchId == batchId && s.Identification != 1 && !s.IsDeleted
                             select
                                 new
                                 {
                                     SensorId = s.SENSOR_ID,
                                     Location = s.SENSOR_LOCATION_DESCRIPTION,
                                     Columns = f.FACTOR_VALUE_COLUMNS,
                                    // Unit = f.FACTOR_VALUE_UNIT
                                    // Unit = Config.GetUnitBySensorID(s.SENSOR_ID)[0]
                                 };

                var info = query2.FirstOrDefault();
                var unit =new string[ ]{};
                foreach (var item in query2)
                {
                    unit = Config.GetUnitBySensorID(item.SensorId);
                }

                if (info != null)
                {
                    return
                        new JArray(
                            new JObject(
                                new JProperty("sensorId", info.SensorId),
                                new JProperty("location", info.Location),
                                new JProperty("columns", info.Columns.Split(',')),
                               // new JProperty("unit", new[] { "cm/s" }),
                               new JProperty("unit", unit),
                                new JProperty(
                                    "data",
                                    data.Count > 0
                                        ? data.Select(
                                            d =>
                                            new JObject(
                                                new JProperty("value", new JArray(d.Speed)),
                                                new JProperty("collectTime", d.CollectTime)))
                                        : new List<JObject>())));
                }

                return null;
            }
        }

        /// <summary>
        /// 获取振动实时时域数据
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取振动实时时域数据", false)]
        public object GetRtOriginalData(int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                if (!(from b in db.T_THEMES_VIBRATION_BATCH where b.SensorId == sensorId select b.CollectTime).Any())
                {
                    return null;
                }

                var lastDate =
                    (from b in db.T_THEMES_VIBRATION_BATCH where b.SensorId == sensorId select b.CollectTime).Max();

                var lastBatch =
                    (from i in db.T_THEMES_VIBRATION_BATCH
                     where i.SensorId == sensorId && i.CollectTime == lastDate
                     select i.BatchId).ToList().Max();

                var maxFrequency =
                    (from b in db.T_THEMES_VIBRATION_BATCH where b.BatchId == lastBatch select b.MaxFrequency)
                        .FirstOrDefault();

                var query = from d in db.T_THEMES_VIBRATION_ORIGINAL
                            where d.BatchId == lastBatch
                            orderby d.CollectTime
                            select new { d.Speed, d.CollectTime };

                var data = query.ToList();

                var query2 = from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             where sensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                             && s.Identification != 1 && !s.IsDeleted
                             select
                                 new
                                 {
                                     SensorId = s.SENSOR_ID,
                                     Location = s.SENSOR_LOCATION_DESCRIPTION,
                                     Columns = f.FACTOR_VALUE_COLUMNS,
                                    // Unit = f.FACTOR_VALUE_UNIT
                                   //  Unit = Config.GetUnitBySensorID(s.SENSOR_ID)[0]
                                 };

                var info = query2.FirstOrDefault();
                var unit = new string[] { };
                foreach (var item in query2)
                {
                    unit = Config.GetUnitBySensorID(item.SensorId);
                }
                if (info != null)
                {
                    return
                        new JArray(
                            new JObject(
                                new JProperty("collectTime", lastDate),
                                new JProperty("maxFrequency", maxFrequency),
                                new JProperty("sensorId", info.SensorId),
                                new JProperty("location", info.Location),
                                new JProperty("columns", info.Columns.Split(',')),
                               // new JProperty("unit", new[] { "cm/s" }),
                               new JProperty("unit", unit),

                                new JProperty(
                                    "data",
                                    data.Count > 0
                                        ? data.Select(
                                            d =>
                                            new JObject(
                                                new JProperty("value", new JArray(d.Speed)),
                                                new JProperty("collectTime", d.CollectTime)))
                                        : new List<JObject>())));
                }

                return null;
            }
        }
       
    }

    // model
    public class DataBatch
    {
        [JsonProperty("batchId")]
        public Guid BatchId { get; set; }

        [JsonProperty("collectTime")]
        public DateTime CollectTime { get; set; }

        [JsonProperty("maxFrequency")]
        public int MaxFrequency { get; set; }
    }

    // model
    public class DataOriginal
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        [JsonProperty("batchId")]
        public Guid BatchId { get; set; }

        [JsonProperty("speed")]
        public decimal Speed { get; set; }

        [JsonProperty("collectTime")]
        public double CollectTime { get; set; }
    }

    // model
    public class DataBatchRShell
    {
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        [JsonProperty("batchId")]
        public Guid BatchId { get; set; }

        [JsonProperty("collectTime")]
        public DateTime CollectTime { get; set; }

        [JsonProperty("maxFrequency")]
        public int MaxFrequency { get; set; }
    }

    public class VibrationData
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("frequency")]
        public decimal Frequency { get; set; }
    }
}