namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.HotSpot.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HotSpotController : ApiController
    {
        private readonly DAL.Data dalData = new DAL.Data();
        private IEnumerable<FactorConfig> configs;
        private int factorTypeVib = 54;//振动
        private int factorTypeWeld = 53;//焊缝
        private int factorTypeStress = 52;//应力
        private int factorTypeTemp = 26;//温度
        private int factorTypeWind = 30;//风速
        private int factorTypeSurfDis = 9;//表面位移
        private int factorTypeSettle = 11;//沉降
        private int factorTypeBearingDis = 50;//支座位移
        /// <summary>
        /// 获取热点
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns>热点列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物热点", false)]
        public IList<HotSpot> GetHotSpotInfoByStruct(int structId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];
            int beachFactorId = int.Parse(ConfigurationManager.AppSettings["BeachFactorId"]);
            int vibrationFactorId = int.Parse(ConfigurationManager.AppSettings["VibrationFactorId"]);
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query =
                    from sensor in entity.T_DIM_SENSOR
                    from structure in entity.T_DIM_STRUCTURE
                    join sg in entity.T_DIM_SENSOR_GROUP_CEXIE on sensor.SENSOR_ID equals sg.SENSOR_ID
                        into reslt
                    from r in reslt.DefaultIfEmpty()
                    from factor in entity.T_DIM_SAFETY_FACTOR_TYPE
                    from product in entity.T_DIM_SENSOR_PRODUCT
                    from protype in entity.T_DIM_PRODUCT_TYPE
                    join hotspot in entity.T_DIM_HOTSPOT on sensor.SENSOR_ID equals hotspot.SENSOR_ID
                        into h
                    from hotspot in h.DefaultIfEmpty()
                    where sensor.STRUCT_ID == structId
                          && !sensor.IsDeleted
                          && sensor.Identification != 1
                          && sensor.STRUCT_ID == structure.ID
                          && sensor.SAFETY_FACTOR_TYPE_ID == factor.SAFETY_FACTOR_TYPE_ID
                          && sensor.PRODUCT_SENSOR_ID == product.PRODUCT_ID
                          && product.PRODUCT_NAME != gpsBase
                          && product.PRODUCT_TYPE_ID == protype.PRODUCT_TYPE_ID
                          && factor.SAFETY_FACTOR_TYPE_ID != beachFactorId
                          && factor.SAFETY_FACTOR_TYPE_ID != vibrationFactorId
                          && hotspot.SECTION_ID == null
                    select new HotSpot
                    {
                        XAxis = hotspot.SPOT_X_AXIS,
                        YAxis = hotspot.SPOT_Y_AXIS,
                        SensorId = sensor.SENSOR_ID,
                        GroupId = r.GROUP_ID,
                        FactorId = factor.SAFETY_FACTOR_TYPE_ID,
                        ValueNumber = factor.FACTOR_VALUE_COLUMN_NUMBER,
                        StructName = structure.STRUCTURE_NAME_CN,
                        ProductTypeId = product.PRODUCT_TYPE_ID,
                        ProductName = protype.PRODUCT_TYPE_NAME,
                        Location = sensor.SENSOR_LOCATION_DESCRIPTION
                    };
                var infos = query.ToList();

                if (infos.Count == 0)
                {
                    return new List<HotSpot>();
                }

                // 绑定数据
                IList<MonitorData> dataSet = this.dalData.GetLastMonitorData(infos.Select(i => i.SensorId).ToArray());

                // 读取配置文件--修改了
                this.configs = Config.GetConfigByFactors(infos.Select(m => Convert.ToInt32(m.FactorId)).ToArray(), structId);

                MonitorData data;
                for (int i = 0; i < infos.Count; i++)
                {
                    data = dataSet.FirstOrDefault(d => d.SensorId == infos[i].SensorId);
                    if (data == null)
                    {
                        continue;
                    }

                    infos[i].Data =
                        this.FormatDataString(data, infos[i].FactorId);
                    infos[i].Time = data.AcquisitionTime.ToString();
                }


                // 绑定告警
                DataTable dt = new DAL.Warning().GetTopWarningLevel(infos.Select(i => i.SensorId).ToArray());
                for (int i = 0; i < infos.Count; i++)
                {
                    DataRow dr = dt.AsEnumerable().FirstOrDefault(r => r[0].ToString() == infos[i].SensorId.ToString());
                    if (dr == null)
                    {
                        infos[i].HasWarning = false;
                        infos[i].WarningLevel = 5;
                    }
                    else
                    {
                        infos[i].HasWarning = true;
                        infos[i].WarningLevel = dr[1] == DBNull.Value ? 4 : Convert.ToInt32(dr[1]);
                    }
                }

                return infos;
            }
        }


        /// <summary>
        /// 获取热点
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns>热点列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物热点", false)]
        public IList<HotSpotRShell> GetHotSpotInfoByStructRShell(int structId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];
            int beachFactorId = int.Parse(ConfigurationManager.AppSettings["BeachFactorId"]);
            int vibrationFactorId = int.Parse(ConfigurationManager.AppSettings["VibrationFactorId"]);
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query =
                    from sensor in entity.T_DIM_SENSOR
                    from structure in entity.T_DIM_STRUCTURE
                    from factor in entity.T_DIM_SAFETY_FACTOR_TYPE
                    from product in entity.T_DIM_SENSOR_PRODUCT
                    from protype in entity.T_DIM_PRODUCT_TYPE
                    join hotspot in entity.T_DIM_HOTSPOT on sensor.SENSOR_ID equals hotspot.SENSOR_ID
                        into h
                    from hotspot in h.DefaultIfEmpty()

                    //from sf in entity.T_DIM_FORMULAID_SET

                    where sensor.STRUCT_ID == structId
                          && !sensor.IsDeleted
                          && sensor.Identification != 1
                          && sensor.STRUCT_ID == structure.ID
                          && sensor.SAFETY_FACTOR_TYPE_ID == factor.SAFETY_FACTOR_TYPE_ID
                          && sensor.PRODUCT_SENSOR_ID == product.PRODUCT_ID
                          && product.PRODUCT_NAME != gpsBase
                          && product.PRODUCT_TYPE_ID == protype.PRODUCT_TYPE_ID
                          && factor.SAFETY_FACTOR_TYPE_ID != beachFactorId
                          && factor.SAFETY_FACTOR_TYPE_ID != vibrationFactorId
                          && hotspot.SECTION_ID == null
                          //&& sf.SENSOR_ID == sensor.SENSOR_ID
                    select new HotSpotRShell
                    {
                        XAxis = hotspot.SPOT_X_AXIS,
                        YAxis = hotspot.SPOT_Y_AXIS,
                        SensorId = sensor.SENSOR_ID,
                        FactorId = factor.SAFETY_FACTOR_TYPE_ID,
                        ValueNumber = factor.FACTOR_VALUE_COLUMN_NUMBER,
                        StructName = structure.STRUCTURE_NAME_CN,
                        ProductTypeId = product.PRODUCT_TYPE_ID,
                        ProductName = protype.PRODUCT_TYPE_NAME,
                        Location = sensor.SENSOR_LOCATION_DESCRIPTION,
                        //DataTypeParam = sf.Parameter1
                    };
                var infos = query.ToList();

                if (infos.Count == 0)
                {
                    return new List<HotSpotRShell>();
                }

                // 绑定告警
                DataTable dt = new DAL.Warning().GetTopWarningLevelRShell(infos.Select(i => i.SensorId).ToArray());
                for (int i = 0; i < infos.Count; i++)
                {
                    DataRow dr = dt.AsEnumerable().FirstOrDefault(r => r[0].ToString() == infos[i].SensorId.ToString());
                    if (dr == null)
                    {
                        infos[i].HasWarning = false;
                        infos[i].WarningLevel = 5;
                    }
                    else
                    {
                        infos[i].HasWarning = true;
                        infos[i].WarningLevel = dr[1] == DBNull.Value ? 4 : Convert.ToInt32(dr[1]);
                    }
                    // 查询截面类型
                    var sId = infos[i].SensorId;
                    var q = entity.T_DIM_FORMULAID_SET.Where(x => x.SENSOR_ID == sId).Select(y => y.Parameter1).FirstOrDefault();
                    infos[i].DataTypeParam = q;
                }
                return infos;
            }
        }


         /// <summary>
        /// 获取结构物类型
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns>结构物类型编号</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物类型", false)]
        public string GetStructTypeRShell(int structId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_STRUCTURE
                            where s.ID == structId
                            select s.STRUCTURE_TYPE_ID;

                return query.ToList().First();
            }
        }

        class SensorData
        {
            public double? Param3;
            public string Name;
            public int SensorId;
            public DateTime? AcquisitionTime;
            public double? Value;
            public string Unit = "με";
        }
        class SensorYLData
        {
            public DateTime? AcquisitionTime;
            public double? Value;
            public string Unit = "MPa";
            public List<SensorData> data;
        }
        /// <summary>
        /// 获取网壳热点数据
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns>结构物类型编号</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物类型", false)]
        public object GetRShellHotspotData(int sensorId, int factorId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var safety_factor_type_id = factorId;

                var unitList = Config.GetUnitByFactorId(sensorId, factorId);//单位
                
                //温度
                if (safety_factor_type_id == factorTypeTemp)
                {
                    var we1 = unitList[0];
                  int maxId = entity.T_THEMES_ENVI_TEMP_HUMI.Where(i => i.SENSOR_ID == sensorId).Select(i => i.ID).DefaultIfEmpty().Max();
                  var query = from d in entity.T_THEMES_ENVI_TEMP_HUMI
                              from s in entity.T_DIM_SENSOR
                              where s.SENSOR_ID == sensorId && s.SENSOR_ID == d.SENSOR_ID
                              orderby d.ACQUISITION_DATETIME descending
                              select new
                              {
                                  SensorId = d.SENSOR_ID,
                                  AcquisitionTime = d.ACQUISITION_DATETIME,
                                  ValueTemp = d.TEMPERATURE_VALUE,
                                  Unit = we1

                              };

                  return query.ToList().FirstOrDefault();
                }
                else if (safety_factor_type_id == factorTypeWind)
                {//风速
                    var unit1 = unitList[0];
                    var unit2 = unitList[1];
                    var unit3 = unitList[2];

                    var query = from d in entity.T_THEMES_ENVI_WIND
                                from s in entity.T_DIM_SENSOR
                                where s.SENSOR_ID == sensorId && s.SENSOR_ID == d.SENSOR_ID
                                orderby d.ACQUISITION_DATETIME descending
                                select new
                                {
                                    SensorId = d.SENSOR_ID,
                                    AcquisitionTime = d.ACQUISITION_DATETIME,
                                    ValuesSpeed = d.WIND_SPEED_VALUE,
                                    UnitSpeed = unit1,
                                    ValuesDir = d.WIND_DIRECTION_VALUE,
                                    UnitDir = unit2,
                                    ValuesElev = d.WIND_ELEVATION_VALUE,
                                    UnitElecv = unit3
                                };

                    return query.ToList().FirstOrDefault();
                }
                else if (safety_factor_type_id == factorTypeSurfDis)
                {//表面位移
                    var unit1 = unitList[0];
                    
                    var query = from d in entity.T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT
                                from s in entity.T_DIM_SENSOR
                                where s.SENSOR_ID == sensorId && s.SENSOR_ID == d.SENSOR_ID
                                orderby d.ACQUISITION_DATETIME descending
                                select new
                                {
                                    SensorId = d.SENSOR_ID,
                                    AcquisitionTime = d.ACQUISITION_DATETIME,
                                    ValuesX = d.SURFACE_DISPLACEMENT_X_VALUE,
                                    ValuesY = d.SURFACE_DISPLACEMENT_Y_VALUE,
                                    ValuesZ = d.SURFACE_DISPLACEMENT_Z_VALUE,
                                    Unit = unit1
                                };
                    return query.ToList().FirstOrDefault();
                }
                else if (safety_factor_type_id == factorTypeSettle)
                {//沉降
                    var unit1 = unitList[0];
                    var query = from d in entity.T_THEMES_DEFORMATION_SETTLEMENT
                                from s in entity.T_DIM_SENSOR
                                where s.SENSOR_ID == sensorId && s.SENSOR_ID == d.SENSOR_ID
                                orderby d.ACQUISITION_DATETIME descending
                                select new
                                {
                                    SensorId = d.SENSOR_ID,
                                    AcquisitionTime = d.ACQUISITION_DATETIME,
                                    Values = d.SETTLEMENT_VALUE,
                                    Unit = unit1
                                };

                    return query.ToList().FirstOrDefault();
                }
                else if (safety_factor_type_id == factorTypeBearingDis)
                {//支座位移
                    var unit1 = unitList[0];
                    var query = from d in entity.T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT
                                from s in entity.T_DIM_SENSOR
                                where s.SENSOR_ID == sensorId && s.SENSOR_ID == d.SENSOR_ID
                                orderby d.ACQUISITION_DATETIME descending
                                select new
                                {
                                    SensorId = d.SENSOR_ID,
                                    AcquisitionTime =d.ACQUISITION_DATETIME,
                                    ValuesX = d.SURFACE_DISPLACEMENT_X_VALUE,
                                    Unit = unit1
                                };
                   

                    return query.ToList().FirstOrDefault();
                }
                else if (safety_factor_type_id == factorTypeStress)
                {//应力
                    var querySensors = from scc in entity.T_DIM_SENSOR_CORRENT
                                       from s in entity.T_DIM_SENSOR
                                       from f in entity.T_DIM_FORMULAID_SET
                                       where scc.SensorId == sensorId && s.SENSOR_ID == scc.CorrentSensorId && scc.CorrentSensorId == f.SENSOR_ID
                                       select new
                                       {
                                           SensorId = (int)scc.CorrentSensorId,
                                           Name = s.SENSOR_LOCATION_DESCRIPTION,
                                           Param3=f.Parameter3
                                       };
   var sensors = querySensors.ToList();
  var unitLast = Config.GetUnitBySensorID(sensors[0].SensorId);//新增
  var unit = unitLast[0];

                    var query = from fb in entity.T_THEMES_FORCE_BEAM
                                where fb.SENSOR_ID == sensorId
                                orderby fb.ACQUISITION_DATETIME descending
                                select new
                                {
                                    AcquisitionTime = fb.ACQUISITION_DATETIME,
                                    Value = fb.FORCE_VALUE,
                                    Unit = unit,
                                    data = (from d in entity.T_THEMES_FBG_STRESS_STRAIN
                                            from q in querySensors
                                            where d.SENSOR_ID == q.SensorId && d.ACQUISITION_DATETIME == fb.ACQUISITION_DATETIME
                                            group d by new {q.SensorId ,q.Name, q.Param3, d.STRESS_STRAIN_VALUE } into g
                                            select new SensorData
                                            {
                                                SensorId=g.Key.SensorId,
                                                Param3 =(double?) g.Key.Param3,
                                                Name = g.Key.Name,
                                                AcquisitionTime = fb.ACQUISITION_DATETIME,
                                                //Value = g.Where(x => x.ACQUISITION_DATETIME == fb.ACQUISITION_DATETIME).Select(y => y.STRESS_STRAIN_VALUE).FirstOrDefault(),
                                                Value =(double?) g.Key.STRESS_STRAIN_VALUE,
                                                Unit = unit
                                            })
                                };
                    var results = query.FirstOrDefault();//先查出有数据的传感器
                    if (results == null) {
                        return results;
                    } else {
                        List<SensorData> result = new List<SensorData>();
                        var v = new SensorYLData
                        {
                            AcquisitionTime = results.AcquisitionTime,
                            Value = (double?)results.Value,
                            Unit = results.Unit
                        };
                        foreach (var s in sensors)
                        {
                            var d = new SensorData
                            {
                                Param3 = Convert.ToDouble(s.Param3),
                                Name = s.Name,
                                SensorId = s.SensorId,
                                AcquisitionTime = results.AcquisitionTime,
                                Value = (double?)null,
                                Unit = "με"
                            };
                            
                            foreach (var vi in results.data)
                            {
                                if (vi.SensorId == d.SensorId)
                                {
                                    d.Value = vi.Value;
                                    break;
                                }
                            }
                            result.Add(d);
                        }
                        v.data = result;
                        return v;
                    }

                }
                else if (safety_factor_type_id == factorTypeWeld)
                {//焊缝应变
                    var unit = unitList[0];
                    var querySensors = from scc in entity.T_DIM_SENSOR_CORRENT
                                       from s in entity.T_DIM_SENSOR
                                       from f in entity.T_DIM_FORMULAID_SET
                                       where scc.SensorId == sensorId && s.SENSOR_ID == scc.CorrentSensorId && scc.CorrentSensorId == f.SENSOR_ID
                                       select new
                                       {
                                           SensorId = (int)scc.CorrentSensorId,
                                           Name = s.SENSOR_LOCATION_DESCRIPTION,
                                           Param3 = f.Parameter3
                                       };
                    var sensors = querySensors.ToList();
                    List<SensorData> result = new List<SensorData>();
                    var hasRecord = SqlHelper.ExecuteScalar(CommandType.Text, string.Format(@"select count(*)
	from T_THEMES_FBG_STRESS_STRAIN T
	left join T_DIM_SENSOR_CORRENT C 
	on T.SENSOR_ID = C.CorrentSensorId
	where C.SensorId = {0}", sensorId), null);

                    if ((int)hasRecord > 0)
                    {
                        string getMaxTime = string.Format(@"select top 1 ACQUISITION_DATETIME from [dbo].[T_THEMES_FBG_STRESS_STRAIN] 
where SENSOR_ID in (
	select CorrentSensorId from T_DIM_SENSOR_CORRENT where SensorId = {0}
) ORDER BY ACQUISITION_DATETIME DESC", sensorId);
                        var maxTime = (System.DateTime)SqlHelper.ExecuteScalar(CommandType.Text, getMaxTime, null);
                        var values = from s in entity.T_THEMES_FBG_STRESS_STRAIN
                                     from q in querySensors
                                     where s.SENSOR_ID == q.SensorId && s.ACQUISITION_DATETIME == maxTime
                                     select new
                                     {
                                         SensorId = q.SensorId,
                                         Value = s.STRESS_STRAIN_VALUE
                                     };
                        var results = values.ToList();
                        foreach (var s in sensors)
                        {
                            var v = new SensorData
                            {
                                Param3 = Convert.ToDouble(s.Param3),
                                Name = s.Name,
                                SensorId = s.SensorId,
                                AcquisitionTime = maxTime,
                                Value = (double?)null,
                               Unit = unit
                            };
                            foreach (var vi in results)
                            {
                                if (vi.SensorId == s.SensorId)
                                {
                                    v.Value = Convert.ToDouble(vi.Value);
                                    break;
                                }
                            }
                            result.Add(v);
                        }
                    }
                    else
                    {
                        // no record.
                        foreach (var s in sensors)
                        {
                            var v = new SensorData
                            {
                                Param3 = Convert.ToDouble(s.Param3),
                                Name = s.Name,
                                SensorId = s.SensorId,
                                AcquisitionTime = (DateTime?)null,
                                Value =(double?) null,
                                Unit = unit
                            };
                            result.Add(v);
                        }
                    }
                    return result;
                }
                else if (safety_factor_type_id == factorTypeVib)
                {//网壳振动
                    var unit = unitList[0];
                    var querySensors = from scc in entity.T_DIM_SENSOR_CORRENT
                                       from s in entity.T_DIM_SENSOR
                                       where scc.SensorId == sensorId && s.SENSOR_ID==scc.CorrentSensorId
                                       select new {
                                           SensorId = (int)scc.CorrentSensorId,
                                           Name = s.SENSOR_LOCATION_DESCRIPTION
                                       };
                    var batches = from b in entity.T_THEMES_VIBRATION_BATCH
                                  from s in querySensors
                                  where s.SensorId == b.SensorId
                                  group b by new { b.SensorId, s.Name } into g
                                  select  new {
                                      g.Key.Name,
                                      BatchId = g.Where(X=> X.CollectTime == (g.Max(b => b.CollectTime))).Select(y=>y.BatchId).FirstOrDefault()
                                  };
                    var maxTime = (from ba in batches
                                   from or in entity.T_THEMES_VIBRATION_ORIGINAL
                                   where ba.BatchId == or.BatchId
                                   select or.CollectTime).DefaultIfEmpty().Max();

                    var maxTimeParam = maxTime;
                    var a = from q in batches
                            select new
                            {
                                q.Name,
                                Data = (from vo in entity.T_THEMES_VIBRATION_ORIGINAL
                                        where vo.CollectTime == maxTimeParam && vo.BatchId == q.BatchId
                                        select vo.Speed == null ? null : (decimal?)vo.Speed),
                                DateTime = maxTime,
                                Unit = unit
                            };


                    return a.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取实时热点
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns>实时热点列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取实时热点", false)]
        public object GetRealTimeHotSpotInfoByStruct(int structId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query =
                    from hotspot in entity.T_DIM_HOTSPOT
                    from sensor in entity.T_DIM_SENSOR
                    where hotspot.SENSOR_ID == sensor.SENSOR_ID
                          && sensor.STRUCT_ID == structId
                          && hotspot.IS_REALTIME_SENSOR
                          && !sensor.IsDeleted
                          && sensor.Identification != 1
                    select new
                    {
                        sensorId = hotspot.SENSOR_ID,
                        xAxis = hotspot.INFO_X_AXIS,
                        yAxis = hotspot.INFO_Y_AXIS
                    };
                return query.ToList();
            }
        }

        /// <summary>
        /// 获取热点图配置
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取热点图配置", false)]
        [Authorization(AuthorizationCode.S_Org_Logo_Layout)]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public IList<HotSpotConfig> GetHotSpotConfigByStruct(int structId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var sensors =
                    from sensor in entity.T_DIM_SENSOR
                    where sensor.STRUCT_ID == structId
                          && sensor.T_DIM_SENSOR_PRODUCT.PRODUCT_NAME != gpsBase
                          && !sensor.IsDeleted && sensor.Identification != 1
                    select sensor;

                var query = from sensor in sensors
                            join hotspot in entity.T_DIM_HOTSPOT on sensor.SENSOR_ID equals hotspot.SENSOR_ID
                                into hotspot
                            from h in hotspot.DefaultIfEmpty()
                            where h.SPOT_X_AXIS != null && h.SPOT_Y_AXIS != null && h.SECTION_ID == null
                            select new HotSpotConfig
                            {
                                HotspotId = h.ID,
                                SensorId = sensor.SENSOR_ID,
                                XAxis = h.SPOT_X_AXIS,
                                YAxis = h.SPOT_Y_AXIS,
                                ProductTypeId = sensor.T_DIM_SENSOR_PRODUCT.T_DIM_PRODUCT_TYPE.PRODUCT_TYPE_ID,
                                ProductName = sensor.T_DIM_SENSOR_PRODUCT.T_DIM_PRODUCT_TYPE.PRODUCT_TYPE_NAME,
                                Location = sensor.SENSOR_LOCATION_DESCRIPTION,
                                SvgPath = h.SPOT_PATH
                            };
                var infos = query.ToList();
                return infos;
            }
        }

        /// <summary>
        /// 获取未配置的传感器
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取未配置热点的传感器", false)]
        [Authorization(AuthorizationCode.S_Org_Logo_Layout)]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public IList<HotSpotNonConfig> GetHotSpotNonConfigByStruct(int structId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var sensors =
                    from sensor in entity.T_DIM_SENSOR
                    where sensor.STRUCT_ID == structId
                          && sensor.T_DIM_SENSOR_PRODUCT.PRODUCT_NAME != gpsBase
                          && !sensor.IsDeleted 
                    select sensor;

                var query = from sensor in sensors
                            join hotspot in entity.T_DIM_HOTSPOT on sensor.SENSOR_ID equals hotspot.SENSOR_ID
                                into hotspot
                            from h in hotspot.DefaultIfEmpty()
                            where h.SPOT_X_AXIS == null || h.SPOT_Y_AXIS == null
                            select new HotSpotNonConfig
                            {
                                SensorId = sensor.SENSOR_ID,
                                ProductTypeId = sensor.T_DIM_SENSOR_PRODUCT.T_DIM_PRODUCT_TYPE.PRODUCT_TYPE_ID,
                                ProductName = sensor.T_DIM_SENSOR_PRODUCT.T_DIM_PRODUCT_TYPE.PRODUCT_TYPE_NAME,
                                Location = sensor.SENSOR_LOCATION_DESCRIPTION
                            };
                var infos = query.ToList();
                return infos;
            }
        }

        /// <summary>
        /// 获取未配置热点的传感器(根据传感器类型分组)
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取未配置热点的传感器(根据传感器类型分组)", false)]
        public object GetProductHotspotNonConfigByStruct(int structId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (var entity = new SecureCloud_Entities())
            {
                var sensors = from sensor in entity.T_DIM_SENSOR
                              where sensor.STRUCT_ID == structId
                              && sensor.T_DIM_SENSOR_PRODUCT.PRODUCT_NAME != gpsBase
                              && !sensor.IsDeleted && sensor.Identification != 1
                              select sensor;

                var query = from sensor in sensors
                            join hotspot in entity.T_DIM_HOTSPOT on sensor.SENSOR_ID equals hotspot.SENSOR_ID into nhotspot
                            from ihotspot in nhotspot.DefaultIfEmpty()
                            where ihotspot.SPOT_X_AXIS == null || ihotspot.SPOT_Y_AXIS == null
                            select new
                            {
                                sensorId = sensor.SENSOR_ID,
                                productTypeId = sensor.T_DIM_SENSOR_PRODUCT.T_DIM_PRODUCT_TYPE.PRODUCT_TYPE_ID,
                                productName = sensor.T_DIM_SENSOR_PRODUCT.T_DIM_PRODUCT_TYPE.PRODUCT_TYPE_NAME,
                                location = sensor.SENSOR_LOCATION_DESCRIPTION
                            };
                var list = query.ToList();
                return 
                    list.GroupBy(g => new { g.productTypeId, g.productName }).Select(s =>
                        new JObject(
                            new JProperty("productTypeId", s.Key.productTypeId),
                            new JProperty("productName", s.Key.productName),
                            new JProperty("sensors", 
                                new JArray(s.Select(v => 
                                    new JObject(
                                        new JProperty("sensorId", v.sensorId),
                                        new JProperty("location", v.location)))))));
            }
        }

        /// <summary>
        /// 添加热点
        /// </summary>
        /// <param name="config">热点配置</param>
        /// <returns>添加结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("添加热点", true)]
        [Authorization(AuthorizationCode.S_Org_Logo_Layout)]
        public HttpResponseMessage AddHotSpotConfig([FromBody]HotSpotConfig config)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                T_DIM_HOTSPOT hotspot = new T_DIM_HOTSPOT();
                hotspot.SENSOR_ID = config.SensorId;
                hotspot.SPOT_X_AXIS = config.XAxis;
                hotspot.SPOT_Y_AXIS = config.YAxis;
                hotspot.SPOT_PATH = config.SvgPath;

                DbEntityEntry<T_DIM_HOTSPOT> entry = entity.Entry<T_DIM_HOTSPOT>(hotspot);
                entry.State = EntityState.Added;

                #region 日志信息

                var sensor =
                    entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == config.SensorId)
                        .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                        .FirstOrDefault();

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                this.Request.Properties["ActionParameterShow"] = string.Format("传感器：{0}", sensor);
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        new JObject(new JProperty("hotspotId", hotspot.ID)).ToString());
                }
                catch
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("添加失败"));
                }
            }
        }

        /// <summary>
        /// 修改结构物或施工截面的传感器热点配置
        /// </summary>
        /// <param name="hotspotId"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改结构物或施工截面的传感器热点配置", true)]
        [Authorization(AuthorizationCode.S_Org_Logo_Layout)]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public object ModifyHotSpotConfig([FromUri]int hotspotId, [FromBody]HotSpotConfig config)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var hotspot = entity.T_DIM_HOTSPOT.FirstOrDefault(s => s.ID == hotspotId);
                if (hotspot == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("传感器热点不存在，修改失败"));
                }

                if (config.SensorId != default(int))
                {
                    var sensor = entity.T_DIM_SENSOR.FirstOrDefault(s => s.SENSOR_ID == config.SensorId);
                    if (sensor == null)
                    {
                        return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("此传感器不存在，修改失败"));
                    }
                    hotspot.SENSOR_ID = config.SensorId;
                }
                if (config.XAxis != default(decimal?))
                {
                    hotspot.SPOT_X_AXIS = config.XAxis;
                }
                if (config.YAxis != default(decimal?))
                {
                    hotspot.SPOT_Y_AXIS = config.YAxis;
                }
                hotspot.SPOT_PATH = config.SvgPath == string.Empty ? null : config.SvgPath; // svgPath为"",则更新为null值.

                #region 日志信息
                var location =
                    entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == config.SensorId)
                        .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                        .FirstOrDefault();
                Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                Request.Properties["ActionParameterShow"] = string.Format("传感器：{0}", location ?? string.Empty);
                #endregion

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
                        StringHelper.GetMessageString("修改传感器热点失败"));
                }
            }
        }

        /// <summary>
        /// 删除热点配置
        /// </summary>
        /// <param name="hotspots">热点配置编号</param>
        /// <returns>删除结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除热点", true)]
        [Authorization(AuthorizationCode.S_Org_Logo_Layout)]
        public HttpResponseMessage RemoveHotspotConfig(string hotspots)
        {
            int[] arrHotspotId = hotspots.Split(',').Select(s => Convert.ToInt32(s)).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("共{0}条: ", arrHotspotId.Length);

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                foreach (var hotspotId in arrHotspotId)
                {
                    #region 日志信息
                    var sensor = (from h in entity.T_DIM_HOTSPOT
                                  from s in entity.T_DIM_SENSOR
                                  where h.SENSOR_ID == s.SENSOR_ID && h.ID == hotspotId
                                  select s.SENSOR_LOCATION_DESCRIPTION).FirstOrDefault();
                    if (sensor == null)
                    {
                        return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("传感器热点不存在，删除失败")); 
                    }
                    sb.AppendFormat(" 传感器:{0};", sensor);
                    #endregion

                    T_DIM_HOTSPOT hotspot = new T_DIM_HOTSPOT { ID = hotspotId };

                    DbEntityEntry<T_DIM_HOTSPOT> entry = entity.Entry<T_DIM_HOTSPOT>(hotspot);
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
                        StringHelper.GetMessageString("删除成功"));
                }
                catch
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除失败"));
                }
            }
        }

        /// <summary>
        /// 格式化数据字符串
        /// </summary>
        /// <param name="monitorData"> 监测数据 </param>
        /// <param name="factorId"> 监测因素id </param>
        /// <returns> <see cref="string"/>格式化后的字符串. </returns>
        [NonAction]
        private string FormatDataString(MonitorData monitorData, int factorId)
        {
            StringBuilder sb = new StringBuilder(100);
            FactorConfig config = this.configs.FirstOrDefault(c => c.Id == factorId);
            if (config == null)
            {
                throw new ConfigurationErrorsException(string.Format("缺少 factorid:{0} 的配置数据", factorId));
            }

            for (int i = 0; i < config.Display.Length; i++)
            {
                var v = monitorData.Values[i];
                if (v != null)
                {
                    sb.AppendFormat(
                        "{0}：{1}{2}",
                        config.Display[i],
                        v.Value.ToString("f" + config.DecimalPlaces[i]),
                        config.Unit[i]);
                }
                else
                {
                    sb.AppendFormat(
                        "{0}：{1}{2}",
                        config.Display[i],
                        null,
                        config.Unit[i]);
                }
                if (i != config.Display.Length - 1)
                {
                    sb.Append("<br/>");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取施工截面的传感器热点配置
        /// </summary>
        /// <param name="sectionId"> 施工截面编号 </param>
        /// <returns> 施工截面的传感器热点列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取施工截面的传感器热点配置", false)]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public object GetHotSpotConfigBySection(int sectionId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from sc in entity.T_DIM_SECTION
                            from h in entity.T_DIM_HOTSPOT
                            from se in entity.T_DIM_SENSOR
                            where sc.SectionId == sectionId
                                && sc.SectionId == h.SECTION_ID
                                && h.SENSOR_ID == se.SENSOR_ID
                                && !se.IsDeleted && se.Identification != 1
                            select new HotSpotConfig
                            {
                                HotspotId = h.ID,
                                SensorId = h.SENSOR_ID,
                                XAxis = h.SPOT_X_AXIS,
                                YAxis = h.SPOT_Y_AXIS,
                                ProductTypeId = se.T_DIM_SENSOR_PRODUCT.PRODUCT_TYPE_ID,
                                ProductName = se.T_DIM_SENSOR_PRODUCT.PRODUCT_NAME,
                                Location = se.SENSOR_LOCATION_DESCRIPTION,
                                SvgPath = h.SPOT_PATH
                            };
                return query.ToList();
            }
        }

        /// <summary>
        /// 添加施工截面的传感器热点配置
        /// </summary>
        /// <param name="sectionId"> 施工截面编号 </param>
        /// <param name="config"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("添加施工截面的传感器热点配置", true)]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public HttpResponseMessage AddHotSpotConfigBySection([FromUri]int sectionId, [FromBody]HotSpotConfig config)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var section = entity.T_DIM_SECTION.FirstOrDefault(s => s.SectionId == sectionId);
                if (section == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("施工截面不存在，新增截面的传感器热点失败"));
                }

                var hotspot = new T_DIM_HOTSPOT
                {
                    SECTION_ID = sectionId,
                    SENSOR_ID = config.SensorId,
                    SPOT_X_AXIS = config.XAxis,
                    SPOT_Y_AXIS = config.YAxis,
                    SPOT_PATH = config.SvgPath
                };

                DbEntityEntry<T_DIM_HOTSPOT> entry = entity.Entry(hotspot);
                entry.State = EntityState.Added;

                #region 日志信息
                var sensor =
                    entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == config.SensorId)
                        .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                        .FirstOrDefault();

                Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                Request.Properties["ActionParameterShow"] = string.Format("传感器：{0}", sensor ?? string.Empty);
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        new JObject(new JProperty("hotspotId", hotspot.ID)).ToString());
                }
                catch
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("添加施工截面的传感器热点配置失败"));
                }
            }
        }

        /// <summary>
        /// 添加结构物下施工截面以及截面的多个传感器热点配置
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("添加结构物下施工截面以及截面的多个传感器热点配置", true)]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public HttpResponseMessage AddSectionAndHotSpotsConfig([FromUri]int structId, [FromBody]SectionHotspotsConfig model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var structure = entity.T_DIM_STRUCTURE.FirstOrDefault(s => s.ID == structId);
                if (structure == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("结构物不存在, 新增截面失败."));
                }
                if (entity.T_DIM_SECTION.Where(w => w.StructId == structId).Any(a => a.SectionName == model.SectionName))
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("该结构物已存在此截面名称, 新增截面失败."));
                }

                // 新增施工截面
                var section = new T_DIM_SECTION
                {
                    SectionName = model.SectionName,
                    SectionStatus = model.SectionStatus,
                    HeapMapName = model.HeapMapName,
                    StructId = structId
                };
                var entry1 = entity.Entry(section);
                entry1.State = EntityState.Added;

                if (model.HotSpotsConfig == null) // Web端代理(Proxy.ashx.cs)没有传递"[](空数组)"参数
                {
                    model.HotSpotsConfig = new List<HotSpotConfig>();
                }
                // 新增施工截面的传感器热点
                foreach (var config in model.HotSpotsConfig)
                {
                    var isExisted = entity.T_DIM_HOTSPOT.Any(a => a.SENSOR_ID == config.SensorId);
                    if (isExisted)
                    {
                        return
                            Request.CreateResponse(
                                System.Net.HttpStatusCode.BadRequest,
                                StringHelper.GetMessageString("添加施工截面的传感器热点失败, 原因: 传感器热点已存在."));
                    }
                    var hotspot = new T_DIM_HOTSPOT
                    {
                        SECTION_ID = section.SectionId,
                        SENSOR_ID = config.SensorId,
                        SPOT_X_AXIS = config.XAxis,
                        SPOT_Y_AXIS = config.YAxis,
                        SPOT_PATH = config.SvgPath
                    };
                    DbEntityEntry<T_DIM_HOTSPOT> entry2 = entity.Entry(hotspot);
                    entry2.State = EntityState.Added;
                }

                #region 日志信息

                var querySensors = from c in model.HotSpotsConfig
                    from s in entity.T_DIM_SENSOR
                    where c.SensorId == s.SENSOR_ID
                    select new
                    {
                        location = s.SENSOR_LOCATION_DESCRIPTION
                    };
                var sensors = string.Join(",", querySensors.Select(s => s.location).ToArray());
                Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                Request.Properties["ActionParameterShow"] = string.Format("新增截面名称:{0}. 传感器热点:{1}.", model.SectionName, sensors);

                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("添加施工截面及其多个传感器热点成功."));
                }
                catch
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("添加施工截面及其多个传感器热点失败."));
                }
            }
        }

        /// <summary>
        /// 删除施工截面的传感器热点配置
        /// </summary>
        /// <param name="hotspotId"> 传感器热点编号 </param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除施工截面的传感器热点配置", true)]
        [Authorization(AuthorizationCode.S_Structure_Construct_Section_Modify)]
        public HttpResponseMessage RemoveHotSpotConfigBySection(int hotspotId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var hs = entity.T_DIM_HOTSPOT.Select(s => new { s.ID, s.SENSOR_ID }).FirstOrDefault(w => w.ID == hotspotId);
                if (hs == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("传感器热点不存在，删除失败"));
                }

                T_DIM_HOTSPOT hotspot = new T_DIM_HOTSPOT();
                hotspot.ID = hotspotId;

                DbEntityEntry<T_DIM_HOTSPOT> entry = entity.Entry<T_DIM_HOTSPOT>(hotspot);
                entry.State = EntityState.Deleted;

                #region 日志信息
                var sensor = (from h in entity.T_DIM_HOTSPOT
                              from s in entity.T_DIM_SENSOR
                              where h.SENSOR_ID == s.SENSOR_ID && h.ID == hotspotId
                              select s.SENSOR_LOCATION_DESCRIPTION).FirstOrDefault();
                Request.Properties["ActionParameterShow"] = string.Format("传感器：{0}", sensor ?? string.Empty);
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("删除施工截面的传感器热点配置成功"));
                }
                catch
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除施工截面的传感器热点配置失败"));
                }
            }
        }

        /// <summary>
        /// 获取施工截面的传感器热点信息
        /// </summary>
        /// <param name="sectionId"> 施工截面编号 </param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取施工截面的传感器热点信息", false)]
        public object GetHotSpotInfoBySection(int sectionId)
        {
            string gpsBase = ConfigurationManager.AppSettings["GPSBaseStation"];
            int beachFactorId = int.Parse(ConfigurationManager.AppSettings["BeachFactorId"]);
            int vibrationFactorId = int.Parse(ConfigurationManager.AppSettings["VibrationFactorId"]);
            using (var entity = new SecureCloud_Entities())
            {
                var query = from sc in entity.T_DIM_SECTION
                            from h in entity.T_DIM_HOTSPOT
                            from se in entity.T_DIM_SENSOR
                            join sg in entity.T_DIM_SENSOR_GROUP_CEXIE on se.SENSOR_ID equals sg.SENSOR_ID into nsg
                            from isg in nsg.DefaultIfEmpty()
                            from factor in entity.T_DIM_SAFETY_FACTOR_TYPE
                            from product in entity.T_DIM_SENSOR_PRODUCT
                            from pt in entity.T_DIM_PRODUCT_TYPE
                            where sc.SectionId == sectionId
                                && sc.SectionId == h.SECTION_ID
                                && h.SENSOR_ID == se.SENSOR_ID
                                && !se.IsDeleted && se.Identification != 1
                                && se.SAFETY_FACTOR_TYPE_ID == factor.SAFETY_FACTOR_TYPE_ID
                                && factor.SAFETY_FACTOR_TYPE_ID != beachFactorId
                                && factor.SAFETY_FACTOR_TYPE_ID != vibrationFactorId
                                && se.PRODUCT_SENSOR_ID == product.PRODUCT_ID
                                && product.PRODUCT_NAME != gpsBase
                                && product.PRODUCT_TYPE_ID == pt.PRODUCT_TYPE_ID
                            select new HotSpotOfSection
                            {
                                SensorId = h.SENSOR_ID,
                                XAxis = h.SPOT_X_AXIS,
                                YAxis = h.SPOT_Y_AXIS,
                                GroupId = isg.GROUP_ID,
                                FactorId = factor.SAFETY_FACTOR_TYPE_ID,
                                ValueNumber = factor.FACTOR_VALUE_COLUMN_NUMBER,
                                StructName = sc.T_DIM_STRUCTURE.STRUCTURE_NAME_CN,
                                SectionName = sc.SectionName,
                                ProductTypeId = product.PRODUCT_TYPE_ID,
                                ProductName = pt.PRODUCT_TYPE_NAME,
                                Location = se.SENSOR_LOCATION_DESCRIPTION,
                                SvgPath = h.SPOT_PATH
                            };
                var list = query.ToList();

                if (list.Count == 0)
                {
                    return new JArray();
                }

                // 传感器最新数据
                var dataSet = dalData.GetLastMonitorData(list.Select(s => s.SensorId).ToArray());

                var firstSensorId = query.Select(s => s.SensorId).FirstOrDefault();
                if (firstSensorId != 0)
                {
                    var structId = Config.GetStructId(firstSensorId);
                    configs = Config.GetConfigByFactors(list.Select(s => Convert.ToInt32(s.FactorId)).ToArray(), structId);
                }
                
                for (int i = 0; i < list.Count; i++)
                {
                    MonitorData data = dataSet.FirstOrDefault(d => d.SensorId == list[i].SensorId);
                    if (data == null)
                    {
                        continue;
                    }

                    list[i].Data = FormatDataString(data, list[i].FactorId);
                    list[i].Time = data.AcquisitionTime.ToString();
                }

                // 传感器最严重告警等级
                DataTable dt = new DAL.Warning().GetTopWarningLevel(list.Select(s => s.SensorId).ToArray());
                for (int i = 0; i < list.Count; i++)
                {
                    DataRow dr = dt.AsEnumerable().FirstOrDefault(r => r[0].ToString() == list[i].SensorId.ToString());
                    if (dr == null)
                    {
                        list[i].HasWarning = false;
                        list[i].WarningLevel = 5;
                    }
                    else
                    {
                        list[i].HasWarning = true;
                        list[i].WarningLevel = dr[1] == DBNull.Value ? 4 : Convert.ToInt32(dr[1]);
                    }
                }

                return list;
            }
        }
    }

    /// <summary>
    /// 热点
    /// </summary>
    public class HotSpot
    {
        /// <summary>
        /// X坐标
        /// </summary>
        [JsonProperty("xAxis")]
        public decimal? XAxis { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        [JsonProperty("yAxis")]
        public decimal? YAxis { get; set; }

        /// <summary>
        /// 传感器Id
        /// </summary>
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        /// <summary>
        /// 分组Id
        /// </summary>
        [JsonProperty("groupId")]
        public int? GroupId { get; set; }

        /// <summary>
        /// 因素Id
        /// </summary>
        [JsonProperty("factorId")]
        public int FactorId { get; set; }

        /// <summary>
        /// 数值数量
        /// </summary>
        [JsonProperty("valueNumber")]
        public int? ValueNumber { get; set; }

        /// <summary>
        /// 结构物名称
        /// </summary>
        [JsonProperty("structName")]
        public string StructName { get; set; }

        /// <summary>
        /// 设备类型Id
        /// </summary>
        [JsonProperty("productTypeId")]
        public string ProductTypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }

        /// <summary>
        /// 有无告警
        /// </summary>
        [JsonProperty("hasWarning")]
        public bool HasWarning { get; set; }

        /// <summary>
        /// 告警等级
        /// </summary>
        [JsonProperty("warningLevel")]
        public int WarningLevel { get; set; }
    }

    /// <summary>
    /// 热点
    /// </summary>
    public class HotSpotRShell
    {
        /// <summary>
        /// X坐标
        /// </summary>
        [JsonProperty("xAxis")]
        public decimal? XAxis { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        [JsonProperty("yAxis")]
        public decimal? YAxis { get; set; }

        /// <summary>
        /// 传感器Id
        /// </summary>
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        /// <summary>
        /// 分组Id
        /// </summary>
        [JsonProperty("groupId")]
        public int? GroupId { get; set; }

        /// <summary>
        /// 因素Id
        /// </summary>
        [JsonProperty("factorId")]
        public int FactorId { get; set; }

        /// <summary>
        /// 数值数量
        /// </summary>
        [JsonProperty("valueNumber")]
        public int? ValueNumber { get; set; }

        /// <summary>
        /// 结构物名称
        /// </summary>
        [JsonProperty("structName")]
        public string StructName { get; set; }

        /// <summary>
        /// 设备类型Id
        /// </summary>
        [JsonProperty("productTypeId")]
        public string ProductTypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }

        /// <summary>
        /// 有无告警
        /// </summary>
        [JsonProperty("hasWarning")]
        public bool HasWarning { get; set; }

        /// <summary>
        /// 告警等级
        /// </summary>
        [JsonProperty("warningLevel")]
        public int WarningLevel { get; set; }

        /// <summary>
        ///界面类型
        /// </summary>
        [JsonProperty("DataTypeParam")]
        public decimal? DataTypeParam { get; set; }
    }

    /// <summary>
    /// 热点配置
    /// </summary>
    public class HotSpotConfig
    {
        /// <summary>
        /// 热点编号
        /// </summary>      
        [JsonProperty("hotspotId")]
        public int HotspotId { get; set; }

        /// <summary>
        /// 传感器编号
        /// </summary>
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        [JsonProperty("xAxis")]
        public decimal? XAxis { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        [JsonProperty("yAxis")]
        public decimal? YAxis { get; set; }

        /// <summary>
        /// 产品类型编号
        /// </summary>
        [JsonProperty("productTypeId")]
        public string ProductTypeId { get; set; }

        /// <summary>
        /// 产品类型名称
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// 位置描述
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// Path 文本。
        /// </summary>
        [JsonProperty("svgPath")]
        public string SvgPath { get; set; }
    }

    /// <summary>
    /// 施工截面及其热点配置
    /// </summary>
    public class SectionHotspotsConfig
    {
        /// <summary>
        /// 截面名称
        /// </summary>
        [JsonProperty("sectionName")]
        public string SectionName { get; set; }

        /// <summary>
        /// 截面工程状态
        /// </summary>
        [JsonProperty("sectionStatus")]
        public int? SectionStatus { get; set; }

        /// <summary>
        /// 截面热点图名称
        /// </summary>
        [JsonProperty("heapMapName")]
        public string HeapMapName { get; set; }

        [JsonProperty("hotspotsConfig")]
        public List<HotSpotConfig> HotSpotsConfig { get; set; }
    }

    /// <summary>
    /// 未配置热点
    /// </summary>
    public class HotSpotNonConfig
    {
        /// <summary>
        /// 传感器编号
        /// </summary>
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        /// <summary>
        /// 产品类型编号
        /// </summary>
        [JsonProperty("productTypeId")]
        public string ProductTypeId { get; set; }

        /// <summary>
        /// 产品类型名称
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// 位置描述
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }
    }

    /// <summary>
    /// 施工截面的传感器热点信息
    /// </summary>
    public class HotSpotOfSection
    {
        /// <summary>
        /// X坐标
        /// </summary>
        [JsonProperty("xAxis")]
        public decimal? XAxis { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        [JsonProperty("yAxis")]
        public decimal? YAxis { get; set; }

        /// <summary>
        /// 传感器Id
        /// </summary>
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        /// <summary>
        /// 施工截面名称
        /// </summary>
        [JsonProperty("sectionName")]
        public string SectionName { get; set; }

        /// <summary>
        /// 分组Id
        /// </summary>
        [JsonProperty("groupId")]
        public int? GroupId { get; set; }

        /// <summary>
        /// 因素Id
        /// </summary>
        [JsonProperty("factorId")]
        public int FactorId { get; set; }

        /// <summary>
        /// 数值数量
        /// </summary>
        [JsonProperty("valueNumber")]
        public int? ValueNumber { get; set; }

        /// <summary>
        /// 结构物名称
        /// </summary>
        [JsonProperty("structName")]
        public string StructName { get; set; }

        /// <summary>
        /// 设备类型Id
        /// </summary>
        [JsonProperty("productTypeId")]
        public string ProductTypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }

        /// <summary>
        /// 有无告警
        /// </summary>
        [JsonProperty("hasWarning")]
        public bool HasWarning { get; set; }

        /// <summary>
        /// 告警等级
        /// </summary>
        [JsonProperty("warningLevel")]
        public int WarningLevel { get; set; }

        /// <summary>
        /// 热点路径文本。
        /// </summary>
        [JsonProperty("svgPath")]
        public string SvgPath { get; set; }
    }
}
