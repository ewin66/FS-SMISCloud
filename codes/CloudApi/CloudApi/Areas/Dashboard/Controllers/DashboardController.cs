namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Dashboard.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Service;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using FS.Service;

    using log4net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class DashboardController : ApiController
    {
        /// <summary>
        /// 统计用户下项目状态
        /// </summary>
        /// <param name="userId"> 用户id </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns> 项目状态结果 </returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetProjectsStatusStatisticsByUser(int userId, DateTime startTime, DateTime endTime)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var queryUserRole = from u in entity.T_DIM_USER
                    from r in entity.T_DIM_ROLE
                    where u.ROLE_ID == r.ROLE_ID && u.USER_NO == userId && u.USER_IS_ENABLED
                    select u.ROLE_ID;
                var listUserRole = queryUserRole.ToList();
                if (!listUserRole.Any())
                {
                    return listUserRole;
                }
                
                int[] arrUserStruct;
                var listUserStruct = new List<UserStruct>();
                if (queryUserRole.FirstOrDefault() == 1) // 超级管理员
                {
                    var queryUserStruct = from s in entity.T_DIM_STRUCTURE
                        from os in entity.T_DIM_ORG_STUCTURE
                        from o in entity.T_DIM_ORGANIZATION
                        where s.ID == os.STRUCTURE_ID
                              && os.ORGANIZATION_ID == o.ID
                              && o.IsDeleted == false
                              && s.IsDelete != 1
                        select new UserStruct
                        {
                            ProjectId = o.ID,
                            ProjectName = o.SystemName,
                            ProjectAbbreviation = o.SystemNameAbbreviation,
                            StructId = s.ID,
                            StructStatus = true // 初始化为"true" { true:正常状态, false:异常状态, null:未知状态 }
                        };
                    arrUserStruct = queryUserStruct.Select(s => s.StructId).ToArray();
                    listUserStruct = queryUserStruct.ToList();
                }
                else
                {
                    var queryUserStruct = from u in entity.T_DIM_USER
                        from us in entity.T_DIM_USER_STRUCTURE
                        from s in entity.T_DIM_STRUCTURE
                        from os in entity.T_DIM_ORG_STUCTURE
                        from o in entity.T_DIM_ORGANIZATION
                        where u.USER_NO == userId
                              && u.USER_NO == us.USER_NO
                              && us.STRUCTURE_ID == s.ID
                              && s.ID == os.STRUCTURE_ID
                              && os.ORGANIZATION_ID == o.ID
                              && o.IsDeleted == false
                              && s.IsDelete != 1
                        select new UserStruct
                        {
                            ProjectId = o.ID,
                            ProjectName = o.SystemName,
                            ProjectAbbreviation = o.SystemNameAbbreviation,
                            StructId = s.ID,
                            StructStatus = true // 初始化为"true" { true:正常状态, false:异常状态, null:未知状态 }
                        };
                    arrUserStruct = queryUserStruct.Select(s => s.StructId).ToArray();
                    listUserStruct = queryUserStruct.ToList();
                }
                
                if (arrUserStruct.Length == 0)
                {
                    return new JArray();
                }

                var listStruct = GetStructsIdentification(arrUserStruct); // 获取结构物标识 { 0:实体型, 1:传输型, 2:虚拟型 }
                var listStructAlarm = GetLatestUnprocessedAlarmByStruct(arrUserStruct, startTime, endTime); // 结构物下最近24小时未确认告警
                var listStructDtu = GetLatestNotOnlineDtuByStruct(arrUserStruct); // 结构物下当前不在线DTU(包括"离线DTU"和"从未上线DTU")
                var listStructSensor = GetLatestAbnormalSensorCountByStruct(arrUserStruct); // 从ET接口获取结构物下当前异常传感器个数

                // 确定结构物状态
                foreach (var itemUserStruct in listUserStruct)
                {
                    var structId = itemUserStruct.StructId;
                    // 根据结构物下当前不在线DTU(包括"离线DTU"和"从未上线DTU")确定对应结构物状态
                    if (listStructDtu.Any(a => a.StructId == structId))
                    {
                        itemUserStruct.StructStatus = false;
                    }
                    // 根据结构物下最近24小时未确认告警确定对应结构物状态
                    if (listStructAlarm.Any(a => a.StructId == structId))
                    {
                        itemUserStruct.StructStatus = false;
                    }
                    // 根据"结构物标识类型"及"从ET接口获取结构物下当前异常传感器个数"确定对应结构物状态
                    if (listStruct.Any(a => a.StructId == structId && a.StructIdentification != "传输型"))
                    {
                        var abnormalSensorCount =
                            listStructSensor.Where(w => w.StructId == structId).Select(s => s.AbnormalSensorCount).FirstOrDefault();
                        if (abnormalSensorCount != 0) // "abnormalSensorCount == null" or "abnormalSensorCount == -1" or "abnormalSensorCount > 0"
                        {
                            itemUserStruct.StructStatus = false;
                        }
                    }
                }

                return
                    listUserStruct.GroupBy(g => new { g.ProjectId, g.ProjectName }).Select(s => new
                    {
                        projectId = s.Key.ProjectId,
                        projectName = s.Key.ProjectName,
                        projectAbbreviation = s.Select(v => v.ProjectAbbreviation).FirstOrDefault(),
                        projectStatus = !(s.Any(a => a.StructStatus == false)),
                        abnormalStructCount = s.Count(c => c.StructStatus == false),
                        normalStructCount = s.Count(c => c.StructStatus)
                    })
                    .OrderByDescending(o => o.abnormalStructCount);
            }
        }

        /// <summary>
        /// 统计用户项目下结构物状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetStructsStatusStatisticsByProject(int userId, int projectId, string status, DateTime startTime, DateTime endTime)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var queryUserRole = from u in entity.T_DIM_USER
                    from r in entity.T_DIM_ROLE
                    where u.ROLE_ID == r.ROLE_ID && u.USER_NO == userId && u.USER_IS_ENABLED
                    select u.ROLE_ID;
                var listUserRole = queryUserRole.ToList();
                if (!listUserRole.Any())
                {
                    return listUserRole;
                }

                int[] arrProjectStruct;
                var listProjectStruct = new List<ProjectStruct>();
                if (queryUserRole.FirstOrDefault() == 1) // 超级管理员
                {
                    var queryProjectStruct = from s in entity.T_DIM_STRUCTURE
                        join os in entity.T_DIM_ORG_STUCTURE on s.ID equals os.STRUCTURE_ID into nos
                        from ios in nos
                        join o in entity.T_DIM_ORGANIZATION on ios.ORGANIZATION_ID equals o.ID into no
                        from io in no
                        where io.ID == projectId
                              && io.IsDeleted == false
                              && s.IsDelete != 1
                        select new ProjectStruct
                        {
                            ProjectName = io.SystemName,
                            StructId = s.ID,
                            StructName = s.STRUCTURE_NAME_CN,
                            StructIdentification = "实体型", // 初始化结构物标识为"实体型" { 0:实体型, 1:传输型, 2:虚拟型 }
                            StructStatus = true,
                            UnprocessedAlarmCount = 0,
                            NotOnlineDtuCount = 0,
                            AbnormalSensorCount = null
                        };
                    arrProjectStruct = queryProjectStruct.Select(s => s.StructId).ToArray();
                    listProjectStruct = queryProjectStruct.ToList();
                }
                else
                {
                    var queryProjectStruct = from us in entity.T_DIM_USER_STRUCTURE
                        join s in entity.T_DIM_STRUCTURE on us.STRUCTURE_ID equals s.ID into ns
                        from js in ns
                        join os in entity.T_DIM_ORG_STUCTURE on js.ID equals os.STRUCTURE_ID into nos
                        from ios in nos
                        join o in entity.T_DIM_ORGANIZATION on ios.ORGANIZATION_ID equals o.ID into no
                        from io in no
                        where us.USER_NO == userId
                              && io.ID == projectId
                              && io.IsDeleted == false
                              && js.IsDelete != 1
                        select new ProjectStruct
                        {
                            ProjectName = io.SystemName,
                            StructId = js.ID,
                            StructName = js.STRUCTURE_NAME_CN,
                            StructIdentification = "实体型", // 初始化结构物标识为"实体型" { 0:实体型, 1:传输型, 2:虚拟型 }
                            StructStatus = true,
                            UnprocessedAlarmCount = 0,
                            NotOnlineDtuCount = 0,
                            AbnormalSensorCount = null
                        };
                    arrProjectStruct = queryProjectStruct.Select(s => s.StructId).ToArray();
                    listProjectStruct = queryProjectStruct.ToList();
                }
                
                if (arrProjectStruct.Length == 0)
                {
                    return null;
                }

                var listStruct = GetStructsIdentification(arrProjectStruct); // 获取结构物标识 { 0:实体型, 1:传输型, 2:虚拟型 }
                var listStructAlarm = GetLatestUnprocessedAlarmByStruct(arrProjectStruct, startTime, endTime); // 结构物下最近24小时未确认告警
                var listStructDtu = GetLatestNotOnlineDtuByStruct(arrProjectStruct); // 结构物下当前不在线DTU(包括"离线DTU"和"从未上线DTU")
                var listStructSensor = GetLatestAbnormalSensorCountByStruct(arrProjectStruct); // 从ET接口获取结构物下当前异常传感器个数

                // 确定结构物状态
                foreach (var itemProjectStruct in listProjectStruct)
                {
                    var structId = itemProjectStruct.StructId;
                    // 根据结构物下当前不在线DTU(包括"离线DTU"和"从未上线DTU")确定对应结构物状态
                    if (listStructDtu.Any(a => a.StructId == structId))
                    {
                        itemProjectStruct.StructStatus = false;
                        itemProjectStruct.NotOnlineDtuCount = listStructDtu.Count(c => c.StructId == structId);
                    }
                    // 根据结构物下最近24小时未确认告警确定对应结构物状态
                    if (listStructAlarm.Any(a => a.StructId == structId))
                    {
                        itemProjectStruct.StructStatus = false;
                        itemProjectStruct.UnprocessedAlarmCount = listStructAlarm.Count(c => c.StructId == structId);
                    }
                    // 根据"结构物标识类型"及"从ET接口获取结构物下当前异常传感器个数"确定对应结构物状态
                    if (listStruct.Any(a => a.StructId == structId && a.StructIdentification != "传输型"))
                    {
                        itemProjectStruct.AbnormalSensorCount =
                            listStructSensor.Where(w => w.StructId == structId).Select(s => s.AbnormalSensorCount).FirstOrDefault();
                        if (itemProjectStruct.AbnormalSensorCount == -1) // 若结构物在ET中无缓存传感器, 则重置该结构物异常传感器总数为"null"
                        {
                            itemProjectStruct.AbnormalSensorCount = null;
                        }
                        if (itemProjectStruct.AbnormalSensorCount != 0) // "abnormalSensorCount == null" or "abnormalSensorCount > 0"
                        {
                            itemProjectStruct.StructStatus = false;
                        }
                    }
                    else
                    {
                        itemProjectStruct.StructIdentification = "传输型";
                    }
                }

                // 根据接口请求中"status"返回结果
                if (status == "all")
                    return
                        listProjectStruct.GroupBy(g => g.ProjectName).Select(s =>
                            new JObject(
                                new JProperty("projectName", s.Key),
                                new JProperty("abnormalStructCount", s.Count(c => c.StructStatus == false)),
                                new JProperty("normalStructCount", s.Count(c => c.StructStatus)),
                                new JProperty("structs",
                                    new JArray(s.Select(v =>
                                        new JObject(
                                            new JProperty("structId", v.StructId),
                                            new JProperty("structName", v.StructName),
                                            new JProperty("structIdentification", v.StructIdentification),
                                            new JProperty("structStatus", v.StructStatus),
                                            new JProperty("unprocessedAlarmCount", v.UnprocessedAlarmCount),
                                            new JProperty("notOnlineDtuCount", v.NotOnlineDtuCount),
                                            new JProperty("abnormalSensorCount", v.AbnormalSensorCount))))))).FirstOrDefault();
                bool? boolStatus = null;
                switch (status)
                {
                    case "abnormal":
                        boolStatus = false;
                        break;
                    case "normal":
                        boolStatus = true;
                        break;
                }
                return
                    listProjectStruct.GroupBy(g => g.ProjectName).Select(s =>
                        new
                        {
                            projectName = s.Key,
                            abnormalStructCount = s.Count(c => c.StructStatus == false),
                            normalStructCount = s.Count(c => c.StructStatus),
                            structs = s.Select(v => new 
                            {
                                structId = v.StructId,
                                structName = v.StructName,
                                structIdentification = v.StructIdentification,
                                structStatus = v.StructStatus,
                                unprocessedAlarmCount = v.UnprocessedAlarmCount,
                                notOnlineDtuCount = v.NotOnlineDtuCount,
                                abnormalSensorCount = v.AbnormalSensorCount
                            }).Where(w => w.structStatus == boolStatus)
                        }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 统计结构物下告警、DTU、传感器状态
        /// </summary>
        /// <param name="structId"> 结构物id </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetAlarmsDtusSensorsStatusStatisticsByStruct(int structId, DateTime startTime, DateTime endTime)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var validStruct = entity.T_DIM_STRUCTURE.Any(a => a.ID == structId && a.IsDelete != 1);
                if (!validStruct)
                {
                    return null;
                }

                var queryStructDtu = from s in entity.T_DIM_STRUCTURE
                    join sd in entity.T_DIM_STRUCT_DTU on s.ID equals sd.StructureId into nsd
                    from isd in nsd
                    join rd in entity.T_DIM_REMOTE_DTU on isd.DtuId equals rd.ID into nrd
                    from ird in nrd
                    where s.ID == structId
                          && s.IsDelete != 1
                          && ird.REMOTE_DTU_STATE == true
                          && ird.T_DIM_DTU_PRODUCT.DtuModel != "本地文件" // DTU的产品类型为"本地文件"时, 该DTU为"虚拟型DTU".
                    select new
                    {
                        structName = s.STRUCTURE_NAME_CN,
                        dtuId = ird.ID
                    };

                var queryStructSensor = from sd in queryStructDtu
                    join se in entity.T_DIM_SENSOR on sd.dtuId equals se.DTU_ID into nse
                    from ise in nse
                    where ise.IsDeleted == false
                          && ise.Identification != 2 // 传感器标识 { 0:实体, 1:数据, 2:虚拟/组合 }
                          && ise.DTU_ID != null
                          && ise.MODULE_NO != null
                    select new
                    {
                        sensorId = ise.SENSOR_ID
                    };

                int[] arrStruct = { structId };
                var listStruct = GetStructsIdentification(arrStruct); // 获取结构物标识 { 0:实体型, 1:传输型, 2:虚拟型 }
                var listStructAlarm = GetLatestUnprocessedAlarmByStruct(arrStruct, startTime, endTime); // 结构物下最近24小时未确认告警
                var listStructDtu = GetLatestNotOnlineDtuByStruct(arrStruct); // 结构物下当前不在线DTU(包括"离线DTU"和"从未上线DTU")
                var listStructSensor = GetLatestAbnormalSensorCountByStruct(arrStruct); // 从ET接口获取结构物下当前异常传感器个数
                
                // 根据结构物下当前不在线DTU(包括"离线DTU"和"从未上线DTU")确定对应结构物状态
                bool structStatus = listStructDtu.All(a => a.StructId != structId);
                // 根据结构物下最近24小时未确认告警确定对应结构物状态
                if (listStructAlarm.Any(a => a.StructId == structId))
                {
                    structStatus = false;
                }
                // 根据"结构物标识类型"及"从ET接口获取结构物下当前异常传感器个数"确定对应结构物状态
                int? abnormalSensorCount;
                string structIdentification = "实体型"; // 初始化结构物标识为"实体型" { 0:实体型, 1:传输型, 2:虚拟型 }
                if (listStruct.Any(a => a.StructId == structId && a.StructIdentification != "传输型"))
                {
                    abnormalSensorCount =
                        listStructSensor.Where(w => w.StructId == structId).Select(s => s.AbnormalSensorCount).FirstOrDefault();
                    if (abnormalSensorCount == -1) // 若结构物在ET中无缓存传感器, 则重置该结构物异常传感器总数为"null"
                    {
                        abnormalSensorCount = null;
                    }
                    if (abnormalSensorCount != 0) // "abnormalSensorCount == null" or "abnormalSensorCount > 0"
                    {
                        structStatus = false;
                    }
                }
                else
                {
                    abnormalSensorCount = null; // "传输型"结构物异常传感器个数: null(未知)
                    structIdentification = "传输型";
                }

                return new
                {
                    structName = queryStructDtu.Select(s => s.structName).FirstOrDefault(),
                    structIdentification,
                    structStatus,
                    unprocessedAlarmCount = listStructAlarm.Count,
                    offlineDtuCount = listStructDtu.Count(w => w.DtuStatus == false), // 离线DTU个数
                    neverUplineCount = listStructDtu.Count(w => w.DtuStatus == null), // 从未上线DTU个数
                    dtuTotles = queryStructDtu.Select(s => s.dtuId).Distinct().Count(),
                    abnormalSensorCount,
                    sensorTotles = queryStructSensor.Select(s => s.sensorId).Distinct().Count()
                };
            }
        }

        /// <summary>
        /// 统计结构物下各等级未确认告警数
        /// </summary>
        /// <param name="structId"> 结构物id </param>
        /// <param name="status"> 告警状态 </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetEachAlarmCountStatisticsByStruct(int structId, string status, DateTime startTime, DateTime endTime)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var validStruct = entity.T_DIM_STRUCTURE.Any(a => a.ID == structId && a.IsDelete != 1);
                if (!validStruct)
                {
                    return null;
                }

                var query = from ws in entity.T_WARNING_SENSOR
                    where ws.StructId == structId
                          && ws.T_DIM_STRUCTURE.IsDelete != 1
                          && (ws.Time >= startTime && ws.Time <= endTime)
                          && (ws.DealFlag == 1 || ws.DealFlag == 3)
                    select new
                    {
                        structName = ws.T_DIM_STRUCTURE.STRUCTURE_NAME_CN,
                        alarmLevel = ws.T_DIM_WARNING_TYPE.WarningLevel
                    };
                var list = query.ToList();

                if (list.Count == 0) 
                {
                    var structName = entity.T_DIM_STRUCTURE.Where(w => w.ID == structId && w.IsDelete != 1)
                        .Select(s => s.STRUCTURE_NAME_CN).FirstOrDefault();
                    return
                        new JObject(
                            new JProperty("structName", structName),
                            new JProperty("alarms", new JArray()));
                } 

                return
                    list.GroupBy(g => g.structName).Select(s => new
                    {
                        structName = s.Key,
                        alarms = s.Select(v => new { v.alarmLevel }).GroupBy(r => r.alarmLevel).Select(e => new 
                            { 
                                alarmLevel = e.Key, 
                                alarmCount = e.Count() 
                            })
                    }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取结构物下DTU最新状态
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetDtusLatestStatusByStruct(int structId, string status) 
        {
            using (var entity = new SecureCloud_Entities())
            {
                var validStruct = entity.T_DIM_STRUCTURE.Any(a => a.ID == structId && a.IsDelete != 1);
                if (!validStruct)
                {
                    return null;
                }

                var query = from sd in entity.T_DIM_STRUCT_DTU
                    join rd in entity.T_DIM_REMOTE_DTU on sd.DtuId equals rd.ID into nrd
                    from ird in nrd
                    join ds in entity.T_DIM_DTU_STATUS on ird.ID equals ds.DtuId into nds
                    from ids in nds.DefaultIfEmpty()
                    where sd.StructureId == structId
                          && sd.T_DIM_STRUCTURE.IsDelete != 1
                    select new
                    {
                        structName = sd.T_DIM_STRUCTURE.STRUCTURE_NAME_CN,
                        dtuId = (int?) ird.ID,
                        dtuNo = ird.REMOTE_DTU_NUMBER,
                        dtuIdentification = ird.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "传输" ? "传输型" : (ird.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "本地文件" ? "虚拟型" : "实体型"),
                        dtuStatus = (bool?) ids.Status,
                        time = (DateTime?) ids.Time
                    };
                var list = query.ToList();

                if (list.Count == 0)
                {
                    var structName = entity.T_DIM_STRUCTURE.Where(w => w.ID == structId && w.IsDelete != 1)
                        .Select(s => s.STRUCTURE_NAME_CN).FirstOrDefault();
                    return
                        new JObject(
                            new JProperty("structName", structName),
                            new JProperty("dtus", new JArray()));
                }

                // 根据接口请求中"status"返回结果
                if (status == "all")
                    return
                        list.GroupBy(g => g.structName).Select(s => new
                        {
                            structName = s.Key,
                            dtus = s.Select(v => new 
                            { 
                                v.dtuId, 
                                v.dtuNo, 
                                v.dtuIdentification,
                                v.dtuStatus, 
                                v.time 
                            }).GroupBy(r => new { r.dtuId, r.dtuNo, r.dtuIdentification }).Select(e => new
                            {
                                e.Key.dtuId,
                                e.Key.dtuNo,
                                e.Key.dtuIdentification,
                                status = e.OrderByDescending(o => o.time).Select(l => l.dtuStatus).FirstOrDefault()
                            })
                        }).FirstOrDefault();
                bool? boolStatus = null;
                switch (status)
                {
                    case "offline": // "离线"状态
                        boolStatus = false;
                        break;
                    case "online": // "在线"状态
                        boolStatus = true;
                        break;
                }
                return
                    list.GroupBy(g => g.structName).Select(s => new
                    {
                        structName = s.Key,
                        dtus = s.Select(v => new
                        {
                            v.dtuId,
                            v.dtuNo,
                            v.dtuIdentification,
                            v.dtuStatus,
                            v.time
                        }).GroupBy(r => new { r.dtuId, r.dtuNo, r.dtuIdentification }).Select(e => new
                        {
                            e.Key.dtuId,
                            e.Key.dtuNo,
                            e.Key.dtuIdentification,
                            status = e.OrderByDescending(o => o.time).Select(l => l.dtuStatus).FirstOrDefault()
                        }).Where(w => w.status == boolStatus)
                    }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取DTU最新状态
        /// </summary>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetDtuLatestStatus(int dtuId) 
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from rd in entity.T_DIM_REMOTE_DTU
                    join ds in entity.T_DIM_DTU_STATUS on rd.ID equals ds.DtuId into nds
                    from ids in nds.DefaultIfEmpty()
                    where rd.ID == dtuId
                    select new
                    {
                        dtuNo = rd.REMOTE_DTU_NUMBER,
                        identification = rd.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "传输" ? "传输型" : (rd.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "本地文件" ? "虚拟型" : "实体型"),
                        status = (bool?) ids.Status,
                        time = (DateTime?) ids.Time
                    };

                if (!query.Any())
                {
                    return null;
                }

                var orderedDtuStatus = query.ToList().OrderByDescending(o => o.time);
                var list = orderedDtuStatus.Take(2);
                var lastOnlineTime = list.Select(s => s.time).LastOrDefault();
                var currentOfflineTime = list.Select(s => s.time).FirstOrDefault();
                var dtuNo = list.Select(s => s.dtuNo).FirstOrDefault();
                var identification = list.Select(s => s.identification).FirstOrDefault();

                var dtuStatus = orderedDtuStatus.Select(s => s.status).FirstOrDefault();
                if (dtuStatus == true)
                {
                    lastOnlineTime = orderedDtuStatus.Select(s => s.time).FirstOrDefault();
                    currentOfflineTime = null;
                }

                return
                    new JObject(
                        new JProperty("dtuNo", dtuNo),
                        new JProperty("identification", identification),
                        new JProperty("status", dtuStatus),
                        new JProperty("now", DateTime.Now),
                        new JProperty("lastOnlineTime", lastOnlineTime),
                        new JProperty("currentOfflineTime", currentOfflineTime)); // 当前离线时间
            }
        }

        /// <summary>
        /// 获取DTU历史状态
        /// </summary>
        /// <param name="dtuId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetDtuHistoryStatus(int dtuId, DateTime startTime, DateTime endTime) 
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from rd in entity.T_DIM_REMOTE_DTU
                    join ds in entity.T_DIM_DTU_STATUS on rd.ID equals ds.DtuId into nds
                    from ids in nds.DefaultIfEmpty()
                    where rd.ID == dtuId
                          && (ids.Time >= startTime && ids.Time <= endTime)
                    select new
                    {
                        dtuNo = rd.REMOTE_DTU_NUMBER,
                        status = (bool?) ids.Status,
                        time = (DateTime?) ids.Time
                    };
                var list = query.ToList();

                if (list.Count == 0)
                {
                    var dtuNo = entity.T_DIM_REMOTE_DTU.Where(w => w.ID == dtuId).Select(s => s.REMOTE_DTU_NUMBER).FirstOrDefault();
                    return
                        new JObject(
                            new JProperty("dtuNo", dtuNo),
                            new JProperty("datas", new JArray()));
                }

                return
                    list.GroupBy(g => g.dtuNo).Select(s => new
                    {
                        dtuNo = s.Key,
                        now = DateTime.Now,
                        datas = s.OrderBy(o => o.time).Select(v => new 
                        {
                            v.status,
                            v.time
                        })
                    }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取DTU基本信息
        /// </summary>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetDtuDetails(int dtuId) 
        { 
            using (var entity = new SecureCloud_Entities())
            {
                var query = from rd in entity.T_DIM_REMOTE_DTU
                    where rd.ID == dtuId
                    select new
                    {
                        dtuNo = rd.REMOTE_DTU_NUMBER,
                        identification = rd.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "传输" ? "传输型" : (rd.T_DIM_DTU_PRODUCT.DtuModel.Trim() == "本地文件" ? "虚拟型" : "实体型"),
                        factory = rd.T_DIM_DTU_PRODUCT.DtuFactory,
                        model = rd.T_DIM_DTU_PRODUCT.DtuModel,
                        network = rd.T_DIM_DTU_PRODUCT.NetworkType.ToUpper(),
                        sim = rd.REMOTE_DTU_SUBSCRIBER.Trim(),
                        granularity = rd.REMOTE_DTU_GRANULARITY,
                        mode = rd.DtuMode,
                        ip = rd.DTU_IP.Trim(),
                        port = rd.DTU_PORT,
                        ip2 = rd.DtuIp2.Trim(),
                        port2 = rd.DtuPort2,
                        packetInterval = rd.PacketInterval,
                        reconnectionCount = rd.ReconnectionCount
                    };
                return query.FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取指定结构物的指定DTU下传感器状态
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetSensorsStatusByStructAndDtu(int structId, int dtuId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var validStruct = entity.T_DIM_STRUCTURE.Any(a => a.ID == structId && a.IsDelete != 1);
                if (!validStruct)
                {
                    return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("不存在该结构物"));
                }
                var validDtu = entity.T_DIM_REMOTE_DTU.Any(a => a.ID == dtuId);
                if (!validDtu)
                {
                    return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("不存在该DTU")); 
                }

                var dtuModel =
                    entity.T_DIM_REMOTE_DTU.Where(w => w.ID == dtuId).Select(s => s.T_DIM_DTU_PRODUCT.DtuModel.Trim()).FirstOrDefault();
                var dtuIdentification = dtuModel == "传输" ? "传输型" : (dtuModel == "本地文件" ? "虚拟型" : "实体型");

                var dtuStatus = 
                    entity.T_DIM_DTU_STATUS.Where(w => w.DtuId == dtuId).OrderByDescending(o => o.Time).Select(s => s.Status).FirstOrDefault();

                var query = from s in entity.T_DIM_SENSOR
                    where s.STRUCT_ID == structId
                          && s.DTU_ID == dtuId
                          && s.Identification != 2 // 传感器标识 { 0:实体, 1:数据, 2:虚拟/组合 }
                          && s.IsDeleted == false
                    select new
                    {
                        dtuIdentification,
                        dtuStatus,
                        sensorId = s.SENSOR_ID,
                        location = s.SENSOR_LOCATION_DESCRIPTION,
                        identify = s.Identification,
                        module = s.MODULE_NO,
                        channel = s.DAI_CHANNEL_NUMBER
                    };
                var list = query.ToList();
                var listSensorStatus = GetLatestSensorsStatus(structId, list.Select(s => s.sensorId).ToArray());

                return
                    list.GroupBy(g => new { g.dtuIdentification, g.dtuStatus}).Select(s => new
                    {
                        s.Key.dtuIdentification,
                        s.Key.dtuStatus,
                        sensors = s.Select(v => new
                        {
                            v.sensorId,
                            v.location,
                            v.identify,
                            v.module,
                            v.channel,
                            dacStatus = listSensorStatus.Where(w => w.SensorId == v.sensorId).Select(e => e.DacStatus).FirstOrDefault()
                        })
                    }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 统计结构物下传感器状态, 从ET获取
        /// </summary>
        /// <param name="structId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetSensorsStatusStatisticsByStruct(int structId)
        {
            ILog log = LogManager.GetLogger("AllSensorsStatusByStruct");
            log.DebugFormat("Instance sensors status of struct: structId={0}", structId);
            // 设置消息头
            FsMessageHeader header = new FsMessageHeader
            {
                S = "WebClient",
                A = "GET",
                R = "/et/status/struct/allSensors" // request url.
            };
            FsMessage msg = new FsMessage
            {
                Header = header,
                Body = new { structId }
            };

            var sw = new Stopwatch();
            sw.Start();
            FsMessage msgReceived = WebClientService.RequestToET(msg, 100); // 向ET Request 结构物下传感器状态, 超时时间100ms.
            sw.Stop();
            log.InfoFormat("统计结构物下传感器状态超时时间设定为100ms, api和ET通信实际耗时: {0}ms", sw.ElapsedMilliseconds);

            if (msgReceived == null || msgReceived.Body == null) // 超时或通信忙状态或返回结果为空
            {
                return new JArray();
            }

            return
            JsonConvert.DeserializeObject<List<object>>(msgReceived.Body.ToString());//反序列化
        }

        ///<summary>
        /// 获取未知状态的传感器列表
        /// </summary>
        [AcceptVerbs("Get")]
        public object GetUnknowSensor(int structId, string sensorId)
        {
            using (var entity = new SecureCloud_Entities())
            {

                var alist = sensorId.Split(',');
                var arr=alist.Select(m => Convert.ToInt32(m)).ToArray();

                var query1 =
                    from b in entity.T_DIM_SENSOR
                    where b.STRUCT_ID == structId && !arr.Contains(b.SENSOR_ID) && b.Identification != 2 && b.IsDeleted == false //不包含 !arr.Contains(b.SENSOR_ID)
                          && b.DTU_ID != null && b.MODULE_NO != null
                    select new
                        {   Id=b.SENSOR_ID,
                            location = b.SENSOR_LOCATION_DESCRIPTION
                        };


                return query1.Distinct().ToList();


            }
        }


        ///<summary>
        /// 获取禁用的传感器列表
        /// </summary>
        [AcceptVerbs("Get")]
       public object GetUnableSensor(int structId)
       {
           using (var entity = new SecureCloud_Entities())
           {
               var query = from s in entity.T_DIM_SENSOR
                           where s.STRUCT_ID == structId && s.Identification != 2 && s.IsDeleted == false
                                 && s.DTU_ID != null && s.MODULE_NO != null && s.Enable == true
                           select new
                               {
                                   location = s.SENSOR_LOCATION_DESCRIPTION,
                                   Id = s.SENSOR_ID
                               };




               return query.Distinct().ToList();

           }
       }
 

        /// <summary>
        /// 获取传感器最新状态
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetSensorLatestStatus(int structId,int sensorId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                    join rd in entity.T_DIM_REMOTE_DTU on s.DTU_ID equals rd.ID into nrd
                    from ird in nrd.DefaultIfEmpty()
                    where s.SENSOR_ID == sensorId
                    select new
                    {
                        location = s.SENSOR_LOCATION_DESCRIPTION,
                        identify = s.Identification, // 传感器标识 { 0:实体, 1:数据, 2:虚拟/组合 }
                        dtuId = (int?) ird.ID,
                        dtuNo = ird.REMOTE_DTU_NUMBER,
                        dtuStatus = (bool?) ird.T_DIM_DTU_STATUS.OrderByDescending(o => o.Time).Select(e => e.Status).FirstOrDefault(),
                        module = s.MODULE_NO,
                        channel = s.DAI_CHANNEL_NUMBER,
                        dtuIdentify = ird.ProductDtuId,
                        enable = s.Enable
                    };

                int[] arrSensor = { sensorId };
                var listSensorStatus = GetLatestSensorsStatus(structId,arrSensor);//xu

                return
                    query.ToList().Select(s => new
                    {
                        s.location,
                        s.identify,
                        s.dtuId,
                        s.dtuNo,
                        s.dtuStatus,
                        s.module,
                        s.channel,
                        s.dtuIdentify,
                        s.enable,
                        dacStatus = listSensorStatus.Where(w => w.SensorId == sensorId).Select(e => e.DacStatus).FirstOrDefault()
                    }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 统计传感器历史异常次数
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetSensorHistoryAbnormalCountStatistics(int sensorId, DateTime startTime, DateTime endTime )
        {
            using (var entity = new SecureCloud_Entities())
            {
                var validSensor = entity.T_DIM_SENSOR.Any(a => a.SENSOR_ID == sensorId);
                if (!validSensor)
                {
                    return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("不存在该传感器")); 
                }

                var query = from ws in entity.T_WARNING_SENSOR
                    where ws.DeviceId == sensorId
                          && (ws.Time >= startTime && ws.Time <= endTime)
                    select new
                    {
                        alarmTypeId = ws.WarningTypeId,
                        reason = ws.T_DIM_WARNING_TYPE.Reason,
                        time = ws.Time
                    };
                return
                    query.ToList().GroupBy(g => new { g.alarmTypeId, g.reason }).Select(s => new
                    {
                        s.Key.alarmTypeId,
                        //data = s.Key.reason + "次数: " + s.Count() + "次"
                        data = s.Key.reason + ": " + s.Count()
                    });
            }
        }

        /// <summary>
        /// 获取传感器指定告警类型下历史状态
        /// </summary>
        /// <param name="sensorId"> 传感器id </param>
        /// <param name="alarmTypeId"> 告警类型id </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Dashboard)]
        public object GetSensorHistoryStatusByAlarmType(int sensorId, string alarmTypeId, DateTime startTime, DateTime endTime)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var validSensor = entity.T_DIM_SENSOR.Any(a => a.SENSOR_ID == sensorId);
                if (!validSensor)
                {
                    return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("不存在该传感器")); 
                }

                var query = from ws in entity.T_WARNING_SENSOR
                    from s in entity.T_DIM_SENSOR
                    where ws.DeviceId == sensorId
                          && ws.DeviceId == s.SENSOR_ID
                          && ws.WarningTypeId == alarmTypeId
                          && (ws.Time >= startTime && ws.Time <= endTime)
                    select new
                    {
                        location = s.SENSOR_LOCATION_DESCRIPTION,
                        reason = ws.T_DIM_WARNING_TYPE.Reason,
                        time = ws.Time
                    };
                return
                    query.GroupBy(g => new { g.location, g.reason }).Select(s => new
                    {
                        s.Key.location,
                        status = s.Key.reason,
                        datas = s.Select(v => new { v.time })
                    }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取用户下组织结构物列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        public object GetOrgStructsListByUser(int userId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var queryUserRole = from u in entity.T_DIM_USER
                    from r in entity.T_DIM_ROLE
                    where u.ROLE_ID == r.ROLE_ID && u.USER_NO == userId && u.USER_IS_ENABLED
                    select u.ROLE_ID;
                var listUserRole = queryUserRole.ToList();
                if (!listUserRole.Any())
                {
                    return listUserRole;
                }

                var list = new List<UserOrgStruct>();
                if (queryUserRole.FirstOrDefault() == 1) // 超级管理员
                {
                    var query = from s in entity.T_DIM_STRUCTURE
                        from os in entity.T_DIM_ORG_STUCTURE
                        from o in entity.T_DIM_ORGANIZATION
                        where s.ID == os.STRUCTURE_ID
                              && os.ORGANIZATION_ID == o.ID
                              && o.IsDeleted == false
                              && s.IsDelete != 1
                        select new UserOrgStruct
                        {
                            UserName = entity.T_DIM_USER.Where(w => w.USER_NO == userId).Select(e => e.USER_NAME).FirstOrDefault(),
                            OrgId = (int?) o.ID,
                            OrgName = o.ABB_NAME_CN,
                            StructId = (int?) s.ID,
                            StructName = s.STRUCTURE_NAME_CN
                        };
                    list = query.ToList();
                }
                else
                {
                    var query = from u in entity.T_DIM_USER
                        join us in entity.T_DIM_USER_STRUCTURE on u.USER_NO equals us.USER_NO into nus
                        from ius in nus.DefaultIfEmpty()
                        from s in entity.T_DIM_STRUCTURE
                        from os in entity.T_DIM_ORG_STUCTURE
                        where u.USER_NO == userId
                              && ius.STRUCTURE_ID == s.ID
                              && s.ID == os.STRUCTURE_ID
                              && os.T_DIM_ORGANIZATION.IsDeleted == false
                              && s.IsDelete != 1
                        select new UserOrgStruct
                        {
                            UserName = u.USER_NAME,
                            OrgId = (int?) os.ORGANIZATION_ID,
                            OrgName = os.T_DIM_ORGANIZATION.ABB_NAME_CN,
                            StructId = (int?) s.ID,
                            StructName = s.STRUCTURE_NAME_CN
                        };
                    list = query.ToList();
                }
                return
                    list.GroupBy(g => g.UserName).Select(s => new
                    {
                        userName = s.Key,
                        orgs = s.GroupBy(r => new { r.OrgId, r.OrgName }).Select(v => new
                        {
                            orgId = v.Key.OrgId,
                            orgName = v.Key.OrgName,
                            structs = v.Select(e => new
                            {
                                structId = e.StructId,
                                structName = e.StructName
                            })
                        })
                    }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取结构物标识 { 0:实体型, 1:传输型, 2:虚拟型 }
        /// </summary>
        /// <param name="structs"> 结构物id数组 </param>
        /// <returns></returns>
        public List<StructIdentificationModel> GetStructsIdentification(int[] structs)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var queryStruct = from s in entity.T_DIM_STRUCTURE
                    where structs.Contains(s.ID)
                          && s.IsDelete != 1
                    select new
                    {
                        structId = s.ID,
                        structIdentification = 0
                    };
                var listStruct =
                    queryStruct.ToList().Select(s => new StructIdentificationModel
                    {
                        StructId = s.structId,
                        StructIdentification = ((StructIdentification)s.structIdentification).ToString() // 初始化为"实体型"结构物
                    }).ToList();
                // 确定结构物标识
                foreach (var itemStruct in listStruct)
                {
                    var queryDtu = from d in entity.T_DIM_REMOTE_DTU
                        from sd in entity.T_DIM_STRUCT_DTU
                        where d.ID == sd.DtuId
                              && sd.StructureId == itemStruct.StructId
                        select new
                        {
                            dtuId = d.ID,
                            dtuModel = d.T_DIM_DTU_PRODUCT.DtuModel,
                            dtuStatus = d.T_DIM_DTU_STATUS.OrderByDescending(o => o.Time).Select(s => s.Status).FirstOrDefault(),
                            sensorTotles = entity.T_DIM_SENSOR.Count(w => w.DTU_ID == d.ID 
                                && w.IsDeleted == false
                                && w.Identification != 2 // 传感器标识 { 0:实体, 1:数据, 2:虚拟/组合 }
                                && w.DTU_ID != null
                                && w.MODULE_NO != null)
                        };
                    if (queryDtu.Any(a => a.dtuModel == "传输"))
                    {
                        itemStruct.StructIdentification = ((StructIdentification) 1).ToString(); // 设置为"传输型"结构物
                    }
                }

                return listStruct;
            }
        }

        /// <summary>
        /// 统计结构物下当前不在线DTU("不在线DTU"包括"离线DTU"和"从未上线DTU")
        /// </summary>
        /// <param name="structs"> 结构物id数组 </param>
        /// <returns></returns>
        public List<StructDtuStatus> GetLatestNotOnlineDtuByStruct(int[] structs)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var queryStructDtu = from s in entity.T_DIM_STRUCTURE
                    join sd in entity.T_DIM_STRUCT_DTU on s.ID equals sd.StructureId into nsd
                    from isd in nsd
                    join rd in entity.T_DIM_REMOTE_DTU on isd.DtuId equals rd.ID into nrd
                    from ird in nrd
                    join ds in entity.T_DIM_DTU_STATUS on ird.ID equals ds.DtuId into nds
                    from ids in nds.DefaultIfEmpty()
                    where structs.Contains(s.ID)
                          && ird.REMOTE_DTU_STATE == true
                          && ird.T_DIM_DTU_PRODUCT.DtuModel != "本地文件" // DTU的产品类型为"本地文件"时, 该DTU为"虚拟型DTU".
                    select new
                    {
                        structId = s.ID,
                        dtuId = (int?) ird.ID,
                        dtuStatus = (bool?) ids.Status,
                        time = (DateTime?) ids.Time
                    };
                var listStructDtu = queryStructDtu.GroupBy(g => new {g.structId, g.dtuId})
                    .Select(s => new StructDtuStatus
                    {
                        StructId = s.Key.structId,
                        DtuId = s.Key.dtuId,
                        DtuStatus = s.OrderByDescending(o => o.time).Select(a => a.dtuStatus).FirstOrDefault(),
                        Time = s.OrderByDescending(o => o.time).Select(a => a.time).FirstOrDefault()
                    })
                    .Where(w => w.DtuStatus == false || w.DtuStatus == null).ToList();

                return listStructDtu;
            }
        }

        /// <summary>
        /// 统计结构物下最近24小时未确认告警
        /// </summary>
        /// <param name="structs"> 结构物id数组 </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StructAlarmStatus> GetLatestUnprocessedAlarmByStruct(int[] structs, DateTime startTime, DateTime endTime)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var queryStructAlarm = from ws in entity.T_WARNING_SENSOR
                    where structs.Contains(ws.StructId)
                          && (ws.Time >= startTime && ws.Time <= endTime)
                          && (ws.DealFlag == 1 || ws.DealFlag == 3)
                    select new StructAlarmStatus
                    {
                        StructId = ws.StructId,
                        WarningId = (int?) ws.Id
                    };

                return queryStructAlarm.ToList();
            }
        }

        /// <summary>
        /// 统计结构物下异常传感器个数, 从ET获取
        /// </summary>
        /// <param name="structs"> 结构物id数组 </param>
        /// <returns></returns>
        public List<StructAbnormalSensorCount> GetLatestAbnormalSensorCountByStruct(int[] structs)
        {
            ILog log = LogManager.GetLogger("AbnormalSensorCountByStructs");
            log.DebugFormat("Instance abnormal sensors count of structs: structIds={0}", string.Join(",", structs));

            // 设置消息头
            FsMessageHeader header = new FsMessageHeader
            {
                S = "WebClient",
                A = "GET",
                R = "/et/status/structs/abnormalSensorCount" // request url.
            };
            FsMessage msg = new FsMessage
            {
                Header = header,
                Body = new { structIds = structs }
            };

            var sw = new Stopwatch();
            sw.Start();
            FsMessage msgReceived = WebClientService.RequestToET(msg, 1000); // 向ET Request 结构物下异常传感器个数, 超时时间1000ms.
            sw.Stop();
            log.InfoFormat("统计结构物下异常传感器个数超时时间设定为1000ms, api和ET通信实际耗时: {0}ms", sw.ElapsedMilliseconds);

            if (msgReceived == null || msgReceived.Body == null) // 超时或通信忙状态或返回结果为空
            {
                return
                    structs.Select(s => new StructAbnormalSensorCount
                    {
                        StructId = s,
                        AbnormalSensorCount = null
                    }).ToList();
            }

            return
                JsonConvert.DeserializeObject<List<StructAbnormalSensorCount>>(msgReceived.Body.ToString());
        }

        /// <summary>
        /// 获取传感器最新采集状态, 从ET获取
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="sensors"></param>
        /// <returns></returns>
        public List<SensorStatus> GetLatestSensorsStatus(int structId,int[] sensors)
        {
            ILog log = LogManager.GetLogger("SensorStatus");
            log.DebugFormat("Instance sensors status:structId={0}, sensorIds={1}",structId ,string.Join(",", sensors));

            // 设置消息头
            FsMessageHeader header = new FsMessageHeader
            {
                S = "WebClient",
                A = "GET",
                R = "/et/status/sensors" // request url.
            };
            FsMessage msg = new FsMessage
            {
                Header = header,
                Body = new
                {
                    structId = structId,
                    sensorIds = sensors
                }
            };

            var sw = new Stopwatch();
            sw.Start();
            FsMessage msgReceived = WebClientService.RequestToET(msg, 100); // 向ET Request 传感器最新采集状态, 超时时间100ms.
            sw.Stop();
            log.InfoFormat("获取传感器最新采集状态超时时间设定为100ms, api和ET通信实际耗时: {0}ms", sw.ElapsedMilliseconds);

            if (msgReceived == null || msgReceived.Body == null) // 超时或通信忙状态或返回结果为空
            {
                return
                    sensors.Select(s => new SensorStatus
                    {
                        SensorId = s,
                        DacStatus = "N/A"
                    }).ToList();
            }

            return
                JsonConvert.DeserializeObject<List<SensorStatus>>(msgReceived.Body.ToString());
        }
    }


    public class UserStruct
    {
        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("projectAbbreviation")]
        public string ProjectAbbreviation { get; set; }

        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("structStatus")]
        public bool StructStatus { get; set; }
    }

    public class StructIdentificationModel
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("structIdentification")]
        public string StructIdentification { get; set; }
    }

    public enum StructIdentification
    {
        实体型 = 0,
        传输型 = 1,
        虚拟型 = 2
    }

    public class StructDtuStatus
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("dtuId")]
        public int? DtuId { get; set; }

        [JsonProperty("dtuStatus")]
        public bool? DtuStatus { get; set; }

        [JsonProperty("time")]
        public DateTime? Time { get; set; }
    }

    public class StructAlarmStatus
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("warningId")]
        public int? WarningId { get; set; }
    }

    public class StructAbnormalSensorCount
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("abnormalSensorCount")]
        public int? AbnormalSensorCount { get; set; }
    }

    public class SensorStatus
    {
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }

        [JsonProperty("dacStatus")]
        public string DacStatus { get; set; }
    }

    public class ProjectStruct
    {
        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("structName")]
        public string StructName { get; set; }

        [JsonProperty("structIdentification")]
        public string StructIdentification { get; set; }

        [JsonProperty("structStatus")]
        public bool StructStatus { get; set; }

        [JsonProperty("unprocessedAlarmCount")]
        public int UnprocessedAlarmCount { get; set; }

        [JsonProperty("notOnlineDtuCount")]
        public int NotOnlineDtuCount { get; set; }

        [JsonProperty("abnormalSensorCount")]
        public int? AbnormalSensorCount { get; set; }
    }

    public class UserOrgStruct 
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("orgId")]
        public int? OrgId { get; set; }

        [JsonProperty("orgName")]
        public string OrgName { get; set; }

        [JsonProperty("structId")]
        public int? StructId { get; set; }

        [JsonProperty("structName")]
        public string StructName { get; set; }
    }
}
