using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config;
using FreeSun.FS_SMISCloud.Server.CloudApi.Service;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    using System.Configuration;
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SensorController : ApiController
    {
        private readonly string gps = ConfigurationManager.AppSettings["GPSBaseStation"];

        /// <summary>
        /// 获取传感器信息
        /// </summary>
        /// <param name="sensorId"> 传感器编号 </param>
        /// <returns> 传感器信息 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取指定结构物和监测因素下所有传感器信息", false)]
        public object GetSensorInfoByStruct(int structId, int factorId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var sql = from s in entity.T_DIM_SENSOR
                          from d in entity.T_DIM_REMOTE_DTU
                          from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                          from p in entity.T_DIM_SENSOR_PRODUCT
                          join fl in entity.T_DIM_FORMULAID on p.FORMAULAID equals fl.FormulaID into fl
                          from flm in fl.DefaultIfEmpty()
                          where
                              s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                              && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && s.DTU_ID == d.ID && s.IsDeleted == false && s.STRUCT_ID == structId && s.SAFETY_FACTOR_TYPE_ID == factorId
                          select
                              new
                              {
                                  sensorId = s.SENSOR_ID,
                                  productId = s.PRODUCT_SENSOR_ID,
                                  location = s.SENSOR_LOCATION_DESCRIPTION,

                              };

                var sensors = sql.ToList();
                List<JObject> items = new List<JObject>();
                foreach (var sensor in sensors)
                {
                    if (sensor != null)
                    {
                        // 查询已配置的公式参数
                        var para = entity.T_DIM_FORMULAID_SET.FirstOrDefault(p => p.SENSOR_ID==sensor.sensorId);
                        var paras = new List<Param2>();
                        if (para != null)
                        {
                            var pname = (from f in entity.T_DIM_FORMULA_PARA
                                         from fn in entity.T_DIM_FORMULA_PARA_NAME
                                         where f.ParaNameID == fn.ParaNameID
                                         select new { f.FormulaParaID, fn.ParaAlias }).ToList();
                            // 绑定参数
                            for (int i = 0; i < 100; i++)
                            {
                                var p1 = para.GetType().GetProperty("FormulaParaID" + (i + 1));
                                var p2 = para.GetType().GetProperty("Parameter" + (i + 1));

                                if (p1 == null || p2 == null || p1.GetValue(para, null) == null)
                                {
                                    break;
                                }

                                paras.Add(
                                    new Param2
                                    {
                                        Id = (int)p1.GetValue(para, null),
                                        Key =
                                            pname.Where(n => n.FormulaParaID == (int)p1.GetValue(para, null))
                                            .Select(s => s.ParaAlias)
                                            .FirstOrDefault(),
                                        Value = (double)((decimal?)p2.GetValue(para, null) ?? 0)
                                    });
                            }
                        }
                        else
                        {
                            // 未配置公式参数
                            var query = from p in entity.T_DIM_FORMULA_PARA
                                        from pn in entity.T_DIM_FORMULA_PARA_NAME
                                        from sp in entity.T_DIM_SENSOR_PRODUCT
                                        where
                                            sp.FORMAULAID == p.FormulaID && p.ParaNameID == pn.ParaNameID
                                            && sp.PRODUCT_ID == sensor.productId
                                        select new Param2 { Id = p.FormulaParaID, Key = pn.ParaAlias, Value = 0 };
                            paras = query.ToList();
                        }

                         items.Add(new JObject(
                            new JProperty("sensorId", sensor.sensorId),
                            new JProperty("location", sensor.location),
                            new JProperty(
                                "items",
                                paras.Select(
                                    p =>
                                    new JObject(
                                        new JProperty("id", p.Id),
                                        new JProperty("key", p.Key),
                                        new JProperty("value", p.Value))))));
                    }
                }

                return items;
            }
        }



        /// <summary>
        /// 获取结构物下的传感器列表
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 传感器列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下的所有传感器", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object FindSensorsByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                            from d in entity.T_DIM_REMOTE_DTU
                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entity.T_DIM_SENSOR_PRODUCT
                            where
                                s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && s.DTU_ID == d.ID
                                && s.STRUCT_ID == structId && s.IsDeleted == false
                                && p.PRODUCT_NAME != gps
                            select
                                new
                                    {
                                        sensorId = s.SENSOR_ID,
                                        factorName = f.SAFETY_FACTOR_TYPE_NAME,
                                        dtuId = d.ID,
                                        dtuNo = d.REMOTE_DTU_NUMBER,
                                        moduleNo = s.MODULE_NO,
                                        channel = s.DAI_CHANNEL_NUMBER,
                                        productId = s.PRODUCT_SENSOR_ID,
                                        sensorType = p.PRODUCT_NAME,
                                        sensorModel = p.PRODUCT_CODE,
                                        location = s.SENSOR_LOCATION_DESCRIPTION,
                                        identify = s.Identification,//标识
                                        enable=s.Enable//2-26
                                    };
                return query.ToList();
            }
        }

        /// <summary>
        /// 获取结构物下所有DTU及DTU下所有非虚拟(实体/数据)传感器列表 struct/{structId}/dtu-sensors/list/non-virtual
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> DTU及DTU下传感器列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下所有非虚拟(实体/数据)传感器列表", false)]
        public object FindDtuNonVirtualSensorsListByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var queryDtuSensors =
                    from sd in entity.T_DIM_STRUCT_DTU
                    join rd in entity.T_DIM_REMOTE_DTU on sd.DtuId equals rd.ID into nrd
                    from ird in nrd
                    join se in entity.T_DIM_SENSOR on ird.ID equals se.DTU_ID into nse
                    from ise in nse.DefaultIfEmpty()
                    join sft in entity.T_DIM_SAFETY_FACTOR_TYPE on ise.SAFETY_FACTOR_TYPE_ID equals sft.SAFETY_FACTOR_TYPE_ID into nsft
                    from isft in nsft.DefaultIfEmpty()
                    join sp in entity.T_DIM_SENSOR_PRODUCT on ise.PRODUCT_SENSOR_ID equals sp.PRODUCT_ID into nsp
                    from isp in nsp.DefaultIfEmpty()
                    where sd.StructureId == structId
                    select new
                    {
                        dtuId = (int?)ird.ID,
                        dtuNo = ird.REMOTE_DTU_NUMBER,
                        dtuIdentification = ird.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "传输" ? "传输型" : (ird.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "本地文件" ? "虚拟型" : "实体型"),
                        sensorId = (int?)ise.SENSOR_ID,
                        location = ise.SENSOR_LOCATION_DESCRIPTION,
                        sensorIdentification = (int?)ise.Identification, // 传感器标识 { 实体(0)/数据(1)/虚拟(2) }
                        sensorIsDeleted = (bool?)ise.IsDeleted,
                        moduleNo = ise.MODULE_NO,
                        channel = ise.DAI_CHANNEL_NUMBER,
                        productId = (int?)ise.PRODUCT_SENSOR_ID,
                        sensorType = isp.PRODUCT_NAME,
                        sensorModel = isp.PRODUCT_CODE,
                        factorName = isft.SAFETY_FACTOR_TYPE_NAME
                    };
                return
                    queryDtuSensors.GroupBy(g => new {g.dtuId, g.dtuNo, g.dtuIdentification}).Select(s => new
                    {
                        s.Key.dtuId,
                        s.Key.dtuNo,
                        s.Key.dtuIdentification,
                        sensors = s.Select(v => new
                        {
                            v.sensorId,
                            v.location,
                            v.sensorIdentification,
                            v.sensorIsDeleted,
                            v.moduleNo,
                            v.channel,
                            v.productId,
                            v.sensorType,
                            v.sensorModel,
                            v.factorName
                        }).Where(w => w.sensorIsDeleted == false && w.sensorIdentification != 2 && w.sensorType != gps)
                    }).ToList();
            }
        }

        /// <summary>
        /// 获取结构物下所有非虚拟(实体/数据)传感器列表 struct/{structId}/non-virtual/sensors
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 传感器列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下所有非虚拟(实体/数据)传感器列表", false)]
        public object FindNonVirtualSensorsByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                            from d in entity.T_DIM_REMOTE_DTU
                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entity.T_DIM_SENSOR_PRODUCT
                            where
                                s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && s.DTU_ID == d.ID
                                && s.STRUCT_ID == structId && s.IsDeleted == false && s.Identification != 2
                                && p.PRODUCT_NAME != gps
                            select
                                new
                                {
                                    sensorId = s.SENSOR_ID,
                                    factorName = f.SAFETY_FACTOR_TYPE_NAME,
                                    dtuId = d.ID,
                                    dtuNo = d.REMOTE_DTU_NUMBER,
                                    dtuIdentification = d.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "传输" ? "传输型" : (d.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "本地文件" ? "虚拟型" : "实体型"),
                                    moduleNo = s.MODULE_NO,
                                    channel = s.DAI_CHANNEL_NUMBER,
                                    productId = s.PRODUCT_SENSOR_ID,
                                    sensorType = p.PRODUCT_NAME,
                                    sensorModel = p.PRODUCT_CODE,
                                    location = s.SENSOR_LOCATION_DESCRIPTION,
                                    identify = s.Identification // 传感器标识 { 实体(0)/数据(1)/虚拟(2) }
                                };

                return query.ToList();
            }
        }

        /// <summary>
        /// 获取结构物下的传感器类型
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 传感器类型列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下的所有传感器类型", false)]
        public object FindSensorTypeByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                            from p in entity.T_DIM_SENSOR_PRODUCT
                            where
                                s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                && s.STRUCT_ID == structId && s.IsDeleted == false
                                && p.PRODUCT_NAME != gps
                            select
                                new
                                {
                                    sensorId = s.SENSOR_ID,
                                    sensorType = p.PRODUCT_NAME,
                                    location = s.SENSOR_LOCATION_DESCRIPTION
                                };
                return query.ToList().GroupBy(g => g.sensorType).Select(s => new {
                       sensorType = s.Key,
                       sensors = s.Select(v => new { v.sensorId, v.location }) 
                });
            }
        }

        /// <summary>
        /// 新增传感器
        /// </summary>
        /// <param name="model"> 传感器信息 </param>
        /// <returns> 新增结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("新增传感器", true)]
        [Authorization(AuthorizationCode.S_Structure_Sensor_Modify)]
        public HttpResponseMessage AddSensor([FromBody]Sensor model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var sensor = new T_DIM_SENSOR();
                    sensor.STRUCT_ID = model.StructId;
                    sensor.SAFETY_FACTOR_TYPE_ID = model.FactorId;
                    sensor.DTU_ID = model.DtuId;
                    sensor.MODULE_NO = model.ModuleNo;
                    sensor.DAI_CHANNEL_NUMBER = (byte?)model.Channel;
                    sensor.PRODUCT_SENSOR_ID = model.ProductId;
                    sensor.SENSOR_LOCATION_DESCRIPTION = model.Location;
                    sensor.IsDeleted = false;
                    sensor.Identification = model.Identify;
                    sensor.Enable = model.Enable;

                    var entry = db.Entry(sensor);
                    entry.State = System.Data.EntityState.Added;

                    var query = (from p in db.T_DIM_SENSOR_PRODUCT
                                 join fn in db.T_DIM_FORMULA_PARA on p.FORMAULAID equals fn.FormulaID into forluma
                                 from f in forluma.DefaultIfEmpty()
                                 join fname in db.T_DIM_FORMULA_PARA_NAME on f.ParaNameID equals fname.ParaNameID into
                                     name
                                 from n in name
                                 where p.PRODUCT_ID == model.ProductId
                                 orderby f.Order
                                 select f.FormulaParaID).ToList();

                    var para = (from q in query
                               from v in model.Params
                               where q == v.Id
                               select new { q, v.Value }).ToList();

                    var old = from o in db.T_DIM_FORMULAID_SET where o.SENSOR_ID == sensor.SENSOR_ID select o;
                    foreach (var o in old)
                    {
                        db.Entry(o).State = System.Data.EntityState.Deleted;
                    }

                    var newParam = new T_DIM_FORMULAID_SET();
                    newParam.SENSOR_ID = sensor.SENSOR_ID;
                    for (int i = 0; i < para.Count(); i++)
                    {
                        newParam.GetType().GetProperty("FormulaParaID" + (i + 1)).SetValue(newParam, para[i].q, null);
                        newParam.GetType().GetProperty("Parameter" + (i + 1)).SetValue(newParam, (decimal?)para[i].Value, null);
                    }

                    db.Entry(newParam).State = System.Data.EntityState.Added;

                    if (model.CorrentId != null)
                    {
                        var correntSensor = new T_DIM_SENSOR_CORRENT();
                        var array = model.CorrentId.Split(',');
                        for (int j = 0; j < array.Length; j++)
                        {
                            correntSensor.SensorId = sensor.SENSOR_ID;
                            var correntId = array.GetValue(j);
                            correntSensor.CorrentSensorId = Convert.ToInt32(correntId);
                            db.T_DIM_SENSOR_CORRENT.Add(correntSensor);
                            db.SaveChanges();
                        }

                    }


                    #region 日志信息

                    var fac =
                        db.T_DIM_SAFETY_FACTOR_TYPE.Where(f => f.SAFETY_FACTOR_TYPE_ID == model.FactorId)
                            .Select(f => f.SAFETY_FACTOR_TYPE_NAME)
                            .FirstOrDefault();

                    var dtu =
                        db.T_DIM_REMOTE_DTU.Where(d => d.ID == model.DtuId)
                            .Select(d => d.REMOTE_DTU_NUMBER)
                            .FirstOrDefault();

                    var pdt =
                        db.T_DIM_SENSOR_PRODUCT.Where(p => p.PRODUCT_ID == model.ProductId)
                            .Select(p => new { p.PRODUCT_NAME, p.PRODUCT_CODE })
                            .FirstOrDefault();

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] =
                        string.Format(
                            "位置：{0}，监测因素：{1}，dtu：{2}，模块号：{3},通道号：{4},设备类型：{5},参数：{6}",
                            string.IsNullOrEmpty(model.Location) ? string.Empty : model.Location,
                            fac ?? string.Empty,
                            dtu ?? string.Empty,
                            model.ModuleNo,
                            model.Channel,
                            pdt == null ? string.Empty : string.Format("{0}({1})", pdt.PRODUCT_NAME, pdt.PRODUCT_CODE),
                            model.Params == null ? string.Empty : string.Join(",", model.Params.Select(p => p.Value)));

                    #endregion

                    db.SaveChanges();
                    if (sensor.Identification != 2)
                    {
                        Entity.Config.Sensor sensorinfo = GetSensor(sensor);
                        var senopera = new SensorOperation
                        {
                            Action = Operations.Add,
                            Sensor = sensorinfo,
                            OldDtuId = sensorinfo.DtuID,
                            OldSensorId = sensorinfo.SensorID
                        };
                        WebClientService.SendToET(ConfigChangedMsgHelper.GetSensorConfigChangedMsg(senopera));
                    }
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("传感器新增成功"));
                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("传感器新增失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器新增失败"));
                }
            }
        }

        /// <summary>
        /// 获取传感器信息
        /// </summary>
        /// <param name="sensorId"> 传感器编号 </param>
        /// <returns> 传感器信息 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器信息", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object GetSensorInfo(int sensorId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var sql = from s in entity.T_DIM_SENSOR
                              from d in entity.T_DIM_REMOTE_DTU
                              from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                              from p in entity.T_DIM_SENSOR_PRODUCT
                              join fl in entity.T_DIM_FORMULAID on p.FORMAULAID equals fl.FormulaID into fl
                              from flm in fl.DefaultIfEmpty()
                              where
                                  s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                  && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID && s.DTU_ID == d.ID
                                  && s.SENSOR_ID == sensorId && s.IsDeleted == false
                              select
                                  new
                                      {
                                          sensorId = s.SENSOR_ID,
                                          factorId = f.SAFETY_FACTOR_TYPE_ID,
                                          factorName = f.SAFETY_FACTOR_TYPE_NAME,
                                          dtuId = d.ID,
                                          dtuNo = d.REMOTE_DTU_NUMBER,
                                          moduleNo = s.MODULE_NO,
                                          channel = s.DAI_CHANNEL_NUMBER,
                                          productId = s.PRODUCT_SENSOR_ID,
                                          sensorType = p.PRODUCT_NAME,
                                          sensorModel = p.PRODUCT_CODE,
                                          location = s.SENSOR_LOCATION_DESCRIPTION,
                                          formula = flm.FormulaExpression,
                                          description = f.DESCRIPTION,
                                          identify = s.Identification,//标识
                                          enable=s.Enable//2-26

                                      };

                var sensor = sql.FirstOrDefault();

                if (sensor != null)
                {
                    // 查询已配置的公式参数
                    var para = entity.T_DIM_FORMULAID_SET.FirstOrDefault(p => p.SENSOR_ID == sensorId);
                    var paras = new List<Param2>();
                    if (para != null)
                    {
                        var pname = (from f in entity.T_DIM_FORMULA_PARA
                                     from fn in entity.T_DIM_FORMULA_PARA_NAME
                                     where f.ParaNameID == fn.ParaNameID
                                     select new { f.FormulaParaID, fn.ParaAlias }).ToList();
                        // 绑定参数
                        for (int i = 0; i < 100; i++)
                        {
                            var p1 = para.GetType().GetProperty("FormulaParaID" + (i + 1));
                            var p2 = para.GetType().GetProperty("Parameter" + (i + 1));

                            if (p1 == null || p2 == null || p1.GetValue(para, null) == null)
                            {
                                break;
                            }

                            paras.Add(
                                new Param2
                                    {
                                        Id = (int)p1.GetValue(para, null),
                                        Key =
                                            pname.Where(n => n.FormulaParaID == (int)p1.GetValue(para, null))
                                            .Select(s => s.ParaAlias)
                                            .FirstOrDefault(),
                                        Value = (double)((decimal?)p2.GetValue(para, null) ?? 0)
                                    });
                        }
                    }
                    else 
                    {
                        // 未配置公式参数
                        var query = from p in entity.T_DIM_FORMULA_PARA
                                    from pn in entity.T_DIM_FORMULA_PARA_NAME
                                    from sp in entity.T_DIM_SENSOR_PRODUCT
                                    where
                                        sp.FORMAULAID == p.FormulaID && p.ParaNameID == pn.ParaNameID
                                        && sp.PRODUCT_ID == sensor.productId
                                    select new Param2 { Id = p.FormulaParaID, Key = pn.ParaAlias, Value = 0 };
                        paras = query.ToList();
                    }

                    return new JObject(
                        new JProperty("sensorId", sensor.sensorId),
                        new JProperty("factorId", sensor.factorId),
                        new JProperty(
                            "factorName",
                            sensor.factorName + (sensor.description != null
                                ? string.Format("({0})", sensor.description)
                                : "")),
                        new JProperty("dtuId", sensor.dtuId),
                        new JProperty("dtuNo", sensor.dtuNo),
                        new JProperty("moduleNo", sensor.moduleNo),
                        new JProperty("channel", sensor.channel),
                        new JProperty("productId", sensor.productId),
                        new JProperty("sensorType", sensor.sensorType),
                        new JProperty("sensorModel", sensor.sensorModel),
                        new JProperty("location", sensor.location),
                        new JProperty("formula", sensor.formula),
                         new JProperty("identify", sensor.identify),
                         new JProperty("enable",sensor.enable),
                        new JProperty(
                            "params",
                            paras.Select(
                                p =>
                                new JObject(
                                    new JProperty("id", p.Id),
                                    new JProperty("key", p.Key),
                                    new JProperty("value", p.Value)))));
                }

                return null;
            }
        }

        /// <summary>
        /// 获取关联传感器信息--许凤琴
        /// </summary>
        /// <param name="sensorId"> 传感器编号 </param>correntSensor/{sensorId}/info
        /// <returns>关联传感器信息 </returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object GetCorrentSensorInfo(int sensorId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var sql = from s in entity.T_DIM_SENSOR_CORRENT
                          from p in entity.T_DIM_SENSOR

                          where
                              s.SensorId == sensorId && p.SENSOR_ID == s.CorrentSensorId

                          select
                              new
                              {
                                  sensorId = p.SENSOR_ID,
                                  location = p.SENSOR_LOCATION_DESCRIPTION,
                                  correntId = s.CorrentSensorId,
                              };

                var list = sql.ToList();
                return list;
            }
        }



        /// <summary>
        /// 修改传感器信息
        /// </summary>
        /// <param name="sensorId"> 传感器编号  </param>
        /// <param name="model"> The sensor. </param>
        /// <returns> 修改结果  </returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改传感器信息", true)]
        [Authorization(AuthorizationCode.S_Structure_Sensor_Modify)]
        public HttpResponseMessage ModifySensor([FromUri]int sensorId, [FromBody]Sensor model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var sensor = db.T_DIM_SENSOR.FirstOrDefault(s => s.SENSOR_ID == sensorId && !s.IsDeleted);
                   if (sensor == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("传感器修改失败:传感器不存在或已删除"));
                    }
                     int dtuid = sensor.DTU_ID.Value;
                    StringBuilder sb = new StringBuilder();
                    var sensorLoc = sensor.SENSOR_LOCATION_DESCRIPTION;
                    sb.AppendFormat("原传感器:{0}:", sensorLoc);

                    if (model.FactorId != default(int) && model.FactorId != sensor.SAFETY_FACTOR_TYPE_ID)
                    {
                        sensor.SAFETY_FACTOR_TYPE_ID = model.FactorId;
                        var fac =
                        db.T_DIM_SAFETY_FACTOR_TYPE.Where(f => f.SAFETY_FACTOR_TYPE_ID == model.FactorId)
                            .Select(f => f.SAFETY_FACTOR_TYPE_NAME)
                            .FirstOrDefault();
                        sb.AppendFormat("监测因素改为:{0};", fac);
                    }
                    if (model.DtuId != default(int) && model.DtuId != sensor.DTU_ID)
                    {
                        sensor.DTU_ID = model.DtuId;
                        var dtu =
                        db.T_DIM_REMOTE_DTU.Where(d => d.ID == model.DtuId)
                            .Select(d => d.REMOTE_DTU_NUMBER)
                            .FirstOrDefault();
                        sb.AppendFormat("dtu号改为:{0};", dtu);
                    }
                    if (model.ModuleNo != default(int) && model.ModuleNo != sensor.MODULE_NO)
                    {
                        sensor.MODULE_NO = model.ModuleNo;
                        sb.AppendFormat("模块号改为:{0};", model.ModuleNo);
                    }
                    if (model.Channel != null && model.Channel != sensor.DAI_CHANNEL_NUMBER)
                    {
                        sensor.DAI_CHANNEL_NUMBER = (byte?)model.Channel;
                        sb.AppendFormat("通道改为:{0};", model.Channel);
                    }
                    //2-26
                    if (model.Enable != sensor.Enable)
                    {
                        sensor.Enable = model.Enable;
                        sb.AppendFormat("使能改为:{0};", model.Enable);
                    }
                    if (model.ProductId != default(int) && model.ProductId != sensor.PRODUCT_SENSOR_ID)
                    {
                        sensor.PRODUCT_SENSOR_ID = model.ProductId;
                        var pdt =
                        db.T_DIM_SENSOR_PRODUCT.Where(p => p.PRODUCT_ID == model.ProductId)
                            .Select(p => new { p.PRODUCT_NAME, p.PRODUCT_CODE })
                            .FirstOrDefault();
                        sb.AppendFormat("设备改为{0}({1});", pdt.PRODUCT_NAME, pdt.PRODUCT_CODE);
                    }
                    if (model.Location != default(string) && model.Location != sensor.SENSOR_LOCATION_DESCRIPTION)
                    {
                        sensor.SENSOR_LOCATION_DESCRIPTION = model.Location;
                        sb.AppendFormat("位置标识改为{0};", model.Location);
                    }
                    if (model.Params != null)
                    {
                        var query = (from p in db.T_DIM_SENSOR_PRODUCT
                                     join fn in db.T_DIM_FORMULA_PARA on p.FORMAULAID equals fn.FormulaID into forluma
                                     from f in forluma.DefaultIfEmpty()
                                     join fname in db.T_DIM_FORMULA_PARA_NAME on f.ParaNameID equals fname.ParaNameID
                                         into name
                                     from n in name
                                     where p.PRODUCT_ID == model.ProductId
                                     orderby f.Order
                                     select new {f.FormulaParaID, n.ParaAlias}).ToList();

                        var para =
                            (from q in query 
                             from v in model.Params 
                             where q.FormulaParaID == v.Id 
                             select new { q.FormulaParaID, v.Value }).ToList();

                        var paramStr = (from q in query
                                        from v in model.Params
                                        where q.FormulaParaID == v.Id
                                        select new { q.ParaAlias, v.Value }).ToList();

                        sb.AppendFormat(
                            "参数修改为:{0}",
                            string.Join(
                                "-",
                                paramStr.Select(p => string.Format("{0}:{1}", p.ParaAlias, p.Value)).ToArray()));

                        var old = from o in db.T_DIM_FORMULAID_SET where o.SENSOR_ID == sensor.SENSOR_ID select o;
                        foreach (var o in old)
                        {
                            db.Entry(o).State = System.Data.EntityState.Deleted;
                        }

                        var newParam = new T_DIM_FORMULAID_SET();
                        newParam.SENSOR_ID = sensor.SENSOR_ID;
                        for (int i = 0; i < para.Count(); i++)
                        {
                            newParam.GetType()
                                .GetProperty("FormulaParaID" + (i + 1))
                                .SetValue(newParam, para[i].FormulaParaID, null);
                            newParam.GetType()
                                .GetProperty("Parameter" + (i + 1))
                                .SetValue(newParam, (decimal?)para[i].Value, null);
                        }

                        db.Entry(newParam).State = System.Data.EntityState.Added;

                    }

                    //关联传感器
                    if (model.CorrentId != null)
                    {

                        var correntTable = from cp in db.T_DIM_SENSOR_CORRENT where cp.SensorId == sensorId select cp;

                        foreach (var o in correntTable)
                        {
                            db.T_DIM_SENSOR_CORRENT.Remove(o);

                        }
                        var array = model.CorrentId.Split(',');
                        for (int j = 0; j < array.Length; j++)
                        {
                            var correntSensor = new T_DIM_SENSOR_CORRENT();

                            correntSensor.SensorId = sensorId;
                            var correntId = array.GetValue(j);
                            correntSensor.CorrentSensorId = Convert.ToInt32(correntId);
                            db.T_DIM_SENSOR_CORRENT.Add(correntSensor);
                            db.SaveChanges();
                        }


                    }

                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = sb.ToString();

                    #endregion


                    db.SaveChanges();
                    if (sensor.Identification != 2)
                    {
                        Entity.Config.Sensor sensorinfo = GetSensor(sensor);
                        var senopera = new SensorOperation
                        {
                            Sensor = sensorinfo,
                            OldDtuId = (uint) dtuid,
                            OldSensorId = sensorinfo.SensorID,
                            Action = sensor.DTU_ID == dtuid ? Operations.Update : Operations.ChangedDtu
                        };
                        WebClientService.SendToET(ConfigChangedMsgHelper.GetSensorConfigChangedMsg(senopera));
                    }
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("传感器修改成功"));
                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("传感器修改失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器修改失败"));
                }
            }
        }

        /// <summary>
        /// 删除传感器
        /// </summary>
        /// <param name="sensorId"> 传感器编号 </param>
        /// <returns> 删除结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除传感器", true)]
        [Authorization(AuthorizationCode.S_Structure_Sensor_Modify)]
        public HttpResponseMessage RemoveSensor([FromUri] int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var sensor = db.T_DIM_SENSOR.FirstOrDefault(s => s.SENSOR_ID == sensorId && !s.IsDeleted);
                if (sensor == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器不存在"));
                }

                sensor.MODULE_NO *= -1;
                sensor.DAI_CHANNEL_NUMBER = (byte)(sensor.DAI_CHANNEL_NUMBER * -1);
                sensor.IsDeleted = true;

                //表T_DIM_SENSOR_CORRENT的同步变动
                IQueryable<T_DIM_SENSOR_CORRENT> Corrent = from p in db.T_DIM_SENSOR_CORRENT
                                                           where p.SensorId == sensorId || p.CorrentSensorId == sensorId//反向删除
                                                           select p;
                foreach (var CorrentConfig in Corrent)
                {
                    db.T_DIM_SENSOR_CORRENT.Remove(CorrentConfig);
                }

                #region 日志信息

                this.Request.Properties["ActionParameterShow"] = "传感器位置：" + sensor.SENSOR_LOCATION_DESCRIPTION;
                #endregion

                try
                {
                    if (sensor.Identification != 2)
                    {
                        var sensorinfo = new Entity.Config.Sensor
                        {
                            DtuID = sensor.DTU_ID == null ? 0 : (uint)sensor.DTU_ID,
                            SensorID = (uint)sensor.SENSOR_ID,
                            StructId = sensor.STRUCT_ID == null ? 0 : (uint)sensor.STRUCT_ID,
                            ModuleNo = sensor.MODULE_NO == null ? 0 : (uint)sensor.MODULE_NO,
                            ChannelNo = sensor.DAI_CHANNEL_NUMBER == null ? 0 : (uint)sensor.DAI_CHANNEL_NUMBER,
                            Name = sensor.SENSOR_LOCATION_DESCRIPTION,
                            UnEnable = sensor.Enable//3-3
                        };
                        var senopera = new SensorOperation
                        {
                            Action = Operations.Delete,
                            OldDtuId = sensorinfo.DtuID,
                            OldSensorId = sensorinfo.SensorID,
                            Sensor = sensorinfo
                        };
                        WebClientService.SendToET(ConfigChangedMsgHelper.GetSensorConfigChangedMsg(senopera));
                    }

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("传感器删除成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器删除失败"));
                }
            }
        }

        /// <summary>
        /// 获取sensor实例
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        private Entity.Config.Sensor GetSensor(T_DIM_SENSOR sensor)
        {
            using (var db = new SecureCloud_Entities())
            {
                if (sensor == null)
                    return null;
                
                var sensinfo = (from p in db.T_DIM_SENSOR_PRODUCT
                                from s in db.T_DIM_PROTOCOL_TYPE
                                from f in db.T_DIM_SAFETY_FACTOR_TYPE
                                where p.PROTOCOL_ID == s.PROTOCOL_ID && p.PRODUCT_ID == sensor.PRODUCT_SENSOR_ID && f.SAFETY_FACTOR_TYPE_ID == sensor.SAFETY_FACTOR_TYPE_ID
                                select new { p.PRODUCT_ID,p.PRODUCT_CODE, p.FORMAULAID, s.PROTOCOL_CODE, f.THEMES_TABLE_NAME, f.THEMES_COLUMNS })
                        .FirstOrDefault();
                string dtucode = (from d in db.T_DIM_REMOTE_DTU
                    where d.ID == sensor.DTU_ID
                    select d.REMOTE_DTU_NUMBER).FirstOrDefault();
                if (sensinfo == null)
                    return null;
                
                var sensorinfo = new Entity.Config.Sensor
                {
                    SensorID = (uint) sensor.SENSOR_ID,
                    StructId = sensor.STRUCT_ID == null ? 0 : (uint)sensor.STRUCT_ID,
                    DtuID = sensor.DTU_ID == null ? 0 : (uint)sensor.DTU_ID,
                    DtuCode = dtucode,
                    ModuleNo = sensor.MODULE_NO==null ? 0 : (uint)sensor.MODULE_NO,
                    ChannelNo = sensor.DAI_CHANNEL_NUMBER == null ? 0 : (uint)sensor.DAI_CHANNEL_NUMBER,
                    Name = sensor.SENSOR_LOCATION_DESCRIPTION,
                    ProtocolType = Convert.ToUInt32(sensinfo.PROTOCOL_CODE),
                    FormulaID = sensinfo.FORMAULAID == null ? 0 : (uint)sensinfo.FORMAULAID,
                    FactorType = sensor.SAFETY_FACTOR_TYPE_ID == null ? 0 : (uint)sensor.SAFETY_FACTOR_TYPE_ID,
                    FactorTypeTable = sensinfo.THEMES_TABLE_NAME,
                    TableColums = sensinfo.THEMES_COLUMNS,
                    ProductCode = sensinfo.PRODUCT_CODE,
                    ProductId = sensinfo.PRODUCT_ID,
                    SensorType = (SensorType)sensor.Identification,
                    //TODO 3-3
                    UnEnable = sensor.Enable
                };
                if (sensinfo.FORMAULAID != null)
                {
                    var pname = (from pf in db.T_DIM_FORMULA_PARA
                                 from pn in db.T_DIM_FORMULA_PARA_NAME
                                 where pf.ParaNameID == pn.ParaNameID && pf.FormulaID == sensinfo.FORMAULAID
                                 orderby pf.Order
                                 select new { pf.FormulaID, pf.FormulaParaID, pn.ParaName, pn.ParaAlias, pf.Order }
                    ).ToList();
                    //sensorinfo.ParamCount = (ushort)pname.Count;
                    T_DIM_FORMULAID_SET param0 = (from pf in db.T_DIM_FORMULAID_SET
                        where pf.SENSOR_ID == sensor.SENSOR_ID
                        select pf).ToList().FirstOrDefault();
                    if (param0 == null) return sensorinfo;
                    for (int i = 0; i < pname.Count; i++)
                    {
                        var f = new FormulaParam
                        {
                            FID = (int) pname[i].FormulaID,
                            PID = pname[i].FormulaParaID,
                            Index = (int) pname[i].Order,
                            Name = pname[i].ParaName,
                            Alias = pname[i].ParaAlias
                        };
                        var sp = new SensorParam(f)
                        {
                            Value =
                                Convert.ToDouble(param0.GetType()
                                    .GetProperty("Parameter" + (i + 1))
                                    .GetValue(param0, null))
                        };
                        sensorinfo.AddParameter(sp);
                    }
                }
                //else
                //{
                //    sensorinfo.ParamCount = 0;
                //}
                
                return sensorinfo;
            }
        }
    }

    public class Sensor
    {
        public int StructId { get; set; }

        public int FactorId { get; set; }

        public int DtuId { get; set; }

        public int ModuleNo { get; set; }

        public int Channel { get; set; }

        public int ProductId { get; set; }

        public string Location { get; set; }

        public IList<Param> Params { get; set; }

        public string CorrentId { get; set; }

        public int Identify { get; set; }

        public bool Enable { get; set; }
    }

    public class Param
    {
        public int Id { get; set; }

        public double Value { get; set; }
    }

    public class Param2
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public double Value { get; set; }
    }
}
