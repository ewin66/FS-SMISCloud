namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.User
{
    using System.Web.Http;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 用户名是否存在
            routes.MapHttpRoute(
                name: "CheckUserExists",
                routeTemplate: "user/check-exists/{username}",
                defaults: new { controller = "User", action = "CheckUserExists" });

            // 登录
            routes.MapHttpRoute(
                name: "LoginAndReturnInfo",
                routeTemplate: "user/login/{username}/{password}/info",
                defaults: new { controller = "User", action = "LoginAndReturnInfo" });

            routes.MapHttpRoute(
                name: "Login",
                routeTemplate: "user/login/{username}/{password}",
                defaults: new { controller = "User", action = "Login" });

            // 修改信息
            routes.MapHttpRoute(
                name: "ModifyInfo",
                routeTemplate: "user/modify-info",
                defaults: new { controller = "User", action = "ModifyInfo" });

            // 重置密码
            routes.MapHttpRoute(
                name: "ResetPwd",
                routeTemplate: "user/reset-password",
                defaults: new { controller = "User", action = "ResetPassword" });

            // 登出
            routes.MapHttpRoute(
                name: "Logout",
                routeTemplate: "user/logout/{token}",
                defaults: new { controller = "User", action = "Logout" });

            // 组织列表
            routes.MapHttpRoute(
                name: "GetOrgList",
                routeTemplate: "user/{userId}/org/list",
                defaults: new { controller = "Organization", action = "GetOrgList" });
                //constraints: new { userId = @"^\d+$" }); 

            routes.MapHttpRoute(
                name: "GetUserManageOrgList",
                routeTemplate: "userManage/{userId}/org/list",
                defaults: new { controller = "Organization", action = "GetUserManageOrgList" });

            routes.MapHttpRoute(
                name: "GetOrgInfo",
                routeTemplate: "org/{orgId}/info",
                defaults: new { controller = "Organization", action = "GetOrgInfo" },
                constraints: new { orgId = @"^\d+$" });            

            routes.MapHttpRoute(
                name: "AddOrg",
                routeTemplate: "user/{userId}/org/add",
                defaults: new { controller = "Organization", action = "AddOrg" });

            routes.MapHttpRoute(
                name: "ModifyOrg",
                routeTemplate: "org/modify/{orgId}",
                defaults: new { controller = "Organization", action = "ModifyOrg" },
                constraints: new { orgId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "RemoveOrg",
                routeTemplate: "org/remove/{orgId}",
                defaults: new { controller = "Organization", action = "RemoveOrg" },
                constraints: new { orgId = @"^\d+$" });
            
            // 用户列表
            routes.MapHttpRoute(
                name: "GetUsers",
                routeTemplate: "user/{userId}/list",
                defaults: new { controller = "User", action = "GetUsers" });

            routes.MapHttpRoute(
                name: "GetUserInfo",
                routeTemplate: "user/{userId}/info",
                defaults: new { controller = "User", action = "GetUserInfo" });

            routes.MapHttpRoute(
                name: "AddUser",
                routeTemplate: "user/add",
                defaults: new { controller = "User", action = "Add" });

            routes.MapHttpRoute(
                name: "ModifyUser",
                routeTemplate: "user/modify/{userId}",
                defaults: new { controller = "User", action = "Modify" },
                constraints: new { userId = @"^\d+$" });

            routes.MapHttpRoute(
                name: "RemoveUser",
                routeTemplate: "user/remove/{userId}",
                defaults: new { controller = "User", action = "Remove" },
                constraints: new { userId = @"^\d+$" });

            //角色列表
            routes.MapHttpRoute(
                name: "GetRoles",
                routeTemplate: "role/list",
                defaults: new { controller = "Role", action = "GetRoles" });

            // 数据服务用户
            routes.MapHttpRoute(
                name: "GetToken",
                routeTemplate: "token/apply",
                defaults: new { controller = "ServiceUser", action = "GetToken" });

            routes.MapHttpRoute(
                name: "UpdateToken",
                routeTemplate: "token/alive",
                defaults: new { controller = "ServiceUser", action = "UpdateToken" });

            routes.MapHttpRoute(
                name: "DropToken",
                routeTemplate: "token/drop",
                defaults: new { controller = "ServiceUser", action = "DropToken" });

            routes.MapHttpRoute(
                name: "CheckRoleResource",
                routeTemplate: "auth/{resId}",
                defaults: new { controller = "User", action = "CheckRoleResource" });

        }
    }
}