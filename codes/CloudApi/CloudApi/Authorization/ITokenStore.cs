// --------------------------------------------------------------------------------------------
// <copyright file="ITokenStore.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
// 文件功能描述：Token存储接口
//
// 创建标识：Liuxinyi2014-1-3
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
    /// <summary> Interface for token store. </summary>
    /// <remarks> Liuxinyi, 2014-1-3. </remarks>
    public interface ITokenStore
    {
        /// <summary> Adds token. </summary>
        /// <param name="token"> The token. </param>
        /// <param name="info">  The information. </param>
        void Add(string token, AuthorizationInfo info);

        /// <summary> Adds token. </summary>
        /// <remarks> Updated By Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="info">  The information. </param>
        /// <param name="duration">  The expire duration. </param>
        void Add(string token, AuthorizationInfo info, double duration);

        /// <summary> Removes the given token. </summary>
        /// <param name="token"> The token. </param>
        void Remove(string token);

        /// <summary> Gets. </summary>
        /// <param name="token"> The token. </param>
        /// <returns> An AuthorityInfo. </returns>
        AuthorizationInfo Get(string token);

        /// <summary> Updates this object. </summary>
        /// <param name="token"> The token. </param>
        /// <param name="info">  The information. </param>
        void Update(string token, AuthorizationInfo info);

        /// <summary> Updates this object. </summary>
        /// <remarks> Updated By Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="duration">  The expire duration. </param>
        void Update(string token, double duration);
    }
}