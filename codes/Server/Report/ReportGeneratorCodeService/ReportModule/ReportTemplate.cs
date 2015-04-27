// // --------------------------------------------------------------------------------------------
// // <copyright file="ReportTemplate.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace ReportGeneratorService.ReportModule
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    public class ReportTemplate
    {
        private static string FilePath = System.AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["templatePath"];
        public string Name { get; set; }

        public string FullName 
        {
            get
            {
                return Path.Combine(FilePath, Name); 
            }
        }

        public string HandleName { get; set; }

        //public string Para1 { get; set; }
        //public string Para2 { get; set; }
        //public string Para3 { get; set; }
        //public string Para4 { get; set; }

        public int? FactorId { get; set; }

        public IList<string> Para { get; set; }
    }
}