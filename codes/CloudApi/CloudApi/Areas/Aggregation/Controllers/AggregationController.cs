using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Aggregation.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Service;

    using FS.Service;

    public class AggregationController : ApiController
    {
        // GET api/aggregation
        /// <summary>
        /// </summary>
        /// <param name="structId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物聚集条件配置", false)]
        public object GetStructureAggConfig(int structId)
        {
            try
            {
                using (var db = new SecureCloud_Entities())
                {
                    var result = from aggconfig in db.T_DIM_AGG_CONFIG
                                 from factor in db.T_DIM_SAFETY_FACTOR_TYPE
                                 from aggway in db.T_DIM_AGG_WAY
                                 from aggtype in db.T_DIM_AGG_TYPE
                                 where
                                     aggconfig.StructureId == structId
                                     && aggconfig.FacotrId == factor.SAFETY_FACTOR_TYPE_ID
                                     && aggconfig.AggTypeId == aggtype.Id && aggconfig.AggWayId == aggway.Id
                                     && aggconfig.IsDelete == false
                                 select
                                     new
                                         {
                                             id = aggconfig.Id,
                                             factorId = factor.SAFETY_FACTOR_TYPE_ID,
                                             factorName = factor.SAFETY_FACTOR_TYPE_NAME,
                                             aggtypeId = aggtype.Id,
                                             aggtypeName = aggtype.TypeName,
                                             aggwayId = aggway.Id,
                                             aggwayName = aggway.Name,
                                             enable = aggconfig.IsEnable,
                                             beginHour = aggconfig.DataBeginHour,
                                             endHour = aggconfig.DataEndHour,
                                             beginDate = aggconfig.DateBegin,
                                             endDate = aggconfig.DateEnd,
                                             timeMode = aggconfig.TimeMode
                                         };

                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 修改聚集条件
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("修改聚集条件", true)]
        public HttpResponseMessage UpdateStructureAggConfig([FromUri] int configId, [FromBody] AggConfigData model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var config =
                        (from aggconfig in db.T_DIM_AGG_CONFIG where aggconfig.Id == configId select aggconfig).First();
                    if (config == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("聚集配置不存在"));
                    }
                    if (!IsConfigChanged(config, model))
                    {
                        config.IsEnable = false;
                        config.IsDelete = true;
                        var newConfig = new T_DIM_AGG_CONFIG();
                        ToT_DIM_AGG_CONFIG(model, ref newConfig);
                        db.Entry(config).State = System.Data.EntityState.Modified;
                        db.Entry(newConfig).State = System.Data.EntityState.Added;
                        db.SaveChanges();
                        SendAggConfigChangedMsg();
                    }
          
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("聚集配置修改成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("聚集配置修改失败"));
                }
            }
        }

        /// <summary>
        /// 修改聚集条件
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("删除聚集条件", true)]
        public HttpResponseMessage DeleteStructureAggConfig([FromUri] int configId)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var config =
                        (from aggconfig in db.T_DIM_AGG_CONFIG where aggconfig.Id == configId select aggconfig).First();
                    if (config == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("聚集配置不存在"));
                    }
                   
                        config.IsEnable = false;
                        config.IsDelete = true;
                       
                        db.Entry(config).State = System.Data.EntityState.Modified;
                       
                        db.SaveChanges();
                        SendAggConfigChangedMsg();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("聚集配置修改成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("聚集配置修改失败"));
                }
            }
        }

        /// <summary>
        /// 新增聚集条件
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("新增聚集条件", true)]
        public HttpResponseMessage AddStructureAggConfig([FromBody] AggConfigData model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try   
                {
                   
                    var newConfig = new T_DIM_AGG_CONFIG();
                    ToT_DIM_AGG_CONFIG(model, ref newConfig);
                       
                    db.Entry(newConfig).State = System.Data.EntityState.Added;
                    db.SaveChanges();
                    SendAggConfigChangedMsg();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("聚集配置修改成功"));

                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("聚集配置修改失败"));
                }
            }
        }

        private bool IsConfigChanged(T_DIM_AGG_CONFIG oldConfig, AggConfigData newData)
        {
            return (oldConfig.AggTypeId == newData.AggTypeId && oldConfig.AggWayId == newData.AggWayId
                    && oldConfig.DataBeginHour == newData.BeginHour && oldConfig.DataEndHour == newData.EndHour
                    && oldConfig.DateBegin == newData.BeginDate && oldConfig.DateEnd == newData.EndDate
                    && oldConfig.FacotrId == newData.FacotrId && oldConfig.StructureId == newData.StructureId
                    && oldConfig.IsEnable == newData.IsEnable && oldConfig.TimeMode == newData.TimingMode);
        }

        private static void ToT_DIM_AGG_CONFIG(AggConfigData model, ref T_DIM_AGG_CONFIG config)
        {
            config.AggTypeId = model.AggTypeId;
            config.AggWayId = model.AggWayId;
            config.DataBeginHour = model.BeginHour;
            config.DataEndHour = model.EndHour;
            config.DateBegin = model.BeginDate;
            config.DateEnd = model.EndDate;
            config.FacotrId = model.FacotrId;
            config.StructureId = model.StructureId;
            config.IsEnable = model.IsEnable;
            config.TimeMode = model.TimingMode;
        }

        private static void SendAggConfigChangedMsg()
        {
            FsMessage msg = ConfigChangedMsgHelper.GetAggConfigChangedMsg();
            WebClientService.SendToAgg(msg);
        }
    }

    public class AggConfigData
    {
        public int StructureId { get; set; }

        public int FacotrId { get; set; }

        public int AggTypeId { get; set; }

        public int AggWayId { get; set; }

        public int BeginHour { get; set; }

        public int EndHour { get; set; }

        public int? BeginDate { get; set; }

        public int? EndDate { get; set; }

        public string TimingMode { get; set; }

        public bool IsEnable { get; set; }
    }
}
