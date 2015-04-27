#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ProjectInfo.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140529 by WIN .
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

    [Table(Name = "ProjectInfo")]
    public class ProjectInfo : TableBase<ProjectInfo>
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey(Name = "ID")]
        public long Id { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [Field(Name = "ProjectName")]
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [Field(Name = "ProjectCode")]
        public int ProjectCode { get; set; }

        /// <summary>
        /// 存放端口、串口、IP等
        /// </summary>
        [Field(Name = "TargetName")]
        public string TargetName { get; set; }

        /// <summary>
        /// 提取粒度
        /// </summary>
        [Field(Name = "IntervalTime")]
        public int IntervalTime { get; set; }
    }
}