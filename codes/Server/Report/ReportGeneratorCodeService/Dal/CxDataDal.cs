// --------------------------------------------------------------------------------------------
// <copyright file="DataAccess.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：测斜数据
// 
// 创建标识：刘歆毅20140521
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ReportGeneratorService.DataModule;

namespace ReportGeneratorService.Dal
{
    using ReportGeneratorService.DataModule;

    public class CxDataDal
    {
        /// <summary>
        /// 测斜数据
        /// </summary>
        /// <param name="groupId">传感器组编号（只能是数字）</param>        
        /// <param name="startdate">开始时间（ISO时间）</param>
        /// <param name="enddate">结束时间（ISO时间）</param>
        /// <returns>深部位移数据</returns>
        public List<CxData> GetByGroupDirectAndDateGroupByTime(int groupId, DateTime startdate, DateTime enddate)
        {
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from d in entities.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            where d.SENSOR_ID == sg.SENSOR_ID
                                  && sg.GROUP_ID == groupId
                                  && d.ACQUISITION_DATETIME >= startdate && d.ACQUISITION_DATETIME <= enddate
                            select new
                            {
                                depth = sg.DEPTH,
                                xvalue = d.DEEP_CUMULATIVEDISPLACEMENT_X_VALUE,
                                yvalue = d.DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE,
                                acquistiontime = d.ACQUISITION_DATETIME
                            };
                var list = query.ToList();

                return
                    list.GroupBy(d => d.acquistiontime)
                        .OrderBy(d => d.Key)
                        .Select(
                            d =>
                            new CxData
                                {
                                    DateTime = d.Key,
                                    Data =
                                        d.Select(
                                            g =>
                                            new CxInernalData
                                                {
                                                    Depth = g.depth,
                                                    XValue = g.xvalue,
                                                    YValue = g.yvalue
                                                }).ToList()
                                })
                        .ToList();
            }
        }
    }
}
