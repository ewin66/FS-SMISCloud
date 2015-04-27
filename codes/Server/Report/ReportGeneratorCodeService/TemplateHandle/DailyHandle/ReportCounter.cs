/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：ReportCounter.cs
// 功能描述：
// 
// 创建标识： 2015/3/4 18:38:14
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using log4net;
using log4net.Repository.Hierarchy;

namespace ReportGeneratorService.TemplateHandle
{
    public class ReportCounter
    {
        private static ConcurrentDictionary<string, int> _counters = new ConcurrentDictionary<string, int>();
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private const string fileName = "Count.xml";
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void tryInit()
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("root"));
                    xml.Save(fileName);
                }
                using (_lock)
                {
                    lock (fileName)
                    {
                        var doc = XDocument.Load(fileName);
                        if (doc == null)
                        {
                            File.Delete(fileName);
                            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("root"));
                            xml.Save(fileName);
                        }
                        else
                        {
                            var root = doc.Element("root");
                            if (root != null)
                            {
                                foreach (XElement n in root.Elements("rc"))
                                {
                                    if (n.Attribute("id") != null)
                                    {
                                        string k = n.Attribute("id").Value;
                                        if (n.Attribute("value") != null)
                                        {
                                            string v = n.Attribute("value").Value;
                                            _counters[k] = Convert.ToInt32(v);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Warn("read xml  error: " + e.Message);
                throw e;
            }
        }

        public static int Get(string key)
        {
            tryInit();
            if (_counters.ContainsKey(key))
            {
                return _counters[key];
            }
            return 0;
        }

        public static void Inc(string key)
        {
            if (_counters.ContainsKey(key))
            {
                _counters[key] += 1;
            }
            else
            {
                _counters[key] = 1;
            }
            trySave();
        }


        private static void trySave()
        {
            try
            {
                using (_lock)
                {
                    var doc = new XDocument();
                    var root = new XElement("root");
                    doc.Add(root);
                    foreach (var k in _counters.Keys)
                    {
                        var ei = new XElement("rc");
                        root.Add(ei);
                        ei.SetAttributeValue("id", k);
                        ei.SetAttributeValue("value", Convert.ToString(_counters[k]));
                    }
                    lock (fileName)
                    {
                        using (XmlWriter w = new XmlTextWriter(fileName, Encoding.UTF8))
                        {
                            doc.WriteTo(w);
                            w.Flush();
                            w.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Warn("write xml  error: " + e.Message);
                throw e;
            }
        }
    }
}
