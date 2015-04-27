namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Structure.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;    

    public class StructureController : ApiController
    {
        /// <summary>
        /// GET user/{userId}/structs
        /// </summary>
        /// <param name="userId">用户名（只能是数字组成）</param>
        /// <returns>结构物列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户结构物", false)]
        public IEnumerable<StructModel> FindStructsByUserName(int userId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from struc in entity.T_DIM_STRUCTURE
                    from us in entity.T_DIM_USER_STRUCTURE
                    from user in entity.T_DIM_USER
                    where struc.ID == us.STRUCTURE_ID
                          && us.USER_NO == user.USER_NO
                          && user.USER_NO == userId
                          && struc.IsDelete != 1
                    select new
                    {
                        StructId = struc.ID,
                        StructName = struc.STRUCTURE_NAME_CN,
                        ImageName = struc.Imagename,
                        Longitude = struc.STRUCTURE_LONGITUDE,
                        Latitude = struc.STRUCTURE_LATITUDE,
                        ProjectStatus = struc.ProjectStatus,
                        Score = (from s in entity.T_FACT_STRUCTURE_SCORE
                                  from os in entity.T_DIM_ORG_STUCTURE
                                  where s.ORG_STRUC_ID == os.ORG_STRUC_ID
                                      && struc.ID == os.STRUCTURE_ID
                                  orderby s.EVALUATION_DATETIME descending
                                  select s.STRUCTURE_SCORE).FirstOrDefault()
                    };
                var list =
                    query.ToList()
                        .Select(
                            d =>
                            new StructModel
                                {
                                    StructId = d.StructId,
                                    StructName = d.StructName,
                                    ImageName = d.ImageName,
                                    Longitude = d.Longitude,
                                    Latitude = d.Latitude,
                                    ProjectStatus =
                                        d.ProjectStatus == null
                                            ? string.Empty
                                            : ((ProjectStatus)d.ProjectStatus).ToString(),
                                    Score = d.Score
                                })
                        .ToList();

                var arr = list.Select(s => s.StructId).ToArray();
                if (arr.Length == 0) return new List<StructModel>();

                DataTable warn = new DAL.Warning().GetTopWarning(arr);

                foreach (var row in warn.AsEnumerable())
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].StructId == row.Field<int>("StructId"))
                        {
                            list[i].WorstWarning = new Warning
                            {
                                Source = row.Field<string>("Source"),
                                Level = row["WarningLevel"] == DBNull.Value ? 4 : row.Field<byte>("WarningLevel"),
                                Time = row.Field<DateTime>("Time")
                            };
                            break;
                        }
                    }
                }

                foreach (var structModel in list)
                {
                    structModel.Status = "无";
                    if (structModel.Score == null)
                    {
                        continue;
                    }
                    if (structModel.Score >= 0 && structModel.Score < 20)
                    {
                        structModel.Status = "差";
                    }
                    else if (structModel.Score >= 20 && structModel.Score < 40)
                    {
                        structModel.Status = "劣";
                    }
                    else if (structModel.Score >= 40 && structModel.Score < 60)
                    {
                        structModel.Status = "中";
                    }
                    else if (structModel.Score >= 60 && structModel.Score < 80)
                    {
                        structModel.Status = "良";
                    }
                    else if (structModel.Score >= 80 && structModel.Score <= 100)
                    {
                        structModel.Status = "优";
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// GET user/structs
        /// </summary>
        /// <returns>结构物列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("查询当前服务用户的结构物", false)]
        public IEnumerable<Object> FindStructsOfCurrentServiceUser()
        {
            var token = this.Request.GetQueryString("token");
            var userInfo = new TokenAuthorizationProvider().GetAuthorizationInfo(token);
            if (userInfo == null) return new List<StructModel>();
            var structlist2 = DataService.GetServiceUserStruct(userInfo.UserName);
            if (structlist2 == null || structlist2.Count == 0) return new List<StructModel>();
            var structlist = from s in structlist2
                             select Convert.ToInt32(s);
            using (var entity = new SecureCloud_Entities())
            {
                var query = from st in entity.T_DIM_STRUCTURE
                            from si in structlist
                            where st.ID == si
                            select new
                            {
                                StructId = st.ID,
                                StructName = st.STRUCTURE_NAME_CN,
                                Longitude = st.STRUCTURE_LONGITUDE,
                                Latitude = st.STRUCTURE_LATITUDE,
                                ProjectStatus = st.ProjectStatus
                            };
                var lst = from d in query.ToList()
                          select new
                          {
                              StructId = d.StructId,
                              StructName = d.StructName,
                              Longitude = d.Longitude,
                              Latitude = d.Latitude,
                              ProjectStatus =
                                  d.ProjectStatus == null
                                      ? string.Empty
                                      : ((ProjectStatus)d.ProjectStatus).ToString()
                          };


                return lst;
            }
        }
        /// <summary>
        /// 获取组织下结构物信息 
        /// </summary>
        public class OrgStruct
        {
            public int? structId { get; set; }

            public string structName { get; set; }

            public string structType { get; set; }

            public string region { get; set; }

            public string street { get; set; }

            public decimal? longitude { get; set; }

            public decimal? latitude { get; set; }

            public int? projectStatus { get; set; }

            public string consCompany { get; set; }

            public string description { get; set; }

            public string imageName { get; set; }
        }
        /// <summary>
        /// Get user/{userId}/org/{orgId}/structs
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="orgId">组织编号</param>
        /// <returns>结构物列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取组织结构物", false)]
        public object FindStructsByOrg(int userId, int orgId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                 var ur = (from r in entity.T_DIM_ROLE
                          from u in entity.T_DIM_USER
                          where u.ROLE_ID == r.ROLE_ID
                           && u.USER_NO == userId
                           && u.USER_IS_ENABLED
                          select u.ROLE_ID);

                 if (!ur.ToList().Any())
                 {
                     return ur.ToList();
                 }
                 else {
                     List<OrgStruct> list = new List<OrgStruct>();
                     if (ur.FirstOrDefault() == 1)
                     {
                         var query = from o in entity.T_DIM_ORG_STUCTURE
                                     from s in entity.T_DIM_STRUCTURE
                                     from st in entity.T_DIM_STRUCTURE_TYPE
                                     where o.ORGANIZATION_ID == orgId && o.STRUCTURE_ID == s.ID
                                     && s.IsDelete != 1 && s.STRUCTURE_TYPE_ID == st.ID 
                                     select new OrgStruct
                                     {
                                         structId = s.ID,
                                         structName = s.STRUCTURE_NAME_CN,
                                         structType = st.NAME_STRUCTURE_TYPE_CN,
                                         region = s.RegionPath,
                                         street = s.STRUCTURE_DETAIL_ADDRESS,
                                         longitude = s.STRUCTURE_LONGITUDE,
                                         latitude = s.STRUCTURE_LATITUDE,
                                         projectStatus = s.ProjectStatus,
                                         consCompany = s.CONSTRUCTION_COMPANY_NAME,
                                         description = s.DESCRIPTION,
                                         imageName = s.Imagename
                                     };
                         list = query.ToList();
                     }
                     else {
                         /****start*用户关注结构物*****/
                         var structs = from uo in entity.T_DIM_USER_STRUCTURE
                                       where uo.USER_NO == userId
                                       select uo.STRUCTURE_ID;//查找用户关注结构物
                         /****end*用户关注结构物*****/

                         var query = from sts in structs
                                     from o in entity.T_DIM_ORG_STUCTURE
                                     from s in entity.T_DIM_STRUCTURE
                                     from st in entity.T_DIM_STRUCTURE_TYPE
                                     where o.ORGANIZATION_ID == orgId && o.STRUCTURE_ID == s.ID
                                     && s.IsDelete != 1 && s.STRUCTURE_TYPE_ID == st.ID && o.STRUCTURE_ID == sts
                                     select new OrgStruct
                                         {
                                             structId = s.ID,
                                             structName = s.STRUCTURE_NAME_CN,
                                             structType = st.NAME_STRUCTURE_TYPE_CN,
                                             region = s.RegionPath,
                                             street = s.STRUCTURE_DETAIL_ADDRESS,
                                             longitude = s.STRUCTURE_LONGITUDE,
                                             latitude = s.STRUCTURE_LATITUDE,
                                             projectStatus = s.ProjectStatus,
                                             consCompany = s.CONSTRUCTION_COMPANY_NAME,
                                             description = s.DESCRIPTION,
                                             imageName = s.Imagename
                                         };
                         list = query.ToList();
                     }
                     var addressCode = from l in list
                                       select
                                           new
                                           {
                                               l.structId,
                                               l.region,
                                               provinceCode =
                                       l.region == null ? -1 : Convert.ToInt32(l.region.Substring(0, l.region.IndexOf(','))),
                                               cityCode =
                                       l.region == null
                                           ? -1
                                           : Convert.ToInt32(
                                               l.region.Substring(
                                                   l.region.IndexOf(',') + 1,
                                                   l.region.LastIndexOf(',') - l.region.IndexOf(',') - 1)),
                                               countryCode =
                                       l.region == null
                                           ? -1
                                           : Convert.ToInt32(l.region.Substring(l.region.LastIndexOf(',') + 1))
                                           };

                     var address = from a in addressCode
                                   join r1 in entity.T_DIM_REGION on a.provinceCode equals r1.REGION_ID into rp
                                   from pr in rp.DefaultIfEmpty()
                                   join r2 in entity.T_DIM_REGION on a.cityCode equals r2.REGION_ID into rc
                                   from cr in rc.DefaultIfEmpty()
                                   join r3 in entity.T_DIM_REGION on a.countryCode equals r3.REGION_ID into ro
                                   from or in ro.DefaultIfEmpty()
                                   where a.region != null
                                   select
                                       new
                                       {
                                           a.structId,
                                           province = pr.REGION_NAME_CN ?? string.Empty,
                                           city = cr.REGION_NAME_CN ?? string.Empty,
                                           country = or.REGION_NAME_CN ?? string.Empty
                                       };

                     var rslt = from l in list
                                join a in address on l.structId equals a.structId into a1
                                from ad in a1.DefaultIfEmpty()
                                select
                                    new
                                    {
                                        l.structId,
                                        l.structName,
                                        l.structType,
                                        province = l.region != null ? ad.province : string.Empty,
                                        city = l.region != null ? ad.city : string.Empty,
                                        country = l.region != null ? ad.country : string.Empty,
                                        l.street,
                                        l.longitude,
                                        l.latitude,
                                        projectStatus = l.projectStatus == null ? string.Empty : ((ProjectStatus)l.projectStatus).ToString(),
                                        l.consCompany,
                                        l.description,
                                        l.imageName
                                    };

                     return rslt.ToList();
                 }
            }
        }

        /// <summary>
        /// Get user/{userId}/org/{orgs}/struct-list
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="orgs">组织编号</param>
        /// <returns>结构物列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取组织结构物", false)]
        public object GetStructListByOrgs(int userId, string orgs)
        {
            var orgIds = orgs.Split(',').Select(s => Convert.ToInt32(s));

            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                 var ur = (from r in entity.T_DIM_ROLE
                          from u in entity.T_DIM_USER
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
                     if (ur.FirstOrDefault() == 1)
                     {
                         var query = from o in entity.T_DIM_ORGANIZATION
                                     from os in entity.T_DIM_ORG_STUCTURE
                                     from s in entity.T_DIM_STRUCTURE
                                     from st in entity.T_DIM_STRUCTURE_TYPE
                                     where
                                         orgIds.Contains(os.ORGANIZATION_ID) && os.STRUCTURE_ID == s.ID && s.IsDelete != 1
                                         && s.STRUCTURE_TYPE_ID == st.ID && o.ID==os.ORGANIZATION_ID && o.IsDeleted==false
                                     select new { structId = s.ID, structName = s.STRUCTURE_NAME_CN };
                         var list = query.ToList();

                         return list;
                     }
                     else {
                         /****start*用户关注结构物*****/
                         var structs = from uo in entity.T_DIM_USER_STRUCTURE
                                       where uo.USER_NO == userId
                                       select uo.STRUCTURE_ID;//查找用户关注结构物
                         /****end*用户关注结构物*****/

                         var query = from o in entity.T_DIM_ORGANIZATION
                                     from sts in structs
                                     from os in entity.T_DIM_ORG_STUCTURE
                                     from s in entity.T_DIM_STRUCTURE
                                     from st in entity.T_DIM_STRUCTURE_TYPE
                                     where
                                         orgIds.Contains(os.ORGANIZATION_ID) && os.STRUCTURE_ID == s.ID && s.IsDelete != 1
                                         && s.STRUCTURE_TYPE_ID == st.ID && o.ID == os.ORGANIZATION_ID && o.IsDeleted == false
                                     && s.ID == sts
                                     select new { structId = s.ID, structName = s.STRUCTURE_NAME_CN };
                         var list = query.ToList();

                         return list;
                     }
                 }

               
            }
        }
        /// <summary>
        /// 获取用户结构物信息 
        /// </summary>
        public class UserStruct
        {
            public int? structId { get; set; }

            public string structName { get; set; }

            public string structType { get; set; }

            public string region { get; set; }

            public string street { get; set; }

            public decimal? longitude { get; set; }

            public decimal? latitude { get; set; }

            public int? projectStatus { get; set; }

            public string consCompany { get; set; }

            public string description { get; set; }

            public string imageName { get; set; }
        }
        /// <summary>
        /// 获取所有结构物列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns> 结构物列表 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取所有结构物列表", false)]
        public object GetStructs(int userId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                 var ur = (from r in entity.T_DIM_ROLE
                          from u in entity.T_DIM_USER
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
                     List<UserStruct> list = new List<UserStruct>();
                     if (ur.FirstOrDefault() == 1)
                     {
                         var query = from oo in entity.T_DIM_ORGANIZATION
                                     from os in entity.T_DIM_ORG_STUCTURE
                                     from s in entity.T_DIM_STRUCTURE
                                     from st in entity.T_DIM_STRUCTURE_TYPE
                                     where s.IsDelete != 1 && s.STRUCTURE_TYPE_ID == st.ID && os.STRUCTURE_ID == s.ID && oo.ID == os.ORGANIZATION_ID && oo.IsDeleted == false
                                     select new UserStruct
                                     {
                                         structId = s.ID,
                                         structName = s.STRUCTURE_NAME_CN,
                                         structType = st.NAME_STRUCTURE_TYPE_CN,
                                         region = s.RegionPath,
                                         street = s.STRUCTURE_DETAIL_ADDRESS,
                                         longitude = s.STRUCTURE_LONGITUDE,
                                         latitude = s.STRUCTURE_LATITUDE,
                                         projectStatus = s.ProjectStatus,
                                         consCompany = s.CONSTRUCTION_COMPANY_NAME,
                                         description = s.DESCRIPTION,
                                         imageName = s.Imagename
                                     };
                         list = query.ToList();
                     }
                     else {
                         /****start*用户关注结构物*****/
                         var structs = from uo in entity.T_DIM_USER_STRUCTURE
                                       where uo.USER_NO == userId
                                       select uo.STRUCTURE_ID;//查找用户关注结构物
                         /****end*用户关注结构物*****/

                         var query = from oo in entity.T_DIM_ORGANIZATION
                                     from os in entity.T_DIM_ORG_STUCTURE
                                     from sts in structs
                                     from s in entity.T_DIM_STRUCTURE
                                     from st in entity.T_DIM_STRUCTURE_TYPE
                                     where s.IsDelete != 1 && s.STRUCTURE_TYPE_ID == st.ID && s.ID == sts && os.STRUCTURE_ID == s.ID && oo.ID == os.ORGANIZATION_ID && oo.IsDeleted == false
                                     select new UserStruct
                                     {
                                         structId = s.ID,
                                         structName = s.STRUCTURE_NAME_CN,
                                         structType = st.NAME_STRUCTURE_TYPE_CN,
                                         region = s.RegionPath,
                                         street = s.STRUCTURE_DETAIL_ADDRESS,
                                         longitude = s.STRUCTURE_LONGITUDE,
                                         latitude = s.STRUCTURE_LATITUDE,
                                         projectStatus = s.ProjectStatus,
                                         consCompany = s.CONSTRUCTION_COMPANY_NAME,
                                         description = s.DESCRIPTION,
                                         imageName = s.Imagename
                                     };
                         list = query.ToList();
                     }
                     var addressCode = from l in list
                                       select
                                           new
                                           {
                                               l.structId,
                                               l.region,
                                               provinceCode =
                                       l.region == null ? -1 : Convert.ToInt32(l.region.Substring(0, l.region.IndexOf(','))),
                                               cityCode =
                                       l.region == null
                                           ? -1
                                           : Convert.ToInt32(
                                               l.region.Substring(
                                                   l.region.IndexOf(',') + 1,
                                                   l.region.LastIndexOf(',') - l.region.IndexOf(',') - 1)),
                                               countryCode =
                                       l.region == null
                                           ? -1
                                           : Convert.ToInt32(l.region.Substring(l.region.LastIndexOf(',') + 1))
                                           };

                     var address = from a in addressCode
                                   join r1 in entity.T_DIM_REGION on a.provinceCode equals r1.REGION_ID into rp
                                   from pr in rp.DefaultIfEmpty()
                                   join r2 in entity.T_DIM_REGION on a.cityCode equals r2.REGION_ID into rc
                                   from cr in rc.DefaultIfEmpty()
                                   join r3 in entity.T_DIM_REGION on a.countryCode equals r3.REGION_ID into ro
                                   from or in ro.DefaultIfEmpty()
                                   where a.region != null
                                   select
                                       new
                                       {
                                           a.structId,
                                           province = pr.REGION_NAME_CN ?? string.Empty,
                                           city = cr.REGION_NAME_CN ?? string.Empty,
                                           country = or.REGION_NAME_CN ?? string.Empty
                                       };

                     var rslt = from l in list
                                join a in address on l.structId equals a.structId into a1
                                from ad in a1.DefaultIfEmpty()
                                select
                                    new
                                    {
                                        l.structId,
                                        l.structName,
                                        l.structType,
                                        province = l.region != null ? ad.province : string.Empty,
                                        city = l.region != null ? ad.city : string.Empty,
                                        country = l.region != null ? ad.country : string.Empty,
                                        l.street,
                                        l.longitude,
                                        l.latitude,
                                        projectStatus = l.projectStatus == null ? string.Empty : ((ProjectStatus)l.projectStatus).ToString(),
                                        l.consCompany,
                                        l.description,
                                        l.imageName
                                    };

                     return rslt.ToList();
                 }
            }
        }

        [AcceptVerbs("Get")]
        [LogInfo("获取所有结构物介绍", false)]
        [NonAuthorization]
        public object GetStructsIntro()
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_STRUCTURE
                            from st in entity.T_DIM_STRUCTURE_TYPE
                            where s.IsDelete != 1 && s.STRUCTURE_TYPE_ID == st.ID
                            select
                                new
                                    {                                        
                                        structName = s.STRUCTURE_NAME_CN,
                                        structType = st.NAME_STRUCTURE_TYPE_CN,                                        
                                        longitude = s.STRUCTURE_LONGITUDE,
                                        latitude = s.STRUCTURE_LATITUDE,
                                        projectStatus = s.ProjectStatus == 0 ? "施工期" : "运营期",
                                        consCompany = s.CONSTRUCTION_COMPANY_NAME,
                                        description = s.DESCRIPTION                                        
                                    };
                var list = query.ToList();

                return list;
            }
        }

        /// <summary>
        /// 获取单个结构物详细信息
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 结构物对象 </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物信息", false)]
        [Authorization(AuthorizationCode.S_Org_Logo_Upload)]
        [Authorization(AuthorizationCode.U_Common)]
        [Authorization(AuthorizationCode.S_Structure_Scheme)]
        public object GetStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_STRUCTURE
                            from st in entity.T_DIM_STRUCTURE_TYPE
                            where s.IsDelete != 1 && s.STRUCTURE_TYPE_ID == st.ID
                            && s.ID == structId
                            select
                                new
                                    {
                                        structName = s.STRUCTURE_NAME_CN,
                                        structType = st.NAME_STRUCTURE_TYPE_CN,
                                        region = s.RegionPath,
                                        street = s.STRUCTURE_DETAIL_ADDRESS,
                                        longitude = s.STRUCTURE_LONGITUDE,
                                        latitude = s.STRUCTURE_LATITUDE,
                                        consCompany = s.CONSTRUCTION_COMPANY_NAME,
                                        description = s.DESCRIPTION,
                                        imageName = s.Imagename
                                    };
                var stc = query.FirstOrDefault();
                if (stc == null)
                {
                    return null;
                }

                var rslt = new 
                        {
                            structName = stc.structName,
                            structType = stc.structType,
                            province = string.Empty,
                            city = string.Empty,
                            country = string.Empty,
                            street = stc.street,
                            longitude = stc.longitude,
                            latitude = stc.latitude,
                            consCompany = stc.consCompany,
                            description = stc.description,
                            imageName = stc.imageName
                        };                
                if (stc.region != null)
                {
                    var provinceCode = Convert.ToInt32(stc.region.Substring(0, stc.region.IndexOf(',')));
                    var cityCode =
                        Convert.ToInt32(stc.region.Substring(stc.region.IndexOf(',') + 1, stc.region.LastIndexOf(',') - stc.region.IndexOf(',') - 1));
                    var countryCode = Convert.ToInt32(stc.region.Substring(stc.region.LastIndexOf(',') + 1));

                    var address = (from r in entity.T_DIM_REGION
                                   select
                                       new
                                           {
                                               province =
                                       (from r1 in entity.T_DIM_REGION
                                        where r1.REGION_ID == provinceCode
                                        select r1.REGION_NAME_CN).FirstOrDefault(),
                                               city =
                                       (from r2 in entity.T_DIM_REGION
                                        where r2.REGION_ID == cityCode
                                        select r2.REGION_NAME_CN).FirstOrDefault(),
                                               country =
                                       (from r3 in entity.T_DIM_REGION
                                        where r3.REGION_ID == countryCode
                                        select r3.REGION_NAME_CN).FirstOrDefault()
                                           }).FirstOrDefault();

                    if (address != null)
                    {
                        return
                            new
                                {
                                    structName = stc.structName,
                                    structType = stc.structType,
                                    province = address.province,
                                    city = address.city,
                                    country = address.country,
                                    street = stc.street,
                                    longitude = stc.longitude,
                                    latitude = stc.latitude,
                                    consCompany = stc.consCompany,
                                    description = stc.description,
                                    imageName = stc.imageName
                                };
                    }
                }

                return rslt;
            }
        }

        /// <summary>
        /// 为组织添加结构物
        /// </summary>
        /// <param name="userId"> 用户编号 </param>
        /// <param name="orgId"> 组织编号 </param>
        /// <param name="model"> 结构物模型 </param>
        /// <returns> 添加结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("添加组织下的结构物", true)]
        [Authorization(AuthorizationCode.S_Structure_Add)]
        public HttpResponseMessage AddOrgStruct(int userId,[FromUri]int orgId, [FromBody]StructConfigModel model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                // 新增结构物
                var stc = new T_DIM_STRUCTURE();
                stc.STRUCTURE_NAME_CN = model.StructName;
                stc.STRUCTURE_TYPE_ID = model.StructTypeId;
                stc.RegionPath = string.Format("{0},{1},{2}", model.ProvinceCode, model.CityCode, model.CountryCode);
                stc.STRUCTURE_DETAIL_ADDRESS = model.Street;
                stc.STRUCTURE_LONGITUDE = model.Longitude;
                stc.STRUCTURE_LATITUDE = model.Latitude;
                stc.ProjectStatus = model.ProjectStatus;
                stc.CONSTRUCTION_COMPANY_NAME = model.ConsCompany;
                stc.DESCRIPTION = model.Description;
                stc.Imagename = model.ImageName;

                var entry1 = entity.Entry(stc);
                entry1.State = EntityState.Added;

                // 分配给组织
                var orgStc = new T_DIM_ORG_STUCTURE();
                orgStc.ORGANIZATION_ID = orgId;
                orgStc.STRUCTURE_ID = stc.ID;

                var entry2 = entity.Entry(orgStc);
                entry2.State = EntityState.Added;

                #region 日志信息

                var org =
                    entity.T_DIM_ORGANIZATION.Where(o => o.ID == orgId).Select(o => o.ABB_NAME_CN).FirstOrDefault();

                var stcType =
                    entity.T_DIM_STRUCTURE_TYPE.Where(st => st.ID == model.StructTypeId)
                        .Select(st => st.NAME_STRUCTURE_TYPE_CN).FirstOrDefault();

                var province =
                    entity.T_DIM_REGION.Where(r => r.REGION_ID == model.ProvinceCode)
                        .Select(r => r.REGION_NAME_CN)
                        .FirstOrDefault();

                var city =
                    entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CityCode)
                        .Select(r => r.REGION_NAME_CN)
                        .FirstOrDefault();

                var country =
                    entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CountryCode)
                        .Select(r => r.REGION_NAME_CN)
                        .FirstOrDefault();

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                this.Request.Properties["ActionParameterShow"] =
                    string.Format(
                        "在组织：{0} 下添加结构物：{1}，信息如下：结构物类型：{2},地址：{3}-{4}-{5},{6},经度{7},纬度：{8},项目状态:{9},施工单位：{10},描述信息:{11},热点图：{12}",
                        org ?? string.Empty,
                        string.IsNullOrEmpty(model.StructName) ? string.Empty : model.StructName,
                        stcType ?? string.Empty,
                        province,
                        city,
                        country,
                        string.IsNullOrEmpty(model.Street) ? string.Empty : model.Street,
                        model.Longitude ?? 0,
                        model.Latitude ?? 0,
                        model.ProjectStatus == null ? string.Empty : ((ProjectStatus)model.ProjectStatus).ToString(),
                        string.IsNullOrEmpty(model.ConsCompany) ? string.Empty : model.ConsCompany,
                        string.IsNullOrEmpty(model.Description) ? string.Empty : model.Description,
                        string.IsNullOrEmpty(model.ImageName) ? string.Empty : model.ImageName);
                #endregion

                try
                {
                    entity.SaveChanges();

                    /****start*add 用户自动关注结构物*****/
                    int pk = stc.ID;
                    var us = new T_DIM_USER_STRUCTURE();
                    us.STRUCTURE_ID = pk;
                    us.USER_NO = userId;
                    var entryUserStruct = entity.Entry(us);
                    entryUserStruct.State = System.Data.EntityState.Added;
                    entity.SaveChanges();
                    /****end******/

                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("新增成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("新增失败"));
                }
            }
        }

        /// <summary>
        /// 修改结构物信息
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="model"> 结构物模型 </param>
        /// <returns> 修改结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改结构物信息", true)]
        [Authorization(AuthorizationCode.S_Structure_Modify)]
        public HttpResponseMessage ModifyStruct([FromUri] int structId, [FromBody] StructConfigModel model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var stc = entity.T_DIM_STRUCTURE.FirstOrDefault(s => s.ID == structId);
                if (stc == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改失败,结构物不存在"));
                }

                var sb = new StringBuilder(50);

                sb.AppendFormat("结构物：{0}:", stc.STRUCTURE_NAME_CN);

                if (model.StructName != default(string) && model.StructName != stc.STRUCTURE_NAME_CN)
                {
                    stc.STRUCTURE_NAME_CN = model.StructName;
                    sb.AppendFormat("名称改为：{0},", model.StructName);
                }
                if (model.StructTypeId != default(string) && stc.STRUCTURE_TYPE_ID != model.StructTypeId)
                {
                    stc.STRUCTURE_TYPE_ID = model.StructTypeId;
                    var stcType =
                        entity.T_DIM_STRUCTURE_TYPE.Where(st => st.ID == model.StructTypeId)
                            .Select(st => st.NAME_STRUCTURE_TYPE_CN)
                            .FirstOrDefault();
                    sb.AppendFormat("类型改为：{0},", stcType ?? string.Empty);
                }
                if (model.ProvinceCode != default(int) && model.CityCode != default(int)
                    && model.CountryCode != default(int))
                {
                    var value = string.Format("{0},{1},{2}", model.ProvinceCode, model.CityCode, model.CountryCode);
                    if (value != stc.RegionPath)
                    {
                        stc.RegionPath = value;
                        var province =
                            entity.T_DIM_REGION.Where(r => r.REGION_ID == model.ProvinceCode)
                                .Select(r => r.REGION_NAME_CN)
                                .FirstOrDefault();

                        var city =
                            entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CityCode)
                                .Select(r => r.REGION_NAME_CN)
                                .FirstOrDefault();

                        var country =
                            entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CountryCode)
                                .Select(r => r.REGION_NAME_CN)
                                .FirstOrDefault();
                        sb.AppendFormat("省市县改为:{0}-{1}-{2},", province, city, country);
                    }
                }
                if (model.Street != default(string) && stc.STRUCTURE_DETAIL_ADDRESS != model.Street)
                {
                    stc.STRUCTURE_DETAIL_ADDRESS = model.Street;
                    sb.AppendFormat("地址改为：{0},", model.Street);
                }
                if (model.Longitude != default(decimal?) && stc.STRUCTURE_LONGITUDE != model.Longitude)
                {
                    stc.STRUCTURE_LONGITUDE = model.Longitude;
                    sb.AppendFormat("经度改为:{0},", model.Longitude);
                }
                if (model.Latitude != default(decimal?) && stc.STRUCTURE_LATITUDE != model.Latitude)
                {
                    stc.STRUCTURE_LATITUDE = model.Latitude;
                    sb.AppendFormat("纬度改为:{0},", model.Latitude);
                }
                if (model.ProjectStatus != default(int?) && stc.ProjectStatus != model.ProjectStatus)
                {
                    if (model.ProjectStatus == -1)
                    {
                        stc.ProjectStatus = null;
                        sb.Append("项目状态改为NULL");
                    }
                    else
                    {
                        stc.ProjectStatus = model.ProjectStatus;
                        sb.AppendFormat("项目状态改为:{0}", (ProjectStatus)model.ProjectStatus);
                    }
                }
                if (model.ConsCompany != default(string) && stc.CONSTRUCTION_COMPANY_NAME != model.ConsCompany)
                {
                    stc.CONSTRUCTION_COMPANY_NAME = model.ConsCompany;
                    sb.AppendFormat("施工单位改为:{0},", model.ConsCompany);
                }
                if (model.Description != default(string) && stc.DESCRIPTION != model.Description)
                {
                    stc.DESCRIPTION = model.Description;
                    sb.AppendFormat("描述改为:{0}", model.Description);
                }
                if (model.ImageName != default(string) && stc.Imagename != model.ImageName)
                {
                    if (stc.Imagename == null)
                    {
                        sb.AppendFormat("上传热点图:{0},", model.ImageName);
                    }
                    else
                    {
                        sb.AppendFormat("热点图改为:{0},", model.ImageName);
                    }
                    stc.Imagename = model.ImageName;                    
                }

                #region 日志信息
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                var entry1 = entity.Entry(stc);
                entry1.State = EntityState.Modified;

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("修改成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改失败"));
                }
            }
        }

        /// <summary>
        /// 修改结构物信息热点图上传
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="model"> 结构物模型 </param>
        /// <returns> 修改结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改结构物信息热点图上传", true)]
        [Authorization(AuthorizationCode.S_Org_Logo_Upload)]
        public HttpResponseMessage ModifyStructHotspot([FromUri] int structId, [FromBody] StructConfigModel model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var stc = entity.T_DIM_STRUCTURE.FirstOrDefault(s => s.ID == structId);
                if (stc == null)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改失败,结构物不存在"));
                }

                var sb = new StringBuilder(50);

                sb.AppendFormat("结构物：{0}:", stc.STRUCTURE_NAME_CN);

                if (model.StructName != default(string) && model.StructName != stc.STRUCTURE_NAME_CN)
                {
                    stc.STRUCTURE_NAME_CN = model.StructName;
                    sb.AppendFormat("名称改为：{0},", model.StructName);
                }
                if (model.StructTypeId != default(string) && stc.STRUCTURE_TYPE_ID != model.StructTypeId)
                {
                    stc.STRUCTURE_TYPE_ID = model.StructTypeId;
                    var stcType =
                        entity.T_DIM_STRUCTURE_TYPE.Where(st => st.ID == model.StructTypeId)
                            .Select(st => st.NAME_STRUCTURE_TYPE_CN)
                            .FirstOrDefault();
                    sb.AppendFormat("类型改为：{0},", stcType ?? string.Empty);
                }
                if (model.ProvinceCode != default(int) && model.CityCode != default(int)
                    && model.CountryCode != default(int))
                {
                    var value = string.Format("{0},{1},{2}", model.ProvinceCode, model.CityCode, model.CountryCode);
                    if (value != stc.RegionPath)
                    {
                        stc.RegionPath = value;
                        var province =
                            entity.T_DIM_REGION.Where(r => r.REGION_ID == model.ProvinceCode)
                                .Select(r => r.REGION_NAME_CN)
                                .FirstOrDefault();

                        var city =
                            entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CityCode)
                                .Select(r => r.REGION_NAME_CN)
                                .FirstOrDefault();

                        var country =
                            entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CountryCode)
                                .Select(r => r.REGION_NAME_CN)
                                .FirstOrDefault();
                        sb.AppendFormat("省市县改为:{0}-{1}-{2},", province, city, country);
                    }
                }
                if (model.Street != default(string) && stc.STRUCTURE_DETAIL_ADDRESS != model.Street)
                {
                    stc.STRUCTURE_DETAIL_ADDRESS = model.Street;
                    sb.AppendFormat("地址改为：{0},", model.Street);
                }
                if (model.Longitude != default(decimal?) && stc.STRUCTURE_LONGITUDE != model.Longitude)
                {
                    stc.STRUCTURE_LONGITUDE = model.Longitude;
                    sb.AppendFormat("经度改为:{0},", model.Longitude);
                }
                if (model.Latitude != default(decimal?) && stc.STRUCTURE_LATITUDE != model.Latitude)
                {
                    stc.STRUCTURE_LATITUDE = model.Latitude;
                    sb.AppendFormat("纬度改为:{0},", model.Latitude);
                }
                if (model.ProjectStatus != default(int?) && stc.ProjectStatus != model.ProjectStatus)
                {
                    if (model.ProjectStatus == -1)
                    {
                        stc.ProjectStatus = null;
                        sb.Append("项目状态改为NULL");
                    }
                    else
                    {
                        stc.ProjectStatus = model.ProjectStatus;
                        sb.AppendFormat("项目状态改为:{0}", (ProjectStatus)model.ProjectStatus);
                    }
                }
                if (model.ConsCompany != default(string) && stc.CONSTRUCTION_COMPANY_NAME != model.ConsCompany)
                {
                    stc.CONSTRUCTION_COMPANY_NAME = model.ConsCompany;
                    sb.AppendFormat("施工单位改为:{0},", model.ConsCompany);
                }
                if (model.Description != default(string) && stc.DESCRIPTION != model.Description)
                {
                    stc.DESCRIPTION = model.Description;
                    sb.AppendFormat("描述改为:{0}", model.Description);
                }
                if (model.ImageName != default(string) && stc.Imagename != model.ImageName)
                {
                    if (stc.Imagename == null)
                    {
                        sb.AppendFormat("上传热点图:{0},", model.ImageName);
                    }
                    else
                    {
                        sb.AppendFormat("热点图改为:{0},", model.ImageName);
                    }
                    stc.Imagename = model.ImageName;
                }

                #region 日志信息
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                var entry1 = entity.Entry(stc);
                entry1.State = EntityState.Modified;

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("修改成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改失败"));
                }
            }
        }

        /// <summary>
        /// 删除组织下的结构物
        /// </summary>
        /// <param name="orgId"> 组织编号 </param>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 删除结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除组织下的结构物", true)]
        [Authorization(AuthorizationCode.S_Structure_Modify)]
        public HttpResponseMessage RemoveOrgStruct(int orgId, int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var stc =
                    entity.T_DIM_ORG_STUCTURE.Where(
                        s => s.ORGANIZATION_ID == orgId && s.STRUCTURE_ID == structId);
                if (!stc.Any())
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除失败,组织下没有该结构物"));
                }

                foreach (var orgStucture in stc)
                {                    
                    var entry1 = entity.Entry(orgStucture);
                    entry1.State = EntityState.Deleted;    
                }

                #region 日志信息   

                var org =
                    entity.T_DIM_ORGANIZATION.Where(o => o.ID == orgId).Select(o => o.ABB_NAME_CN).FirstOrDefault();

                var strc =
                    entity.T_DIM_STRUCTURE.Where(st => st.ID == structId)
                        .Select(st => st.STRUCTURE_NAME_CN)
                        .FirstOrDefault();

                this.Request.Properties["ActionParameterShow"] = string.Format(
                    "组织：{0},解绑的结构物：{1}",
                    org ?? string.Empty,
                    strc ?? string.Empty);
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("删除成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除失败"));
                }
            }
        }

        /// <summary>
        /// 删除结构物
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 删除结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除结构物", true)]
        [Authorization(AuthorizationCode.S_Structure_Modify)]
        public HttpResponseMessage RemoveStruct(int structId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                // 删除结构物                
                var structure = entity.T_DIM_STRUCTURE.FirstOrDefault(s => s.ID == structId && s.IsDelete != 1);
                if (structure == null)
                {
                    return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("删除失败,没有该结构物"));
                }

                structure.IsDelete = 1;

                // 删除用户结构物表中对应的结构物
                var userStruct = entity.T_DIM_USER_STRUCTURE.Where(w => w.STRUCTURE_ID == structId);
                if(userStruct.Any())
                {
                    foreach(var us in userStruct)
                    {
                        var entry = entity.Entry(us);
                        entry.State = EntityState.Deleted;
                    }
                }

                #region 日志信息                
                this.Request.Properties["ActionParameterShow"] = "结构物：" + structure.STRUCTURE_NAME_CN;
                #endregion

                try
                {
                    int i = entity.SaveChanges();
                    if (i == 0)
                    {
                        return Request.CreateResponse(
                            System.Net.HttpStatusCode.BadRequest,
                            StringHelper.GetMessageString("删除失败,没有该结构物"));
                    }

                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("删除成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除失败"));
                }
            }
        }

        /// <summary>
        /// 设置关注结构物：user/{userId}/struct/{structId}/focused
        /// </summary>
        /// <param name="userId"> 用户编号 </param>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 修改结果 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("设置关注结构物", true)]
        public HttpResponseMessage ModifyStructFocused([FromUri]int userId, [FromUri]int structId, [FromBody]StructFocused sf)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var structEntity = entity.T_DIM_USER_STRUCTURE.FirstOrDefault(s => s.USER_NO == userId && s.STRUCTURE_ID == structId);
                if (structEntity == null)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("不存在此用户结构物!"));
                }

                structEntity.StructConcerned = sf.StructFocusedStatus;

                #region 日志信息

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(sf);

                var structName = entity.T_DIM_STRUCTURE.Where(w => w.ID == structId).Select(s => s.STRUCTURE_NAME_CN).FirstOrDefault();
                var result = string.Empty;
                if (sf.StructFocusedStatus == false)
                {
                    result = "取消关注";
                }
                else
                {
                    result = "设为关注";
                }
                this.Request.Properties["ActionParameterShow"] = string.Format("结构物：{0}，{1}", structName, result);

                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("关注结构物设置成功"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("关注结构物设置失败"));
                }
            }
        }

    }

    public class StructFocused
    {
        public bool StructFocusedStatus { get; set; }
    }

    public class ProjectModel
    {
        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("structStatus")]
        public List<StructureStatus> StructStatus { get; set; }
    }

    public class StructureStatus
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("structName")]
        public string StructName { get; set; }
        // 工程状态
        [JsonProperty("projectStatus")]
        public string ProjectStatus { get; set; }
        // 是否为关注结构物
        [JsonProperty("structFocused")]
        public bool? StructFocused { get; set; }

        [JsonProperty("dtuStatus")]
        public string DtuStatus { get; set; }

        [JsonProperty("dtuStatusInfo")]
        public List<DtuStatusInfo> DtuStatusInfo { get; set; }
        // 结构物最高告警等级
        [JsonProperty("warningStatus")]
        public int WarningStatus { get; set; }

        [JsonProperty("warningStatusInfo")]
        public List<WarningStatusInfo> WarningStatusInfo { get; set; }

        [JsonProperty("dataStatus")]
        public string DataStatus { get; set; }

        [JsonProperty("dataStatusInfo")]
        public List<DataStatusInfo> DataStatusInfo { get; set; }
    }

    public class DtuStatusInfo
    {
        [JsonProperty("dtuId")]
        public int? DtuId { get; set; }

        [JsonProperty("dtuNo")]
        public string DtuNo { get; set; }

        [JsonProperty("dtuStatus")]
        public string DtuStatus { get; set; }
        // DTU本次上线时间
        [JsonProperty("lastOnlineTime")]
        public DateTime? LastOnlineTime { get; set; }
        // DTU本次下线时间
        [JsonProperty("currentOfflineTime")]
        public DateTime? CurrentOfflineTime { get; set; }
    }

    public class WarningStatusInfo
    {
        [JsonProperty("warningSource")]
        public string WarningSource { get; set; }

        [JsonProperty("warningLevel")]
        public int WarningLevel { get; set; }

        [JsonProperty("warningContent")]
        public string WarningContent { get; set; }

        [JsonProperty("warningTime")]
        public DateTime? WarningTime { get; set; }
    }

    public class DataStatusInfo
    {
        [JsonProperty("sensorId")]
        public int? SensorId { get; set; }
        [JsonProperty("sensorLocation")]
        public string SensorLocation { get; set; }
        [JsonProperty("dataStatus")]
        public string DataStatus { get; set; }
        // 上次数据采集时间
        [JsonProperty("lastAcquisitionTime")]
        public DateTime? LastAcquisitionTime { get; set; }
    }

    public class StructModel
    {
        [JsonProperty("structId")]
        public int StructId { get; set; }

        [JsonProperty("structName")]
        public string StructName { get; set; }

        [JsonProperty("imageName")]
        public string ImageName { get; set; }

        [JsonProperty("longitude")]
        public decimal? Longitude { get; set; }

        [JsonProperty("latitude")]
        public decimal? Latitude { get; set; }

        [JsonProperty("projectStatus")]
        public string ProjectStatus { get; set; }

        [JsonProperty("worstWarning")]
        public Warning WorstWarning { get; set; }

        [JsonProperty("score")]
        public int? Score { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Warning
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }

    public class StructConfigModel
    {
        public string StructName { get; set; }

        public string StructTypeId { get; set; }

        public int ProvinceCode { get; set; }

        public int CityCode { get; set; }

        public int CountryCode { get; set; }

        public string Street { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public int? ProjectStatus { get; set; }

        public string ConsCompany { get; set; }

        public string Description { get; set; }

        public string ImageName { get; set; }
    }

    public enum ProjectStatus
    {        
        施工期 = 0,
        运营期 = 1
    }
}
