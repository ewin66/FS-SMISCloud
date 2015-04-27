using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Diagnostics;
using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Factor.Controllers
{
    
    public class CorrelationController : ApiController
    {
        private readonly string gps = ConfigurationManager.AppSettings["GPSBaseStation"];
        ///<summary>
        ///根据结构物和因素获取关联的监测因素 struct/{structId}/factor/{factorId}/correlation 
        ///</summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factorId"> 因素编号 </param>
        /// <returns> 关联因素列表 <see cref="object"/>
        /// </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物监测因素的关联监测因素", false)]
        [Authorization(AuthorizationCode.S_DataCorrelation)]
        [Authorization(AuthorizationCode.U_Common)]
        public object CorrelationFactors(int structId, int factorId)
        {
            string baseStation = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (SecureCloud_Entities entities = new SecureCloud_Entities())
            {
                var query = from s in entities.T_DIM_STRUCTURE_FACTOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entities.T_DIM_CORRELATION_FACTOR
                            where s. SAFETY_FACTOR_TYPE_ID== p.Factor_Id
                                  && s.STRUCTURE_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                  && p.FactorId == factorId
                                 
                            select new
                            {
                                factorid = p.Factor_Id,
                                factorname = f.SAFETY_FACTOR_TYPE_NAME,
                                valuenumber = f.FACTOR_VALUE_COLUMN_NUMBER
                            };
                var list = query.ToList().OrderBy(l => l.factorid);//排序
                return list;                                                                                        
            }
        }

        /// <summary>
        /// 获取监测因素下传感器产品列表factor/{factorId}/correlate-product-type/info
        /// </summary>
        /// <returns> 传感器产品列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取监测因素下传感器产品列表", false)]
        public object GetFactorCorrelateProduct(int factorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var productList = from pl in db.T_DIM_FACTOR_PRODUCT_TYPE
                                  where pl.FACTOR_ID == factorId
                                  select new
                                      {
                                          productType = pl.PRODUCT_TYPE_ID
                                      };
                var productLength = productList.ToArray().Length;
                if (productLength > 0)
                {
                    var query1 = from q in db.T_DIM_SENSOR_PRODUCT
                                 from a1 in productList
                                 where q.PRODUCT_TYPE_ID.Equals(a1.productType)
                                 select q;
                    var query =
                        query1.GroupBy(p => p.PRODUCT_NAME)
                              .Select(
                                  g =>
                                  new
                                      {
                                          productName = g.Key,
                                          productTypeId=g.Select(n=>new {product_TypeId=n.PRODUCT_TYPE_ID}),
                                          models =
                                      g.Select(m => new {productId = m.PRODUCT_ID, productCode = m.PRODUCT_CODE})
                                      });
                    return query.ToList();
                }
                else
                {
                    return null;
                }

            }
        }


        /// <summary>
        /// 获取组合传感器的关联传感器列表combined-sensor/{structId}/{productTypeId}/sensorList/info
        /// </summary>
        /// <returns> 传感器列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取组合传感器的关联传感器列表", false)]
        public object GetCombinedSensorList(int structId, string productTypeId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                            from p in entity.T_DIM_SENSOR_PRODUCT
                            where
                                s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                && s.STRUCT_ID == structId && s.IsDeleted == false
                                && p.PRODUCT_TYPE_ID == productTypeId && p.PRODUCT_NAME != gps
                            select
                                new
                                    {
                                        sensorId = s.SENSOR_ID,
                                        productId = s.PRODUCT_SENSOR_ID,
                                        sensorType = p.PRODUCT_NAME,
                                        location = s.SENSOR_LOCATION_DESCRIPTION,
                                        identify = s.Identification //标识
                                    };
                return query.ToList();
            }
        }

        /// <summary>
        /// 获取监测因素下可配置的单位列表factor/{structId}/unitList/info
        /// </summary>
        /// <returns> 单位列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取监测因素下可配置的单位列表", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public List<FactorUnitInfo> GetFactorUnitList(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var factorList = from sf in entity.T_DIM_STRUCTURE_FACTOR
                                 from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                                
                                 where
                                     sf.STRUCTURE_ID == structId
                                     && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                 select new
                                     {
                                         Id = sf.SAFETY_FACTOR_TYPE_ID,
                                         NameCN = f.SAFETY_FACTOR_TYPE_NAME,
                                         Display = f.FACTOR_VALUE_COLUMNS,
                                         DisplayNumber = f.FACTOR_VALUE_COLUMN_NUMBER ?? 1

                                     };
                var factUnit = from fl in factorList
                               from fu in entity.T_DIM_FACTOR_UNIT_INT
                               where fl.Id == fu.FACTOR_ID
                               select new
                                   {
                                       factId = fl.Id,
                                       valueIndex = fu.VALUE_INDEX,
                                       Unit = fu.UNIT,
                                   };

                var queryList = new List<FactorUnitInfo>();
                foreach (var item in factorList)
                {
                    var subFactor = item.Display.Split(',');
                    var factorId = item.Id;
                    for (int i = 0; i < item.DisplayNumber; i++)
                    {
                        foreach (var s in factUnit)
                        {
                            var info = new FactorUnitInfo();
                            var innerFactor = s.factId;
                            var index = s.valueIndex;
                            var a = i + 1;
                            if (factorId == innerFactor && index == a)
                            {
                                info.FactorId = factorId;
                                info.FactorName = item.NameCN;
                                info.ItemId = index;
                                info.ItemName = subFactor[index - 1];
                                info.Unit = GetUnitList(innerFactor, index);
                                queryList.Add(info);
                                break;
                            }
                        }
                       
                    }
                   
                }
                return queryList;
            }
        }

        private object GetUnitList(int factorId,int index)
        {
            using (var entity = new SecureCloud_Entities())
            {

                var factUnit = from fu in entity.T_DIM_FACTOR_UNIT_INT
                               where  fu.FACTOR_ID==factorId &&  fu.VALUE_INDEX==index 
                               select new
                               {
                                   Unit = fu.UNIT,
                                   id=fu.ID
                               };

                return factUnit.ToList();


            }
        }

        /// <summary>
        /// 配置监测因素的展示单位factor/{structId}/unit/add
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("配置监测因素的展示单位", true)]
        [Authorization(AuthorizationCode.S_Structure_FactorUnit)]
        public HttpResponseMessage AddFactorUnit([FromUri] int structId, [FromBody] allData configs)
        {
            using (var db = new SecureCloud_Entities())
            {
                var configAll = configs.Date;
                foreach (var config in configAll)
                {
                    if (config.Unit == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                                                      StringHelper.GetMessageString("添加配置信息失败，单位不能为空"));
                    }

                    //修改
                    try
                    {
                        var subId = from sb in db.T_DIM_FACTOR_UNIT_INT
                                    
                                    where sb.FACTOR_ID == config.FactorId && sb.VALUE_INDEX == config.ItemId
                                          && sb.UNIT == config.Unit
                                    select new
                                    {
                                        id = sb.ID
                                    };
                        var idex = (config.FactorId + config.ItemId).ToString();
                        var unitInfo =
                            db.T_DIM_STRUCT_FACTOR_UNIT.FirstOrDefault(
                                m =>
                                m.STRUCT_ID == structId && m.FACTOR_ID == config.FactorId && m.VALUE_ID == config.ItemId);
                        if (unitInfo != null)
                        {
                            foreach (var s in subId)
                            {
                                unitInfo.SUB_FACTOR_ID = s.id;

                            }
                            unitInfo.STRUCT_ID = config.StructId;
                            unitInfo.FACTOR_ID = config.FactorId;
                            unitInfo.VALUE_ID = config.ItemId;
                            var unitUp = db.Entry(unitInfo);
                            unitUp.State = EntityState.Modified;
                            db.SaveChanges();

                        }
                        else
                        {
                            //新增配置单位
                            var unitConfig = new T_DIM_STRUCT_FACTOR_UNIT();
                            foreach (var s in subId)
                            {
                                unitConfig.SUB_FACTOR_ID = s.id;
                            }
                            unitConfig.STRUCT_ID = config.StructId;
                            unitConfig.FACTOR_ID = config.FactorId;
                            unitConfig.VALUE_ID = config.ItemId;

                            var entry = db.Entry(unitConfig);
                            entry.State = EntityState.Added;
                            db.SaveChanges();
                        }
                    }

                    catch
                        (Exception ex)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("配置失败"));
                    }

                    //#region 日志信息

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);

                    this.Request.Properties["ActionParameterShow"] = string.Format(
                        "监测因素编号：{0}，监测子因素编号：{1}，单位：{2}，结构物编号：{3}",
                        config.FactorId,
                        config.ItemId,
                        config.Unit,
                        config.StructId
                        );

                    //#endregion
                }
            }
            
            return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("配置成功"));
        }


        /// <summary>
        /// 获取单个监测因素已配置的单位factor/{structId}/{factorId}/{valueIndex}/unit/info
        /// </summary>
        /// <returns> 单位 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取单个监测因素已配置的单位", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object GetSubFactorUnit(int structId, int factorId, int valueIndex)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from sfu in db.T_DIM_STRUCT_FACTOR_UNIT
                            where
                                sfu.STRUCT_ID == structId && sfu.FACTOR_ID == factorId && sfu.VALUE_ID == valueIndex
                            select new
                                {
                                    id = sfu.SUB_FACTOR_ID
                                };
                return query.ToList();
            }
        }


        /// <summary>
        /// 获取单个结构物已配置的单位个数factor/{structId}/unit/count
        /// </summary>
        /// <returns> 单位 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取单个结构物已配置的单位个数", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object GetUnitCount(int structId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from sfu in db.T_DIM_STRUCT_FACTOR_UNIT
                            from g in db.T_DIM_STRUCTURE_FACTOR
                            where
                                sfu.STRUCT_ID == structId&&g.STRUCTURE_ID==structId
                                &&g.SAFETY_FACTOR_TYPE_ID==sfu.FACTOR_ID
                            select sfu;

                return query.ToList();
            }
        }

    }

    public class allData
    {
        public ConfigInfo[] Date { get; set; }
    }


    public class ConfigInfo
    {
        public int FactorId { get; set; }
        public int ItemId { get; set; }
        public int StructId { get; set; }
        public string Unit { get; set; }
    }

    public class FactorUnitInfo
    {
        [JsonProperty("factorId")]
        public int FactorId { get; set; }

        [JsonProperty("factorName")]
        public string FactorName { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("unit")]
        public object Unit { get; set; }
    }
    
}
