using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json.Linq;

    public class SensorProductController : ApiController
    {
        /// <summary>
        /// 获取传感器产品列表
        /// </summary>
        /// <returns> 传感器产品列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取所有传感器产品", false)]
        public object GetSensorProductList()
        {
            using (var db = new SecureCloud_Entities())
            {
                var query =
                    db.T_DIM_SENSOR_PRODUCT.GroupBy(p => p.PRODUCT_NAME)
                        .Select(
                            g =>
                            new
                                {
                                    productName = g.Key,
                                    models = g.Select(m => new { productId = m.PRODUCT_ID, productCode = m.PRODUCT_CODE })
                                });
                return query.ToList();
            }
        }

        /// <summary>
        /// 获取传感器产品信息
        /// </summary>
        /// <param name="productId"> 传感器产品编号 </param>
        /// <returns> 产品信息对象 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器产品信息", false)]
        public object GetSensorProductInfo(int productId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from p in db.T_DIM_SENSOR_PRODUCT
                            join pro in db.T_DIM_PROTOCOL_TYPE on p.PROTOCOL_ID equals pro.PROTOCOL_ID into protocol
                            from ptc in protocol.DefaultIfEmpty()
                            join fp in db.T_DIM_FORMULA_PARA on p.FORMAULAID equals fp.FormulaID into formula
                            from f in formula.DefaultIfEmpty()
                            join fn in db.T_DIM_FORMULA_PARA_NAME on f.ParaNameID equals fn.ParaNameID into formulaname
                            from fname in formulaname.DefaultIfEmpty()
                            where p.PRODUCT_ID == productId
                            orderby f.Order
                            select
                                new
                                    {
                                        protocolCode = ptc.PROTOCOL_CODE,
                                        protocolName = ptc.PROTOCOL_TYPE,
                                        protocolDesc = ptc.DESCRIPTION,
                                        formula = f.T_DIM_FORMULAID.FormulaExpression,
                                        id = (int?)f.FormulaParaID,
                                        key = fname.ParaAlias
                                    };
                return
                    query.ToList()
                        .GroupBy(p => new { p.protocolCode, p.protocolName, p.protocolDesc, p.formula })
                        .Select(
                            g =>
                            new JObject(
                                new JProperty("protocolCode", g.Key.protocolCode),
                                new JProperty("protocolName", g.Key.protocolName),
                                new JProperty("protocolDesc", g.Key.protocolDesc),
                                new JProperty("formula", g.Key.formula),
                                new JProperty(
                                "params",
                                g.Select(pa => new JObject(new JProperty("id", pa.id), new JProperty("key", pa.key))))))
                        .FirstOrDefault();
            }
        }
    }
}
