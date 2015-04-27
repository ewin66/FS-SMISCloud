// // --------------------------------------------------------------------------------------------
// // <copyright file="TaskContext.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141022
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace ReportGeneratorService
{
    using System.Collections.Generic;

    using ReportGeneratorService.DataModule;

    public class TaskContext
    {
        public List<ReportGroup> ReportHandles { get; set; }
    }
}