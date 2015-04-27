#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataBaseName.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140527 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model.Config
{
    using SqliteORM;

    [Table("DataBaseName")]
    public class DataBaseName : TableBase<DataBaseName>
    {
        /// <summary>
        /// 提取目标数据库ID
        /// </summary>
        [PrimaryKey(Name = "ID")]
        public long ID { get; set; }

        /// <summary>
        /// 目标数据库名
        /// </summary>
        [Field(Name = "DataBaseCode")]
         public string DataBaseCode { get; set; }

        /// <summary>
        /// 目标数据库路径或目标数据库服务器IP
        /// </summary>
        [Field(Name = "Location")]
        public string Location { get; set; }

        /// <summary>
        /// 目标数据库类型
        /// </summary>
        [Field(Name = "DataBaseType")]
        public int DataBaseType { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Field(Name = "UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Field(Name = "Password")]
        public string Password { get; set; }
    }
}