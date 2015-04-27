/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：StructureMonthlyReportTemplateHandler.cs
// 功能描述：
// 
// 创建标识： 2014/10/21 16:19:04
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
using ReportGeneratorService.Interface;
using log4net;
using System.Reflection;
using System.IO;
using NPOI.XWPF.UserModel;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModule;

namespace ReportGeneratorService.TemplateHandle
{
    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.Interface;
    using ReportGeneratorService.ReportModule;

    public class StructureMonthlyReportTemplateHandler : TemplateHandleBase
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public StructureMonthlyReportTemplateHandler(TemplateHandlerPrams para)
            : base(para)
        {

        }

        public override void WriteFile()
        {
            if (!File.Exists(base.TemplateHandlerPrams.TemplateFileName))
            {
                logger.Warn("模版文件 : " + base.TemplateHandlerPrams.TemplateFileName + "未找到");
                throw new FileNotFoundException("模版文件 : " + base.TemplateHandlerPrams.TemplateFileName);
            }
         logger.Debug("开始拷贝模板");
            // 读取模版
            XWPFDocument template;
            using (var fs = new FileStream(base.TemplateHandlerPrams.TemplateFileName, FileMode.Open, FileAccess.Read))
            {
                template = new XWPFDocument(fs);
                fs.Close();
            }
        logger.Debug("拷贝模板结束");
            //获取word中要填充的内容
      logger.Debug("获取报表基本信息");
            DataSourceStartEndTime dataSourceStartEndTime = GetDataStartEndTime(base.TemplateHandlerPrams.Date);
            var ReportDate = dataSourceStartEndTime.ReportDate;
            DateTime end = dataSourceStartEndTime.EndTime;
            DateTime start = dataSourceStartEndTime.StarTime;

            var structId = base.TemplateHandlerPrams.Structure.Id;
            var structName = base.TemplateHandlerPrams.Structure.Name;
            var proName = base.TemplateHandlerPrams.Organization.SystemName;
            var orgName = base.TemplateHandlerPrams.Organization.Name;
            logger.Debug("获取基本信息结束");

            logger.Debug("开始获取监测因素");
            //获取结构物下的监测因素列表
            var factorList = DataAccess.FindFactorsByStruct(structId);
            var ArrayFactorId = new List<int>();
            foreach (var factor in factorList)
            {
                foreach (var child in factor.Children)
                {
                    ArrayFactorId.Add(child.Id);

                }
            }
            logger.Debug("获取监测因素结束");

            logger.Debug("开始获取监测因素对应的图片流");

            //获取结构物下各个监测因素对应的图片流
            List<ChartByFactor> structStream = new List<ChartByFactor>();
            foreach (var i in ArrayFactorId)
            {
                //遍历获取结构物下每个监测因素编号、名称、对应的图片流
                var chart_factor_temp = new ChartByFactor();
                var getChart = new DrawHighcharts();
                chart_factor_temp.factorId = i;
                chart_factor_temp.factorName = DataAccess.GetFactorInfoById(i).NameCN;
                logger.Debug(string.Format("开始监测因素....{0}.....对应的图片流", chart_factor_temp.factorName));
                //3-13
                chart_factor_temp.ChartStreams = getChart.Draw(structId, i, start, end,"month");
                logger.Debug(string.Format("结束获取监测因素....{0}.....对应的图片流", chart_factor_temp.factorName));

                //将监测因素的图片流相关信息存到结构物图片流列表中
                structStream.Add(chart_factor_temp);
            }
            logger.Debug("获取监测因素对应的图片流结束");


            logger.Debug("开始替换word特殊标记");
            const string markerString = "$Pro,$Str,$Date,$Org,$Factors,$N,$FacDes,$Chart,$Topo,$Type,$Des";
            var markerList = markerString.Split(',').ToList();
            var markerHandler = new MarkerHandler();
            XWPFDocument currentTemplate = template;
            XWPFDocument lastTemplate = template;
            foreach (var marker in markerList)
            {
                switch (marker)
                {
                    case "$Pro":
                        currentTemplate = markerHandler.ProjectNameHandler(lastTemplate, proName);
                        lastTemplate = currentTemplate;
                        break;
                    case "$Str":
                        currentTemplate = markerHandler.StructNameHandler(lastTemplate, structName);
                        lastTemplate = currentTemplate;
                        break;
                    case "$Date":
                        currentTemplate = markerHandler.ReportDateHandler(lastTemplate, ReportDate);
                        lastTemplate = currentTemplate;
                        break;
                    case "$Org":
                        currentTemplate = markerHandler.OrgNameHandler(lastTemplate, orgName);
                        lastTemplate = currentTemplate;
                        break;
                    case "$Factors":
                        currentTemplate = markerHandler.FactorListHandler(lastTemplate, ArrayFactorId);
                        lastTemplate = currentTemplate;
                        break;
                    case "$N":
                        currentTemplate = markerHandler.FactorsNumHandler(lastTemplate, ArrayFactorId);
                        lastTemplate = currentTemplate;
                        break;
                    case "$FacDes":
                        currentTemplate = markerHandler.FactorDescriptionHandler(lastTemplate, ArrayFactorId);
                        lastTemplate = currentTemplate;
                        break;
                    case "$Chart":
                        currentTemplate = markerHandler.ChartHandler(lastTemplate, structStream);
                        lastTemplate = currentTemplate;
                        break;
                    case "$Topo":
                        break;
                    case "$Type":
                        currentTemplate = markerHandler.TypeNameHandler(lastTemplate, "月");
                        lastTemplate = currentTemplate;
                        break;
                    case "$Des":
                        currentTemplate = markerHandler.TypeDescHandler(lastTemplate,"月报");
                        lastTemplate = currentTemplate;
                        break;
                }
            }
            logger.Debug("结束替换word特殊标记");
            logger.Debug("开始保存报表文件");
            var path = base.TemplateHandlerPrams.FileFullName;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    currentTemplate.Write(fs);
                }
                logger.Info(string.Format("报表生成成功,路径:{0}", path));
            }
            catch (Exception e)
            {
                logger.Warn(string.Format("报表生成失败:{0}\n", path), e);
                throw e;
            }
        }

        /// <summary>
        /// 获取报表监测数据的起始时间及监测报告日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataSourceStartEndTime GetDataStartEndTime(DateTime date)
        {
            var dateTime = date.AddMonths(-1);
            string reportDate = dateTime.Year.ToString() + "年" + dateTime.Month.ToString() + "月";
            var startDate = dateTime.Year.ToString() + "-" + dateTime.Month.ToString() + "-" + "01" + " 00:00:00";
            var endDate = date.Year.ToString() + "-" + date.Month.ToString() + "-" + "01" + " 00:00:00";
            DateTime endTime = Convert.ToDateTime(endDate);
            DateTime startTime = Convert.ToDateTime(startDate);
            var dataSourceStartEndTime = new DataSourceStartEndTime
            {
                EndTime = endTime,
                StarTime = startTime,
                ReportDate = reportDate
            };

            return dataSourceStartEndTime;

        }
     
     

    }

    public class DataSourceStartEndTime
    {
        public DateTime StarTime { set; get; }
        public DateTime EndTime { set; get; }
        public string ReportDate { set; get; }
    }
}
