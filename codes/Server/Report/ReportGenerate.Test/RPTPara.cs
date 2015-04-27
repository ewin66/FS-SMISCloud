/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：RPTPara.cs
// 功能描述：
// 
// 创建标识： 2014/12/5 9:19:04
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
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ReportGenerate.Test
{
    public class RPTPara
    {
        private static string TemplateFilePath = ConfigurationManager.AppSettings["TemplatePathForTest"];

        public static string GetTemplateFilePath()
        {
            return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,TemplateFilePath);
        }
    }
}
