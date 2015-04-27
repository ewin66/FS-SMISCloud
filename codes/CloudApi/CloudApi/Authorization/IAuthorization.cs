// --------------------------------------------------------------------------------------------
// <copyright file="IAuthority.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2013 飞尚科技
// 版权所有。
// </copyright>
// <summary>
// 文件功能描述：验证接口
//
// 创建标识：Liuxinyi2013-12-27
//
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Authorization
{
    using System.Net.Http;
    using System.Net;

    /// <summary> Interface for authority. </summary>
    /// <remarks> Liuxinyi, 2013-12-27. </remarks>
    public interface IAuthorization
    {
        /// <summary> Query if 'request' is authorized. </summary>
        /// <param name="request"> The request. </param>
        /// <returns> true if authorized, false if not. </returns>
        bool IsAuthorized(HttpRequestMessage request,out HttpStatusCode retcode);

        /// <summary> Removes the verify ticket described by token. </summary>
        /// <param name="token"> The token. </param>
        void RemoveVerifyTicket(string token);

        /// <summary> Saves a verify ticket. </summary>
        /// <param name="token">    The token. </param>
        /// <param name="userInfo"> The user info. </param>
        void SaveVerifyTicket(string token, AuthorizationInfo userInfo);

        /// <summary> Saves a verify ticket. </summary>
        /// <remarks> Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token">    The token. </param>
        /// <param name="userInfo"> The user info. </param>
        /// <param name="duration"></param>
        void SaveVerifyTicket(string token, AuthorizationInfo userInfo, double duration);

        /// <summary> Get Authorization Info. </summary>
        /// <param name="token"> The token. </param>
        /// <returns> The Authorization Info. </returns>
        AuthorizationInfo GetAuthorizationInfo(string token);

        /// <summary> Update Authorization Expire. </summary>
        /// <remarks> Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="duration"></param>        
        void UpdateAuthorizationInfo(string token, double duration);
    }
}