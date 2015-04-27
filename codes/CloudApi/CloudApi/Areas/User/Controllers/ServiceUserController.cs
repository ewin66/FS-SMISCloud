using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.User.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json.Linq;

    public class ServiceUserController : ApiController
    {
        private TokenAuthorizationProvider auth = new TokenAuthorizationProvider();

        private const int RoleId = 6;
        private const double DefaultDuration = 30;

        [AcceptVerbs("Post")]
        [NonAuthorization]
        [LogInfo("数据服务申请Token", true)]
        public object GetToken([FromBody]TokenApplyContext ctx)
        {
            var user = DataService.GetServiceUser(ctx.UserName);
            if (user != null)
            {
                var md5Rslt = CalcMd5(ctx.UserName, ctx.Random, user.ApiKey);
                var guid = Guid.NewGuid();
                while (auth.GetAuthorizationInfo(guid.ToString()) != null)
                {
                    guid = Guid.NewGuid();
                }
                if(System.String.Compare(md5Rslt, ctx.Sig, System.StringComparison.OrdinalIgnoreCase)==0)
                {
                    var authInfo = new AuthorizationInfo
                                       {
                                           UserName = user.UserName,
                                           RoleId = RoleId,
                                           IsSlideExpire = false,
                                           Expire = DateTimeOffset.Now.AddMinutes(DefaultDuration),
                                           Token = guid.ToString(),
                                           HashCode =
                                               this.auth.GetHashValue(
                                                   guid.ToString(),
                                                   Request.GetClientIp()),
                                           AuthorisedResources = new List<string>()
                                       };
                    using (var entity = new SecureCloud_Entities())
                    {
                        var authorizationResources = from s in entity.T_DIM_ROLE_RESOURCE
                                                     where s.ROLE_ID == RoleId
                                                     select s.RESOURCE_ID.Trim();
                        authInfo.AuthorisedResources.AddRange(authorizationResources);

                        this.auth.RemoveVerifyTicket(guid.ToString());
                        this.auth.SaveVerifyTicket(guid.ToString(), authInfo, DefaultDuration);
                        this.Request.Properties["AuthorizationInfo"] = authInfo;
                    }

                    return this.Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        new JObject(new JProperty("res", true), new JProperty("token", guid)));
                }

                return this.Request.CreateResponse(
                    HttpStatusCode.Accepted,
                    new JObject(new JProperty("res", false), new JProperty("token", string.Empty)));

            }

            return this.Request.CreateResponse(
                HttpStatusCode.Accepted,
                new JObject(new JProperty("res", false), new JProperty("token", string.Empty)));
        }

        [AcceptVerbs("Post")]
        [NonAuthorization]
        [LogInfo("数据服务更新Token过期时间", true)]
        public object UpdateToken([FromBody]TokenConfigContext ctx)
        {
            var authInfo = auth.GetAuthorizationInfo(ctx.Token);
            if (authInfo != null)
            {
                var user = DataService.GetServiceUser(authInfo.UserName);
                if (user != null)
                {
                    var md5Rslt = CalcMd5(authInfo.UserName, ctx.Random, user.ApiKey);
                    if (System.String.Compare(md5Rslt, ctx.Sig, System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var dateOffset = authInfo.Expire - DateTimeOffset.Now;
                        if (dateOffset.TotalMinutes > 0 && dateOffset.TotalMinutes < 10)
                        {
                            auth.UpdateAuthorizationInfo(ctx.Token, DefaultDuration);
                            return this.Request.CreateResponse(
                                HttpStatusCode.Accepted,
                                new JObject(new JProperty("res", true)));
                        }
                    }
                }
            }

            return this.Request.CreateResponse(
                HttpStatusCode.Accepted,
                new JObject(new JProperty("res", false)));
        }

        [AcceptVerbs("Post")]
        [NonAuthorization]
        [LogInfo("数据服务销毁Token", true)]
        public object DropToken([FromBody]TokenConfigContext ctx)
        {
            var authInfo = auth.GetAuthorizationInfo(ctx.Token);
            if (authInfo != null)
            {
                var user = DataService.GetServiceUser(authInfo.UserName);
                if (user != null)
                {
                    var md5Rslt = CalcMd5(authInfo.UserName, ctx.Random, user.ApiKey);
                    if (System.String.Compare(md5Rslt, ctx.Sig, System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        auth.RemoveVerifyTicket(ctx.Token);

                        return this.Request.CreateResponse(
                            HttpStatusCode.Accepted,
                            new JObject(new JProperty("res", true)));
                    }
                }
            }

            return this.Request.CreateResponse(
                HttpStatusCode.Accepted,
                new JObject(new JProperty("res", false)));
        }

        private string CalcMd5(string username, string random, string apikey)
        {
            Byte[] clearBytes = Encoding.Default.GetBytes(username + random + apikey);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }
    }

    public class TokenApplyContext
    {
        public string UserName { get; set; }

        public string Random { get; set; }

        public string Sig { get; set; }
    }

    public class TokenConfigContext
    {
        public string Token { get; set; }

        public string Random { get; set; }

        public string Sig { get; set; }
    }
}