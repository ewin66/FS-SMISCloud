using NetMQ.zmq;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Warning.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL.Alarm;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Push;

    using log4net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class WarningController : ApiController
    {
        // 告警确认状态
        private const int WarningSupportUnprocessed = 1;
        private const int WarningSupportProcessed = 2;
        private const int WarningClientUnprocessed = 3;
        private const int WarningClientProcessed = 4;
        // 告警通知(以邮件/短信方式)状态
        private const int WarningClientToInformed = 4;
        private const int WarningClientInformedButNoneRecipient = 5;
        private const int WarningClientInformedAndRecipient = 6;

        private DAL.Warning warningDal = new DAL.Warning();
        //private string apiurl = "http://cloudapi.free-sun.com.cn:8008"; //发布接口
        private string apiurl = "http://192.168.1.30:8002"; // 测试接口

        private WarningServer pushServer = new WarningServer();

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 获取用户告警数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户告警数量", false)]
        public object GetWarningCountByUser(int userId, string status, DateTime startDate, DateTime endDate)
        {
            int roleId = 5;
            if (Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
            }

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query =
                    from u in entity.T_DIM_USER
                    from us in entity.T_DIM_USER_STRUCTURE
                    from s in entity.T_DIM_STRUCTURE
                    where u.USER_NO == us.USER_NO
                          && us.STRUCTURE_ID == s.ID
                          && u.USER_NO == userId
                          && s.IsDelete == 0
                    select s;
                var list = query.ToList().Select(s => new
                {
                    structId = s.ID,
                    structName = s.STRUCTURE_NAME_CN,
                    warnings = new List<Warning>()
                }).ToList();

                int? dealFlag = null;
                switch (status)
                {
                    case "unprocessed":
                        dealFlag = roleId == 5 ? WarningClientUnprocessed : WarningSupportUnprocessed;
                        break;
                    case "processed":
                        dealFlag = roleId == 5 ? WarningClientProcessed : WarningSupportProcessed;
                        break;
                }

                if (list.Count == 0)
                {
                    return new JObject(new JProperty("count", 0));
                }

                int count = this.warningDal.GetWarningsCountByStruct(
                    list.Select(s => s.structId).ToArray(),
                    startDate,
                    endDate,
                    dealFlag,
                    roleId);

                return new JObject(new JProperty("count", count));
            }
        }

        /// <summary>
        /// 获取结构物告警数量
        /// </summary>
        /// <param name="structs"></param>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物告警数量", false)]
        [Authorization(AuthorizationCode.S_Warn)]
        [Authorization(AuthorizationCode.U_Common)]
        public object GetWarningCountByStruct(string structs, string status, DateTime startDate, DateTime endDate)
        {
            int roleId = 5;
            if (Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
            }

            string[] stcs = structs.Split(',');
            var strcNums = stcs.Select(s => Convert.ToInt32(s)).ToArray();

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_STRUCTURE where strcNums.Contains(s.ID) && s.IsDelete == 0 select s;
                var list =
                    query.ToList()
                        .Select(
                            s =>
                            new { structId = s.ID, structName = s.STRUCTURE_NAME_CN, warnings = new List<Warning>() })
                        .ToList();

                int? dealFlag = null;
                switch (status)
                {
                    case "unprocessed":
                        dealFlag = roleId == 5 ? WarningClientUnprocessed : WarningSupportUnprocessed;
                        break;
                    case "processed":
                        dealFlag = roleId == 5 ? WarningClientProcessed : WarningSupportProcessed;
                        break;
                }

                int count = this.warningDal.GetWarningsCountByStruct(
                    strcNums,
                    startDate,
                    endDate,
                    dealFlag,
                    roleId);

                return new JObject(new JProperty("count", count));
            }
        }

        /// <summary>
        /// 获取传感器告警数量
        /// </summary>
        /// <param name="sensors"></param>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器告警数量", false)]
        public object GetWarningCountBySensor(string sensors, string status, DateTime startDate, DateTime endDate)
        {
            int roleId = 5;
            if (Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
            }

            string[] stcs = sensors.Split(',');
            var strcNums = stcs.Select(s => Convert.ToInt32(s)).ToArray();

            int? dealFlag = null;
            switch (status)
            {
                case "unprocessed":
                    dealFlag = roleId == 5 ? WarningClientUnprocessed : WarningSupportUnprocessed;
                    break;
                case "processed":
                    dealFlag = roleId == 5 ? WarningClientProcessed : WarningSupportProcessed;
                    break;
            }

            int count = warningDal.GetWarningsCountBySensor(strcNums, startDate, endDate, dealFlag, roleId);

            return new JObject(new JProperty("count", count));
        }

        /// <summary>
        /// Get user/{userId}/warnings/{status}/{startDate}/{endDate}
        /// </summary>
        /// <param name="userId"> 用户编号（只能数字组成）</param>
        /// <param name="status"> 告警状态（(all)|(processed)|(unprocessed)） </param>
        /// <param name="startDate"> 开始时间 </param>
        /// <param name="endDate"> 结束时间 </param>
        /// <param name="startRow">开始行数</param>
        /// <param name="endRow">结束行数</param>
        /// <returns> 告警列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户告警", false)]
        public object FindWarningsByUser(int userId, string status, DateTime startDate, DateTime endDate, int? startRow = null, int? endRow = null)
        {
            int roleId = 5;
            if (Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
            }

            int? dealFlag = null;
            if (roleId == 5)
            {
                switch (status)
                {
                    case "unprocessed":
                        dealFlag = WarningClientUnprocessed; // unprocessed for Client.
                        break;
                    case "processed":
                        dealFlag = WarningClientProcessed; // processed for Client.
                        break;
                }
            }
            else
            {
                switch (status)
                {
                    case "unprocessed":
                        dealFlag = WarningSupportUnprocessed; // unprocessed for Support.
                        break;
                    case "processed":
                        dealFlag = WarningSupportProcessed; // processed for Support.
                        break;
                }
            }

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query =
                    from u in entity.T_DIM_USER
                    from us in entity.T_DIM_USER_STRUCTURE
                    from s in entity.T_DIM_STRUCTURE
                    where u.USER_NO == us.USER_NO
                          && us.STRUCTURE_ID == s.ID
                          && u.USER_NO == userId
                          && s.IsDelete == 0
                    select s;
                var list = query.ToList().Select(s => new
                {
                    structId = s.ID,
                    structName = s.STRUCTURE_NAME_CN,
                    warnings = new List<Warning>()
                }).ToList();

                if (list.Count == 0)
                {
                    return new JArray();
                }

                DataTable dt = null;

                if (startRow != null && endRow != null)
                {
                    dt = warningDal.GetPagedWarningsByStruct(
                        list.Select(s => s.structId).ToArray(),
                        startDate,
                        endDate,
                        dealFlag,
                        (int)startRow,
                        (int)endRow,
                        roleId);
                }
                else
                {
                    dt = this.warningDal.GetWarningsByStruct(
                        list.Select(s => s.structId).ToArray(),
                        startDate,
                        endDate,
                        dealFlag,
                        roleId);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    foreach (var dataRow in dt.AsEnumerable())
                    {
                        if (dataRow.Field<int>("StructId") == list[i].structId)
                        {
                            list[i].warnings.Add(new Warning
                            {
                                WarningId = dataRow.Field<int>("Id"),
                                WarningTypeId = dataRow.Field<string>("WarningTypeId"),
                                Source = dataRow.Field<string>("Source"),
                                Level = dataRow["WarningLevel"] == DBNull.Value ? 4 : dataRow.Field<byte>("WarningLevel"),
                                Content = dataRow.Field<string>("Content"),
                                Reason = dataRow.Field<string>("Reason"),
                                Time = dataRow.Field<DateTime>("Time"),
                                DealFlag = dataRow.Field<int>("DealFlag"),
                                Confirmor = dataRow["Confirmor"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Confirmor"),
                                Suggestion = dataRow["Suggestion"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Suggestion"),
                                ConfirmTime = dataRow["ConfirmTime"] == DBNull.Value ? default(DateTime) : dataRow.Field<DateTime>("ConfirmTime")
                            });
                        }
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// 获取结构物下"全部/已确认/未确认/已下发"告警内容
        /// </summary>
        /// <param name="structs"> 结构物编号数组（只能是一个或多个数字） </param>
        /// <param name="status"> 告警状态（all,disposed,undisposed） </param>
        /// <param name="startDate"> 开始时间 </param>
        /// <param name="endDate"> 结束时间 </param>
        /// <param name="startRow">开始行数</param>
        /// <param name="endRow">结束行数</param>
        /// <returns> 告警列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物告警", false)]
        [Authorization(AuthorizationCode.S_Warn)]
        [Authorization(AuthorizationCode.U_Common)]
        public object FindWarningsByStruct(string structs, string status, DateTime startDate, DateTime endDate, int? startRow = null, int? endRow = null)
        {
            int roleId = 5; // 5-"普通用户"角色(Client).
            if (Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
            }

            int? dealFlag = null;
            if (roleId == 5)
            {
                switch (status)
                {
                    case "unprocessed":
                        dealFlag = WarningClientUnprocessed; // unprocessed for Client.
                        break;
                    case "processed":
                        dealFlag = WarningClientProcessed; // processed for Client.
                        break;
                }
            }
            else
            {
                switch (status)
                {
                    case "unprocessed":
                        dealFlag = WarningSupportUnprocessed; // unprocessed for Support.
                        break;
                    case "processed":
                        dealFlag = WarningSupportProcessed; // processed for Support.
                        break;
                    case "issued": // "已下发"告警
                        dealFlag = 5; // unprocessed and processed for Client.
                        break;
                }
            }

            string[] stcs = structs.Split(',');
            var strcNums = stcs.Select(s => Convert.ToInt32(s)).ToArray();

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_STRUCTURE
                            where strcNums.Contains(s.ID) && s.IsDelete == 0
                            select s;
                var list = query.ToList().Select(s => new
                {
                    structId = s.ID,
                    structName = s.STRUCTURE_NAME_CN,
                    warnings = new List<Warning>()
                }).ToList();

                DataTable dt = null;

                if (startRow != null && endRow != null)
                {
                    dt = warningDal.GetPagedWarningsByStruct(strcNums, startDate, endDate, dealFlag, (int)startRow, (int)endRow, roleId);
                }
                else
                {
                    dt = warningDal.GetWarningsByStruct(strcNums, startDate, endDate, dealFlag, roleId);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    foreach (var dataRow in dt.AsEnumerable())
                    {
                        if (dataRow.Field<int>("StructId") == list[i].structId)
                        {
                            list[i].warnings.Add(new Warning
                            {
                                WarningId = dataRow.Field<int>("Id"),
                                WarningTypeId = dataRow.Field<string>("WarningTypeId"),
                                Source = dataRow.Field<string>("Source"),
                                Level = dataRow["WarningLevel"] == DBNull.Value ? 4 : dataRow.Field<byte>("WarningLevel"),
                                Content = dataRow.Field<string>("Content"),
                                Reason = dataRow.Field<string>("Reason"),
                                Time = dataRow.Field<DateTime>("Time"),
                                DealFlag = dataRow.Field<int>("DealFlag"),
                                Confirmor = dataRow["Confirmor"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Confirmor"),
                                Suggestion = dataRow["Suggestion"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Suggestion"),
                                ConfirmTime = dataRow["ConfirmTime"] == DBNull.Value ? default(DateTime) : dataRow.Field<DateTime>("ConfirmTime")
                            });
                        }
                    }
                }

                return list;
            }
        }
        /// <summary>
        /// 获取结构物下过滤及排序后的告警数目 
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="alarm_1"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [Authorization(AuthorizationCode.S_Warn)]
        [Authorization(AuthorizationCode.U_Common)]
        public object FindFilteredOrderedAlarmsByStructCount([FromUri] int structId, [FromBody] AlarmModel_1 alarm_1)
        {
            try
            {
                int count = 0;
                // 分解告警等级
                string[] levels = alarm_1.FilteredLevel.Split(',');
                var levelList = levels.Select(s => Convert.ToInt32(s)).ToList();
                AlarmModel alarm = new AlarmModel
                {
                    FilteredDeviceType = alarm_1.FilteredDeviceType,
                    FilteredStatus = alarm_1.FilteredStatus,
                    FilteredLevel = levelList,
                    FilteredStartTime = alarm_1.FilteredStartTime,
                    FilteredEndTime = alarm_1.FilteredEndTime,
                    OrderedDevice = alarm_1.OrderedDevice,
                    OrderedLevel = alarm_1.OrderedLevel,
                    OrderedTime = alarm_1.OrderedTime
                };
                int roleId = 5; // 5-"普通用户"角色(Client).
                if (Request.Properties.ContainsKey("AuthorizationInfo"))
                {
                    var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                    roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
                }

                using (SecureCloud_Entities entity = new SecureCloud_Entities())
                {
                    var objAlarm = new Alarm();
                    var condition = objAlarm.GetFilteredOrderedConditionOfAlarm(alarm, roleId);

                    DataTable dt = objAlarm.GetFilteredOrderedAlarmsByStruct(structId, condition);
                        foreach (var dataRow in dt.AsEnumerable())
                        {
                            if (dataRow.Field<int>("StructId") == structId)
                            {
                                count++;
                            }
                        }
                        return new AlarmCount { Count = count };
                }
            }
            catch (Exception e)
            {
                return new AlarmCount { Count = 0 };
                //throw e;
            }
        }

        /// <summary>
        /// 获取结构物下过滤及排序后的告警内容
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="alarm"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [Authorization(AuthorizationCode.S_Warn)]
        [Authorization(AuthorizationCode.U_Common)]
        public StructWarn FindFilteredOrderedAlarmsByStruct([FromUri]int structId, [FromBody]AlarmModel_1 alarm_1)
        {
            try
            {
                // 分解告警等级
                string[] levels = alarm_1.FilteredLevel.Split(',');
                var levelList = levels.Select(s => Convert.ToInt32(s)).ToList();
                AlarmModel alarm = new AlarmModel
                {
                    FilteredDeviceType = alarm_1.FilteredDeviceType,
                    FilteredStatus = alarm_1.FilteredStatus,
                    FilteredLevel = levelList,
                    FilteredStartTime = alarm_1.FilteredStartTime,
                    FilteredEndTime = alarm_1.FilteredEndTime,
                    OrderedDevice = alarm_1.OrderedDevice,
                    OrderedLevel = alarm_1.OrderedLevel,
                    OrderedTime = alarm_1.OrderedTime
                };
                int roleId = 5; // 5-"普通用户"角色(Client).
                if (Request.Properties.ContainsKey("AuthorizationInfo"))
                {
                    var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                    roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
                }

                using (SecureCloud_Entities entity = new SecureCloud_Entities())
                {
                    var query = from s in entity.T_DIM_STRUCTURE
                                where s.ID == structId && s.IsDelete == 0
                                select s;
                    var list = query.ToList().Select(s => new StructWarn
                    {
                        structId = s.ID,
                        structName = s.STRUCTURE_NAME_CN,
                        warnings = new List<Warning>()
                    }).ToList();

                    var objAlarm = new Alarm();
                    var condition = objAlarm.GetFilteredOrderedConditionOfAlarm(alarm, roleId);

                    DataTable dt = null;
                    int start = 0, end = 0;
                    if (this.Request.GetQueryString("amp;startRow") != null && this.Request.GetQueryString("amp;endRow") != null)
                    {
                        start = int.Parse(this.Request.GetQueryString("amp;startRow"));
                        end = int.Parse(this.Request.GetQueryString("amp;endRow"));
                        dt = objAlarm.GetPagedFilteredOrderedAlarmsByStruct(structId, condition, start, end);
                    }
                    else
                    {
                        dt = objAlarm.GetFilteredOrderedAlarmsByStruct(structId, condition);
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        foreach (var dataRow in dt.AsEnumerable())
                        {
                            if (dataRow.Field<int>("StructId") == list[i].structId)
                            {
                                list[i].warnings.Add(new Warning
                                {
                                    WarningId = dataRow.Field<int>("Id"),
                                    WarningTypeId = dataRow.Field<string>("WarningTypeId"),
                                    Source = dataRow.Field<string>("Source"),
                                    Level = dataRow["WarningLevel"] == DBNull.Value ? 4 : dataRow.Field<byte>("WarningLevel"),
                                    Content = dataRow.Field<string>("Content"),
                                    Reason = dataRow.Field<string>("Reason"),
                                    Time = dataRow.Field<DateTime>("Time"),
                                    DealFlag = dataRow.Field<int>("DealFlag"),
                                    Confirmor = dataRow["Confirmor"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Confirmor"),
                                    Suggestion = dataRow["Suggestion"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Suggestion"),
                                    ConfirmTime = dataRow["ConfirmTime"] == DBNull.Value ? default(DateTime) : dataRow.Field<DateTime>("ConfirmTime")
                                });
                            }
                        }
                    }
                    return list.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                return new StructWarn();
                //throw e;
            }

        }

        /// <summary>
        /// Get sensor/{sensorId}/warnings/{status}/{startDate}/{endDate}
        /// </summary>
        /// <param name="sensors"> 传感器数组 </param>
        /// <param name="status"> 告警状态（(all)|(processed)|(unprocessed)） </param>
        /// <param name="startDate"> 开始时间 </param>
        /// <param name="endDate"> 结束时间 </param>
        /// <param name="startRow">开始行数</param>
        /// <param name="endRow">结束行数</param>
        /// <returns> 告警列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取传感器告警", false)]
        public object FindWarningsBySensor(string sensors, string status, DateTime startDate, DateTime endDate, int? startRow = null, int? endRow = null)
        {
            int roleId = 5;
            if (Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
            }

            string[] stcs = sensors.Split(',');
            var strcNums = stcs.Select(s => Convert.ToInt32(s)).ToArray();

            int? dealFlag = null;
            switch (status)
            {
                case "unprocessed":
                    dealFlag = roleId == 5 ? WarningClientUnprocessed : WarningSupportUnprocessed;
                    break;
                case "processed":
                    dealFlag = roleId == 5 ? WarningClientProcessed : WarningSupportProcessed;
                    break;
            }

            DataTable dt = null;

            if (startRow != null && endRow != null)
            {
                dt = warningDal.GetPagedWarningsBySensor(strcNums, startDate, endDate, dealFlag, (int)startRow, (int)endRow, roleId);
            }
            else
            {
                dt = warningDal.GetWarningsBySensor(strcNums, startDate, endDate, dealFlag, roleId);
            }

            return
                new JArray(
                    dt.AsEnumerable()
                        .Select(
                            dataRow =>
                            new JObject(
                                new JProperty("warningId", dataRow.Field<int>("Id")),
                                new JProperty("warningTypeId", dataRow.Field<string>("WarningTypeId")),
                                new JProperty("location", dataRow.Field<string>("Location")),
                                new JProperty("productTypeId", dataRow.Field<string>("ProductTypeId")),
                                new JProperty("productName", dataRow.Field<string>("ProductName")),
                                new JProperty("level", dataRow["WarningLevel"] == DBNull.Value ? 4 : dataRow.Field<byte>("WarningLevel")),
                                new JProperty("content", dataRow.Field<string>("Content")),
                                new JProperty("reason", dataRow.Field<string>("Reason")),
                                new JProperty("time", dataRow.Field<DateTime>("Time")),
                                new JProperty("dealFlag", dataRow.Field<int>("DealFlag")),
                                new JProperty(
                                    "confirmor",
                                    dataRow["Confirmor"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Confirmor")),
                                new JProperty(
                                    "suggestion",
                                    dataRow["Suggestion"] == DBNull.Value ? string.Empty : dataRow.Field<string>("Suggestion")),
                                new JProperty(
                                    "confirmTime",
                                    dataRow["ConfirmTime"] == DBNull.Value ? default(DateTime) : dataRow.Field<DateTime>("ConfirmTime")))));
        }

        /// <summary>
        /// 告警数量统计 struct/{structs}/warn-number/{status}/{startDate}/{endDate}
        /// </summary>
        /// <param name="structs"></param>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物告警数量统计", false)]
        public IEnumerable<WarningStats> GetWarningStatus(string structs, string status, DateTime startDate, DateTime endDate)
        {
            int roleId = 5;
            if (Request.Properties.ContainsKey("AuthorizationInfo"))
            {
                var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
            }

            string[] stcs = structs.Split(',');
            var strcNums = stcs.Select(s => Convert.ToInt32(s)).ToArray();

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from w in entity.T_WARNING_SENSOR
                            where strcNums.Contains(w.StructId)
                                && w.Time >= startDate && w.Time <= endDate
                            select w;

                switch (status)
                {
                    case "processed":
                        if (roleId == 5)
                        {
                            query = from w in query
                                    where w.DealFlag == WarningClientProcessed
                                    select w;
                        }
                        else
                        {
                            query = from w in query
                                    where w.DealFlag == WarningSupportProcessed
                                    select w;
                        }
                        break;
                    case "unprocessed":
                        if (roleId == 5)
                        {
                            query = from w in query
                                    where w.DealFlag == WarningClientUnprocessed
                                    select w;
                        }
                        else
                        {
                            query = from w in query
                                    where w.DealFlag == WarningSupportUnprocessed
                                    select w;
                        }
                        break;
                }

                var q = from w in query
                        from wt in entity.T_DIM_WARNING_TYPE
                        where w.WarningTypeId == wt.TypeId
                        select new
                                   {
                                       w.StructId,
                                       Level = (int)(wt.WarningLevel ?? 4)
                                   };

                var grp = from w in q
                          group w by new { w.StructId, w.Level }
                              into g
                              select new
                              {
                                  StructId = g.Key.StructId,
                                  Level = g.Key.Level,
                                  Number = g.Count()
                              };

                var list = grp.ToList();

                IList<WarningStats> rslt = new List<WarningStats>();
                foreach (var stc in strcNums)
                {
                    var warn = new WarningStats { StructId = stc, Stats = new List<Stats>() };

                    foreach (var item in list)
                    {
                        if (item.StructId == stc)
                        {
                            warn.Stats.Add(new Stats { Level = item.Level, Number = item.Number });
                        }
                    }

                    rslt.Add(warn);
                }

                return rslt;
            }
        }

        /// <summary>
        /// 下发告警 POST warnings/distribute/{warnids}
        /// </summary>
        /// <param name="warnIds"> 告警id数组 </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/> 是否成功 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("下发告警", true)]
        [Authorization(AuthorizationCode.S_Warn_Post)]
        public HttpResponseMessage DistributeWarnings([FromUri] string warnIds)
        {
            int[] warnArray = warnIds.Split(',').Select(s => Convert.ToInt32(s)).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("共{0}条:", warnArray.Length);

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                foreach (var warnId in warnArray)
                {
                    var warn = entity.T_WARNING_SENSOR.FirstOrDefault(w => w.Id == warnId);
                    if (warn == null
                        || warn.WarningStatus == WarningClientToInformed
                        || warn.WarningStatus == WarningClientInformedButNoneRecipient
                        || warn.WarningStatus == WarningClientInformedAndRecipient)
                    {
                        return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("告警已被下发!"));
                    }

                    warn.WarningStatus = WarningClientToInformed; // update WarningStatus to WarningClientToInformed.
                    warn.DealFlag = WarningClientUnprocessed; // update DealFlag to WarningClientUnprocessed.

                    #region 日志信息
                    var stc =
                                    entity.T_DIM_STRUCTURE.Where(s => s.ID == warn.StructId)
                                        .Select(s => s.STRUCTURE_NAME_CN)
                                        .FirstOrDefault();
                    string type = warn.DeviceTypeId == 1 ? "dtu" : "传感器";
                    string name = string.Empty;
                    if (warn.DeviceTypeId == 1)
                    {
                        var dtuId = warn.DeviceId.ToString();
                        name =
                            entity.T_DIM_REMOTE_DTU.Where(
                                d => d.ID == warn.DeviceId || d.REMOTE_DTU_NUMBER == dtuId)
                                .Select(d => d.REMOTE_DTU_NUMBER)
                                .FirstOrDefault();
                    }
                    else
                    {
                        name =
                            entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == warn.DeviceId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault();
                    }
                    sb.AppendFormat("{0}.{1}:{2}-{3};", stc, type, name, warn.Content);
                    #endregion

                    // 推送告警
                    Task t = new Task(
                        () =>
                        {
                            pushServer.PushWarn(WarningServer.BuildWarningInfo(warnId));
                        });
                    t.Start();
                    t.ContinueWith(
                        task =>
                        {
                            if (task.Exception != null)
                            {
                                log.Error("告警推送失败，告警编号：" + warnId + "，时间：" + DateTime.Now);
                            }
                        });
                }

                #region 日志信息

                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("下发成功！"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("下发失败!"));
                }
            }
        }

        /// <summary>
        /// 确认告警 POST warnings/confirm/{warnids}
        /// </summary>
        /// <param name="warnIds"> 告警id数组 </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/> 是否成功 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("确认告警", true)]
        [Authorization(AuthorizationCode.S_Warn_Confirm)]
        [Authorization(AuthorizationCode.U_Common)]
        public HttpResponseMessage ConfirmWarnings([FromUri] string warnIds, [FromBody] ConfirmModel model)
        {
            int[] warnArray = warnIds.Split(',').Select(s => Convert.ToInt32(s)).ToArray();

            if (model == null)
            {
                return Request.CreateResponse(
                    System.Net.HttpStatusCode.BadRequest,
                    StringHelper.GetMessageString("参数无效,缺少确认信息"));
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("共{0}条:", warnArray.Length);

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                foreach (var warnId in warnArray)
                {
                    var warn = entity.T_WARNING_SENSOR.FirstOrDefault(w => w.Id == warnId);
                    if (warn.DealFlag == WarningSupportProcessed || warn.DealFlag == WarningClientProcessed)
                    {
                        return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("告警已被处理!"));
                    }

                    int roleId = 5;
                    if (Request.Properties.ContainsKey("AuthorizationInfo"))
                    {
                        var info = Request.Properties["AuthorizationInfo"] as AuthorizationInfo;
                        roleId = info != null && info.RoleId != null ? (int)info.RoleId : 5;
                    }

                    warn.DealFlag = roleId == 5 ? WarningClientProcessed : WarningSupportProcessed; // update DealFlag for Client or Support.

                    var entry1 = entity.Entry(warn);
                    entry1.State = EntityState.Modified;

                    #region 日志信息
                    var stc =
                                    entity.T_DIM_STRUCTURE.Where(s => s.ID == warn.StructId)
                                        .Select(s => s.STRUCTURE_NAME_CN)
                                        .FirstOrDefault();
                    string type = warn.DeviceTypeId == 1 ? "dtu" : "传感器";
                    string name = string.Empty;
                    if (warn.DeviceTypeId == 1)
                    {
                        string dtuId = warn.DeviceId.ToString();
                        name =
                            entity.T_DIM_REMOTE_DTU.Where(
                                d => d.ID == warn.DeviceId || d.REMOTE_DTU_NUMBER == dtuId)
                                .Select(d => d.REMOTE_DTU_NUMBER)
                                .FirstOrDefault();
                    }
                    else
                    {
                        name =
                            entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == warn.DeviceId)
                                .Select(s => s.SENSOR_LOCATION_DESCRIPTION)
                                .FirstOrDefault();
                    }
                    sb.AppendFormat("{0}.{1}:{2}-{3};", stc, type, name, warn.Content);
                    #endregion

                    var dealDetails = new T_WARNING_DEALDETAILS();
                    dealDetails.WarningId = warnId;
                    dealDetails.UserNo = model.Confirmor;
                    dealDetails.Suggestion = model.Suggestion;
                    dealDetails.ConfirmTime = DateTime.Now;

                    DbEntityEntry<T_WARNING_DEALDETAILS> entry2 = entity.Entry<T_WARNING_DEALDETAILS>(dealDetails);
                    entry2.State = EntityState.Added;
                }

                #region 日志信息
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("处理成功！"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("处理失败!"));
                }
            }
        }
    }

    public class ConfirmModel
    {
        public int Confirmor { get; set; }

        public string Suggestion { get; set; }
    }

    public class StructWarn
    {
        public int structId { get; set; }

        public string structName { get; set; }

        public List<Warning> warnings { get; set; }
    }

    public class AlarmCount
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
    public class Warning
    {
        [JsonProperty("warningId")]
        public int WarningId { get; set; }

        [JsonProperty("warningTypeId")]
        public string WarningTypeId { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("time")]
        public DateTime? Time { get; set; }

        [JsonProperty("dealFlag")]
        public int DealFlag { get; set; }

        [JsonProperty("confirmor")]
        public string Confirmor { get; set; }

        [JsonProperty("suggestion")]
        public string Suggestion { get; set; }

        [JsonProperty("confirmTime")]
        public DateTime? ConfirmTime { get; set; }
    }

    public class WarningStats
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("stats")]
        public IList<Stats> Stats { get; set; }
    }

    public class Stats
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }
    }
}