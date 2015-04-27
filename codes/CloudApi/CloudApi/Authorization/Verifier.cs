// --------------------------------------------------------------------------------------------
// <copyright file="Verifier.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
// 文件功能描述：资源权限验证类
//
// 创建标识：Liuxinyi2014-1-2
//
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System.Web.Http.Controllers;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;

    public class Verifier
    {
        /// <summary> Dictionary of verifies. </summary>
        private static Dictionary<string, Func<int, string, bool>> verifyDict =
            new Dictionary<string, Func<int, string, bool>>();

        static Verifier()
        {
            verifyDict.Add("userId", VerifyUserId);
            verifyDict.Add("structId", VerifyStructId);
            verifyDict.Add("structs", VerifyStructs);
            verifyDict.Add("sensorId", VerifySensorId);
            verifyDict.Add("sensors", VerifySensors);
            verifyDict.Add("warnIds", VerifyWarnIds);
            verifyDict.Add("groupId", VerifyGroupId);
        }

        /// <summary> Verify permission. </summary>
        /// <remarks> Liuxinyi, 2014-1-7. </remarks>
        /// <param name="info">    The information. </param>
        /// <param name="request"> The request. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool VerifyPermission(AuthorizationInfo info, HttpRequestMessage request)
        {
            if (info.RoleId == 1) return true;  // supper admin
            return VertifyAction(info, request) && VertifyOwner(info, request);
        }

        /// <summary> Vertify action. </summary>
        /// <remarks> Liuxinyi, 2014-1-7. </remarks>
        /// <param name="info">    The information. </param>
        /// <param name="request"> The request. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private bool VertifyAction(AuthorizationInfo info, HttpRequestMessage request)
        {

            HttpActionDescriptor actionDescriptor = request.GetActionDescriptor();
            var authorizationAttributes = actionDescriptor.GetCustomAttributes<AuthorizationAttribute>();
            if (authorizationAttributes.Any())
            {
                return authorizationAttributes.Any(authorAttr => info.AuthorisedResources.Contains(authorAttr.StrAuthorizationCode));
            }
            return true;
        }

        /// <summary> Vertify owner. </summary>
        /// <remarks> Liuxinyi, 2014-1-7. </remarks>
        /// <param name="info">    The information. </param>
        /// <param name="request"> The request. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private bool VertifyOwner(AuthorizationInfo info, HttpRequestMessage request)
        {
            /*if (info.RoleId != 5)   // @TODO 超级管理员权限
            {
                return true;
            }*/

            var routeData = request.GetRouteData();
            // 数据服务API角色
            if (info.RoleId == 6)
            {
                if (routeData.Values.ContainsKey("structId"))
                {
                    if (!DataService.GetServiceUserStruct(info.UserName).Contains(routeData.Values["structId"]))
                    {
                        return false;
                    }
                }

                if (routeData.Values.ContainsKey("sensorId"))
                {
                    var sensorId = routeData.Values["sensorId"];
                    int senId = Convert.ToInt32(sensorId);
                    using (SecureCloud_Entities entity = new SecureCloud_Entities())
                    {
                        var query = from sensor in entity.T_DIM_SENSOR
                                    where sensor.SENSOR_ID == senId
                                    select sensor.STRUCT_ID;
                        if (query.FirstOrDefault() == null
                            || !DataService.GetServiceUserStruct(info.UserName).Contains(query.First().ToString()))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return true;
            }

            int userId = info.UserId;
            foreach (var pair in verifyDict)
            {
                if (routeData.Values.ContainsKey(pair.Key))
                {
                    return pair.Value(userId, routeData.Values[pair.Key].ToString());
                }
            }
            return true;
        }

        #region 资源验证
        /// <summary> Verify user name. </summary>
        /// <remarks> Liuxinyi, 2014-1-8. </remarks>
        /// <param name="userId"> The username. </param>
        /// <param name="key">      The key. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private static bool VerifyUserId(int userId, string key)
        {
            if (key != userId.ToString())
            {
                return false;
            }
            return true;
        }

        /// <summary> Verify structure identifier. </summary>
        /// <remarks> Liuxinyi, 2014-1-8. </remarks>
        /// <param name="userId"> The username. </param>
        /// <param name="structId"> Identifier for the structure. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private static bool VerifyStructId(int userId, string structId)
        {
            int stctId = Convert.ToInt32(structId);
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from us in entity.T_DIM_USER_STRUCTURE
                            where us.USER_NO == userId && us.STRUCTURE_ID == stctId
                            select us.Id;
                if (!query.Any())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Verify structs. </summary>
        /// <remarks> Liuxinyi, 2014-1-8. </remarks>
        /// <param name="userId"> The username. </param>
        /// <param name="structs">  The structs. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private static bool VerifyStructs(int userId, string structs)
        {
            int[] strcIds = structs.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from us in entity.T_DIM_USER_STRUCTURE
                            where us.USER_NO == userId
                            select us.STRUCTURE_ID;
                if (!strcIds.All(query.ToList().Contains)) //不是所有资源都属于用户
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Verify sensor identifier. </summary>
        /// <remarks> Liuxinyi, 2014-1-8. </remarks>
        /// <param name="userId"> The username. </param>
        /// <param name="sensorId"> Identifier for the sensor. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private static bool VerifySensorId(int userId, string sensorId)
        {
            int senId = Convert.ToInt32(sensorId);
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from user in entity.T_DIM_USER
                            from us in entity.T_DIM_USER_STRUCTURE
                            from sensor in entity.T_DIM_SENSOR
                            where user.USER_NO == us.USER_NO
                                  && us.STRUCTURE_ID == sensor.STRUCT_ID
                                  && user.USER_NO == userId
                                  && sensor.SENSOR_ID == senId
                            select sensor.SENSOR_ID;
                if (!query.Any())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Verify sensors. </summary>
        /// <remarks> Liuxinyi, 2014-1-8. </remarks>
        /// <param name="userId"> The username. </param>
        /// <param name="sensors">  The sensors. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private static bool VerifySensors(int userId, string sensors)
        {
            int[] senIds = sensors.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from user in entity.T_DIM_USER
                            from us in entity.T_DIM_USER_STRUCTURE
                            from sensor in entity.T_DIM_SENSOR
                            where user.USER_NO == us.USER_NO
                                  && us.STRUCTURE_ID == sensor.STRUCT_ID
                                  && user.USER_NO == userId
                            select sensor.SENSOR_ID;
                if (!senIds.All(query.ToList().Contains))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Verify warning identifiers. </summary>
        /// <remarks> Liuxinyi, 2014-1-8. </remarks>
        /// <param name="userId"> The username. </param>
        /// <param name="warnids">  The warnids. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private static bool VerifyWarnIds(int userId, string warnids)
        {
            int[] warnIds = warnids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from user in entity.T_DIM_USER
                            from us in entity.T_DIM_USER_STRUCTURE                        
                            from warn in entity.T_WARNING_SENSOR
                            where user.USER_NO == us.USER_NO
                                  && us.STRUCTURE_ID == warn.StructId
                                  && user.USER_NO == userId
                            select warn.Id;
                if (!warnIds.All(query.ToList().Contains))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Verify group identifier. </summary>
        /// <remarks> Liuxinyi, 2014-1-8. </remarks>
        /// <param name="username"> The username. </param>
        /// <param name="groupId">  Identifier for the group. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private static bool VerifyGroupId(int userId, string groupId)
        {
            int grpId = Convert.ToInt32(groupId);
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                // TODO:分组改变
                var query = from user in entity.T_DIM_USER
                            from us in entity.T_DIM_USER_STRUCTURE
                            from sensor in entity.T_DIM_SENSOR
                            from senGrp in entity.T_DIM_SENSOR_GROUP_CEXIE
                            where user.USER_NO == us.USER_NO                                  
                                  && us.STRUCTURE_ID == sensor.STRUCT_ID
                                  && user.USER_NO == userId
                                  && sensor.SENSOR_ID == senGrp.SENSOR_ID
                                  && senGrp.GROUP_ID == grpId
                            select senGrp.GROUP_ID;

                var query2 = from user in entity.T_DIM_USER
                            from us in entity.T_DIM_USER_STRUCTURE
                            from sensor in entity.T_DIM_SENSOR
                            from senGrp in entity.T_DIM_SENSOR_GROUP_CHENJIANG
                            where user.USER_NO == us.USER_NO
                                  && us.STRUCTURE_ID == sensor.STRUCT_ID
                                  && user.USER_NO == userId
                                  && sensor.SENSOR_ID == senGrp.SENSOR_ID
                                  && senGrp.GROUP_ID == grpId
                            select senGrp.GROUP_ID;

                var query3 = from user in entity.T_DIM_USER
                            from us in entity.T_DIM_USER_STRUCTURE
                            from sensor in entity.T_DIM_SENSOR
                            from senGrp in entity.T_DIM_SENSOR_GROUP_JINRUNXIAN
                            where user.USER_NO == us.USER_NO
                                  && us.STRUCTURE_ID == sensor.STRUCT_ID
                                  && user.USER_NO == userId
                                  && sensor.SENSOR_ID == senGrp.SENSOR_ID
                                  && senGrp.GROUP_ID == grpId
                            select senGrp.GROUP_ID;

                if (!(query.Any() || query2.Any() || query3.Any()))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}