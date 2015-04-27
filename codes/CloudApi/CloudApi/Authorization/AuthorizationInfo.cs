using System.Collections.Generic;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Authorization
{
    using System;

    public class AuthorizationInfo
    {
        /// <remarks> Updated By Liuxinyi, 2014-3-25. </remarks>
        public AuthorizationInfo()
        {
            this.IsSlideExpire = true;
        }

        public int UserId { get; set; }

        /// <summary> 用户名 </summary>
        /// <remarks> Updated By Liuxinyi, 2014-3-25. </remarks>
        public string UserName { get; set; }

        public string Token { get; set; }

        // 移动设备令牌
        public string DeviceToken { get; set; }

        // 验证码
        public string HashCode { get; set; }

        /// <summary> 是否滑动过期 </summary>
        /// <remarks> Updated By Liuxinyi, 2014-3-25. </remarks>
        public bool IsSlideExpire { get; set; }

        public DateTimeOffset Expire { get; set; }

        public int? RoleId  { get; set; }

        public List<string> AuthorisedResources { get; set; }
    }
}