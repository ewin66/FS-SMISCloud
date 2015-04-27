namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Structure
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {            
            routes.MapHttpRoute(
                name: "FindStructsByUserName",
                routeTemplate: "user/{userId}/structs",
                defaults: new { controller = "Structure", action = "FindStructsByUserName" },
                constraints: new { userId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "FindStructsOfCurrentServiceUser",
                routeTemplate: "user/structs",
                defaults: new { controller = "Structure", action = "FindStructsOfCurrentServiceUser" });

            routes.MapHttpRoute(
                name: "FindStructsByOrg",
                routeTemplate: "user/{userId}/org/{orgId}/structs",
                defaults: new { controller = "Structure", action = "FindStructsByOrg" },
                constraints: new { orgId = @"^(\-?[1-9]+\d*)|0$" });

            routes.MapHttpRoute(
                name: "GetStructListByOrgs",
                routeTemplate: "user/{userId}/org/{orgs}/struct-list",
                defaults: new { controller = "Structure", action = "GetStructListByOrgs" });

            routes.MapHttpRoute(
                name: "GetStructs",
                routeTemplate: "user/{userId}/struct/list",
                defaults: new { controller = "Structure", action = "GetStructs" });

            routes.MapHttpRoute(
                name: "GetStruct",
                routeTemplate: "struct/{structId}/info",
                defaults: new { controller = "Structure", action = "GetStruct" },
                constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "AddOrgStruct",
                routeTemplate: "user/{userId}/org/{orgId}/struct/add",
                defaults: new { controller = "Structure", action = "AddOrgStruct" },
                constraints: new { orgId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "ModifyStruct",
                routeTemplate: "struct/modify/{structId}",
                defaults: new { controller = "Structure", action = "ModifyStruct" },
                constraints: new { structId = @"^\d+$" });
            //结构物信息热点图上传，修改结构物
            routes.MapHttpRoute(
               name: "ModifyStructHotspot",
               routeTemplate: "struct/hotspot/modify/{structId}",
               defaults: new { controller = "Structure", action = "ModifyStructHotspot" },
               constraints: new { structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "RemoveStruct",
                routeTemplate: "struct/remove/{structId}",
                defaults: new { controller = "Structure", action = "RemoveStruct" },
                constraints: new { structId = @"^\d+$" });

            
            routes.MapHttpRoute(
                name: "RemoveOrgStruct",
                routeTemplate: "org/{orgId}/struct/remove/{structId}",
                defaults: new { controller = "Structure", action = "RemoveOrgStruct" },
                constraints: new { orgId = @"^\d+$", structId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "GetStructType",
                routeTemplate: "struct/type/list",
                defaults: new { controller = "StructureType", action = "GetStructType" });

            // 设置关注结构物
            routes.MapHttpRoute(
                name: "ModifyStructFocused",
                routeTemplate: "user/{userId}/struct/{structId}/focused",
                defaults: new { controller = "Structure", action = "ModifyStructFocused" },
                constraints: new { userId = @"^\d+$", structId = @"^\d+$" });


            routes.MapHttpRoute(
                name: "GetStructsIntro",
                routeTemplate: "struct/intro",
                defaults: new { controller = "Structure", action = "GetStructsIntro" });
        }
    }
}