using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config;
using FreeSun.FS_SMISCloud.Server.CloudApi.Service;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using System.Web.Http;

    using System.Net;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using log4net;

    public class SensorGroupController : ApiController
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 获取传感器分组类型
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器分组类型", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object FindGroupType(int structId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var list = new List<GroupType>();

                var sensorFactor =
                    (from s in db.T_DIM_SENSOR where s.STRUCT_ID == structId && !s.IsDeleted && s.Identification != 1 select s.SAFETY_FACTOR_TYPE_ID).Distinct();

                foreach (var factor in sensorFactor)
                {
                    if (factor == 10)
                    {
                        list.Add(new GroupType { GroupTypeId = 1, GroupTypeName = "测斜分组" });
                    }
                    else if (factor == 31)
                    {
                        list.Add(new GroupType { GroupTypeId = 2, GroupTypeName = "挠度分组" });
                    }
                    else if (factor == 34)
                    {
                        list.Add(new GroupType { GroupTypeId = 3, GroupTypeName = "浸润线分组" });
                    }
                    else if (factor == 42)
                    {
                        list.Add(new GroupType { GroupTypeId = 2, GroupTypeName = "沉降分组" });
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// 获取结构物下的测斜分组
        /// </summary>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下的测斜分组", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        [Authorization(AuthorizationCode.U_Common)]
        public object FindGroupsCeXieByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                            from q in entity.T_DIM_SENSOR_GROUP_CEXIE
                            from m in entity.T_DIM_GROUP
                            where
                                s.STRUCT_ID == structId && s.IsDeleted == false && s.SENSOR_ID == q.SENSOR_ID
                                && q.GROUP_ID == m.GROUP_ID && s.Identification != 1
                            select
                                new
                                    {
                                        groupId = m.GROUP_ID,
                                        groupName = m.GROUP_NAME,
                                        sensorId = s.SENSOR_ID,
                                        sensorLocation = s.SENSOR_LOCATION_DESCRIPTION,
                                        depth = q.DEPTH
                                    };

                var query2 = from s in query
                             group s by new { s.groupId, s.groupName }
                                 into g
                                 select
                                     new
                                         {
                                             g.Key.groupId,
                                             g.Key.groupName,
                                             sensorList = from p in g select new { p.sensorId, p.sensorLocation, p.depth }
                                         };

                return query2.ToList();
            }
        }

        /// <summary>
        /// 获取结构物下沉降分组
        /// </summary>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下的沉降分组", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        [Authorization(AuthorizationCode.U_Common)]
        public object FindGroupsChenJiangByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                            from q in entity.T_DIM_SENSOR_GROUP_CHENJIANG
                            from m in entity.T_DIM_GROUP
                            where s.STRUCT_ID == structId && s.IsDeleted == false
                                && s.SENSOR_ID == q.SENSOR_ID
                                && q.GROUP_ID == m.GROUP_ID
                                && s.Identification != 1
                            select
                                new
                                {
                                    groupId = m.GROUP_ID,
                                    groupName = m.GROUP_NAME,
                                    sensorId = s.SENSOR_ID,
                                    sensorLocation = s.SENSOR_LOCATION_DESCRIPTION,
                                    len = q.LENGTH,
                                    isDatum = q.isJIZHUNDIAN
                                };

                var query2 = from s in query
                             group s by new { s.groupId, s.groupName }
                                 into g
                                 select
                                     new
                                     {
                                         g.Key.groupId,
                                         g.Key.groupName,
                                         sensorList = from p in g select new { p.sensorId, p.sensorLocation, p.len, p.isDatum }
                                     };

                return query2.ToList();
            }
        }

        /// <summary>
        /// 获取结构物下的浸润线分组
        /// </summary>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物下的浸润线分组", false)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        [Authorization(AuthorizationCode.U_Common)]
        public object FindGroupsJinRunXianByStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR
                            from q in entity.T_DIM_SENSOR_GROUP_JINRUNXIAN
                            from m in entity.T_DIM_GROUP
                            where
                                s.STRUCT_ID == structId && s.IsDeleted == false && s.SENSOR_ID == q.SENSOR_ID
                                && q.GROUP_ID == m.GROUP_ID && s.Identification != 1
                            select
                                new
                                    {
                                        groupId = m.GROUP_ID,
                                        groupName = m.GROUP_NAME,
                                        sensorId = s.SENSOR_ID,
                                        sensorLocation = s.SENSOR_LOCATION_DESCRIPTION,
                                        height = q.HEIGHT
                                    };

                var query2 = from s in query
                             group s by new { s.groupId, s.groupName }
                                 into g
                                 select
                                     new
                                         {
                                             g.Key.groupId,
                                             g.Key.groupName,
                                             sensorList = from p in g select new { p.sensorId, p.sensorLocation, p.height }
                                         };

                return query2.ToList();
            }
        }

        /// <summary>
        /// 删除传感器组
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("删除传感器组", true)]
        [Authorization(AuthorizationCode.S_Structure_SensorGroup_Modify)]
        public HttpResponseMessage RemoveGroup([FromUri] int groupId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var group = (from s in db.T_DIM_GROUP
                             where s.GROUP_ID == groupId
                             select s).FirstOrDefault();
                if (group == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器组不存在"));
                }

                #region 日志

                this.Request.Properties["ActionParameterShow"] = string.Format("组:{0}", group.GROUP_NAME);
                #endregion

                var cexie = from c in db.T_DIM_SENSOR_GROUP_CEXIE
                            where c.GROUP_ID == groupId
                            select c;
                foreach (var c in cexie)
                {
                    db.T_DIM_SENSOR_GROUP_CEXIE.Remove(c);
                }

                var gscexie = (from s in db.T_DIM_SENSOR
                               from sg in cexie
                               from d in db.T_DIM_REMOTE_DTU
                               where s.SENSOR_ID == sg.SENSOR_ID && d.ID == s.DTU_ID
                               select
                                   new SensorGroup
                                   {
                                       GroupId = groupId,
                                       DtuId = s.DTU_ID.Value,
                                       DtuCode = d.REMOTE_DTU_NUMBER,
                                       DacInterval = d.REMOTE_DTU_GRANULARITY.Value
                                   }).Distinct();


                var chenjiang = from j in db.T_DIM_SENSOR_GROUP_CHENJIANG
                                where j.GROUP_ID == groupId
                                select j;

                var gschenjiang = (from s in db.T_DIM_SENSOR
                                   from sg in chenjiang
                               from d in db.T_DIM_REMOTE_DTU
                               where s.SENSOR_ID == sg.SENSOR_ID && d.ID == s.DTU_ID
                               select
                                   new SensorGroup
                                   {
                                       GroupId = groupId,
                                       DtuId = s.DTU_ID.Value,
                                       DtuCode = d.REMOTE_DTU_NUMBER,
                                       DacInterval = d.REMOTE_DTU_GRANULARITY.Value
                                   }).Distinct();

                foreach (var j in chenjiang)
                {
                    db.T_DIM_SENSOR_GROUP_CHENJIANG.Remove(j);
                }

                var jinrunxian = from r in db.T_DIM_SENSOR_GROUP_JINRUNXIAN
                                 where r.GROUP_ID == groupId
                                 select r;
                foreach (var r in jinrunxian)
                {
                    db.T_DIM_SENSOR_GROUP_JINRUNXIAN.Remove(r);
                }

                db.T_DIM_GROUP.Remove(group);
                try
                {
                    var lstce = gscexie.ToList();
                    var lstcj = gschenjiang.ToList();
                    db.SaveChanges();
                    try
                    {
                        SendMsg2ET(lstce);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    try
                    {
                        SendMsg2ET(lstcj);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("传感器组删除成功"));

                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器组删除失败"));
                }
            }
        }

        /// <summary>
        /// 增加测斜传感器分组
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("新增测斜分组", true)]
        [Authorization(AuthorizationCode.S_Structure_SensorGroup_Modify)]
        public HttpResponseMessage AddGroupsCeXie([FromBody]SensorGroupCeXie model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var group = new T_DIM_GROUP();
                    group.GROUP_NAME = model.GroupName;
                    group.GROUP_TYPE_ID = 1;

                    var entry = db.Entry(group);
                    entry.State = System.Data.EntityState.Added;

                    var sensorIds = model.SensorList.Select(s => s.SensorId);
                    var sensors = db.T_DIM_SENSOR.Where(s => sensorIds.Contains(s.SENSOR_ID)).ToList();
                    StringBuilder sb = new StringBuilder();

                    foreach (var p in model.SensorList)
                    {
                        sb.AppendFormat(
                            "位置-{0}_深度-{1};",
                            sensors.Where(s => s.SENSOR_ID == p.SensorId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault(),
                            p.Depth);

                        var sensorGroupCeXie = new T_DIM_SENSOR_GROUP_CEXIE();
                        sensorGroupCeXie.GROUP_ID = group.GROUP_ID;
                        sensorGroupCeXie.SENSOR_ID = p.SensorId;
                        sensorGroupCeXie.DEPTH = p.Depth;

                        var test = db.Entry(sensorGroupCeXie);
                        test.State = System.Data.EntityState.Added;
                    }

                    #region 日志

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = string.Format("组:{0},传感器:{1}", model.GroupName, sb);
                    #endregion

                    db.SaveChanges();
                    int gid = group.GROUP_ID;
                    // 增加的组信息
                    var gs = (from s in db.T_DIM_SENSOR
                        from sg in sensorIds
                        from d in db.T_DIM_REMOTE_DTU
                        where s.SENSOR_ID == sg && d.ID == s.DTU_ID
                        select
                            new SensorGroup
                            {
                                GroupId = gid,
                                DtuId =  s.DTU_ID.Value,
                                DtuCode = d.REMOTE_DTU_NUMBER,
                                DacInterval =  d.REMOTE_DTU_GRANULARITY.Value
                            })
                        .Distinct();
                    try
                    {
                        SendMsg2ET(gs.ToList());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                   

                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("测斜传感器组新增成功"));
                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("测斜传感器组新增失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("测斜传感器组新增失败"));
                }
            }
        }

        /// <summary>
        /// 修改测斜传感器分组
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("修改测斜传感器分组", true)]
        [Authorization(AuthorizationCode.S_Structure_SensorGroup_Modify)]
        public HttpResponseMessage ModifyGroupCeXie([FromUri] int groupId, [FromBody]SensorGroupCeXie model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var group = (from s in db.T_DIM_GROUP
                                 where s.GROUP_ID == groupId
                                 select s
                                   ).FirstOrDefault();

                    if (group == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器组不存在"));
                    }

                    var sensorIds = model.SensorList.Select(s => s.SensorId);
                    var sensors = db.T_DIM_SENSOR.Where(s => sensorIds.Contains(s.SENSOR_ID)).ToList();
                    StringBuilder sb = new StringBuilder();

                    group.GROUP_NAME = model.GroupName;

                    // 修改前的传感器列表
                    var sensorExists = from s in db.T_DIM_SENSOR_GROUP_CEXIE
                                       where s.GROUP_ID == groupId
                                       select s;
                    // 修改前 组信息
                    var gsexist = (from s in db.T_DIM_SENSOR
                        from sg in sensorExists
                        from d in db.T_DIM_REMOTE_DTU
                        where s.SENSOR_ID == sg.SENSOR_ID && d.ID == s.DTU_ID
                        select
                            new SensorGroup
                            {
                                GroupId = groupId,
                                DtuId = s.DTU_ID.Value,
                                DtuCode = d.REMOTE_DTU_NUMBER,
                                DacInterval = d.REMOTE_DTU_GRANULARITY.Value
                            }).Distinct();
                   
                    // 删除旧的配置
                    foreach (var sensorExist in sensorExists)
                    {
                        db.T_DIM_SENSOR_GROUP_CEXIE.Remove(sensorExist);
                    }

                    // 修改后的传感器列表
                    var sensorModels = model.SensorList;
                    // 插入新的配置
                    foreach (var sensor in sensorModels)
                    {
                        sb.AppendFormat(
                            "位置-{0}_深度-{1};",
                            sensors.Where(s => s.SENSOR_ID == sensor.SensorId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault(),
                            sensor.Depth);

                        var sensorGroupCeXie = new T_DIM_SENSOR_GROUP_CEXIE();
                        sensorGroupCeXie.GROUP_ID = groupId;
                        sensorGroupCeXie.SENSOR_ID = sensor.SensorId;
                        sensorGroupCeXie.DEPTH = sensor.Depth;

                        var entry = db.Entry(sensorGroupCeXie);
                        entry.State = System.Data.EntityState.Added;
                    }

                    #region 日志

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = string.Format("组:{0},传感器:{1}", model.GroupName, sb);
                    #endregion

                    db.SaveChanges();

                    var gs = (from s in db.T_DIM_SENSOR
                        from sg in sensorIds
                        from d in db.T_DIM_REMOTE_DTU
                        where s.SENSOR_ID == sg && d.ID == s.DTU_ID
                        select
                            new SensorGroup
                            {
                                GroupId = groupId,
                                DtuId = s.DTU_ID.Value,
                                DtuCode = d.REMOTE_DTU_NUMBER,
                                DacInterval = d.REMOTE_DTU_GRANULARITY.Value
                            })
                        .Distinct();
                    try
                    {
                        SendMsg2ET(gs.ToList(), gsexist.ToList());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    

                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("测斜传感器组修改成功"));

                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("测斜传感器组修改失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("测斜传感器组修改失败"));
                }
            }
        }

        /// <summary>
        /// 增加沉降传感器分组
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("新增沉降传感器分组", true)]
        [Authorization(AuthorizationCode.S_Structure_SensorGroup_Modify)]
        public HttpResponseMessage AddGroupsChenJiang([FromBody]SensorGroupChenJiang model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var group = new T_DIM_GROUP();
                    group.GROUP_NAME = model.GroupName;
                    group.GROUP_TYPE_ID = 2;

                    var entry = db.Entry(group);
                    entry.State = System.Data.EntityState.Added;

                    var sensorIds = model.SensorList.Select(s => s.SensorId);
                    var sensors = db.T_DIM_SENSOR.Where(s => sensorIds.Contains(s.SENSOR_ID)).ToList();
                    StringBuilder sb = new StringBuilder();

                    foreach (var p in model.SensorList)
                    {
                        sb.AppendFormat(
                            "位置-{0}_长度-{2}_是否基准点-{1};",
                            sensors.Where(s => s.SENSOR_ID == p.SensorId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault(),
                            p.IsDatum,
                            p.Len);

                        var sensorGroupChenJiang = new T_DIM_SENSOR_GROUP_CHENJIANG();
                        sensorGroupChenJiang.GROUP_ID = group.GROUP_ID;
                        sensorGroupChenJiang.SENSOR_ID = p.SensorId;
                        sensorGroupChenJiang.isJIZHUNDIAN = p.IsDatum;
                        sensorGroupChenJiang.LENGTH = p.Len;

                        var test = db.Entry(sensorGroupChenJiang);
                        test.State = System.Data.EntityState.Added;
                    }

                    #region 日志

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = string.Format("组:{0},传感器:{1}", model.GroupName, sb);
                    #endregion

                    db.SaveChanges();
                    int gid = group.GROUP_ID;
                    // 增加的组信息
                    var gs = (from s in db.T_DIM_SENSOR
                              from sg in sensorIds
                        from d in db.T_DIM_REMOTE_DTU
                        where s.SENSOR_ID == sg && d.ID == s.DTU_ID
                        select
                            new SensorGroup
                            {
                                GroupId = gid,
                                DtuId = s.DTU_ID.Value,
                                DtuCode = d.REMOTE_DTU_NUMBER,
                                DacInterval = d.REMOTE_DTU_GRANULARITY.Value
                            })
                        .Distinct();
                    try
                    {
                        SendMsg2ET(gs.ToList());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    

                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("测斜传感器组新增成功"));
                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("测斜传感器组新增失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("测斜传感器组新增失败"));
                }
            }
        }

        /// <summary>
        /// 修改沉降传感器分组
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("修改沉降传感器分组", true)]
        [Authorization(AuthorizationCode.S_Structure_SensorGroup_Modify)]
        public HttpResponseMessage ModifyGroupChenJiang([FromUri] int groupId, [FromBody]SensorGroupChenJiang model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var group = (from s in db.T_DIM_GROUP
                                 where s.GROUP_ID == groupId
                                 select s
                                   ).FirstOrDefault();

                    if (group == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器组不存在"));
                    }

                    var sensorIds = model.SensorList.Select(s => s.SensorId);
                    var sensors = db.T_DIM_SENSOR.Where(s => sensorIds.Contains(s.SENSOR_ID)).ToList();
                    StringBuilder sb = new StringBuilder();

                    group.GROUP_NAME = model.GroupName;

                    // 修改前的传感器列表
                    var sensorExists = from s in db.T_DIM_SENSOR_GROUP_CHENJIANG
                                       where s.GROUP_ID == groupId
                                       select s;
                    // 修改前 组信息
                    var gsexist = (from s in db.T_DIM_SENSOR
                        from sg in sensorExists
                        from d in db.T_DIM_REMOTE_DTU
                        where s.SENSOR_ID == sg.SENSOR_ID && d.ID == s.DTU_ID
                        select
                            new SensorGroup
                            {
                                GroupId = groupId,
                                DtuId = s.DTU_ID.Value,
                                DtuCode = d.REMOTE_DTU_NUMBER,
                                DacInterval = d.REMOTE_DTU_GRANULARITY.Value
                            }).Distinct();
                   
                    // 删除旧的配置
                    foreach (var sensorExist in sensorExists)
                    {
                        db.T_DIM_SENSOR_GROUP_CHENJIANG.Remove(sensorExist);
                    }

                    // 修改后的传感器列表
                    var sensorModels = model.SensorList;
                    // 插入新的配置
                    foreach (var sensor in sensorModels)
                    {
                        sb.AppendFormat(
                            "位置-{0}_长度-{2}_是否基准点-{1};",
                            sensors.Where(s => s.SENSOR_ID == sensor.SensorId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault(),
                            sensor.IsDatum,
                            sensor.Len);

                        var sensorGroupCeXie = new T_DIM_SENSOR_GROUP_CHENJIANG();
                        sensorGroupCeXie.GROUP_ID = groupId;
                        sensorGroupCeXie.SENSOR_ID = sensor.SensorId;
                        sensorGroupCeXie.isJIZHUNDIAN = sensor.IsDatum;
                        sensorGroupCeXie.LENGTH = sensor.Len;

                        var entry = db.Entry(sensorGroupCeXie);
                        entry.State = System.Data.EntityState.Added;
                    }

                    #region 日志

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = string.Format("组:{0},传感器:{1}", model.GroupName, sb);
                    #endregion

                    db.SaveChanges();
                    // 修改后的组信息
                    var gs = (from s in db.T_DIM_SENSOR
                              from sg in sensorIds
                        from d in db.T_DIM_REMOTE_DTU
                        where s.SENSOR_ID == sg && d.ID == s.DTU_ID
                        select
                            new SensorGroup
                            {
                                GroupId = groupId,
                                DtuId = s.DTU_ID.Value,
                                DtuCode = d.REMOTE_DTU_NUMBER,
                                DacInterval = d.REMOTE_DTU_GRANULARITY.Value
                            }).Distinct();
                    try
                    {
                        SendMsg2ET(gs.ToList(), gsexist.ToList());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("测斜传感器组修改成功"));

                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("测斜传感器组修改失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("测斜传感器组修改失败"));
                }
            }
        }

        // 浸润线分组暂不通知et

        /// <summary>
        /// 增加浸润线传感器分组
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("新增浸润线传感器分组", true)]
        [Authorization(AuthorizationCode.S_Structure_SensorGroup_Modify)]
        public HttpResponseMessage AddGroupsJinRunXian([FromBody]SensorGroupJinRunXian model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var group = new T_DIM_GROUP();
                    group.GROUP_NAME = model.GroupName;
                    group.GROUP_TYPE_ID = 3;

                    var entry = db.Entry(group);
                    entry.State = System.Data.EntityState.Added;

                    var sensorIds = model.SensorList.Select(s => s.SensorId);
                    var sensors = db.T_DIM_SENSOR.Where(s => sensorIds.Contains(s.SENSOR_ID)).ToList();
                    StringBuilder sb = new StringBuilder();

                    foreach (var sensor in model.SensorList)
                    {
                        sb.AppendFormat(
                            "位置-{0}_高度-{1};",
                            sensors.Where(s => s.SENSOR_ID == sensor.SensorId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault(),
                            sensor.Height);

                        var sensorGroupJinRunXian = new T_DIM_SENSOR_GROUP_JINRUNXIAN();
                        sensorGroupJinRunXian.GROUP_ID = group.GROUP_ID;
                        sensorGroupJinRunXian.SENSOR_ID = sensor.SensorId;
                        sensorGroupJinRunXian.HEIGHT = sensor.Height;

                        var test = db.Entry(sensorGroupJinRunXian);
                        test.State = System.Data.EntityState.Added;
                    }

                    #region 日志

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = string.Format("组:{0},传感器:{1}", model.GroupName, sb);
                    #endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("测斜传感器组新增成功"));
                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("测斜传感器组新增失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("测斜传感器组新增失败"));
                }
            }
        }

        /// <summary>
        /// 修改浸润线传感器分组
        /// </summary>
        [AcceptVerbs("Post")]
        [LogInfo("修改浸润线传感器分组", true)]
        [Authorization(AuthorizationCode.S_Structure_SensorGroup_Modify)]
        public HttpResponseMessage ModifyGroupJinRunXian([FromUri] int groupId, [FromBody]SensorGroupJinRunXian model)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var group = (from s in db.T_DIM_GROUP
                                 where s.GROUP_ID == groupId
                                 select s
                                   ).FirstOrDefault();

                    if (group == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("传感器组不存在"));
                    }

                    var sensorIds = model.SensorList.Select(s => s.SensorId);
                    var sensors = db.T_DIM_SENSOR.Where(s => sensorIds.Contains(s.SENSOR_ID)).ToList();
                    StringBuilder sb = new StringBuilder();

                    group.GROUP_NAME = model.GroupName;

                    // 修改前的传感器列表
                    var sensorExists = from s in db.T_DIM_SENSOR_GROUP_JINRUNXIAN
                                       where s.GROUP_ID == groupId
                                       select s;
                    // 删除旧的配置
                    foreach (var sensorExist in sensorExists)
                    {
                        db.T_DIM_SENSOR_GROUP_JINRUNXIAN.Remove(sensorExist);
                    }

                    // 修改后的传感器列表
                    var sensorModels = model.SensorList;
                    // 插入新的配置
                    foreach (var sensor in sensorModels)
                    {
                        sb.AppendFormat(
                            "位置-{0}_是否基准点-{1};",
                            sensors.Where(s => s.SENSOR_ID == sensor.SensorId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault(),
                            sensor.Height);

                        var sensorGroupJinRunXian = new T_DIM_SENSOR_GROUP_JINRUNXIAN();
                        sensorGroupJinRunXian.GROUP_ID = groupId;
                        sensorGroupJinRunXian.SENSOR_ID = sensor.SensorId;
                        sensorGroupJinRunXian.HEIGHT = sensor.Height;

                        var entry = db.Entry(sensorGroupJinRunXian);
                        entry.State = System.Data.EntityState.Added;
                    }

                    #region 日志

                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                    this.Request.Properties["ActionParameterShow"] = string.Format("组:{0},传感器:{1}", model.GroupName, sb);
                    #endregion

                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("测斜传感器组修改成功"));

                }
                catch (NullReferenceException e)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("测斜传感器组修改失败:参数无效"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("测斜传感器组修改失败"));
                }
            }
        }

        private bool IsEqual(IList<SensorGroup> gp1, IList<SensorGroup> gp2)
        {
            bool isequal = true;
            foreach (var v in gp1)
            {
                if (gp2.Count(g => g.DtuId == v.DtuId) < 0)
                {
                    isequal = false;
                }
            }
            if (isequal)
            {
                foreach (var v in gp2)
                {
                    if (gp1.Count(g => g.DtuId == v.DtuId) < 0)
                    {
                        isequal = false;
                    }
                }
            }
            return isequal;
        }

        private void SendMsg2ET(IList<SensorGroup> newgp, IList<SensorGroup> oldgp)
        {
            try
            {
                if (newgp.Count() > 1 || oldgp.Count() > 1)
                {
                    if (newgp.Count() != oldgp.Count() || !IsEqual(newgp, oldgp))
                    {
                        // TODO

                        WebClientService.SendToET(ConfigChangedMsgHelper.GetSensorGroupMessage());
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SendMsg2ET error {0}", ex);
            }
        }

        private void SendMsg2ET(IList<SensorGroup> gp)
        {
            try
            {
                if (gp.Count() > 1)
                {
                    // TODO

                    WebClientService.SendToET(ConfigChangedMsgHelper.GetSensorGroupMessage());
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SendMsg2ET error {0}", ex);
            }
        }
    }

    public class GroupType
    {
        [JsonProperty("groupTypeId")]
        public int GroupTypeId { get; set; }

        [JsonProperty("groupTypeName")]
        public string GroupTypeName { get; set; }
    }

    public class SensorGroupCeXie
    {
        public string GroupName { get; set; }
        public IList<SensorCeXie> SensorList { get; set; }
    }

    public class SensorCeXie
    {
        public int SensorId { get; set; }
        public decimal Depth { get; set; }
    }

    public class SensorGroupChenJiang
    {
        public string GroupName { get; set; }
        public IList<SensorChenJiang> SensorList { get; set; }
    }

    public class SensorChenJiang
    {
        public int SensorId { get; set; }
        public int Len { get; set; }
        public bool IsDatum { get; set; }
    }

    public class SensorGroupJinRunXian
    {
        public string GroupName { get; set; }
        public IList<SensorJinRunXian> SensorList { get; set; }
    }

    public class SensorJinRunXian
    {
        public int SensorId { get; set; }
        public decimal Height { get; set; }
    }
}
