// // --------------------------------------------------------------------------------------------
// // <copyright file="ReportGroup.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141023
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace ReportGeneratorService.DataModule
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using ReportGeneratorService.ReportModule;

    public class ReportGroup : MarshalByRefObject
    {
        public ReportConfig Config { get; set; }

        public IList<ReportTemplate> Templates { get; set; }

        public DateTime CreateDate { get; set; }

        public string ExtName {
            get
            {
                if (Templates.Count > 0)
                {
                    return Path.GetExtension(Templates[0].Name);
                }

                return string.Empty;
            }
        }

        public ReportGroup()
        {
            Config = new ReportConfig();
            Templates = new List<ReportTemplate>();
        }
    }
}