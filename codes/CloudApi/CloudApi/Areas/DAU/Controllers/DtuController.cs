using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config;
using FreeSun.FS_SMISCloud.Server.CloudApi.Service;
using FS.Service;
using log4net;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.DAU.Controllers
{
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log; 
    using FreeSun.FS_SMISCloud.Server.CloudApi.Service.DtuService;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class DtuController : ApiController
    {
        /// <summary>
        /// Get org/{orgId}/struct/{structId}/dtu
        /// </summary>
        /// <param name="structId">结构物id</param>
        /// <returns>dtu列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取DTU列表", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object GetDtu(int structId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from dtu in entity.T_DIM_REMOTE_DTU
                            from sd in entity.T_DIM_STRUCT_DTU
                            join dp in entity.T_DIM_DTU_PRODUCT on dtu.ProductDtuId equals dp.ProductId into p
                            from pd in p.DefaultIfEmpty()
                            where dtu.ID == sd.DtuId
                                && sd.StructureId == structId
                                && dtu.REMOTE_DTU_STATE == true
                            select
                                new
                                {
                                    dtuId = dtu.ID,
                                    dtuNo = dtu.REMOTE_DTU_NUMBER,
                                    granularity = dtu.REMOTE_DTU_GRANULARITY,
                                    networkType = pd.NetworkType,
                                    sim = dtu.REMOTE_DTU_SUBSCRIBER,
                                    ip = dtu.DTU_IP.Trim(),
                                    port = dtu.DTU_PORT,
                                    p1 = dtu.P1,
                                    p2 = dtu.P2,
                                    p3 = dtu.P3,
                                    p4 = dtu.P4
                                };
                return query.ToList();
            }
        }

        [AcceptVerbs("Get")]
        [LogInfo("获取DTU配置信息", false)]
        public object GetDtuInfo(int dtuId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from dtu in entity.T_DIM_REMOTE_DTU
                            join dp in entity.T_DIM_DTU_PRODUCT on dtu.ProductDtuId equals dp.ProductId into p
                            from pd in p.DefaultIfEmpty()
                            where dtu.ID == dtuId && dtu.REMOTE_DTU_STATE == true
                            select
                                new
                                    {
                                        dtuId = dtu.ID,
                                        dtuNo = dtu.REMOTE_DTU_NUMBER,
                                        sim = dtu.REMOTE_DTU_SUBSCRIBER,
                                        granularity = dtu.REMOTE_DTU_GRANULARITY,
                                        ip = dtu.DTU_IP.Trim(),
                                        port = dtu.DTU_PORT,
                                        dtuFactory = pd.DtuFactory,
                                        dtuModel = pd.DtuModel,
                                        networkType = pd.NetworkType,
                                        p1 = dtu.P1,
                                        p2 = dtu.P2,
                                        p3 = dtu.P3,
                                        p4 = dtu.P4
                                    };
                return query.FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据结构物获取同一个组织下的dtu
        /// </summary>
        /// <param name="structId">结构物id</param>
        /// <returns>dtu列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物同一组织下的DTU", false)]
        public object GetOrgDtu(int structId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from dtu in entity.T_DIM_REMOTE_DTU
                            from sd in entity.T_DIM_STRUCT_DTU
                            from os in entity.T_DIM_ORG_STUCTURE
                            from os2 in entity.T_DIM_ORG_STUCTURE
                            where os.STRUCTURE_ID == structId
                                && os.ORGANIZATION_ID == os2.ORGANIZATION_ID
                                && os2.STRUCTURE_ID == sd.StructureId
                                && sd.DtuId == dtu.ID
                                && dtu.REMOTE_DTU_STATE == true
                            select
                                new
                                {
                                    dtuId = dtu.ID,
                                    dtuNo = dtu.REMOTE_DTU_NUMBER
                                };
                return query.Distinct().ToList();
            }
        }

        /// <summary>
        /// 添加映射
        /// </summary>
        /// <param name="dtuId"></param>
        /// <param name="structId"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("给结构物添加现有DTU", true)]
        [Authorization(AuthorizationCode.S_Structure_DTU_Add)]
        public HttpResponseMessage AddMap(int dtuId, int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                if (entity.T_DIM_STRUCT_DTU.Any(d => d.DtuId == dtuId && d.StructureId == structId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("添加DTU失败"));
                }

                // 添加关联关系
                var sdEntity = new T_DIM_STRUCT_DTU();
                sdEntity.DtuId = dtuId;
                sdEntity.StructureId = structId;

                var entry2 = entity.Entry(sdEntity);
                entry2.State = System.Data.EntityState.Added;

                #region 日志信息

                var stc = entity.T_DIM_STRUCTURE.FirstOrDefault(s => s.ID == structId);
                var dtu = entity.T_DIM_REMOTE_DTU.FirstOrDefault(d => d.ID == dtuId);
                this.Request.Properties["ActionParameterShow"] = string.Format(
                    "dtu号：{0},结构物：{1}",
                    dtu == null ? string.Empty : dtu.REMOTE_DTU_NUMBER,
                    stc == null ? string.Empty : stc.STRUCTURE_NAME_CN);
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("添加DTU成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("添加DTU失败"));
                }
            }
        }

        /// <summary>
        /// POST dtu/add
        /// </summary>
        /// <returns>添加结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("添加DTU", true)]
        [Authorization(AuthorizationCode.S_Structure_DTU_Add)]
        public HttpResponseMessage Add([FromBody]DtuModel dtu)
        {
            using (var entity = new SecureCloud_Entities())
            {
                // 新增dtu
                var dtuEntity = new T_DIM_REMOTE_DTU();
                dtuEntity.REMOTE_DTU_NUMBER = dtu.DtuNo;
                dtuEntity.REMOTE_DTU_SUBSCRIBER = dtu.Sim;
                dtuEntity.REMOTE_DTU_GRANULARITY = (short)dtu.Granularity;
                dtuEntity.DTU_IP = dtu.Ip;
                dtuEntity.DTU_PORT = dtu.Port;
                dtuEntity.P1 = dtu.P1;
                dtuEntity.P2 = dtu.P2;
                dtuEntity.P3 = dtu.P3;
                dtuEntity.P4 = dtu.P4;
                dtuEntity.ProductDtuId = dtu.ProductId;
                dtuEntity.REMOTE_DTU_STATE = true;

                var entry = entity.Entry(dtuEntity);
                entry.State = System.Data.EntityState.Added;

                // 添加关联关系
                var sdEntity = new T_DIM_STRUCT_DTU();
                sdEntity.DtuId = dtuEntity.ID;
                sdEntity.StructureId = dtu.StructId;

                var entry2 = entity.Entry(sdEntity);
                entry2.State = System.Data.EntityState.Added;

                #region 日志信息

                var stc = entity.T_DIM_STRUCTURE.FirstOrDefault(s => s.ID == dtu.StructId);
                var product = entity.T_DIM_DTU_PRODUCT.FirstOrDefault(p => p.ProductId == dtu.ProductId);
                var paramName1 = (product != null && product.NetworkType.ToLower().Contains("local")) ? "文件路径" : "参数1";
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(dtu);
                this.Request.Properties["ActionParameterShow"] =
                    string.Format(
                        "dtu号：{0},结构物：{1},采集粒度：{3},产品厂商:{6},产品型号:{7},sim卡号:{2},ip地址：{4},端口：{5},[{12}:{8},参数2:{9},参数3:{10},参数4:{11}]",
                        dtu.DtuNo ?? string.Empty,
                        stc == null ? string.Empty : stc.STRUCTURE_NAME_CN,
                        dtu.Sim ?? string.Empty,
                        dtu.Granularity,
                        dtu.Ip,
                        dtu.Port,
                        product != null ? product.DtuFactory : string.Empty,
                        product != null ? product.DtuModel : string.Empty,
                        dtu.P1,
                        dtu.P2,
                        dtu.P3,
                        dtu.P4,
                        paramName1);
                #endregion

                try
                {
                    entity.SaveChanges();
                    var dtnod =new DtuNode
                    {
                        DtuId = (uint)dtuEntity.ID,
                        Type = DtuType.Gprs,
                        DtuCode = dtuEntity.REMOTE_DTU_NUMBER,
                        Name = dtuEntity.DESCRIPTION,
                        DacInterval = dtuEntity.REMOTE_DTU_GRANULARITY == null ? DtuNode.DefaultDacInterval : (uint)(dtuEntity.REMOTE_DTU_GRANULARITY * 60),
                        NetworkType = dtuEntity.ProductDtuId == 2 ? NetworkType.hclocal : NetworkType.gprs
                    };
                    if (dtnod.NetworkType == NetworkType.hclocal)
                        dtnod.AddProperty("param1", dtuEntity.P1);
                    WebClientService.SendToET(ConfigChangedMsgHelper.GetDtuConfigChangedMsg(ChangedStatus.Add, dtnod));
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("添加DTU成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("添加DTU失败"));
                }
            }
        }

        /// <summary>
        /// POST dtu/modify
        /// </summary>
        /// <returns>修改结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改DTU信息", true)]
        [Authorization(AuthorizationCode.S_Structure_DTU_Modify)]
        public HttpResponseMessage Modify([FromUri]int dtuId, [FromBody]DtuModel dtu)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var dtuEntity = entity.T_DIM_REMOTE_DTU.FirstOrDefault(d => d.ID == dtuId);
                if (dtuEntity == null || dtuEntity.REMOTE_DTU_STATE == false)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("DTU不存在或已禁用"));
                }

                StringBuilder sb = new StringBuilder();
                string dtucode = dtuEntity.REMOTE_DTU_NUMBER;
                if (dtu.DtuNo != default(string) && dtu.DtuNo != dtuEntity.REMOTE_DTU_NUMBER)
                {
                    dtuEntity.REMOTE_DTU_NUMBER = dtu.DtuNo;
                    sb.AppendFormat("Dtu编号改为:{0},", dtu.DtuNo);
                }                
                if (dtu.Granularity != default(int) && dtu.Granularity != dtuEntity.REMOTE_DTU_GRANULARITY)
                {
                    dtuEntity.REMOTE_DTU_GRANULARITY = (short)dtu.Granularity;
                    sb.AppendFormat("采集间隔改为:{0},", dtu.Granularity);
                }
                if (dtu.Sim != dtuEntity.REMOTE_DTU_SUBSCRIBER)
                {
                    dtuEntity.REMOTE_DTU_SUBSCRIBER = dtu.Sim;
                    sb.AppendFormat("sim卡号改为:{0},", dtu.Sim);
                }
                if (dtu.Ip != dtuEntity.DTU_IP)
                {
                    dtuEntity.DTU_IP = dtu.Ip;
                    sb.AppendFormat("Ip改为:{0},", dtu.Ip);
                }
                if (dtu.Port != dtuEntity.DTU_PORT)
                {
                    dtuEntity.DTU_PORT = dtu.Port;
                    sb.AppendFormat("端口改为:{0},", dtu.Port);
                }
                if (dtu.ProductId != default(int) && dtu.ProductId != dtuEntity.ProductDtuId)
                {
                    dtuEntity.ProductDtuId = dtu.ProductId;
                    var product = entity.T_DIM_DTU_PRODUCT.FirstOrDefault(p => p.ProductId == dtu.ProductId);
                    if (product != null)
                    {
                        sb.AppendFormat("产品厂商改为{0},型号改为:{1}", product.DtuFactory, product.DtuModel);
                    }
                    else
                    {
                        sb.AppendFormat("产品id改为{0}", dtu.ProductId);
                    }
                }
                if (dtu.P1 != dtuEntity.P1)
                {
                    dtuEntity.P1 = dtu.P1;
                    sb.AppendFormat("参数1改为:{0}", dtu.P1);
                }
                if (dtu.P2 != dtuEntity.P2)
                {
                    dtuEntity.P2 = dtu.P2;
                    sb.AppendFormat("参数2改为:{0}", dtu.P1);
                } 
                if (dtu.P3 != dtuEntity.P3)
                {
                    dtuEntity.P3 = dtu.P3;
                    sb.AppendFormat("参数3改为:{0}", dtu.P1);
                } 
                if (dtu.P4 != dtuEntity.P4)
                {
                    dtuEntity.P4 = dtu.P4;
                    sb.AppendFormat("参数4改为:{0}", dtu.P1);
                }

                #region 日志信息
                
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(dtu);
                this.Request.Properties["ActionParameterShow"] = string.Format("dtu号：{0}:{1}", dtu.DtuNo ?? string.Empty, sb);
                #endregion

                try
                {
                    entity.SaveChanges();
                    var dtnod = new DtuNode
                    {
                        DtuId = (uint) dtuEntity.ID,
                        Type = DtuType.Gprs,
                        DtuCode = dtuEntity.REMOTE_DTU_NUMBER,
                        NetworkType = dtuEntity.ProductDtuId == 2 ? NetworkType.hclocal : NetworkType.gprs,
                        Name = dtuEntity.DESCRIPTION,
                        DacInterval = dtuEntity.REMOTE_DTU_GRANULARITY == null ? DtuNode.DefaultDacInterval : (uint) dtuEntity.REMOTE_DTU_GRANULARITY
                    };
                    if (dtnod.NetworkType == NetworkType.hclocal)
                        dtnod.AddProperty("param1", dtuEntity.P1);
                    WebClientService.SendToET(ConfigChangedMsgHelper.GetDtuConfigChangedMsg(ChangedStatus.Modify, dtnod, dtucode));
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("DTU信息修改成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("DTU信息修改失败"));
                }
            }
        }

        /// <summary>
        /// POST dtu/remove
        /// </summary>
        /// <returns>删除结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除DTU", true)]
        [Authorization(AuthorizationCode.S_Structure_DTU_Modify)]
        public HttpResponseMessage Remove(int dtuId, int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var dtuEntity = entity.T_DIM_REMOTE_DTU.FirstOrDefault(d => d.ID == dtuId);
                if (dtuEntity == null || dtuEntity.REMOTE_DTU_STATE == false)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("DTU不存在或已禁用"));
                }

                // 检查传感器关联
                var sens = entity.T_DIM_SENSOR.Where(s => s.DTU_ID == dtuId && !s.IsDeleted);
                if (sens.Any())
                {
                    return Request.CreateResponse(
                        HttpStatusCode.Conflict,
                        StringHelper.GetMessageString("请先删除该DTU下的传感器"));
                }

                // 删除关联
                var sd = entity.T_DIM_STRUCT_DTU.Where(d => d.DtuId == dtuId && d.StructureId == structId);
                foreach (var m in sd)
                {
                    entity.T_DIM_STRUCT_DTU.Remove(m);
                }

                // 检查关联
                if (!entity.T_DIM_STRUCT_DTU.Any(d => d.DtuId == dtuId))
                {
                    dtuEntity.REMOTE_DTU_STATE = false;
                }

                #region 日志信息

                var info = (from d in entity.T_DIM_REMOTE_DTU
                           from s in entity.T_DIM_STRUCT_DTU
                           from st in entity.T_DIM_STRUCTURE
                           where d.ID == s.DtuId && d.ID == dtuId && s.StructureId == st.ID
                           select new { d.REMOTE_DTU_NUMBER, st.STRUCTURE_NAME_CN }).FirstOrDefault();
                var dtu = info == null ? string.Empty : info.REMOTE_DTU_NUMBER;
                var stc = info == null ? string.Empty : info.STRUCTURE_NAME_CN;

                this.Request.Properties["ActionParameterShow"] = string.Format("dtu号：{0}， 所属结构物：{1}", dtu, stc);
                #endregion

                try
                {
                    var dtnod = new DtuNode
                    {
                        DtuId = (uint)dtuEntity.ID,
                        Type = DtuType.Gprs,
                        DtuCode = dtuEntity.REMOTE_DTU_NUMBER,
                        Name = dtuEntity.DESCRIPTION,
                        DacInterval = dtuEntity.REMOTE_DTU_GRANULARITY == null ? DtuNode.DefaultDacInterval : (uint)(dtuEntity.REMOTE_DTU_GRANULARITY * 60),
                        NetworkType = dtuEntity.ProductDtuId == 2 ? NetworkType.hclocal : NetworkType.gprs
                    };
                    if (dtnod.NetworkType == NetworkType.hclocal)
                        dtnod.AddProperty("param1", dtuEntity.P1);
                    entity.SaveChanges();
                    WebClientService.SendToET(ConfigChangedMsgHelper.GetDtuConfigChangedMsg(ChangedStatus.Delete, dtnod));
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("DTU删除成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("DTU删除失败"));
                }
            }
        }

        /// <summary>
        /// 获取DTU远程配置：GET dtu/{dtuId}/remote-config
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取DTU远程配置", false)]
        [Authorization(AuthorizationCode.S_DTU)]
        public object GetDtuRometeConfig(int dtuId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from rd in entity.T_DIM_REMOTE_DTU
                            where rd.ID == dtuId
                            select new
                            {
                                dtuNo = rd.REMOTE_DTU_NUMBER,
                                ip = rd.DTU_IP.Trim(),
                                port = rd.DTU_PORT,
                                ip2 = rd.DtuIp2.Trim(),
                                port2 = rd.DtuPort2,
                                ip3 = rd.DtuIp3.Trim(),
                                port3 = rd.DtuPort3,
                                dtuMode = rd.DtuMode,
                                packetSize = rd.PacketSize,
                                packetInterval = rd.PacketInterval,
                                reconnectionCount = rd.ReconnectionCount
                            };
                var list = query.ToList();
                return list;
            }
        }

        /// <summary>
        /// 修改DTU远程配置请求：POST dtu/{dtuId}/remote-config/modify-request
        /// </summary>
        /// <returns>修改结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改DTU远程配置请求", true)]
        [Authorization(AuthorizationCode.S_DTU_RemoteConfig)]
        public object ModifyDtuRemoteConfigRequest([FromUri]int dtuId, [FromBody]DtuConfig dtuConfig)
        {
            // ET通信
            ILog log = LogManager.GetLogger("DtuRemoteConfig");
            log.DebugFormat("Instant DTU remote config: DTU={0}, dtuConfig={1}", dtuId, dtuConfig);
            // 设置消息头
            Guid guid = Guid.NewGuid();
            FsMessageHeader header = new FsMessageHeader
            {
                S = "WebClient",
                A = "GET",
                R = "/et/dtu/instant/at", // request url.
                U = guid,
                T = Guid.NewGuid()
            };
            // 设置部分消息体
            RemoteConfig rc = new RemoteConfig
            {
                Count = 2,
                Ip1 = dtuConfig.Ip,
                Port1 = dtuConfig.Port,
                Ip2 = dtuConfig.Ip2,
                Port2 = dtuConfig.Port2,
                Mode = dtuConfig.DtuMode.ToUpper(),
                ByteInterval = dtuConfig.PacketInterval,
                Retry = dtuConfig.ReconnectionCount
            };
            List<CommandConfig> listCmd = RemoteConfigService.GetCommand(rc);

            FsMessage msg = new FsMessage
            {
                Header = header,
                Body = new { dtuId, cmds = listCmd }
            };
            // return msg.Body;
            WebClientService.SendToET(msg); // 向ET Push DTU远程配置消息

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

        /// <summary>
        /// 修改DTU远程配置：POST messageId/{messageId}/dtu/{dtuId}/remote-config/modify
        /// </summary>
        /// <returns>修改结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改DTU远程配置", true)]
        [Authorization(AuthorizationCode.S_DTU_RemoteConfig)]
        public object ModifyDtuRemoteConfig([FromUri]string messageId, [FromUri]int dtuId, [FromBody]DtuConfig dtuConfig)
        {
            // 查询 T_TASK_INSTANT 表获取数据:
            using (var entity = new SecureCloud_Entities())
            {
                var dtuEntity = entity.T_DIM_REMOTE_DTU.FirstOrDefault(d => d.ID == dtuId);
                if (dtuEntity == null || dtuEntity.REMOTE_DTU_STATE == false)
                {
                    return ConstructJobject(dtuId, "DTU不存在或已禁用", HttpStatusCode.BadRequest);
                }

                var query = from ti in entity.T_TASK_INSTANT
                            where ti.MSG_ID == messageId
                            select new
                            {
                                dtuId = ti.DTU_ID,
                                data = ti.RESULT_JSON,
                                time = ti.FINISHED,
                                status = ti.RESULT_MSG
                            };
                var list = query.ToList();
                if (list.Count == 0)
                {
                    return null; // 200: OK
                }
                if (list.Select(s => s.dtuId).FirstOrDefault() != dtuId)
                {
                    return ConstructJobject(dtuId, "任务中DTU和当前DTU不匹配", HttpStatusCode.BadRequest);
                }

                var strData = list.Select(s => s.data).FirstOrDefault();
                if (strData == null || strData.Trim() == "")
                {
                    return list.Select(s => new { result = JsonConvert.DeserializeObject(""), s.time, s.status }).FirstOrDefault();
                }

                JObject jObj = list.Select(s => JObject.Parse(s.data)).FirstOrDefault();
                DtuConfig.UpdateDtuConfig(dtuEntity, jObj, dtuConfig);

                #region 日志信息

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(dtuConfig);

                var dtuNo = entity.T_DIM_REMOTE_DTU.Where(w => w.ID == dtuId).Select(s => s.REMOTE_DTU_NUMBER).FirstOrDefault();
                this.Request.Properties["ActionParameterShow"] =
                    string.Format("DTU编号：{0}，主中心ip：{1}，主中心端口：{2}，副中心2ip：{3}，副中心2端口：{4}，" +
                                "DTU工作模式：{5}，封包间隔时间：{6}，重连次数：{7}",
                                dtuNo,
                                dtuConfig.Ip ?? string.Empty,
                                dtuConfig.Port,
                                dtuConfig.Ip2 ?? string.Empty,
                                dtuConfig.Port2,
                                string.IsNullOrEmpty(dtuConfig.DtuMode) ? string.Empty : dtuConfig.DtuMode,
                                dtuConfig.PacketInterval,
                                dtuConfig.ReconnectionCount
                    );

                #endregion

                try
                {
                    entity.SaveChanges();
                    return list.Select(s => new { result = JsonConvert.DeserializeObject(s.data), s.time, s.status }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return ConstructJobject(dtuId, "DTU信息修改失败", HttpStatusCode.BadRequest);
                }
            }
        }

        // 构造DTU远程配置结果
        public static JObject ConstructJobject(int dtuId, string msg, HttpStatusCode statusCode)
        {
            return new JObject(
                new JProperty("result",
                    new JObject(
                        new JProperty("dtuId", dtuId),
                        new JProperty("cmds", 
                            new JArray(
                                new JObject(
                                    new JProperty("cmd", "modifyDtuRemoteConfig"),
                                    new JProperty("result", "DTU信息修改失败")))))),
                new JProperty("time", null),
                new JProperty("status", "xhrStatus-" + statusCode));
        }

        /// <summary>
        /// 获取DTU重启请求：GET dtu/{dtuId}/restart-request
        /// </summary>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取DTU重启请求", false)]
        [Authorization(AuthorizationCode.S_DTU_RemoteConfig)]
        public object GetDtuRestartRequest(int dtuId)
        {
            var guid = Guid.NewGuid();
            var msg = new FsMessage
            {
                Header =
                {
                    S = "WebClient", 
                    A = "GET", 
                    R = "/et/dtu/instant/at-restart", 
                    U = guid, 
                    T = Guid.NewGuid()
                },
                Body = new { dtuId }
            };
            WebClientService.SendToET(msg); // 向ET Push DTU重启消息

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

        /// <summary>
        /// 获取DTU重启结果：messageId/{messageId}/dtu/restart-result
        /// </summary>
        /// <param name="messageId">消息id</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取DTU重启结果", false)]
        [Authorization(AuthorizationCode.S_DTU_RemoteConfig)]
        public object GetDtuRestartResult(string messageId)
        {
            // 查询 T_TASK_INSTANT 表获取数据:
            using (var entity = new SecureCloud_Entities())
            {
                var query = from ti in entity.T_TASK_INSTANT
                            where ti.MSG_ID == messageId
                            select new
                            {
                                dtuId = ti.DTU_ID,
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

    }

    public class DtuConfig
    {
        public string Ip { get; set; }

        public int? Port { get; set; }

        public string Ip2 { get; set; }

        public int? Port2 { get; set; }

        public string Ip3 { get; set; }

        public int? Port3 { get; set; }

        public string DtuMode { get; set; }

        public int? PacketSize { get; set; }

        public int? PacketInterval { get; set; }

        public int? ReconnectionCount { get; set; }

        /// <summary>
        /// 更新DTU远程配置
        /// </summary>
        /// <param name="dtuEntity"></param>
        /// <param name="jObj"></param>
        /// <param name="dtuConfig"></param>
        public static void UpdateDtuConfig(T_DIM_REMOTE_DTU dtuEntity, JObject jObj, DtuConfig dtuConfig)
        {
            // JToken tokenDtuId = jObj["dtuId"];
            var cmds = from cmd in jObj["cmds"].Children()
                       select new { cmd = (string)cmd["cmd"], result = (string)cmd["result"] };
            foreach (var cmd in cmds)
            {
                switch (cmd.cmd)
                {
                    case "setCount":
                        if (cmd.result != "OK")
                        {
                            return;
                        }
                        break;
                    case "setIP1":
                        if (cmd.result == "OK" && dtuConfig.Ip != default(string) && dtuConfig.Ip != dtuEntity.DTU_IP)
                        {
                            dtuEntity.DTU_IP = dtuConfig.Ip;
                        }
                        break;
                    case "setPort1":
                        if (cmd.result == "OK" && dtuConfig.Port != default(int?) && dtuConfig.Port != dtuEntity.DTU_PORT)
                        {
                            dtuEntity.DTU_PORT = dtuConfig.Port;
                        }
                        break;
                    case "setIP2":
                        if (cmd.result == "OK" && dtuConfig.Ip2 != default(string) && dtuConfig.Ip2 != dtuEntity.DtuIp2)
                        {
                            dtuEntity.DtuIp2 = dtuConfig.Ip2;
                        }
                        break;
                    case "setPort2":
                        if (cmd.result == "OK" && dtuConfig.Port2 != default(int?) && dtuConfig.Port2 != dtuEntity.DtuPort2)
                        {
                            dtuEntity.DtuPort2 = dtuConfig.Port2;
                        }
                        break;
                    case "setMode":
                        if (cmd.result == "OK" && dtuConfig.DtuMode != default(string) && dtuConfig.DtuMode.ToUpper() != dtuEntity.DtuMode)
                        {
                            dtuEntity.DtuMode = dtuConfig.DtuMode.ToUpper();
                        }
                        break;
                    case "setByteInterval":
                        if (cmd.result == "OK" && dtuConfig.PacketInterval != default(int?) && dtuConfig.PacketInterval != dtuEntity.PacketInterval)
                        {
                            dtuEntity.PacketInterval = dtuConfig.PacketInterval;
                        }
                        break;
                    case "setRetry":
                        if (cmd.result == "OK" && dtuConfig.ReconnectionCount != default(int?) && dtuConfig.ReconnectionCount != dtuEntity.ReconnectionCount)
                        {
                            dtuEntity.ReconnectionCount = dtuConfig.ReconnectionCount;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class DtuModel
    {        
        public int StructId { get; set; }
        
        public string DtuNo { get; set; }
        
        public string Sim { get; set; }
        
        public int Granularity { get; set; }
        
        public string Ip { get; set; }

        public int ProductId { get; set; }

        public int Port { get; set; }

        public string P1 { get; set; }

        public string P2 { get; set; }

        public string P3 { get; set; }

        public string P4 { get; set; }
    }
}
