namespace FreeSun.FS_SMISCloud.Server.CloudApi.Authorization
{
    using System;
    using System.Configuration;
    using System.Runtime.Caching;

    public class MemoryCacheTokenStore : ITokenStore
    {
        private static ObjectCache cache = MemoryCache.Default;
        private double _duration;

        public MemoryCacheTokenStore()
        {
            if (!double.TryParse(ConfigurationManager.AppSettings["TokenExpireDuration"], out this._duration))
            {
                this._duration = 20;
            }
        }

        /// <summary> Adds token. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="info">  The information. </param>
        public void Add(string token, AuthorizationInfo info)
        {
            cache.Set(token, info, DateTimeOffset.Now.AddMinutes(this._duration));
        }

        /// <summary> Adds token. </summary>
        /// <remarks> Updated By Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="info">  The information. </param>
        /// <param name="duration">  The expire duration. </param>
        public void Add(string token, AuthorizationInfo info, double duration)
        {
            cache.Set(token, info, DateTimeOffset.Now.AddMinutes(duration));
        }

        /// <summary> Removes the given token. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="token"> The token. </param>
        public void Remove(string token)
        {
            cache.Remove(token);
        }

        /// <summary> Gets. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="token"> The token. </param>
        /// <returns> An AuthorityInfo. </returns>
        public AuthorizationInfo Get(string token)
        {
            AuthorizationInfo info = cache.Get(token) as AuthorizationInfo;
            if (info != null && info.IsSlideExpire)
            {
                cache.Set(token, info, DateTimeOffset.Now.AddMinutes(this._duration));
            }
            return info;
        }

        /// <summary> Updates this object. </summary>
        /// <remarks> Liuxinyi, 2014-1-3. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="info">  The information. </param>
        public void Update(string token, AuthorizationInfo info)
        {
            cache.Set(token, info, DateTimeOffset.Now.AddMinutes(this._duration));
        }

        /// <summary> Updates this object. </summary>
        /// <remarks> Updated By Liuxinyi, 2014-3-25. </remarks>
        /// <param name="token"> The token. </param>
        /// <param name="duration">  The expire duration. </param>
        public void Update(string token, double duration)
        {
            AuthorizationInfo info = cache.Get(token) as AuthorizationInfo;
            if (info != null)
            {
                cache.Set(token, info, DateTimeOffset.Now.AddMinutes(duration));
            }
        }
    }
}