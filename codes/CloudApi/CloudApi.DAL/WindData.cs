// --------------------------------------------------------------------------------------------
// <copyright file="WindData.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20140305
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;

    /// <summary>
    /// 风速数据
    /// </summary>
    public class WindData
    {
        /// <summary>
        /// 风玫瑰图统计数据
        /// </summary>
        /// <param name="sensorId"> The sensor Id. </param>
        /// <param name="startDate"> The start Date. </param>
        /// <param name="endDate"> The end Date. </param>
        /// <returns> 统计数据 </returns>
        public IList<WindStatData> GetWindStatData(int sensorId, DateTime startDate, DateTime endDate)
        {
            string sql = @"select data.direct,
	                               percent1 = sum(case when data.speed >= 0 and data.speed < 0.5 then 1.0 else 0.0 end)/data.total*100,
	                               percent2 = sum(case when data.speed >= 0.5 and data.speed < 2 then 1.0 else 0.0 end)/data.total*100,
	                               percent3 = sum(case when data.speed >= 2 and data.speed < 4 then 1.0 else 0.0 end)/data.total*100,
	                               percent4 = sum(case when data.speed >= 4 and data.speed < 6 then 1.0 else 0.0 end)/data.total*100,
	                               percent5 = sum(case when data.speed >= 6 and data.speed < 8 then 1.0 else 0.0 end)/data.total*100,
	                               percent6 = sum(case when data.speed >= 8 and data.speed < 10 then 1.0 else 0.0 end)/data.total*100,
	                               percent7 = sum(case when data.speed >= 10 then 1 else 0 end)/data.total*100,
	                               totalPercet = COUNT(*)*1.0/data.total*100
                            from (select direct = case 
						                            when WIND_DIRECTION_VALUE >= 0 and WIND_DIRECTION_VALUE < 11.25 then 'N'
						                            when WIND_DIRECTION_VALUE >= 11.25 and WIND_DIRECTION_VALUE < 33.75 then 'NNE'
						                            when WIND_DIRECTION_VALUE >= 33.75 and WIND_DIRECTION_VALUE < 56.25 then 'NE'
						                            when WIND_DIRECTION_VALUE >= 56.25 and WIND_DIRECTION_VALUE < 78.75 then 'ENE'
						                            when WIND_DIRECTION_VALUE >= 78.75 and WIND_DIRECTION_VALUE < 101.25 then 'E'
						                            when WIND_DIRECTION_VALUE >= 101.25 and WIND_DIRECTION_VALUE < 123.75 then 'ESE'
						                            when WIND_DIRECTION_VALUE >= 124.75 and WIND_DIRECTION_VALUE < 146.25 then 'SE'
						                            when WIND_DIRECTION_VALUE >= 146.25 and WIND_DIRECTION_VALUE < 168.75 then 'SSE'
						                            when WIND_DIRECTION_VALUE >= 168.75 and WIND_DIRECTION_VALUE < 191.25 then 'S'
						                            when WIND_DIRECTION_VALUE >= 191.25 and WIND_DIRECTION_VALUE < 213.75 then 'SSW'
						                            when WIND_DIRECTION_VALUE >= 213.75 and WIND_DIRECTION_VALUE < 236.25 then 'SW'
						                            when WIND_DIRECTION_VALUE >= 236.25 and WIND_DIRECTION_VALUE < 258.75 then 'WSW'
						                            when WIND_DIRECTION_VALUE >= 258.75 and WIND_DIRECTION_VALUE < 281.25 then 'W'
						                            when WIND_DIRECTION_VALUE >= 281.25 and WIND_DIRECTION_VALUE < 303.75 then 'WNW'
						                            when WIND_DIRECTION_VALUE >= 303.75 and WIND_DIRECTION_VALUE < 326.25 then 'NW'
						                            when WIND_DIRECTION_VALUE >= 326.25 and WIND_DIRECTION_VALUE < 348.75 then 'NNW'
						                            else 'N'
					                            end,
		                               number = case 
						                            when WIND_DIRECTION_VALUE >= 0 and WIND_DIRECTION_VALUE < 11.25 then 1
						                            when WIND_DIRECTION_VALUE >= 11.25 and WIND_DIRECTION_VALUE < 33.75 then 2
						                            when WIND_DIRECTION_VALUE >= 33.75 and WIND_DIRECTION_VALUE < 56.25 then 3
						                            when WIND_DIRECTION_VALUE >= 56.25 and WIND_DIRECTION_VALUE < 78.75 then 4
						                            when WIND_DIRECTION_VALUE >= 78.75 and WIND_DIRECTION_VALUE < 101.25 then 5
						                            when WIND_DIRECTION_VALUE >= 101.25 and WIND_DIRECTION_VALUE < 123.75 then 6
						                            when WIND_DIRECTION_VALUE >= 124.75 and WIND_DIRECTION_VALUE < 146.25 then 7
						                            when WIND_DIRECTION_VALUE >= 146.25 and WIND_DIRECTION_VALUE < 168.75 then 8
						                            when WIND_DIRECTION_VALUE >= 168.75 and WIND_DIRECTION_VALUE < 191.25 then 9
						                            when WIND_DIRECTION_VALUE >= 191.25 and WIND_DIRECTION_VALUE < 213.75 then 10
						                            when WIND_DIRECTION_VALUE >= 213.75 and WIND_DIRECTION_VALUE < 236.25 then 11
						                            when WIND_DIRECTION_VALUE >= 236.25 and WIND_DIRECTION_VALUE < 258.75 then 12
						                            when WIND_DIRECTION_VALUE >= 258.75 and WIND_DIRECTION_VALUE < 281.25 then 13
						                            when WIND_DIRECTION_VALUE >= 281.25 and WIND_DIRECTION_VALUE < 303.75 then 14
						                            when WIND_DIRECTION_VALUE >= 303.75 and WIND_DIRECTION_VALUE < 326.25 then 15
						                            when WIND_DIRECTION_VALUE >= 326.25 and WIND_DIRECTION_VALUE < 348.75 then 16
						                            else 1
					                            end,
		                               speed = WIND_SPEED_VALUE,
		                               total = (select COUNT(*)
					                            from dbo.T_THEMES_ENVI_WIND 
					                            where SENSOR_ID=@sensorId and ACQUISITION_DATETIME between @start and @end)																	
	                            from dbo.T_THEMES_ENVI_WIND
	                            where SENSOR_ID=@sensorId and ACQUISITION_DATETIME between @start and @end) data
                            group by data.direct,data.total,data.number
                            order by data.number";
            SqlParameter[] paras =
                {
                    new SqlParameter("@sensorId", sensorId), new SqlParameter("@start", startDate), new SqlParameter("@end", endDate)
                };
            DataTable table = SqlHelper.ExecuteDataSet(CommandType.Text, sql, paras).Tables[0];
            var rslt = new List<WindStatData>();
            foreach (DataRow row in table.Rows)
            {
                rslt.Add(
                    new WindStatData
                        {
                            Direct = row[0].ToString(),
                            Percent1 = Convert.ToDecimal(row[1]),
                            Percent2 = Convert.ToDecimal(row[2]),
                            Percent3 = Convert.ToDecimal(row[3]),
                            Percent4 = Convert.ToDecimal(row[4]),
                            Percent5 = Convert.ToDecimal(row[5]),
                            Percent6 = Convert.ToDecimal(row[6]),
                            Percent7 = Convert.ToDecimal(row[7]),
                            TotalPercent = Convert.ToDecimal(row[8]),
                        });
            }

            return rslt;
        }
    }
}