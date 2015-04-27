namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.User.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;

    public class OrganizationController : ApiController
    {
        /// <summary>
        /// 获取组织列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns>组织列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取组织列表", false)]
        public object GetOrgList(int userId)
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
                    if (ur.FirstOrDefault() == 1) {
                        var query = from o in entity.T_DIM_ORGANIZATION
                                    join r1 in entity.T_DIM_REGION on o.ProvinceCode equals r1.REGION_ID into o1
                                    from org1 in o1.DefaultIfEmpty()
                                    join r2 in entity.T_DIM_REGION on o.CityCode equals r2.REGION_ID into o2
                                    from org2 in o2.DefaultIfEmpty()
                                    join r3 in entity.T_DIM_REGION on o.CountryCode equals r3.REGION_ID into o3
                                    from org3 in o3.DefaultIfEmpty()
                                    where o.IsDeleted == null || o.IsDeleted == false 
                                    select
                                        new
                                        {
                                            orgId = o.ID,
                                            orgName = o.ABB_NAME_CN,
                                            provinceCode = o.ProvinceCode,
                                            province = org1.REGION_NAME_CN,
                                            cityCode = o.CityCode,
                                            city = org2.REGION_NAME_CN,
                                            countryCode = o.CountryCode,
                                            country = org3.REGION_NAME_CN,
                                            street = o.ADDRESS_STREET_CN,
                                            zipCode = o.ZIPCODE,
                                            phone = o.MOBILE_PHONE_NUMBER,
                                            fax = o.FAX_NUMBER,
                                            website = o.WEBSITE,
                                            systemName = o.SystemName,
                                            systemAbbreviation = o.SystemNameAbbreviation,
                                            logo = o.Logo
                                        };
                        return query.ToList();
                
                    } else {
                        /****start*用户关注组织*****/
                        var orgs = from uo in entity.T_DIM_USER_ORG
                                   where uo.USER_NO == userId
                                   select uo.ORGANIZATION_ID;//查找用户关注组织
                        /****end*用户关注组织*****/

                        var query = from org in orgs
                                    from o in entity.T_DIM_ORGANIZATION
                                    join r1 in entity.T_DIM_REGION on o.ProvinceCode equals r1.REGION_ID into o1
                                    from org1 in o1.DefaultIfEmpty()
                                    join r2 in entity.T_DIM_REGION on o.CityCode equals r2.REGION_ID into o2
                                    from org2 in o2.DefaultIfEmpty()
                                    join r3 in entity.T_DIM_REGION on o.CountryCode equals r3.REGION_ID into o3
                                    from org3 in o3.DefaultIfEmpty()
                                    where o.IsDeleted == null || o.IsDeleted == false && o.ID == org
                                    select
                                        new
                                        {
                                            orgId = o.ID,
                                            orgName = o.ABB_NAME_CN,
                                            provinceCode = o.ProvinceCode,
                                            province = org1.REGION_NAME_CN,
                                            cityCode = o.CityCode,
                                            city = org2.REGION_NAME_CN,
                                            countryCode = o.CountryCode,
                                            country = org3.REGION_NAME_CN,
                                            street = o.ADDRESS_STREET_CN,
                                            zipCode = o.ZIPCODE,
                                            phone = o.MOBILE_PHONE_NUMBER,
                                            fax = o.FAX_NUMBER,
                                            website = o.WEBSITE,
                                            systemName = o.SystemName,
                                            systemAbbreviation = o.SystemNameAbbreviation,
                                            logo = o.Logo
                                        };
                        return query.ToList();
                    }
                }
            }
        }

        /// <summary>
        /// 获取用户管理组织列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns>组织列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户管理组织列表", false)]
        public object GetUserManageOrgList(int userId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                /****start*用户关注组织*****/
                var orgs = from uo in entity.T_DIM_USER_ORG
                           where uo.USER_NO == userId
                           select uo.ORGANIZATION_ID;//查找用户关注组织
                /****end*用户关注组织*****/

                var query = 
                    //from org in orgs
                            from o in entity.T_DIM_ORGANIZATION
                            join r1 in entity.T_DIM_REGION on o.ProvinceCode equals r1.REGION_ID into o1
                            from org1 in o1.DefaultIfEmpty()
                            join r2 in entity.T_DIM_REGION on o.CityCode equals r2.REGION_ID into o2
                            from org2 in o2.DefaultIfEmpty()
                            join r3 in entity.T_DIM_REGION on o.CountryCode equals r3.REGION_ID into o3
                            from org3 in o3.DefaultIfEmpty()
                            where o.IsDeleted == null || o.IsDeleted == false 
                            //&& o.ID == org
                            select
                                new
                                {
                                    orgId = o.ID,
                                    orgName = o.ABB_NAME_CN,
                                    provinceCode = o.ProvinceCode,
                                    province = org1.REGION_NAME_CN,
                                    cityCode = o.CityCode,
                                    city = org2.REGION_NAME_CN,
                                    countryCode = o.CountryCode,
                                    country = org3.REGION_NAME_CN,
                                    street = o.ADDRESS_STREET_CN,
                                    zipCode = o.ZIPCODE,
                                    phone = o.MOBILE_PHONE_NUMBER,
                                    fax = o.FAX_NUMBER,
                                    website = o.WEBSITE,
                                    systemName = o.SystemName,
                                    systemAbbreviation = o.SystemNameAbbreviation,
                                    logo = o.Logo
                                };
                return query.ToList();
            }
        }

        /// <summary>
        /// 获取组织信息
        /// </summary>
        /// <param name="orgId">组织编号</param>
        /// <returns>组织信息</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取组织信息", false)]
        [Authorization(AuthorizationCode.S_Org_Logo)]
        [Authorization(AuthorizationCode.S_Org_Modify)]
        public object GetOrgInfo(int orgId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                var query = from o in entity.T_DIM_ORGANIZATION
                            join r1 in entity.T_DIM_REGION on o.ProvinceCode equals r1.REGION_ID into o1
                            from org1 in o1.DefaultIfEmpty()
                            join r2 in entity.T_DIM_REGION on o.CityCode equals r2.REGION_ID into o2
                            from org2 in o2.DefaultIfEmpty()
                            join r3 in entity.T_DIM_REGION on o.CountryCode equals r3.REGION_ID into o3
                            from org3 in o3.DefaultIfEmpty()
                            where o.IsDeleted == false
                                  && o.ID == orgId
                            select
                                new
                                {
                                    orgId = o.ID,
                                    orgName = o.ABB_NAME_CN,
                                    provinceCode = o.ProvinceCode,
                                    province = org1.REGION_NAME_CN,
                                    cityCode = o.CityCode,
                                    city = org2.REGION_NAME_CN,
                                    countryCode = o.CountryCode,
                                    country = org3.REGION_NAME_CN,
                                    street = o.ADDRESS_STREET_CN,
                                    zipCode = o.ZIPCODE,
                                    phone = o.MOBILE_PHONE_NUMBER,
                                    fax = o.FAX_NUMBER,
                                    website = o.WEBSITE,
                                    systemName = o.SystemName,
                                    logo = o.Logo
                                };
                return query.FirstOrDefault();
            }
        }

        /// <summary>
        /// 新增组织
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="userId">用户编号</param>
        /// <returns> 新增结果  </returns>
        [AcceptVerbs("Post")]
        [LogInfo("新增组织", true)]
        [Authorization(AuthorizationCode.S_Org_Add)]
        public HttpResponseMessage AddOrg(int userId,[FromBody]OrgModel model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var org = new T_DIM_ORGANIZATION();
                org.ABB_NAME_CN = model.OrgName;
                org.ProvinceCode = model.ProvinceCode;
                org.CityCode = model.CityCode;
                org.CountryCode = model.CountryCode;
                org.ADDRESS_STREET_CN = model.Street;
                org.ZIPCODE = model.ZipCode;
                org.MOBILE_PHONE_NUMBER = model.Phone;
                org.FAX_NUMBER = model.Fax;
                org.WEBSITE = model.Website;
                org.IsDeleted = false;
                org.SystemName = model.SystemName;
                org.SystemNameAbbreviation = model.SystemAbbreviation;
                org.Logo = model.Logo;
                
                var entry = entity.Entry(org);
                entry.State = System.Data.EntityState.Added;

                #region 日志信息

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
                        "组织名称：{0}，{1}-{2}-{3},{4},邮编:{5},联系电话:{6},传真：{7},网址:{8},系统名称:{9},组织logo:{10}",
                        model.OrgName ?? string.Empty,
                        province ?? string.Empty,
                        city ?? string.Empty,
                        country ?? string.Empty,
                        string.IsNullOrEmpty(model.Street) ? string.Empty : model.Street,
                        string.IsNullOrEmpty(model.ZipCode) ? string.Empty : model.ZipCode,
                        string.IsNullOrEmpty(model.Phone) ? string.Empty : model.Phone,
                        string.IsNullOrEmpty(model.Fax) ? string.Empty : model.Fax,
                        string.IsNullOrEmpty(model.Website) ? string.Empty : model.Website,
                        string.IsNullOrEmpty(model.SystemName) ? string.Empty : model.SystemName,
                        string.IsNullOrEmpty(model.Logo) ? string.Empty : model.Logo);

                #endregion

                try
                {
                    entity.SaveChanges();

                    /****start*add 用户自动关注组织*****/
                    int pk = org.ID;
                    var uo = new T_DIM_USER_ORG();
                    uo.ORGANIZATION_ID = pk;
                    uo.USER_NO = userId;
                    var entryUserOrg = entity.Entry(uo);
                    entryUserOrg.State = System.Data.EntityState.Added;
                    entity.SaveChanges();
                    /****end******/

                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("新增组织成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("新增组织失败,请检查组织名称是否重复"));
                }
            }
        }

        /// <summary>
        /// 修改组织信息
        /// </summary>
        /// <param name="orgId"> The org Id. </param>
        /// <param name="model"> The model. </param>
        /// <returns> 修改结果  </returns>
        [AcceptVerbs("Post")]
        [LogInfo("修改组织信息", true)]
        [Authorization(AuthorizationCode.S_Org_Logo)]
        [Authorization(AuthorizationCode.S_Org_Modify)]
        public HttpResponseMessage ModifyOrg([FromUri]int orgId, [FromBody]OrgModel model)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var sb = new StringBuilder(20);
                var org = entity.T_DIM_ORGANIZATION.FirstOrDefault(o => o.ID == orgId);
                if (org == null)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("组织不存在"));
                }

                sb.AppendFormat("组织名称：{0}:", org.ABB_NAME_CN);

                if (model.ProvinceCode != default(int) && org.ProvinceCode != model.ProvinceCode)
                {
                    org.ProvinceCode = model.ProvinceCode;
                    var province =
                    entity.T_DIM_REGION.Where(r => r.REGION_ID == model.ProvinceCode)
                        .Select(r => r.REGION_NAME_CN)
                        .FirstOrDefault();

                    sb.AppendFormat("省改为:{0},", province);
                }
                if (model.CityCode != default(int) && org.CityCode != model.CityCode)
                {
                    org.CityCode = model.CityCode;
                    var city =
                        entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CityCode)
                            .Select(r => r.REGION_NAME_CN)
                            .FirstOrDefault();

                    sb.AppendFormat("市改为：{0},", city);
                }
                if (model.CountryCode != default(int) && org.CountryCode != model.CountryCode)
                {
                    org.CountryCode = model.CountryCode;
                    var country =
                        entity.T_DIM_REGION.Where(r => r.REGION_ID == model.CountryCode)
                            .Select(r => r.REGION_NAME_CN)
                            .FirstOrDefault();

                    sb.AppendFormat("县区改为:{0},", country);
                }
                if (model.Street != default(string) && org.ADDRESS_STREET_CN != model.Street)
                {
                    org.ADDRESS_STREET_CN = model.Street;
                    sb.AppendFormat("街道地址改为：{0},", model.Street);
                }
                if (model.ZipCode != default(string) && org.ZIPCODE != model.ZipCode)
                {
                    org.ZIPCODE = model.ZipCode;
                    sb.AppendFormat("邮编改为:{0},", model.ZipCode);
                }
                if (model.Phone != default(string) && org.MOBILE_PHONE_NUMBER != model.Phone)
                {
                    org.MOBILE_PHONE_NUMBER = model.Phone;
                    sb.AppendFormat("联系电话改为:{0},", model.Phone);
                }
                if (model.Fax != default(string) && org.FAX_NUMBER != model.Fax)
                {
                    org.FAX_NUMBER = model.Fax;
                    sb.AppendFormat("传真改为：{0},", model.Fax);
                }
                if (model.Website != default(string) && org.WEBSITE != model.Website)
                {
                    org.WEBSITE = model.Website;
                    sb.AppendFormat("网址改为:{0},", model.Website);
                }
                if (model.SystemName != default(string) && org.SystemName != model.SystemName)
                {
                    org.SystemName = model.SystemName;
                    sb.AppendFormat("系统名称改为：{0},", model.SystemName);
                }
                if (model.SystemAbbreviation != default(string) && org.SystemNameAbbreviation != model.SystemAbbreviation)
                {
                    org.SystemNameAbbreviation = model.SystemAbbreviation;
                    sb.AppendFormat("系统简称改为：{0},", model.SystemAbbreviation);
                }
                if (model.Logo != default(string) && org.Logo != model.Logo)
                {
                    org.Logo = model.Logo;
                    sb.AppendFormat("组织Logo改为:{0}", model.Logo);
                }

                var entry = entity.Entry(org);
                entry.State = System.Data.EntityState.Modified;

                #region 日志信息
                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(model);
                this.Request.Properties["ActionParameterShow"] = sb.ToString();
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("修改组织信息成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("修改组织信息失败"));
                }
            }
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="orgId"> The org Id. </param>
        /// <returns> 删除结果  </returns>
        [AcceptVerbs("Post")]
        [LogInfo("删除组织", true)]
        [Authorization(AuthorizationCode.S_Org_Modify)]
        public HttpResponseMessage RemoveOrg(int orgId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var org = entity.T_DIM_ORGANIZATION.FirstOrDefault(o => o.ID == orgId && o.IsDeleted != true);
                if (org == null)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("组织不存在或已被删除"));
                }

                org.IsDeleted = true;

                var entry = entity.Entry(org);
                entry.State = System.Data.EntityState.Modified;

                /****start*remove 自动删除用户关注组织关系，与该组织相关绑定也一并 删除*****/
                var entryUserOrgs =from uo in entity.T_DIM_USER_ORG where uo.ORGANIZATION_ID == orgId select uo;
                foreach (var userOrg in entryUserOrgs)
                {
                    var entryUserOrg = entity.Entry(userOrg);
                    entryUserOrg.State = System.Data.EntityState.Deleted;
                }

                //用户与该组织下结构物的绑定，一并删除
                var userOrgStructs = from us in entity.T_DIM_USER_STRUCTURE
                                    from os in entity.T_DIM_ORG_STUCTURE
                                    where os.ORGANIZATION_ID == orgId && us.STRUCTURE_ID == os.STRUCTURE_ID
                                    select us;
                foreach (var userOrgstr in userOrgStructs)
                {
                    var entryUserOrgStr = entity.Entry(userOrgstr);
                    entryUserOrgStr.State = System.Data.EntityState.Deleted;
                }
                /****end******/

                #region 日志信息
              
                this.Request.Properties["ActionParameterShow"] = "组织名称：" + org.ABB_NAME_CN;
                #endregion

                try
                {
                    entity.SaveChanges();
                    return Request.CreateResponse(
                        HttpStatusCode.Accepted,
                        StringHelper.GetMessageString("删除组织成功"));
                }
                catch (Exception)
                {
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        StringHelper.GetMessageString("删除组织失败"));
                }
            }
        }
    }

    public class OrgModel
    {
        public string OrgName { get; set; }

        public int  ProvinceCode { get; set; }

        public int CityCode { get; set; }

        public int CountryCode { get; set; }

        public string Street { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Website { get; set; }

        public string SystemName { get; set; }

        public string SystemAbbreviation { get; set; }

        public string Logo { get; set; }
    }
}
