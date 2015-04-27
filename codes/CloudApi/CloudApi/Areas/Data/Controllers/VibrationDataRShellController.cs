using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
using Newtonsoft.Json.Linq;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    public class VibrationDataRShellController : ApiController
    {
        /// <summary>
        /// 获取网壳振动触发时段
        /// </summary>
        /// <param name="sensorId">传感器组编号</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取网壳振动触发时段", false)]
        public object GetDataBatchRShell(int sensorId, DateTime startDate, DateTime endDate)
        {
            using (var db = new SecureCloud_Entities())
            {
                var querySensors = from scc in db.T_DIM_SENSOR_CORRENT
                                   where scc.SensorId == sensorId
                                   select new
                                   {
                                       SensorId = (int)scc.CorrentSensorId,
                                   };

                var query = from b in db.T_THEMES_VIBRATION_BATCH
                            from q in querySensors
                            where b.SensorId == q.SensorId && b.CollectTime >= startDate && b.CollectTime <= endDate
                            orderby b.CollectTime
                            select
                                new
                                {
                                    CollectTime = b.CollectTime
                                };

                return query.ToList().Distinct();
            }
        }

        /// <summary>
        /// 获取网壳振动时域数据
        /// </summary>
        /// <param name="batchId">批次编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取网壳振动时域数据", false)]
        public object GetOriginalDataRShell(DateTime collectTime,int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var querySensors = from scc in db.T_DIM_SENSOR_CORRENT
                                   from s in db.T_DIM_SENSOR
                                   where scc.SensorId == sensorId && s.SENSOR_ID == scc.CorrentSensorId
                                   select new
                                   {
                                       SensorId = (int)scc.CorrentSensorId,
                                       Name = s.SENSOR_LOCATION_DESCRIPTION
                                   };

                var lastBatch = from q in querySensors
                                select new
                                {
                                    BatchId = (from i in db.T_THEMES_VIBRATION_BATCH
                                               where i.SensorId == q.SensorId && i.CollectTime == collectTime
                                               select i.BatchId == null ? (Guid?)null : i.BatchId),
                                    SensorId = q.SensorId
                                };


               
                var query2 = from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             from q in querySensors

                             from l in lastBatch
                             where q.SensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && l.SensorId == s.SENSOR_ID
                                 //&& s.Identification != 1 && !s.IsDeleted
                              && !s.IsDeleted
                             select new
                                 {
                                     SensorId = s.SENSOR_ID,
                                     Location = s.SENSOR_LOCATION_DESCRIPTION,
                                     Columns = f.FACTOR_VALUE_COLUMNS,
                                    // Unit = f.FACTOR_VALUE_UNIT,
                                    // Unit = unit,
                                     data = (from o in db.T_THEMES_VIBRATION_ORIGINAL
                                             where o.BatchId == l.BatchId.FirstOrDefault()
                                             select o).Select(o => new
                                             {
                                                 Speed = o.Speed,
                                                 CollectTime = o.CollectTime//数据时间
                                             })
                                 };
                
               

                var info = query2.ToList();
                var oneUnit = new List<OneUnit>();
                foreach (var item in info)
                {
                    var one = new OneUnit();
                    one.Unit = Config.GetUnitBySensorID(item.SensorId)[0];
                    one.SensorId = item.SensorId;
                    oneUnit.Add(one);
                }
                var infoNew = from unit in oneUnit
                              from a in info
                              where a.SensorId == unit.SensorId
                              select new
                              {
                                  SensorId = a.SensorId,
                                  Location = a.Location,
                                  Columns = a.Columns,
                                  // Unit = f.FACTOR_VALUE_UNIT,
                                  Unit = unit.Unit,
                                  data = a.data
                              };

                return infoNew.ToList();
            }
        }

        /// <summary>
        /// 获取网壳振动实时时域数据
        /// </summary>
        /// <param name="sensorId">传感器组编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取网壳振动实时时域数据", false)]
        public object GetRtOriginalDataRShell(int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var querySensors = from scc in db.T_DIM_SENSOR_CORRENT
                                   from s in db.T_DIM_SENSOR
                                   where scc.SensorId == sensorId && s.SENSOR_ID == scc.CorrentSensorId
                                   select new
                                   {
                                       SensorId = (int)scc.CorrentSensorId,
                                       Name = s.SENSOR_LOCATION_DESCRIPTION
                                   };

                if (!(from b in db.T_THEMES_VIBRATION_BATCH from q in querySensors where b.SensorId == q.SensorId select b.CollectTime).Any())
                {
                    return null;
                }

                var lastDate = (from b in db.T_THEMES_VIBRATION_BATCH
                                from q in querySensors
                                where b.SensorId == q.SensorId
                                select b.CollectTime ).DefaultIfEmpty().Max();

                var collectTime = lastDate;

               

                var lastBatch = from q in querySensors
                                join i in (db.T_THEMES_VIBRATION_BATCH ).Where(k=>k.CollectTime==collectTime)
                                on q.SensorId equals i.SensorId
                                into j
                                from o in j.DefaultIfEmpty()
                                select new LastBatch
                                {
                                    BatchId = o.BatchId,
                                    SensorId = q.SensorId
                                };

                var query2 = from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             from q in querySensors

                             from l in lastBatch
                             where q.SensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && l.SensorId == s.SENSOR_ID
                                 //&& s.Identification != 1 && !s.IsDeleted
                              && !s.IsDeleted

                             select new
                             {
                                 SensorId = s.SENSOR_ID,
                                 Location = s.SENSOR_LOCATION_DESCRIPTION,
                                 Columns = f.FACTOR_VALUE_COLUMNS,
                                // Unit = f.FACTOR_VALUE_UNIT,
                                // Unit = unit,
                                 CollectTime = collectTime,//最新时段
                                 data = (from o in db.T_THEMES_VIBRATION_ORIGINAL
                                         where o.BatchId == l.BatchId
                                         select o).Select(o => new
                                         {
                                             Speed = o.Speed,
                                             CollectTime = o.CollectTime//数据时间
                                         })
                             };

                var info = query2.ToList();
                var oneUnit = new List<OneUnit>();
                foreach (var item in info)
                {
                    var one = new OneUnit();
                    one.Unit = Config.GetUnitBySensorID(item.SensorId)[0];
                    one.SensorId = item.SensorId;
                    oneUnit.Add(one);
                }
                var infoNew = from unit in oneUnit
                              from a in info
                              where a.SensorId == unit.SensorId
                              select new
                                  {
                                      SensorId = a.SensorId,
                                      Location = a.Location,
                                      Columns = a.Columns,
                                      // Unit = f.FACTOR_VALUE_UNIT,
                                      Unit = unit.Unit,
                                      CollectTime = a.CollectTime, //最新时段
                                      data = a.data
                                  };

                return infoNew.ToList();
            }
        }

        /// <summary>
        /// 获取网壳振动频谱数据
        /// </summary>
        /// <param name="batchId">批次编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取网壳振动频谱数据", false)]
        public object GetSpectrumDataRShell(DateTime collectTime, int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var querySensors = from scc in db.T_DIM_SENSOR_CORRENT
                                   from s in db.T_DIM_SENSOR
                                   where scc.SensorId == sensorId && s.SENSOR_ID == scc.CorrentSensorId
                                   select new
                                   {
                                       SensorId = (int)scc.CorrentSensorId,
                                       Name = s.SENSOR_LOCATION_DESCRIPTION
                                   };
                var lastBatch =
                   (from i in db.T_THEMES_VIBRATION_BATCH
                    from q in querySensors
                    where i.SensorId == q.SensorId && i.CollectTime == collectTime
                    select new
                    {
                        CollectTime = collectTime,//最新时段
                        BatchId = i.BatchId,
                        SensorId = q.SensorId
                    });
               
                var query2 = from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             from q in querySensors
                             from l in lastBatch
                             where q.SensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && l.SensorId == s.SENSOR_ID
                             && !s.IsDeleted
                             select
                                 new
                                 {
                                     SensorId = s.SENSOR_ID,
                                     Location = s.SENSOR_LOCATION_DESCRIPTION,
                                     Columns = f.FACTOR_VALUE_COLUMNS,
                                    // Unit = f.FACTOR_VALUE_UNIT,
                                    // Unit = unit,
                                     data = (from o in db.T_THEMES_VIBRATION
                                             where o.BatchId == l.BatchId
                                             select o).Select(o => new
                                             {
                                                 Frequency = o.Frequency,
                                                 Value = o.Value//数据时间
                                             })
                                 };

                var info = query2.ToList();

                var oneUnit = new List<OneUnit>();

                foreach (var item in info)
                {
                    var one = new OneUnit();
                    one.Unit = Config.GetUnitBySensorID(item.SensorId)[0];
                    one.SensorId = item.SensorId;
                    oneUnit.Add(one);
                }
                var infoNew = from unit in oneUnit
                              from a in info
                              where a.SensorId == unit.SensorId
                              select new
                              {
                                  SensorId = a.SensorId,
                                  Location = a.Location,
                                  Columns = a.Columns,
                                  Unit = unit.Unit,
                                  data = a.data
                              };

                return infoNew.ToList();
            }
        }

        /// <summary>
        /// 获取网壳振动实时频谱数据
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <returns>数据</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取网壳振动实时频谱数据", false)]
        public object GetRtSpectrumDataRShell(int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var querySensors = from scc in db.T_DIM_SENSOR_CORRENT
                                   from s in db.T_DIM_SENSOR
                                   where scc.SensorId == sensorId && s.SENSOR_ID == scc.CorrentSensorId
                                   select new
                                   {
                                       SensorId = (int)scc.CorrentSensorId,
                                       Name = s.SENSOR_LOCATION_DESCRIPTION
                                   };
                if (!(from b in db.T_THEMES_VIBRATION_BATCH from q in querySensors where b.SensorId == q.SensorId select b.CollectTime).Any())
                {
                    return null;
                }

                var lastDate =
                    (from b in db.T_THEMES_VIBRATION_BATCH from q in querySensors where b.SensorId == q.SensorId select b.CollectTime).Max();

                var lastBatch =
                    (from i in db.T_THEMES_VIBRATION_BATCH
                     from q in querySensors
                     where i.SensorId == q.SensorId && i.CollectTime == lastDate
                     select new
                     {
                         CollectTime = lastDate,//最新时段
                         BatchId = i.BatchId,
                         SensorId = q.SensorId
                     });
                
                var query2 = from s in db.T_DIM_SENSOR
                             from f in db.T_DIM_SAFETY_FACTOR_TYPE
                             from q in querySensors
                             from l in lastBatch
                             where q.SensorId == s.SENSOR_ID && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && l.SensorId == s.SENSOR_ID
                             && !s.IsDeleted
                             select
                                 new
                                 {
                                     SensorId = s.SENSOR_ID,
                                     Location = s.SENSOR_LOCATION_DESCRIPTION,
                                     Columns = f.FACTOR_VALUE_COLUMNS,
                                     data = (from o in db.T_THEMES_VIBRATION
                                             where o.BatchId == l.BatchId
                                             select o).Select(o => new
                                             {
                                                 Frequency = o.Frequency,
                                                 Value = o.Value//数据时间
                                             })
                                 };

                var info = query2.ToList();

                var oneUnit = new List<OneUnit>();

                foreach (var item in info)
                {
                    var one = new OneUnit();
                    one.Unit = Config.GetUnitBySensorID(item.SensorId)[0];
                    one.SensorId = item.SensorId;
                    oneUnit.Add(one);
                }
                var infoNew = from unit in oneUnit
                              from a in info
                              where a.SensorId == unit.SensorId
                              select new
                              {
                                  SensorId = a.SensorId,
                                  Location = a.Location,
                                  Columns = a.Columns,
                                  // Unit = f.FACTOR_VALUE_UNIT,
                                  Unit = unit.Unit,
                                  data = a.data
                              };

                return infoNew.ToList();
            }
        }
    }

    public class OneUnit
    {
        public int SensorId { get; set; }
        public string Unit { get; set; }
    }

    /// <summary>
    /// 热点
    /// </summary>
    public class LastBatch
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public Guid? BatchId { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int? SensorId { get; set; }
    }
}
