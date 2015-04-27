namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL
{
    using System;
    using System.Data;
    using System.Text;

    /// <summary>
    /// 告警数据类
    /// </summary>
    public class Warning
    {

        private static int sensorTypeStrain = 157;
        private static int sensorTypeVib = 160;
        
        /// <summary>
        /// 获取传感器最严重告警等级
        /// </summary>
        /// <param name="sensors">传感器数组</param>
        /// <returns>告警列表</returns>
        public DataTable GetTopWarningLevel(int[] sensors)
        {
            string sqlString = string.Format(
                @"select DeviceId,MIN(t.WarningLevel) from T_WARNING_SENSOR w
					join T_DIM_WARNING_TYPE t on w.WarningTypeId=t.TypeId
                    where DeviceId in({0}) and DealFlag=3 and DeviceTypeId=2
                    group by DeviceId", 
                string.Join(",", sensors));
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取传感器最严重告警等级
        /// </summary>
        /// <param name="sensors">传感器数组</param>
        /// <returns>告警列表</returns>
        public DataTable GetTopWarningLevelRShell(int[] sensors)
        {
            string sqlStringOther = string.Format(
                @"select DeviceId,MIN(t.WarningLevel) from T_WARNING_SENSOR w
					join T_DIM_WARNING_TYPE t on w.WarningTypeId=t.TypeId
                    where DeviceId in ( select SENSOR_ID from dbo.T_DIM_SENSOR where SENSOR_ID in({0}) 
                    and PRODUCT_SENSOR_ID!={1} and PRODUCT_SENSOR_ID!={2}) 
                    and DealFlag=3 and DeviceTypeId=2 
                    group by DeviceId",
                string.Join(",", sensors), sensorTypeStrain, sensorTypeVib);
            DataSet dsOther = SqlHelper.ExecuteDataSetText(sqlStringOther, null);

            string sqlStringWeldVib = string.Format(
                @"select  distinct k.SensorId as DeviceId,MIN(t.WarningLevel) as WarningLevel from T_WARNING_SENSOR w ,dbo.T_DIM_SENSOR_CORRENT k,T_DIM_WARNING_TYPE t
                    where DeviceId in ( select b.CorrentSensorId from dbo.T_DIM_SENSOR as a,dbo.T_DIM_SENSOR_CORRENT as b where b.SensorId  in({0}) 
                    and a.SENSOR_ID=b.CorrentSensorId and  a.PRODUCT_SENSOR_ID={1} or a.PRODUCT_SENSOR_ID ={2}  ) 
                    and DealFlag=3 and DeviceTypeId=2  and k.CorrentSensorId=w.DeviceId and w.WarningTypeId=t.TypeId
                    group by DeviceId ,k.SensorId",
                string.Join(",", sensors), sensorTypeStrain, sensorTypeVib);
            DataSet dsWeldVib = SqlHelper.ExecuteDataSetText(sqlStringWeldVib, null);
            dsOther.Merge(dsWeldVib, true, MissingSchemaAction.AddWithKey);
            return dsOther.Tables[0];
        }

        /// <summary>
        /// 获取结构物最严重告警
        /// 告警编号-结构物编号-告警源-等级-内容-原因-时间-告警类型编号
        /// </summary>
        /// <param name="structs">结构物</param>
        /// <returns>告警列表        
        /// </returns>
        public DataTable GetTopWarning(int[] structs)
        {
            string sqlString = string.Format(
                @"select t.Id,t.StructId,
	                Source=case t.DeviceTypeId
				                when 1 then 'dtu:' + CONVERT(nvarchar,t.DeviceId)
				                when 2 then '传感器:' + (select Convert(nvarchar,MIN(SENSOR_LOCATION_DESCRIPTION)) from T_DIM_SENSOR where SENSOR_ID=t.DeviceId)
							                + '-' + (select CONVERT(nvarchar,MIN(p.PRODUCT_NAME)) from T_DIM_SENSOR s join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID where s.SENSOR_ID=t.DeviceId)
				                when 3 then '采集仪模块号:' + (select Convert(nvarchar,MIN(MODULE_NO)) from T_DIM_SENSOR where SENSOR_ID=t.DeviceId)
		                   end
                ,
                t.WarningLevel,t.Content,t.Reason,t.Time,t.TypeId from
                (select ROW_NUMBER() over(partition by StructId order by WarningLevel,Time desc) as row,* from T_WARNING_SENSOR w
                    join T_DIM_WARNING_TYPE wt on w.WarningTypeId=wt.TypeId
                    where DealFlag=3 and StructId in ({0})
                ) t
                where t.row=1",
                string.Join(",", structs));
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取项目告警信息
        /// 告警编号-结构物编号-告警源-等级-内容-原因-时间-告警类型编号
        /// </summary>
        /// <param name="structs">结构物id数组</param>
        /// <returns>未确认告警列表        
        /// </returns>
        public DataTable GetProjectWarning(int[] structs)
        {
            string sqlString = string.Format(
                @"select t.Id,t.StructId,
	                Source=case t.DeviceTypeId
				                when 1 then 'dtu:' + CONVERT(nvarchar,t.DeviceId)
				                when 2 then '传感器:' + (select Convert(nvarchar,MIN(SENSOR_LOCATION_DESCRIPTION)) from T_DIM_SENSOR where SENSOR_ID=t.DeviceId)
							                + '-' + (select CONVERT(nvarchar,MIN(p.PRODUCT_NAME)) from T_DIM_SENSOR s join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID where s.SENSOR_ID=t.DeviceId)
				                when 3 then '采集仪模块号:' + (select Convert(nvarchar,MIN(MODULE_NO)) from T_DIM_SENSOR where SENSOR_ID=t.DeviceId)
		                   end
                ,
                t.WarningLevel,t.Content,t.Reason,t.Time,t.TypeId from
                (select ROW_NUMBER() over(partition by StructId order by WarningLevel,Time desc) as row,* from T_WARNING_SENSOR w
                    join T_DIM_WARNING_TYPE wt on w.WarningTypeId=wt.TypeId
                    where DealFlag=1 and StructId in ({0})
                ) t
                where t.row between 1 and 15",
                string.Join(",", structs));
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取结构物告警数量
        /// </summary>
        /// <param name="structs">结构物列表</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="deal">是否处理</param>
        /// <param name="roleId">用户角色</param>
        /// <returns></returns>
        public int GetWarningsCountByStruct(int[] structs, DateTime begin, DateTime end, int? deal, int roleId)
        {
            string filter = string.Empty;
            if (deal != null)
            {
                filter = "and w.DealFlag=" + deal;
            }
            else
            {
                switch (roleId)
                {
                    case 3:
                        filter = "and w.DealFlag in (1, 2)"; // all warnings for Support.
                        break;
                    case 5:
                        filter = "and w.DealFlag in (3, 4)"; // all warnings for Client.
                        break;
                }
            }

            string sqlString = string.Format(
                @"select COUNT(*)
                    from T_WARNING_SENSOR w
                    where w.StructId in ({0})  
                    and (w.Time between '{1}' and '{2}') {3}",
                string.Join(",", structs), begin, end, filter);
            int count = (int)SqlHelper.ExecuteScalarText(sqlString, null);
            return count;
        }

        /// <summary>
        /// 获取结构物告警
        /// </summary>
        /// <param name="structs">结构物列表</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="deal">是否处理</param>
        /// <param name="roleId">用户角色</param>
        /// <returns>告警列表</returns>
        public DataTable GetWarningsByStruct(int[] structs, DateTime begin, DateTime end, int? deal, int roleId)
        {
            string filter = string.Empty;
            if (deal != null)
            {
                filter = "and w.DealFlag=" + deal;
            }
            else
            {
                switch (roleId)
                {
                    case 3:
                        filter = "and w.DealFlag in (1, 2)"; // all warnings for Support.
                        break;
                    case 5:
                        filter = "and w.DealFlag in (3, 4)"; // all warnings for Client.
                        break;
                }
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
            where w.StructId in ({0}) and (w.Time between '{1}' and '{2}') {3}
            order by w.DealFlag, wt.WarningLevel, w.Time desc",
                string.Join(",", structs), begin, end, filter);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取结构物告警-分页
        /// </summary>
        /// <param name="structs">结构物列表</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="deal">是否确认</param>
        /// <param name="startIndex">开始条数</param>
        /// <param name="endIndex">结束条数</param>
        /// <param name="roleId">用户角色</param>
        /// <returns>告警数据</returns>
        public DataTable GetPagedWarningsByStruct(int[] structs, DateTime begin, DateTime end, int? deal, int startIndex, int endIndex, int roleId)
        {
            string filter = string.Empty;
            if (deal != null)
            {
                filter = "and w.DealFlag=" + deal;
            }
            else 
            {
                switch (roleId)
                {
                    case 3:
                        filter = "and w.DealFlag in (1, 2)"; // all warnings for Support.
                        break;
                    case 5:
                        filter = "and w.DealFlag in (3, 4)"; // all warnings for Client.
                        break;
                }
            }

            string sqlString = string.Format(
                @"select r.* from (
                    select ROW_NUMBER() over(order by w.DealFlag, wt.WarningLevel, w.Time desc) as rowId, w.Id,w.StructId,w.WarningTypeId,
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
            where w.StructId in ({0}) and (w.Time between '{1}' and '{2}') {3} 
            ) r where r.rowId between {4} and {5}",
                string.Join(",", structs), begin, end, filter, startIndex, endIndex);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取传感器告警数量
        /// </summary>
        /// <param name="sensorId">传感器列表</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="deal">是否处理</param>
        /// <param name="roleId">用户角色</param>
        /// <returns>告警数量</returns>
        public int GetWarningsCountBySensor(int[] sensorId, DateTime begin, DateTime end, int? deal, int roleId)
        {
            string filter = string.Empty;
            if (deal != null)
            {
                filter = "and w.DealFlag=" + deal;
            }
            else
            {
                switch (roleId)
                {
                    case 3:
                        filter = "and w.DealFlag in (1, 2)"; // all warnings for Support.
                        break;
                    case 5:
                        filter = "and w.DealFlag in (3, 4)"; // all warnings for Client.
                        break;
                }
            }

            string sqlString = string.Format(
                @"select count(*)
                    from T_WARNING_SENSOR w            
                    where w.DeviceTypeId=2 and w.DeviceId in ({0}) 
                    and (w.Time>='{1}' and w.Time<='{2}') {3}",
                string.Join(",", sensorId), begin, end, filter);
            int count = (int)SqlHelper.ExecuteScalarText(sqlString, null);
            return count;
        }

        /// <summary>
        /// 获取传感器告警
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="deal">是否处理</param>
        /// <param name="roleId">用户角色</param>
        /// <returns>告警列表</returns>
        public DataTable GetWarningsBySensor(int[] sensorId, DateTime begin, DateTime end, int? deal, int roleId)
        {
            string filter = string.Empty;
            if (deal != null)
            {
                filter = "and w.DealFlag=" + deal;
            }
            else
            {
                switch (roleId)
                {
                    case 3:
                        filter = "and w.DealFlag in (1, 2)"; // all warnings for Support.
                        break;
                    case 5:
                        filter = "and w.DealFlag in (3, 4)"; // all warnings for Client.
                        break;
                }
            }

            string sqlString = string.Format(
                @"select w.Id,SensorId=w.DeviceId,w.WarningTypeId,
		            Location=(
						select MIN(SENSOR_LOCATION_DESCRIPTION) 
						from T_DIM_SENSOR 
						where SENSOR_ID=w.DeviceId),
		            ProductTypeId=(select MIN(p.PRODUCT_TYPE_ID) 
						from T_DIM_SENSOR s
						join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID
						where s.SENSOR_ID=w.DeviceId),
					ProductName=(
						select MIN(pt.PRODUCT_TYPE_NAME) 
						from T_DIM_SENSOR s 
						join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID
						join T_DIM_PRODUCT_TYPE pt on p.PRODUCT_TYPE_ID=pt.PRODUCT_TYPE_ID 
						where s.SENSOR_ID=w.DeviceId),
		            wt.WarningLevel,w.Content,wt.Reason,w.Time,w.DealFlag,
		            u.USER_NAME as Confirmor,d.Suggestion,d.ConfirmTime
            from T_WARNING_SENSOR w
            join T_DIM_WARNING_TYPE wt on w.WarningTypeId=wt.TypeId
            left join T_WARNING_DEALDETAILS d on w.Id=d.WarningId
            left join dbo.T_DIM_USER u on u.USER_NO=d.UserNo
            where w.DeviceTypeId=2 and w.DeviceId in ({0}) and (w.Time>='{1}' and w.Time<='{2}') {3}
            order by w.DealFlag, wt.WarningLevel, w.Time desc",
                string.Join(",", sensorId), begin, end, filter);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取传感器告警-分页
        /// </summary>
        /// <param name="sensorId">传感器数组</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="deal">是否确认</param>
        /// <param name="startIndex">开始条数</param>
        /// <param name="endIndex">结束条数</param>
        /// <param name="roleId">用户角色</param>
        /// <returns>告警列表</returns>
        public DataTable GetPagedWarningsBySensor(int[] sensorId, DateTime begin, DateTime end, int? deal, int startIndex, int endIndex, int roleId)
        {
            string filter = string.Empty;
            if (deal != null)
            {
                filter = "and w.DealFlag=" + deal;
            }
            else
            {
                switch (roleId)
                {
                    case 3:
                        filter = "and w.DealFlag in (1, 2)"; // all warnings for Support.
                        break;
                    case 5:
                        filter = "and w.DealFlag in (3, 4)"; // all warnings for Client.
                        break;
                }
            }

            string sqlString = string.Format(
                @"select r.* from (
	                select ROW_NUMBER() over(order by w.DealFlag, wt.WarningLevel, w.Time desc) as rowId, w.Id,SensorId=w.DeviceId,w.WarningTypeId,
		            Location=(
						select MIN(SENSOR_LOCATION_DESCRIPTION) 
						from T_DIM_SENSOR 
						where SENSOR_ID=w.DeviceId),
		            ProductTypeId=(select MIN(p.PRODUCT_TYPE_ID) 
						from T_DIM_SENSOR s
						join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID
						where s.SENSOR_ID=w.DeviceId),
					ProductName=(
						select MIN(pt.PRODUCT_TYPE_NAME) 
						from T_DIM_SENSOR s 
						join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID=p.PRODUCT_ID
						join T_DIM_PRODUCT_TYPE pt on p.PRODUCT_TYPE_ID=pt.PRODUCT_TYPE_ID 
						where s.SENSOR_ID=w.DeviceId),
		            wt.WarningLevel,w.Content,wt.Reason,w.Time,w.DealFlag,
		            u.USER_NAME as Confirmor,d.Suggestion,d.ConfirmTime
            from T_WARNING_SENSOR w
            join T_DIM_WARNING_TYPE wt on w.WarningTypeId=wt.TypeId
            left join T_WARNING_DEALDETAILS d on w.Id=d.WarningId
            left join dbo.T_DIM_USER u on u.USER_NO=d.UserNo
            where w.DeviceTypeId=2 and w.DeviceId in ({0}) and (w.Time>='{1}' and w.Time<='{2}') {3}
            ) r where r.rowId between {4} and {5}",
                string.Join(",", sensorId), begin, end, filter, startIndex, endIndex);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }
        
        /// <summary>
        /// 获取告警对应的用户
        /// </summary>
        /// <param name="warnId">告警ID</param>
        /// <returns>用户列表</returns>
        public DataTable GetUserByWarnId(int warnId)
        {
            string sqlString = string.Format(
                @"SELECT USER_NAME FROM T_DIM_USER WHERE USER_NO =
                      (SELECT USER_NO FROM T_DIM_USER_STRUCTURE WHERE STRUCTURE_ID =
                          (SELECT StructId FROM T_WARNING_SENSOR WHERE Id = {0})
                       )",
                warnId);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取用户名对应的deviceToken
        /// </summary>
        /// <param name="user">用户名</param>
        /// <returns>deviceToken列表</returns>
        public DataTable GetDeviceTokenByUser(string user)
        {
            string sqlString = string.Format(
                @"SELECT DeviceToken FROM T_DIM_DEVICETOKEN WHERE OnlineUser='{0}'", user);
            DataSet ds = SqlHelper.ExecuteDataSetText(sqlString, null);
            return ds.Tables[0];
        }
    }
}