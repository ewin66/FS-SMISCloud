 // --------------------------------------------------------------------------------------------
 // <copyright file="TemplateHandleBase.cs" company="江苏飞尚安全监测咨询有限公司">
 // Copyright (C) 2014 飞尚科技
 // 版权所有。 
 // </copyright>
 // <summary>
 // 文件功能描述：
 //
 // 创建标识：xusuwei 20141020
 //
 // 修改标识：
 // 修改描述：
 //
 // 修改标识：
 // 修改描述：
 // </summary>
 // ---------------------------------------------------------------------------------------------

using System.IO;

namespace ReportGeneratorService.Interface
{
    using System;

    using ReportGeneratorService.ReportModule;

    public abstract class TemplateHandleBase
    {
        public TemplateHandleBase(TemplateHandlerPrams para)
        {
            this.TemplateHandlerPrams = para;
        }

        //public delegate bool Write();

        /// <summary>
        /// 模板处理类要用到的参数统一放在TemplateHandlerPramsConfig类中
        /// </summary>
        public TemplateHandlerPrams TemplateHandlerPrams { get; set; }

        /// <summary>
        /// 写文件
        /// </summary>
        public abstract void WriteFile();
    }
}