/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：TemplateHandleFactory.cs
// 功能描述：
// 
// 创建标识： 2014/10/21 15:38:01
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReportGeneratorService.ReportModule
{
    using ReportGeneratorService.TemplateHandle;

    using ReportGeneratorService.Interface;

    class TemplateHandleFactory
    {
        public static TemplateHandleBase Create(string handleName,TemplateHandlerPrams para)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();//获取包含当前执行的代码的程序集

            string className = "ReportGeneratorService.TemplateHandle." + handleName;
            //                   + config.TemplateFileName.Substring(0, config.TemplateFileName.IndexOf("."));
            TemplateHandleBase handle =
                        (TemplateHandleBase)
                        assembly.CreateInstance(
                            className,
                            true,
                            BindingFlags.Default,
                            null,
                            new object[]{para},
                            null,
                            null);
            return handle;
        }
    }
}
