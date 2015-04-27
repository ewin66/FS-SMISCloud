#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataBaseType.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140603 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model
{
    public enum DataBaseType
    {
        /// <summary>
        /// SQLite数据库
        /// </summary>
        SQLite,

        /// <summary>
        /// SQLServer数据库
        /// </summary>
        SQLServer,
        
        /// <summary>
        /// ACCESS数据库(07以前)
        /// </summary>
        ACCESSOld,

        /// <summary>
        /// ACCESS数据库(07及更高版本)
        /// </summary>
        ACCESSNew,

        /// <summary>
        /// 振动文本数据
        /// 
        /// </summary>
        Shake,

        /// <summary>
        /// 光栅光纤文本数据
        /// </summary>
        Fiber,

        /// <summary>
        /// 自家振动
        /// </summary>
        Vibration
    }
}