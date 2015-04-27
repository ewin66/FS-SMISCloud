namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL.Alarm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    public class Alarm 
    {
        /// <summary>
        /// 获取告警的过滤及排序条件
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        public AlarmFilteredOrderedCondition GetFilteredOrderedConditionOfAlarm(AlarmModel alarm, int roleId)
        {
            var listDeviceTypeId = new List<int>();
            switch (alarm.FilteredDeviceType)
            {
                case "dtu":
                    listDeviceTypeId.Add(1);
                    break;
                case "sensor":
                    listDeviceTypeId.Add(2);
                    break;
                default: // 默认值: "all"
                    listDeviceTypeId.Add(1);
                    listDeviceTypeId.Add(2);
                    break;
            }

            var listDealFlag = new List<int>();
            if (roleId == 5) // 5-"普通用户"角色(Client).
            {
                switch (alarm.FilteredStatus)
                {
                    case "all": // 全部, unprocessed and processed for Client.
                        listDealFlag.Add(3);
                        listDealFlag.Add(4);
                        break;
                    case "processed": // processed for Client.
                        listDealFlag.Add(4);
                        break;
                    default: // 默认值: "unprocessed", unprocessed for Client.
                        listDealFlag.Add(3);
                        break;
                }
            }
            else
            {
                switch (alarm.FilteredStatus)
                {
                    case "all": // 全部, unprocessed and processed and issued for Support.
                        listDealFlag.Add(1);
                        listDealFlag.Add(2);
                        listDealFlag.Add(3);
                        listDealFlag.Add(4);
                        break;
                    case "processed":
                        listDealFlag.Add(2);
                        break;
                    case "issued":
                        listDealFlag.Add(3);
                        listDealFlag.Add(4);
                        break;
                    default: // 默认值: "unprocessed"
                        listDealFlag.Add(1);
                        break;
                }
            }

            var listLevel = new List<int>();
            if (alarm.FilteredLevel.Count == 0) // 所有告警等级: [1,2,3,4]
            {
                listLevel.Add(1);
                listLevel.Add(2);
                listLevel.Add(3);
                listLevel.Add(4);
            }
            else
            {
                listLevel = alarm.FilteredLevel;
            }

            var orderedDevice = String.Empty;
            switch (alarm.OrderedDevice)
            {
                case "down":
                    orderedDevice = "desc";
                    break;
                case "up":
                    orderedDevice = "asc";
                    break;
                default:
                    orderedDevice = "none";
                    break;
            }

            var orderedLevel = String.Empty;
            switch (alarm.OrderedLevel)
            {
                case "none":
                    orderedLevel = "none";
                    break;
                case "down":
                    orderedLevel = "desc";
                    break;
                default:
                    orderedLevel = "asc";
                    break;
            }

            var orderedTime = String.Empty;
            switch (alarm.OrderedTime)
            {
                case "down":
                    orderedTime = "desc";
                    break;
                case "up":
                    orderedTime = "asc";
                    break;
                default:
                    orderedTime = "none";
                    break;
            }

            return new AlarmFilteredOrderedCondition
            {
                DeviceTypeIds = listDeviceTypeId,
                DealFlags = listDealFlag,
                Levels = listLevel, // 过滤条件: 告警等级数组, 支持同时查询多个告警等级, 默认值: 所有告警等级[1,2,3,4]
                StarTime = DateTime.Parse(alarm.FilteredStartTime),
                EndTime = DateTime.Parse(alarm.FilteredEndTime),
                OrderedDevice = orderedDevice, // 按照设置位置排序, 值域: {"desc", "asc", "none"}, 默认值: "none"
                OrderedLevel = orderedLevel, // 按照告警等级排序, 值域: {"desc", "asc", "none"}, 默认值: "asc"
                OrderedTime = orderedTime // 按照告警产生时间排序, 值域: {"desc", "asc", "none"}, 默认值: "none"
            };
        }

        public DataTable GetPagedFilteredOrderedAlarmsByStruct(int structId, AlarmFilteredOrderedCondition condition, int startIndex, int endIndex)
        {
            string order = "order by wt.WarningLevel";
            string retOrder = "order by r.WarningLevel";
            if (condition.OrderedLevel != "none")
            {
                order = "order by wt.WarningLevel " + condition.OrderedLevel;
                retOrder = "order by r.WarningLevel  " + condition.OrderedLevel;
            }
            if (condition.OrderedDevice != "none")
            {
                //order = "order by Source " + condition.OrderedDevice;
                order = "order by w.DeviceTypeId " + condition.OrderedDevice;
                retOrder = "order by r.Source  " + condition.OrderedDevice;
            }
            if (condition.OrderedTime != "none")
            {
                order = "order by w.Time " + condition.OrderedTime;
                retOrder = "order by r.Time " + condition.OrderedTime;
            }

            string sqlString = string.Format(
                @"select r.* from (
                    select ROW_NUMBER() over({6}) as rowId, w.Id,w.StructId,w.WarningTypeId,
		            Source=case w.DeviceTypeId
				            when 1 then 'dtu:' + Convert(nvarchar,w.DeviceId)
				            when 2 then '传感器:' + (select Convert(nvarchar,MIN(SENSOR_LOCATION_DESCRIPTION)) from T_DIM_SENSOR where SENSOR_ID=w.DeviceId)
							            + '-' + (select CONVERT(nvarchar,MIN(p.PRODUCT_NAME)) from T_DIM_SENSOR s join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID where s.SENSOR_ID=w.DeviceId)
				            when 3 then '采集仪模块号:' + (select Convert(nvarchar,MIN(MODULE_NO)) from T_DIM_SENSOR where SENSOR_ID=w.DeviceId)
		               end,
		            wt.WarningLevel,w.Content,wt.Reason,w.Time,w.DealFlag,
		            u.USER_NAME as Confirmor,d.Suggestion,d.ConfirmTime
            from T_WARNING_SENSOR w
            join T_DIM_WARNING_TYPE wt on w.WarningTypeId=wt.TypeId
            left join T_WARNING_DEALDETAILS d on w.Id=d.WarningId
            left join dbo.T_DIM_USER u on u.USER_NO=d.UserNo
            where w.StructId={0} and w.DeviceTypeId in ({1}) and wt.WarningLevel in ({2}) and (w.Time between '{3}' and '{4}') and w.DealFlag in ({5}) 
            ) r where r.rowId between {7} and {8} {9} ",
                structId, string.Join(",", condition.DeviceTypeIds), string.Join(",", condition.Levels), condition.StarTime, condition.EndTime, string.Join(",", condition.DealFlags), order, startIndex, endIndex, retOrder);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 查询结构物下过滤及排序后的告警内容
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable GetFilteredOrderedAlarmsByStruct(int structId, AlarmFilteredOrderedCondition condition)
        {
            string order = "order by wt.WarningLevel";
            if (condition.OrderedLevel != "none")
            {
                order = "order by wt.WarningLevel " + condition.OrderedLevel;
            }
            if (condition.OrderedDevice != "none")
            {
                order = "order by Source " + condition.OrderedDevice;
            }
            if (condition.OrderedTime != "none")
            {
                order = "order by w.Time " + condition.OrderedTime;
            }

            string sqlString = string.Format(
                @"select w.Id,w.StructId,w.WarningTypeId,
		            Source=case w.DeviceTypeId
				        when 1 then 'dtu:' + Convert(nvarchar,w.DeviceId)
				        when 2 then '传感器:' + (select Convert(nvarchar,MIN(SENSOR_LOCATION_DESCRIPTION)) from T_DIM_SENSOR where SENSOR_ID=w.DeviceId)
							        + '-' + (select CONVERT(nvarchar,MIN(p.PRODUCT_NAME)) from T_DIM_SENSOR s join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID where s.SENSOR_ID=w.DeviceId)
				        when 3 then '采集仪模块号:' + (select Convert(nvarchar,MIN(MODULE_NO)) from T_DIM_SENSOR where SENSOR_ID=w.DeviceId)
		            end,
		            wt.WarningLevel,w.Content,wt.Reason,w.Time,w.DealFlag,
		            u.USER_NAME as Confirmor,d.Suggestion,d.ConfirmTime
            from T_WARNING_SENSOR w
            join T_DIM_WARNING_TYPE wt on w.WarningTypeId=wt.TypeId
            left join T_WARNING_DEALDETAILS d on w.Id=d.WarningId
            left join dbo.T_DIM_USER u on u.USER_NO=d.UserNo
            where w.StructId={0} and w.DeviceTypeId in ({1}) and wt.WarningLevel in ({2}) and (w.Time between '{3}' and '{4}') and w.DealFlag in ({5}) {6}",
                structId, string.Join(",", condition.DeviceTypeIds), string.Join(",", condition.Levels), condition.StarTime, condition.EndTime, string.Join(",", condition.DealFlags), order);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }
    }

    public class AlarmModel
    {
        public string FilteredDeviceType { get; set; }

        public string FilteredStatus { get; set; }

        public List<int> FilteredLevel { get; set; }

        public string FilteredStartTime { get; set; }

        public string FilteredEndTime { get; set; }

        public string OrderedDevice { get; set; }

        public string OrderedLevel { get; set; }

        public string OrderedTime { get; set; }
    }

    public class AlarmModel_1
    {
        public string FilteredDeviceType { get; set; }

        public string FilteredStatus { get; set; }

        public string FilteredLevel { get; set; }

        public string FilteredStartTime { get; set; }

        public string FilteredEndTime { get; set; }

        public string OrderedDevice { get; set; }

        public string OrderedLevel { get; set; }

        public string OrderedTime { get; set; }
    }


    public class AlarmFilteredOrderedCondition
    {
        public List<int> DeviceTypeIds { get; set; }

        public List<int> DealFlags { get; set; }

        public List<int> Levels { get; set; }

        public DateTime StarTime { get; set; }

        public DateTime EndTime { get; set; }

        public string OrderedDevice { get; set; }

        public string OrderedLevel { get; set; }

        public string OrderedTime { get; set; }
    }
}
