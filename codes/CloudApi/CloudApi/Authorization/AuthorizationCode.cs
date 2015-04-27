using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Authorization
{
    public static class AuthorizationCode
    {
        /// <summary>
        /// 普通用户通用权限
        /// </summary>
        public const string U_Common = "00";
        /// <summary>
        /// 允许查看项目仪表盘
        /// </summary>
        public const string S_Dashboard = "01";
        /// <summary>
        /// 允许查看DTU基本信息界面
        /// </summary>
        public const string S_DTU = "0101";
        /// <summary>
        /// 允许远程管理DTU
        /// </summary>
        public const string S_DTU_RemoteConfig = "010101";
        /// <summary>
        /// 允许查询DTU余额
        /// </summary>
        public const string S_DTU_BalanceInquiry = "010102";
        /// <summary>
        /// 配置模块菜单(不属实际权限)
        /// </summary>
        public const string S_Config = "02";
        /// <summary>
        /// 允许查看业主单位管理界面
        /// </summary>
        public const string S_Org = "0201";
        /// <summary>
        /// 允许添加新的组织
        /// </summary>
        public const string S_Org_Add = "020101";
        /// <summary>
        /// 允许添加或修改组织的LOGO
        /// </summary>
        public const string S_Org_Logo = "020102";
        /// <summary>
        /// 允许修改和删除组织
        /// </summary>
        public const string S_Org_Modify = "020103";
        /// <summary>
        /// 允许查看结构物管理界面
        /// </summary>
        public const string S_Structure = "0202";
        /// <summary>
        /// 允许查看结构物方案配置界面
        /// </summary>
        public const string S_Structure_Scheme = "020201";
        /// <summary>
        /// 允许修改结构物监测因素
        /// </summary>
        public const string S_Structure_Factor_Modify = "02020101";
        /// <summary>
        /// 允许新增DTU
        /// </summary>
        public const string S_Structure_DTU_Add = "02020102";
        /// <summary>
        /// 允许修改和删除DTU
        /// </summary>
        public const string S_Structure_DTU_Modify = "02020103";
        /// <summary>
        /// 允许增加、修改和删除传感器
        /// </summary>
        public const string S_Structure_Sensor_Modify = "02020104";
        /// <summary>
        /// 允许增加、修改和删除传感器组
        /// </summary>
        public const string S_Structure_SensorGroup_Modify = "02020105";
        /// <summary>
        /// 允许修改阈值配置
        /// </summary>
        public const string S_Structure_Threshold = "02020106";
        /// <summary>
        /// 允许修改数据验证配置
        /// </summary>
        public const string S_Structure_DatavalRESOURCE_ID = "02020107";
        /// <summary>
        /// 允许修改监测因素展示单位
        /// </summary>
        public const string S_Structure_FactorUnit = "02020108";
        /// <summary>
        /// 允许查看结构物热点图配置界面
        /// </summary>
        public const string S_Structure_Topo = "020202";
        /// <summary>
        /// 允许上传结构物热点图
        /// </summary>
        public const string S_Org_Logo_Upload = "02020201";
        /// <summary>
        /// 允许对结构物热点图进行布点
        /// </summary>
        public const string S_Org_Logo_Layout = "02020202";
        /// <summary>
        /// 允许查看施工线路/截面配置管理界面
        /// </summary>
        public const string S_Structure_Construct = "020203";
        /// <summary>
        /// 允许增加施工路线
        /// </summary>
        public const string S_Structure_Construct_Route_Add = "02020301";
        /// <summary>
        /// 允许修改和删除施工路线
        /// </summary>
        public const string S_Structure_Construct_Route_Modify = "02020302";
        /// <summary>
        /// 允许增加施工截面
        /// </summary>
        public const string S_Structure_Construct_Section_Add = "02020303";
        /// <summary>
        /// 允许修改和删除施工截面
        /// </summary>
        public const string S_Structure_Construct_Section_Modify = "02020304";
        /// <summary>
        /// 允许增加结构物
        /// </summary>
        public const string S_Structure_Add = "020204";
        /// <summary>
        /// 允许修改和删除结构物
        /// </summary>
        public const string S_Structure_Modify = "020205";
        /// <summary>
        /// 允许查看结构物权重配置界面
        /// </summary>
        public const string S_Weight = "0203";
        /// <summary>
        /// 允许对结构物权重进行配置
        /// </summary>
        public const string S_Weight_Config = "020301";
        /// <summary>
        /// 允许查看用户管理界面
        /// </summary>
        public const string S_User = "0204";
        /// <summary>
        /// 允许添加用户
        /// </summary>
        public const string S_User_Add = "020401";
        /// <summary>
        /// 允许修改用户信息
        /// </summary>
        public const string S_User_Update = "020402";
        /// <summary>
        /// 允许删除用户
        /// </summary>
        public const string S_User_Delete = "020403";
        /// <summary>
        /// 项目监控菜单(不属实际权限)
        /// </summary>
        public const string S_Monitor = "03";
        /// <summary>
        /// 允许查看告警管理界面
        /// </summary>
        public const string S_Warn = "0301";
        /// <summary>
        /// 允许确认告警
        /// </summary>
        public const string S_Warn_Confirm = "030101";
        /// <summary>
        /// 允许下发告警
        /// </summary>
        public const string S_Warn_Post = "030102";
        /// <summary>
        /// 数据服务菜单(不属实际权限)
        /// </summary>
        public const string S_Service = "04";
        /// <summary>
        /// 允许查看报表配置界面
        /// </summary>
        public const string S_Report = "0401";
        /// <summary>
        /// 允许新增报表配置
        /// </summary>
        public const string S_Report_Config_Add = "040101";
        /// <summary>
        /// 允许修改报表配置
        /// </summary>
        public const string S_Report_Config_Update = "040102";
        /// <summary>
        /// 允许删除报表配置
        /// </summary>
        public const string S_Report_Config_Delete = "040103";
        /// <summary>
        /// 允许查看报表管理界面和下载报表
        /// </summary>
        public const string S_Report_Manage = "0402";
        /// <summary>
        /// 允许对报表进行重命名操作
        /// </summary>
        public const string S_Report_Manage_Rename = "040201";
        /// <summary>
        /// 允许上传报表
        /// </summary>
        public const string S_Report_Manage_Upload = "040202";
        /// <summary>
        /// 允许删除报表
        /// </summary>
        public const string S_Report_Manage_Delete = "040203";
        /// <summary>
        /// 允许人工上传报表
        /// </summary>
        public const string S_Report_Manage_ManualUpload = "040204";
        /// <summary>
        /// 允许查看原始数据
        /// </summary>
        public const string S_DataOriginal = "0403";
        /// <summary>
        /// 允许查看数据对比界面
        /// </summary>
        public const string S_DataCompare = "0404";
        /// <summary>
        /// 允许查看数据关联界面
        /// </summary>
        public const string S_DataCorrelation = "0405";
        /// <summary>
        /// 允许查看即时采集界面
        /// </summary>
        public const string S_InstantCollect = "0406";
        /// <summary>
        /// 允许下发及时采集命令
        /// </summary>
        public const string S_InstantCollect_Issue = "040601";
        /// <summary>
        /// 日志管理菜单(不属实际权限)
        /// </summary>
        public const string S_Log = "05";
        /// <summary>
        /// 允许查看用户日志界面
        /// </summary>
        public const string S_UserLog = "0501";
        /// <summary>
        /// 允许查看系统日志界面
        /// </summary>
        public const string S_SysLog = "0502";
        /// <summary>
        /// 允许使用数据服务接口
        /// </summary>
        public const string U_DataService = "06";

    }
}