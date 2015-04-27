using ReportGeneratorService.DataModule;
/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：TemplateHandlerPramsConfig.cs
// 功能描述：
// 
// 创建标识： 2014/10/21 17:31:28
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
// </summary>

//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGeneratorService.ReportModule
{
    using ReportGeneratorService.DataModule;

    public class TemplateHandlerPrams: MarshalByRefObject
    {
        /// <summary>
        /// 生成文件完整路径名称
        /// </summary>
        public string FileFullName { get; set; }

        /// <summary>
        /// 模板文件名
        /// </summary>
        public string TemplateFileName { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 特征值参数：Xls：Sheet页名称，Word：标签名称
        /// </summary>
        //public string CharacterPara { get; set; }

        /// <summary>
        /// 组织信息
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// 结构物信息
        /// </summary>
        public Structure Structure { get; set; }


        /// <summary>
        /// 监测因素信息
        /// </summary>
        public Factor Factor { get; set; }

    }
}
