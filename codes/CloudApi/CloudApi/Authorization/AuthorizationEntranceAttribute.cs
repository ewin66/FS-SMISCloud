// --------------------------------------------------------------------------------------------
// <copyright file="AuthorityEntranceAttribute.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2013 飞尚科技
// 版权所有。
// </copyright>
// <summary>
// 文件功能描述：验证入口特性
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

    /// <summary> Attribute for authority entrance. </summary>
    /// <remarks> Liuxinyi, 2013-12-27. </remarks>
    public class NonAuthorizationAttribute : Attribute
    {
    }
}