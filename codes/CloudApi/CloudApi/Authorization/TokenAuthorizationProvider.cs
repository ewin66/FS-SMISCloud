// --------------------------------------------------------------------------------------------
// <copyright file="CookieAuthority.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2013 飞尚科技
// 版权所有。
// </copyright>
// <summary>
// 文件功能描述：使用Token验证权限
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
    using System;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Net;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;

    public class TokenAuthorizationProvider : IAuthorization
    {
        private ITokenStore storer;

        public TokenAuthorizationProvider(ITokenStore storer)
        {
            this.storer = storer ?? new MemoryCacheTokenStore();
        }

        public TokenAuthorizationProvider()
        {
            this.storer = new MemoryCacheTokenStore();
        }

        /// <summary> Query if 'request' is authorized. </summary>
        /// <remarks> Liuxinyi, 2014-1-2. </remarks>
        /// <param name="request"> The request. </param>
        /// <param name="retcode"> The return httpstatuscode</param>
        /// <returns> true if authorized, false if not. </returns>
        public bool IsAuthorized(HttpRequestMessage request,out HttpStatusCode retcode)
        {
            string token = request.GetQueryString("token");
            retcode = HttpStatusCode.OK;
            if (token != null)
            {
                AuthorizationInfo info = this.storer.Get(token);
                if (info != null)
                {
                    // 验证资源
                    if (info.HashCode == this.GetHashValue(token, request.GetClientIp())
                        && new Verifier().VerifyPermission(info, request))
                    {
                        return true;
                    }
                    else
                    {
                        retcode = HttpStatusCode.MethodNotAllowed;  // 405
                    }
                }
                else
                {
                    retcode = HttpStatusCode.Forbidden; //  403
                }
            }

            return false;
        }

        /// <summary> Removes the verify ticket described by token. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="token"> The token. </param>
        public void RemoveVerifyTicket(string token)
        {
            this.storer.Remove(token);
        }

        /// <summary> Saves a verify ticket. </summary>
        /// <remarks> Liuxinyi, 2014-1-7. </remarks>
        /// <param name="token">    The token. </param>
        /// <param name="userId"> The username. </param>
        /// <param name="roleId">   Identifier for the role. </param>
        /// <param name="request">  The request. </param>
        public void SaveVerifyTicket(string token, AuthorizationInfo userInfo)
        {
            this.storer.Add(token, userInfo);
        }

        /// <summary> Saves a verify ticket. </summary>
        /// <remarks> Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token">    The token. </param>
        /// <param name="userInfo"></param>
        /// <param name="duration"></param>
        public void SaveVerifyTicket(string token, AuthorizationInfo userInfo, double duration)
        {
            this.storer.Add(token, userInfo, duration);
        }

        /// <summary> Get the authorization info. </summary>
        /// <param name="token"> The token. </param>
        /// <returns> The authorization info. </returns>
        public AuthorizationInfo GetAuthorizationInfo(string token)
        {
            return this.storer.Get(token);
        }

        /// <summary> Update Authorization Expire. </summary>
        /// <remarks> Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="duration"></param>
        public void UpdateAuthorizationInfo(string token, double duration)
        {
            this.storer.Update(token, duration);
        }

        /// <summary> Gets hash value. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="token">    The token. </param>
        /// <param name="clientIp"> The client IP. </param>
        /// <returns> The hash value. </returns>
        public string GetHashValue(string token, string clientIp)
        {
            Byte[] clearBytes = Encoding.Default.GetBytes(token + clientIp);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }
    }
}