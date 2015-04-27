/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：Report.cs
// 功能描述：
// 
// 创建标识： 2014/10/21 13:27:33
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
using log4net;
using System.Reflection;
using System.Configuration;

namespace ReportGeneratorService.ReportModule
{

    public abstract class Report
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Property

        /// <summary>
        /// 报表编号
        /// </summary>

        public virtual string Id { get; private set; }

        /// <summary>
        /// 报表名称
        /// </summary>
        public virtual string Name { get; private set; }

        /// <summary>
        /// 拓展名
        /// </summary>
        public abstract string ExtName { get; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 模版
        /// </summary>
        public string TemplateFileName { get; set; }//需要修改数据库模板配置里面的模板名称

        /// <summary>
        /// 时间类型
        /// </summary>
        public DateType DateType { get; set; }

        /// <summary>
        /// 完成度
        /// </summary>
        public virtual char Status { get; set; }

        #endregion

        #region ctor

       /// <summary>
       /// 创建报表
       /// </summary>
       /// <param name="dateType"></param>
       /// <param name="date"></param>
       /// <param name="templateFileName"></param>
       /// <param name="name"></param>
        protected Report(DateType dateType, DateTime date, string templateFileName, string name)
        {
            this.DateType = dateType;
            this.Date = date;

            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
            this.TemplateFileName = templateFileName;

        }

        #endregion

        #region Method
        /// <summary>
        /// 应用模版
        /// </summary>
        /// <param name="templateFileName"></param>
        public void ApplyTemplate(string templateFileName)
        {
            this.TemplateFileName = templateFileName;
        }

        #endregion
    }

}
