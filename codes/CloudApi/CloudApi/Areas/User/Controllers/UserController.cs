namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.User.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class UserController : ApiController
    {
        private TokenAuthorizationProvider auth = new TokenAuthorizationProvider();

        /// <summary>
        ///  验证登录：GET user/login/{username}/{password}
        /// </summary>
        /// <param name="username">用户名（只能是字母和数字组成）</param>
        /// <param name="password">密码（只能是字母和数字组成）</param>        
        /// <returns>
        /// authorized 验证结果 false：失败；true：成功
        /// token 用户唯一标识 失败为空
        /// </returns>
        [AcceptVerbs("Get", "Post")]
        [NonAuthorization]
        [LogInfo("登录", true)]
        public HttpResponseMessage Login(string username, string password)
        {
            HttpResponseMessage response;
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                int count = entity.T_DIM_USER.Count(u => u.USER_NAME == username && u.USER_PWD == password && u.USER_IS_ENABLED);
                if (count > 0)
                {
                    Guid guid = Guid.NewGuid();
                    response = this.Request.CreateResponse(
                        HttpStatusCode.OK,
                        new JObject(
                            new JProperty("authorized", true),
                            new JProperty("token", guid.ToString())));
                    int? roleId = null;
                    int userId = -1;
                    var firstOrDefault = entity.T_DIM_USER.FirstOrDefault(u => u.USER_NAME == username);
                    if (firstOrDefault != null)
                    {
                        roleId = firstOrDefault.ROLE_ID;
                        userId = firstOrDefault.USER_NO;
                    }

                    string deviceToken = this.Request.GetQueryString("deviceToken");
                    var authInfo = new AuthorizationInfo
                                       {
                                           UserId = userId,
                                           RoleId = roleId,
                                           DeviceToken = deviceToken,
                                           Token = guid.ToString(),
                                           HashCode =
                                               this.auth.GetHashValue(
                                                   guid.ToString(),
                                                   Request.GetClientIp()),
                                           AuthorisedResources = new List<string>()
                                       };

                    var authorizationResources = from s in entity.T_DIM_ROLE_RESOURCE where s.ROLE_ID == roleId select s.RESOURCE_ID.Trim();
                    authInfo.AuthorisedResources.AddRange(authorizationResources);

                    this.auth.RemoveVerifyTicket(guid.ToString());
                    this.auth.SaveVerifyTicket(guid.ToString(), authInfo);
                    this.Request.Properties["AuthorizationInfo"] = authInfo;

                    // 更新设备令牌
                    if (deviceToken != null)
                    {
                        var item = entity.T_DIM_DEVICETOKEN.Where(d => d.DeviceToken == deviceToken);
                        foreach (var i in item)
                        {
                            var entry = entity.Entry(i);
                            entry.State = System.Data.EntityState.Deleted;
                        }

                        var record = new T_DIM_DEVICETOKEN { DeviceToken = deviceToken, OnlineUser = username };
                        var entry2 = entity.Entry(record);
                        entry2.State = System.Data.EntityState.Added;

                        entity.SaveChanges();
                    }
                }
                else
                {
                    response = this.Request.CreateResponse(
                        HttpStatusCode.OK,
                        new JObject(
                            new JProperty("authorized", false),
                            new JProperty("token", string.Empty)));
                }
            }            

            return response;
        }
        /// <summary>
        /// 登录返回用户信息
        /// </summary>
        public class UserLogin
        {
            public int? USER_NO { get; set; }

            public int? orgid { get; set; }

            public string USER_EMAIL { get; set; }

            public string ABB_NAME_CN { get; set; }

            public int? ROLE_ID { get; set; }

            public string ROLE_CODE { get; set; }

            public string SystemName { get; set; }

            public string Logo { get; set; }
        }
        /// <summary>
        /// 登录并返回用户信息 GET user/login/{username}/{password}/info
        /// </summary>
        /// <param name="username"> 用户名 </param>
        /// <param name="password"> 密码 </param>        
        /// <returns> 用户信息 <see cref="object"/>.
        /// </returns>
        [AcceptVerbs("Get", "Post")]
        [NonAuthorization]
        [LogInfo("登录", true)]
        public HttpResponseMessage LoginAndReturnInfo(string username, string password)
        {
            HttpResponseMessage response;
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var ur = (from r in entity.T_DIM_ROLE
                          from u in entity.T_DIM_USER
                          where u.ROLE_ID == r.ROLE_ID
                           && u.USER_NAME == username
                           && u.USER_PWD == password
                           && u.USER_IS_ENABLED
                          select u.ROLE_ID);
                List<UserLogin> users = new List<UserLogin>();
                if (!ur.ToList().Any())
                {
                    response = this.Request.CreateResponse(
                        HttpStatusCode.OK,
                        new JObject(
                            new JProperty("authorized", false),
                            new JProperty("token", string.Empty),
                            new JProperty("userId", string.Empty),
                            new JProperty("email", string.Empty),
                            new JProperty("orgId", string.Empty),
                            new JProperty("organization", string.Empty),
                            new JProperty("roleId", string.Empty),
                            new JProperty("roleCode", string.Empty),
                            new JProperty("systemName", string.Empty),
                            new JProperty("logo", string.Empty)));

                }
                else
                {
                    var uos = (from u in entity.T_DIM_USER
                             from oo in entity.T_DIM_ORGANIZATION
                             where oo.ID == u.USER_ORG
                             && u.USER_NAME == username
                               && u.USER_PWD == password
                               && u.USER_IS_ENABLED
                             select oo.SystemName);
                    if (ur.FirstOrDefault() == 1)
                    {
                        var query =
                            from u in entity.T_DIM_USER
                            from r in entity.T_DIM_ROLE
                            from o in entity.T_DIM_ORGANIZATION
                            where u.ROLE_ID == r.ROLE_ID
                                  && u.USER_NAME == username
                                  && u.USER_PWD == password
                                  && u.USER_IS_ENABLED
                            select new UserLogin
                            {
                                USER_NO = u.USER_NO,
                                USER_EMAIL = u.USER_EMAIL,
                                orgid = o.ID,
                                ABB_NAME_CN = o.ABB_NAME_CN,
                                ROLE_ID = r.ROLE_ID,
                                ROLE_CODE = r.ROLE_CODE,
                                SystemName = uos.FirstOrDefault() == null ? o.SystemName : uos.FirstOrDefault(),
                                Logo = o.Logo
                            };
                        users = query.ToList();
                    }
                    else
                    {
                        var query =
                            from u in entity.T_DIM_USER
                            from r in entity.T_DIM_ROLE
                            join uo in entity.T_DIM_USER_ORG
                            on u.USER_NO equals uo.USER_NO into org
                            from or in org.DefaultIfEmpty()
                            join o in entity.T_DIM_ORGANIZATION
                            on or.ORGANIZATION_ID equals o.ID
                            into uor
                            from uo in uor.DefaultIfEmpty()
                            where u.ROLE_ID == r.ROLE_ID
                                  && u.USER_NAME == username
                                  && u.USER_PWD == password
                                  && u.USER_IS_ENABLED
                            select new UserLogin
                            {
                                USER_NO = u.USER_NO,
                                USER_EMAIL = u.USER_EMAIL,
                                orgid = uo.ID,
                                ABB_NAME_CN = uo.ABB_NAME_CN,
                                ROLE_ID = r.ROLE_ID,
                                ROLE_CODE = r.ROLE_CODE,
                                SystemName = uos.FirstOrDefault() == null ? uo.SystemName : uos.FirstOrDefault(),
                                Logo = uo.Logo
                            };
                        users = query.ToList();
                    }
                    //var users = query.ToList();
                    if (users.Any())
                    {
                        Guid guid = Guid.NewGuid();
                        var info = users.First();
                        response = this.Request.CreateResponse(
                            HttpStatusCode.OK,
                            new JObject(
                                new JProperty("authorized", true),
                                new JProperty("token", guid.ToString()),
                                new JProperty("userId", info.USER_NO),
                                new JProperty("email", info.USER_EMAIL),
                                new JProperty("orgId", info.orgid),
                                new JProperty("organization", info.ABB_NAME_CN),
                                new JProperty("roleId", info.ROLE_ID),
                                new JProperty("roleCode", info.ROLE_CODE),
                                new JProperty("systemName", info.SystemName),
                                new JProperty("logo", info.Logo)));

                        this.auth.RemoveVerifyTicket(guid.ToString());
                        string deviceToken = this.Request.GetQueryString("deviceToken");
                        var authInfo = new AuthorizationInfo
                                           {
                                               UserId = (int)info.USER_NO,
                                               RoleId = info.ROLE_ID,
                                               DeviceToken = deviceToken,
                                               Token = guid.ToString(),
                                               HashCode =
                                                   this.auth.GetHashValue(
                                                       guid.ToString(),
                                                       Request.GetClientIp()),
                                               AuthorisedResources = new List<string>()
                                           };

                        var authorizationResources = from s in entity.T_DIM_ROLE_RESOURCE where s.ROLE_ID == info.ROLE_ID select s.RESOURCE_ID.Trim();
                        authInfo.AuthorisedResources.AddRange(authorizationResources);

                        this.auth.SaveVerifyTicket(guid.ToString(), authInfo);
                        this.Request.Properties["AuthorizationInfo"] = authInfo;

                        // 更新移动令牌
                        if (deviceToken != null)
                        {
                            var item = entity.T_DIM_DEVICETOKEN.Where(d => d.DeviceToken == deviceToken);
                            foreach (var i in item)
                            {
                                var entry = entity.Entry(i);
                                entry.State = System.Data.EntityState.Deleted;
                            }

                            var record = new T_DIM_DEVICETOKEN { DeviceToken = deviceToken, OnlineUser = username };
                            var entry2 = entity.Entry(record);
                            entry2.State = System.Data.EntityState.Added;

                            entity.SaveChanges();
                        }
                    }
                    else
                    {
                        response = this.Request.CreateResponse(
                            HttpStatusCode.OK,
                            new JObject(
                                new JProperty("authorized", false),
                                new JProperty("token", string.Empty),
                                new JProperty("userId", string.Empty),
                                new JProperty("email", string.Empty),
                                new JProperty("orgId", string.Empty),
                                new JProperty("organization", string.Empty),
                                new JProperty("roleId", string.Empty),
                                new JProperty("roleCode", string.Empty),
                                new JProperty("systemName", string.Empty),
                                new JProperty("logo", string.Empty)));
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="token"> The token. </param>
        /// <returns> 登出结果 </returns>
        [NonAuthorization]
        [AcceptVerbs("Get", "POST")]
        [LogInfo("退出登录", true)]
        public HttpResponseMessage Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    StringHelper.GetMessageString("token无效,登出失败"));
            }

            this.auth.RemoveVerifyTicket(token);
            // 删除移动设备令牌
            string deviceToken = this.Request.GetQueryString("deviceToken");
            if (deviceToken != null)
            {
                using (var db = new SecureCloud_Entities())
                {
                    var item = db.T_DIM_DEVICETOKEN.Where(d => d.DeviceToken == deviceToken);
                    if (item.Any())
                    {
                        foreach (var i in item)
                        {
                            i.OnlineUser = null;
                        }
                    }

                    db.SaveChanges();
                }
            }

            return Request.CreateResponse(
                HttpStatusCode.Accepted,
                StringHelper.GetMessageString("登出成功"));
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns>修改结果</returns>
        [NonAuthorization]
        [AcceptVerbs("Post")]
        [LogInfo("修改用户信息", true)]
        public HttpResponseMessage ModifyInfo([FromBody]UserInfo info)
        {
            #region 日志信息
            this.Request.Properties["AuthorizationInfo"] = new AuthorizationInfo
                                                                   {
                                                                       UserId = info.UserId,
                                                                       Token = string.Empty
                                                                   };
            this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(info);
            if (!string.IsNullOrEmpty(info.NewPwd))
            {
                this.Request.Properties["ActionParameterShow"] = string.Format(
                    "新密码：{0}",
                    StringHelper.Confuse(info.NewPwd));
            }
            else if (!string.IsNullOrEmpty(info.Email))
            {
                this.Request.Properties["ActionParameterShow"] = string.Format(
                    "新邮箱:{0}",
                    StringHelper.Confuse(info.Email));
            } 
            #endregion

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                T_DIM_USER user;
                if (info.UserId != default(int))
                {
                    user = entity.T_DIM_USER.FirstOrDefault(u => u.USER_NO == info.UserId);
                }
                else
                {
                    user = entity.T_DIM_USER.FirstOrDefault(u => u.USER_NAME == info.UserName);
                }

                if (user == null)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("用户不存在"));
                }

                if (user.USER_PWD != info.OldPwd)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("密码有误"));
                }

                if (!string.IsNullOrEmpty(info.NewPwd)) // 修改密码
                {
                    user.USER_PWD = info.NewPwd;
                }
                else if (!string.IsNullOrEmpty(info.Email)) // 修改邮箱
                {
                    user.USER_EMAIL = info.Email;
                }
                else
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("内容不完整"));
                }

                var entry = entity.Entry(user);
                entry.State = System.Data.EntityState.Modified;

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("修改成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改失败"));
                }
            }            
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns>重置结果</returns>        
        [AcceptVerbs("Post")]
        [NonAuthorization]
        [LogInfo("重置密码", true)]
        public HttpResponseMessage ResetPassword([FromBody]UserInfo info)
        {
            #region 日志信息
            this.Request.Properties["AuthorizationInfo"] = new AuthorizationInfo
                {
                    UserId = info.UserId,
                    Token = string.Empty
                };
            this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(info);
            if (!string.IsNullOrEmpty(info.Email))
            {
                this.Request.Properties["ActionParameterShow"] = string.Format(
                    "使用邮箱：{0}",
                    StringHelper.Confuse(info.Email));
            } 
            #endregion

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var email = entity.T_DIM_USER.FirstOrDefault(u => u.USER_EMAIL == info.Email);

                if (email == null)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("用户未填写email"));
                }

                // 发送邮件
                return Request.CreateResponse(
                    HttpStatusCode.Accepted,
                    StringHelper.GetMessageString("已发送重置邮件"));
            }
        }
        /// <summary>
        /// 获取用户列表信息
        /// </summary>
        public class UserListInfo
        {
            public int? userId { get; set; }

            public string userName { get; set; }

            public string password { get; set; }

            public string  email { get; set; }

            public string phone { get; set; }

            public int? roleId { get; set; }

            public string roleCode { get; set; }

            public int? orgId { get; set; }

            public string orgName { get; set; }

            public int? stcId { get; set; }

            public string stcName { get; set; }
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户列表", false)]
        [Authorization(AuthorizationCode.S_User)]
        public object GetUsers(int userId)
        {
            using (var db = new SecureCloud_Entities())
            {
                 var ur = (from r in db.T_DIM_ROLE
                          from u in db.T_DIM_USER
                          where u.ROLE_ID == r.ROLE_ID
                           && u.USER_NO == userId
                           && u.USER_IS_ENABLED
                          select u.ROLE_ID);

                 if (!ur.ToList().Any())
                 {
                     return ur.ToList();
                 }
                 else
                 {
                     List<UserListInfo> data = new List<UserListInfo>();
                     if (ur.FirstOrDefault() == 1)
                     {
                         var query = from u in db.T_DIM_USER
                                     join r in db.T_DIM_ROLE on u.ROLE_ID equals r.ROLE_ID into r1
                                     from role in r1.DefaultIfEmpty()
                                     join uo in db.T_DIM_USER_ORG on u.USER_NO equals uo.USER_NO into uo1
                                     from userOrg in uo1.DefaultIfEmpty()
                                     join o in db.T_DIM_ORGANIZATION on userOrg.ORGANIZATION_ID equals o.ID into o1
                                     from org in o1.DefaultIfEmpty()
                                     join us in db.T_DIM_USER_STRUCTURE on u.USER_NO equals us.USER_NO into us1
                                     from userStc in us1.DefaultIfEmpty()
                                     join uos in db.T_DIM_ORG_STUCTURE on userStc.STRUCTURE_ID equals uos.STRUCTURE_ID into uos1
                                     from userOrgStc in uos1
                                     join s in db.T_DIM_STRUCTURE on userOrgStc.STRUCTURE_ID equals s.ID into s1
                                     from stc in s1.DefaultIfEmpty()
                                     where u.USER_IS_ENABLED && u.ROLE_ID != 1
                                     select new UserListInfo
                                     {
                                         userId = u.USER_NO,
                                         userName = u.USER_NAME,
                                         password = u.USER_PWD,
                                         email = u.USER_EMAIL,
                                         phone = u.USER_PHONE,
                                         roleId = u.ROLE_ID,
                                         roleCode = role.ROLE_DESCRIPTION,
                                         orgId = org.ID,
                                         orgName = org.ABB_NAME_CN,
                                         stcId = stc.ID,
                                         stcName = stc.STRUCTURE_NAME_CN
                                     };
                         data = query.ToList();
                     }
                     else
                     {
                         var query = from u in db.T_DIM_USER
                                     join r in db.T_DIM_ROLE on u.ROLE_ID equals r.ROLE_ID into r1
                                     from role in r1.DefaultIfEmpty()
                                     join uo in db.T_DIM_USER_ORG on u.USER_NO equals uo.USER_NO into uo1
                                     from userOrg in uo1.DefaultIfEmpty()
                                     join o in db.T_DIM_ORGANIZATION on userOrg.ORGANIZATION_ID equals o.ID into o1
                                     from org in o1.DefaultIfEmpty()
                                     join us in db.T_DIM_USER_STRUCTURE on u.USER_NO equals us.USER_NO into us1
                                     from userStc in us1.DefaultIfEmpty()
                                     join uos in db.T_DIM_ORG_STUCTURE on userStc.STRUCTURE_ID equals uos.STRUCTURE_ID into uos1
                                     from userOrgStc in uos1
                                     join s in db.T_DIM_STRUCTURE on userOrgStc.STRUCTURE_ID equals s.ID into s1
                                     from stc in s1.DefaultIfEmpty()
                                     where u.USER_IS_ENABLED
                                     && u.USER_NO == userId && u.ROLE_ID != 1
                                     select new UserListInfo
                                     {
                                         userId = u.USER_NO,
                                         userName = u.USER_NAME,
                                         password = u.USER_PWD,
                                         email = u.USER_EMAIL,
                                         phone = u.USER_PHONE,
                                         roleId = u.ROLE_ID,
                                         roleCode = role.ROLE_DESCRIPTION,
                                         orgId = org.ID,
                                         orgName = org.ABB_NAME_CN,
                                         stcId = stc.ID,
                                         stcName = stc.STRUCTURE_NAME_CN
                                     };
                         data = query.ToList();
                     }
                     return
                  data.GroupBy(
                      d => new { d.userId, d.userName, d.password, d.email, d.phone, d.roleId, d.roleCode })
                      .Select(
                          g =>
                          new
                          {
                              g.Key.userId,
                              g.Key.userName,
                              g.Key.password,
                              g.Key.email,
                              g.Key.phone,
                              g.Key.roleId,
                              g.Key.roleCode,
                              orgs =
                          g.Select(o => o.orgId).FirstOrDefault() != null
                              ? g.Select(o => new { id = o.orgId, name = o.orgName }).Distinct()
                              : null,
                              structs =
                          g.Select(o => o.stcId).FirstOrDefault() != null
                              ? g.Select(s => new { id = s.stcId, name = s.stcName }).Distinct()
                              : null
                          });
                 }
            }
        }

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取单个用户信息", false)]
        [Authorization(AuthorizationCode.S_User)]
        [Authorization(AuthorizationCode.U_Common)]
        public object GetUserInfo(int userId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from u in db.T_DIM_USER
                            join r in db.T_DIM_ROLE on u.ROLE_ID equals r.ROLE_ID into r1
                            from role in r1.DefaultIfEmpty()
                            join uo in db.T_DIM_USER_ORG on u.USER_NO equals uo.USER_NO into uo1
                            from userOrg in uo1.DefaultIfEmpty()
                            join o in db.T_DIM_ORGANIZATION on userOrg.ORGANIZATION_ID equals o.ID into o1
                            from org in o1.DefaultIfEmpty()

                            join beOrg in db.T_DIM_ORGANIZATION on u.USER_ORG equals beOrg.ID into beO1
                            from beOrg in beO1.DefaultIfEmpty()

                            join us in db.T_DIM_USER_STRUCTURE on u.USER_NO equals us.USER_NO into us1
                            from userStc in us1.DefaultIfEmpty()
                            join s in db.T_DIM_STRUCTURE on userStc.STRUCTURE_ID equals s.ID into s1
                            from stc in s1.DefaultIfEmpty()
                            where u.USER_IS_ENABLED && u.USER_NO == userId
                            select
                                new
                                {
                                    userId = u.USER_NO,
                                    userName = u.USER_NAME,
                                    password = u.USER_PWD,
                                    email = u.USER_EMAIL,
                                    phone = u.USER_PHONE,
                                    roleId = u.ROLE_ID,
                                    roleCode = role.ROLE_DESCRIPTION,
                                    orgId = (int?)org.ID,
                                    orgName = org.ABB_NAME_CN,
                                    stcId = (int?)stc.ID,
                                    stcName = stc.STRUCTURE_NAME_CN,
                                    beOrgName = beOrg.ABB_NAME_CN 
                                };
                var data = query.ToList();

                return
                    data.GroupBy(
                        d => new { d.userId, d.userName, d.password, d.email, d.phone, d.roleId, d.roleCode ,d.beOrgName})
                        .Select(
                            g =>
                            new
                            {
                                g.Key.userId,
                                g.Key.userName,
                                g.Key.password,
                                g.Key.email,
                                g.Key.phone,
                                g.Key.roleId,
                                g.Key.roleCode,
                                g.Key.beOrgName,
                                orgs =
                            g.Select(o => o.orgId).FirstOrDefault() != null
                                ? g.Select(o => new { id = o.orgId, name = o.orgName }).Distinct()
                                : null,
                                structs =
                            g.Select(o => o.stcId).FirstOrDefault() != null
                                ? g.Select(s => new { id = s.stcId, name = s.stcName }).Distinct()
                                : null
                            }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>添加结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("新增用户", true)]
        [Authorization(AuthorizationCode.S_User_Add)]
        public HttpResponseMessage Add([FromBody] User user)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var newUser = new T_DIM_USER
                                      {
                                          USER_NAME = user.UserName,
                                          USER_PWD = user.Password,
                                          USER_EMAIL = user.Email,
                                          USER_PHONE = user.Phone,
                                          USER_IS_TRIL = false,
                                          USER_IS_ENABLED = true,
                                          ROLE_ID = user.RoleId,
                                          USER_ORG=user.beOrg
                                          
                                      };
                    db.T_DIM_USER.Add(newUser);
                    db.SaveChanges();
                    int userId = newUser.USER_NO;

                    string sbOrg = string.Empty;
                    // 配置组织  
                    if (user.Orgs != null)
                    {
                        var orgs = user.Orgs.Split(',').Select(o => Convert.ToInt32(o));

                        var org = db.T_DIM_ORGANIZATION.Where(o => orgs.Contains(o.ID)).Select(o=>o.ABB_NAME_CN);
                        sbOrg = string.Join(",", org);

                        foreach (var orgId in orgs)
                        {
                            var uo = new T_DIM_USER_ORG { USER_NO = userId, ORGANIZATION_ID = orgId };
                            var entry2 = db.Entry(uo);
                            entry2.State = System.Data.EntityState.Added;                            
                        }
                    }

                    string sbStc = string.Empty;
                    //配置结构物
                    if (user.Structs != null)
                    {
                        var stcs = user.Structs.Split(',').Select(o => Convert.ToInt32(o));
                        var st = db.T_DIM_STRUCTURE.Where(s => stcs.Contains(s.ID)).Select(s => s.STRUCTURE_NAME_CN);
                        sbStc = string.Join(",", st);

                        foreach (var stcId in stcs)
                        {
                            var us = new T_DIM_USER_STRUCTURE { USER_NO = userId, STRUCTURE_ID = stcId };
                            var entry3 = db.Entry(us);
                            entry3.State = System.Data.EntityState.Added;
                        }
                    }
                    db.SaveChanges();

                    var role = db.T_DIM_ROLE.FirstOrDefault(r => r.ROLE_ID == user.RoleId);
                    var roleName = role != null ? role.ROLE_DESCRIPTION : string.Empty;

                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(user);
                    this.Request.Properties["ActionParameterShow"] 
                        = string.Format("用户名：{0}，密码：{1},邮箱：{2},联系电话：{3},角色：{4},所属组织:{5},关注结构物:{6}", 
                        user.UserName,
                        StringHelper.Confuse(user.Password),
                        StringHelper.Confuse(user.Email),
                        user.Phone,roleName,
                        sbOrg,sbStc);

                    #endregion

                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("用户添加成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("用户添加失败"));
                }                
            }
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="user">用户对象</param>
        /// <returns>修改结果</returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改用户信息", true)]
        [Authorization(AuthorizationCode.S_User_Update)]
        public HttpResponseMessage Modify([FromUri] int userId, [FromBody] User user)
        {
            using (var db = new SecureCloud_Entities())
            {
                var paraShow = new StringBuilder(20);
                try
                {
                    var userEntity = db.T_DIM_USER.FirstOrDefault(u => u.USER_NO == userId);

                    if (userEntity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("用户不存在"));
                    }
                    paraShow.AppendFormat("用户名：{0},", userEntity.USER_NAME);

                    if (user.Password != default(string))
                    {
                        userEntity.USER_PWD = user.Password;
                        paraShow.AppendFormat("密码改为：{0}，", StringHelper.Confuse(user.Password));
                    }
                    if (user.Email != default(string))
                    {
                        userEntity.USER_EMAIL = user.Email;
                        paraShow.AppendFormat("邮箱改为：{0}，", StringHelper.Confuse(user.Email));
                    }
                    if (user.Phone != default(string))
                    {
                        userEntity.USER_PHONE = user.Phone;
                        paraShow.AppendFormat("联系电话改为：{0}，", user.Phone);
                    }
                    if (user.RoleId != default(int))
                    {
                        userEntity.ROLE_ID = user.RoleId;
                        var role = db.T_DIM_ROLE.FirstOrDefault(r => r.ROLE_ID == user.RoleId);
                        var roleName = role != null ? role.ROLE_DESCRIPTION : string.Empty;
                        paraShow.AppendFormat("角色改为：{0}，", roleName);
                    }

                    // 配置组织 
                    if (user.Orgs != null || user.Orgs == "clear")
                    {
                        var queryOrg = from uo in db.T_DIM_USER_ORG where uo.USER_NO == userId select uo;
                        foreach (var userOrg in queryOrg)
                        {
                            var entry = db.Entry(userOrg);
                            entry.State = System.Data.EntityState.Deleted;
                        }

                        if (user.Orgs == "clear")
                        {
                            paraShow.AppendFormat("清除所属组织单位，");
                        }

                        if (user.Orgs.Length != 0 && user.Orgs != "clear")
                        {
                            var orgs = user.Orgs.Split(',').Select(o => Convert.ToInt32(o));
                            var org = db.T_DIM_ORGANIZATION.Where(o => orgs.Contains(o.ID)).Select(o=>o.ABB_NAME_CN);
                            paraShow.AppendFormat("所属组织单位改为：{0},", string.Join(",", org));

                            foreach (var orgId in orgs)
                            {
                                var uo = new T_DIM_USER_ORG { USER_NO = userId, ORGANIZATION_ID = orgId };
                                var entry2 = db.Entry(uo);
                                entry2.State = System.Data.EntityState.Added;
                            }
                        }
                    }

                    //配置结构物
                    if (user.Structs != null || user.Structs == "clear")
                    {
                        var queryStc = from us in db.T_DIM_USER_STRUCTURE where us.USER_NO == userId select us;
                        foreach (var userStructure in queryStc)
                        {
                            var entry = db.Entry(userStructure);
                            entry.State = System.Data.EntityState.Deleted;
                        }

                        if (user.Orgs == "clear")
                        {
                            paraShow.AppendFormat("清除关注结构物，");
                        }

                        if (user.Structs.Length != 0 && user.Structs != "clear")
                        {
                            var stcs = user.Structs.Split(',').Select(o => Convert.ToInt32(o));
                            var stc = db.T_DIM_STRUCTURE.Where(o => stcs.Contains(o.ID)).Select(o => o.STRUCTURE_NAME_CN);
                            paraShow.AppendFormat("关注结构物改为：{0},", string.Join(",", stc));

                            foreach (var stcId in user.Structs.Split(',').Select(o => Convert.ToInt32(o)))
                            {
                                var us = new T_DIM_USER_STRUCTURE { USER_NO = userId, STRUCTURE_ID = stcId };
                                var entry3 = db.Entry(us);
                                entry3.State = System.Data.EntityState.Added;
                            }
                        }
                    }

                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(user);
                    this.Request.Properties["ActionParameterShow"] = paraShow.ToString();                        

                    #endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("用户信息修改成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("用户信息修改失败"));
                }
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除用户", true)]
        [Authorization(AuthorizationCode.S_User_Delete)]
        public HttpResponseMessage Remove([FromUri] int userId)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var user = db.T_DIM_USER.FirstOrDefault(u => u.USER_NO == userId);

                    if (user == null || !user.USER_IS_ENABLED)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("用户不存在"));
                    }

                    user.USER_IS_ENABLED = false;

                    #region 日志信息
                    this.Request.Properties["ActionParameter"] = "userId:" + userId;
                    this.Request.Properties["ActionParameterShow"] = "用户名：" + user.USER_NAME;

                    #endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("用户删除成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("用户删除失败"));
                }
            }
        }

        /// <summary>
        /// 判定用户名是否存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>是否存在</returns>
        [NonAuthorization]
        [AcceptVerbs("Get")]
        [LogInfo("判断用户名是否存在", false)]
        public bool CheckUserExists(string username)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = db.T_DIM_USER.Where(u => u.USER_NAME == username);
                return query.Any();
            }
        }


        /// <summary>
        /// 判断当前用户是否拥有指定权限
        /// </summary>
        /// <param name="resId">资源ID字符串</param>
        /// <returns>是否存在</returns>
        [NonAuthorization]
        [AcceptVerbs("Post")]
        [LogInfo("判断当前用户是否拥有指定权限", false)]
        public bool CheckRoleResource(string resId)
        {
            var token = this.Request.GetQueryString("token");
            var userInfo = new TokenAuthorizationProvider().GetAuthorizationInfo(token);
            if (userInfo == null) return false;
            if (userInfo.RoleId == 1) return true;
            return userInfo.AuthorisedResources.Contains(resId.Trim());
        }
    }

    /// <summary>
    /// 用户模型
    /// </summary>
    public class User
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int RoleId { get; set; }

        public int beOrg { get; set; }  

        public string Orgs { get; set; }

        public string Structs { get; set; }        
    }

    /// <summary>
    /// 用户信息model
    /// </summary>
    public class UserInfo
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
     
        public string OldPwd { get; set; }

        public string NewPwd { get; set; }

        public string Email { get; set; }
    }
}
