using System.Configuration;
using System.IO;
using NPOI.XWPF.UserModel;
/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：MarkerHandler.cs
// 功能描述：
// 
// 创建标识： 2014/10/22 11:48:57
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
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Dal;

namespace ReportGeneratorService.TemplateHandle
{
    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;

    /// <summary>
    /// word模板中的特殊字符替换处理
    /// </summary>
    public class MarkerHandler
    {
        private Dictionary<string, int> chartType = new Dictionary<string, int>
        {
            {"General", 1},
            {"CexieLeiji", 2}
        };
     
        /// <summary>
        /// 将数据趋势图插入到word文档指定位置
        /// </summary>
        /// <param name="templateFile">模板</param>
        /// <param name="structStream">结构物下各个监测因素对应的图片流集合</param>
        /// <returns></returns>
        public XWPFDocument ChartHandler(XWPFDocument templateFile, List<ChartByFactor> structStream)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Chart")
                    {
                        run.SetText("", 0);
                        run.AddCarriageReturn();
                        XWPFRun chartRunPoint = run.Paragraph.CreateRun();
                        //XWPFRun chartRunPoint = run;
                        PutChartToWord(template, chartRunPoint, structStream);
                    }
                }
            }
            return template;
        }

        /// <summary>
        /// 将数据趋势图插入到word文档的具体实现方法
        /// </summary>
        /// <param name="template">模板</param>
        /// <param name="startPointRun">图片插入的起始位置</param>
        /// <param name="structStream">结构物下各个监测因素对应的图片流集合</param>
        public void PutChartToWord(XWPFDocument template, XWPFRun startPointRun, List<ChartByFactor> structStream)
        {
            XWPFRun run = startPointRun;
            if (structStream == null || structStream.Count == 0)
            {
                //XWPFParagraph gp = run.Paragraph;
                //XWPFRun gr = gp.CreateRun();
                XWPFRun gr = run;
                gr.SetText("当前结构物此段时间无监测因素对应的监测数据");
                gr.AddCarriageReturn();
                return;
            }
            XWPFRun lastRunPoint = run;
           
            for (int i = 0; i < structStream.Count; i++)
            {
                var factorStream = structStream[i];//监测因素对应的图片流
                lastRunPoint.Paragraph.SetAlignment(ParagraphAlignment.LEFT);
                //run.SetText("4." + (i + 1) + factorStream.factorName, 0);
                int j = i + 1;
                lastRunPoint.SetText("(" + j + ") " + factorStream.factorName);
                lastRunPoint.AddCarriageReturn();
                //Console.WriteLine("开始插入");
                if (factorStream.ChartStreams == null || factorStream.ChartStreams.Count == 0)
                {
                    XWPFRun gr = lastRunPoint;
                    gr.SetText(factorStream.factorName + "此段时间没有监测数据");
                    gr.AddCarriageReturn();//回车换行
                    lastRunPoint = gr.Paragraph.CreateRun();
                    continue;
                }
                //遍历监测因素下的图片流
                foreach (var sensorStream in factorStream.ChartStreams)
                {
                    XWPFParagraph gp = lastRunPoint.Paragraph;
                    XWPFRun gr = gp.CreateRun();
                    if (!Directory.Exists("Reports/Image"))
                    {
                        Directory.CreateDirectory("Reports/Image");
                    }
                    string picName = Guid.NewGuid() + ".png";
                    string optionFile = "Reports/Image/" + picName;
                    if (sensorStream == null)
                    {
                        continue;
                    }
                    StreamToFile(sensorStream.ChartStream, optionFile);
                    //inline方式插图
                    int ChartTypeId = chartType[sensorStream.ChartType];
                    var GeneralWidth = Convert.ToInt32(ConfigurationManager.AppSettings["GeneralWidth"]);
                    var GeneralHeight = Convert.ToInt32(ConfigurationManager.AppSettings["GeneralHeight"]);
                    var CexieLeijiWidth = Convert.ToInt32(ConfigurationManager.AppSettings["CexieLeijiWidth"]);
                    var CexieLeijiHeight = Convert.ToInt32(ConfigurationManager.AppSettings["CexieLeijiHeight"]);
                    

                    using (FileStream picFileStrem = new FileStream(optionFile, FileMode.Open, FileAccess.Read))
                    {
                        if (ChartTypeId == 2)//测斜累计位移
                        {
                            gr.AddPicture(picFileStrem, (int)PictureType.PNG, picName, CexieLeijiWidth, CexieLeijiHeight);
                        }
                        else
                        {
                            gr.AddPicture(picFileStrem, (int)PictureType.PNG, picName, GeneralWidth, GeneralHeight);
                        }
                        picFileStrem.Close();
                    }
                    File.Delete(optionFile);
                    //Console.WriteLine("结束插入");                    
                    gr.AddCarriageReturn();
                    lastRunPoint = gr;
                }
                if (i < structStream.Count - 1)
                {
                    lastRunPoint.AddCarriageReturn();
                    lastRunPoint = lastRunPoint.Paragraph.CreateRun();
                }
            }

            #region test
            //for (int i = 0; i < structStream.Count; i++)
            //{
            //    var factorStream = structStream[i];//监测因素对应的图片流
            //    run.Paragraph.SetAlignment(ParagraphAlignment.LEFT);
            //    //run.SetText("4." + (i + 1) + factorStream.factorName, 0);
            //    int j = i + 1;
            //    run.SetText("(" + j + ")" + factorStream.factorName, 0);
            //    run.AddCarriageReturn();
            //    //Console.WriteLine("开始插入");
            //    if (factorStream.ChartStreams == null || factorStream.ChartStreams.Count == 0)
            //    {
            //        XWPFParagraph gp = run.Paragraph;
            //        XWPFRun gr = gp.CreateRun();
            //        gr.SetText(factorStream.factorName + "此月没有监测数据", 0);
            //        gr.AddCarriageReturn();
            //        run = gr;
            //        continue;
            //    }
            //    //遍历监测因素下的图片流
            //    foreach (var sensorStream in factorStream.ChartStreams)
            //    {
            //        XWPFParagraph gp = run.Paragraph;
            //        XWPFRun gr = gp.CreateRun();
            //        if (!Directory.Exists("Reports/Image"))
            //        {
            //            Directory.CreateDirectory("Reports/Image");
            //        }
            //        string picName = Guid.NewGuid() + ".png";
            //        string optionFile = "Reports/Image/" + picName;
            //        if (sensorStream == null)
            //        {
            //            continue;
            //        }
            //        StreamToFile(sensorStream, optionFile);
            //        //inline方式插图
            //        using (FileStream picFileStrem = new FileStream(optionFile, FileMode.Open, FileAccess.Read))
            //        {
            //            gr.AddPicture(picFileStrem, (int)PictureType.PNG, picName, 350 * 10000, 250 * 10000);
            //            picFileStrem.Close();
            //        }
            //        File.Delete(optionFile);
            //        //Console.WriteLine("结束插入");                    
            //        gr.AddCarriageReturn();
            //        run = gr;
            //    }
            //    if (i < structStream.Count - 1)
            //    {
            //        run.AddCarriageReturn();
            //        run = run.Paragraph.CreateRun();
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// 将Stream流写入文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName">文件的全名(路径+文件名)</param>
        public void StreamToFile(Stream stream, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(stream);
                sw.Flush();
                stream.Position = 0;
                stream.CopyTo(fs);
                stream.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// 工程名称
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="proName"></param>
        /// <returns></returns>
        public XWPFDocument ProjectNameHandler(XWPFDocument templateFile, string proName)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Pro")
                    {
                        run.SetText(proName, 0);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// 报表类型3-10
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public XWPFDocument TypeNameHandler(XWPFDocument templateFile, string typeName)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Type")
                    {
                        run.SetText(typeName, 0);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// 报表类型描述3-10
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="typeDesc"></param>
        /// <returns></returns>
        public XWPFDocument TypeDescHandler(XWPFDocument templateFile, string typeDesc)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Des")
                    {
                        run.SetText(typeDesc, 0);
                    }
                }
            }

            return template;
        }
        /// <summary>
        /// 结构物名称
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="structName"></param>
        /// <returns></returns>
        public XWPFDocument StructNameHandler(XWPFDocument templateFile, string structName)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Str")
                    {
                        run.SetText(structName, 0);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// 组织机构名称
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public XWPFDocument OrgNameHandler(XWPFDocument templateFile, string orgName)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Org")
                    {
                        run.SetText(orgName, 0);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// 报表监测日期
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="reportDate"></param>
        /// <returns></returns>
        public XWPFDocument ReportDateHandler(XWPFDocument templateFile, string reportDate)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Date")
                    {
                        run.SetText(reportDate, 0);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// 监测项目名称
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="arrayFactorId">监测因素编号集合</param>
        /// <returns></returns>
        public XWPFDocument FactorListHandler(XWPFDocument templateFile, List<int> arrayFactorId)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$Factors")
                    {
                        var ProName = "";
                        for (int id = 0; id < arrayFactorId.Count; id++)
                        {
                            ProName += DataAccess.GetFactorInfoById(arrayFactorId[id]).NameCN;
                            if (id != arrayFactorId.Count - 1)
                            {
                                ProName += '、';
                            }
                        }
                        run.SetText(ProName, 0);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// 监测项目数目
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="arrayFactorId"></param>
        /// <returns></returns>
        public XWPFDocument FactorsNumHandler(XWPFDocument templateFile, List<int> arrayFactorId)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$N")
                    {
                        string num = arrayFactorId.Count.ToString();
                        run.SetText(num, 0);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// 监测因素描述
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="arrayFactorId"></param>
        /// <returns></returns>
        public XWPFDocument FactorDescriptionHandler(XWPFDocument templateFile, List<int> arrayFactorId)
        {
            XWPFDocument template = templateFile;
            for (int i = 0; i < template.Paragraphs.Count; i++)
            {
                var runs = template.Paragraphs[i].Runs;
                for (int j = 0; j < runs.Count; j++)
                {
                    var run = runs[j];
                    string text = run.GetText(0);
                    if (text == "$FacDes")
                    {
                        run.SetText("", 0);
                        run.AddCarriageReturn();
                        XWPFParagraph gp = run.Paragraph;
                        XWPFRun gr = gp.CreateRun();
                        for (int id = 0; id < arrayFactorId.Count; id++)
                        {
                            XWPFRun tempRun = gr;
                            var FacName = "(";
                            FacName += id + 1 + ")  ";
                            FacName += DataAccess.GetFactorInfoById(arrayFactorId[id]).NameCN + ": ";
                            tempRun.SetText(FacName, 0);
                            if (id < arrayFactorId.Count - 1)
                            {
                                tempRun.AddCarriageReturn();
                                gr = tempRun.Paragraph.CreateRun();
                            }
                            
                        }
                    }
                }
            }

            return template;
        }

    }
}
